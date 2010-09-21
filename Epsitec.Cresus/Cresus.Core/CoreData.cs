//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.BusinessLogic;


namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData(bool forceDatabaseCreation)
		{
			this.IsReady = false;
			this.ForceDatabaseCreation = forceDatabaseCreation;

			this.dbInfrastructure = new DbInfrastructure ();
			this.dataInfrastructure = new DataLayer.Infrastructure.DataInfrastructure (this.dbInfrastructure);
			this.independentEntityContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Independent Entities");
			this.refIdGeneratorPool = new BusinessLogic.RefIdGeneratorPool (this);
			this.connectionManager = new CoreDataConnectionManager (this);
			this.locker = new CoreDataLocker (this.dataInfrastructure);
			this.businessContextPool =  new BusinessContextPool (this);
		}

		public DataLayer.Infrastructure.DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}

		public bool IsDataContextActive
		{
			get
			{
				return this.activeDataContext != null;
			}
		}

		public DataContext DataContext
		{
			get
			{
				return this.EnsureDataContext (ref this.activeDataContext, "Active");
			}
		}

		public CoreDataLocker DataLocker
		{
			get
			{
				return this.locker;
			}
		}

		public CoreDataConnectionManager ConnectionManager
		{
			get
			{
				return this.connectionManager;
			}
		}

		public BusinessLogic.RefIdGeneratorPool RefIdGeneratorPool
		{
			get
			{
				return this.refIdGeneratorPool;
			}
		}

		public bool IsReady
		{
			get;
			private set;
		}

		public bool ForceDatabaseCreation
		{
			get;
			private set;
		}


		public DataContext GetDataContext(Data.DataLifetimeExpectancy lifetimeExpectancy)
		{
			switch (lifetimeExpectancy)
			{
				case Data.DataLifetimeExpectancy.Stable:
					return this.EnsureDataContext (ref this.stableDataContext, lifetimeExpectancy.ToString ());

				case Data.DataLifetimeExpectancy.Immutable:
					return this.EnsureDataContext (ref this.immutableDataContext, lifetimeExpectancy.ToString ());
			}

			return this.DataContext;
		}

		private DataContext EnsureDataContext(ref DataContext dataContext, string name)
		{
			if (dataContext == null)
			{
				dataContext = this.CreateDataContext (name);
			}
			
			return dataContext;
		}


		public void SetupDatabase()
		{
			if (!this.IsReady)
			{
				System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen == false);
				System.Diagnostics.Debug.Assert (this.activeDataContext == null);

				var  databaseAccess = CoreData.GetDatabaseAccess ();
				bool databaseIsNew  = this.ConnectToDatabase (databaseAccess);

				System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen);
				System.Diagnostics.Debug.Assert (this.activeDataContext == null);

				this.SetupDataContext (this.CreateDataContext ("setup-only"));
				this.SetupDatabase (databaseIsNew || this.ForceDatabaseCreation);
				this.DisposeDataContext (this.activeDataContext);

				System.Diagnostics.Debug.Assert (this.activeDataContext == null);
				System.Diagnostics.Debug.WriteLine ("Database ready");
			}

			this.IsReady = true;
		}

		private void PreserveNavigation(System.Action action)
		{
			var navigator = this.GetActiveDataViewController ().Navigator;

			if (navigator == null)
			{
				action ();
			}
			else
			{
				navigator.PreserveNavigation (action);
			}
		}

		private DataViewController GetActiveDataViewController(CommandContext context = null)
		{
			return CoreApplication.GetController<DataViewController> (context);
		}

		public static void Sort<T>(List<T> list)
			where T : AbstractEntity, new ()
		{
			if (EntityInfo<T>.Implements<IItemRank> ())
			{
				if (list.Count > 0)
				{
					list.Sort ((a, b) => CoreData.CompareItems (a as IItemRank, b as IItemRank));
				}
			}
		}

		private static int CompareItems<T>(T ra, T rb)
			where T : IItemRank
		{
			int valueA = ra.Rank ?? -1;
			int valueB = rb.Rank ?? -1;

			if (valueA < valueB)
			{
				return -1;
			}
			if (valueA > valueB)
			{
				return 1;
			}
			return 0;
		}

		class ItemRankComparer : IComparer<IItemRank>
		{
			#region IComparer<IItemRank> Members

			public int Compare(IItemRank x, IItemRank y)
			{
				int valueA = x.Rank ?? -1;
				int valueB = y.Rank ?? -1;

				if (valueA < valueB)
				{
					return -1;
				}
				if (valueA > valueB)
				{
					return 1;
				}
				return 0;
			}

			#endregion
		}

		private static ItemRankComparer itemRankComparer = new ItemRankComparer ();


		public IEnumerable<T> GetAllEntities<T>(Extraction extraction = Extraction.Default)
			where T : AbstractEntity, new ()
		{
			var repository = this.GetRepository<T> ();
			
			if (repository != null)
			{
				var all = repository.GetAllEntities ();

				if ((extraction & Extraction.IncludeArchives) == 0)
				{
					all = all.Where (x => (x is ILifetime) ? !((ILifetime) x).IsArchive : true);
				}

				if ((extraction & Extraction.Sorted) != 0)
				{
					if (EntityInfo<T>.Implements<IItemRank> ())
					{
						all = all.OrderBy (x => x as IItemRank, CoreData.itemRankComparer);
					}
				}

				return all;
			}
			else
			{
				return new T[0];
			}
		}

		public Repositories.Repository<T> GetRepository<T>()
			where T : AbstractEntity, new ()
		{
			return Resolvers.RepositoryResolver.Resolve (typeof (T), this) as Repositories.Repository<T>;
		}
		
		
		public DataContext CreateDataContext(string name)
		{
			var context = new DataContext (this.dbInfrastructure, true)
			{
				Name = name,
			};

			DataContextPool.Instance.Add (context);

			return context;
		}

		public void DisposeDataContext(DataContext context)
		{
			if (DataContextPool.Instance.Remove (context))
			{
				context.Dispose ();

				if (this.activeDataContext == context)
				{
					this.activeDataContext = null;
				}
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
		}


		public AbstractEntity CreateDummyEntity(Druid entityId)
		{
			return this.independentEntityContext.CreateEmptyEntity (entityId);
		}

		public bool IsDummyEntity(AbstractEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			else
			{
				return entity.GetEntityContext () == this.independentEntityContext;
			}
		}



		#region IDisposable Members


		public void Dispose()
		{
			this.locker.Dispose ();

			if (this.activeDataContext != null)
			{
				this.activeDataContext.Dispose ();
				this.activeDataContext = null;
			}
			
			this.connectionManager.Dispose ();

			if (this.dbInfrastructure.IsConnectionOpen)
			{
				this.dbInfrastructure.Dispose ();
			}
		}


		#endregion
		
		private bool ConnectToDatabase(DbAccess access)
		{
			if (this.ForceDatabaseCreation)
			{
				this.DeleteDatabase (access);
			}

			if (this.dbInfrastructure.AttachToDatabase (access))
			{
				System.Diagnostics.Trace.WriteLine ("Connected to database");

				return false;
			}
			else
			{
				System.Diagnostics.Trace.WriteLine ("Cannot connect to database");

				try
				{
					this.dbInfrastructure.CreateDatabase (access);
				}
				catch (System.Exception ex)
				{
					UI.ShowErrorMessage (
						Res.Strings.Error.CannotConnectToLocalDatabase,
						Res.Strings.Hint.Error.CannotConnectToLocalDatabase, ex);

					System.Environment.Exit (0);
				}

				System.Diagnostics.Trace.WriteLine ("Created new database");
				
				return true;
			}
		}

		private void DeleteDatabase(DbAccess access)
		{
			string path = DbFactory.GetDatabaseFilePaths (access).First ();

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (path);
					System.Console.Out.WriteLine ("Finally succeeded");
				}
				catch
				{
					System.Console.Out.WriteLine ("Failed again, giving up");
					throw;
				}
			}
		}

		private void SetupDatabase(bool createNewDatabase)
		{
			if (createNewDatabase)
			{
				this.CreateDatabaseSchemas ();
				this.PopulateDatabase ();
			}
			else
			{
				this.VerifyDatabaseSchemas ();
				this.ReloadDatabase ();
			}

			this.ValidateConnection ();
			this.VerifyUidGenerators ();
		}

		private void ValidateConnection()
		{
			this.connectionManager.Validate ();
			this.locker.Validate ();
		}

		private void VerifyUidGenerators()
		{
			this.refIdGeneratorPool.GetGenerator<RelationEntity> ();
			this.refIdGeneratorPool.GetGenerator<AffairEntity> ();
			this.refIdGeneratorPool.GetGenerator<ArticleDefinitionEntity> ();
		}

		private void VerifyDatabaseSchemas()
		{
			// TODO
		}

		private void CreateDatabaseSchemas()
		{
			var dataContext = this.DataContext;

			dataContext.CreateSchema<RelationEntity> ();
			dataContext.CreateSchema<NaturalPersonEntity> ();
			dataContext.CreateSchema<AbstractPersonEntity> ();
			dataContext.CreateSchema<MailContactEntity> ();
			dataContext.CreateSchema<TelecomContactEntity> ();
			dataContext.CreateSchema<UriContactEntity> ();
			dataContext.CreateSchema<ArticleDefinitionEntity> ();
			dataContext.CreateSchema<VatDefinitionEntity> ();
			dataContext.CreateSchema<InvoiceDocumentEntity> ();

			dataContext.CreateSchema<ArticleDocumentItemEntity> ();
			dataContext.CreateSchema<TextDocumentItemEntity> ();
			dataContext.CreateSchema<PriceDocumentItemEntity> ();
			dataContext.CreateSchema<TaxDocumentItemEntity> ();

			dataContext.CreateSchema<EnumValueArticleParameterDefinitionEntity> ();
			dataContext.CreateSchema<NumericValueArticleParameterDefinitionEntity> ();
			
			dataContext.CreateSchema<PaymentDetailEventEntity> ();
			dataContext.CreateSchema<TotalDocumentItemEntity> ();

			dataContext.CreateSchema<SoftwareUserEntity> ();
		}

		private void PopulateDatabase()
		{
			this.PopulateDatabaseHack ();
			this.PopulateUsers ();
		}

		private void PopulateUsers()
		{
			var logicUser  = new BusinessLogic.Logic (typeof (SoftwareUserEntity), null);
			var logicGroup = new BusinessLogic.Logic (typeof (SoftwareUserGroupEntity), null);

			var groupSystem   = this.CreateUserGroup (logicGroup, "Système", Business.UserManagement.UserPowerLevel.System);
			var groupDev      = this.CreateUserGroup (logicGroup, "Développeurs", Business.UserManagement.UserPowerLevel.Developer);
			var groupAdmin    = this.CreateUserGroup (logicGroup, "Administrateurs", Business.UserManagement.UserPowerLevel.Administrator);
			var groupStandard = this.CreateUserGroup (logicGroup, "Utilisateurs", Business.UserManagement.UserPowerLevel.Standard);

			var userStandard = this.CreateUser (logicUser, groupStandard, "Utilisateur par défaut");
			var userDev = this.CreateUser (logicUser, groupDev, "Epsitec", "Epsitec", "admin");

			this.DataContext.SaveChanges ();
		}

		private SoftwareUserGroupEntity CreateUserGroup(Logic logicGroup, string name, Business.UserManagement.UserPowerLevel level)
		{
			var group = this.DataContext.CreateEntity<SoftwareUserGroupEntity> ();
			
			logicGroup.ApplyRules (RuleType.Setup, group);
			
			group.Name           = name;
			group.UserPowerLevel = level;
			
			return group;
		}

		private SoftwareUserEntity CreateUser(Logic logicUser, SoftwareUserGroupEntity group, FormattedText displayName, string userLogin = null, string userPassword = null)
		{
			var user = this.DataContext.CreateEntity<SoftwareUserEntity> ();

			logicUser.ApplyRules (RuleType.Setup, user);
			
			user.AuthenticationMethod = Business.UserManagement.UserAuthenticationMethod.System;
			user.DisplayName = displayName;
			user.LoginName = userLogin ?? System.Environment.UserName;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);
			
			return user;
		}

		private void ReloadDatabase()
		{
			// TODO
		}

		public void SetupDataContext(DataContext dataContext)
		{
			var oldContext = this.activeDataContext;
			this.activeDataContext = dataContext;
		}

		private static DbAccess GetDatabaseAccess()
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			return access;
		}



		internal string GetNewAffairId()
		{
			var repo = new Epsitec.Cresus.Core.Repositories.AffairRepository (this);

			return (repo.GetAllEntities ().Select (x => CoreData.RobustParseNumber (x.IdA)).OrderByDescending (n => n).FirstOrDefault () + 1).ToString ();
		}


		public static int RobustParseNumber(string value)
		{
			int result;
			int.TryParse (value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out result);
			return result;
		}
		
		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
		private readonly EntityContext independentEntityContext;
		private readonly BusinessLogic.RefIdGeneratorPool refIdGeneratorPool;
		private readonly CoreDataConnectionManager connectionManager;
		private readonly BusinessContextPool businessContextPool;
		private readonly CoreDataLocker locker;

		private DataContext immutableDataContext;
		private DataContext stableDataContext;

		private DataContext activeDataContext;
		private int dataContextChangedLevel;

		public BusinessLogic.BusinessContext CreateBusinessContext()
		{
			return this.businessContextPool.CreateBusinessContext ();
		}
	}

	[System.Flags]
	public enum Extraction
	{
		Default = 0,
		
		Sorted			= 0x0001,

		IncludeArchives = 0x00010000,
	}
}