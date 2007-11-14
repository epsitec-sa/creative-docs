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
	/// <summary>
	/// The <c>EntityDataProxy</c> class automatically loads entities on demand.
	/// </summary>
	public class EntityDataProxy : IEntityProxy
	{
		public EntityDataProxy(DataContext context, DbKey rowKey, Druid entityId)
		{
			this.context = context;
			this.rowKey = rowKey;
			this.entityId = entityId;
		}

		#region IEntityProxy Members

		object IEntityProxy.GetReadEntityValue(IValueStore store, string id)
		{
			return this.ResolveEntity (store, id);
		}

		object IEntityProxy.GetWriteEntityValue(IValueStore store, string id)
		{
			return this;
		}

		#endregion

		private object ResolveEntity(IValueStore store, string id)
		{
			object value = this.context.ResolveEntity (this.rowKey, this.entityId);
			store.SetValue (id, value);
			return value;
		}

		readonly DataContext context;
		readonly DbKey rowKey;
		readonly Druid entityId;
	}
}
