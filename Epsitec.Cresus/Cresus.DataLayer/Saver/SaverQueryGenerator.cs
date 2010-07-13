using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Common.Types;


namespace Epsitec.Cresus.DataLayer.Saver
{
	
	
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


		public DbKey GetNewDbKey(AbstractEntity entity)
		{
			// Gets the root entity Id.
			// Ask the DbInfrastructure to reserve a new DbKey for this Id.
			// Return this DbKey.

			throw new System.NotImplementedException ();
		}


		public void InsertEntityValues(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.InsertEntityValues (entity, localEntityId, dbKey);
			}
		}


		private void InsertEntityValues(AbstractEntity entity, Druid localEntityId, DbKey dbKey)
		{
			// Generate SQL request
			// Execute SQL request

			throw new System.NotImplementedException ();
		}


		public void UpdateEntityValues(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.UpdateEntityValues (entity, localEntityId, dbKey);
			}
		}


		private void UpdateEntityValues(AbstractEntity entity, Druid localEntityId, DbKey dbKey)
		{
			// Generate SQL request
			// Execute SQL request

			throw new System.NotImplementedException ();
		}


		public void DeleteEntityValues(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.DeleteEntityValues (localEntityId, dbKey);
			}
		}


		private void DeleteEntityValues(Druid localEntityId, DbKey dbKey)
		{
			// Generate SQL request
			// Execute SQL request

			throw new System.NotImplementedException ();
		}


		public void InsertEntityRelations(AbstractEntity entity)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		public void UpdateEntityRelations(AbstractEntity entity)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		public void DeleteEntityTargetRelations(AbstractEntity entity)
		{
			// TODO

			throw new System.NotImplementedException ();
		}


		public void DeleteEntitySourceRelations(AbstractEntity entity)
		{
			// TODO

			throw new System.NotImplementedException ();
		}
		private void RemoveEntityValueData(AbstractEntity entity, DbKey entityKey)
		{
			//foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			//{
			//    // TODO
			//    //this.RichCommand.DeleteExistingRow (this.LoadDataRow (entity, entityKey, currentId));
			//}
		}


		private void RemoveEntitySourceReferenceData(AbstractEntity entity, DbKey entityKey)
		{
			//Druid leafEntityId = entity.GetEntityStructuredTypeId();

			//var relationFields =
			//    from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
			//    let rel = field.Relation
			//    where  rel == FieldRelation.Reference || rel == FieldRelation.Collection
			//    select field;

			//foreach (StructuredTypeField field in relationFields)
			//{
			//    // TODO
			//    //string relationTableName = this.GetRelationTableName (currentId, field);

			//    //IEnumerable<System.Data.DataRow> relationRows = this.RichCommand.FindRelationRows (relationTableName, entityKey.Id);
			//    //System.Data.DataRow[] existingRelationRows = DbRichCommand.FilterExistingRows (relationRows).ToArray ();

			//    //foreach (System.Data.DataRow row in existingRelationRows)
			//    //{
			//    //    this.DeleteRelationRow (row);
			//    //}
			//}
		}





		private void RemoveEntityTargetReferenceDataInDatabase(AbstractEntity entity)
		{
			//EntityDataMapping targetMapping = this.FindEntityDataMapping (entity);

			//List<EntityFieldPath> sources = new List<EntityFieldPath> ();

			//foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			//{
			//    sources.AddRange (this.DbInfrastructure.GetSourceReferences (currentId));
			//}

			//SqlFieldList fields = new Database.Collections.SqlFieldList ();
			//fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbKey.RawTypeForStatus));

			//SqlFieldList conditions = new Database.Collections.SqlFieldList ();
			//SqlField nameColId = SqlField.CreateName (Tags.ColumnRefTargetId);
			//SqlField constantId = SqlField.CreateConstant (targetMapping.RowKey.Id, DbKey.RawTypeForId);

			//conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, nameColId, constantId));

			//using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction ())
			//{
			//    foreach (EntityFieldPath source in sources)
			//    {
			//        string sourceTableName = this.SchemaEngine.GetDataTableName (source.EntityId);
			//        string sourceColumnName = this.SchemaEngine.GetDataColumnName (source.Fields[0]);
			//        string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
			//        DbTable relationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);

			//        this.RichCommand.Update (transaction, relationTable, fields, conditions);
			//    }

			//    transaction.Commit ();
			//}
		}



		//    Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			
		//    foreach (Druid localId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
		//    {
		//        StructuredType entityType = this.EntityContext.GetStructuredType (localId) as StructuredType;
		//        Druid baseTypeId = entityType.BaseTypeId;

		//        //	Either create and fill a new row in the database for this entity
		//        //	or use and update an existing row.

		//        System.Data.DataRow dataRow;

		//        if (existsInDatabase)
		//        {
		//            dataRow = this.CreateDataRow (mapping, localId);
		//        }
		//        else
		//        {
		//            dataRow = this.LoadDataRow (entity, mapping.RowKey, localId);
		//        }

		//        dataRow.BeginEdit ();

		//        //	If this is the root entity in the entity hierarchy (it has no base
		//        //	type), then we will have to save the instance type identifying the
		//        //	entity.

		//        if (baseTypeId.IsEmpty)
		//        {
		//            dataRow[Tags.ColumnInstanceType] = leafEntityId.ToLong ();
		//        }

		//        this.SerializeEntityLocal (entity, dataRow, localId);

		//        dataRow.EndEdit ();
		//    }




		/// <summary>
		/// Serializes fields local to the specified entity into a given data
		/// row.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="dataRow">The data row.</param>
		/// <param name="entityId">The entity id.</param>
		private void SerializeEntityLocal(AbstractEntity entity, System.Data.DataRow dataRow, Druid entityId)
		{
			//foreach (StructuredTypeField fieldDef in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			//{
			//    switch (fieldDef.Relation)
			//    {
			//        case FieldRelation.None:
			//            this.WriteFieldValueInDataRow (entity, fieldDef, dataRow);
			//            break;

			//        case FieldRelation.Reference:
			//            this.WriteFieldReference (entity, entityId, fieldDef);
			//            break;

			//        case FieldRelation.Collection:
			//            this.WriteFieldCollection (entity, entityId, fieldDef);
			//            break;
			//    }
			//}
		}


		private void WriteFieldValueInDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			//object value = entity.InternalGetValue (fieldDef.Id);

			//System.Diagnostics.Debug.Assert (!UnknownValue.IsUnknownValue (value));

			//AbstractType  fieldType = fieldDef.Type as AbstractType;
			//INullableType nullableType = fieldDef.GetNullableType ();

			//if (UndefinedValue.IsUndefinedValue (value) || nullableType.IsNullValue (value))
			//{
			//    if (nullableType.IsNullable)
			//    {
			//        value = System.DBNull.Value;
			//    }
			//    else
			//    {
			//        value = fieldType.DefaultValue;
			//    }
			//}

			//string columnName = this.SchemaEngine.GetDataColumnName (fieldDef.Id);

			//dataRow[columnName] = this.ConvertToInternal (value, dataRow.Table.TableName, columnName);
		}


		private void WriteFieldReference(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			//AbstractEntity targetEntity = sourceEntity.InternalGetValue (fieldDef.Id) as AbstractEntity;

			//if ((EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (targetEntity)) ||
			//    (EntityNullReferenceVirtualizer.IsNullEntity (targetEntity)))
			//{
			//    targetEntity = null;
			//}

			//EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);
			//EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

			//string relationTableName = this.GetRelationTableName (entityId, fieldDef);

			//System.Data.DataRow[] relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToArray ();

			//if (targetEntity != null && this.CheckIfEntityCanBeSaved (targetEntity))
			//{
			//    System.Diagnostics.Debug.Assert (targetMapping != null);

			//    if (targetMapping.RowKey.IsEmpty)
			//    {
			//        this.SaveEntity (targetEntity);
			//    }

			//    if (relationRows.Length == 0)
			//    {
			//        this.CreateRelationRow (relationTableName, sourceMapping, targetMapping);
			//    }
			//    else if (relationRows.Length == 1)
			//    {
			//        this.UpdateRelationRow (relationRows[0], sourceMapping, targetMapping);
			//    }
			//    else
			//    {
			//        throw new System.InvalidOperationException ();
			//    }
			//}
			//else
			//{
			//    if (relationRows.Length == 1)
			//    {
			//        this.DeleteRelationRow (relationRows[0]);
			//    }
			//    else if (relationRows.Length > 1)
			//    {
			//        throw new System.InvalidOperationException ();
			//    }
			//}

			//this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
		}


		private void WriteFieldCollection(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			//IList collection = sourceEntity.InternalGetFieldCollection (fieldDef.Id);

			//System.Diagnostics.Debug.Assert (collection != null);

			//EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);

			//string relationTableName = this.GetRelationTableName (entityId, fieldDef);

			//List<System.Data.DataRow> relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToList ();
			//List<System.Data.DataRow> resultingRows = new List<System.Data.DataRow> ();

			//for (int i = 0; i < collection.Count; i++)
			//{
			//    AbstractEntity targetEntity  = collection[i] as AbstractEntity;

			//    if (this.CheckIfEntityCanBeSaved (targetEntity))
			//    {
			//        EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

			//        System.Diagnostics.Debug.Assert (targetMapping != null);

			//        if (targetMapping.RowKey.IsEmpty)
			//        {
			//            this.SaveEntity (targetEntity);
			//        }

			//        long targetRowId = targetMapping.RowKey.Id.Value;

			//        System.Diagnostics.Debug.Assert (targetEntity != null);
			//        System.Diagnostics.Debug.Assert (targetMapping != null);

			//        System.Data.DataRow row = relationRows.FirstOrDefault (r => targetRowId == (long) r[Tags.ColumnRefTargetId]);

			//        if (row == null)
			//        {
			//            resultingRows.Add (this.CreateRelationRow (relationTableName, sourceMapping, targetMapping));
			//        }
			//        else
			//        {
			//            relationRows.Remove (row);
			//            resultingRows.Add (row);
			//        }
			//    }
			//}

			//foreach (System.Data.DataRow row in relationRows)
			//{
			//    this.DeleteRelationRow (row);
			//}

			//int rank = -1;

			//foreach (System.Data.DataRow row in resultingRows)
			//{
			//    rank++;

			//    int rowRank = (int) row[Tags.ColumnRefRank];

			//    if ((rowRank < rank) ||
			//        (rowRank > rank+1000))
			//    {
			//        row[Tags.ColumnRefRank] = rank;
			//    }
			//    else if (rowRank > rank)
			//    {
			//        rank = rowRank;
			//    }
			//}

			//this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
		}

		private bool SaveTablesFilterImplementation(System.Data.DataTable table)
		{
			//	If the table contains the data for a root entity (the one which
			//	has no base type, i.e. no parent class), then let DbRichCommand
			//	attribute the DbKey for its rows.
			//	
			//	Tables which contain the data for derived entities should be
			//	skipped. See method SaveTablesRowIdAssignmentCallbackImplementation.

			return table.Columns.Contains (Tags.ColumnInstanceType) || table.Columns.Contains (Tags.ColumnRefSourceId);
		}


		///// <summary>
		///// This is the implementation of the raw ID assignment callback for the
		///// <see cref="DbRichCommand.SaveTables(DbTransaction, System.Predicate&lt;System.Data.DataTable&gt;, DbRichCommand.RowIdAssignmentCallback)"/>
		///// method. When this method is called, it updates the <see cref="DbKey"/>
		///// for the rows which contain the data for the associated entity.
		///// </summary>
		///// <param name="tableDef">The table definition.</param>
		///// <param name="table">The data table.</param>
		///// <param name="oldKey">The old key.</param>
		///// <param name="newKey">The new key.</param>
		///// <returns>
		///// Always returns <see cref="DbKey.Empty"/>, since it updates the
		///// row keys itself.
		///// </returns>
		//private DbKey SaveTablesRowIdAssignmentCallbackImplementation(DbTable tableDef, System.Data.DataTable table, DbKey oldKey, DbKey newKey)
		//{
		//    if (tableDef.Category == DbElementCat.Relation)
		//    {
		//        return newKey;
		//    }
		//    else
		//    {
		//        TemporaryRowCollection temporaryRows;
		//        temporaryRows = this.GetTemporaryRows (table.TableName);
		//        EntityDataMapping mapping = temporaryRows.UpdateAssociatedRowKeys (this.RichCommand, oldKey, newKey);

		//        if (mapping != null && mapping.IsReadOnly)
		//        {
		//            this.entityDataCache.DefineRowKey (mapping);
		//        }

		//        return DbKey.Empty;
		//    }
		//}


		private object ConvertToInternal(object value, string tableName, string columnName)
		{
			//    if (value == System.DBNull.Value)
			//    {
			//        //	Nothing to convert : a DBNull value stays a DBNull value.
			//    }
			//    else
			//    {
			//        System.Diagnostics.Debug.Assert (value != null);

			//        DbTable   tableDef  = this.RichCommand.Tables[tableName];
			//        DbColumn  columnDef = tableDef.Columns[columnName];

			//        value = DataSaver.ConvertToInternal (value, columnDef);
			//    }

			return value;
		}


		private static object ConvertToInternal(object value, DbColumn columnDef)
		{
			//if (value != System.DBNull.Value)
			//{
			//    DbTypeDef typeDef = columnDef.Type;

			//    if (typeDef.SimpleType == DbSimpleType.Decimal)
			//    {
			//        decimal decimalValue;

			//        if (InvariantConverter.Convert (value, out decimalValue))
			//        {
			//            value = decimalValue;
			//        }
			//        else
			//        {
			//            throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
			//        }
			//    }

			//    value = TypeConverter.ConvertFromSimpleType (value, typeDef.SimpleType, typeDef.NumDef);
			//}

			return value;
		}


		//private void UpdateRelationRow(System.Data.DataRow relationRow, EntityDataMapping sourceMapping, EntityDataMapping targetMapping)
		//{
		//    System.Diagnostics.Debug.Assert (sourceMapping.RowKey.Id.Value == (long) relationRow[Tags.ColumnRefSourceId]);
		//    System.Diagnostics.Debug.Assert (-1 == (int) relationRow[Tags.ColumnRefRank]);

		//    relationRow.BeginEdit ();
		//    relationRow[Tags.ColumnRefTargetId] = targetMapping.RowKey.Id.Value;
		//    relationRow.EndEdit ();
		//}

		//private System.Data.DataRow CreateRelationRow(string relationTableName, EntityDataMapping sourceMapping, EntityDataMapping targetMapping)
		//{
		//    System.Data.DataRow relationRow = this.RichCommand.CreateRow (relationTableName);
		//    DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);

		//    relationRow.BeginEdit ();
		//    key.SetRowKey (relationRow);
		//    relationRow[Tags.ColumnRefSourceId] = sourceMapping.RowKey.Id.Value;
		//    relationRow[Tags.ColumnRefTargetId] = targetMapping.RowKey.Id.Value;
		//    relationRow[Tags.ColumnRefRank] = -1;
		//    relationRow.EndEdit ();

		//    return relationRow;
		//}

		//private void DeleteRelationRow(System.Data.DataRow relationRow)
		//{
		//    switch (relationRow.RowState)
		//    {
		//        case System.Data.DataRowState.Added:
		//            relationRow.Table.Rows.Remove (relationRow);
		//            break;

		//        case System.Data.DataRowState.Modified:
		//        case System.Data.DataRowState.Unchanged:
		//            this.RichCommand.DeleteExistingRow (relationRow);
		//            break;

		//        default:
		//            throw new System.InvalidOperationException ();
		//    }
		//}


		///// <summary>
		///// Creates a new data row for the specified entity. The row will store
		///// data only for the locally defined fields of the given entity.
		///// </summary>
		///// <param name="mapping">The entity mapping.</param>
		///// <param name="entityId">The entity id.</param>
		///// <returns>A new data row with a temporary ID.</returns>
		//private System.Data.DataRow CreateDataRow(EntityDataMapping mapping, Druid entityId)
		//{
		//    System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
		//    System.Diagnostics.Debug.Assert (mapping.RowKey.IsTemporary);

		//    string tableName = this.SchemaEngine.GetDataTableName (entityId);
		//    System.Data.DataRow row = this.RichCommand.CreateRow (tableName);

		//    TemporaryRowCollection temporaryRows;
		//    temporaryRows = this.GetTemporaryRows (mapping.RootEntityId);
		//    temporaryRows.AssociateRow (this.RichCommand, mapping, row);

		//    return row;
		//}









	}


}
