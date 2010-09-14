//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Orchestrators.Navigation;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public sealed class BusinessContext : System.IDisposable
	{
		public BusinessContext(BusinessContextPool pool)
		{
			this.pool = pool;
			this.UniqueId = System.Threading.Interlocked.Increment (ref BusinessContext.nextUniqueId);
			this.pool.Add (this);
			this.entityRecords = new List<EntityRecord> ();

			this.dataContext = this.pool.CreateDataContext (this);
			this.dataContext.EntityChanged += this.HandleDataContextEntityChanged;
			
			this.locker = this.Data.DataLocker;
		}


		public int UniqueId
		{
			get;
			private set;
		}

		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}

		public CoreData Data
		{
			get
			{
				return CoreProgram.Application.Data;
			}
		}

		public bool IsLocked
		{
			get
			{
				return this.lockTransaction != null;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		public bool IsDiscarded
		{
			get
			{
				return this.dataContextDiscarded;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return (this.activeEntity == null)
					&& (this.entityRecords.Count == 0)
					&& (this.activeNavigationPathElement == null);
			}
		}

		public bool ContainsChanges
		{
			get
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
		}

		public AbstractEntity ActiveEntity
		{
			get
			{
				return this.activeEntity;
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

		
		public void SetActiveEntity(EntityKey? entityKey, NavigationPathElement navigationPathElement)
		{
			this.SetActiveEntity (this.dataContext.ResolveEntity (entityKey));
			this.SetNavigationPathElement (navigationPathElement);
		}

		
		public void Discard()
		{
			if (this.dataContextDiscarded == false)
			{
				this.dataContextDiscarded = true;
			}
		}

		public void SaveChanges()
		{
			if (this.ContainsChanges)
            {
				this.dataContext.SaveChanges ();
				this.OnContainsChangesChanged ();
            }
		}

		
		private void SetNavigationPathElement(NavigationPathElement navigationPathElement)
		{
			this.activeNavigationPathElement = navigationPathElement;
		}

		private void SetActiveEntity(AbstractEntity entity)
		{
			System.Diagnostics.Debug.Assert (entity != null);

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
			return this.ApplyRules (RuleType.Setup, this.DataContext.CreateEntity (entityType));
		}
		
		public T CreateEntity<T>()
			where T : AbstractEntity, new ()
		{
			return this.ApplyRules (RuleType.Setup, this.DataContext.CreateEntity<T> ());
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
//-			return this.entityRecords.Select (x => CoreDataLocker.GetLockName (x.DataContext, x.Entity));
		}


		private void OnContainsChangesChanged()
		{
			var handler = this.ContainsChangesChanged;

			if (handler != null)
			{
				handler (this);
			}
		}


		public event EventHandler ContainsChangesChanged;

		private static int nextUniqueId;

		private readonly BusinessContextPool pool;
		private readonly DataContext dataContext;
		private readonly List<EntityRecord> entityRecords;
		private readonly CoreDataLocker locker;

		private int dataChangedCounter;
		private bool dataContextDirty;
		private bool dataContextDiscarded;
		private bool isDisposed;
		private CoreDataLockTransaction lockTransaction;

		private AbstractEntity activeEntity;
		private NavigationPathElement activeNavigationPathElement;
	}
}