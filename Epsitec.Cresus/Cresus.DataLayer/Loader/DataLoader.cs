using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Database;
using Epsitec.Common.Types;
using System.Collections.Generic;
using Epsitec.Cresus.DataLayer.Data;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal sealed class DataLoader
	{


		public DataLoader(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		public AbstractEntity ResolveEntity(EntityKey entityKey)
		{
			return this.ResolveEntity (entityKey.RowKey, entityKey.EntityId);
		}

		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId)
		{
			return this.InternalResolveEntity (rowKey, entityId, EntityResolutionMode.Load) as AbstractEntity;
		}

		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId, EntityResolutionMode mode)
		{
			return this.InternalResolveEntity (rowKey, entityId, mode) as AbstractEntity;
		}

		public TEntity ResolveEntity<TEntity>(DbKey rowKey)
			where TEntity : AbstractEntity, new ()
		{
			Druid entityId = EntityClassFactory.GetEntityId (typeof (TEntity));

			return this.ResolveEntity (rowKey, entityId) as TEntity;
		}

		internal AbstractEntity ResolveEntity(EntityData entityData, bool loadFromDatabase)
		{
			return this.InternalResolveEntity (entityData, loadFromDatabase);
		}

		


		internal AbstractEntity InternalResolveEntity(EntityData entityData, bool loadFromDatabase)
		{
			if (this.isDisposed)
			{
				throw new System.InvalidOperationException ("DataContext was disposed");
			}

			var rootEntityId = this.EntityContext.GetRootEntityId (entityData.LoadedEntityId);
			var entity = this.entityDataCache.FindEntity (entityData.Key, entityData.ConcreteEntityId, rootEntityId);

			if ((entity == null) &&
				(loadFromDatabase))
			{
				return this.InternalResolveEntityBasedOnDataLoadedFromDatabase (entityData);
			}
			else
			{
				return entity;
			}
		}


		private AbstractEntity InternalResolveEntityBasedOnDataLoadedFromDatabase(EntityData entityData)
		{
			var entity = this.EntityContext.CreateEmptyEntity (entityData.ConcreteEntityId);

			this.entityDataCache.DefineRowKey (this.GetEntityDataMapping (entity), entityData.Key);

			using (entity.DefineOriginalValues ())
			{
				Druid[] entityIds = this.EntityContext.GetInheritedEntityIds (entityData.ConcreteEntityId).ToArray ();

				foreach (Druid currentId in entityIds.TakeWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocalWithProxy (entity, currentId, entityData.Key);
				}

				foreach (Druid currentId in entityIds.SkipWhile (id => id != entityData.LoadedEntityId))
				{
					this.DeserializeEntityLocalWithReference (entity, entityData, currentId);
				}
			}

			return entity;
		}

		internal object InternalResolveEntity(DbKey rowKey, Druid entityId, EntityResolutionMode mode)
		{
			if (this.isDisposed)
			{
				throw new System.InvalidOperationException ("DataContext was disposed");
			}

			Druid baseEntityId = this.EntityContext.GetRootEntityId (entityId);
			AbstractEntity entity = this.entityDataCache.FindEntity (rowKey, entityId, baseEntityId);

			if (entity != null)
			{
				return entity;
			}

			switch (mode)
			{
				case EntityResolutionMode.Find:
					return null;

				case EntityResolutionMode.Load:
					return this.DeserializeEntity (rowKey, entityId);

				case EntityResolutionMode.DelayLoad:
					return new Proxies.EntityKeyProxy (this, entityId, rowKey);

				default:
					throw new System.NotImplementedException (string.Format ("Resolution mode {0} not implemented", mode));
			}
		}


		internal AbstractEntity DeserializeEntity(DbKey rowKey, Druid entityId)
		{
			Druid baseEntityId = this.EntityContext.GetRootEntityId (entityId);

			System.Data.DataRow dataRow = this.LoadDataRow (null, rowKey, baseEntityId);
			long typeValueId = (long) dataRow[Tags.ColumnInstanceType];
			Druid realEntityId = Druid.FromLong (typeValueId);
			AbstractEntity entity = this.EntityContext.CreateEmptyEntity (realEntityId);

			using (entity.DefineOriginalValues ())
			{
				this.DeserializeEntity (entity, realEntityId, rowKey);
			}

			this.dataLoadedEntities.Add (entity);

			return entity;
		}

		private void DeserializeEntity(AbstractEntity entity, Druid entityId, DbKey entityKey)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			System.Diagnostics.Debug.Assert (mapping.EntityId == entityId);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsEmpty);

			this.entityDataCache.DefineRowKey (mapping, entityKey);

			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entityId))
			{
				System.Data.DataRow dataRow = this.LoadDataRow (null, mapping.RowKey, currentId);

				this.DeserializeEntityLocal (entity, dataRow, currentId);
			}
		}


		private void DeserializeEntityLocal(AbstractEntity entity, System.Data.DataRow dataRow, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:
						this.ReadFieldValueFromDataRow (entity, field, dataRow);
						break;

					case FieldRelation.Reference:

						object target1 = new Proxies.EntityFieldProxy (this, entity, field.CaptionId);
						entity.InternalSetValue (field.Id, target1);

						break;

					case FieldRelation.Collection:

						object target2 = new Proxies.EntityCollectionFieldProxy (this, entity, field.CaptionId);
						entity.InternalSetValue (field.Id, target2);

						break;
				}
			}
		}


		private void DeserializeEntityLocalWithProxy(AbstractEntity entity, Druid entityId, DbKey rowKey)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:

						object value = new Proxies.ValueFieldProxy (this, entity, field.CaptionId);
						entity.InternalSetValue (field.Id, value);

						break;

					case FieldRelation.Reference:

						object target1 = new Proxies.EntityFieldProxy (this, entity, field.CaptionId);
						entity.InternalSetValue (field.Id, target1);

						break;

					case FieldRelation.Collection:

						object target2 = new Proxies.EntityCollectionFieldProxy (this, entity, field.CaptionId);
						entity.InternalSetValue (field.Id, target2);

						break;
				}
			}
		}


		private void DeserializeEntityLocalWithReference(AbstractEntity entity, EntityData entityData, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:

						if (entityData.ValueData.Contains (field.CaptionId))
						{
							object value = this.GetFieldValue (entity, field, entityData.ValueData[field.CaptionId]);
							entity.InternalSetValue (field.Id, value);
						}

						break;

					case FieldRelation.Reference:

						if (entityData.ReferenceData.Contains (field.CaptionId))
						{
							object target1 = this.InternalResolveEntity (entityData.ReferenceData[field.CaptionId], field.TypeId, EntityResolutionMode.DelayLoad);
							entity.InternalSetValue (field.Id, target1);
						}

						break;

					case FieldRelation.Collection:

						IList collection = entity.InternalGetFieldCollection (field.Id);

						foreach (DbKey key in entityData.CollectionData[field.CaptionId])
						{
							object target2 = this.InternalResolveEntity (key, field.TypeId, EntityResolutionMode.DelayLoad);
							collection.Add (target2);
						}

						break;
				}
			}
		}


		private void ReadFieldValueFromDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			object value = this.GetFieldValue (entity, fieldDef, dataRow);

			if (value != System.DBNull.Value)
			{
				entity.InternalSetValue (fieldDef.Id, value);
			}
		}
		public object GetFieldValue(AbstractEntity entity, Druid fieldId)
		{
			Druid entityId = entity.GetEntityStructuredTypeId ();
			StructuredTypeField fieldDef = entity.GetEntityContext ().GetEntityFieldDefinition (entityId, fieldId.ToResourceId ());

			DbKey rowKey = this.entityDataCache.FindMapping (entity.GetEntitySerialId ()).RowKey;
			System.Data.DataRow dataRow = this.LoadDataRow (entity, rowKey, entityId);

			return this.GetFieldValue (entity, fieldDef, dataRow);
		}

		private object GetFieldValue(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			string columnName = this.SchemaEngine.GetDataColumnName (fieldDef.Id);
			object value = dataRow[columnName];

			System.Diagnostics.Debug.Assert (fieldDef.Expression == null);
			System.Diagnostics.Debug.Assert (dataRow.Table.Columns.Contains (columnName));

			return this.GetFieldValue (entity, fieldDef, value);
		}


		private object GetFieldValue(AbstractEntity entity, StructuredTypeField field, object value)
		{
			string columnName = this.SchemaEngine.GetDataColumnName (field.Id);
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
					var entityId = entity.GetEntityStructuredTypeId ();
					var tableName = this.SchemaEngine.GetDataTableName (entityId);

					//	The conversion is a two step process:
					//	1. Convert from an ADO.NET type to a simple type (i.e. almost all numbers map to decimal)
					//	2. Convert from the simple type to the expected field type

					newValue = this.ConvertFromInternal (newValue, tableName, columnName);
					bool ok = InvariantConverter.Convert (newValue, field, out newValue);

					System.Diagnostics.Debug.Assert (ok, string.Format ("Could not convert column {0}.{1} to type {2}", tableName, columnName, field.Type.Name));
				}
			}

			return newValue;
		}
		private object ConvertFromInternal(object value, string tableName, string columnName)
		{
			if (value == System.DBNull.Value)
			{
				//	Nothing to convert : a DBNull value stays a DBNull value.
			}
			else
			{
				System.Diagnostics.Debug.Assert (value != null);

				DbTable   tableDef  = this.RichCommand.Tables[tableName];
				DbColumn  columnDef = tableDef.Columns[columnName];

				value = DataContext.ConvertFromInternal (value, columnDef);
			}

			return value;
		}
		private static object ConvertFromInternal(object value, DbColumn columnDef)
		{
			if (value != System.DBNull.Value)
			{
				DbTypeDef typeDef = columnDef.Type;
				value = TypeConverter.ConvertToSimpleType (value, typeDef.SimpleType, typeDef.NumDef);
			}

			return value;
		}

		internal IEnumerable<object> ReadFieldRelation(AbstractEntity entity, Druid entityId, StructuredTypeField fieldDef, EntityResolutionMode resolutionMode)
		{
			EntityDataMapping sourceMapping = this.GetEntityDataMapping (entity);
			string tableName = this.GetRelationTableName (entityId, fieldDef);

			System.Comparison<System.Data.DataRow> comparer = null;

			if (fieldDef.Relation == FieldRelation.Collection)
			{
				//	TODO: check that comparer really works !

				comparer = (a, b) =>
				{
					int valueA = (int) a[Tags.ColumnRefRank];
					int valueB = (int) b[Tags.ColumnRefRank];
					return valueA.CompareTo (valueB);
				};
			}

			bool found = false;

			for (int i = 0; i < 2 && !found; i++)
			{
				foreach (System.Data.DataRow relationRow in Collection.Enumerate (this.RichCommand.FindRelationRows (tableName, sourceMapping.RowKey.Id), comparer))
				{
					long relationTargetId = (long) relationRow[Tags.ColumnRefTargetId];
					object targetEntity = this.InternalResolveEntity (new DbKey (new DbId (relationTargetId)), fieldDef.TypeId, resolutionMode);
					yield return targetEntity;

					found = true;
				}

				if (!found)
				{
					this.LoadRelationRows (entity, entityId, fieldDef.CaptionId, tableName, sourceMapping.RowKey);
				}
			}
		}


		private void LoadDataRows(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.FindEntityDataMapping (entity);

			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			{
				this.LoadDataRow (entity, mapping.RowKey, currentId);

				StructuredTypeField[] localFields = this.EntityContext.GetEntityLocalFieldDefinitions (currentId).ToArray ();

				foreach (StructuredTypeField field in localFields.Where (f => f.Relation == FieldRelation.Reference || f.Relation == FieldRelation.Collection))
				{
					this.LoadRelationRows (entity, currentId, field.CaptionId, this.GetRelationTableName (currentId, field), mapping.RowKey);
				}
			}

			this.dataLoadedEntities.Add (entity);
		}


		private System.Data.DataRow LoadDataRow(AbstractEntity entity, DbKey rowKey, Druid entityId)
		{
			string tableName = this.SchemaEngine.GetDataTableName (entityId);

			System.Data.DataRow row = this.RichCommand.FindRow (tableName, rowKey.Id);

			bool noRow = row == null;
			bool bulkLoaded = this.BulkMode && this.tableBulkLoaded.ContainsKey (tableName) && this.tableBulkLoaded[tableName];
			bool alreadyLoaded = this.dataLoadedEntities.Contains (entity);

			if (noRow && !bulkLoaded && !alreadyLoaded)
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDef = this.SchemaEngine.FindTableDefinition (entityId);
					DbSelectCondition condition = this.DbInfrastructure.CreateSelectCondition (DbSelectRevision.LiveActive);

					if (this.BulkMode)
					{
						this.tableBulkLoaded[tableName] = true;
					}
					else
					{
						DbAbstractCondition part1 = condition.Condition;
						DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (tableDef.Columns[Tags.ColumnId]), DbSimpleConditionOperator.Equal, rowKey.Id.Value);

						condition.Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2);
					}

					this.RichCommand.ImportTable (transaction, tableDef, condition);
					this.LoadTableRelationSchemas (transaction, tableDef);
					transaction.Commit ();
				}

				row = this.RichCommand.FindRow (tableName, rowKey.Id);
			}

			return row;
		}


		private void LoadRelationRows(AbstractEntity entity, Druid entityId, Druid fieldId, string tableName, DbKey sourceRowKey)
		{
			bool bulkLoaded = this.BulkMode && this.tableBulkLoaded.ContainsKey (tableName) && this.tableBulkLoaded[tableName];
			bool alreadyLoaded = this.relationLoadedEntity.ContainsKey (fieldId) && this.relationLoadedEntity[fieldId].Contains (entity);

			if (!bulkLoaded && !alreadyLoaded)
			{
				DbTable relationTableDef = this.RichCommand.Tables[tableName];

				if (relationTableDef == null)
				{
					this.LoadTableRelationSchemas (entityId);

					relationTableDef = this.RichCommand.Tables[tableName];

					System.Diagnostics.Debug.Assert (relationTableDef != null);
				}

				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbSelectCondition condition = this.DbInfrastructure.CreateSelectCondition ();

					if (this.BulkMode)
					{
						this.tableBulkLoaded[tableName] = true;
					}
					else
					{
						DbAbstractCondition part1 = condition.Condition;
						DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (relationTableDef.Columns[Tags.ColumnRefSourceId]), DbSimpleConditionOperator.Equal, sourceRowKey.Id.Value);

						condition.Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2);
					}

					this.RichCommand.ImportTable (transaction, relationTableDef, condition);
					transaction.Commit ();
				}

				this.RelationRowIsLoaded (entity, fieldId);
			}
		}

		private string GetRelationTableName(Druid entityId, StructuredTypeField fieldDef)
		{
			string sourceTableName = this.SchemaEngine.GetDataTableName (entityId);
			string sourceColumnName = this.SchemaEngine.GetDataColumnName (fieldDef.Id);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}


		private void RelationRowIsLoaded(AbstractEntity entity, Druid fieldId)
		{
			if (!this.relationLoadedEntity.ContainsKey (fieldId))
			{
				this.relationLoadedEntity[fieldId] = new HashSet<AbstractEntity> ();
			}

			this.relationLoadedEntity[fieldId].Add (entity);
		}


		private readonly DataContext dataContext;


	}


}
