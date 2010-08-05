using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	// TODO Comment this class
	// Marc
	

	internal sealed class SaverQueryGenerator
	{


		public SaverQueryGenerator(DataContext dataContext)
		{
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


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
		}


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.SchemaEngine;
			}
		}


		public DbKey GetNewDbKey(DbTransaction transaction, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (leafEntityId);

			DbTable table = this.SchemaEngine.GetEntityTableDefinition (rootEntityId);

			return this.GetNewDbKeys (transaction, table, 1).Single ();
		}


		private IEnumerable<DbKey> GetNewDbKeys(DbTransaction transaction, DbTable table, int count)
		{
			long start = this.DbInfrastructure.NewRowIdInTable (transaction, table, count);

			for (long newId = start; newId < start + count; newId++)
			{
				yield return new DbKey (new DbId (newId));
			}
		}


		public IEnumerable<AbstractSynchronisationJob> InsertEntityValues(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity)
		{
			this.InsertEntityValuesWorker (transaction, newEntityKeys, entity);

			return new List<AbstractSynchronisationJob> ();
		}


		public IEnumerable<AbstractSynchronisationJob> InsertEntityRelations(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity)
		{
			DbKey dbKey = this.GetNonPersistentEntityDbKey (entity, newEntityKeys);
			
			this.InsertEntityReferences (transaction, newEntityKeys, entity, dbKey);
			this.InsertEntityCollections (transaction, newEntityKeys, entity, dbKey);

			return new List<AbstractSynchronisationJob> ();
		}


		public IEnumerable<AbstractSynchronisationJob> UpdateEntityValues(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity)
		{
			DbKey dbKey = this.GetPersistentEntityDbKey (entity);

			return this.UpdateEntityValues (transaction, newEntityKeys, entity, dbKey).ToList ();
		}


		public IEnumerable<AbstractSynchronisationJob> UpdateEntityRelations(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity)
		{
			DbKey dbKey = this.GetPersistentEntityDbKey (entity);

			var jobs1 = this.UpdateEntityReferences (transaction, newEntityKeys, entity, dbKey).ToList ();
			var jobs2 = this.UpdateEntityCollections (transaction, newEntityKeys, entity, dbKey).ToList ();

			return jobs1.Concat (jobs2);
		}


		public void DeleteEntity(DbTransaction transaction, AbstractEntity entity)
		{
			DbKey dbKey = this.GetPersistentEntityDbKey (entity);

			this.DeleteEntityValues (transaction, entity, dbKey);
			this.DeleteEntitySourceRelations (transaction, entity, dbKey);
			this.DeleteEntityTargetRelations (transaction, entity, dbKey);
		}


		private void InsertEntityValuesWorker(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId).Reverse ())
			{
				this.InsertEntityValuesWorker (transaction, newEntityKeys, entity, localEntityId);
			}
		}


		private void InsertEntityValuesWorker(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid localEntityId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);

			if (localEntityId == rootEntityId)
			{
				newEntityKeys[entity] = this.GetNewDbKey (transaction, entity);
			}
			
			DbKey dbKey = this.GetNonPersistentEntityDbKey (entity, newEntityKeys);
						
			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			
			SqlFieldList fields = new SqlFieldList ();
			
			var fieldIds = from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
						   where field.Relation == FieldRelation.None
						   select field.CaptionId;

			fields.AddRange (this.CreateSqlFields (table, entity, localEntityId, fieldIds));

			fields.Add (this.CreateSqlFieldForKey (table, dbKey));
			fields.Add (this.CreateSqlFieldForStatus (table, DbRowStatus.Live));
			fields.Add (this.CreateSqlFieldForLog (table));

			if (localEntityId == rootEntityId)
			{
				fields.Add (this.CreateSqlFieldForType (table, leafEntityId));
			}

			string tableName = table.GetSqlName ();
			
			transaction.SqlBuilder.InsertData (tableName, fields);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private IEnumerable<AbstractSynchronisationJob> UpdateEntityValues(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				var newJobs = this.UpdateEntityValues (transaction, newEntityKeys, entity, localEntityId, dbKey);

				foreach (var newJob in newJobs)
				{
					yield return newJob;
				}
			}
		}


		private IEnumerable<AbstractSynchronisationJob> UpdateEntityValues(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid localEntityId, DbKey dbKey)
		{
			SqlFieldList fields = new SqlFieldList ();

			List<Druid> fieldIds = new List<Druid> (
				from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
				let fieldId = field.CaptionId
				where field.Relation == FieldRelation.None
				where entity.HasValueChanged(fieldId)
				select fieldId
			);

			if (fieldIds.Any ())
			{
				DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);

				fields.AddRange (this.CreateSqlFields (table, entity, localEntityId, fieldIds));

				SqlFieldList conditions = new SqlFieldList ();
				conditions.Add (this.CreateConditionForRowId (table, dbKey));

				string tableName = table.GetSqlName ();

				transaction.SqlBuilder.UpdateData (tableName, fields, conditions);

				this.DbInfrastructure.ExecuteNonQuery (transaction);
			}

			foreach (Druid fieldId in fieldIds)
			{
				yield return this.CreateValueSynchronizationJob (newEntityKeys, entity, fieldId);
			}
		}


		private void DeleteEntityValues(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.DeleteEntityValues (transaction, localEntityId, dbKey);
			}
		}


		private void DeleteEntityValues(DbTransaction transaction, Druid localEntityId, DbKey dbKey)
		{
			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			
			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForRowId (table, dbKey));

			string tableName = table.GetSqlName ();

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private void InsertEntityReferences(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Reference
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.InsertEntityReference (transaction, newEntityKeys, entity, fieldId, dbKey);
			}
		}


		private void InsertEntityReference(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AbstractEntity target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ());

			if (this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target))
			{
				List<DbKey> targetKeys = new List<DbKey> ()
				{
					this.GetEntityDbKey (target, newEntityKeys)
				};

				this.InsertEntityRelation (transaction, localEntityId, fieldId, dbKey, targetKeys);
			}
		}


		private void InsertEntityCollections(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.InsertEntityCollection (transaction, newEntityKeys, entity, fieldId, dbKey);
			}
		}


		private void InsertEntityCollection(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			List<DbKey> targetKeys = new List<DbKey> (
				from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select this.GetEntityDbKey (target, newEntityKeys)
			);

			if (targetKeys.Any ())
			{
				this.InsertEntityRelation (transaction, localEntityId, fieldId, dbKey, targetKeys);
			}
		}


		private IEnumerable<AbstractSynchronisationJob> UpdateEntityReferences(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let fieldId = field.CaptionId
						   where field.Relation == FieldRelation.Reference
						   where entity.HasReferenceChanged (fieldId)
						   select fieldId;

			foreach (Druid fieldId in fieldIds)
			{
				var newJob = this.UpdateEntityReference (transaction, newEntityKeys, entity, fieldId, dbKey);

				if (newJob != null)
				{
					yield return newJob;
				}
			}
		}


		private AbstractSynchronisationJob UpdateEntityReference(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			// TODO This function might be optimized in the following way. I'm not too sure if it
			// would be better than the current implementation.
			// - If target is null => remove the row in the database
			// - If target is not null and a row exists in the database => update the row in the database
			// - If target is not null and a row does not exist in the database => create the row in the database
			// Marc
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, dbKey);

			this.InsertEntityReference (transaction, newEntityKeys, entity, fieldId, dbKey);

			return this.CreateReferenceSynchronizationJob (newEntityKeys, entity, fieldId);
		}


		private IEnumerable<AbstractSynchronisationJob> UpdateEntityCollections(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let fieldId = field.CaptionId
						   where field.Relation == FieldRelation.Collection
						   where entity.HasCollectionChanged (fieldId)
						   select fieldId;

			foreach (Druid fieldId in fieldIds)
			{
				var newJob = this.UpdateEntityCollection (transaction, newEntityKeys, entity, fieldId, dbKey);

				yield return newJob;
			}
		}


		private AbstractSynchronisationJob UpdateEntityCollection(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			// TODO This function might be optimized by having a better policy to delete/update/Insert
			// the relation rows. It could take advantage to what already exists in the database, which
			// would have the following two advantages:
			// - It might require less queries
			// - If another user has modified the data and our version of the relations is not up to
			//   date, it might do less overwrite.

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, dbKey);

			this.InsertEntityCollection (transaction, newEntityKeys, entity, fieldId, dbKey);

			return this.CreateCollectionSynchronizationJob (newEntityKeys, entity, fieldId);
		}


		private void DeleteEntitySourceRelations(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let rel = field.Relation
						   where rel == FieldRelation.Reference || rel == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.DeleteEntitySourceRelation(transaction, entity, fieldId, dbKey);
			}
		}


		private void DeleteEntitySourceRelation(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, dbKey);
		}


		private void DeleteEntityTargetRelations(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				var sourceReferences = this.DbInfrastructure.GetSourceReferences (localEntityId);

				foreach (EntityFieldPath fieldPath in sourceReferences)
				{
					this.DeleteEntityTargetRelation (transaction, dbKey, fieldPath);
				}
			}
		}


		private void DeleteEntityTargetRelation(DbTransaction transaction, DbKey dbKey, EntityFieldPath fieldPath)
		{
			Druid localEntityId = fieldPath.EntityId;
			Druid fieldId = new Druid (fieldPath.Fields.First ());
			
			this.DeleteEntityTargetRelation (transaction, localEntityId, fieldId, dbKey);
		}


		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, IEnumerable<DbKey> targetKeys)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);

			string tableName = table.GetSqlName ();
				
			List<DbKey> targetKeysList = targetKeys.ToList ();
			List<DbKey> dbKeys = this.GetNewDbKeys (transaction, table, targetKeysList.Count).ToList ();

			for (int rank = 0; rank < targetKeysList.Count; rank++)
			{
				SqlFieldList fields = new SqlFieldList ();

				fields.Add (this.CreateSqlFieldForKey (table, dbKeys[rank]));
				fields.Add (this.CreateSqlFieldForStatus (table, DbRowStatus.Live));
				fields.Add (this.CreateSqlFieldForSourceId (table, sourceKey));
				fields.Add (this.CreateSqlFieldForTargetId (table, targetKeysList[rank]));
				fields.Add (this.CreateSqlFieldForRank (table, rank));

				transaction.SqlBuilder.InsertData (tableName, fields);

				this.DbInfrastructure.ExecuteNonQuery (transaction);
			}
		}


		private void DeleteEntitySourceRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			string tableName = table.GetSqlName ();

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForSourceId (table, sourceKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private void DeleteEntityTargetRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey targetKey)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			string tableName = table.GetSqlName ();

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForTargetId (table, targetKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private SqlField CreateConditionForRowId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}
		

		private SqlField CreateConditionForSourceId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnRefSourceId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}


		private SqlField CreateConditionForTargetId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnRefTargetId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}


		private SqlField CreateConditionForField(DbColumn column, object value)
		{
			string columnName = column.GetSqlName ();
			SqlField columnField = SqlField.CreateName (columnName);
			
			DbTypeDef columnType = column.Type;
			object convertedValue = this.ConvertToInternal (columnType, value);
			DbRawType convertedRawType = this.ConvertToInternal (columnType);

			SqlField constantField = SqlField.CreateConstant (convertedValue, convertedRawType);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);

			return SqlField.CreateFunction (condition);
		}


		private IEnumerable<SqlField> CreateSqlFields(DbTable table, AbstractEntity entity, Druid localEntityId, IEnumerable<Druid> fieldIds)
		{
			foreach (Druid fieldId in fieldIds)
			{
				object value = entity.GetField<object> (fieldId.ToResourceId ());

				var field = this.EntityContext.GetEntityFieldDefinition (localEntityId, fieldId.ToResourceId ());
				
				AbstractType fieldType = field.Type as AbstractType;
				INullableType nullableType = field.GetNullableType ();

				if (UndefinedValue.IsUndefinedValue (value) || nullableType.IsNullValue (value))
				{
					value = (nullableType.IsNullable) ? System.DBNull.Value : fieldType.DefaultValue;
				}

				yield return this.CreateSqlFieldForEntityValueField (table, fieldId, value);
			}
		}


		private SqlField CreateSqlFieldForKey(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForStatus(DbTable table, DbRowStatus status)
		{
			DbColumn column = table.Columns[Tags.ColumnStatus];
			object value = (short) status;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForType(DbTable table, Druid leafEntityId)
		{
			DbColumn column = table.Columns[Tags.ColumnInstanceType];
			object value = leafEntityId.ToLong ();

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForLog(DbTable table)
		{
			// TODO Get the real value for the log
			// Marc

			DbColumn column = table.Columns[Tags.ColumnRefLog];
			object value = 0;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForSourceId(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnRefSourceId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForTargetId(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnRefTargetId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForRank(DbTable table, int rank)
		{
			DbColumn column = table.Columns[Tags.ColumnRefRank];
			object value = rank;

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForEntityValueField(DbTable table, Druid fieldId, object value)
		{
			string columnName = this.SchemaEngine.GetEntityColumnName (fieldId);
			DbColumn column = table.Columns[columnName];

			return this.CreateSqlFieldForColumn (column, value);
		}


		private SqlField CreateSqlFieldForColumn(DbColumn column, object value)
		{
			DbTypeDef type = column.Type;
			
			object convertedValue = this.ConvertToInternal (type, value);
			DbRawType convertedRawType = this.ConvertToInternal (type);
			
			SqlField SqlField = SqlField.CreateConstant (convertedValue, convertedRawType);
			SqlField.Alias = column.GetSqlName ();

			return SqlField;
		}


		// TODO All this conversion stuff is kind of low level and should be moved elsewhere. Especially
		// the part about converting unsupported types to supported types should be kept inside the
		// database implementation part.
		// Marc


		private object ConvertToInternal(DbTypeDef dbType, object value)
		{
			object newValue = value;

			if (value != System.DBNull.Value)
			{
				if (dbType.SimpleType == DbSimpleType.Decimal)
				{
					decimal decimalValue;

					bool success = InvariantConverter.Convert (value, out decimalValue);

					if (!success)
					{
						throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
					}

					newValue = decimalValue;
				}

				newValue = TypeConverter.ConvertFromSimpleType (newValue, dbType.SimpleType, dbType.NumDef);
			}

			DbRawType rawType = dbType.RawType;

			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (rawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool sucess = typeConverter.GetRawTypeConverter (rawType, out rawTypeConverter);

				if (!sucess)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + rawType);
				}

				newValue = rawTypeConverter.ConvertToInternalType (newValue);
			}

			return newValue;
		}


		private DbRawType ConvertToInternal(DbTypeDef dbType)
		{
			DbRawType newRawType = dbType.RawType;
						
			ITypeConverter typeConverter = this.DbInfrastructure.Converter;

			if (!typeConverter.CheckNativeSupport (newRawType))
			{
				IRawTypeConverter rawTypeConverter;
				bool sucess = typeConverter.GetRawTypeConverter (newRawType, out rawTypeConverter);

				if (!sucess)
				{
					throw new System.NotSupportedException ("Unable to convert unsupported raw type: " + newRawType);
				}

				newRawType = rawTypeConverter.InternalType;
			}

			return newRawType;
		}


		private DbKey GetPersistentEntityDbKey(AbstractEntity entity)
		{
			return this.DataContext.GetEntityKey (entity).Value.RowKey;
		}


		private DbKey GetNonPersistentEntityDbKey(AbstractEntity entity, Dictionary<AbstractEntity, DbKey> newEntityKeys)
		{
			return newEntityKeys[entity];
		}


		private DbKey GetEntityDbKey(AbstractEntity entity, Dictionary<AbstractEntity, DbKey> newEntityKeys)
		{
			if (this.DataContext.IsPersistent (entity))
			{
				return this.GetPersistentEntityDbKey (entity);
			}
			else
			{
				return this.GetNonPersistentEntityDbKey (entity, newEntityKeys);
			}
		}


		private AbstractSynchronisationJob CreateValueSynchronizationJob(Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId)
		{
			int dataContextId = this.DataContext.UniqueId;
			EntityKey entityKey = this.GetEntityKey (entity, newEntityKeys);

			string fieldName = fieldId.ToResourceId ();
			object newValue = entity.GetField<object> (fieldName);

			return new ValueSynchronizationJob (dataContextId, entityKey, fieldId, newValue);
		}


		private AbstractSynchronisationJob CreateReferenceSynchronizationJob(Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId)
		{
			int dataContextId = this.DataContext.UniqueId;
			EntityKey entityKey = this.GetEntityKey (entity, newEntityKeys);

			string fieldName = fieldId.ToResourceId ();
			AbstractEntity target = entity.GetField<AbstractEntity> (fieldName);

			AbstractSynchronisationJob job = null;

			if (target == null)
			{
				job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, null);
			}
			else if (this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target))
			{
				EntityKey newValue = this.GetEntityKey (target, newEntityKeys);

				job = new ReferenceSynchronizationJob (dataContextId, entityKey, fieldId, newValue);
			}

			return job;
		}


		private AbstractSynchronisationJob CreateCollectionSynchronizationJob(Dictionary<AbstractEntity, DbKey> newEntityKeys, AbstractEntity entity, Druid fieldId)
		{
			int dataContextId = this.DataContext.UniqueId;
			EntityKey entityKey = this.GetEntityKey (entity, newEntityKeys);

			string fieldName = fieldId.ToResourceId ();
			var newValues = from e in entity.GetFieldCollection<AbstractEntity> (fieldName)
							where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (e)
							select this.GetEntityKey (e, newEntityKeys);

			return new CollectionSynchronizationJob (dataContextId, entityKey, fieldId, newValues);
		}


		private EntityKey GetEntityKey(AbstractEntity entity, Dictionary<AbstractEntity, DbKey> newEntityKeys)
		{
			EntityKey entityKey;
			
			if (this.DataContext.IsPersistent (entity))
			{
				entityKey = this.DataContext.GetEntityKey (entity).Value;
			}
			else
			{
				DbKey dbKey = this.GetNonPersistentEntityDbKey (entity, newEntityKeys);

				entityKey = new EntityKey (entity, dbKey);
			}

			return entityKey;
		}


	}


}
