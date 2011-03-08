using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>ReferenceFieldProxy</c> class is used as a placeholder for an <see cref="AbstractEntity"/>.
	/// The <see cref="AbstractEntity"/> that it represents is defined by another
	/// <see cref="AbstractEntity"/> and the <see cref="Druid"/> of a field of this
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ReferenceFieldProxy : AbstractFieldProxy, IEntityProxy
	{


		/// <summary>
		/// Builds a new <c>ReferenceFieldProxy</c> which represents the <see cref="AbstractEntity"/> that
		/// is the one referenced by the field with the <see cref="Druid"/> <paramref name="fieldId"/>
		/// of <paramref name="entity"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this instance.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="dataContext"/> is null.
		/// If <paramref name="entity"/> is null.
		/// </exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="dataContext"/>.</exception>
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>ReferenceFieldProxy</c>.</exception>
		public ReferenceFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
			: base (dataContext, entity, fieldId)
		{
		}


		/// <summary>
		/// Gets the kind of <see cref="FieldRelation"/> of the field used by this instance.
		/// </summary>
		protected override FieldRelation FieldRelation
		{
			get
			{
				return FieldRelation.Reference;
			}
		}


		#region IEntityProxy Members


		/// <summary>
		/// Gets the real instance to be used when reading on this proxy.
		/// </summary>
		/// <param name="store">The value store.</param>
		/// <param name="id">The value id.</param>
		/// <returns>The real instance to be used.</returns>
		public object GetReadEntityValue(Common.Types.IValueStore store, string id)
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
		public object GetWriteEntityValue(Common.Types.IValueStore store, string id)
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
		public bool DiscardWriteEntityValue(Common.Types.IValueStore store, string id, ref object value)
		{
			return false;
		}


		/// <summary>
		/// Promotes the proxy to its real instance.
		/// </summary>
		/// <returns>The real instance.</returns>
		public virtual object PromoteToRealInstance()
		{
			object result = this.DataContext.DataLoader.ResolveReferenceField (this.Entity, this.FieldId);

			return result ?? UndefinedValue.Value;
		}


		#endregion


	}


}
