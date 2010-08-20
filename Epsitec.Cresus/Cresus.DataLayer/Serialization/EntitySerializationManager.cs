using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Proxies;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Serialization
{

	
	// TODO Comment this class.
	// Marc
	

	internal sealed class EntitySerializationManager
	{


		public EntitySerializationManager(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}



		public EntityData Serialize(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			DbKey rowKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid loadedEntityId = entity.GetEntityStructuredTypeId ();

			ValueData valueData = this.GetValueData (entity);
			ReferenceData referenceData = this.GetReferenceData (entity);
			CollectionData collectionData = this.GetCollectionData (entity);
			
			return new EntityData (rowKey, leafEntityId, loadedEntityId, valueData, referenceData, collectionData);
		}


		private ValueData GetValueData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						 where field.Relation == FieldRelation.None
						 where field.Source == FieldSource.Value
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


		private ReferenceData GetReferenceData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						 where field.Relation == FieldRelation.Reference
						 where field.Source == FieldSource.Value
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

		
		private CollectionData GetCollectionData(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fields = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						 where field.Relation == FieldRelation.Collection
						 where field.Source == FieldSource.Value
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
		

		public AbstractEntity Deserialize(EntityData data)
		{
			data.ThrowIfNull ("data");
			
			Druid leafEntityId = data.LeafEntityId;
			DbKey rowKey = data.RowKey;

			AbstractEntity entity = this.DataContext.CreateEntity (leafEntityId);

			this.DataContext.DefineRowKey (entity, rowKey);

			using (entity.DefineOriginalValues ())
			{
				using (entity.DisableEvents ())
				{
					List<Druid> entityIds = this.EntityContext.GetInheritedEntityIds (leafEntityId).ToList ();

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

			return entity;
		}


		private void DeserializeEntityLocalWithProxies(AbstractEntity entity, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				object proxy = this.GetProxyForField (entity, field);

				entity.InternalSetValue (field.Id, proxy);
			}
		}


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

		private void DeserializeEntityLocal(AbstractEntity entity, EntityData entityData, Druid localEntityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId))
			{
				this.DeserializeEntityLocalField (entity, entityData, field);
			}
		}


		private void DeserializeEntityLocalField(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.None:
					this.DeserializeEntityLocalFieldValue (entity, entityData, field);
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


		private void DeserializeEntityLocalFieldValue(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			object fieldValue = entityData.ValueData[fieldId];

			entity.InternalSetValue (field.Id, fieldValue);
		}


		private void DeserializeEntityLocalFieldReference(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			DbKey? targetKey = entityData.ReferenceData[fieldId];

			if (targetKey.HasValue)
			{
				EntityKey entityKey = EntityKey.CreateNormalizedEntityKey (this.EntityContext, field.TypeId, targetKey.Value);

				object target = new KeyedReferenceFieldProxy (this.DataContext, entity, fieldId, entityKey);

				entity.InternalSetValue (fieldId.ToResourceId (), target);
			}
		}


		private void DeserializeEntityLocalFieldCollection(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			Druid fieldId = field.CaptionId;
			List<DbKey> collectionKeys = entityData.CollectionData[fieldId];

			if (collectionKeys.Any ())
			{
				var targetKeys = from rowKey in collectionKeys
								 select EntityKey.CreateNormalizedEntityKey (this.EntityContext, field.TypeId, rowKey);

				object target = new KeyedCollectionFieldProxy (this.DataContext, entity, fieldId, targetKeys);

				entity.InternalSetValue (fieldId.ToResourceId (), target);
			}
		}


	}


}
