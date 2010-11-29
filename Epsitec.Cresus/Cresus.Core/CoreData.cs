//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;
using System.Linq;


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
			this.refIdGeneratorPool = new RefIdGeneratorPool (this);
			this.connectionManager = new CoreDataConnectionManager (this);
			this.locker = new DataLocker (this.dataInfrastructure);
			this.businessContextPool =  new BusinessContextPool (this);
			this.imageDataStore = new ImageDataStore (this);
		}

		public DataInfrastructure				DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}

		public bool								IsDataContextActive
		{
			get
			{
				return this.activeDataContext != null;
			}
		}

		public DataContext						DataContext
		{
			get
			{
				return this.EnsureDataContext (ref this.activeDataContext, "Active");
			}
		}

		public DataContextPool					DataContextPool
		{
			get
			{
				return this.DataInfrastructure.DataContextPool;
			}
		}

		public DataLocker					DataLocker
		{
			get
			{
				return this.locker;
			}
		}

		public CoreDataConnectionManager		ConnectionManager
		{
			get
			{
				return this.connectionManager;
			}
		}

		public RefIdGeneratorPool				RefIdGeneratorPool
		{
			get
			{
				return this.refIdGeneratorPool;
			}
		}

		public bool								IsReady
		{
			get;
			private set;
		}

		public bool								ForceDatabaseCreation
		{
			get;
			private set;
		}

		public ImageDataStore					ImageDataStore
		{
			get
			{
				return this.imageDataStore;
			}
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

				var databaseAccess = CoreData.GetDatabaseAccess ();
				
				try
				{
					bool databaseIsNew  = this.ConnectToDatabase (databaseAccess);

					System.Diagnostics.Debug.Assert (this.dbInfrastructure.IsConnectionOpen);
					System.Diagnostics.Debug.Assert (this.activeDataContext == null);

					this.SetupDataContext (this.CreateDataContext ("setup-only"));
					this.SetupDatabase (createNewDatabase: databaseIsNew || this.ForceDatabaseCreation);
					this.DisposeDataContext (this.activeDataContext);

					System.Diagnostics.Debug.Assert (this.activeDataContext == null);
					System.Diagnostics.Debug.WriteLine ("Database ready");
				}
				catch (Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException ex)
				{
					System.Diagnostics.Trace.WriteLine ("Failed to connect to database: " + ex.Message + "\n\n" + ex.StackTrace);

					UI.ShowErrorMessage (
						Res.Strings.Error.IncompatibleDatabase,
						Res.Strings.Hint.Error.IncompatibleDatabase, ex);

					this.dbInfrastructure.Dispose ();
					this.DeleteDatabase (databaseAccess);

					//	TODO: start external migration process...

					System.Environment.Exit (0);
				}
				catch (System.Exception ex)
				{
					UI.ShowErrorMessage (
						Res.Strings.Error.CannotConnectToLocalDatabase,
						Res.Strings.Hint.Error.CannotConnectToLocalDatabase, ex);

					System.Environment.Exit (0);
				}
			}

			this.IsReady = true;
		}

		public void SetupBusiness()
		{
			TaxContext.Initialize (this);
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

		/// <summary>
		/// Finds the (normalized) entity key for the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The (normalized) entity key or <c>null</c> if it cannot be found in the database.</returns>
		public static EntityKey? FindEntityKey(AbstractEntity entity)
		{
			return CoreProgram.Application.Data.DataContextPool.FindEntityKey (entity);
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


		public IEnumerable<T> GetAllEntities<T>(DataExtractionMode extraction = DataExtractionMode.Default, DataContext dataContext = null)
			where T : AbstractEntity, new ()
		{
			var repository = this.GetRepository<T> (dataContext);
			
			if (repository != null)
			{
				var all = repository.GetAllEntities ();

				if ((extraction & DataExtractionMode.IncludeArchives) == 0)
				{
					all = all.Where (x => (x is ILifetime) ? !((ILifetime) x).IsArchive : true);
				}

				if ((extraction & DataExtractionMode.Sorted) != 0)
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

		public Repositories.Repository<T> GetRepository<T>(DataContext dataContext = null)
			where T : AbstractEntity, new ()
		{
			return Resolvers.RepositoryResolver.Resolve (typeof (T), this, dataContext) as Repositories.Repository<T>;
		}
		
		
		public DataContext CreateDataContext(string name)
		{
			var context = this.dataInfrastructure.CreateDataContext (true);
			context.Name = name;

			return context;
		}

		public void DisposeDataContext(DataContext context)
		{
			if (this.dataInfrastructure.ContainsDataContext (context))
			{
				if (this.activeDataContext == context)
				{
					this.activeDataContext = null;
				}

				CoreApplication.QueueAsyncCallback (
					delegate
					{
						this.dataInfrastructure.DeleteDataContext (context);
					});
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
		}

		public BusinessContext CreateBusinessContext()
		{
			return this.businessContextPool.CreateBusinessContext ();
		}

		public void DisposeBusinessContext(BusinessContext context)
		{
			this.businessContextPool.DisposeBusinessContext (context);
		}

		public AbstractEntity CreateDummyEntity(Druid entityId)
		{
			return this.independentEntityContext.CreateEmptyEntity (entityId);
		}

		public void DisposeDummyEntity(AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (this.IsDummyEntity (entity));

			//	Nothing to do -- the entity is not referenced by the entity context; it will
			//	simply be garbage collected.
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

			try
			{
				if (DbInfrastructure.CheckForDatabaseFiles (access))
				{
					this.dbInfrastructure.AttachToDatabase (access);
					System.Diagnostics.Trace.WriteLine ("Connected to database");
					return false;
				}
			}
			catch (Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException)
			{
				throw;
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine ("Failed to connect to database: " + ex.Message + "\n\n" + ex.StackTrace);
			}

			System.Diagnostics.Trace.WriteLine ("Cannot connect to database");
			this.dbInfrastructure.CreateDatabase (access);
			System.Diagnostics.Trace.WriteLine ("Created new database");

			return true;
		}

		private void DeleteDatabase(DbAccess access)
		{
			DbInfrastructure.DeleteDatabaseFiles (access);
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
			this.connectionManager.Validate ();

			foreach (var entityId in this.GetManagedEntityIds ())
			{
				this.VerifyDatabaseSchema (entityId);
			}
		}


		private void VerifyDatabaseSchema(Druid entityId)
		{
			if (!this.dataInfrastructure.CheckSchema (entityId))
			{
				System.Type type = EntityClassFactory.FindType (entityId);
				string typeName = type == null ? entityId.ToString () : type.FullName;

				throw new Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException (string.Format ("Incompatible database schema: schema for {0} or one of its dependencies does not exist or is incorrect", typeName));
			}
		}

		private IEnumerable<Druid> GetManagedEntityIds()
		{
			yield return EntityInfo<RelationEntity>.GetTypeId ();
			yield return EntityInfo<NaturalPersonEntity>.GetTypeId ();
			yield return EntityInfo<LegalPersonEntity>.GetTypeId ();
			yield return EntityInfo<MailContactEntity>.GetTypeId ();
			yield return EntityInfo<TelecomContactEntity>.GetTypeId ();
			yield return EntityInfo<UriContactEntity>.GetTypeId ();

			yield return EntityInfo<VatDefinitionEntity>.GetTypeId ();
			
			yield return EntityInfo<BusinessDocumentEntity>.GetTypeId ();
			yield return EntityInfo<DocumentMetadataEntity>.GetTypeId ();
			yield return EntityInfo<ImageBlobEntity>.GetTypeId ();
			yield return EntityInfo<ImageEntity>.GetTypeId ();
			
			yield return EntityInfo<ArticleDocumentItemEntity>.GetTypeId ();
			yield return EntityInfo<TextDocumentItemEntity>.GetTypeId ();
			yield return EntityInfo<SubTotalDocumentItemEntity>.GetTypeId ();
			yield return EntityInfo<EndTotalDocumentItemEntity>.GetTypeId ();
			yield return EntityInfo<TaxDocumentItemEntity>.GetTypeId ();

			yield return EntityInfo<ArticleDefinitionEntity>.GetTypeId ();
			yield return EntityInfo<EnumValueArticleParameterDefinitionEntity>.GetTypeId ();
			yield return EntityInfo<NumericValueArticleParameterDefinitionEntity>.GetTypeId ();
			yield return EntityInfo<FreeTextValueArticleParameterDefinitionEntity>.GetTypeId ();
			
			yield return EntityInfo<AffairEntity>.GetTypeId ();
			yield return EntityInfo<WorkflowEntity>.GetTypeId ();
			yield return EntityInfo<SoftwareUserEntity>.GetTypeId ();
			yield return EntityInfo<BusinessSettingsEntity>.GetTypeId ();
		}


		private void CreateDatabaseSchemas()
		{
			this.connectionManager.Validate ();

			foreach (var entityId in this.GetManagedEntityIds ())
			{
				this.DataInfrastructure.CreateSchema (entityId);
			}
		}

		private void PopulateDatabase()
		{
			this.connectionManager.Validate ();

			this.PopulateDatabaseHack ();
			this.PopulateUsers ();
		}

		private void PopulateUsers()
		{
			var role = this.DataContext.CreateEntity<SoftwareUserRoleEntity> ();

			role.Code = "?";
			role.Name = "Principal";

			var logicUser  = new Logic (typeof (SoftwareUserEntity), null);
			var logicGroup = new Logic (typeof (SoftwareUserGroupEntity), null);

			var groupSystem     = this.CreateUserGroup (logicGroup, role, "Système",                   Business.UserManagement.UserPowerLevel.System);
			var groupDev        = this.CreateUserGroup (logicGroup, role, "Développeurs",              Business.UserManagement.UserPowerLevel.Developer);
			var groupAdmin      = this.CreateUserGroup (logicGroup, role, "Administrateurs",           Business.UserManagement.UserPowerLevel.Administrator);
			var groupPowerUser  = this.CreateUserGroup (logicGroup, role, "Utilisateurs avec pouvoir", Business.UserManagement.UserPowerLevel.PowerUser);
			var groupStandard   = this.CreateUserGroup (logicGroup, role, "Utilisateurs standards",    Business.UserManagement.UserPowerLevel.Standard);
			var groupRestricted = this.CreateUserGroup (logicGroup, role, "Utilisateurs restreints",   Business.UserManagement.UserPowerLevel.Restricted);

#if false
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 1", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 2", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 3", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 4", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 5", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 6", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 7", Business.UserManagement.UserPowerLevel.Restricted);
			this.CreateUserGroup (logicGroup, "Utilisateurs restreints 8", Business.UserManagement.UserPowerLevel.Restricted);
#endif

			var userStandard1 = this.CreateUser (logicUser, groupDev, "Pierre Arnaud", "arnaud",  "smaky", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard2 = this.CreateUser (logicUser, groupDev, "Marc Bettex",   "Marc",    "tiger", Business.UserManagement.UserAuthenticationMethod.System);
			var userStandard3 = this.CreateUser (logicUser, groupDev, "Daniel Roux",   "Daniel",  "blupi", Business.UserManagement.UserAuthenticationMethod.System);
			var userEpsitec   = this.CreateUser (logicUser, groupDev, "Epsitec",       "Epsitec", "admin", Business.UserManagement.UserAuthenticationMethod.Password);

			userStandard1.UserGroups.Add (groupStandard);
			userStandard2.UserGroups.Add (groupStandard);
			userStandard3.UserGroups.Add (groupStandard);
			userEpsitec.UserGroups.Add (groupAdmin);

			this.DataContext.SaveChanges ();
		}

		private SoftwareUserGroupEntity CreateUserGroup(Logic logicGroup, SoftwareUserRoleEntity role, string name, Business.UserManagement.UserPowerLevel level)
		{
			var group = this.DataContext.CreateEntity<SoftwareUserGroupEntity> ();
			
			logicGroup.ApplyRules (RuleType.Setup, group);

			group.Code           = "?";
			group.Name           = name;
			group.UserPowerLevel = level;
			group.Roles.Add (role);
			
			return group;
		}

		private SoftwareUserEntity CreateUser(Logic logicUser, SoftwareUserGroupEntity group, FormattedText displayName, string userLogin, string userPassword, Business.UserManagement.UserAuthenticationMethod am)
		{
			var user = this.DataContext.CreateEntity<SoftwareUserEntity> ();

			logicUser.ApplyRules (RuleType.Setup, user);

			user.AuthenticationMethod = am;
			user.DisplayName = displayName;
			user.LoginName = userLogin;
			user.UserGroups.Add (group);
			user.SetPassword (userPassword);

			FormattedText[] p = displayName.Split (" ");
			if (p.Length == 2)
			{
				var person = this.SearchNaturalPerson (p[0].ToString (), p[1].ToString ());
				if (person.IsNotNull ())
				{
					user.Person = person;
				}
			}
			
			return user;
		}

		private NaturalPersonEntity SearchNaturalPerson(string firstName, string lastName)
		{
			var example = new NaturalPersonEntity ();
			example.Firstname = firstName;
			example.Lastname = lastName;

			return this.DataContext.GetByExample<NaturalPersonEntity> (example).FirstOrDefault ();
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
		private readonly RefIdGeneratorPool refIdGeneratorPool;
		private readonly CoreDataConnectionManager connectionManager;
		private readonly BusinessContextPool businessContextPool;
		private readonly DataLocker locker;
		private readonly ImageDataStore imageDataStore;

		private DataContext immutableDataContext;
		private DataContext stableDataContext;
		private DataContext activeDataContext;
	}
}