//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.BusinessLogic
{
	public class BusinessContext
	{
		public BusinessContext(DataContext dataContext)
		{
			this.dataContext = dataContext;
			this.entityRecords = new List<EntityRecord> ();
		}


		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
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

		public void ApplyRules(RuleType ruleType)
		{
			this.entityRecords.ForEach (x => x.Logic.ApplyRules (ruleType, x.Entity));
		}

		
		#region EntityRecord Class

		private class EntityRecord
		{
			public EntityRecord(AbstractEntity entity, BusinessContext businessContext)
			{
				this.entity = entity;
				this.businessContext = businessContext;
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
						this.logic = new Logic (this.entity.GetType (), this.businessContext);
                    }

					return this.logic;
				}
			}


			private readonly AbstractEntity entity;
			private readonly BusinessContext businessContext;
			private Logic logic;
		}

		#endregion


		private readonly DataContext dataContext;
		private readonly List<EntityRecord> entityRecords;
	}
}