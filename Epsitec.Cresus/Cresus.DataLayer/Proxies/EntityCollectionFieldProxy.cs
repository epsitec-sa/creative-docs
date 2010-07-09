using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections;
using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>EntityFieldProxy</c> class is used as a placeholder for a collection of
	/// <see cref="AbstractEntity"/>. The <see cref="AbstractEntity"/> that it represents are defined
	/// by another <see cref="AbstractEntity"/> and the <see cref="Druid"/> of a collection field of
	/// this <see cref="AbstractEntity"/>.
	/// </summary>
	internal class EntityCollectionFieldProxy : IEntityProxy
	{


		/// <summary>
		/// Builds a new <c>EntityCollectionFieldProxy</c> which represents the <see cref="AbstractEntity"/>
		/// that are the ones referenced by the field with the <see cref="Druid"/> <paramref name="fieldId"/>
		/// of <paramref name="entity"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"></see> responsible of <paramref name="entity"></paramref>.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this instance.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		public EntityCollectionFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
		{
			this.dataContext = dataContext;
			this.entity = entity;
			this.fieldId = fieldId;
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
			EntityContext entityContext = this.entity.GetEntityContext ();

			Druid entityId = this.entity.GetEntityStructuredTypeId ();
			Druid localEntityId = entityContext.GetLocalEntityId (entityId, this.fieldId);
			StructuredTypeField field = entityContext.GetEntityFieldDefinition (entityId, this.fieldId.ToResourceId ());

			IList targets = new EntityCollection<AbstractEntity> (this.fieldId.ToResourceId (), this.entity, false) as IList;
			
			using (this.entity.DefineOriginalValues ())
			{
				foreach (object item in this.dataContext.DataLoader.ReadFieldRelation (this.entity, localEntityId, field, EntityResolutionMode.Load))
				{
					targets.Add (item);
				}
			}

			return targets;
		}


		#endregion


		/// <summary>
		/// The <see cref="DataContext"/> responsible of the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private DataContext dataContext;


		/// <summary>
		/// The <see cref="AbstractEntity"/> that references the <see cref="AbstractEntity"/> of this
		/// instance.
		/// </summary>
		private AbstractEntity entity;


		/// <summary>
		/// The <see cref="Druid"/> of the field of the <see cref="AbstractEntity"/> that targets the
		/// <see cref="AbstractEntity"/> of this instance.
		/// </summary>
		private Druid fieldId;


	}


}
