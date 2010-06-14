//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.EntityData;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataContext</c> class provides a context in which entities can
	/// be loaded from the database, modified and then saved back.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay ("DataContext #{UniqueId}")]
	public sealed partial class DataContext : System.IDisposable, IEntityPersistenceManager
	{
		public DataContext(DbInfrastructure infrastructure, bool bulkMode = false)
		{
			this.uniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);

			this.BulkMode = bulkMode;
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure) ?? new SchemaEngine (this.DbInfrastructure);
			this.RichCommand = new DbRichCommand (this.DbInfrastructure);
			this.entityContext = EntityContext.Current;
			this.entityDataCache = new EntityDataCache ();
			this.tableBulkLoaded = new Dictionary<string, bool> ();
			this.dataLoadedEntities = new HashSet<AbstractEntity> ();
			this.relationLoadedEntity = new Dictionary<Druid, HashSet<AbstractEntity>> ();
			this.entityTableDefinitions = new Dictionary<Druid, DbTable> ();
			this.temporaryRows = new Dictionary<string, TemporaryRowCollection> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.deletedEntities = new HashSet<AbstractEntity> ();

			this.EntityContext.EntityAttached += this.HandleEntityCreated;
			this.EntityContext.PersistenceManagers.Add (this);
		}

		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}

		public SchemaEngine SchemaEngine
		{
			get;
			private set;
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
			get;
			private set;
		}

		public bool BulkMode
		{
			get;
			private set;
		}

		public bool EnableEntityNullReferenceVirtualizer
		{
			get;
			set;
		}

		public int UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		/// <summary>
		/// Creates the schema for the specified type.
		/// </summary>
		/// <typeparam name="TEntity">The entity type for which to create the schema.</typeparam>
		/// <returns><c>true</c> if a new table definition was created; otherwise, <c>false</c>.</returns>
		public bool CreateSchema<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			TEntity entity = new TEntity ();
			Druid entityId = entity.GetEntityStructuredTypeId ();

			DbTable tableDef = this.SchemaEngine.FindTableDefinition (entityId);

			if (tableDef == null)
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.SchemaEngine.CreateTableDefinition (entityId);
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
			return this.EntityContext.CreateEntity (entityId);
		}
		
		public TEntity CreateEntity<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			return this.EntityContext.CreateEntity<TEntity> ();
		}

		public TEntity CreateEmptyEntity<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			return this.EntityContext.CreateEmptyEntity<TEntity> ();
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

		internal AbstractEntity ResolveEntity(EntityDataContainer entityData, bool loadFromDatabase)
		{
			return this.InternalResolveEntity (entityData, loadFromDatabase);
		}

		public EntityKey GetEntityKey(AbstractEntity entity)
		{
			var mapping = this.GetEntityDataMapping (entity);

			if (mapping == null)
			{
				return EntityKey.Empty;
			}
			else
			{
				return new EntityKey (mapping.RowKey, entity.GetEntityStructuredTypeId ());
			}
		}


		public bool Contains(AbstractEntity entity)
		{
			if (entity == null)
			{
				throw new System.ArgumentNullException ("entity");
			}

			return this.entityDataCache.ContainsEntity (entity);
		}

		public void DeleteEntity(AbstractEntity entity)
		{
			if (!this.deletedEntities.Contains (entity))
			{
				this.entitiesToDelete.Add (entity);
			}
		}


		public bool SerializeChanges()
		{
			bool containsChanges = false;

			foreach (AbstractEntity entity in this.entitiesToDelete)
			{
				this.RemoveEntity (entity);
				this.deletedEntities.Add (entity);
				this.entityDataCache.Remove (entity.GetEntitySerialId ());
				containsChanges = true;
			}

			this.deletedEntities.Clear ();
			
			foreach (AbstractEntity entity in this.GetModifiedEntities ().Except(this.deletedEntities))
			{
				this.SerializeEntity (entity);
				containsChanges = true;
			}

			if (containsChanges)
			{
				this.EntityContext.NewDataGeneration ();
			}

			return containsChanges;
		}
		
		public void SaveChanges()
		{
			bool containsChanges = this.SerializeChanges ();

			if (containsChanges)
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
				{
					this.RichCommand.SaveTables (transaction, this.SaveTablesFilterImplementation, this.SaveTablesRowIdAssignmentCallbackImplementation);
					transaction.Commit ();
				}
			}
		}


		public bool IsDeleted(AbstractEntity entity)
		{
			return this.deletedEntities.Contains (entity);
		}


		public IEnumerable<AbstractEntity> GetManagedEntities()
		{
			return this.entityDataCache.GetEntities ();
		}

		public IEnumerable<AbstractEntity> GetModifiedEntities()
		{
			return this.GetManagedEntities ().Where (entity => entity.GetEntityDataGeneration () >= this.EntityContext.DataGeneration);
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

			return table.Columns.Contains (Tags.ColumnInstanceType) || table.Columns.Contains (Tags.ColumnRefSourceId);
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
				EntityDataMapping mapping = temporaryRows.UpdateAssociatedRowKeys (this.RichCommand, oldKey, newKey);

				if (mapping != null && mapping.IsReadOnly)
				{
					this.entityDataCache.DefineRowKey (mapping);
				}

				return DbKey.Empty;
			}
		}


		private void RemoveEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			if (mapping.SerialGeneration != this.EntityContext.DataGeneration)
			{
				mapping.SerialGeneration = this.EntityContext.DataGeneration;

				if (!mapping.RowKey.IsEmpty)
				{
					this.LoadDataRows (entity);
					
					this.RemoveEntityValueData (entity, mapping.RowKey);
					this.RemoveEntitySourceReferenceData (entity, mapping.RowKey);
					this.RemoveEntityTargetReferenceDataInMemory (entity);
					this.RemoveEntityTargetReferenceDataInDatabase (entity);
				}
			}
		}


		private void RemoveEntityValueData(AbstractEntity entity, DbKey entityKey)
		{
			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			{
				this.RichCommand.DeleteExistingRow (this.LoadDataRow(entity, entityKey, currentId));
			}
		}


		private void RemoveEntitySourceReferenceData(AbstractEntity entity, DbKey entityKey)
		{
			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			{
				IEnumerable<StructuredTypeField> localRelationFields = this.EntityContext.GetEntityLocalFieldDefinitions (currentId).Where (f => f.Relation == FieldRelation.Reference || f.Relation == FieldRelation.Collection);

				foreach (StructuredTypeField field in localRelationFields)
				{
					string relationTableName = this.GetRelationTableName (currentId, field);

					IEnumerable<System.Data.DataRow> relationRows = this.RichCommand.FindRelationRows (relationTableName, entityKey.Id);
					System.Data.DataRow[] existingRelationRows = DbRichCommand.FilterExistingRows (relationRows).ToArray ();

					foreach (System.Data.DataRow row in existingRelationRows)
					{
						this.DeleteRelationRow (row);
					}
				}
			}
		}


		private void RemoveEntityTargetReferenceDataInMemory(AbstractEntity entity)
		{
			foreach (System.Tuple<AbstractEntity, EntityFieldPath> item in new DataBrowser (this).GetReferencers (entity, false))
			{
				AbstractEntity sourceEntity = item.Item1;
				StructuredTypeField field = this.EntityContext.GetStructuredTypeField (sourceEntity, item.Item2.Fields.First ());

				using (sourceEntity.UseSilentUpdates ())
				{
					switch (field.Relation)
					{
						case FieldRelation.Reference:
							sourceEntity.InternalSetValue (field.Id, null);
							break;
							
						case FieldRelation.Collection:

							IList collection = sourceEntity.InternalGetFieldCollection (field.Id) as IList;

							while (collection.Contains (entity))
							{
								collection.Remove (entity);
							}

							break;

						default:
							throw new System.InvalidOperationException ();
					}
				}
			}
		}


		private void RemoveEntityTargetReferenceDataInDatabase(AbstractEntity entity)
		{
			EntityDataMapping targetMapping = this.FindEntityDataMapping (entity);

			List<EntityFieldPath> sources = new List<EntityFieldPath> ();

			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			{
				sources.AddRange (this.DbInfrastructure.GetSourceReferences (currentId));
			}

			SqlFieldList fields = new Database.Collections.SqlFieldList ();
			fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (DbKey.ConvertToIntStatus (DbRowStatus.Deleted), DbKey.RawTypeForStatus));

			SqlFieldList conditions = new Database.Collections.SqlFieldList ();
			SqlField nameColId = SqlField.CreateName (Tags.ColumnRefTargetId);
			SqlField constantId = SqlField.CreateConstant (targetMapping.RowKey.Id, DbKey.RawTypeForId);

			conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, nameColId, constantId));

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction ())
			{
				foreach (EntityFieldPath source in sources)
				{
					string sourceTableName = this.SchemaEngine.GetDataTableName (source.EntityId);
					string sourceColumnName = this.SchemaEngine.GetDataColumnName (source.Fields[0]);
					string relationTableName = DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
					DbTable relationTable = this.DbInfrastructure.ResolveDbTable (relationTableName);

					this.RichCommand.Update (transaction, relationTable, fields, conditions);
				}

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Serializes the entity to the in-memory data set. This will either
		/// update or create data rows in one or several data tables.
		/// </summary>
		/// <param name="entity">The entity.</param>
		private void SerializeEntity(AbstractEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			if (!this.Contains (entity))
            {
				//	TODO: should we propagate the serialization to another DataContext ?
				return;
            }

			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			System.Diagnostics.Debug.Assert (mapping != null);

			if (mapping.SerialGeneration == this.EntityContext.DataGeneration)
			{
				return;
			}
			else
			{
				mapping.SerialGeneration = this.EntityContext.DataGeneration;
			}

			Druid entityId = entity.GetEntityStructuredTypeId ();

			bool createRow = mapping.RowKey.IsEmpty;

			if (createRow)
			{
				mapping.RowKey = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
				this.dataLoadedEntities.Add (entity);
			}
			else
			{
				this.LoadDataRows (entity);
			}

			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entityId))
			{
				StructuredType entityType = this.EntityContext.GetStructuredType (currentId) as StructuredType;
				Druid          baseTypeId = entityType.BaseTypeId;

				System.Diagnostics.Debug.Assert (entityType != null);
				System.Diagnostics.Debug.Assert (entityType.CaptionId == currentId);

				//	Either create and fill a new row in the database for this entity
				//	or use and update an existing row.

				System.Data.DataRow dataRow;

				if (createRow)
				{
					dataRow = this.CreateDataRow (mapping, currentId);
				}
				else
				{
					dataRow = this.LoadDataRow (entity, mapping.RowKey, currentId);
				}

				dataRow.BeginEdit ();

				//	If this is the root entity in the entity hierarchy (it has no base
				//	type), then we will have to save the instance type identifying the
				//	entity.

				if (baseTypeId.IsEmpty)
				{
					dataRow[Tags.ColumnInstanceType] = entityId.ToLong ();
				}

				this.SerializeEntityLocal (entity, dataRow, currentId);

				dataRow.EndEdit ();
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
			foreach (StructuredTypeField fieldDef in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (fieldDef.Relation)
				{
					case FieldRelation.None:
						this.WriteFieldValueInDataRow (entity, fieldDef, dataRow);
						break;

					case FieldRelation.Reference:
						this.WriteFieldReference (entity, entityId, fieldDef);
						break;

					case FieldRelation.Collection:
						this.WriteFieldCollection (entity, entityId, fieldDef);
						break;
				}
			}
		}


		internal AbstractEntity InternalResolveEntity(EntityDataContainer entityData, bool loadFromDatabase)
		{
			if (this.isDisposed)
			{
				throw new System.InvalidOperationException ("DataContext was disposed");
			}

			var rootEntityId = this.EntityContext.GetRootEntityId (entityData.LoadedEntityId);
			var entity = this.entityDataCache.FindEntity (entityData.Key, entityData.RealEntityId, rootEntityId);

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


		private AbstractEntity InternalResolveEntityBasedOnDataLoadedFromDatabase(EntityDataContainer entityData)
		{
			var entity = this.EntityContext.CreateEmptyEntity (entityData.RealEntityId);

			this.entityDataCache.DefineRowKey (this.GetEntityDataMapping (entity), entityData.Key);

			using (entity.DefineOriginalValues ())
			{
				Druid[] entityIds = this.EntityContext.GetInheritedEntityIds (entityData.RealEntityId).ToArray ();

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
					return new Helpers.EntityKeyProxy (this, rowKey, entityId);

				default:
					throw new System.NotImplementedException (string.Format ("Resolution mode {0} not implemented", mode));
			}
		}


		internal AbstractEntity DeserializeEntity(DbKey rowKey, Druid entityId)
		{
			Druid baseEntityId = this.EntityContext.GetRootEntityId (entityId);

			System.Data.DataRow dataRow = this.LoadDataRow(null, rowKey, baseEntityId);
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
				System.Data.DataRow dataRow = this.LoadDataRow(null, mapping.RowKey, currentId);

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

						object target1 = new Helpers.EntityFieldProxy (this, entity, field);
						entity.InternalSetValue (field.Id, target1);

						break;

					case FieldRelation.Collection:

						object target2 = new Helpers.EntityCollectionFieldProxy (this, entity, field);
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

						object value = new Helpers.ValueFieldProxy (this, entity, entityId, rowKey, field);
						entity.InternalSetValue (field.Id, value);

						break;

					case FieldRelation.Reference:

						object target1 = new Helpers.EntityFieldProxy (this, entity, field);
						entity.InternalSetValue (field.Id, target1);

						break;

					case FieldRelation.Collection:

						object target2 = new Helpers.EntityCollectionFieldProxy (this, entity, field);
						entity.InternalSetValue (field.Id, target2);

						break;
				}
			}
		}


		private void DeserializeEntityLocalWithReference(AbstractEntity entity, EntityDataContainer entityData, Druid entityId)
		{
			foreach (StructuredTypeField field in this.EntityContext.GetEntityLocalFieldDefinitions (entityId))
			{
				switch (field.Relation)
				{
					case FieldRelation.None:

						if (entityData.ValueData.Contains (field))
						{
							object value = this.GetFieldValue (entity, field, entityData.ValueData[field]);
							entity.InternalSetValue (field.Id, value);
						}

						break;

					case FieldRelation.Reference:

						if (entityData.ReferenceData.Contains (field))
						{
							object target1 = this.InternalResolveEntity (entityData.ReferenceData[field], field.TypeId, EntityResolutionMode.DelayLoad);
							entity.InternalSetValue (field.Id, target1);
						}

						break;

					case FieldRelation.Collection:

						IList collection = entity.InternalGetFieldCollection (field.Id);

						foreach (DbKey key in entityData.CollectionData[field])
						{
							object target2 = this.InternalResolveEntity (key, field.TypeId, EntityResolutionMode.DelayLoad);
							collection.Add (target2);
						}

						break;
				}
			}
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

			string columnName = this.SchemaEngine.GetDataColumnName (fieldDef.Id);

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

				DbTable   tableDef  = this.RichCommand.Tables[tableName];
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

				DbTable   tableDef  = this.RichCommand.Tables[tableName];
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

			System.Data.DataRow[] relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToArray ();

			if (targetEntity != null && !this.entitiesToDelete.Contains (targetEntity))
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

			this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
		}

		private void WriteFieldCollection(AbstractEntity sourceEntity, Druid entityId, StructuredTypeField fieldDef)
		{
			IList collection = sourceEntity.InternalGetFieldCollection (fieldDef.Id);

			System.Diagnostics.Debug.Assert (collection != null);

			EntityDataMapping sourceMapping = this.GetEntityDataMapping (sourceEntity);

			string relationTableName = this.GetRelationTableName (entityId, fieldDef);

			List<System.Data.DataRow> relationRows = DbRichCommand.FilterExistingRows (this.RichCommand.FindRelationRows (relationTableName, sourceMapping.RowKey.Id)).ToList ();
			List<System.Data.DataRow> resultingRows = new List<System.Data.DataRow> ();

			for (int i = 0; i < collection.Count; i++)
			{
				AbstractEntity targetEntity  = collection[i] as AbstractEntity;

				if (!this.entitiesToDelete.Contains (targetEntity))
				{
					EntityDataMapping targetMapping = this.GetEntityDataMapping (targetEntity);

					System.Diagnostics.Debug.Assert (targetMapping != null);

					if (targetMapping.RowKey.IsEmpty)
					{
						this.SerializeEntity (targetEntity);
					}

					long targetRowId = targetMapping.RowKey.Id.Value;

					System.Diagnostics.Debug.Assert (targetEntity != null);
					System.Diagnostics.Debug.Assert (targetMapping != null);

					System.Data.DataRow row = relationRows.FirstOrDefault (r => targetRowId == (long) r[Tags.ColumnRefTargetId]);

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

			this.RelationRowIsLoaded (sourceEntity, fieldDef.CaptionId);
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
			System.Data.DataRow relationRow = this.RichCommand.CreateRow (relationTableName);
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
					this.RichCommand.DeleteExistingRow (relationRow);
					break;

				default:
					throw new System.InvalidOperationException ();
			}
		}

		private string GetRelationTableName(Druid entityId, StructuredTypeField fieldDef)
		{
			string sourceTableName = this.SchemaEngine.GetDataTableName (entityId);
			string sourceColumnName = this.SchemaEngine.GetDataColumnName (fieldDef.Id);

			return DbTable.GetRelationTableName (sourceTableName, sourceColumnName);
		}

		public object GetFieldValue(AbstractEntity entity, Druid entityId, DbKey rowKey, StructuredTypeField fieldDef)
		{
			System.Data.DataRow dataRow = this.LoadDataRow(entity, rowKey, entityId);

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


		private void ReadFieldValueFromDataRow(AbstractEntity entity, StructuredTypeField fieldDef, System.Data.DataRow dataRow)
		{
			object value = this.GetFieldValue (entity, fieldDef, dataRow);

			if (value != System.DBNull.Value)
			{
				entity.InternalSetValue (fieldDef.Id, value);
			}
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
					this.LoadRelationRows(entity, entityId, fieldDef.CaptionId, tableName, sourceMapping.RowKey);
				}
			}
		}


		private void LoadDataRows(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.FindEntityDataMapping (entity);

			foreach (Druid currentId in this.EntityContext.GetInheritedEntityIds (entity.GetEntityStructuredTypeId ()))
			{
				this.LoadDataRow(entity, mapping.RowKey, currentId);

				StructuredTypeField[] localFields = this.EntityContext.GetEntityLocalFieldDefinitions (currentId).ToArray ();

				foreach (StructuredTypeField field in localFields.Where (f => f.Relation == FieldRelation.Reference || f.Relation == FieldRelation.Collection))
				{
					this.LoadRelationRows(entity, currentId, field.CaptionId, this.GetRelationTableName(currentId, field), mapping.RowKey);
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
						
						condition.Condition = DbConditionCombiner.Combine (part1, part2);
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

						condition.Condition = DbConditionCombiner.Combine (part1, part2);
					}

					this.RichCommand.ImportTable (transaction, relationTableDef, condition);
					transaction.Commit ();
				}

				this.RelationRowIsLoaded (entity, fieldId);
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

			string tableName = this.SchemaEngine.GetDataTableName (entityId);
			System.Data.DataRow row = this.RichCommand.CreateRow (tableName);

			TemporaryRowCollection temporaryRows;
			temporaryRows = this.GetTemporaryRows (mapping.RootEntityId);
			temporaryRows.AssociateRow (this.RichCommand, mapping, row);

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
			string baseTableName = this.SchemaEngine.GetDataTableName (baseEntityId);

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
				DbTable tableDef = this.SchemaEngine.FindTableDefinition (entityId);

				if (tableDef == null)
				{
					StructuredType type = this.EntityContext.GetStructuredType (entityId) as StructuredType;

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
				
				StructuredType entityType = this.EntityContext.GetStructuredType (entityId) as StructuredType;
				Druid          baseTypeId = entityType.BaseTypeId;

				if (baseTypeId.IsValid)
				{
					this.LoadEntitySchema (baseTypeId);
				}
			}
		}

		internal void LoadTableSchema(DbTable tableDefinition)
		{
			if (this.RichCommand.Tables.Contains (tableDefinition.Name))
			{
				//	Nothing to do, we already know this table.
			}
			else
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					this.RichCommand.ImportTable (transaction, tableDefinition, null);
					this.LoadTableRelationSchemas (transaction, tableDefinition);

					transaction.Commit ();
				}
			}
		}

		private void LoadTableRelationSchemas(Druid entityId)
		{
			DbTable tableDef = this.SchemaEngine.FindTableDefinition (entityId);

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
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
					DbTable relationTableDef = this.DbInfrastructure.ResolveDbTable (transaction, relationTableName);
					this.RichCommand.ImportTable (transaction, relationTableDef, null);
				}
			}
		}


		private void RelationRowIsLoaded(AbstractEntity entity, Druid fieldId)
		{
			if (!this.relationLoadedEntity.ContainsKey (fieldId))
			{
				this.relationLoadedEntity[fieldId] = new HashSet<AbstractEntity> ();
			}

			this.relationLoadedEntity[fieldId].Add (entity);
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

			return mapping != null && !mapping.RowKey.IsEmpty;
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
				this.isDisposed = true;
				this.EntityContext.EntityAttached -= this.HandleEntityCreated;
				this.EntityContext.PersistenceManagers.Remove (this);
			}
		}

		private void HandleEntityCreated(object sender, EntityEventArgs e)
		{
			AbstractEntity entity = e.Entity;

			System.Diagnostics.Debug.Assert (this.EntityContext == entity.GetEntityContext ());

			if (this.EnableEntityNullReferenceVirtualizer)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			}

			Druid entityId      = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId  = this.EntityContext.GetRootEntityId (entityId);
			var   entityMapping = new EntityDataMapping (entity, entityId, rootEntityId);
			
			long entitySerialId = entity.GetEntitySerialId ();

			this.entityDataCache.Add (entitySerialId, entityMapping);

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

		private static int nextUniqueId;

		private readonly int uniqueId;
		private readonly EntityContext entityContext;
		private readonly EntityDataCache entityDataCache;
		private readonly Dictionary<string, bool> tableBulkLoaded;
		private readonly HashSet<AbstractEntity> dataLoadedEntities;
		private readonly Dictionary<Druid, HashSet<AbstractEntity>> relationLoadedEntity;
		private readonly Dictionary<Druid, DbTable> entityTableDefinitions;
		private readonly Dictionary<string, TemporaryRowCollection> temporaryRows;
		private readonly HashSet<AbstractEntity> entitiesToDelete;
		private readonly HashSet<AbstractEntity> deletedEntities;

		private bool isDisposed;
	}
}
