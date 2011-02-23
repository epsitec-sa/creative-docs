//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Orchestrators.Navigation;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Business
{
	public sealed class BusinessContext : IIsDisposed, IBusinessContext
	{
		public BusinessContext(BusinessContextPool pool)
		{
			this.pool = pool;
			this.UniqueId = System.Threading.Interlocked.Increment (ref BusinessContext.nextUniqueId);
			this.pool.Add (this);
			this.entityRecords = new List<EntityRecord> ();
			this.masterEntities = new List<AbstractEntity> ();

			this.data = this.pool.Data;
			this.dataContext = this.pool.CreateDataContext (this);
			this.dataContext.EntityChanged += this.HandleDataContextEntityChanged;
			
			this.locker = this.Data.DataLocker;
		}


		public int								UniqueId
		{
			get;
			private set;
		}

		public DataContext						DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public CoreData							Data
		{
			get
			{
				return this.data;
			}
		}

		public bool								IsLocked
		{
			get
			{
				return this.lockTransaction != null;
			}
		}

		public bool								IsDiscarded
		{
			get
			{
				return this.dataContextDiscarded;
			}
		}

		public bool								IsEmpty
		{
			get
			{
				return (this.activeEntity == null)
					&& (this.entityRecords.Count == 0)
					&& (this.activeNavigationPathElement == null);
			}
		}

		public AbstractEntity					ActiveEntity
		{
			get
			{
				return this.activeEntity;
			}
		}

		public NavigationPathElement			ActiveNavigationPathElement
		{
			get
			{
				return this.activeNavigationPathElement;
			}
		}

		public GlobalLock						GlobalLock
		{
			get
			{
				return this.globalLock;
			}
			set
			{
				if (this.IsLocked)
				{
					throw new System.InvalidOperationException ();
				}

				this.globalLock = value;
			}
		}

		#region IIsDisposed Members
		
		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		#endregion

		
		public bool ContainsChanges()
		{
			if ((this.isDisposed) ||
				(this.IsDiscarded))
			{
				return false;
			}
			else
			{
				return this.hasExternalChanges || this.dataContext.ContainsChanges ();
			}
		}

		public bool AreAllLocksAvailable()
		{
			return this.locker.AreAllLocksAvailable (this.GetLockNames ());
		}

		public void NotifyExternalChanges()
		{
			if (this.hasExternalChanges == false)
			{
				this.hasExternalChanges = true;
				this.OnContainsChangesChanged ();
			}
		}


		/// <summary>
		/// Acquires the lock (as defined by the <see cref="BusinessContext.GlobalLock"/> and
		/// <see cref="BusinessContext.ActiveEntity"/> properties).
		/// </summary>
		/// <returns><c>true</c> if the lock could be acquired; otherwise, <c>false</c>.</returns>
		public bool AcquireLock()
		{
			IList<LockOwner> foreignLockOwners;
			return this.AcquireLock (out foreignLockOwners);
		}

		/// <summary>
		/// Acquires the lock (as defined by the <see cref="BusinessContext.GlobalLock"/> and
		/// <see cref="BusinessContext.ActiveEntity"/> properties).
		/// </summary>
		/// <param name="foreignLockOwners">The foreign lock owners, if the lock acquisition failed.</param>
		/// <returns><c>true</c> if the lock could be acquired; otherwise, <c>false</c>.</returns>
		public bool AcquireLock(out IList<LockOwner> foreignLockOwners)
		{
			if (this.IsLocked)
			{
				foreignLockOwners = EmptyList<LockOwner>.Instance;
				return true;
			}

			var lockNames       = this.GetLockNames ().Where (name => !string.IsNullOrEmpty (name));
			var lockTransaction = this.locker.RequestLock (lockNames, out foreignLockOwners);

			if (lockTransaction == null)
			{
				return false;
			}

			if (lockTransaction.LockSate == DataLayer.Infrastructure.LockState.Locked)
			{
				this.lockTransaction = lockTransaction;
				this.OnLockAcquired ();

				return true;
			}
			else
			{
				lockTransaction.Dispose ();
				return false;
			}
		}

		/// <summary>
		/// Releases the lock acquired by <see cref="BusinessContext.AcquireLock"/>.
		/// </summary>
		/// <returns></returns>
		public bool ReleaseLock()
		{
			if (this.IsLocked == false)
			{
				return false;
			}

			System.Diagnostics.Debug.WriteLine ("*** LOCK RELEASED ***");
			this.lockTransaction.Dispose ();
			this.lockTransaction = null;

			return true;
		}

		public LockMonitor CreateLockMonitor()
		{
			return this.Data.DataLocker.CreateLockMonitor (this.GetLockNames ());
		}
		
		public System.IDisposable AutoLock<T>(T entity)
			where T : AbstractEntity
		{
			if (this.IsLocked)
			{
				return new NoOpUnlocker (this);
			}
			if (entity.IsNull ())
            {
				throw new System.ArgumentNullException ("Cannot acquire lock on null entity");
            }

			this.lockEntity = entity;

			if (this.AcquireLock ())
			{
				return new Unlocker (this);
			}

			throw new System.InvalidOperationException ("Could not acquire lock");
		}

		private class Unlocker : System.IDisposable
		{
			public Unlocker(BusinessContext context)
			{
				this.context = context;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (this.context != null)
				{
					this.context.ReleaseLock ();
					this.context.lockEntity = null;
				}
			}

			#endregion

			private readonly BusinessContext	context;
		}

		private class NoOpUnlocker : System.IDisposable
		{
			public NoOpUnlocker(BusinessContext context)
			{
				this.context = context;
			}

			#region IDisposable Members

			public void Dispose()
			{
				if (this.context != null)
				{
					System.Diagnostics.Debug.Assert (this.context.IsLocked);
				}
			}

			#endregion

			private readonly BusinessContext	context;
		}

		public void SetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement = null)
		{
			System.Diagnostics.Debug.Assert (this.activeEntity == null);
			System.Diagnostics.Debug.Assert (this.activeNavigationPathElement == null);

			this.SetActiveEntity (this.dataContext.ResolveEntity (entityKey));
			this.SetNavigationPathElement (navigationPathElement);
		}

		public T SetActiveEntity<T>(EntityKey? entityKey, NavigationPathElement navigationPathElement = null)
			where T : AbstractEntity
		{
			this.SetActiveEntity (entityKey, navigationPathElement);
			return this.ActiveEntity as T;
		}

		/// <summary>
		/// Adds a master entity of a specific type. See <seealso cref="GerMasterEntity{T}"/>.
		/// </summary>
		/// <param name="masterEntity">The master entity.</param>
		public void AddMasterEntity(AbstractEntity masterEntity)
		{
			this.masterEntities.Add (masterEntity);
			this.OnMasterEntitiesChanged ();
		}

		/// <summary>
		/// Removes a master entity of a specific type. See <seealso cref="GerMasterEntity{T}"/>.
		/// </summary>
		/// <param name="masterEntity">The master entity.</param>
		public void RemoveMasterEntity(AbstractEntity masterEntity)
		{
			this.masterEntities.Remove (masterEntity);
			this.OnMasterEntitiesChanged ();
		}

		/// <summary>
		/// Gets the master entity of type <typeparamref name="T"/>. A master entity is an entity
		/// for which there is currently an open piece of UI, or simply an entity which is a key
		/// element in a graph (for instance the 'active' affair for a customer).
		/// </summary>
		/// <typeparam name="T">Type of the entity.</typeparam>
		/// <returns>The master entity of type <typeparamref name="T"/> if it exists; otherwise, returns a wrapped null entity.</returns>
		public T GetMasterEntity<T>()
			where T : AbstractEntity, new ()
		{
			//	Either use the most recent entity on which the business logic is currently
			//	operating, or the freshest entity defined as a master entity.
			
			T master;

			if (Logic.Current == null)
			{
				master = null;
			}
			else
			{
				master = Logic.Current.Find<T> ().FirstOrDefault ();
			}

			if (master == null)
			{
				master = this.masterEntities.OfType<T> ().LastOrDefault ();
			}

			if (master == null)
			{
				//	There is no defined master entity in the current context; try to derive it
				//	from the active entities.
				
				var entities = this.DataContext.GetEntitiesOfType<T> ();
				
				foreach (var entity in entities)
				{
					if (master != null)
					{
						throw new System.InvalidOperationException ("More than one entity of type " + typeof (T).Name);
					}

					master = entity;
				}
			}

			return master.WrapNullEntity ();
		}

		public IEnumerable<AbstractEntity> GetMasterEntities()
		{
			return this.masterEntities;
		}

		public Date GetReferenceDate()
		{
			//	TODO: use real reference date, based on the active master entities...
			return Date.Today;
		}
		
		public void Discard()
		{
			if (this.dataContextDiscarded == false)
			{
				this.dataContextDiscarded = true;
			}
		}

		void IBusinessContext.SaveChanges()
		{
			this.SaveChanges (EntitySaveMode.None);
		}

		public void SaveChanges(EntitySaveMode entitySaveMode = EntitySaveMode.None)
		{
			if (this.ContainsChanges ())
			{
				var notYetPersistedEntities = new HashSet<AbstractEntity> (this.dataContext.GetEntities ().Where (x => !this.dataContext.IsPersistent (x)));
				System.Predicate<AbstractEntity> isEmptyEntity = x => x.IsEntityEmpty;

				if (entitySaveMode.HasFlag (EntitySaveMode.IncludeEmpty))
				{
					isEmptyEntity = x => false;
				}
				
				foreach (var entity in notYetPersistedEntities)
				{
					this.dataContext.UpdateEmptyEntityStatus (entity, isEmptyEntity (entity));
				}

				var e = new CancelEventArgs ();

				this.hasExternalChanges = false;
				this.OnSavingChanges (e);

				if (e.Cancel == false)
				{
					this.dataContext.SaveChanges ();
				}

				this.OnContainsChangesChanged ();
			}
		}

		/// <summary>
		/// Gets the local equivalent of the specified entity: if the entity belongs to another
		/// data context, then it will have to be mapped to the local data context through its
		/// database key.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <param name="entity">The entity (or <c>null</c>).</param>
		/// <returns>The local entity (or a wrapped null entity if <c>entity</c> was <c>null</c>).</returns>
		public T GetLocalEntity<T>(T entity)
			where T : AbstractEntity, new ()
		{
			if (entity == null)
            {
				return entity.WrapNullEntity ();
            }

			return this.dataContext.GetLocalEntity (entity);
		}

		/// <summary>
		/// Gets the local equivalent of the specified entity: if the entity belongs to another
		/// data context, then it will have to be mapped to the local data context through its
		/// database key.
		/// </summary>
		/// <param name="entity">The entity (which may not be <c>null</c>).</param>
		/// <returns>The local entity.</returns>
		public AbstractEntity GetLocalEntity(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			return this.dataContext.GetLocalEntity (entity);
		}

		/// <summary>
		/// Gets the specified repository. The entities returned by the repository will
		/// automatically be mapped to the current context.
		/// </summary>
		/// <typeparam name="T">The repository type.</typeparam>
		/// <returns>The repository.</returns>
		public T GetSpecificRepository<T>()
			where T : Repositories.Repository
		{
			var repository = this.Data.GetSpecificRepository<T> ();
			return repository.DefineMapper (x => this.GetLocalEntity (x)) as T;
		}

		/// <summary>
		/// Gets the repository for the specified entity type. The entities returned by the
		/// repository will automatically be mapped to the current context.
		/// </summary>
		/// <typeparam name="T">The entity type.</typeparam>
		/// <returns>The repository.</returns>
		public Repositories.Repository<T> GetRepository<T>()
			where T : AbstractEntity, new ()
		{
			var repository = this.Data.GetRepository<T> ();
			return repository.DefineMapper (x => this.GetLocalEntity (x)) as Repositories.Repository<T>;
		}
		


		private void SetNavigationPathElement(NavigationPathElement navigationPathElement)
		{
			this.activeNavigationPathElement = navigationPathElement;
		}

		public void SetActiveEntity(AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (entity != null);
			System.Diagnostics.Debug.Assert (this.activeEntity == null);
			
			this.activeEntity = entity;

			this.Register (entity);
		}

		public void Register(AbstractEntity entity)
		{
			if (entity.IsNotNull ())
			{
				if (this.entityRecords.Any (x => x.Entity == entity))
				{
					throw new System.InvalidOperationException ("Duplicate entity registration");
				}

				this.entityRecords.Add (new EntityRecord (entity, this));
				this.ApplyRules (RuleType.Bind, entity);
			}
		}

		public void Register(IEnumerable<AbstractEntity> collection)
		{
			if (collection != null)
			{
				collection.ForEach (entity => this.Register (entity));
			}
		}

		public void ApplyRulesToRegisteredEntities(RuleType ruleType)
		{
			this.entityRecords.ForEach (x => x.Logic.ApplyRules (ruleType, x.Entity));
		}

		public T ApplyRules<T>(RuleType ruleType, T entity)
			where T : AbstractEntity
		{
			var logic = this.CreateLogic (entity);
			logic.ApplyRules (ruleType, entity);
			return entity;
		}

		public AbstractEntity CreateEntity(Druid entityType)
		{
			var newEntity = this.DataContext.CreateEntity (entityType);
			return this.ApplyRules (RuleType.Setup, newEntity);
		}

		public T CreateEntity<T>()
			where T : AbstractEntity, new ()
		{
			T newEntity = this.DataContext.CreateEntity<T> ();
			return this.ApplyRules (RuleType.Setup, newEntity);
		}

		public T CreateEntityAndRegisterAsEmpty<T>()
			where T : AbstractEntity, new ()
		{
			T newEntity = this.DataContext.CreateEntityAndRegisterAsEmpty<T> ();
			return this.ApplyRules (RuleType.Setup, newEntity);
		}

		public bool ArchiveEntity<T>(T entity)
			where T : AbstractEntity, new ()
		{
			ILifetime lifetime = entity as ILifetime;

			if (lifetime != null)
			{
				if (this.DataContext.IsPersistent (entity))
				{
					lifetime.IsArchive = true;
				}
				else
				{
					this.DataContext.DeleteEntity (entity);
				}

				return true;
			}

			throw new System.InvalidOperationException ("ArchiveEntity not possible on this entity type");
		}


		#region IDisposable Members

		public void Dispose()
		{
			if (this.isDisposed == false)
			{
				this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;

				this.pool.DisposeDataContext (this, this.dataContext);
				this.pool.Remove (this);

				if (this.lockTransaction != null)
				{
					this.ReleaseLock ();
				}

				System.GC.SuppressFinalize (this);
				this.isDisposed = true;
			}
		}

		#endregion
		
		#region EntityRecord Class

		private class EntityRecord
		{
			public EntityRecord(AbstractEntity entity, BusinessContext businessContext)
			{
				this.entity = entity;
				this.businessContext = businessContext;
				this.dataContext = this.businessContext.Data.DataContextPool.FindDataContext (entity);
			}

			public DataContext DataContext
			{
				get
				{
					return this.dataContext;
				}
			}

			public AbstractEntity Entity
			{
				get
				{
					return this.entity;
				}
			}

			public Logic Logic
			{
				get
				{
					if (this.logic == null)
                    {
						this.logic = this.businessContext.CreateLogic (this.entity);
                    }

					return this.logic;
				}
			}


			private readonly AbstractEntity entity;
			private readonly BusinessContext businessContext;
			private readonly DataContext dataContext;
			private Logic logic;
		}

		#endregion

		private void OnLockAcquired()
		{
			System.Diagnostics.Debug.WriteLine ("*** LOCK ACQUIRED ***");

			this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;
			this.dataContext.Reload ();
			this.dataContext.EntityChanged += this.HandleDataContextEntityChanged;
		}

		
		private Logic CreateLogic(AbstractEntity entity)
		{
			return new Logic (entity, this);
		}

		private void HandleDataContextEntityChanged(object sender, EntityChangedEventArgs e)
		{
			try
			{
				if (System.Threading.Interlocked.Increment (ref this.dataChangedCounter) == 1)
				{
					if (this.dataContextDirty == false)
					{
						this.dataContextDirty = true;
						this.HandleFirstEntityChange ();
					}

					if (Logic.Current == null)
					{
						this.ApplyRulesToRegisteredEntities (RuleType.Update);
					}
				}

				this.OnContainsChangesChanged ();

				this.AsyncUpdateMainWindowController ();
			}
			finally
			{
				System.Threading.Interlocked.Decrement (ref this.dataChangedCounter);
			}
		}

		private void AsyncUpdateMainWindowController()
		{
			Dispatcher.Queue (this.SyncUpdateMainWindowController);
		}

		private void SyncUpdateMainWindowController()
		{
			if (this.isDisposed)
			{
				System.Diagnostics.Debug.WriteLine ("Calling BusinessContext.SyncUpdateMainWindowController on disposed context");
				return;
			}

			Dispatcher.RequestRefreshUI ();
		}

		private void HandleFirstEntityChange()
		{
			this.AcquireLock ();
		}

		private IEnumerable<string> GetLockNames()
		{
			if (this.globalLock !=  null)
			{
				yield return this.globalLock.LockId;
			}

			if (this.activeEntity != null)
			{
				yield return Locker.GetEntityLockName (this.dataContext, this.activeEntity);
			}
			if (this.lockEntity != null)
			{
				yield return Locker.GetEntityLockName (this.dataContext, this.lockEntity);
			}
		}


		private void OnSavingChanges(CancelEventArgs e)
		{
			var handler = this.SavingChanges;

			if (handler != null)
			{
				handler (this, e);
			}
		}

		private void OnContainsChangesChanged()
		{
			var handler = this.ContainsChangesChanged;

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnMasterEntitiesChanged()
		{
			var handler = this.MasterEntitiesChanged;

			if (handler != null)
            {
				handler (this);
            }
		}

        public event EventHandler<CancelEventArgs>	SavingChanges;
		public event EventHandler					ContainsChangesChanged;
		public event EventHandler					MasterEntitiesChanged;

		private static int						nextUniqueId;

		private readonly BusinessContextPool	pool;
		private readonly DataContext			dataContext;
		private readonly List<EntityRecord>		entityRecords;
		private readonly List<AbstractEntity>	masterEntities;
		private readonly Locker					locker;
		private readonly CoreData				data;

		private int								dataChangedCounter;
		private bool							dataContextDirty;
		private bool							dataContextDiscarded;
		private bool							isDisposed;
		private bool							hasExternalChanges;
		private Data.LockTransaction			lockTransaction;

		private GlobalLock						globalLock;
		private AbstractEntity					activeEntity;
		private AbstractEntity					lockEntity;
		private NavigationPathElement			activeNavigationPathElement;
	}
}
