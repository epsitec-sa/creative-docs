//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Controllers;

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

			this.dataContext = this.pool.DataContext;
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

		public void Register(AbstractEntity entity)
		{
			if (this.entityRecords.Any (x => x.Entity == entity))
            {
				throw new System.InvalidOperationException ("Duplicate entity registration");
            }

			this.entityRecords.Add (new EntityRecord (entity, this));
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

		public void ApplyRules(RuleType ruleType, AbstractEntity entity)
		{
			var logic = this.CreateLogic (entity.GetType ());
			logic.ApplyRules (ruleType, entity);
		}

		public T CreateEntity<T>()
			where T : AbstractEntity, new ()
		{
			T entity = this.DataContext.CreateEntity<T> ();

			this.ApplyRules (RuleType.Setup, entity);

			return entity;
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;

			this.Data.LowLevelSaveDataContext (this.dataContext);
			this.pool.Remove (this);

			if (this.lockTransaction != null)
			{
				this.lockTransaction.Dispose ();
				this.lockTransaction = null;
			}
			
			System.GC.SuppressFinalize (this);
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
			return this.entityRecords.Select (x => CoreDataLocker.GetLockName (x.DataContext, x.Entity));
		}


		private static int nextUniqueId;

		private readonly BusinessContextPool pool;
		private readonly DataContext dataContext;
		private readonly List<EntityRecord> entityRecords;
		private readonly CoreDataLocker locker;

		private int dataChangedCounter;
		private bool dataContextDirty;
		private CoreDataLockTransaction lockTransaction;
	}

	public sealed class BusinessContextPool
	{
		public BusinessContextPool(CoreData data)
		{
			this.data = data;
			this.businessContexts = new List<BusinessContext> ();
		}

		public DataContext DataContext
		{
			get
			{
				return this.context;
			}
		}
		public bool IsEmpty
		{
			get
			{
				return this.businessContexts.Count == 0;
			}
		}

		public void Add(BusinessContext context)
		{
			if (this.IsEmpty)
			{
				this.context = this.data.CreateDataContext ("BusinessContextPool");
			}

			this.businessContexts.Add (context);
		}

		public void Remove(BusinessContext context)
		{
			this.businessContexts.Remove (context);

			if (this.IsEmpty)
			{
				this.data.DisposeDataContext (this.context);
				this.context = null;
			}
		}

		private readonly List<BusinessContext> businessContexts;
		private readonly CoreData data;
		private DataContext context;
	}
}