using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	/// <summary>
	/// The <c>PersistenceJobProcessor</c> class is used to execute the <see cref="AbstractPersistenceJob"/>
	/// in order to persist them to the database.
	/// </summary>
	internal sealed class PersistenceJobProcessor
	{
		

		/// <summary>
		/// Creates a new <c>PersistenceJobProcessor</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> used by this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public PersistenceJobProcessor(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> used by this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="EntityContext"/> used by this instance.
		/// </summary>
		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}


		/// <summary>
		/// The <see cref="SchemaEngine"/> used by this instance.
		/// </summary>
		private SchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.SchemaEngine;
			}
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> used by this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DataInfrastructure.DbInfrastructure;
			}
		}


		/// <summary>
		/// The <see cref="DataConverter"/> used by this instance.
		/// </summary>
		private DataConverter DataConverter
		{
			get
			{
				return this.DataContext.DataConverter;
			}
		}


		/// <summary>
		/// Executes the given sequence of <see cref="AbstractPersistenceJob"/> in order to persist
		/// them to the database. In addition, this method returns a mapping from <see cref="AbstractEntity"/>
		/// to <see cref="DbKey"/> that contain the new <see cref="DbKey"/> that has been assigned to
		/// each <see cref="AbstractEntity"/> which has been inserted to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> that must be used for the operation.</param>
		/// <param name="dbLogEntry">The <see cref="DbLogEntry"/> that must be used to log the operation.</param>
		/// <param name="jobs">The sequence of <see cref="AbstractPersistenceJob"/> to execute.</param>
		/// <returns>The mapping from the <see cref="AbstractEntity"/> that have been inserted in the database to their newly assigned <see cref="DbKey"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="transaction"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="jobs"/> is <c>null</c>.</exception>
		public IEnumerable<KeyValuePair<AbstractEntity, DbKey>> ProcessJobs(DbTransaction transaction, DbLogEntry dbLogEntry, IEnumerable<AbstractPersistenceJob> jobs)
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
				this.ProcessJob (transaction, newEntityKeys, dbLogEntry, rootValueJob);
			}

			foreach (var subRootValueJob in jobsCopy.OfType<ValuePersistenceJob> ().Where (j => !j.IsRootTypeJob))
			{
				this.ProcessJob (transaction, newEntityKeys, dbLogEntry, subRootValueJob);
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


		/// <summary>
		/// Executes the given <see cref="DeletePersistenceJob"/> to the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="job">The <see cref="DeletePersistenceJob"/> to execute.</param>
		private void ProcessJob(DbTransaction transaction, DeletePersistenceJob job)
		{
			AbstractEntity entity = job.Entity;
			DbKey dbKey = this.GetPersistentEntityDbKey (entity);
			
			this.DeleteEntityValues (transaction, entity, dbKey);
			this.DeleteEntitySourceRelations (transaction, entity, dbKey);
			this.DeleteEntityTargetRelations (transaction, entity, dbKey);
		}


		/// <summary>
		/// Deletes all the value rows in the database for the given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose values to delete.</param>
		/// <param name="dbKey">The <see cref="DbKey"/> of the given <see cref="AbstractEntity"/>.</param>
		private void DeleteEntityValues(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				this.DeleteEntityValues (transaction, localEntityId, dbKey);
			}
		}


		/// <summary>
		/// Deletes the value row in the database for the given <see cref="AbstractEntity"/> and the
		/// given type.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the type of the value row to remove.</param>
		/// <param name="dbKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/>.</param>
		private void DeleteEntityValues(DbTransaction transaction, Druid localEntityId, DbKey dbKey)
		{
			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForRowId (table, dbKey));

			string tableName = table.GetSqlName ();

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		/// <summary>
		/// Removes all the relations from a given <see cref="AbstractEntity"/> to other
		/// <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose outward relations to remove.</param>
		/// <param name="dbKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/>.</param>
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


		/// <summary>
		/// Removes all the relations from any <see cref="AbstractEntity"/> to the given
		/// <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose inward relations to remove.</param>
		/// <param name="dbKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/>.</param>
		private void DeleteEntityTargetRelations(DbTransaction transaction, AbstractEntity entity, DbKey dbKey)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			foreach (Druid localEntityId in this.EntityContext.GetInheritedEntityIds (leafEntityId))
			{
				var sourceReferences = this.DataContext.DataInfrastructure.SchemaEngine.GetSourceReferences (localEntityId);

				foreach (EntityFieldPath fieldPath in sourceReferences)
				{
					this.DeleteEntityTargetRelation (transaction, dbKey, fieldPath);
				}
			}
		}


		/// <summary>
		/// Removes all the relations from a given field of other <see cref="AbstractEntity"/> to the
		/// given <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="dbKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> whose inward relation to remove.</param>
		/// <param name="fieldPath">The <see cref="EntityFieldPath"/> defining the field of the relation to remove.</param>
		private void DeleteEntityTargetRelation(DbTransaction transaction, DbKey dbKey, EntityFieldPath fieldPath)
		{
			Druid localEntityId = fieldPath.EntityId;
			Druid fieldId = new Druid (fieldPath.Fields.First ());

			this.DeleteEntityTargetRelation (transaction, localEntityId, fieldId, dbKey);
		}


		/// <summary>
		/// Executes the given <see cref="ValuePersistenceJob"/> in order to persist it to the
		/// database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="dbLogEntry">The <see cref="DbLogEntry"/> that must be used to log the operation.</param>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> to execute.</param>
		private void ProcessJob(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, DbLogEntry dbLogEntry, ValuePersistenceJob job)
		{
			switch (job.JobType)
			{
				case PersistenceJobType.Insert:
					this.InsertValueData (transaction, newEntityKeys, dbLogEntry, job);
					break;

				case PersistenceJobType.Update:
					this.UpdateValueData (transaction, dbLogEntry, job);
					break;

				default:
					throw new System.NotSupportedException ();
			}
		}


		/// <summary>
		/// Executes the given <see cref="ReferencePersistenceJob"/> in order to persist it to the
		/// database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> to execute.</param>
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


		/// <summary>
		/// Executes the given <see cref="CollectionPersistenceJob"/> in order to persist it to the
		/// database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> to execute.</param>
		private void ProcessJob(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, CollectionPersistenceJob job)
		{
			switch (job.JobType)
			{
				case PersistenceJobType.Insert:
					this.InsertCollectionData (transaction, newEntityKeys, job);
					break;
				case PersistenceJobType.Update:
					this.UpdateCollectionData (transaction, newEntityKeys, job);
					break;
				default:
					throw new System.NotSupportedException ();
			}
		}


		/// <summary>
		/// Inserts the value data the given <see cref="ValuePersistenceJob"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="dbLogEntry">The <see cref="DbLogEntry"/> that must be used to log the operation.</param>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> to execute.</param>
		private void InsertValueData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, DbLogEntry dbLogEntry, ValuePersistenceJob job)
		{
			Druid leafEntityId = job.Entity.GetEntityStructuredTypeId ();
			Druid localEntityId = job.LocalEntityId;

			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			string tableName = table.GetSqlName ();

			SqlFieldList fields = new SqlFieldList ();

			fields.AddRange (this.CreateSqlFields (table, localEntityId, job.GetFieldIdsWithValues ()));
			
			if (job.IsRootTypeJob)
			{
				fields.Add (this.CreateSqlFieldForLog (table, dbLogEntry));
			}

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


		/// <summary>
		/// Updates the value data for the given <see cref="ValuePersistenceJob"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="dbLogEntry">The <see cref="DbLogEntry"/> that must be used to log the operation.</param>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> to execute.</param>
		private void UpdateValueData(DbTransaction transaction, DbLogEntry dbLogEntry, ValuePersistenceJob job)
		{
			var fieldIdsWithValues = job.GetFieldIdsWithValues ().ToList ();

			Druid localEntityId = job.LocalEntityId;
			DbKey dbKey = this.GetPersistentEntityDbKey (job.Entity);

			DbTable table = this.SchemaEngine.GetEntityTableDefinition (localEntityId);
			string tableName = table.GetSqlName ();

			SqlFieldList fields = new SqlFieldList ();
			fields.AddRange (this.CreateSqlFields (table, localEntityId, fieldIdsWithValues));

			if (job.IsRootTypeJob)
			{
				fields.Add (this.CreateSqlFieldForLog (table, dbLogEntry));
			}

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForRowId (table, dbKey));

			transaction.SqlBuilder.UpdateData (tableName, fields, conditions);
			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		/// <summary>
		/// Inserts the reference data for the given <see cref="ReferencePersistenceJob"/> in the database .
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> to execute.</param>
		private void InsertReferenceData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, ReferencePersistenceJob job)
		{
			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);
			DbKey targetKey = this.GetEntityDbKey (job.Target, newEntityKeys);

			this.InsertEntityRelation (transaction, job.LocalEntityId, job.FieldId, sourceKey, targetKey);
		}


		/// <summary>
		/// Updates the reference data for the given <see cref="ReferencePersistenceJob"/> in the database .
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> to execute.</param>
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


		/// <summary>
		/// Inserts the collection data for the given <see cref="CollectionPersistenceJob"/> in the database .
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> to execute.</param>
		private void InsertCollectionData(DbTransaction transaction, Dictionary<AbstractEntity, DbKey> newEntityKeys, CollectionPersistenceJob job)
		{
			DbKey sourceKey = this.GetEntityDbKey (job.Entity, newEntityKeys);
			var targetKeys = job.Targets.Select (t => this.GetEntityDbKey (t, newEntityKeys));

			this.InsertEntityRelation (transaction, job.LocalEntityId, job.FieldId, sourceKey, targetKeys);
		}


		/// <summary>
		/// Updates the collection data for the given <see cref="CollectionPersistenceJob"/> in the database .
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping from the newly inserted <see cref="AbstractEntity"/> to their newly assigned <see cref="DbKey"/>.</param>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> to execute.</param>
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


		/// <summary>
		/// Inserts the relation from an <see cref="AbstractEntity"/> to another in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the local type of the <see cref="AbstractEntity"/> that contains the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field of the relation</param>
		/// <param name="sourceKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> that is the source of the relation.</param>
		/// <param name="targetKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> that is the target of the relation.</param>
		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, DbKey targetKey)
		{
			List<DbKey> targetKeys = new List<DbKey> () { targetKey };

			this.InsertEntityRelation (transaction, localEntityId, fieldId, sourceKey, targetKeys);
		}


		/// <summary>
		/// Inserts the relation from an <see cref="AbstractEntity"/> to a sequence of
		/// <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the local type of the <see cref="AbstractEntity"/> that contains the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field of the relation</param>
		/// <param name="sourceKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> that is the source of the relation.</param>
		/// <param name="targetKeys">The sequence of <see cref="DbKey"/> of the <see cref="AbstractEntity"/> that are the target of the relation.</param>
		private void InsertEntityRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey, IEnumerable<DbKey> targetKeys)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);

			string tableName = table.GetSqlName ();

			List<DbKey> targetKeysList = targetKeys.ToList ();

			for (int rank = 0; rank < targetKeysList.Count; rank++)
			{
				SqlFieldList fields = new SqlFieldList ();

				fields.Add (this.CreateSqlFieldForSourceId (table, sourceKey));
				fields.Add (this.CreateSqlFieldForTargetId (table, targetKeysList[rank]));
				fields.Add (this.CreateSqlFieldForRank (table, rank));

				transaction.SqlBuilder.InsertData (tableName, fields);

				this.DbInfrastructure.ExecuteNonQuery (transaction);
			}
		}


		/// <summary>
		/// Deletes all items of the relation defined by the given field that have a given source in
		/// the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/> owning the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <param name="sourceKey">The <see cref="DbKey"/> of the source of the relation.</param>
		private void DeleteEntitySourceRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey sourceKey)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			string tableName = table.GetSqlName ();

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForSourceId (table, sourceKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);
			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		/// <summary>
		/// Deletes all items of the relation defined by the given field that have a given target in
		/// the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> object to use.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/> owning the field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field.</param>
		/// <param name="targetKey">The <see cref="DbKey"/> of the target of the relation.</param>
		private void DeleteEntityTargetRelation(DbTransaction transaction, Druid localEntityId, Druid fieldId, DbKey targetKey)
		{
			DbTable table = this.SchemaEngine.GetRelationTableDefinition (localEntityId, fieldId);
			string tableName = table.GetSqlName ();

			SqlFieldList conditions = new SqlFieldList ();
			conditions.Add (this.CreateConditionForTargetId (table, targetKey));

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		/// <summary>
		/// Builds a <see cref="SqlField"/> that contains the condition that holds true when the
		/// id of a row is equal to the given <see cref="DbKey"/> for the given <see cref="DbTable"/>.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the condition.</param>
		/// <param name="dbKey">The value of the <see cref="DbKey"/> of the condition.</param>
		/// <returns>The <see cref="SqlField"/> that holds the condition.</returns>
		private SqlField CreateConditionForRowId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}


		/// <summary>
		/// Builds a <see cref="SqlField"/> that contains the condition that holds true when the
		/// source id  field of a row is equal to the given <see cref="DbKey"/> for the given
		/// relation <see cref="DbTable"/>.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the condition.</param>
		/// <param name="dbKey">The value of the <see cref="DbKey"/> of the condition.</param>
		/// <returns>The <see cref="SqlField"/> that holds the condition.</returns>
		private SqlField CreateConditionForSourceId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnRefSourceId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}


		/// <summary>
		/// Builds a <see cref="SqlField"/> that contains the condition that holds true when the
		/// target id  field of a row is equal to the given <see cref="DbKey"/> for the given
		/// relation <see cref="DbTable"/>.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the condition.</param>
		/// <param name="dbKey">The value of the <see cref="DbKey"/> of the condition.</param>
		/// <returns>The <see cref="SqlField"/> that holds the condition.</returns>
		private SqlField CreateConditionForTargetId(DbTable table, DbKey dbKey)
		{
			DbColumn column = table.Columns[Tags.ColumnRefTargetId];
			long value = dbKey.Id.Value;

			return this.CreateConditionForField (column, value);
		}


		/// <summary>
		/// Builds a <see cref="SqlField"/> that contains the condition that holds true when the
		/// given <see cref="DbColumn"/> has the given value.
		/// </summary>
		/// <param name="column">The <see cref="DbColumn"/> that is targeted by the condition.</param>
		/// <param name="value">The value that the <see cref="DbColumn"/> must have in order to satisfy the condition.</param>
		/// <returns>The <see cref="SqlField"/> that holds the condition.</returns>
		private SqlField CreateConditionForField(DbColumn column, object value)
		{
			string columnName = column.GetSqlName ();
			SqlField columnField = SqlField.CreateName (columnName);

			DbTypeDef columnType = column.Type;
			DbRawType rawType = columnType.RawType;
			DbSimpleType simpleType = columnType.SimpleType;
			DbNumDef numDef = columnType.NumDef;

			object convertedValue = this.DataConverter.FromCresusToDatabaseValue (rawType, simpleType, numDef, value);
			DbRawType convertedRawType = this.DataConverter.FromDotNetToDatabaseType (columnType.RawType);

			SqlField constantField = SqlField.CreateConstant (convertedValue, convertedRawType);

			SqlFunction condition = new SqlFunction (
				SqlFunctionCode.CompareEqual,
				columnField,
				constantField
			);

			return SqlField.CreateFunction (condition);
		}


		/// <summary>
		/// Builds the sequence of <see cref="SqlField"/> that are used to set the values of the
		/// <see cref="AbstractEntity"/> in an INSERT or an UPDATE SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> in which to insert the values.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the local type of the <see cref="AbstractEntity"/>.</param>
		/// <param name="fieldIdsWithValues">The mapping from the field ids to their values.</param>
		/// <returns>The sequence of the <see cref="SqlField"/> that are used within the SQl Request.</returns>
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


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of the row of a row in a SQL
		/// request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="key">The value of the <see cref="DbKey"/>.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForKey(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of the instance type of a row in
		/// a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="leafEntityId">The value of the instance type.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForType(DbTable table, Druid leafEntityId)
		{
			DbColumn column = table.Columns[Tags.ColumnInstanceType];
			object value = leafEntityId.ToLong ();

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of log of a row in a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="dbLogEntry">The <see cref="DbLogEntry"/> that must be used to log the operation.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForLog(DbTable table, DbLogEntry dbLogEntry)
		{
			DbColumn column = table.Columns[Tags.ColumnRefLog];
			long value = dbLogEntry.EntryId.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of source key of a relation row
		/// in a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="key">The value of the source key.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForSourceId(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnRefSourceId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of target key of a relation row
		/// in a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="key">The value of the target key.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForTargetId(DbTable table, DbKey key)
		{
			DbColumn column = table.Columns[Tags.ColumnRefTargetId];
			object value = key.Id.Value;

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of rank of a relation row
		/// in a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="rank">The value of the rank.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForRank(DbTable table, int rank)
		{
			DbColumn column = table.Columns[Tags.ColumnRefRank];
			object value = rank;

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of an <see cref="AbstractEntity"/>
		/// field of a row in a SQL request.
		/// </summary>
		/// <param name="table">The <see cref="DbTable"/> targeted by the SQL request.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the id of the field.</param>
		/// <param name="value">The value of the field.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForEntityValueField(DbTable table, Druid fieldId, object value)
		{
			string columnName = this.SchemaEngine.GetEntityColumnName (fieldId);
			DbColumn column = table.Columns[columnName];

			return this.CreateSqlFieldForColumn (column, value);
		}


		/// <summary>
		/// Builds the <see cref="SqlField"/> used to set the value of a <see cref="DbColumn"/> in a
		/// row in a SQL request.
		/// </summary>
		/// <param name="column">The <see cref="DbColumn"/> whose value to set.</param>
		/// <param name="value">The value that must be set.</param>
		/// <returns>The <see cref="SqlField"/> that contain the setter clause.</returns>
		private SqlField CreateSqlFieldForColumn(DbColumn column, object value)
		{
			DbTypeDef columnType = column.Type;
			DbRawType rawType = columnType.RawType;
			DbSimpleType simpleType = columnType.SimpleType;
			DbNumDef numDef = columnType.NumDef;

			object convertedValue = this.DataConverter.FromCresusToDatabaseValue (rawType, simpleType, numDef, value);
			DbRawType convertedRawType = this.DataConverter.FromDotNetToDatabaseType (columnType.RawType);

			SqlField SqlField = SqlField.CreateConstant (convertedValue, convertedRawType);
			SqlField.Alias = column.GetSqlName ();

			return SqlField;
		}


		/// <summary>
		/// Retrieves the <see cref="DbKey"/> of an <see cref="AbstractEntity"/> that is persistent
		/// within the <see cref="DataContext"/> used by this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="DbKey"/> to obtain.</param>
		/// <returns>The <see cref="DbKey"/> of the given <see cref="AbstractEntity"/>.</returns>
		private DbKey GetPersistentEntityDbKey(AbstractEntity entity)
		{
			return this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey;
		}


		/// <summary>
		/// Retrieves the <see cref="DbKey"/> of an <see cref="AbstractEntity"/> that is not persistent
		/// within the <see cref="DataContext"/> used by this instance but is defined in the given
		/// <see cref="Dictionary{AbstractEntity,DbKey}"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="DbKey"/> to obtain.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping between the newly inserted <see cref="AbstractEntity"/> and their <see cref="DbKey"/>.</param>
		/// <returns>The <see cref="DbKey"/> of the given <see cref="AbstractEntity"/>.</returns>
		private DbKey GetNonPersistentEntityDbKey(AbstractEntity entity, Dictionary<AbstractEntity, DbKey> newEntityKeys)
		{
			return newEntityKeys[entity];
		}


		/// <summary>
		/// Retrieves the <see cref="DbKey"/> of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="DbKey"/> to obtain.</param>
		/// <param name="newEntityKeys">The <see cref="Dictionary{AbstractEntity, DbKey}"/> containing the mapping between the newly inserted <see cref="AbstractEntity"/> and their <see cref="DbKey"/>.</param>
		/// <returns>The <see cref="DbKey"/> of the given <see cref="AbstractEntity"/>.</returns>
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
