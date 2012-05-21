using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	/// <summary>
	/// The <c>DataSaver</c> class is used to save any modifications made on the
	/// <see cref="AbstractEntity"/> managed by a <see cref="DataContext"/> to the database.
	/// </summary>
	internal sealed class DataSaver
	{


		/// <summary>
		/// Instantiates a new <c>DataSaver</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> associated with the current instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public DataSaver(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
			this.JobConverter = new PersistenceJobConverter (dataContext);
			this.JobGenerator = new PersistenceJobGenerator (dataContext);
			this.JobProcessor = new PersistenceJobProcessor (dataContext);
			this.JobTableComputer = new PersistenceJobTableComputer (dataContext);
		}


		/// <summary>
		/// The <see cref="DataContext"/> associated with this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		private DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.DataContext.DataInfrastructure;
			}
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataInfrastructure.DbInfrastructure;
			}
		}


		/// <summary>
		/// The <see cref="JobConverter"/> associated with this instance.
		/// </summary>
		private PersistenceJobConverter JobConverter
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="JobGenerator"/> associated with this instance.
		/// </summary>
		private PersistenceJobGenerator JobGenerator
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="JobProcessor"/> associated with this instance.
		/// </summary>
		private PersistenceJobProcessor JobProcessor
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="PersistenceJobTableComputer"/> associated with this instance.
		/// </summary>
		private PersistenceJobTableComputer JobTableComputer
		{
			get;
			set;
		}


		/// <summary>
		/// Saves every modification made to the <see cref="DataContext"/> associated with this
		/// instance to the database.
		/// </summary>
		/// <returns>The sequence of <see cref="AbstractSynchronizationJob"/> that represents the modification made by this call to the database.</returns>
		/// <exception cref="System.InvalidOperationException">If some <see cref="AbstractEntity"/> managed by the associated <see cref="DataContext"/> references a foreign <see cref="AbstractEntity"/>.</exception>
		public IEnumerable<AbstractSynchronizationJob> SaveChanges()
		{
			var entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			var entitiesToSave = this.DataContext.GetEntitiesModified ().ToList ();

			// Here we don't remove the deleted entities from the entities to save, because they will
			// be removed afterwards in the GetPersistenceJobs(...) method.
			// Marc

			var persistenceJobs = this.GetPersistenceJobs (entitiesToDelete, entitiesToSave).ToList ();
			var affectedTables = this.GetAffectedTables (persistenceJobs).ToList ();

			var newEntityKeys = this.ProcessPersistenceJobs (persistenceJobs, affectedTables);

			this.CleanSavedEntities (entitiesToSave);
			this.AssignNewEntityKeys (newEntityKeys);

			var synchronizationJobs = this.ConvertPersistenceJobs (persistenceJobs);

			this.DataContext.CleanDeletedEntities (entitiesToDelete, EntityChangedEventSource.Internal, EntityChangedEventSource.External);
			this.DataContext.ClearFieldsToResave ();
			this.UpdateDataGeneration ();

			return synchronizationJobs;
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractPersistenceJob"/> that represents the modifications.
		/// </summary>
		/// <param name="entitiesToDelete">The <see cref="AbstractEntity"/> that must be deleted.</param>
		/// <param name="entitiesToSave">The <see cref="AbstractEntity"/> that have been created or modified.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> GetPersistenceJobs(IEnumerable<AbstractEntity> entitiesToDelete, IEnumerable<AbstractEntity> entitiesToSave)
		{
			var deletionJobs = this.DeleteEntities (entitiesToDelete);
			var saveJobs = this.SaveEntities (entitiesToSave);

			return deletionJobs.Concat (saveJobs);
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractPersistenceJob"/> that represents the modifications
		/// that must be made to delete the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entitiesToDelete">The <see cref="AbstractEntity"/> that must be deleted.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> DeleteEntities(IEnumerable<AbstractEntity> entitiesToDelete)
		{
			return from entity in entitiesToDelete
				   where this.DataContext.IsPersistent (entity)
				   select this.JobGenerator.CreateDeletionJob (entity);
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractPersistenceJob"/> that represents the modifications
		/// that must be made to save the <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entitiesToSave">The <see cref="AbstractEntity"/> that have been created or modified.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
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


		/// <summary>
		/// Gets the sequence of <see cref="DbTable"/> that will be modified when the given sequence
		/// of <see cref="AbstractPersistenceJob"/> will be persisted to the database.
		/// </summary>
		/// <param name="jobs">The sequence of <see cref="AbstractPersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The sequence of <see cref="DbTable"/> that will be affected.</returns>
		private IEnumerable<DbTable> GetAffectedTables(IEnumerable<AbstractPersistenceJob> jobs)
		{
			return jobs
				.SelectMany (j => j.GetAffectedTables (this.JobTableComputer))
				.Distinct ();
		}


		/// <summary>
		/// Processes the given sequence of <see cref="AbstractPersistenceJob"/> to execute them
		/// against the database.
		/// </summary>
		/// <param name="jobs">The <see cref="AbstractPersistenceJob"/> to execute.</param>
		/// <param name="affectedTables">The <see cref="DbTable"/> that will be modified during the execution.</param>
		/// <returns>The <see cref="EntityModificationEntry"/> used by the operation and the mapping between the <see cref="AbstractEntity"/> that have been inserted in the database and their newly assigned <see cref="DbKey"/>.</returns>
		private IEnumerable<KeyValuePair<AbstractEntity, DbKey>> ProcessPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs, IEnumerable<DbTable> affectedTables)
		{
			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, affectedTables))
			{
				IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys = new List<KeyValuePair<AbstractEntity, DbKey>> ();

				EntityModificationEntry entityModificationEntry = this.DataInfrastructure.CreateEntityModificationEntry ();

				newEntityKeys = this.JobProcessor.ProcessJobs (transaction, entityModificationEntry, jobs);

				transaction.Commit ();

				return newEntityKeys;
			}
		}


		/// <summary>
		/// Tells the associated <see cref="DataContext"/> that the <see cref="AbstractEntity"/>
		/// that where to be saved are saved and that it must mark them as saved.
		/// </summary>
		/// <param name="entitiesToSave"></param>
		private void CleanSavedEntities(IEnumerable<AbstractEntity> entitiesToSave)
		{
			foreach (AbstractEntity entity in entitiesToSave)
			{
				entity.SetModifiedValuesAsOriginalValues ();
			}
		}


		/// <summary>
		/// Assigns the <see cref="AbstractEntity"/> that have been inserted in the database to their
		/// new <see cref="DbKey"/>.
		/// </summary>
		/// <param name="newEntityKeys">The <see cref="AbstractEntity"/> that have been inserted in the database.</param>
		private void AssignNewEntityKeys(IEnumerable<KeyValuePair<AbstractEntity, DbKey>> newEntityKeys)
		{
			foreach (var newEntityKey in newEntityKeys)
			{
				AbstractEntity entity = newEntityKey.Key;
				DbKey key = newEntityKey.Value;

				this.DataContext.DefineRowKey (entity, key);
			}
		}


		/// <summary>
		/// Converts a sequence of <see cref="AbstractPersistenceJob"/> to the equivalent sequence of
		/// <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="jobs">The <see cref="AbstractPersistenceJob"/> to convert.</param>
		/// <returns>The converted <see cref="AbstractSynchronizationJob"/>.</returns>
		private IEnumerable<AbstractSynchronizationJob> ConvertPersistenceJobs(IEnumerable<AbstractPersistenceJob> jobs)
		{
			return jobs.SelectMany (j => j.Convert (this.JobConverter)).ToList ();
		}


		/// <summary>
		/// Updates the data generation of the associated <see cref="EntityContext"/>.
		/// </summary>
		private void UpdateDataGeneration()
		{
			this.DataContext.EntityContext.NewDataGeneration ();
		}


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> that should be saved must really be saved
		/// or if it should be discarded.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> must really be saved, <c>false</c> if it should not.</returns>
		/// <exception cref="System.InvalidOperationException">If <paramref name="entity"/> is foreign to the associated <see cref="DataContext"/>.</exception>
		public bool CheckIfEntityCanBeSaved(AbstractEntity entity)
		{
			bool canBeSaved;

			if (entity == null)
			{
				canBeSaved = false;
			}
			else
			{
				this.AssertEntityIsNotForeign (entity);

				canBeSaved = !this.DataContext.IsDeleted (entity)
					&& !this.DataContext.IsRegisteredAsEmptyEntity (entity)
					&& !EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (entity)
					&& !EntityNullReferenceVirtualizer.IsNullEntity (entity);
			}

			return canBeSaved;
		}


		/// <summary>
		/// Throws an <see cref="System.InvalidOperationException"/> if the given <see cref="AbstractEntity"/>
		/// is foreign to the associated <see cref="DataContext"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.InvalidOperationException">If <paramref name="entity"/> is foreign to the associated <see cref="DataContext"/>.</exception>
		private void AssertEntityIsNotForeign(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			if (this.DataContext.IsForeignEntity (entity))
			{
				throw new System.InvalidOperationException ("Usage of foreign entity is not allowed.");
			}
		}


		/// <summary>
		/// Checks if a given field of a given <see cref="AbstractEntity"/> must be saved even if
		/// it has not changed.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field to check.</param>
		/// <returns><c>true</c> if the field must be saved again, <c>false</c> if it should not.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public bool CheckIfFieldMustBeResaved(AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty.");

			var fieldsToResave = this.DataContext.GetFieldsToResave ();

			return fieldsToResave.ContainsKey (entity) && fieldsToResave[entity].Contains (fieldId);
		}


	}


}
