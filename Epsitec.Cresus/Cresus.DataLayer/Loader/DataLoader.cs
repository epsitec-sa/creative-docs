using Epsitec.Common.Support;

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

	// TODO I'm sure that the DataLoader and the DataBrowser could be merged together or that their
	// separation could be better. Therefore, it would be nice to kind of merge these classes.
	// Marc
	internal sealed class DataLoader
	{


		public DataLoader(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.DataBrowser = new DataBrowser (dataContext);
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private DataBrowser DataBrowser
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
			return this.DataBrowser.GetByExample<T> (example);
		}


		public IEnumerable<T> GetByRequest<T>(Request request) where T : AbstractEntity
		{
			return this.DataBrowser.GetByRequest<T> (request);
		}


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target, ResolutionMode resolutionMode = ResolutionMode.Database)
		{
			return this.DataBrowser.GetReferencers (target, resolutionMode);
		}


		public AbstractEntity ResolveEntity(DbKey dbKey, Druid entityId)
		{
			AbstractEntity entity = EntityClassFactory.CreateEmptyEntity (entityId);

			Request request = new Request ()
			{
				RootEntity = entity,
				RootEntityKey = dbKey,
			};

			return this.GetByRequest<AbstractEntity> (request).FirstOrDefault ();
		}


		public AbstractEntity ResolveEntity(EntityData entityData, ResolutionMode resolutionMode = ResolutionMode.Database)
		{
			Druid leafEntityId = entityData.LeafEntityId;
			AbstractEntity entity = this.DataContext.FindEntity (entityData.Key, leafEntityId);

			if (entity == null && resolutionMode == ResolutionMode.Database)
			{
				entity = this.DeserializeEntity (entityData);
			}

			return entity;
		}


		private AbstractEntity DeserializeEntity(EntityData entityData)
		{
			AbstractEntity entity = this.DataContext.CreateEntity (entityData.LeafEntityId);

			EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (entity);
			this.DataContext.DefineRowKey (mapping, entityData.Key);

			using (entity.DefineOriginalValues ())
			{
				List<Druid> entityIds = this.EntityContext.GetInheritedEntityIds (entityData.LeafEntityId).ToList ();

				foreach (Druid currentId in entityIds.TakeWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocal (entity, currentId);
				}

				foreach (Druid currentId in entityIds.SkipWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocal (entity, entityData, currentId);
				}
			}

			return entity;
		}


		private void DeserializeEntityLocal(AbstractEntity entity, Druid entityId)
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
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			object valueData = entityData.ValueData[field.CaptionId];
			object value = this.GetFieldValue (leafEntityId, field, valueData);

			entity.InternalSetValue (field.Id, value);
		}


		private void DeserializeEntityLocalFieldReference(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			DbKey targetKey = entityData.ReferenceData[field.CaptionId];
			object target = new EntityKeyProxy (this.DataContext, field.TypeId, targetKey);

			entity.InternalSetValue (field.Id, target);
		}


		private void DeserializeEntityLocalFieldCollection(AbstractEntity entity, EntityData entityData, StructuredTypeField field)
		{
			IList targets = entity.InternalGetFieldCollection (field.Id);

			foreach (DbKey targetKey in entityData.CollectionData[field.CaptionId])
			{
				object target = new EntityKeyProxy (this.DataContext, field.TypeId, targetKey);

				targets.Add (target);
			}
		}


		public object GetFieldValue(AbstractEntity entity, Druid fieldId)
		{
			// TODO Implement this method with a call to the DataBrowser.
			// Marc

			throw new System.NotImplementedException ();
		}


		private object GetFieldValue(Druid leafEntityId, StructuredTypeField field, object value)
		{
			object newValue = value;

			if (newValue != System.DBNull.Value)
			{
				IStringType stringType = field.Type as IStringType;

				if (stringType != null)
				{
					if (stringType.UseFormattedText)
					{
						newValue = FormattedText.CastToFormattedText (newValue);
					}
				}
				else
				{
					Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, field.CaptionId);
					string columnName = this.SchemaEngine.GetEntityColumnName (field.Id);

					DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
					DbColumn dbColumn = dbTable.Columns[columnName];
									
					//	The conversion is a two step process:
					//	1. Convert from an ADO.NET type to a simple type (i.e. almost all numbers map to decimal)
					//	2. Convert from the simple type to the expected field type

					newValue = this.ConvertFromInternal (dbColumn, newValue);
					InvariantConverter.Convert (newValue, field, out newValue);
				}
			}

			return newValue;
		}


		private object ConvertFromInternal(DbColumn dbColumn, object value)
		{
			if (value == System.DBNull.Value)
			{
				return value;
			}
			else
			{
				DbSimpleType typeDef = dbColumn.Type.SimpleType;
				DbNumDef numDef = dbColumn.Type.NumDef;

				return TypeConverter.ConvertToSimpleType (value, typeDef, numDef);
			}
		}


	}


}
