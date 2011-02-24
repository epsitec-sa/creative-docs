//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.ImportExport;

using System.Collections.Generic;
using System.Linq;



namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData : System.IDisposable
	{
		public CoreData(bool forceDatabaseCreation, bool allowDatabaseUpdate)
		{
			this.IsReady = false;
			this.ForceDatabaseCreation = forceDatabaseCreation;
			this.AllowDatabaseUpdate = allowDatabaseUpdate;

			this.components = new Dictionary<string, CoreDataComponent> ();
			this.dbInfrastructure = new DbInfrastructure ();
			this.dataInfrastructure = new DataLayer.Infrastructure.DataInfrastructure (this.dbInfrastructure);
			this.independentEntityContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Independent Entities");

			Factories.CoreDataComponentFactory.RegisterComponents (this);


			this.refIdGeneratorPool = new RefIdGeneratorPool (this);
			this.connectionManager = new ConnectionManager (this);
			this.locker = new Locker (this);
//-			this.businessContextPool =  new BusinessContextPool (this);
//-			this.imageDataStore = new ImageDataStore (this);
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

		public Locker							DataLocker
		{
			get
			{
				return this.locker;
			}
		}

		public ConnectionManager				ConnectionManager
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

		public bool								AllowDatabaseUpdate
		{
			get;
			private set;
		}

#if false
		public ImageDataStore					ImageDataStore
		{
			get
			{
				return this.imageDataStore;
			}
		}
#endif

		public T GetComponent<T>()
			where T : CoreDataComponent
		{
			CoreDataComponent component;
			string componentName = typeof (T).FullName;

			if (this.components.TryGetValue (componentName, out component))
			{
				T result = component as T;

				System.Diagnostics.Debug.Assert (result != null);

				return result;
			}

			throw new System.InvalidOperationException (string.Format ("The component {0} cannot be found", componentName));
		}

		public IEnumerable<CoreDataComponent> GetComponents()
		{
			return this.components.Select (x => x.Value);
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

		internal bool ContainsComponent(string name)
		{
			return this.components.ContainsKey (name);
		}

		internal void RegisterComponent(string name, CoreDataComponent component)
		{
			this.components[name] = component;
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
					// TODO All this part where we try to update the schema of the database if it is
					// incompatible is experimental, and must be checked/modified in order to be of
					// production quality.
					// Marc
					
					System.Diagnostics.Trace.WriteLine ("Failed to connect to database: " + ex.Message + "\n\n" + ex.StackTrace);

					if (this.AllowDatabaseUpdate)
					{
						IDialog d = MessageDialog.CreateYesNo ("Base de donnée incompatible", DialogIcon.Warning, "La base de donnée est incompatible. Voulez vous la modifier pour la mettre à jour?");

						d.OpenDialog ();

						if (d.Result == DialogResult.Yes)
						{
							try
							{
								this.dataInfrastructure.UpdateSchema (CoreData.GetManagedEntityIds ());
							}
							catch (System.Exception e)
							{
								UI.ShowErrorMessage ("Erreur", "Impossible de mettre à jour la base de donnée.", e);
							}
						}

						System.Environment.Exit (0);
					}
					else
					{
						// TODO This way of exiting the program is kind of violent. It might be a
						// good idea to soften it.
						// Marc
						
						UI.ShowErrorMessage (
							Res.Strings.Error.IncompatibleDatabase,
							Res.Strings.Hint.Error.IncompatibleDatabase, ex);

						System.Environment.Exit (0);
					}
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
			Factories.CoreDataComponentFactory.SetupComponents (this.components.Select (x => x.Value));
		}

#if false
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
#endif

		/// <summary>
		/// Finds the (normalized) entity key for the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The (normalized) entity key or <c>null</c> if it cannot be found in the database.</returns>
		public EntityKey? FindEntityKey(AbstractEntity entity)
		{
			return this.DataContextPool.FindEntityKey (entity);
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
				IEnumerable<T> all = repository.GetAllEntitiesIncludingLiveEntities ();

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

		public T GetSpecificRepository<T>(DataContext dataContext = null)
			where T : Repositories.Repository
		{
			return Resolvers.RepositoryResolver.Resolve<T> (this, dataContext);
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

				//	TODO: postpone destruction of data context...
#if false
				CoreApplication.QueueAsyncCallback (
					delegate
					{
						this.dataInfrastructure.DeleteDataContext (context);
					});
#endif
			}
			else
			{
				throw new System.InvalidOperationException ("Context does not belong to the pool");
			}
		}

#if false
		public BusinessContext CreateBusinessContext()
		{
			return this.businessContextPool.CreateBusinessContext ();
		}

		public void DisposeBusinessContext(BusinessContext context)
		{
			this.businessContextPool.DisposeBusinessContext (context);
		}
#endif

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


		public void SetActiveUser(IItemCode code)
		{
			this.activeUserCode = code == null ? null : code.Code;
		}
		
		internal ItemCode GetActiveUserItemCode()
		{
			if (this.activeUserCode == null)
			{
				return new ItemCode ("<none>");
			}
			else
			{
				return new ItemCode (this.activeUserCode);
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
			if (this.ForceDatabaseCreation && CoreData.CheckDatabaseEsistence (access))
			{
				CoreData.DropDatabase (access);
			}

			try
			{
				if (CoreData.CheckDatabaseEsistence (access))
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
#if false
			this.refIdGeneratorPool.GetGenerator<RelationEntity> ();
			this.refIdGeneratorPool.GetGenerator<AffairEntity> ();
			this.refIdGeneratorPool.GetGenerator<ArticleDefinitionEntity> ();
#endif
		}

		private void VerifyDatabaseSchemas()
		{
			this.connectionManager.Validate ();

			if (!this.dataInfrastructure.CheckSchema (CoreData.GetManagedEntityIds ()))
			{
				throw new Epsitec.Cresus.Database.Exceptions.IncompatibleDatabaseException ("Incompatible database schema");
			}
		}

		private static IEnumerable<Druid> GetManagedEntityIds()
		{
			return EntityClassFactory.GetAllEntityIds ().Where (x => x.Module >= 1000);
#if false
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
			yield return EntityInfo<OptionValueArticleParameterDefinitionEntity>.GetTypeId ();
			
			yield return EntityInfo<AffairEntity>.GetTypeId ();
			yield return EntityInfo<WorkflowEntity>.GetTypeId ();
			yield return EntityInfo<SoftwareUserEntity>.GetTypeId ();
			yield return EntityInfo<BusinessSettingsEntity>.GetTypeId ();

			yield return EntityInfo<DocumentCategoryMappingEntity>.GetTypeId ();
#endif
		}


		private void CreateDatabaseSchemas()
		{
			this.connectionManager.Validate ();

			var entityIds = CoreData.GetManagedEntityIds ().ToArray ();

			this.DataInfrastructure.CreateSchema (entityIds);
		}

		private void PopulateDatabase()
		{
			this.connectionManager.Validate ();

//-			this.PopulateDatabaseHack ();
//-			this.PopulateUsers ();
		}

		public void ImportDatabase(System.IO.FileInfo file)
		{
			RawImportMode importMode = RawImportMode.PreserveIds;

			CoreData.ImportDatabase (file, this.dataInfrastructure, importMode);
		}

		public static void ImportDatabase(System.IO.FileInfo file, DbAccess dbAccess)
		{
			RawImportMode importMode = RawImportMode.PreserveIds;

			CoreData.CreateDatabase (file, dbAccess, importMode);
		}

		public static void CreateUserDatabase(System.IO.FileInfo file, DbAccess dbAccess)
		{
			RawImportMode importMode = RawImportMode.DecrementIds;

			CoreData.CreateDatabase (file, dbAccess, importMode);
		}

		public void CreateUserDatabase(System.IO.FileInfo file)
		{
			RawImportMode importMode = RawImportMode.DecrementIds;

			CoreData.ImportDatabase (file, this.dataInfrastructure, importMode);
		}

		private static void CreateDatabase(System.IO.FileInfo file, DbAccess dbAccess, RawImportMode importMode)
		{
			if (CoreData.CheckDatabaseEsistence (dbAccess))
			{
				CoreData.DropDatabase (dbAccess);
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.CreateDatabase (dbAccess);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("root");

					dataInfrastructure.CreateSchema (CoreData.GetManagedEntityIds ());

					CoreData.ImportDatabase (file, dataInfrastructure, importMode);
				}
			}
		}

		private static void ImportDatabase(System.IO.FileInfo file, DataInfrastructure dataInfrastructure, RawImportMode importMode)
		{
			dataInfrastructure.ImportEpsitecData (file, importMode);
		}

		public void ImportSharedData(System.IO.FileInfo file)
		{
			CoreData.ImportSharedData (file, this.dataInfrastructure);
		}

		public static void ImportSharedData(System.IO.FileInfo file, DbAccess dbAccess)
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (dbAccess);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("root");

					CoreData.ImportSharedData (file, dataInfrastructure);
				}
			}
		}
		
		private static void ImportSharedData(System.IO.FileInfo file, DataInfrastructure dataInfrastructure)
		{
			dataInfrastructure.ImportEpsitecData (file, RawImportMode.DecrementIds);
		}

		public void ExportDatabase(System.IO.FileInfo file, bool exportOnlyUserData)
		{
			CoreData.ExportDatabase (file, this.dataInfrastructure, exportOnlyUserData);
		}

		public static void ExportDatabase(System.IO.FileInfo file, DbAccess dbAccess, bool exportOnlyUserData)
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (dbAccess);

				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("root");

					CoreData.ExportDatabase (file, dataInfrastructure, exportOnlyUserData);
				}
			}
		}

		private static void ExportDatabase(System.IO.FileInfo file, DataInfrastructure dataInfrastructure, bool exportOnlyUserData)
		{
			RawExportMode exportMode = exportOnlyUserData ? RawExportMode.UserData : RawExportMode.EpsitecAndUserData;

			dataInfrastructure.ExportEpsitecData (file, exportMode);
		}

		public static void BackupDatabase(string remoteFilePath, DbAccess dbAccess)
		{
			DbInfrastructure.BackupDatabase (dbAccess, remoteFilePath);
		}

		public static void RestoreDatabase(string remoteFilePath, DbAccess dbAccess)
		{
			DbInfrastructure.RestoreDatabase (dbAccess, remoteFilePath);
		}

		private static bool CheckDatabaseEsistence(DbAccess access)
		{
			return DbInfrastructure.CheckDatabaseExistence (access);
		}

		public static void DropDatabase(DbAccess access)
		{
			DbInfrastructure.DropDatabase (access);
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

		public static DbAccess GetDatabaseAccess()
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess ("core");

			access.IgnoreInitialConnectionErrors = true;
			access.CheckConnection = true;

			return access;
		}



		private readonly DbInfrastructure dbInfrastructure;
		private readonly DataLayer.Infrastructure.DataInfrastructure dataInfrastructure;
		private readonly EntityContext independentEntityContext;
		private readonly RefIdGeneratorPool refIdGeneratorPool;
		private readonly ConnectionManager connectionManager;
//-		private readonly BusinessContextPool businessContextPool;
		private readonly Locker locker;
//		private readonly ImageDataStore imageDataStore;
		private readonly Dictionary<string, CoreDataComponent> components;

		private DataContext immutableDataContext;
		private DataContext stableDataContext;
		private DataContext activeDataContext;

		private string activeUserCode;
	}
}