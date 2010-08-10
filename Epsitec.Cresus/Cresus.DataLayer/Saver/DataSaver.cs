using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	// TODO Comment this class
	// Marc


	internal sealed class DataSaver
	{


		public DataSaver(DataContext dataContext)
		{
			this.DataContext = dataContext;
			this.SaverQueryGenerator = new SaverQueryGenerator (dataContext);
			this.JobConverter = new PersistenceJobConverter (dataContext);
			this.JobGenerator = new PersistenceJobGenerator (dataContext);
			this.JobProcessor = new PersistenceJobProcessor (dataContext);
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private SaverQueryGenerator SaverQueryGenerator
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


		private PersistenceJobConverter JobConverter
		{
			get;
			set;
		}


		private PersistenceJobGenerator JobGenerator
		{
			get;
			set;
		}


		private PersistenceJobProcessor JobProcessor
		{
			get;
			set;
		}


		public IEnumerable<AbstractSynchronizationJob> SaveChanges()
		{
			return this.SaveChanges1 ();
		}


		public IEnumerable<AbstractSynchronizationJob> SaveChanges2()
		{
			var entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			var entitiesToSave = this.DataContext.GetEntitiesModified ().ToList ();

			var persistenceJobs = this.GetPersistenceJobs (entitiesToDelete, entitiesToSave).ToList ();
			var newEntityKeys = this.ProcessPersistenceJobs (persistenceJobs);
			var synchronizationJobs = this.ConvertPersistenceJobs (persistenceJobs);

			this.PostProcessPersistenceJobs (entitiesToDelete, entitiesToSave, newEntityKeys);

			return synchronizationJobs;
		}


		private IEnumerable<AbstractPersistenceJob> GetPersistenceJobs(IEnumerable<AbstractEntity> entitiesToDelete, IEnumerable<AbstractEntity> entitiesToSave)
		{
			var deletionJobs = this.DeleteEntities (entitiesToDelete);
			var saveJobs = this.SaveEntities (entitiesToSave);

			return deletionJobs.Concat (saveJobs);
		}


		private IEnumerable<AbstractPersistenceJob> DeleteEntities(IEnumerable<AbstractEntity> entitiesToDelete)
		{
			return from entity in entitiesToDelete
				   where this.DataContext.IsPersistent (entity)
				   select this.JobGenerator.CreateDeletionJob (entity);
		}

		private IEnumerable<AbstractPersistenceJob> SaveEntities(IEnumerable<AbstractEntity> entitiesToSave)
		{
			List<AbstractPersistenceJob> jobs = new List<AbstractPersistenceJob> ();

			var entities = from entity in entitiesToSave
						   where this.CheckIfEntityCanBeSaved (entity)
						   select entity;

			foreach (AbstractEntity entity in entities)
			{
				if (this.DataContext.IsPersistent (entity))
				{
					jobs.AddRange (this.JobGenerator.CreateUpdateJobs (entity));
				}
				else
				{
					jobs.AddRange (this.JobGenerator.CreateInsertionJobs (entity));
				}
			}

			return jobs;
		}


		private IEnumerable<KeyValuePair<AbstractEntity, DbKey>> ProcessPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs)
		{
			bool done = false;
			IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys;

			do
			{
				try
				{
					using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction ())
					{
						newEntityKeys = this.JobProcessor.ProcessJobs (transaction, jobs);

						transaction.Commit ();
					}

					done = true;
				}
				catch (System.Exception e)
				{
					// catch the appropriate exception here
					// throw after too much tries.
					throw;

					System.Threading.Thread.Sleep (100);
				}
			}
			while (!done);

			return newEntityKeys;
		}


		private void PostProcessPersistenceJobs(IEnumerable<AbstractEntity> entitiesToDelete, IEnumerable<AbstractEntity> entitiesToSave, IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys)
		{
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.DeleteEntityTargetRelationsInMemory (entity, EntityChangedEventSource.Internal);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();

			foreach (AbstractEntity entity in entitiesToSave)
			{
				entity.SetModifiedValuesAsOriginalValues ();
			}

			foreach (var newEntityKey in newEntityKeys)
			{
				AbstractEntity entity = newEntityKey.Key;
				DbKey key = newEntityKey.Value;

				this.DataContext.DefineRowKey (entity, key);
			}

			this.UpdateDataGeneration ();
		}



		private IEnumerable<AbstractSynchronizationJob> ConvertPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs)
		{
			return jobs.SelectMany (j => this.JobConverter.Convert (j));
		}


		public IEnumerable<AbstractSynchronizationJob> SaveChanges1()
		{
			bool containsChanges;

			List<AbstractSynchronizationJob> synchronizationJobs = new List<AbstractSynchronizationJob> ();

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				bool deletedEntities = this.DeleteEntities (transaction, synchronizationJobs);
				bool savedEntities = this.SaveEntities (transaction, synchronizationJobs);

				containsChanges = deletedEntities || savedEntities;

				transaction.Commit ();
			}

			if (containsChanges)
			{
				this.UpdateDataGeneration ();
			}

			return synchronizationJobs;
		}


		private bool DeleteEntities(DbTransaction transaction, List<AbstractSynchronizationJob> synchronizationJobs)
		{
			List<AbstractEntity> entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();

			foreach (AbstractEntity entity in entitiesToDelete)
			{
				if (this.DataContext.IsPersistent (entity))
				{
					int dataContextId = this.DataContext.UniqueId;
					EntityKey entityKey = this.DataContext.GetEntityKey (entity).Value;

					synchronizationJobs.Add (new DeleteSynchronizationJob (dataContextId, entityKey));
				}

				this.RemoveEntity (transaction, entity);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();

			return entitiesToDelete.Any ();
		}


		private void RemoveEntity(DbTransaction transaction, AbstractEntity entity)
		{
			this.DeleteEntityTargetRelationsInMemory (entity, EntityChangedEventSource.Internal);

			if (this.DataContext.IsPersistent (entity))
			{
				this.SaverQueryGenerator.DeleteEntity (transaction, entity);
			}
		}


		public void DeleteEntityTargetRelationsInMemory(AbstractEntity target, EntityChangedEventSource eventSource)
		{
			// This method will probably be too slow for a high number of managed entities, therefore
			// it would be nice to optimize it, either by keeping somewhere a list of entities targeting
			// other entities, or by looping only on a subset of entities, i.e only on the location
			// entities if we look for an entity which can be targeted only by a location.
			// Marc

			Druid leafTargetEntityId = target.GetEntityStructuredTypeId ();

			var fieldPaths = this.EntityContext.GetInheritedEntityIds (leafTargetEntityId)
				.SelectMany (id => this.DbInfrastructure.GetSourceReferences (id))
				.ToDictionary (path => path.EntityId, path => Druid.Parse (path.Fields[0]));

			foreach (AbstractEntity source in this.DataContext.GetEntities ())
			{
				Druid leafSourceEntityId = source.GetEntityStructuredTypeId ();
				var leafInheritedIds = this.EntityContext.GetInheritedEntityIds (leafSourceEntityId);

				foreach (Druid leafInheritedId in leafInheritedIds)
				{
					if (fieldPaths.ContainsKey (leafInheritedId))
					{
						this.DeleteEntityTargetRelationInMemory (source, fieldPaths[leafInheritedId], target, eventSource);
					}
				}
			}
		}


		private void DeleteEntityTargetRelationInMemory(AbstractEntity source, Druid fieldId, AbstractEntity target, EntityChangedEventSource eventSource)
		{
			StructuredTypeField field = this.EntityContext.GetStructuredTypeField (source, fieldId.ToResourceId ());

			bool updated = false;

			using (source.UseSilentUpdates ())
			{
				using (source.DisableEvents ())
				{
					switch (field.Relation)
					{
						case FieldRelation.Reference:

							if (source.InternalGetValue (field.Id) == target)
							{
								source.InternalSetValue (field.Id, null);

								updated = true;
							}

							break;

						case FieldRelation.Collection:

							IList collection = source.InternalGetFieldCollection (field.Id) as IList;

							while (collection.Contains (target))
							{
								collection.Remove (target);

								updated = true;
							}

							break;

						default:

							throw new System.InvalidOperationException ();
					}
				}
			}

			if (updated)
			{
				this.DataContext.NotifyEntityChanged (source, eventSource, EntityChangedEventType.Updated);
			}
		}


		private bool SaveEntities(DbTransaction transaction, List<AbstractSynchronizationJob> synchronizationJobs)
		{
			List<AbstractEntity> entitiesToSave = new List<AbstractEntity> (
				from entity in this.DataContext.GetEntitiesModified ()
				where this.CheckIfEntityCanBeSaved (entity)
				select entity
			);

			HashSet<AbstractEntity> savedEntities = new HashSet<AbstractEntity> ();
			Dictionary<AbstractEntity, DbKey> newEntityKeys = new Dictionary<AbstractEntity, DbKey> ();

			foreach (AbstractEntity entity in entitiesToSave)
			{
				this.SaveEntity (transaction, savedEntities, newEntityKeys, synchronizationJobs, entity);
			}

			foreach (var item in newEntityKeys)
			{
				AbstractEntity entity = item.Key;
				DbKey dbKey = item.Value;

				this.DataContext.DefineRowKey (entity, dbKey);
			}

			foreach (AbstractEntity entity in savedEntities)
			{
				entity.SetModifiedValuesAsOriginalValues ();
			}

			return entitiesToSave.Any ();
		}


		private void SaveEntity(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, Dictionary<AbstractEntity, DbKey> newEntityKeys, List<AbstractSynchronizationJob> synchronizationJobs, AbstractEntity entity)
		{
			if (savedEntities.Contains (entity))
			{
				return;
			}
			else
			{
				savedEntities.Add (entity);
			}
			
			if (!this.DataContext.Contains (entity))
			{
				// TODO: Should we propagate the serialization to another DataContext ?
				// Pierre
				throw new System.Exception ("entity is not owned by the DataContext associated with this DataSaver.");
			}

			bool isPersisted = this.DataContext.IsPersistent (entity);

			if (isPersisted)
			{
				synchronizationJobs.AddRange (this.SaverQueryGenerator.UpdateEntityValues (transaction, newEntityKeys, entity));
			}
			else
			{
				synchronizationJobs.AddRange (this.SaverQueryGenerator.InsertEntityValues (transaction, newEntityKeys, entity));
			}

			this.SaveTargetsIfNotPersisted (transaction, savedEntities, newEntityKeys, synchronizationJobs, entity);

			if (isPersisted)
			{
				synchronizationJobs.AddRange (this.SaverQueryGenerator.UpdateEntityRelations (transaction, newEntityKeys, entity));
			}
			else
			{
				synchronizationJobs.AddRange (this.SaverQueryGenerator.InsertEntityRelations (transaction, newEntityKeys, entity));
			}
		}


		private void SaveTargetsIfNotPersisted(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, Dictionary<AbstractEntity, DbKey> newEntityKeys, List<AbstractSynchronizationJob> synchronizationJobs, AbstractEntity source)
		{
			Druid leafEntityId = source.GetEntityStructuredTypeId ();

			var relations = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
							let rel = field.Relation
							where rel == FieldRelation.Reference || rel == FieldRelation.Collection
							select field;

			foreach (StructuredTypeField field in relations)
			{
				this.SaveTargetsIfNotPersisted (transaction, savedEntities, newEntityKeys, synchronizationJobs, source, field);
			}
		}


		private void SaveTargetsIfNotPersisted(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, Dictionary<AbstractEntity, DbKey> newEntityKeys, List<AbstractSynchronizationJob> synchronizationJobs, AbstractEntity source, StructuredTypeField field)
		{
			List<AbstractEntity> targets = new List<AbstractEntity> ();
			
			switch (field.Relation)
			{
				case FieldRelation.Reference:
				{
					targets.Add (source.GetField<AbstractEntity> (field.Id));
					break;
				}
				case FieldRelation.Collection:
				{
					targets.AddRange (source.GetFieldCollection<AbstractEntity> (field.Id));
					break;
				}
				default:
				{
					throw new System.InvalidOperationException ();
				}
			}

			foreach (AbstractEntity target in targets.Where (t => this.CheckIfTargetMustBeSaved (t)))
			{
				this.SaveEntity (transaction, savedEntities, newEntityKeys, synchronizationJobs, target);
			}
		}


		private bool CheckIfTargetMustBeSaved(AbstractEntity target)
		{
			bool mustBeSaved = target != null
				&& !this.DataContext.IsPersistent (target)
				&&  this.CheckIfEntityCanBeSaved (target);
			
			return mustBeSaved;
		}


		internal bool CheckIfEntityCanBeSaved(AbstractEntity entity)
		{
			bool canBeSaved = entity != null
				&& !this.DataContext.IsDeleted (entity)
				&& !this.DataContext.IsRegisteredAsEmptyEntity (entity)
				&& !EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (entity)
				&& !EntityNullReferenceVirtualizer.IsNullEntity (entity);

			return canBeSaved;
		}


		private void UpdateDataGeneration()
		{
			this.EntityContext.NewDataGeneration ();
		}


	}


}
