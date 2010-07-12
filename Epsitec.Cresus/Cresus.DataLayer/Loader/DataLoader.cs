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


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target)
		{
			return this.DataBrowser.GetReferencers (target);
		}


		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId)
		{
			// TODO Call DataBrowser
			throw new System.NotImplementedException ();
		}


		public AbstractEntity ResolveEntity(EntityData entityData)
		{
			Druid rootEntityId = this.EntityContext.GetRootEntityId (entityData.LoadedEntityId);
			AbstractEntity entity = this.DataContext.FindEntity (entityData.Key, entityData.ConcreteEntityId, rootEntityId);

			if (entity == null)
			{
				return this.DeserializeEntity (entityData);
			}
			
			return entity;
		}


		private AbstractEntity DeserializeEntity(EntityData entityData)
		{
			AbstractEntity entity = this.DataContext.CreateEntity (entityData.ConcreteEntityId);

			EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (entity);
			this.DataContext.DefineRowKey (mapping, entityData.Key);

			using (entity.DefineOriginalValues ())
			{
				List<Druid> entityIds = this.EntityContext.GetInheritedEntityIds (entityData.ConcreteEntityId).ToList ();

				foreach (Druid currentId in entityIds.TakeWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocal (entity, currentId, entityData.Key);
				}

				foreach (Druid currentId in entityIds.SkipWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocal (entity, entityData, currentId);
				}
			}

			return entity;
		}


		private void DeserializeEntityLocal(AbstractEntity entity, Druid entityId, DbKey rowKey)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				object proxy;

				switch (field.Relation)
				{
					case FieldRelation.None:
					{
						proxy = new ValueFieldProxy (this.DataContext, entity, field.CaptionId);
						break;
					}
					case FieldRelation.Reference:
					{
						proxy = new EntityFieldProxy (this.DataContext, entity, field.CaptionId);
						break;
					}
					case FieldRelation.Collection:
					{
						proxy = new EntityCollectionFieldProxy (this.DataContext, entity, field.CaptionId);
						break;
					}
					default:
					{
						throw new System.NotImplementedException ();
					}
				}

				entity.InternalSetValue (field.Id, proxy);
			}
		}


		private void DeserializeEntityLocal(AbstractEntity entity, EntityData entityData, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:
					{
						Druid leafEntityId = entity.GetEntityStructuredTypeId ();

						object valueData = entityData.ValueData[field.CaptionId];
						object value = this.GetFieldValue (leafEntityId, field, valueData);
							
						entity.InternalSetValue (field.Id, value);

						break;
					}
					case FieldRelation.Reference:
					{
						DbKey targetKey = entityData.ReferenceData[field.CaptionId];
						object target = new EntityKeyProxy (this.DataContext, field.TypeId, targetKey);
							
						entity.InternalSetValue (field.Id, target);

						break;
					}
					case FieldRelation.Collection:
					{
						IList targets = entity.InternalGetFieldCollection (field.Id);

						foreach (DbKey targetKey in entityData.CollectionData[field.CaptionId])
						{
							object target = new EntityKeyProxy (this.DataContext, field.TypeId, targetKey);

							targets.Add (target);
						}

						break;
					}
					default:
					{
						throw new System.NotImplementedException ();
					}
				}
			}
		}


		public object GetFieldValue(AbstractEntity entity, Druid fieldId)
		{
			// TODO Call DataBrowser to get the value.
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
					string columnName = this.SchemaEngine.GetDataColumnName (field.Id);

					DbTable dbTable = this.SchemaEngine.GetTableDefinition (localEntityId);
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
