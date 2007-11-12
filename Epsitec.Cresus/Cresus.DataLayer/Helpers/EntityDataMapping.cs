//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	public class EntityDataMapping
	{
		public EntityDataMapping(AbstractEntity entity)
		{
			this.entity = entity;
		}

		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public Druid EntityId
		{
			get
			{
				return this.entity == null ? Druid.Empty : this.entity.GetEntityStructuredTypeId ();
			}
		}


		public DbKey this[Druid entityId]
		{
			get
			{
				if (this.parentRowKeys == null)
				{
					return DbKey.Empty;
				}
				else
				{
					DbKey value;
					
					if (this.parentRowKeys.TryGetValue (entityId, out value))
					{
						return value;
					}
					else
					{
						return DbKey.Empty;
					}
				}
			}
			set
			{
				if (this.parentRowKeys == null)
				{
					this.parentRowKeys = new Dictionary<Druid, DbKey> ();
				}

				if (this.parentRowKeys.ContainsKey (entityId))
				{
					throw new System.InvalidOperationException ("RowKeys cannot be modified");
				}

				this.parentRowKeys[entityId] = value;
			}
		}

		private readonly AbstractEntity entity;
		private Dictionary<Druid, DbKey> parentRowKeys;
	}
}
