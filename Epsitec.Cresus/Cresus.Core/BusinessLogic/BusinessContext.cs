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
		public BusinessContext(DataContext dataContext)
		{
			this.dataContext = dataContext;
			this.entityRecords = new List<EntityRecord> ();
			
			this.UniqueId = System.Threading.Interlocked.Increment (ref BusinessContext.nextUniqueId);

			this.dataContext.EntityChanged += this.HandleDataContextEntityChanged;
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


		public static BusinessContext GetBusinessContext(CoreViewController controller)
		{
			var dataContext = controller.DataContext;

			while (controller != null)
			{
				var context = controller.GetLocalBusinessContext ();

				if ((context != null) ||
					(controller.DataContext != dataContext))
				{
					return context;
				}

				controller = controller.ParentController;
			}

			return null;
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.dataContext.EntityChanged -= this.HandleDataContextEntityChanged;
			
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
			if (Logic.Current == null)
            {
				this.ApplyRulesToRegisteredEntities (RuleType.Update);
            }
		}

		private IEnumerable<string> GetLockNames()
		{
			return this.entityRecords.Select (x => CoreDataLocker.GetLockName (x.DataContext, x.Entity));
		}


		private static int nextUniqueId;

		private readonly DataContext dataContext;
		private readonly List<EntityRecord> entityRecords;
	}
}