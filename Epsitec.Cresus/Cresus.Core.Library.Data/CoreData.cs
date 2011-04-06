//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;
using System.Linq;



namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData : CoreAppComponent, System.IDisposable, ICoreComponentHost<CoreDataComponent>
	{
		public CoreData(CoreApp app, bool forceDatabaseCreation = false, bool allowDatabaseUpdate = true, bool enableConnectionRecycling = true)
			: base (app)
		{
			this.ForceDatabaseCreation		= forceDatabaseCreation;
			this.AllowDatabaseUpdate		= allowDatabaseUpdate;
			this.EnableConnectionRecycling	= enableConnectionRecycling;

			this.components = new CoreComponentHostImplementation<CoreDataComponent> ();
			this.independentEntityContext = new EntityContext (Resources.DefaultManager, EntityLoopHandlingMode.Throw, "Independent Entities");

			Factories.CoreDataComponentFactory.RegisterComponents (this);
		}

		public DataInfrastructure				DataInfrastructure
		{
			get
			{
				return this.ContainsComponent<Infrastructure> () ? this.GetComponent<Infrastructure> ().DataInfrastructure : null;
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
				return this.ContainsComponent<Locker> () ? this.GetComponent<Locker> () : null;
			}
		}

		public ConnectionManager				ConnectionManager
		{
			get
			{
				return this.GetComponent<ConnectionManager> ();
			}
		}

		public bool								IsReady
		{
			get
			{
				return this.isReady;
			}
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

		public bool								EnableConnectionRecycling
		{
			get;
			private set;
		}


		#region ICoreComponentHost Interface

		public T GetComponent<T>()
			where T : CoreDataComponent
		{
			return this.components.GetComponent<T> ();
		}

		public bool ContainsComponent<T>()
			where T : CoreDataComponent
		{
			return this.components.ContainsComponent<T> ();
		}

		CoreDataComponent ICoreComponentHost<CoreDataComponent>.GetComponent(System.Type type)
		{
			return this.components.GetComponent (type);
		}

		IEnumerable<CoreDataComponent> ICoreComponentHost<CoreDataComponent>.GetComponents()
		{
			return this.components.GetComponents ();
		}

		bool ICoreComponentHost<CoreDataComponent>.ContainsComponent(System.Type type)
		{
			return this.components.ContainsComponent (type);
		}

		void ICoreComponentHost<CoreDataComponent>.RegisterComponent<T>(T component)
		{
			this.components.RegisterComponent<T> (component);
		}

		void ICoreComponentHost<CoreDataComponent>.RegisterComponent(System.Type type, CoreDataComponent component)
		{
			this.components.RegisterComponent (type, component);
		}

		void ICoreComponentHost<CoreDataComponent>.RegisterComponentAsDisposable(System.IDisposable disposable)
		{
			this.components.RegisterComponentAsDisposable (disposable);
		}

		#endregion

		public DataContext GetDataContext(DataLifetimeExpectancy lifetimeExpectancy)
		{
			switch (lifetimeExpectancy)
			{
				case DataLifetimeExpectancy.Stable:
					return this.EnsureDataContext (ref this.stableDataContext, lifetimeExpectancy.ToString ());

				case DataLifetimeExpectancy.Immutable:
					return this.EnsureDataContext (ref this.immutableDataContext, lifetimeExpectancy.ToString ());
			}

			throw new System.NotImplementedException ();
		}

		private DataContext EnsureDataContext(ref DataContext dataContext, string name)
		{
			if (dataContext == null)
			{
				dataContext = this.CreateDataContext (name);
			}
			
			return dataContext;
		}



		public void SetupBusiness()
		{
			Factories.CoreDataComponentFactory.SetupComponents (this.components.GetComponents ());
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
						all = all.OrderBy (x => x as IItemRank, CoreData.ItemRankComparer.Instance);
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
			var context = this.DataInfrastructure.CreateDataContext (true);
			context.Name = name;

			return context;
		}

		public void DisposeDataContext(DataContext context)
		{
			if (this.DataInfrastructure.ContainsDataContext (context))
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
						this.DataInfrastructure.DeleteDataContext (context);
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

		internal void NotifyDatabaseReady()
		{
			this.isReady = true;
		}


		#region IDisposable Members


		public void Dispose()
		{
			if (this.activeDataContext != null)
			{
				this.activeDataContext.Dispose ();
				this.activeDataContext = null;
			}

			this.components.Dispose ();
		}


		#endregion

		public void ImportDatabase(System.IO.FileInfo file)
		{
			RawImportMode importMode = RawImportMode.PreserveIds;

			CoreData.ImportDatabase (file, this.DataInfrastructure, importMode);
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

			CoreData.ImportDatabase (file, this.DataInfrastructure, importMode);
		}

		private static void CreateDatabase(System.IO.FileInfo file, DbAccess dbAccess, RawImportMode importMode)
		{
			if (Infrastructure.CheckDatabaseExistence (dbAccess))
			{
				Infrastructure.DropDatabase (dbAccess);
			}

			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.CreateDatabase (dbAccess);
			}

			IEnumerable<Druid> managedEntityIds = Infrastructure.GetManagedEntityIds ();

			EntityEngine.Create (dbAccess, managedEntityIds);
			EntityEngine entityEngine = EntityEngine.Connect (dbAccess, managedEntityIds);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbAccess, entityEngine))
			{
				dataInfrastructure.OpenConnection ("root");

				CoreData.ImportDatabase (file, dataInfrastructure, importMode);
			}
		}

		private static void ImportDatabase(System.IO.FileInfo file, DataInfrastructure dataInfrastructure, RawImportMode importMode)
		{
			dataInfrastructure.ImportEpsitecData (file, importMode);
		}

		public void ImportSharedData(System.IO.FileInfo file)
		{
			CoreData.ImportSharedData (file, this.DataInfrastructure);
		}

		public static void ImportSharedData(System.IO.FileInfo file, DbAccess dbAccess)
		{
			IEnumerable<Druid> managedEntityIds = Infrastructure.GetManagedEntityIds ();

			EntityEngine entityEngine = EntityEngine.Connect (dbAccess, managedEntityIds);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbAccess, entityEngine))
			{
				dataInfrastructure.OpenConnection ("root");

				CoreData.ImportSharedData (file, dataInfrastructure);
			}
		}
		
		private static void ImportSharedData(System.IO.FileInfo file, DataInfrastructure dataInfrastructure)
		{
			dataInfrastructure.ImportEpsitecData (file, RawImportMode.DecrementIds);
		}

		public void ExportDatabase(System.IO.FileInfo file, bool exportOnlyUserData)
		{
			CoreData.ExportDatabase (file, this.DataInfrastructure, exportOnlyUserData);
		}

		public static void ExportDatabase(System.IO.FileInfo file, DbAccess dbAccess, bool exportOnlyUserData)
		{
			IEnumerable<Druid> managedEntityIds = Infrastructure.GetManagedEntityIds ();

			EntityEngine entityEngine = EntityEngine.Connect (dbAccess, managedEntityIds);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbAccess, entityEngine))
			{
				dataInfrastructure.OpenConnection ("root");

				CoreData.ExportDatabase (file, dataInfrastructure, exportOnlyUserData);
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

		#region ItemRankComparer Class

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

			public static readonly ItemRankComparer Instance = new ItemRankComparer ();
		}

		#endregion

		#region Factory Class

		class Factory : Epsitec.Cresus.Core.Factories.DefaultCoreAppComponentFactory<CoreData>
		{
			public override CoreAppComponent Create(CoreApp host)
			{
				var data = new CoreData (host, forceDatabaseCreation: false);
				data.SetupBusiness ();
				return data;
			}
		}

		#endregion

		private readonly CoreApp				app;
		private readonly CoreComponentHostImplementation<CoreDataComponent>	components;

		private readonly EntityContext			independentEntityContext;

		private bool							isReady;
		private DataContext						immutableDataContext;
		private DataContext						stableDataContext;
		private DataContext						activeDataContext;

		private string							activeUserCode;
	}
}