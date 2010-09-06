using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

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
			this.JobTableComputer = new PersistenceJobTableComputer (dataContext);
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


		private PersistenceJobTableComputer JobTableComputer
		{
			get;
			set;
		}



		public IEnumerable<AbstractSynchronizationJob> SaveChanges()
		{
			var entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			var entitiesToSave = this.DataContext.GetEntitiesModified ().ToList ();

			var persistenceJobs = this.GetPersistenceJobs (entitiesToDelete, entitiesToSave).ToList ();
			var affectedTables = this.GetAffectedTables (persistenceJobs).ToList ();
			
			var newEntityKeys = this.ProcessPersistenceJobs (persistenceJobs, affectedTables);

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


		private IEnumerable<DbTable> GetAffectedTables(IEnumerable<AbstractPersistenceJob> jobs)
		{
			return jobs.SelectMany (j => j.GetAffectedTables (this.JobTableComputer));
		}


		private IEnumerable<KeyValuePair<AbstractEntity, DbKey>> ProcessPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs, IEnumerable<DbTable> affectedTables)
		{
			IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys = new List<KeyValuePair<AbstractEntity, DbKey>> ();

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, affectedTables))
			{
				newEntityKeys = this.JobProcessor.ProcessJobs (transaction, jobs);

				transaction.Commit ();
			}

			return newEntityKeys;
		}


		private void CleanDeletedEntities(IEnumerable<AbstractEntity> entitiesToDelete)
		{
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.DataContext.RemoveAllReferences (entity, EntityChangedEventSource.Internal);
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


		internal bool CheckIfEntityCanBeSaved(AbstractEntity entity)
		{
			bool canBeSaved = entity != null
				&& !this.DataContext.IsDeleted (entity)
				&& !this.DataContext.IsRegisteredAsEmptyEntity (entity)
				&& !EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (entity)
				&& !EntityNullReferenceVirtualizer.IsNullEntity (entity);

			return canBeSaved;
		}


		internal bool CheckIfFieldMustBeResaved(AbstractEntity entity, Druid fieldId)
		{
			var fieldsToResave = this.DataContext.GetFieldsToResave ();

			return fieldsToResave.ContainsKey (entity) && fieldsToResave[entity].Contains (fieldId);
		}


	}


}
