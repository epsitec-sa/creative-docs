//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.Business
{
	public sealed class BusinessContext : System.IDisposable, IIsDisposed
	{
		public BusinessContext(BusinessContextPool pool)
		{
			this.pool = pool;
			this.UniqueId = System.Threading.Interlocked.Increment (ref BusinessContext.nextUniqueId);
			this.pool.Add (this);
			this.entityRecords = new List<EntityRecord> ();
			this.masterEntities = new List<AbstractEntity> ();

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
				return CoreProgram.Application.Data;
			}
		}

		public bool								IsLocked
		{
			get
			{
				return this.lockTransaction != null;
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

		
		public bool ContainsChanges()
		{
			if ((this.isDisposed) ||
				(this.IsDiscarded))
			{
				return false;
			}
			else
			{
				return this.dataContext.ContainsChanges ();
			}
		}

		public bool AreAllLocksAvailable()
		{
			return this.locker.AreAllLocksAvailable (this.GetLockNames ());
		}

		
		public bool AcquireLock()
		{
			if (this.IsLocked)
			{
				return true;
			}

			var lockTransaction = this.locker.RequestLock (this.GetLockNames ());

			if (lockTransaction == null)
			{
				return false;
			}

			if (lockTransaction.LockSate == DataLayer.Infrastructure.LockState.Locked)
			{
				System.Diagnostics.Debug.WriteLine ("*** LOCK ACQUIRED ***");
				this.lockTransaction = lockTransaction;
				return true;
			}

			lockTransaction.Dispose ();
			return false;
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
			var entities = this.DataContext.GetEntitiesOfType<T> ();
			T   master   = this.masterEntities.OfType<T> ().LastOrDefault ();

			if (master == null)
			{
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
				

				this.dataContext.SaveChanges ();
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
			if (entity != null)
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
			var logic = this.CreateLogic (entity.GetType ());
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
					System.Diagnostics.Debug.WriteLine ("*** LOCK RELEASED ***");
					this.lockTransaction.Dispose ();
					this.lockTransaction = null;
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
				this.dataContext = DataContextPool.Instance.FindDataContext (entity);
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
						this.logic = this.businessContext.CreateLogic (this.entity.GetType ());
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

		private Logic CreateLogic(System.Type entityType)
		{
			return new Logic (entityType, this);
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
				CoreProgram.Application.MainWindowController.Update ();
			}
			finally
			{
				System.Threading.Interlocked.Decrement (ref this.dataChangedCounter);
			}
		}

		private void HandleFirstEntityChange()
		{
			this.AcquireLock ();
		}

		private IEnumerable<string> GetLockNames()
		{
			if (this.activeEntity != null)
			{
				yield return CoreDataLocker.GetLockName (this.dataContext, this.activeEntity);
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

		public event EventHandler ContainsChangesChanged;
		public event EventHandler MasterEntitiesChanged;

		private static int nextUniqueId;

		private readonly BusinessContextPool pool;
		private readonly DataContext dataContext;
		private readonly List<EntityRecord> entityRecords;
		private readonly List<AbstractEntity> masterEntities;
		private readonly CoreDataLocker locker;

		private int dataChangedCounter;
		private bool dataContextDirty;
		private bool dataContextDiscarded;
		private bool isDisposed;
		private CoreDataLockTransaction lockTransaction;

		private AbstractEntity activeEntity;
		private NavigationPathElement activeNavigationPathElement;
	}
	[System.Flags]
	public enum EntitySaveMode
	{
		None = 0,

		IncludeEmpty = 0x0001,
	}
}