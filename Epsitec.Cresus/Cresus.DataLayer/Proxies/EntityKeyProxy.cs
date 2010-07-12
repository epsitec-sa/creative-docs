//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>EntityKeyProxy</c> class is used as a placeholder for an <see cref="AbstractEntity"/>.
	/// It contains the <see cref="DbKey"/> and the <see cref="Druid"/> as a reference to the
	/// <see cref="AbstractEntity"/> that it represents.
	/// </summary>
	/// <remarks>
	/// There is some consistency issue with this proxy. The problem is that if one is builded, and
	/// the <see cref="AbstractEntity"/> to which it refers is removed from the database, there will
	/// be some "undefined" problems. Use this class with caution.
	/// </remarks>
	internal class EntityKeyProxy : IEntityProxy
	{


		/// <summary>
		/// Builds a new <c>EntityKeyProxy</c> which represents the <see cref="AbstractEntity"/> with
		/// the <see cref="Druid"/> <paramref name="entityId"/> and the <see cref="DbKey"/>
		/// <paramref name="rowKey"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible the <see cref="AbstractEntity"/>.</param>
		/// <param name="entityId">The id of the <see cref="AbstractEntity"></see>.</param>
		/// <param name="rowKey">The row key of the <see cref="AbstractEntity"></see> in the data base.</param>
		public EntityKeyProxy(DataContext dataContext, Druid entityId, DbKey rowKey)
		{
			this.dataContext = dataContext;
			this.entityId = entityId;
			this.rowKey = rowKey;
		}

		
		#region IEntityProxy Members


		/// <summary>
		/// Gets the real instance to be used when reading on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		public object GetReadEntityValue(IValueStore store, string id)
		{
			object value = this.PromoteToRealInstance ();
			store.SetValue (id, value, ValueStoreSetMode.Default);

			return value;
		}


		/// <summary>
		/// Gets the real instance to be used when writing on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		public object GetWriteEntityValue(IValueStore store, string id)
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
		public object PromoteToRealInstance()
		{
			return this.dataContext.DataLoader.ResolveEntity (this.rowKey, this.entityId);
		}


		#endregion


		/// <summary>
		/// The <see cref="DataContext"/> responsible of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private readonly DataContext dataContext;


		/// <summary>
		/// The <see cref="Druid"/> representing the type of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private readonly Druid entityId;


		/// <summary>
		/// The <see cref="DbKey"/> of the row in the database to which the <see cref="AbstractEntity"/>
		/// of this instance corresponds.
		/// </summary>
		private readonly DbKey rowKey;


	}


}
