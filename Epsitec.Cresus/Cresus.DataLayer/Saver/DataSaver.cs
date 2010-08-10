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
			this.JobConverter = new PersistenceJobConverter (dataContext);
			this.JobGenerator = new PersistenceJobGenerator (dataContext);
			this.JobProcessor = new PersistenceJobProcessor (dataContext);
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
			var entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			var entitiesToSave = this.DataContext.GetEntitiesModified ().ToList ();

			var persistenceJobs = this.GetPersistenceJobs (entitiesToDelete, entitiesToSave).ToList ();
			var newEntityKeys = this.ProcessPersistenceJobs (persistenceJobs);

			this.CleanSavedEntities (entitiesToSave);
			this.AssignNewEntityKeys (newEntityKeys);
			
			var synchronizationJobs = this.ConvertPersistenceJobs (persistenceJobs);

			this.CleanDeletedEntities (entitiesToDelete);
			this.UpdateDataGeneration ();

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
			int nbTries = 0;
			System.Random dice = new System.Random ();
			IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys = new List<KeyValuePair<AbstractEntity, DbKey>> ();

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
				catch (Database.Exceptions.GenericException e)
				{
					if (nbTries <= 25)
					{
						int minWaitTime = 10;
						int maxWaitTime = minWaitTime + (10 * nbTries);
						int waitTime = dice.Next (minWaitTime, maxWaitTime);

						System.Threading.Thread.Sleep (waitTime);

						nbTries++;
					}
					else
					{
						throw new System.Exception ("Impossible to persist changes to the database.", e);
					}
				}
			}
			while (!done);

			return newEntityKeys;
		}


		private void CleanDeletedEntities(IEnumerable<AbstractEntity> entitiesToDelete)
		{
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.DeleteEntityTargetRelationsInMemory (entity, EntityChangedEventSource.Internal);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();
		}


		private void CleanSavedEntities(IEnumerable<AbstractEntity> entitiesToSave)
		{
			foreach (AbstractEntity entity in entitiesToSave)
			{
				entity.SetModifiedValuesAsOriginalValues ();
			}
		}


		private void AssignNewEntityKeys(IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys)
		{
			foreach (var newEntityKey in newEntityKeys)
			{
				AbstractEntity entity = newEntityKey.Key;
				DbKey key = newEntityKey.Value;

				this.DataContext.DefineRowKey (entity, key);
			}
		}


		private IEnumerable<AbstractSynchronizationJob> ConvertPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs)
		{
			return jobs.SelectMany (j => j.Convert (this.JobConverter)).ToList ();
		}


		private void UpdateDataGeneration()
		{
			this.EntityContext.NewDataGeneration ();
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


	}


}
