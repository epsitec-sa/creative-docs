//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	public sealed partial class DataContext : System.IDisposable
	{
		public DataContext(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.richCommand = new DbRichCommand (this.infrastructure);
			this.schemaEngine = new SchemaEngine (this.infrastructure);
			this.entityContext = EntityContext.Current;
			this.entityDataMapping = new Dictionary<long, EntityDataMapping> ();
			this.entityTableDefinitions = new Dictionary<Druid, DbTable> ();
			this.temporaryRows = new Dictionary<string, TemporaryRowCollection> ();

			this.entityContext.EntityCreated += this.HandleEntityCreated;
		}

		public SchemaEngine SchemaEngine
		{
			get
			{
				return this.schemaEngine;
			}
		}

		public EntityContext EntityContext
		{
			get
			{
				return this.entityContext;
			}
		}

		public DbRichCommand RichCommand
		{
			get
			{
				return this.richCommand;
			}
		}

		/// <summary>
		/// Counts the managed entities.
		/// </summary>
		/// <returns>The number of entities associated to this data context.</returns>
		public int CountManagedEntities()
		{
			return this.entityDataMapping.Count;
		}

		public void SaveChanges()
		{
			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.richCommand.SaveTables (transaction, this.SaveTablesFilterImplementation, this.SaveTablesRowIdAssignmentCallbackImplementation);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// This is the implementation of the filter for the
		/// <see cref="DbRichCommand.SaveTables(DbTransaction, System.Predicate&lt;System.Data.DataTable&gt;, DbRichCommand.RowIdAssignmentCallback)"/>
		/// method.
		/// </summary>
		/// <param name="table">The data table.</param>
		/// <returns><c>true</c> if the table should be processed; otherwise, <c>false</c>.</returns>
		private bool SaveTablesFilterImplementation(System.Data.DataTable table)
		{
			//	If the table contains the data for a root entity (the one which
			//	has no base type, i.e. no parent class), then let DbRichCommand
			//	attribute the DbKey for its rows.
			//	
			//	Tables which contain the data for derived entities should be
			//	skipped. See method SaveTablesRowIdAssignmentCallbackImplementation.

			if (table.Columns.Contains (Tags.ColumnInstanceType))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// This is the implementation of the raw ID assignment callback for the
		/// <see cref="DbRichCommand.SaveTables(DbTransaction, System.Predicate&lt;System.Data.DataTable&gt;, DbRichCommand.RowIdAssignmentCallback)"/>
		/// method. When this method is called, it updates the <see cref="DbKey"/>
		/// for the rows which contain the data for the associated entity.
		/// </summary>
		/// <param name="table">The data table.</param>
		/// <param name="oldKey">The old key.</param>
		/// <param name="newKey">The new key.</param>
		/// <returns>Always returns <see cref="DbKey.Empty"/>, since it updates the
		/// row keys itself.</returns>
		private DbKey SaveTablesRowIdAssignmentCallbackImplementation(System.Data.DataTable table, DbKey oldKey, DbKey newKey)
		{
			TemporaryRowCollection temporaryRows;
			temporaryRows = this.GetTemporaryRows (table.TableName);
			temporaryRows.UpdateAssociatedRowKeys (this.richCommand, oldKey, newKey);

			return DbKey.Empty;
		}

		/// <summary>
		/// Serializes the entity to the in-memory data set. This will either
		/// update or create data rows in one or several data tables.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void SerializeEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);
			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid id = entityId;
			DbKey rowKey = mapping.RowKey;
			bool createRow;

			if (rowKey.IsEmpty)
			{
				rowKey = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
				mapping.RowKey = rowKey;
				createRow = true;
			}
			else
			{
				createRow = false;
			}
			

			while (id.IsValid)
			{
				StructuredType entityType = this.entityContext.GetStructuredType (id) as StructuredType;
				Druid          baseTypeId = entityType.BaseTypeId;

				System.Diagnostics.Debug.Assert (entityType != null);
				System.Diagnostics.Debug.Assert (entityType.CaptionId == id);

				//	Either create and fill a new row in the database for this entity
				//	or use and update an existing row.

				System.Data.DataRow dataRow = createRow ? this.CreateDataRow (mapping, id) : this.FindDataRow (mapping.RowKey, id);
				
				dataRow.BeginEdit ();

				//	If this is the root entity in the entity hierarchy (it has no base
				//	type), then we will have to save the instance type identifying the
				//	entity.
				
				if (baseTypeId.IsEmpty)
				{
					dataRow[Tags.ColumnInstanceType] = this.GetInstanceTypeValue (entityId);
				}

				this.SerializeEntityLocal (entity, dataRow, id);
				
				dataRow.EndEdit ();

				id = baseTypeId;
			}
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
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityFieldDefinitions (entityId))
			{
				//	Only process fields which are defined locally, since inherited
				//	fields do not belong into the same data table.

				if (fieldDef.Membership == FieldMembership.Inherited)
				{
					continue;
				}
				
				//	Depending on the relation (and therefore cardinality), write
				//	the data into the row :
				
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.WriteFieldValueInDataRow (entity, fieldDef, dataRow);
						break;

					case FieldRelation.Reference:
						this.WriteFieldReference (entity, entityId, fieldDef);
						break;

					default:
						//	TODO: ...
						break;
				}
			}
		}

		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId)
		{
			Druid baseEntityId = this.entityContext.GetBaseEntityId (entityId);

			foreach (EntityDataMapping mapping in this.entityDataMapping.Values)
			{
				if (mapping.Equals (rowKey, baseEntityId))
				{
					return mapping.Entity;
				}
			}

			return this.DeserializeEntity (rowKey, entityId);
		}

		public AbstractEntity DeserializeEntity(DbKey rowKey, Druid entityId)
		{
			Druid baseEntityId = this.entityContext.GetBaseEntityId (entityId);

			System.Data.DataRow dataRow = this.LoadDataRow (rowKey, baseEntityId);
			long typeValueId = (long) dataRow[Tags.ColumnInstanceType];
			Druid realEntityId = Druid.FromLong (typeValueId);
			AbstractEntity entity = this.entityContext.CreateEmptyEntity (realEntityId);

			using (entity.DefineOriginalValues ())
			{
				this.DeserializeEntity (entity, realEntityId, rowKey);
			}
			
			return entity;
		}

		private void DeserializeEntity(AbstractEntity entity, Druid entityId, DbKey entityKey)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			System.Diagnostics.Debug.Assert (mapping.EntityId == entityId);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsEmpty);

			mapping.RowKey = entityKey;

			Druid id = entityId;

			while (id.IsValid)
			{
				StructuredType entityType = this.entityContext.GetStructuredType (id) as StructuredType;
				Druid baseTypeId = entityType.BaseTypeId;

				System.Diagnostics.Debug.Assert (entityType != null);
				System.Diagnostics.Debug.Assert (entityType.CaptionId == id);

				System.Data.DataRow dataRow = this.LoadDataRow (mapping.RowKey, id);
				
				this.DeserializeEntityLocal (entity, dataRow, id);
				
				id = baseTypeId;
			}
		}

		private void DeserializeEntityLocal(AbstractEntity entity, System.Data.DataRow dataRow, Druid entityId)
		{
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityFieldDefinitions (entityId))
			{
				//	Only process fields which are defined locally, since inherited
				//	fields do not belong into the same data table.

				if (fieldDef.Membership == FieldMembership.Inherited)
				{
					continue;
				}

				//	Depending on the relation (and therefore cardinality), write
				//	the data into the row :

				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.ReadFieldValueFromDataRow (entity, fieldDef, dataRow);
						break;

					case FieldRelation.Reference:
						entity.InternalSetValue (fieldDef.Id, Collection.GetFirst<AbstractEntity> (this.ReadFieldRelation (entity, entityId, fieldDef, dataRow), null));
						break;

					case FieldRelation.Collection:
						System.Collections.IList collection = entity.InternalGetFieldCollection (fieldDef.Id);

						//	TODO: verify that this really works

						foreach (AbstractEntity childEntity in this.ReadFieldRelation (entity, entityId, fieldDef, dataRow))
						{
							collection.Add (childEntity);
						}
						break;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		private object GetInstanceTypeValue(Druid entityId)
		{
			return entityId.ToLong ();
		}

		private void WriteFieldValueInDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			object value = entity.InternalGetValue (fieldDef.Id);

			System.Diagnostics.Debug.Assert (!UnknownValue.IsUnknownValue (value));

			AbstractType  fieldType = fieldDef.Type as AbstractType;
			INullableType nullableType = fieldDef.GetNullableType ();

			if ((UndefinedValue.IsUndefinedValue (value)) ||
				(nullableType.IsNullValue (value)))
			{
				if (nullableType.IsNullable)
				{
					value = System.DBNull.Value;
				}
				else
				{
					value = fieldType.DefaultValue;
				}
			}

			string columnName = this.GetDataColumnName (fieldDef);

			dataRow[columnName] = this.ConvertToInternal (value, dataRow.Table.TableName, columnName);
		}

		private object ConvertToInternal(object value, string tableName, string columnName)
		{
			if (value == System.DBNull.Value)
			{
				//	Nothing to convert : a DBNull value stays a DBNull value.
			}
			else
			{
				System.Diagnostics.Debug.Assert (value != null);

				DbTable   tableDef  = this.richCommand.Tables[tableName];
				DbColumn  columnDef = tableDef.Columns[columnName];
				DbTypeDef typeDef   = columnDef.Type;

				if (typeDef.SimpleType == DbSimpleType.Decimal)
				{
					decimal decimalValue;

					if (InvariantConverter.Convert (value, out decimalValue))
					{
						value = decimalValue;
					}
					else
					{
						throw new System.ArgumentException ("Invalid value: not compatible with a numeric type");
					}
				}

				value = TypeConverter.ConvertFromSimpleType (value, typeDef.SimpleType, typeDef.NumDef);
			}
			return value;
		}

		private void WriteFieldReference(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			string tableName = this.GetRelationTableName (entityId, fieldDef);

			AbstractEntity targetEntity = sourceEntity.InternalGetValue (fieldDef.Id) as AbstractEntity;

			if (targetEntity != null)
			{
				EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);
				EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

				if (targetMapping.RowKey.IsEmpty)
				{
					this.SerializeEntity (targetEntity);
				}

				System.Data.DataRow relationRow = this.richCommand.CreateRow (tableName);

				relationRow.BeginEdit ();
				relationRow[Tags.ColumnRefSourceId] = sourceMapping.RowKey.Id.Value;
				relationRow[Tags.ColumnRefTargetId] = targetMapping.RowKey.Id.Value;
				relationRow[Tags.ColumnStatus] = DbKey.ConvertToIntStatus (DbRowStatus.Live);
				relationRow[Tags.ColumnRefRank] = -1;
				relationRow.EndEdit ();
			}
			else
			{
				EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);
				long sourceId = sourceMapping.RowKey.Id.Value;

				System.Data.DataTable relationTable = this.richCommand.DataSet.Tables[tableName];

				for (int i = 0; i < relationTable.Rows.Count; )
				{
					System.Data.DataRow relationRow = relationTable.Rows[i];

					if ((long) relationRow[Tags.ColumnRefSourceId] == sourceId)
					{
						switch (relationRow.RowState)
						{
							case System.Data.DataRowState.Added:
								relationTable.Rows.RemoveAt (i);
								break;

							case System.Data.DataRowState.Deleted:
								i++;
								break;

							case System.Data.DataRowState.Modified:
							case System.Data.DataRowState.Unchanged:
								relationRow.Delete ();
								i++;
								break;

							default:
								throw new System.NotImplementedException ();
						}
					}
				}
			}
		}

		private string GetRelationTableName(Druid entityId, StructuredTypeField fieldDef)
		{
			string sourceTableName = this.GetDataTableName (entityId);
			string sourceColumnName = this.GetDataColumnName (fieldDef);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}

		private void ReadFieldValueFromDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			string columnName = this.GetDataColumnName (fieldDef);
			object value = dataRow[columnName];

			if (System.DBNull.Value == value)
			{
				//	Undefined value. Do nothing.
			}
			else
			{
				entity.InternalSetValue (fieldDef.Id, value);
			}
		}

		private IEnumerable<AbstractEntity> ReadFieldRelation(AbstractEntity entity, Druid entityId, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			EntityDataMapping sourceMapping = this.GetEntityDataMapping (entity);
			string tableName = this.GetRelationTableName (entityId, fieldDef);
			bool found = false;
			System.Comparison<System.Data.DataRow> comparer = null;

			if (fieldDef.Relation == FieldRelation.Collection)
			{
				//	TODO: check that comparer really works !

				comparer =
					delegate (System.Data.DataRow a, System.Data.DataRow b)
					{
						int valueA = (int) a[Tags.ColumnRefRank];
						int valueB = (int) b[Tags.ColumnRefRank];

						return valueA.CompareTo (valueB);
					};
			}

			for (int i = 0; i < 2; i++)
			{
				foreach (System.Data.DataRow relationRow in Collection.Enumerate (this.richCommand.FindRelationRows (tableName, sourceMapping.RowKey.Id), comparer))
				{
					long relationTargetId = (long) relationRow[Tags.ColumnRefTargetId];
					AbstractEntity targetEntity = this.ResolveEntity (new DbKey (new DbId (relationTargetId)), fieldDef.TypeId);
					yield return targetEntity;
					found = true;
				}

				if (found)
				{
					yield break;
				}

				this.LoadRelationRows (entityId, tableName, sourceMapping.RowKey);
			}
		}

		/// <summary>
		/// Finds the data row given a row key and an entity.
		/// </summary>
		/// <param name="rowKey">The row key.</param>
		/// <param name="entityId">The entity id.</param>
		/// <returns>The data row or <c>null</c>.</returns>
		private System.Data.DataRow FindDataRow(DbKey rowKey, Druid entityId)
		{
			System.Diagnostics.Debug.Assert (entityId.IsValid);
			System.Diagnostics.Debug.Assert (rowKey.IsEmpty == false);
			
			string tableName = this.GetDataTableName (entityId);
			return this.richCommand.FindRow (tableName, rowKey.Id);
		}

		private System.Data.DataRow LoadDataRow(DbKey rowKey, Druid entityId)
		{
			System.Data.DataRow row;

			string tableName = this.GetDataTableName (entityId);
			row = this.richCommand.FindRow (tableName, rowKey.Id);

			if (row == null)
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);
					DbSelectCondition condition = this.infrastructure.CreateSelectCondition (DbSelectRevision.LiveActive);
					condition.AddCondition (tableDef.Columns[Tags.ColumnId], DbCompare.Equal, rowKey.Id.Value);
					this.richCommand.ImportTable (transaction, tableDef, condition);
					this.LoadTableRelationSchemas (transaction, tableDef);
					transaction.Commit ();
				}

				row = this.richCommand.FindRow (tableName, rowKey.Id);
			}

			return row;
		}
		
		private void LoadRelationRows(Druid entityId, string tableName, DbKey sourceRowKey)
		{
			DbTable relationTableDef = this.richCommand.Tables[tableName];

			if (relationTableDef == null)
			{
				this.LoadTableRelationSchemas (entityId);

				relationTableDef = this.richCommand.Tables[tableName];

				System.Diagnostics.Debug.Assert (relationTableDef != null);
			}

			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbSelectCondition condition = this.infrastructure.CreateSelectCondition ();
				condition.AddCondition (relationTableDef.Columns[Tags.ColumnRefSourceId], DbCompare.Equal, sourceRowKey.Id.Value);
				this.richCommand.ImportTable (transaction, relationTableDef, condition);
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Creates a new data row for the specified entity. The row will store
		/// data only for the locally defined fields of the given entity.
		/// </summary>
		/// <param name="mapping">The entity mapping.</param>
		/// <param name="entityId">The entity id.</param>
		/// <returns>A new data row with a temporary ID.</returns>
		private System.Data.DataRow CreateDataRow(EntityDataMapping mapping, Druid entityId)
		{
			System.Diagnostics.Debug.Assert (mapping.EntityId.IsValid);
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsTemporary);

			string tableName = this.GetDataTableName (entityId);
			System.Data.DataRow row = this.richCommand.CreateRow (tableName);

			TemporaryRowCollection temporaryRows;
			temporaryRows = this.GetTemporaryRows (mapping.BaseEntityId);
			temporaryRows.AssociateRow (this.richCommand, mapping, row);
			
			return row;
		}

		/// <summary>
		/// Gets the temporary row collection.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The <see cref="TemporaryRowCollection"/> instance.</returns>
		private TemporaryRowCollection GetTemporaryRows(string tableName)
		{
			TemporaryRowCollection rowCollection;

			if (this.temporaryRows.TryGetValue (tableName, out rowCollection) == false)
			{
				rowCollection = new TemporaryRowCollection ();
				this.temporaryRows[tableName] = rowCollection;
			}

			return rowCollection;
		}

		/// <summary>
		/// Gets the temporary row collection.
		/// </summary>
		/// <param name="baseEntityId">The base entity id.</param>
		/// <returns>
		/// The <see cref="TemporaryRowCollection"/> instance.
		/// </returns>
		private TemporaryRowCollection GetTemporaryRows(Druid baseEntityId)
		{
			string baseTableName = this.GetDataTableName (baseEntityId);

			return this.GetTemporaryRows (baseTableName);
		}

		private string GetDataTableName(Druid entityId)
		{
			DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);
			return tableDef == null ? null : tableDef.Name;
		}

		private string GetDataColumnName(StructuredTypeField field)
		{
			System.Diagnostics.Debug.Assert (field.Id.StartsWith ("["));
			System.Diagnostics.Debug.Assert (field.Id.EndsWith ("]"));

			string fieldName = field.Id;
			string nakedName = fieldName.Substring (1, fieldName.Length-2);

			return nakedName;
		}

		public EntityDataMapping GetEntityDataMapping(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			EntityDataMapping mapping;

			if (this.entityDataMapping.TryGetValue (entity.GetEntitySerialId (), out mapping))
			{
				return mapping;
			}
			else
			{
				throw new System.ArgumentException ("Entity not managed by the DataContext");
			}
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Dipose (true);
		}

		#endregion

		private void Dipose(bool disposing)
		{
			if (disposing)
			{
				this.entityContext.EntityCreated -= this.HandleEntityCreated;
			}
		}

		private void HandleEntityCreated(object sender, EntityEventArgs e)
		{
			AbstractEntity entity = e.Entity;
			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid baseEntityId = this.entityContext.GetBaseEntityId (entityId);
			long entitySerialId = entity.GetEntitySerialId ();

			this.entityDataMapping[entitySerialId] = new EntityDataMapping (entity, entityId, baseEntityId);
			this.LoadEntitySchema (entityId);
		}

		readonly DbInfrastructure infrastructure;
		readonly DbRichCommand richCommand;
		readonly SchemaEngine schemaEngine;
		readonly EntityContext entityContext;
		readonly Dictionary<long, EntityDataMapping> entityDataMapping;
		readonly Dictionary<Druid, DbTable> entityTableDefinitions;
		readonly Dictionary<string, TemporaryRowCollection> temporaryRows;
	}
}
