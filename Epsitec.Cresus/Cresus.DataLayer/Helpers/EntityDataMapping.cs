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
			this.rowKey = new DbKey ();
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
				if (this.entityId.IsEmpty)
				{
					this.entityId = this.entity == null ? Druid.Empty : this.entity.GetEntityStructuredTypeId ();
				}

				return this.entityId;
			}
		}


		public DbKey RowKey
		{
			get
			{
				return this.rowKey;
			}
			set
			{
				if ((this.rowKey.IsEmpty) ||
					(this.rowKey.IsTemporary))
				{
					this.rowKey = value;
				}
				else
				{
					throw new System.InvalidOperationException ("RowKeys cannot be modified");
				}
			}
		}

		private readonly AbstractEntity entity;
		private DbKey rowKey;
		private Druid entityId;
	}
}
