//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataContext</c> class provides a context in which entities can
	/// be loaded from the database, modified and then saved back.
	/// </summary>
	public sealed partial class DataContext : System.IDisposable, IEntityPersistenceManager
	{
		public DataContext(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.schemaEngine = SchemaEngine.GetSchemaEngine (this.infrastructure) ?? new SchemaEngine (this.infrastructure);
			this.richCommand = new DbRichCommand (this.infrastructure);
			this.entityContext = EntityContext.Current;
			this.entityDataCache = new EntityDataCache ();
			this.entityTableDefinitions = new Dictionary<Druid, DbTable> ();
			this.temporaryRows = new Dictionary<string, TemporaryRowCollection> ();

			this.entityContext.EntityAttached += this.HandleEntityCreated;
			this.entityContext.PersistenceManagers.Add (this);
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

		public bool BulkMode
		{
			get;
			set;
		}

		/// <summary>
		/// Creates the schema for the specified type.
		/// </summary>
		/// <typeparam name="T">The entity type for which to create the schema.</typeparam>
		/// <returns><c>true</c> if a new table definition was created; otherwise, <c>false</c>.</returns>
		public bool CreateSchema<T>() where T : AbstractEntity, new ()
		{
			T entity = new T ();
			Druid entityId = entity.GetEntityStructuredTypeId ();

			DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);

			if (tableDef == null)
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.schemaEngine.CreateTableDefinition (entityId);
					transaction.Commit ();
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public AbstractEntity CreateEntity(Druid entityId)
		{
			System.Diagnostics.Debug.WriteLine ("Create Entity : " + entityId.ToString ());
			return this.entityContext.CreateEntity (entityId);
		}
		
		public T CreateEntity<T>() where T : AbstractEntity, new ()
		{
			return this.entityContext.CreateEntity<T> ();
		}

		public T CreateEmptyEntity<T>() where T : AbstractEntity, new ()
		{
			return this.entityContext.CreateEmptyEntity<T> ();
		}

		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId)
		{
			return this.InternalResolveEntity (rowKey, entityId, EntityResolutionMode.Load) as AbstractEntity;
		}

		public AbstractEntity ResolveEntity(DbKey rowKey, Druid entityId, EntityResolutionMode mode)
		{
			return this.InternalResolveEntity (rowKey, entityId, mode) as AbstractEntity;
		}

		public T ResolveEntity<T>(DbKey rowKey) where T : AbstractEntity, new ()
		{
			T     entity   = new T ();
			Druid entityId = entity.GetEntityStructuredTypeId ();

			return this.ResolveEntity (rowKey, entityId) as T;
		}

		public AbstractEntity ResolveEntity (Druid realEntityId, Druid askedEntityId, DbKey rowKey, System.Data.DataRow valuesRow, System.Data.DataRow referencesRow)
		{
			Druid baseEntityId =  this.EntityContext.GetBaseEntityId (askedEntityId);

			AbstractEntity entity = this.entityDataCache.FindEntity (rowKey, realEntityId, baseEntityId);
			
			if (entity == null)
			{
				entity = this.entityContext.CreateEmptyEntity (realEntityId);

				this.entityDataCache.DefineRowKey (this.GetEntityDataMapping (entity), rowKey);

				using (entity.DefineOriginalValues ())
				{
					Druid[] entityIds = this.EntityContext.GetHeritedEntityIds (realEntityId).ToArray();

					foreach (Druid currentId in entityIds.TakeWhile (id => id != askedEntityId))
					{
						this.DeserializeEntityLocalWithProxy (entity, currentId, rowKey);
					}

					foreach (Druid currentId in entityIds.SkipWhile(id => id != askedEntityId))
					{
						this.DeserializeEntityLocalWithReference (entity, valuesRow, referencesRow, currentId);
						this.DeserializeEntityLocal (entity, valuesRow, currentId);
					}
				}
			}

			return entity;
		}


		public bool SerializeChanges()
		{
			bool containsChanges = false;

			foreach (AbstractEntity entity in this.GetModifiedEntities ())
			{
				this.SerializeEntity (entity);
				containsChanges = true;
			}

			if (containsChanges)
			{
				this.entityContext.NewDataGeneration ();
			}

			return containsChanges;
		}

		public void SaveChanges()
		{
			// TODO This method will fail and throw an exception if it is called on a DataContext without
			// change. This is because the DbAccess in the RbRichCommand is missing because its InternalFillDataSet
			// method is never called. Something should be done to initialize that DbAccess by some other way.

			System.Diagnostics.Debug.WriteLine ("DataContext: SaveChanges");
			this.SerializeChanges ();

			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.richCommand.SaveTables (transaction, this.SaveTablesFilterImplementation, this.SaveTablesRowIdAssignmentCallbackImplementation);
				transaction.Commit ();
			}
		}

		public IEnumerable<AbstractEntity> GetManagedEntities()
		{
			return this.entityDataCache.GetEntities ();
		}

		public IEnumerable<AbstractEntity> GetManagedEntities(System.Predicate<AbstractEntity> predicate)
		{
			return this.entityDataCache.GetEntities (predicate);
		}

		public IEnumerable<AbstractEntity> GetModifiedEntities()
		{
			long generation = this.entityContext.DataGeneration;

			return this.GetManagedEntities (entity => entity.GetEntityDataGeneration () >= generation);

		}

		/// <summary>
		/// Counts the managed entities.
		/// </summary>
		/// <returns>The number of entities associated to this data context.</returns>
		public int CountManagedEntities()
		{
			return this.entityDataCache.Count;
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

			if ((table.Columns.Contains (Tags.ColumnInstanceType)) ||
				(table.Columns.Contains (Tags.ColumnRefSourceId)))
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
		/// <param name="tableDef">The table definition.</param>
		/// <param name="table">The data table.</param>
		/// <param name="oldKey">The old key.</param>
		/// <param name="newKey">The new key.</param>
		/// <returns>
		/// Always returns <see cref="DbKey.Empty"/>, since it updates the
		/// row keys itself.
		/// </returns>
		private DbKey SaveTablesRowIdAssignmentCallbackImplementation(DbTable tableDef, System.Data.DataTable table, DbKey oldKey, DbKey newKey)
		{
			if (tableDef.Category == DbElementCat.Relation)
			{
				return newKey;
			}
			else
			{
				TemporaryRowCollection temporaryRows;
				temporaryRows = this.GetTemporaryRows (table.TableName);
				temporaryRows.UpdateAssociatedRowKeys (this.richCommand, oldKey, newKey);

				return DbKey.Empty;
			}
		}

		/// <summary>
		/// Serializes the entity to the in-memory data set. This will either
		/// update or create data rows in one or several data tables.
		/// </summary>
		/// <param name="entity">The entity.</param>
		private void SerializeEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			if (mapping.SerialGeneration == this.entityContext.DataGeneration)
			{
				return;
			}
			else
			{
				mapping.SerialGeneration = this.entityContext.DataGeneration;
			}

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

				System.Data.DataRow dataRow = createRow ? this.CreateDataRow (mapping, id) : this.LoadDataRow (mapping.RowKey, id);

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
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.WriteFieldValueInDataRow (entity, fieldDef, dataRow);
						break;

					case FieldRelation.Reference:
						this.WriteFieldReference (entity, entityId, fieldDef);
						break;

					default:
						this.WriteFieldCollection (entity, entityId, fieldDef);
						break;
				}
			}
		}


		internal object InternalResolveEntity(DbKey rowKey, Druid entityId, EntityResolutionMode mode)
		{
			Druid baseEntityId = this.entityContext.GetBaseEntityId (entityId);
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
					return new Helpers.EntityDataProxy (this, rowKey, entityId);

				default:
					throw new System.NotImplementedException (string.Format ("Resolution mode {0} not implemented", mode));
			}
		}


		internal AbstractEntity DeserializeEntity(DbKey rowKey, Druid entityId)
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

			this.entityDataCache.DefineRowKey (mapping, entityKey);

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
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				//	Depending on the relation (and therefore cardinality), write
				//	the data into the row :

				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.ReadFieldValueFromDataRow (entity, fieldDef, dataRow);
						break;

					case FieldRelation.Reference:
						entity.InternalSetValue (fieldDef.Id, Collection.GetFirst (this.ReadFieldRelation(entity, entityId, fieldDef), null));
						break;

					case FieldRelation.Collection:
						System.Collections.IList collection = entity.InternalGetFieldCollection (fieldDef.Id);

						//	TODO: verify that this really works

						foreach (object childEntity in this.ReadFieldRelation(entity, entityId, fieldDef))
						{
							collection.Add (childEntity);
						}
						break;

					default:
						throw new System.NotImplementedException ();
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

						object value = new Helpers.FieldProxy (this, entity, entityId, rowKey, field);
						entity.InternalSetValue (field.Id, value);

						break;

					case FieldRelation.Reference:

						object target1 = this.ReadFieldRelation (entity, entityId, field).FirstOrDefault ();
						entity.InternalSetValue (field.Id, target1);

						break;

					case FieldRelation.Collection:

						System.Collections.IList targets = entity.InternalGetFieldCollection (field.Id);

						foreach (object target2 in this.ReadFieldRelation (entity, entityId, field))
						{
							targets.Add (target2);
						}

						break;
				}
			}
		}


		private void DeserializeEntityLocalWithReference(AbstractEntity entity, System.Data.DataRow valuesRow, System.Data.DataRow referencesRow, Druid entityId)
		{
			foreach (StructuredTypeField fieldDef in this.entityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:

						this.ReadFieldValueFromDataRow (entity, fieldDef, valuesRow);

						break;

					case FieldRelation.Reference:

						object idAsObject = referencesRow[this.SchemaEngine.GetDataColumnName (fieldDef.Id)];

						if (idAsObject != System.DBNull.Value)
						{
							DbKey idAsKey = new DbKey (new DbId (long.Parse (idAsObject as string)));
							object proxy = this.InternalResolveEntity (idAsKey, fieldDef.TypeId, EntityResolutionMode.DelayLoad);
							entity.InternalSetValue (fieldDef.Id, proxy);
						}

						break;

					case FieldRelation.Collection:

						System.Collections.IList collection = entity.InternalGetFieldCollection (fieldDef.Id);

						foreach (object childEntity in this.ReadFieldRelation (entity, entityId, fieldDef))
						{
							collection.Add (childEntity);
						}

						break;
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

			if (UndefinedValue.IsUndefinedValue (value) || nullableType.IsNullValue (value))
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

			string columnName = this.schemaEngine.GetDataColumnName (fieldDef.Id);

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
				
				value = DataContext.ConvertToInternal (value, columnDef);
			}
			
			return value;
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

				DbTable   tableDef  = this.richCommand.Tables[tableName];
				DbColumn  columnDef = tableDef.Columns[columnName];

				value = DataContext.ConvertFromInternal (value, columnDef);
			}

			return value;
		}

		private static object ConvertToInternal(object value, DbColumn columnDef)
		{
			if (value != System.DBNull.Value)
			{
				DbTypeDef typeDef = columnDef.Type;

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

		private static object ConvertFromInternal(object value, DbColumn columnDef)
		{
			if (value != System.DBNull.Value)
			{
				DbTypeDef typeDef = columnDef.Type;
				value = TypeConverter.ConvertToSimpleType (value, typeDef.SimpleType, typeDef.NumDef);
			}

			return value;
		}
		
		private void WriteFieldReference(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			AbstractEntity targetEntity = sourceEntity.InternalGetValue (fieldDef.Id) as AbstractEntity;

			if (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (targetEntity))
			{
				targetEntity = null;
			}

			EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);
			EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

			string relationTableName = this.GetRelationTableName (entityId, fieldDef);

			System.Data.DataRow[] relationRows = DbRichCommand.FilterExistingRows (this.richCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToArray ();

			if (targetEntity != null)
			{
				System.Diagnostics.Debug.Assert (targetMapping != null);

				if (targetMapping.RowKey.IsEmpty)
				{
					this.SerializeEntity (targetEntity);
				}

				if (relationRows.Length == 0)
				{
					this.CreateRelationRow (relationTableName, sourceMapping, targetMapping);
				}
				else if (relationRows.Length == 1)
				{
					this.UpdateRelationRow (relationRows[0], sourceMapping, targetMapping);
				}
				else
				{
					throw new System.InvalidOperationException ();
				}
			}
			else
			{
				if (relationRows.Length == 1)
				{
					this.DeleteRelationRow (relationRows[0]);
				}
				else if (relationRows.Length > 1)
				{
					throw new System.InvalidOperationException ();
				}
			}
		}

		private void WriteFieldCollection(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			System.Collections.IList collection = sourceEntity.InternalGetFieldCollection (fieldDef.Id);

			System.Diagnostics.Debug.Assert (collection != null);

			EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);

			string relationTableName = this.GetRelationTableName (entityId, fieldDef);

			List<System.Data.DataRow> relationRows = DbRichCommand.FilterExistingRows (this.richCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToList ();
			List<System.Data.DataRow> resultingRows = new List<System.Data.DataRow> ();

			for (int i = 0; i < collection.Count; i++)
			{
				AbstractEntity targetEntity  = collection[i] as AbstractEntity;
				EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

				System.Diagnostics.Debug.Assert (targetMapping != null);

				if (targetMapping.RowKey.IsEmpty)
				{
					this.SerializeEntity (targetEntity);
				}

				long targetRowId = targetMapping.RowKey.Id.Value;

				System.Diagnostics.Debug.Assert (targetEntity != null);
				System.Diagnostics.Debug.Assert (targetMapping != null);

				System.Data.DataRow row = DataContext.FindRelationRowForTarget (relationRows, targetRowId);

				if (row == null)
				{
					resultingRows.Add (this.CreateRelationRow (relationTableName, sourceMapping, targetMapping));
				}
				else
				{
					relationRows.Remove (row);
					resultingRows.Add (row);
				}
			}

			foreach (System.Data.DataRow row in relationRows)
			{
				this.DeleteRelationRow (row);
			}

			int rank = -1;

			foreach (System.Data.DataRow row in resultingRows)
			{
				rank++;

				int rowRank = (int) row[Tags.ColumnRefRank];

				if ((rowRank < rank) ||
					(rowRank > rank+1000))
				{
					row[Tags.ColumnRefRank] = rank;
				}
				else if (rowRank > rank)
				{
					rank = rowRank;
				}
			}
		}

		private static System.Data.DataRow FindRelationRowForTarget(IEnumerable<System.Data.DataRow> relationRows, long targetRowId)
		{
			foreach (System.Data.DataRow row in relationRows)
			{
				if ((long) row[Tags.ColumnRefTargetId] == targetRowId)
				{
					return row;
				}
			}

			return null;
		}

		private void UpdateRelationRow(System.Data.DataRow relationRow, EntityDataMapping sourceMapping, EntityDataMapping targetMapping)
		{
			System.Diagnostics.Debug.Assert (sourceMapping.RowKey.Id.Value == (long) relationRow[Tags.ColumnRefSourceId]);
			System.Diagnostics.Debug.Assert (-1 == (int) relationRow[Tags.ColumnRefRank]);

			relationRow.BeginEdit ();
			relationRow[Tags.ColumnRefTargetId] = targetMapping.RowKey.Id.Value;
			relationRow.EndEdit ();
		}

		private System.Data.DataRow CreateRelationRow(string relationTableName, EntityDataMapping sourceMapping, EntityDataMapping targetMapping)
		{
			System.Data.DataRow relationRow = this.richCommand.CreateRow (relationTableName);
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);

			relationRow.BeginEdit ();
			key.SetRowKey (relationRow);
			relationRow[Tags.ColumnRefSourceId] = sourceMapping.RowKey.Id.Value;
			relationRow[Tags.ColumnRefTargetId] = targetMapping.RowKey.Id.Value;
			relationRow[Tags.ColumnRefRank] = -1;
			relationRow.EndEdit ();

			return relationRow;
		}

		private void DeleteRelationRow(System.Data.DataRow relationRow)
		{
			switch (relationRow.RowState)
			{
				case System.Data.DataRowState.Added:
					relationRow.Table.Rows.Remove (relationRow);
					break;

				case System.Data.DataRowState.Modified:
				case System.Data.DataRowState.Unchanged:
					relationRow.Delete ();
					break;

				default:
					throw new System.InvalidOperationException ();
			}
		}

		private string GetRelationTableName(Druid entityId, StructuredTypeField fieldDef)
		{
			string sourceTableName = this.schemaEngine.GetDataTableName (entityId);
			string sourceColumnName = this.schemaEngine.GetDataColumnName (fieldDef.Id);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}

		public object GetFieldValue(AbstractEntity entity, Druid entityId, DbKey rowKey, StructuredTypeField fieldDef)
		{
			System.Data.DataRow dataRow = this.LoadDataRow (rowKey, entityId);

			return this.GetFieldValue (entity, fieldDef, dataRow);
		}

		private object GetFieldValue(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			string columnName = this.schemaEngine.GetDataColumnName (fieldDef.Id);

			System.Diagnostics.Debug.Assert (fieldDef.Expression == null);
			System.Diagnostics.Debug.Assert (dataRow.Table.Columns.Contains (columnName));

			object value = dataRow[columnName];

			if (System.DBNull.Value != value)
			{
				IStringType stringType = fieldDef.Type as IStringType;

				if (stringType != null)
				{
					if (stringType.UseFormattedText)
					{
						value = FormattedText.CastToFormattedText (value);
					}
				}
				else
				{
					var entityId  = entity.GetEntityStructuredTypeId ();
					var tableName = this.schemaEngine.GetDataTableName (entityId);

					value = this.ConvertFromInternal (value, tableName, columnName);
				}
			}

			return value;
		}

		private void ReadFieldValueFromDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			object value = this.GetFieldValue (entity, fieldDef, dataRow);

			if (value != System.DBNull.Value)
			{
				entity.InternalSetValue (fieldDef.Id, value);
			}
		}

		private IEnumerable<object> ReadFieldRelation(AbstractEntity entity, Druid entityId, StructuredTypeField fieldDef)
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
					object targetEntity = this.InternalResolveEntity (new DbKey (new DbId (relationTargetId)), fieldDef.TypeId, EntityResolutionMode.DelayLoad);
					yield return targetEntity;
					found = true;
				}

				if (found)
				{
					yield break;
				}
				else
				{
					this.LoadRelationRows (entityId, tableName, sourceMapping.RowKey);
				}
			}
		}

		private System.Data.DataRow LoadDataRow(DbKey rowKey, Druid entityId)
		{
			System.Data.DataRow row;

			string tableName = this.schemaEngine.GetDataTableName (entityId);
			row = this.richCommand.FindRow (tableName, rowKey.Id);

			if (row == null)
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);
					DbSelectCondition condition = this.infrastructure.CreateSelectCondition (DbSelectRevision.LiveActive);

					if (!this.BulkMode)
					{
						condition.AddCondition (new DbTableColumn (tableDef.Columns[Tags.ColumnId]), DbCompare.Equal, rowKey.Id.Value);
					}

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

				if (!this.BulkMode)
				{
					condition.AddCondition (new DbTableColumn (relationTableDef.Columns[Tags.ColumnRefSourceId]), DbCompare.Equal, sourceRowKey.Id.Value);
				}

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

			string tableName = this.schemaEngine.GetDataTableName (entityId);
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
			string baseTableName = this.schemaEngine.GetDataTableName (baseEntityId);

			return this.GetTemporaryRows (baseTableName);
		}

		
		internal EntityDataMapping GetEntityDataMapping(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}

			EntityDataMapping mapping = this.FindEntityDataMapping (entity);

			if (mapping != null)
			{
				return mapping;
			}
			else
			{
				throw new System.ArgumentException ("Entity not managed by the DataContext");
			}
		}
		
		internal EntityDataMapping FindEntityDataMapping(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}
			else
			{
				return this.entityDataCache.FindMapping (entity.GetEntitySerialId ());
			}
		}


		internal void LoadEntitySchema(Druid entityId)
		{
			if (this.entityTableDefinitions.ContainsKey (entityId))
			{
				//	Nothing to do. The schema has already been loaded.
			}
			else
			{
				DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);

				if (tableDef == null)
				{
					StructuredType type = this.entityContext.GetStructuredType (entityId) as StructuredType;

					if (type == null)
					{
						throw new System.ArgumentException (string.Format ("No schema nor type information available for EntityId {0}", entityId));
					}
					else
					{
						throw new System.ArgumentException (string.Format ("No schema available for EntityId {0} ({1})", type.Caption.Name, entityId));
					}
				}

				this.entityTableDefinitions[entityId] = tableDef;
				this.LoadTableSchema (tableDef);
				
				StructuredType entityType = this.entityContext.GetStructuredType (entityId) as StructuredType;
				Druid          baseTypeId = entityType.BaseTypeId;

				if (baseTypeId.IsValid)
				{
					this.LoadEntitySchema (baseTypeId);
				}
			}
		}

		internal void LoadTableSchema(DbTable tableDefinition)
		{
			if (this.richCommand.Tables.Contains (tableDefinition.Name))
			{
				//	Nothing to do, we already know this table.
			}
			else
			{
				using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					this.richCommand.ImportTable (transaction, tableDefinition, null);
					this.LoadTableRelationSchemas (transaction, tableDefinition);

					transaction.Commit ();
				}
			}
		}

		private void LoadTableRelationSchemas(Druid entityId)
		{
			DbTable tableDef = this.schemaEngine.FindTableDefinition (entityId);

			using (DbTransaction transaction = this.infrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.LoadTableRelationSchemas (transaction, tableDef);

				transaction.Commit ();
			}
		}

		private void LoadTableRelationSchemas(DbTransaction transaction, DbTable tableDefinition)
		{
			foreach (DbColumn columnDefinition in tableDefinition.Columns)
			{
				if (columnDefinition.Cardinality != DbCardinality.None)
				{
					string relationTableName = tableDefinition.GetRelationTableName (columnDefinition);
					DbTable relationTableDef = this.infrastructure.ResolveDbTable (transaction, relationTableName);
					this.richCommand.ImportTable (transaction, relationTableDef, null);
				}
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dipose (true);
		}

		#endregion

		public bool IsPersistent(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.FindEntityDataMapping (entity);

			if ((mapping != null) &&
				(mapping.RowKey.IsEmpty == false))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#region IEntityPersistenceManager Members

		public string GetPersistedId(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.FindEntityDataMapping (entity);

			if ((mapping != null) &&
				(mapping.RowKey.IsEmpty == false))
			{
				Druid entityId  = entity.GetEntityStructuredTypeId ();
				long  keyId     = mapping.RowKey.Id;
				short keyStatus = DbKey.ConvertToIntStatus (mapping.RowKey.Status);

				if (keyStatus == 0)
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}", entityId, keyId);
				}
				else
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}:{2}", entityId, keyId, keyStatus);
				}
			}

			return null;
		}

		public AbstractEntity GetPeristedEntity(string id)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}
			if (id.StartsWith ("db:"))
			{
				string[] args = id.Split (':');
				DbKey    key  = DbKey.Empty;

				Druid entityId = Druid.Empty;
				long  keyId;
				short keyStatus;

				switch (args.Length)
				{
					case 3:
						entityId = Druid.Parse (args[1]);
						
						if ((entityId.IsValid) &&
							(long.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyId)))
						{
							key = new DbKey (keyId);
						}
						break;

					case 4:
						entityId = Druid.Parse (args[1]);

						if ((entityId.IsValid) &&
							(long.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyId)) &&
							(short.TryParse (args[3], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyStatus)))
						{
							key = new DbKey (keyId, DbKey.ConvertFromIntStatus (keyStatus));
						}
						break;
				}

				if (!key.IsEmpty)
				{
					return this.ResolveEntity (key, entityId);
				}
			}

			return null;
		}

		#endregion

		private void Dipose(bool disposing)
		{
			if (disposing)
			{
				this.entityContext.EntityAttached -= this.HandleEntityCreated;
				this.entityContext.PersistenceManagers.Remove (this);
			}
		}

		private void HandleEntityCreated(object sender, EntityEventArgs e)
		{
			AbstractEntity entity = e.Entity;
			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid baseEntityId = this.entityContext.GetBaseEntityId (entityId);
			long entitySerialId = entity.GetEntitySerialId ();

			EntityDataMapping mapping = new EntityDataMapping (entity, entityId, baseEntityId);

			this.entityDataCache.Add (entitySerialId, mapping);

			try
			{
				this.LoadEntitySchema (entityId);
			}
			catch
			{
				this.entityDataCache.Remove (entitySerialId);
				throw;
			}
		}

		private readonly DbInfrastructure infrastructure;
		private readonly DbRichCommand richCommand;
		private readonly SchemaEngine schemaEngine;
		private readonly EntityContext entityContext;
		private readonly EntityDataCache entityDataCache;
		private readonly Dictionary<Druid, DbTable> entityTableDefinitions;
		private readonly Dictionary<string, TemporaryRowCollection> temporaryRows;
	}
}
