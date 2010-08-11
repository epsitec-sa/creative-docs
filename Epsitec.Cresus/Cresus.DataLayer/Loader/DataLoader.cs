using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader.Data;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{

	
	// TODO Comment this class
	// Marc


	internal sealed class DataLoader
	{


		public DataLoader(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.LoaderQueryGenerator = new LoaderQueryGenerator (dataContext);
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private LoaderQueryGenerator LoaderQueryGenerator
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


		public IEnumerable<T> GetByExample<T>(T example) where T : AbstractEntity
		{
			Request request = new Request ()
			{
				RootEntity = example,
				RequestedEntity = example,
			};

			return this.GetByRequest<T> (request);
		}


		public IEnumerable<T> GetByRequest<T>(Request request) where T : AbstractEntity
		{
			request.ThrowIfNull ("request");
			request.RootEntity.ThrowIfNull ("request.RootEntity");
			request.RequestedEntity.ThrowIfNull ("request.RequestedEntity");
			
			foreach (EntityData entityData in this.LoaderQueryGenerator.GetEntitiesData (request))
			{
				T entity = this.ResolveEntity (entityData) as T;

				if (entity != null)
				{
					yield return entity;
				}
			}
		}


		public AbstractEntity ResolveEntity(EntityKey entityKey)
		{
			AbstractEntity entity = EntityClassFactory.CreateEmptyEntity (entityKey.EntityId);

			Request request = new Request ()
			{
				RootEntity = entity,
				RootEntityKey = entityKey.RowKey,
			};

			return this.GetByRequest<AbstractEntity> (request).FirstOrDefault ();
		}


		public AbstractEntity ResolveEntity(EntityData entityData)
		{
			EntityKey entityKey = entityData.EntityKey;

			AbstractEntity entity = this.DataContext.GetEntity (entityKey);

			if (entity == null)
			{
				entity = this.DeserializeEntity (entityData);
			}

			return entity;
		}


		private AbstractEntity DeserializeEntity(EntityData entityData)
		{
			Druid leafEntityId = entityData.EntityKey.EntityId;
			DbKey rowKey = entityData.EntityKey.RowKey;
			
			AbstractEntity entity = this.DataContext.CreateEntity (leafEntityId);

			this.DataContext.DefineRowKey (entity, rowKey);

			using (entity.DefineOriginalValues ())
			{
				using (entity.DisableEvents ())
				{
					List<Druid> entityIds = this.EntityContext.GetInheritedEntityIds (leafEntityId).ToList ();

					foreach (Druid currentId in entityIds.TakeWhile (id => id != entityData.LoadedEntityId))
					{
						this.DeserializeEntityLocalWithProxies (entity, currentId);
					}

					foreach (Druid currentId in entityIds.SkipWhile (id => id != entityData.LoadedEntityId))
					{
						this.DeserializeEntityLocal (entity, entityData, currentId);
					}
				}
			}

			return entity;
		}


		private void DeserializeEntityLocalWithProxies(AbstractEntity entity, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				object proxy = GetProxyForField (entity, field);

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
			DbKey? targetKey = entityData.ReferenceData[field.CaptionId];

			if (targetKey.HasValue)
			{
				EntityKey entityKey = new EntityKey (field.TypeId, targetKey.Value);

				object target = new EntityKeyProxy (this.DataContext, entityKey);

				entity.InternalSetValue (field.Id, target);
			}
		}


		private void DeserializeEntityLocalFieldCollection(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			List<DbKey> collectionKeys = entityData.CollectionData[field.CaptionId];

			if (collectionKeys.Any ())
			{
				IList targets = entity.InternalGetFieldCollection (field.Id);

				foreach (DbKey targetKey in collectionKeys)
				{
					EntityKey entityKey = new EntityKey (field.TypeId, targetKey);

					object target = new EntityKeyProxy (this.DataContext, entityKey);

					targets.Add (target);
				}
			}
		}


		public object GetFieldValue(AbstractEntity entity, Druid fieldId)
		{
			return this.LoaderQueryGenerator.GetFieldValue (entity, fieldId);
		}


	}


}
