//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>EntityKeyProxy</c> class is used as a placeholder for an <see cref="AbstractEntity"/>.
	/// It contains an <see cref="EntityKey"/> as the reference to the <see cref="AbstractEntity"/>
	/// that it represents.
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
		/// the <paramref name="entityKey"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible the <see cref="AbstractEntity"/>.</param>
		/// <param name="entityKey">The <see cref="EntityKey"/> describing the entity to load.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is null.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entityKey"/> is empty.</exception>
		public EntityKeyProxy(DataContext dataContext, EntityKey entityKey)
		{
			dataContext.ThrowIfNull ("dataContext");
			entityKey.ThrowIf (key => key.IsEmpty, "entityKey cannot be empty");
			
			this.dataContext = dataContext;
			this.entityKey = entityKey;
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
			object result = this.dataContext.ResolveEntity (this.entityKey);

			if (result == null)
			{
				result = UndefinedValue.Value;
			}

			return result;
		}


		#endregion


		/// <summary>
		/// The <see cref="DataContext"/> responsible of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private readonly DataContext dataContext;


		/// <summary>
		/// The <see cref="EntityKey"/> representing the entity of this proxy.
		/// </summary>
		private readonly EntityKey entityKey;


	}


}
