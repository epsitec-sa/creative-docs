using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	internal sealed class DataSaver
	{


		public DataSaver(DataContext dataContext)
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


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.SchemaEngine;
			}
		}


		public void SaveChanges()
		{
			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction ())
			{
				this.SerializeChanges ();
				
				transaction.Commit ();
			}
		}


		private void SerializeChanges()
		{
			bool deletedEntities = this.DeleteEntities ();
			bool savedEntities = this.SaveEntities ();

			this.UpdateDataGeneration (deletedEntities || savedEntities);
		}


		private bool DeleteEntities()
		{
			List<AbstractEntity> entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.RemoveEntity (entity);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();

			return entitiesToDelete.Any ();
		}


		private bool SaveEntities()
		{
			List<AbstractEntity> entitiesToSave = new List<AbstractEntity> (
				from entity in this.DataContext.GetEntitiesModified ()
				where this.CheckIfEntityCanBeSaved (entity)
				select entity
			);
						
			foreach (AbstractEntity entity in entitiesToSave)
			{
				this.SaveEntity (entity);
			}

			return entitiesToSave.Any ();
		}


		private void UpdateDataGeneration(bool containsChanges)
		{
			if (containsChanges)
			{
				this.EntityContext.NewDataGeneration ();
			}
		}


		/// <summary>
		/// Serializes the entity to the in-memory data set. This will either
		/// update or create data rows in one or several data tables.
		/// </summary>
		/// <param name="entity">The entity.</param>
		private void SaveEntity(AbstractEntity entity)
		{
			//if (entity == null)
			//{
			//    return;
			//}
			//if (!this.Contains (entity))
			//{
			//    //	TODO: should we propagate the serialization to another DataContext ?
			//    return;
			//}

			//EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			//System.Diagnostics.Debug.Assert (mapping != null);

			//if (mapping.SerialGeneration == this.EntityContext.DataGeneration)
			//{
			//    return;
			//}
			//else
			//{
			//    mapping.SerialGeneration = this.EntityContext.DataGeneration;
			//}

			//Druid entityId = entity.GetEntityStructuredTypeId ();

			//bool createRow = mapping.RowKey.IsEmpty;

			//if (createRow)
			//{
			//    mapping.RowKey = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
			//    this.dataLoadedEntities.Add (entity);
			//}
			//else
			//{
			//    this.LoadDataRows (entity);
			//}

			//foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entityId))
			//{
			//    StructuredType entityType = this.EntityContext.GetStructuredType (currentId) as StructuredType;
			//    Druid          baseTypeId = entityType.BaseTypeId;

			//    System.Diagnostics.Debug.Assert (entityType != null);
			//    System.Diagnostics.Debug.Assert (entityType.CaptionId == currentId);

			//    //	Either create and fill a new row in the database for this entity
			//    //	or use and update an existing row.

			//    System.Data.DataRow dataRow;

			//    if (createRow)
			//    {
			//        dataRow = this.CreateDataRow (mapping, currentId);
			//    }
			//    else
			//    {
			//        dataRow = this.LoadDataRow (entity, mapping.RowKey, currentId);
			//    }

			//    dataRow.BeginEdit ();

			//    //	If this is the root entity in the entity hierarchy (it has no base
			//    //	type), then we will have to save the instance type identifying the
			//    //	entity.

			//    if (baseTypeId.IsEmpty)
			//    {
			//        dataRow[Tags.ColumnInstanceType] = entityId.ToLong ();
			//    }

			//    this.SerializeEntityLocal (entity, dataRow, currentId);

			//    dataRow.EndEdit ();
			//}
		}


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


		private void RemoveEntity(AbstractEntity entity)
		{
			//EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (entity);

			//if (mapping.SerialGeneration != this.EntityContext.DataGeneration)
			//{
			//    mapping.SerialGeneration = this.EntityContext.DataGeneration;

			//    if (!mapping.RowKey.IsEmpty)
			//    {
			//        this.RemoveEntityValueData (entity, mapping.RowKey);
			//        this.RemoveEntitySourceReferenceData (entity, mapping.RowKey);
			//        this.RemoveEntityTargetReferenceDataInMemory (entity);
			//        this.RemoveEntityTargetReferenceDataInDatabase (entity);
			//    }
			//}
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


		private void RemoveEntityTargetReferenceDataInMemory(AbstractEntity entity)
		{
			//foreach (System.Tuple<AbstractEntity, EntityFieldPath> item in new DataBrowser (this).GetReferencers (entity, false))
			//{
			//    AbstractEntity sourceEntity = item.Item1;
			//    StructuredTypeField field = this.EntityContext.GetStructuredTypeField (sourceEntity, item.Item2.Fields.First ());

			//    using (sourceEntity.UseSilentUpdates ())
			//    {
			//        switch (field.Relation)
			//        {
			//            case FieldRelation.Reference:
			//                sourceEntity.InternalSetValue (field.Id, null);
			//                break;

			//            case FieldRelation.Collection:

			//                IList collection = sourceEntity.InternalGetFieldCollection (field.Id) as IList;

			//                while (collection.Contains (entity))
			//                {
			//                    collection.Remove (entity);
			//                }

			//                break;

			//            default:
			//                throw new System.InvalidOperationException ();
			//        }
			//    }
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


		internal bool CheckIfEntityCanBeSaved(AbstractEntity entity)
		{
			bool isDeleted = this.DataContext.IsDeleted (entity);
			bool isEmpty = this.DataContext.IsRegisteredAsEmptyEntity (entity);

			return !isDeleted && !isEmpty;
		}


	}


}
