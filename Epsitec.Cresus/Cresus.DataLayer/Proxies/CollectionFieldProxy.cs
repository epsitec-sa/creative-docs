using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;


namespace Epsitec.Cresus.DataLayer.Proxies
{


	/// <summary>
	/// The <c>CollectionFieldProxy</c> class is used as a placeholder for a collection of
	/// <see cref="AbstractEntity"/>. The <see cref="AbstractEntity"/> that it represents are defined
	/// by another <see cref="AbstractEntity"/> and the <see cref="Druid"/> of a collection field of
	/// this <see cref="AbstractEntity"/>.
	/// </summary>
	internal class CollectionFieldProxy : AbstractFieldProxy, IEntityProxy
	{


		/// <summary>
		/// Builds a new <c>CollectionFieldProxy</c> which represents the <see cref="AbstractEntity"/>
		/// that are the ones referenced by the field with the <see cref="Druid"/> <paramref name="fieldId"/>
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
		/// <exception cref="System.ArgumentException">If the field given by <paramref name="fieldId"/> is not valid for the <c>CollectionFieldProxy</c>.</exception>
		public CollectionFieldProxy(DataContext dataContext, AbstractEntity entity, Druid fieldId)
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
				return FieldRelation.Collection;
			}
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
			EntityContext entityContext = this.Entity.GetEntityContext ();

			Druid leafEntityId = this.Entity.GetEntityStructuredTypeId ();
			string fieldId = this.FieldId.ToResourceId ();
			StructuredTypeField field = entityContext.GetEntityFieldDefinition (leafEntityId, fieldId);

			AbstractEntity rootExample = EntityClassFactory.CreateEmptyEntity (leafEntityId);
			AbstractEntity targetExample = EntityClassFactory.CreateEmptyEntity (field.TypeId);

			rootExample.GetFieldCollection<AbstractEntity> (fieldId).Add (targetExample);

			Request request = new Request ()
			{
				RootEntity = rootExample,
				RootEntityKey = this.DataContext.GetEntityKey (this.Entity).Value.RowKey,
				RequestedEntity = targetExample,
			};

			var targetsIn = this.DataContext.DataLoader.GetByRequest<AbstractEntity> (request);
			var targetsOut = new EntityCollection<AbstractEntity> (fieldId, this.Entity, false);

			using (this.Entity.UseSilentUpdates ())
			{
				using (this.Entity.DisableEvents ())
				{
					foreach (AbstractEntity target in targetsIn)
					{
						targetsOut.Add (target);
					}
				}
			}

			if (targetsOut.Count > 0)
			{
				return targetsOut;
			}
			else
			{
				return UndefinedValue.Value;
			}
		}


		#endregion


	}


}
