//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	/// <summary>
	/// The <c>EntityDataProxy</c> class automatically loads entities on demand.
	/// </summary>
	public class EntityKeyProxy : IEntityProxy
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataProxy"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="rowKey">The row key.</param>
		/// <param name="entityId">The entity id.</param>
		public EntityKeyProxy(DataContext context, DbKey rowKey, Druid entityId)
		{
			this.context = context;
			this.rowKey = rowKey;
			this.entityId = entityId;
		}

		#region IEntityProxy Members

		/// <summary>
		/// Gets the real instance to be used when reading on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		object IEntityProxy.GetReadEntityValue(IValueStore store, string id)
		{
			return this.ResolveEntity (store, id);
		}

		/// <summary>
		/// Gets the real instance to be used when writing on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		object IEntityProxy.GetWriteEntityValue(IValueStore store, string id)
		{
			return this;
		}

		/// <summary>
		/// Checks if the write to the specified entity value should proceed
		/// normally or be discarded completely.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the value should be discarded; otherwise, <c>false</c>.
		/// </returns>
		public bool DiscardWriteEntityValue(IValueStore store, string id, ref object value)
		{
			return false;
		}


		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		object IEntityProxy.PromoteToRealInstance()
		{
			return this.PromoteToRealInstance ();
		}

		#endregion

		/// <summary>
		/// Resolves the entity by promoting it and the storing it back into
		/// the value store.
		/// </summary>
		/// <param name="store">The store.</param>
		/// <param name="id">The id.</param>
		/// <returns>The real instance.</returns>
		private object ResolveEntity(IValueStore store, string id)
		{
			object value = this.PromoteToRealInstance ();
			store.SetValue (id, value, ValueStoreSetMode.Default);
			return value;
		}

		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		private object PromoteToRealInstance()
		{
			return this.context.InternalResolveEntity (this.rowKey, this.entityId, EntityResolutionMode.Load);
		}

		readonly DataContext context;
		readonly DbKey rowKey;
		readonly Druid entityId;
	}
}
