using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections;
using System.Collections.Generic;

using System.Linq;
using Epsitec.Cresus.DataLayer.Proxies;


namespace Epsitec.Cresus.DataLayer.Loader
{

	
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


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
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


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.SchemaEngine;
			}
		}


		public IEnumerable<T> GetByExample<T>(T example) where T : AbstractEntity
		{
			Request request = new Request ()
			{
				RootEntity = example,
				RequestedEntity = example,
				ResolutionMode = ResolutionMode.Database,
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
				T entity = this.ResolveEntity (entityData, request.ResolutionMode) as T;

				if (entity != null)
				{
					yield return entity;
				}
			}
		}


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target, ResolutionMode resolutionMode = ResolutionMode.Database)
		{
			if (this.DataContext.GetEntityKey (target) != null)
			{
				foreach (Druid targetEntityId in this.EntityContext.GetInheritedEntityIds (target.GetEntityStructuredTypeId ()))
				{
					foreach (EntityFieldPath sourceFieldPath in this.DbInfrastructure.GetSourceReferences (targetEntityId))
					{
						foreach (System.Tuple<AbstractEntity, EntityFieldPath> item in this.GetReferencers (sourceFieldPath, target, resolutionMode))
						{
							yield return item;
						}
					}
				}
			}
		}


		private IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(EntityFieldPath sourceFieldPath, AbstractEntity target, ResolutionMode resolutionMode)
		{
			Druid sourceEntityId = sourceFieldPath.EntityId;
			string sourceFieldId = sourceFieldPath.Fields.First ();

			AbstractEntity example = this.EntityContext.CreateEmptyEntity (sourceEntityId);
			StructuredTypeField field = this.EntityContext.GetStructuredTypeField (example, sourceFieldId);

			using (example.DefineOriginalValues ())
			{
				if (field.Relation == FieldRelation.Reference)
				{
					example.SetField<object> (field.Id, target);
				}
				else if (field.Relation == FieldRelation.Collection)
				{
					example.InternalGetFieldCollection (field.Id).Add (target);
				}
			}

			Request request = new Request ()
			{
				RootEntity = example,
				ResolutionMode = resolutionMode,
			};

			return this.GetByRequest<AbstractEntity> (request).Select (sourceEntity => System.Tuple.Create (sourceEntity, sourceFieldPath));
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


		public AbstractEntity ResolveEntity(EntityData entityData, ResolutionMode resolutionMode = ResolutionMode.Database)
		{
			EntityKey entityKey = entityData.EntityKey;

			AbstractEntity entity = this.DataContext.GetEntity (entityKey);

			if (entity == null && resolutionMode == ResolutionMode.Database)
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
					return new EntityFieldProxy (this.DataContext, entity, field.CaptionId);
					
				case FieldRelation.Collection:
					return new EntityCollectionFieldProxy (this.DataContext, entity, field.CaptionId);
					
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
