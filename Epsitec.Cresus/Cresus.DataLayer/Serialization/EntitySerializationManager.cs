using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Serialization
{

	
	/// <summary>
	/// The <c>EntitySerializationManager</c> is used to convert <see cref="AbstractEntity"/> to
	/// <see cref="EntityData"/> back and forth.
	/// </summary>
	internal sealed class EntitySerializationManager
	{


		/// <summary>
		/// Builds a new <c>EntitySerializationManager</c> associated with a given <see cref="DataContext"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> associated with this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public EntitySerializationManager(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> associated with this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		private EntityTypeEngine TypeEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}


		/// <summary>
		/// Serializes the given <see cref="AbstractEntity"/> to an equivalent <see cref="EntityData"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to serialize.</param>
		/// <param name="logId">The value that must be used as the log id.</param>
		/// <returns>The serialized <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public EntityData Serialize(AbstractEntity entity, long logId)
		{
			entity.ThrowIfNull ("entity");
			
			DbKey rowKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid loadedEntityId = entity.GetEntityStructuredTypeId ();

			ValueData valueData = this.GetValueData (entity);
			ReferenceData referenceData = this.GetReferenceData (entity);
			CollectionData collectionData = this.GetCollectionData (entity);
			
			return new EntityData (rowKey, leafEntityId, loadedEntityId, logId, valueData, referenceData, collectionData);
		}


		/// <summary>
		/// Gets the <see cref="ValueData"/> that represents all the values of a given
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="ValueData"/> to get.</param>
		/// <returns>The <see cref="ValueData"/>.</returns>
		private ValueData GetValueData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.TypeEngine.GetValueFields (leafEntityId)
						 let fieldId = field.CaptionId
						 let fieldValue = entity.GetField<object> (fieldId.ToResourceId ())
						 select new
						 {
							 Id = fieldId,
							 Value = fieldValue
						 };

			ValueData valueData = new ValueData ();

			foreach (var field in fields)
			{
				valueData[field.Id] = field.Value;
			}

			return valueData;
		}


		/// <summary>
		/// Gets the <see cref="ReferenceData"/> that represents all the references of a given
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="ReferenceData"/> to get.</param>
		/// <returns>The <see cref="ReferenceData"/>.</returns>
		private ReferenceData GetReferenceData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.TypeEngine.GetReferenceFields(leafEntityId)
						 let fieldId = field.CaptionId
						 let fieldTarget = entity.GetField<AbstractEntity> (fieldId.ToResourceId ())
						 where fieldTarget != null
						 where this.DataContext.IsPersistent (fieldTarget)
						 let fieldTargetKey = this.DataContext.GetNormalizedEntityKey(fieldTarget).Value.RowKey
						 select new
						 {
							 Id = field.CaptionId,
							 TargetKey = fieldTargetKey
						 };

			ReferenceData referenceData = new ReferenceData ();

			foreach (var field in fields)
			{
				referenceData[field.Id] = field.TargetKey;
			}

			return referenceData;
		}


		/// <summary>
		/// Gets the <see cref="CollectionData"/> that represents all the collections of a given
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="CollectionData"/> to get.</param>
		/// <returns>The <see cref="CollectionData"/>.</returns>
		private CollectionData GetCollectionData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.TypeEngine.GetCollectionFields(leafEntityId)
						 let fieldId = field.CaptionId
						 let fieldTargets = entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
						 let fieldTargetKeys = new List<DbKey>
						 (
							from t in fieldTargets
							where t != null
							where this.DataContext.IsPersistent(t)
							select this.DataContext.GetNormalizedEntityKey (t).Value.RowKey
						 )
						 where fieldTargetKeys.Count > 0
						 select new
						 {
							 Id = field.CaptionId,
							 TargetKeys = fieldTargetKeys
						 };


			CollectionData collectionData = new CollectionData ();

			foreach (var field in fields)
			{
				collectionData[field.Id].AddRange (field.TargetKeys);
			}

			return collectionData;
		}
		

		/// <summary>
		/// Builds a new <see cref="AbstractEntity"/> whose type and data are defined by an
		/// <see cref="EntityData"/>.
		/// </summary>
		/// <param name="data">The <see cref="EntityData"/> containing the type and data.</param>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="data"/> is <c>null</c>.</exception>
		public AbstractEntity Deserialize(EntityData data)
		{
			data.ThrowIfNull ("data");
			
			Druid leafEntityId = data.LeafEntityId;
			DbKey rowKey = data.RowKey;

			AbstractEntity entity = this.DataContext.EntityContext.CreateEmptyEntity (leafEntityId);

			this.DataContext.DefineRowKey (entity, rowKey);

			this.DeserializeEntityFields (data, entity);

			return entity;
		}


		/// <summary>
		/// Clears the data of the given <see cref="AbstractEntity"/> and then deserializes the given
		/// <see cref="EntityData"/> into it.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose data to set.</param>
		/// <param name="data">The <see cref="EntityData"/> to put in the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="data"/> is <c>null</c>.</exception>
		public void Deserialize(AbstractEntity entity, EntityData data)
		{
			entity.ThrowIfNull ("entity");
			data.ThrowIfNull ("data");

			entity.ResetValueStores ();

			this.DeserializeEntityFields (data, entity);
		}

		
		/// <summary>
		/// Deserializes the given <see cref="EntityData"/> in the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="data">The <see cref="EntityData"/> to deserialize.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> in which to deserialize the data.</param>
		private void DeserializeEntityFields(EntityData data, AbstractEntity entity)
		{
			Druid localEntityId = entity.GetEntityStructuredTypeId ();
			var entityIds = this.TypeEngine.GetBaseTypes (localEntityId)
				.Select (t => t.CaptionId)
				.ToList ();

			using (entity.UseSilentUpdates ()) // New. Is that requested?
			using (entity.DefineOriginalValues ())
			using (entity.DisableEvents ())
			using (entity.DisableReadOnlyChecks ())
			{
				foreach (Druid currentId in entityIds.TakeWhile (id => id != data.LoadedEntityId))
				{
					this.DeserializeEntityLocalWithProxies (entity, currentId);
				}

				foreach (Druid currentId in entityIds.SkipWhile (id => id != data.LoadedEntityId))
				{
					this.DeserializeEntityLocal (entity, data, currentId);
				}
			}
		}


		/// <summary>
		/// Set all the fields of a single entity type of an <see cref="AbstractEntity"/> to the
		/// appropriate proxy.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose fields to set.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type whose fields to set.</param>
		private void DeserializeEntityLocalWithProxies(AbstractEntity entity, Druid localEntityId)
		{
			foreach (StructuredTypeField field in this.TypeEngine.GetLocalFields (localEntityId))
			{
				this.InsertProxyForField (entity, field);
			}
		}


		/// <summary>
		/// Sets the value of the given field of the given <see cref="AbstractEntity"/> to the
		/// appropriate proxy.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to set.</param>
		/// <param name="field">The field to set.</param>
		private void InsertProxyForField(AbstractEntity entity, StructuredTypeField field)
		{
			object proxy = this.GetProxyForField (entity, field);

			entity.InternalSetValue (field.Id, proxy, ValueStoreSetMode.ShortCircuit);
		}


		/// <summary>
		/// Gets the proxy that must be used for a given field of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> containing the field.</param>
		/// <param name="field">The field for which to create a proxy.</param>
		/// <returns>The proxy object.</returns>
		private object GetProxyForField(AbstractEntity entity, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					return new ValueFieldProxy (this.DataContext, entity, field.CaptionId);

				case FieldRelation.Reference:
					return new ReferenceFieldProxy (this.DataContext, entity, field.CaptionId);

				case FieldRelation.Collection:
					return new CollectionFieldProxy (this.DataContext, entity, field.CaptionId);

				default:
					throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Set all the fields of a single entity type of an <see cref="AbstractEntity"/> to the
		/// appropriate value.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose fields to set.</param>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the fields.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type whose fields to set.</param>
		private void DeserializeEntityLocal(AbstractEntity entity, EntityData entityData, Druid localEntityId)
		{
			foreach (StructuredTypeField field in this.TypeEngine.GetLocalFields (localEntityId))
			{
				this.DeserializeEntityLocalField (entity, entityData, field);
			}
		}


		/// <summary>
		/// Sets the appropriate value to a given field of a given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to set.</param>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the field.</param>
		/// <param name="field">The field whose value to set.</param>
		private void DeserializeEntityLocalField(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:

					if (entityData.ValueData.ContainsValue (field.CaptionId))
					{
						this.DeserializeEntityLocalFieldValue (entity, entityData, field);
					}
					else
					{
						this.InsertProxyForField (entity, field);
					}

					break;

				case FieldRelation.Reference:
					this.DeserializeEntityLocalFieldReference (entity, entityData, field);
					break;

				case FieldRelation.Collection:
					this.DeserializeEntityLocalFieldCollection (entity, entityData, field);
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}


		/// <summary>
		/// Sets the value to a given value field of a given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to set.</param>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the field.</param>
		/// <param name="field">The field whose value to set.</param>
		private void DeserializeEntityLocalFieldValue(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			object fieldValue = entityData.ValueData[fieldId];

			if (fieldValue != null)
			{
				entity.InternalSetValue (field.Id, fieldValue, ValueStoreSetMode.ShortCircuit);
			}
		}


		/// <summary>
		/// Sets the value to a given reference field of a given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to set.</param>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the field.</param>
		/// <param name="field">The field whose value to set.</param>
		private void DeserializeEntityLocalFieldReference(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			DbKey? targetKey = entityData.ReferenceData[fieldId];

			if (targetKey.HasValue)
			{
				EntityKey entityKey = EntityKey.CreateNormalizedEntityKey (this.TypeEngine, field.TypeId, targetKey.Value);

				object target = new KeyedReferenceFieldProxy (this.DataContext, entity, fieldId, entityKey);

				entity.InternalSetValue (fieldId.ToResourceId (), target, ValueStoreSetMode.ShortCircuit);
			}
		}


		/// <summary>
		/// Sets the value to a given collection field of a given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to set.</param>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the field.</param>
		/// <param name="field">The field whose value to set.</param>
		private void DeserializeEntityLocalFieldCollection(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			List<DbKey> collectionKeys = entityData.CollectionData[fieldId];

			if (collectionKeys.Any ())
			{
				var targetKeys = from rowKey in collectionKeys
								 select EntityKey.CreateNormalizedEntityKey (this.TypeEngine, field.TypeId, rowKey);

				object target = new KeyedCollectionFieldProxy (this.DataContext, entity, fieldId, targetKeys);

				entity.InternalSetValue (fieldId.ToResourceId (), target, ValueStoreSetMode.ShortCircuit);
			}
		}


	}


}
