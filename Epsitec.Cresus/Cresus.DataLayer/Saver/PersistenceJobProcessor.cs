using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;



namespace Epsitec.Cresus.DataLayer.Saver
{


	// TODO Comment this class
	// Marc


	internal sealed class PersistenceJobProcessor
	{
		

		public PersistenceJobProcessor(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
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


		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.SchemaEngine;
			}
		}


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
		}


		private DataConverter DataConverter
		{
			get
			{
				return this.DataContext.DataConverter;
			}
		}


		public IEnumerable<KeyValuePair<AbstractEntity, DbKey>> ProcessJobs(DbTransaction transaction, IEnumerable<AbstractPersistenceJob> jobs)
		{
			jobs.ThrowIfNull ("jobs");
			transaction.ThrowIfNull ("transaction");

			List<AbstractPersistenceJob> jobsCopy = jobs.ToList ();
			Dictionary<AbstractEntity, DbKey> newEntityKeys = new Dictionary<AbstractEntity, DbKey> ();

			foreach (var deleteJob in jobsCopy.OfType<DeletePersistenceJob> ())
			{
				this.ProcessJob (transaction, deleteJob);
			}

			foreach (var rootValueJob in jobsCopy.OfType<ValuePersistenceJob> ().Where (j => j.IsRootTypeJob))
			{
				this.ProcessJob (transaction, newEntityKeys, rootValueJob);
			}

			foreach (var subRootValueJob in jobsCopy.OfType<ValuePersistenceJob> ().Where (j => !j.IsRootTypeJob))
			{
				this.ProcessJob (transaction, newEntityKeys, subRootValueJob);
			}

			foreach (var referenceJob in jobsCopy.OfType<ReferencePersistenceJob> ())
			{
				this.ProcessJob (transaction, newEntityKeys, referenceJob);
			}

			foreach (var collectionJob in jobsCopy.OfType<CollectionPersistenceJob> ())
			{
				this.ProcessJob (transaction, newEntityKeys, collectionJob);
			}

			return newEntityKeys;
		}


		private void ProcessJob(DbTransaction transaction, DeletePersistenceJob job)
		{
			AbstractEntity entity = job.Entity;
			DbKey dbKey = this.GetPersistentEntityDbKey (entity);
			
			this.DeleteEntityValues (transaction, entity, dbKey);
			this.DeleteEntitySourceRelations (transaction, entity, dbKey);
			this.DeleteEntityTargetRelations (transaction, entity, dbKey);
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


		private void DeleteEntitySourceRelations(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			
			var fieldIds = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
						   let rel = field.Relation
						   where rel == FieldRelation.Reference || rel == FieldRelation.Collection
						   select field.CaptionId;

			foreach (Druid fieldId in fieldIds)
			{
				Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

				this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, dbKey);
			}
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


		private void ProcessJob(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ValuePersistenceJob job)
		{
			switch (job.JobType)
			{
				case PersistenceJobType.Insert:
					this.InsertValueData (transaction, newEntityKeys, job);
					break;

				case PersistenceJobType.Update:
					this.UpdateValueData (transaction, job);
					break;

				default:
					throw new System.NotSupportedException ();
			}
		}


		private void ProcessJob(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ReferencePersistenceJob job)
		{
			switch (job.JobType)
			{
				case PersistenceJobType.Insert:
					this.InsertReferenceData (transaction, newEntityKeys, job);
					break;
				case PersistenceJobType.Update:
					this.UpdateReferenceData (transaction, newEntityKeys, job);
					break;
				default:
					throw new System.NotSupportedException ();
			}
		}


		private void ProcessJob(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, CollectionPersistenceJob job)
		{
			switch (job.JobType)
			{
				case PersistenceJobType.Insert:
					this.InserCollectionData (transaction, newEntityKeys, job);
					break;
				case PersistenceJobType.Update:
					this.UpdateCollectionData (transaction, newEntityKeys, job);
					break;
				default:
					throw new System.NotSupportedException ();
			}
		}


		private void InsertValueData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ValuePersistenceJob job)
		{
			Druid leafEntityId = job.Entity.GetEntityStructuredTypeId ();
			Druid localEntityId = job.LocalEntityId;

			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			string tableName = table.GetSqlName ();

			SqlFieldList fields = new SqlFieldList ();

			fields.AddRange (this.CreateSqlFields (table, localEntityId, job.GetFieldIdsWithValues ()));
			fields.Add (this.CreateSqlFieldForStatus (table, DbRowStatus.Live));
			fields.Add (this.CreateSqlFieldForLog (table));

			if (job.IsRootTypeJob)
			{
				fields.Add (this.CreateSqlFieldForType (table, leafEntityId));

				SqlFieldList fieldsToReturn = new SqlFieldList ()
			    {
			        new SqlField () { Alias = table.Columns[Tags.ColumnId].GetSqlName() },
			    };

				transaction.SqlBuilder.InsertData (tableName, fields, fieldsToReturn);
				object data = this.DbInfrastructure.ExecuteScalar (transaction);

				newEntityKeys[job.Entity] = new DbKey (new DbId ((long) data));
			}
			else
			{
				DbKey dbKey = this.GetNonPersistentEntityDbKey (job.Entity, newEntityKeys);

				fields.Add (this.CreateSqlFieldForKey (table, dbKey));

				transaction.SqlBuilder.InsertData (tableName, fields);
				this.DbInfrastructure.ExecuteNonQuery (transaction);
			}
		}


		private void UpdateValueData(DbTransaction transaction, ValuePersistenceJob job)
		{
			var fieldIdsWithValues = job.GetFieldIdsWithValues ().ToList ();

			if (fieldIdsWithValues.Any ())
			{
				Druid localEntityId = job.LocalEntityId;
				DbKey dbKey = this.GetPersistentEntityDbKey (job.Entity);

				DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
				string tableName = table.GetSqlName ();

				SqlFieldList fields = new SqlFieldList ();
				fields.AddRange (this.CreateSqlFields (table, localEntityId, fieldIdsWithValues));

				SqlFieldList conditions = new SqlFieldList ();
				conditions.Add (this.CreateConditionForRowId (table, dbKey));
								
				transaction.SqlBuilder.UpdateData (tableName, fields, conditions);
				this.DbInfrastructure.ExecuteNonQuery (transaction);
			}
		}


		private void InsertReferenceData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ReferencePersistenceJob job)
		{
			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);
			DbKey targetKey = this.GetEntityDbKey (job.Target, newEntityKeys);

			this.InsertEntityRelation (transaction, job.LocalEntityId, job.FieldId, sourceKey, targetKey);
		}


		private void UpdateReferenceData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ReferencePersistenceJob job)
		{
			// TODO This function might be optimized in the following way. I'm not too sure if it
			// would be better than the current implementation.
			// - If target is null => remove the row in the database
			// - If target is not null and a row exists in the database => update the row in the database
			// - If target is not null and a row does not exist in the database => create the row in the database
			// Marc

			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);

			Druid localEntityId = job.LocalEntityId;
			Druid fieldId = job.FieldId;

			this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, sourceKey);

			if (job.Target != null)
			{
				DbKey targetKey = this.GetEntityDbKey (job.Target, newEntityKeys);

				this.InsertEntityRelation (transaction, localEntityId, fieldId, sourceKey, targetKey);
			}
		}


		private void InserCollectionData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, CollectionPersistenceJob job)
		{
			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);
			var targetKeys = job.Targets.Select (t => this.GetEntityDbKey (t, newEntityKeys));

			this.InsertEntityRelation (transaction, job.LocalEntityId, job.FieldId, sourceKey, targetKeys);
		}


		private void UpdateCollectionData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, CollectionPersistenceJob job)
		{
			// TODO This function might be optimized by having a better policy to delete/update/Insert
			// the relation rows. It could take advantage to what already exists in the database, which
			// would have the following two advantages:
			// - It might require less queries
			// - If another user has modified the data and our version of the relations is not up to
			//   date, it might do less overwrite.

			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);
			var targetKeys = job.Targets.Select (t => this.GetEntityDbKey (t, newEntityKeys));

			Druid localEntityId = job.LocalEntityId;
			Druid fieldId = job.FieldId;

			this.DeleteEntitySourceRelation (transaction, localEntityId, fieldId, sourceKey);
			this.InsertEntityRelation (transaction, localEntityId, fieldId, sourceKey, targetKeys);
		}


		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, DbKey targetKey)
		{
			List<DbKey> targetKeys = new List<DbKey> () { targetKey };

			this.InsertEntityRelation (transaction, localEntityId, fieldId, sourceKey, targetKeys);
		}


		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, IEnumerable<DbKey> targetKeys)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);

			string tableName = table.GetSqlName ();

			List<DbKey> targetKeysList = targetKeys.ToList ();

			for (int rank = 0; rank < targetKeysList.Count; rank++)
			{
				SqlFieldList fields = new SqlFieldList ();

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
			DbRawType rawType = columnType.RawType;
			DbSimpleType simpleType = columnType.SimpleType;
			DbNumDef numDef = columnType.NumDef;

			object convertedValue = this.DataConverter.ToDatabaseValue (rawType, simpleType, numDef, value);
			DbRawType convertedRawType = this.DataConverter.ToDatabaseType (columnType.RawType);

			SqlField constantField = SqlField.CreateConstant (convertedValue, convertedRawType);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);

			return SqlField.CreateFunction (condition);
		}


		private IEnumerable<SqlField> CreateSqlFields(DbTable table, Druid localEntityId, IEnumerable<KeyValuePair<Druid, object>> fieldIdsWithValues)
		{
			foreach (var fieldIdWithValue in fieldIdsWithValues)
			{
				Druid fieldId = fieldIdWithValue.Key;
				object value = fieldIdWithValue.Value;

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

			DbTypeDef columnType = column.Type;
			DbRawType rawType = columnType.RawType;
			DbSimpleType simpleType = columnType.SimpleType;
			DbNumDef numDef = columnType.NumDef;

			object convertedValue = this.DataConverter.ToDatabaseValue (rawType, simpleType, numDef, value);
			DbRawType convertedRawType = this.DataConverter.ToDatabaseType (type.RawType);

			SqlField SqlField = SqlField.CreateConstant (convertedValue, convertedRawType);
			SqlField.Alias = column.GetSqlName ();

			return SqlField;
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


	}


}
