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
				conditions.Add (this.CreateConditionForId (localEntityId, dbKey));

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
			conditions.Add (this.CreateConditionForId (localEntityId, dbKey));
			
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
			// TODO

			throw new System.NotImplementedException ();
		}


		private void InsertEntityCollections(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   where field.Relation == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.InsertEntityReference (transaction, entity, fieldId, dbKey);
			}
		}


		private void InsertEntityCollection(DbTransaction transaction, AbstractEntity entity, Druid fieldId, DbKey dbKey)
		{
			// TODO

			throw new System.NotImplementedException ();
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
			// TODO

			throw new System.NotImplementedException ();
		}


		//private void WriteFieldReference(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		//{
		//    AbstractEntity targetEntity = sourceEntity.InternalGetValue (fieldDef.Id) as AbstractEntity;

		//    if ((EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (targetEntity)) ||
		//        (EntityNullReferenceVirtualizer.IsNullEntity (targetEntity)))
		//    {
		//        targetEntity = null;
		//    }

		//    EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);
		//    EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

		//    string relationTableName = this.GetRelationTableName (entityId, fieldDef);

		//    System.Data.DataRow[] relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToArray ();

		//    if (targetEntity != null && this.CheckIfEntityCanBeSaved (targetEntity))
		//    {
		//        System.Diagnostics.Debug.Assert (targetMapping != null);

		//        if (targetMapping.RowKey.IsEmpty)
		//        {
		//            this.SaveEntity (targetEntity);
		//        }

		//        if (relationRows.Length == 0)
		//        {
		//            this.CreateRelationRow (relationTableName, sourceMapping, targetMapping);
		//        }
		//        else if (relationRows.Length == 1)
		//        {
		//            this.UpdateRelationRow (relationRows[0], sourceMapping, targetMapping);
		//        }
		//        else
		//        {
		//            throw new System.InvalidOperationException ();
		//        }
		//    }
		//    else
		//    {
		//        if (relationRows.Length == 1)
		//        {
		//            this.DeleteRelationRow (relationRows[0]);
		//        }
		//        else if (relationRows.Length > 1)
		//        {
		//            throw new System.InvalidOperationException ();
		//        }
		//    }

		//    this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
		//}


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
			// TODO

			throw new System.NotImplementedException ();
		}		//private void WriteFieldCollection(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		//{
		//    IList collection = sourceEntity.InternalGetFieldCollection (fieldDef.Id);

		//    System.Diagnostics.Debug.Assert (collection != null);

		//    EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);

		//    string relationTableName = this.GetRelationTableName (entityId, fieldDef);

		//    List<System.Data.DataRow> relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToList ();
		//    List<System.Data.DataRow> resultingRows = new List<System.Data.DataRow> ();

		//    for (int i = 0; i < collection.Count; i++)
		//    {
		//        AbstractEntity targetEntity  = collection[i] as AbstractEntity;

		//        if (this.CheckIfEntityCanBeSaved (targetEntity))
		//        {
		//            EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

		//            System.Diagnostics.Debug.Assert (targetMapping != null);

		//            if (targetMapping.RowKey.IsEmpty)
		//            {
		//                this.SaveEntity (targetEntity);
		//            }

		//            long targetRowId = targetMapping.RowKey.Id.Value;

		//            System.Diagnostics.Debug.Assert (targetEntity != null);
		//            System.Diagnostics.Debug.Assert (targetMapping != null);

		//            System.Data.DataRow row = relationRows.FirstOrDefault (r => targetRowId == (long) r[Tags.ColumnRefTargetId]);

		//            if (row == null)
		//            {
		//                resultingRows.Add (this.CreateRelationRow (relationTableName, sourceMapping, targetMapping));
		//            }
		//            else
		//            {
		//                relationRows.Remove (row);
		//                resultingRows.Add (row);
		//            }
		//        }
		//    }

		//    foreach (System.Data.DataRow row in relationRows)
		//    {
		//        this.DeleteRelationRow (row);
		//    }

		//    int rank = -1;

		//    foreach (System.Data.DataRow row in resultingRows)
		//    {
		//        rank++;

		//        int rowRank = (int) row[Tags.ColumnRefRank];

		//        if ((rowRank < rank) ||
		//            (rowRank > rank+1000))
		//        {
		//            row[Tags.ColumnRefRank] = rank;
		//        }
		//        else if (rowRank > rank)
		//        {
		//            rank = rowRank;
		//        }
		//    }

		//    this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
		//}


		private void DeleteEntitySourceRelations(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let rel = field.Relation
						   where rel == FieldRelation.Reference || rel == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				this.DeleteEntitySourceRelation(transaction, dbKey, fieldId);
			}
		}


		private void DeleteEntitySourceRelation(DbTransaction transaction, DbKey dbKey, Druid fieldId)
		{
			// TODO

			throw new System.NotImplementedException ();

			//Druid currentId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);
			//string relationTableName = this.GetRelationTableName (currentId, field);

			//IEnumerable<System.Data.DataRow> relationRows = this.RichCommand.FindRelationRows (relationTableName, entityKey.Id);
			//System.Data.DataRow[] existingRelationRows = DbRichCommand.FilterExistingRows (relationRows).ToArray ();

			//foreach (System.Data.DataRow row in existingRelationRows)
			//{
			//    this.DeleteRelationRow (row);
			//}
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
			// TODO

			throw new System.NotImplementedException ();

			//SqlFieldList fields = new Database.Collections.SqlFieldList ();
			//fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbKey.RawTypeForStatus));

			//SqlFieldList conditions = new Database.Collections.SqlFieldList ();
			//SqlField nameColId = SqlField.CreateName (Tags.ColumnRefTargetId);
			//SqlField constantId = SqlField.CreateConstant (targetMapping.RowKey.Id, DbKey.RawTypeForId);

			//conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, nameColId, constantId));

			//string sourceTableName = this.SchemaEngine.GetDataTableName (source.EntityId);
			//string sourceColumnName = this.SchemaEngine.GetDataColumnName (source.Fields[0]);
			//string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
			//DbTable relationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);

			//this.RichCommand.Update (transaction, relationTable, fields, conditions);
			//transaction.Commit ();
		}


		private void InsertEntityRelations(Druid localEntityId, Druid fieldId, DbKey sourceKey, IEnumerable<DbKey> targetKeys)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		private void UpdateEntityRelations(Druid localEntityId, Druid fieldId, DbKey sourceId, DbKey targetId)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		private void deleteEntityRelations(Druid localEntityId, Druid fieldId, DbKey sourceKey)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		private SqlField CreateConditionForId(Druid localEntityId, DbKey dbKey)
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

					if (InvariantConverter.Convert (value, out decimalValue))
					{
						newValue = decimalValue;
					}
					else
					{
						throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
					}
				}

				newValue = TypeConverter.ConvertFromSimpleType (value, dbType.SimpleType, dbType.NumDef);
			}

			return newValue;
		}


		private DbKey GetDbKey(AbstractEntity entity)
		{
			return this.DataContext.GetEntityDataMapping (entity).RowKey;
		}


	}


}
