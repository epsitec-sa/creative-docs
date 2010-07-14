using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{
	
	
	internal sealed class SaverQueryGenerator
	{


		public SaverQueryGenerator(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.EntityModificationViewer = new EntityModificationViewer (dataContext);
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private EntityModificationViewer EntityModificationViewer
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

			return this.GetNewDbKsys (transaction, table, 1).Single ();
		}


		private IEnumerable<DbKey> GetNewDbKsys(DbTransaction transaction, DbTable table, int count)
		{
			long start = this.DbInfrastructure.NewRowIdInTable (transaction, table, count);

			for (long newId = start; newId < start + count; newId++)
			{
				yield return new DbKey (new DbId (newId));
			}
		}


		public void InsertEntity(DbTransaction transaction, AbstractEntity entity)
		{
			DbKey dbKey = this.GetDbKey (entity);

			this.InsertEntityValues (transaction, entity, dbKey);
			this.InsertEntityReferences (transaction, entity, dbKey);
			this.InsertEntityCollections (transaction, entity, dbKey);
		}


		public void UpdateEntity(DbTransaction transaction, AbstractEntity entity)
		{
			DbKey dbKey = this.GetDbKey (entity);

			this.UpdateEntityValues (transaction, entity, dbKey);
			this.UpdateEntityReferences (transaction, entity, dbKey);
			this.UpdateEntityCollections (transaction, entity, dbKey);
		}


		public void DeleteEntity(DbTransaction transaction, AbstractEntity entity)
		{
			DbKey dbKey = this.GetDbKey (entity);

			this.DeleteEntityValues (transaction, entity, dbKey);
			this.DeleteEntitySourceRelations (transaction, entity, dbKey);
			this.DeleteEntityTargetRelations (transaction, entity, dbKey);
		}


		private void InsertEntityValues(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.InsertEntityValues (transaction, entity, localEntityId, dbKey);
			}
		}


		private void InsertEntityValues(DbTransaction transaction, AbstractEntity entity, Druid localEntityId, DbKey dbKey)
		{
			string tableName = this.SchemaEngine.GetEntityTableName (localEntityId);

			SqlFieldList fields = new SqlFieldList ();
			
			var fieldIds = from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
						   where field.Relation == FieldRelation.None
						   select field.CaptionId;

			fields.AddRange(this.CreateSqlFields (entity, localEntityId, fieldIds));

			fields.Add (this.CreateSqlFieldForKey (dbKey));
			fields.Add (this.CreateSqlFieldForStatus (DbRowStatus.Live));

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);

			if (localEntityId == rootEntityId)
			{
				fields.Add (this.CreateSqlFieldForType (leafEntityId));
			}

			transaction.SqlBuilder.InsertData (tableName, fields);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private void UpdateEntityValues(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.UpdateEntityValues (transaction, entity, localEntityId, dbKey);
			}
		}


		private void UpdateEntityValues(DbTransaction transaction, AbstractEntity entity, Druid localEntityId, DbKey dbKey)
		{
			SqlFieldList fields = new SqlFieldList ();

			List<Druid> fieldIds = new List<Druid> (
				from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
				let fieldId = field.CaptionId
				where field.Relation == FieldRelation.None
				where this.EntityModificationViewer.HasValueChanged (entity, fieldId)
				select fieldId
			);

			if (fieldIds.Any ())
			{
				fields.AddRange (this.CreateSqlFields (entity, localEntityId, fieldIds));

				SqlFieldList conditions = new SqlFieldList ();
				conditions.Add (this.CreateConditionForRowId (dbKey));

				string tableName = this.SchemaEngine.GetEntityTableName (localEntityId);

				transaction.SqlBuilder.UpdateData (tableName, fields, conditions);

				this.DbInfrastructure.ExecuteNonQuery (transaction);
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
			string tableName = this.SchemaEngine.GetEntityTableName (localEntityId);

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForRowId (dbKey));
			
			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private void InsertEntityReferences(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Reference
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.InsertEntityReference (transaction, entity, fieldId, dbKey);
			}
		}


		private void InsertEntityReference(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AbstractEntity target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ());

			if (this.DataContext.CheckIfEntityCanBeSaved (target))
			{
				List<DbKey> targetKeys = new List<DbKey> () { this.GetDbKey (target) };

				this.InsertEntityRelation (transaction, localEntityId, fieldId, dbKey, targetKeys);
			}
		}


		private void InsertEntityCollections(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.InsertEntityCollection (transaction, entity, fieldId, dbKey);
			}
		}


		private void InsertEntityCollection(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			List<DbKey> targetKeys = new List<DbKey> (
				from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.CheckIfEntityCanBeSaved (target)
				select this.GetDbKey (target)
			);

			if (targetKeys.Any ())
			{
				this.InsertEntityRelation (transaction, localEntityId, fieldId, dbKey, targetKeys);
			}
		}


		private void UpdateEntityReferences(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let fieldId = field.CaptionId
						   where field.Relation == FieldRelation.Reference
						   where this.EntityModificationViewer.HasReferenceChanged (entity, fieldId)
						   select fieldId;

			foreach (Druid fieldId in fieldIds)
			{
				this.UpdateEntityReference (transaction, entity, fieldId, dbKey);
			}
		}


		private void UpdateEntityReference(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
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

			this.InsertEntityReference (transaction, entity, fieldId, dbKey);
		}


		private void UpdateEntityCollections(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let fieldId = field.CaptionId
						   where field.Relation == FieldRelation.Collection
						   where this.EntityModificationViewer.HasCollectionChanged (entity, fieldId)
						   select fieldId;

			foreach (Druid fieldId in fieldIds)
			{
				this.UpdateEntityCollection (transaction, entity, fieldId, dbKey);
			}
		}


		private void UpdateEntityCollection(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
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

			this.InsertEntityCollection (transaction, entity, fieldId, dbKey);
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
			Druid fieldId = new Druid ("[" + fieldPath.Fields.First () + "]");
			
			this.DeleteEntityTargetRelation (transaction, localEntityId, fieldId, dbKey);
		}


		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, IEnumerable<DbKey> targetKeys)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);

			List<DbKey> targetKeysList = targetKeys.ToList();
			List<DbKey> dbKeys = this.GetNewDbKsys (transaction, table, targetKeysList.Count).ToList ();
			
			for (int rank = 0; rank < targetKeysList.Count; rank++)
            {
				SqlFieldList fields = new SqlFieldList ();

				fields.Add (this.CreateSqlFieldForKey (dbKeys[rank]));
				fields.Add (this.CreateSqlFieldForStatus (DbRowStatus.Live));
				fields.Add (this.CreateSqlFieldForSourceId (sourceKey));
				fields.Add (this.CreateSqlFieldForTargetId (targetKeysList[rank]));
				fields.Add (this.CreateSqlFieldForRank (rank));

				transaction.SqlBuilder.InsertData (table.Name, fields);

				this.DbInfrastructure.ExecuteNonQuery (transaction);	
            }
		}


		private void DeleteEntitySourceRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey)
		{
			string tableName = this.SchemaEngine.GetRelationTableName (localEntityId, fieldId);

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForSourceId (sourceKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private void DeleteEntityTargetRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey targetKey)
		{
			string tableName = this.SchemaEngine.GetRelationTableName (localEntityId, fieldId);

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForTargetId (targetKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		// TODO Use the SchemaEngine everywhere below to get the table and column definition to the
		// build the SqlField, for matter of consistency
		// Marc

		private SqlField CreateConditionForRowId(DbKey dbKey)
		{
			SqlField columnField = SqlField.CreateName (Tags.ColumnId);
			SqlField constantField = SqlField.CreateConstant(dbKey.Id, DbKey.RawTypeForId);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);
			
			return SqlField.CreateFunction (condition);
		}
		

		private SqlField CreateConditionForSourceId(DbKey dbKey)
		{
			SqlField columnField = SqlField.CreateName (Tags.ColumnRefSourceId);
			SqlField constantField = SqlField.CreateConstant (dbKey.Id, DbKey.RawTypeForId);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);

			return SqlField.CreateFunction (condition);
		}


		private SqlField CreateConditionForTargetId(DbKey dbKey)
		{
			SqlField columnField = SqlField.CreateName (Tags.ColumnRefTargetId);
			SqlField constantField = SqlField.CreateConstant (dbKey.Id, DbKey.RawTypeForId);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);

			return SqlField.CreateFunction (condition);
		}


		private IEnumerable<SqlField> CreateSqlFields(AbstractEntity entity, Druid localEntityId, IEnumerable<Druid> fieldIds)
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

				yield return this.CreateSqlFieldForEntityValueField (localEntityId, fieldId, value);
			}
		}


		private SqlField CreateSqlFieldForKey(DbKey key)
		{
			string name = Tags.ColumnId;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("[" + Tags.TypeKeyId + "]"));
			object value = key.Id.Value;

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForStatus(DbRowStatus status)
		{
			string name = Tags.ColumnStatus;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("[" + Tags.TypeKeyStatus + "]"));
			object value = (short) status;

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForType(Druid leafEntityId)
		{
			string name = Tags.ColumnInstanceType;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("[" + Tags.TypeKeyId + "]"));
			object value = leafEntityId.ToLong ();

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForSourceId(DbKey key)
		{
			string name = Tags.ColumnRefSourceId;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("[" + Tags.TypeKeyId + "]"));
			object value = key.Id.Value;

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForTargetId(DbKey key)
		{
			string name = Tags.ColumnRefTargetId;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("[" + Tags.TypeKeyId + "]"));
			object value = key.Id.Value;

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForRank(int rank)
		{
			string name = Tags.ColumnRefRank;
			DbTypeDef dbType = this.SchemaEngine.GetTypeDefinition (new Druid ("["  + Tags.TypeCollectionRank + "]"));
			object value = this.ConvertToInternal (dbType, rank);

			return this.CreateSqlField (name, dbType, value);
		}


		private SqlField CreateSqlFieldForEntityValueField(Druid localEntityId, Druid fieldId, object value)
		{
			string columnName = this.SchemaEngine.GetEntityColumnName (fieldId.ToResourceId ());

			DbTable dbTable = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			DbColumn dbColumn = dbTable.Columns[columnName];

			return this.CreateSqlField (dbColumn.Name, dbColumn.Type, value);
		}


		private SqlField CreateSqlField(string name, DbTypeDef dbType, object value)
		{
			object convertedValue = this.ConvertToInternal (dbType, value);
			DbRawType rawType = dbType.RawType;

			SqlField SqlField = SqlField.CreateConstant (convertedValue, rawType);
			SqlField.Alias = name;

			return SqlField;
		}


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
				else
				{
					newValue = TypeConverter.ConvertFromSimpleType (value, dbType.SimpleType, dbType.NumDef);
				}
			}

			return newValue;
		}


		private DbKey GetDbKey(AbstractEntity entity)
		{
			return this.DataContext.GetEntityDataMapping (entity).RowKey;
		}


	}


}
