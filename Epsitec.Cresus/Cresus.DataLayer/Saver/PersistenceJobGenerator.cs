using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;
using System;


namespace Epsitec.Cresus.DataLayer.Saver
{


	/// <summary>
	/// The <c>PersistenceJobGenerator</c> class is used to build the <see cref="AbstractPersistenceJob"/>
	/// that describe the modifications that must be applied on an <see cref="AbstractEntity"/> in
	/// the database in order to persist it.
	/// </summary>
	internal sealed class PersistenceJobGenerator
	{


		/// <summary>
		/// Creates a new <c>PersistenceJobGenerator</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> that will be used to persist the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public PersistenceJobGenerator(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> used to persist the <see cref="AbstractEntity"/>.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		private EntityTypeEngine TypeEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed in
		/// order to insert the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose insertion <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public IEnumerable<AbstractPersistenceJob> CreateInsertionJobs(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			var jobs1 = this.CreateInsertionValueJobs (entity);
			var jobs2 = this.CreateInsertionReferenceJobs (entity);
			var jobs3 = this.CreateInsertionCollectionJobs (entity);

			return jobs1.Concat (jobs2).Concat (jobs3).Where (j => j!=null);
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed in
		/// order to update the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose update <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public IEnumerable<AbstractPersistenceJob> CreateUpdateJobs(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			var jobs1 = this.CreateUpdateValueJobs (entity);
			var jobs2 = this.CreateUpdateReferenceJobs (entity);
			var jobs3 = this.CreateUpdateCollectionJobs (entity);

			return jobs1.Concat (jobs2).Concat (jobs3).Where (j => j!=null);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed in order to 
		/// remove the given <see cref="AbstractEntity"/> in from database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose delete <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public AbstractPersistenceJob CreateDeletionJob(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			return new DeletePersistenceJob (entity);
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// insert the values of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateInsertionValueJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			
			return from localEntityType in this.TypeEngine.GetBaseTypes (leafEntityId)
				   let localEntityId = localEntityType.CaptionId
				   select this.CreateInsertionValueJob (entity, localEntityId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to insert the
		/// values of the given subtype of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the subtype of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateInsertionValueJob(AbstractEntity entity, Druid localEntityId)
		{
			var fieldIds = from field in this.TypeEngine.GetLocalValueFields(localEntityId)
						   select field.CaptionId;

			return this.CreateValueJob (entity, localEntityId, fieldIds, PersistenceJobType.Insert);
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// update the values of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateUpdateValueJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from localEntityType in this.TypeEngine.GetBaseTypes (leafEntityId)
				   let localEntityId = localEntityType.CaptionId
				   select this.CreateUpdateValueJob (entity, localEntityId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to update the
		/// values of the given subtype of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the subtype of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateUpdateValueJob(AbstractEntity entity, Druid localEntityId)
		{
			AbstractPersistenceJob job = null;

			List<Druid> fieldIds = new List<Druid> (
				from field in this.TypeEngine.GetLocalValueFields(localEntityId)
				let fieldId = field.CaptionId
				where entity.HasValueChanged (fieldId)
				select fieldId
			);

			Druid rootEntityId = this.TypeEngine.GetRootType (localEntityId).CaptionId;
			bool isRootType = localEntityId == rootEntityId;

			if (fieldIds.Any () || isRootType)
			{
				job = this.CreateValueJob (entity, localEntityId, fieldIds, PersistenceJobType.Update);
			}

			return job;
		}


		/// <summary>
		/// Creates the appropriate <see cref="AbstractPersistenceJob"/> that must be used to insert
		/// or update the given fields of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the subtype of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="fieldIds">The sequence of <see cref="Druid"/> identifying the fields to insert or update.</param>
		/// <param name="jobType">Tells whether to update or insert the values.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateValueJob(AbstractEntity entity, Druid localEntityId, IEnumerable<Druid> fieldIds, PersistenceJobType jobType)
		{
			Druid rootEntityId = this.TypeEngine.GetRootType (localEntityId).CaptionId;
			bool isRootType = localEntityId == rootEntityId;

			var fieldIdsWithValues = fieldIds.ToDictionary
			(
				id => id,
				id => entity.GetField<object> (id.ToResourceId ())
			);

			return new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootType, jobType);
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// insert the references of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateInsertionReferenceJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from localEntityType in this.TypeEngine.GetBaseTypes (leafEntityId)
				   let localEntityId = localEntityType.CaptionId
				   select this.CreateInsertionReferenceJob (entity, localEntityId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to insert the
		/// reference of the given field of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateInsertionReferenceJob(AbstractEntity entity, Druid localEntityId)
		{
			AbstractPersistenceJob job = null;

			var fieldIds = new List<Druid> (
				from field in this.TypeEngine.GetLocalReferenceFields(localEntityId)
				let fieldId = field.CaptionId
				let target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select fieldId
			);

			if (fieldIds.Any ())
			{
				job = this.CreateReferenceJob (entity, localEntityId, fieldIds, PersistenceJobType.Insert);
			}

			return job;
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// update the references of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateUpdateReferenceJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from localEntityType in this.TypeEngine.GetBaseTypes (leafEntityId)
				   let localEntityId = localEntityType.CaptionId
				   select this.CreateUpdateReferenceJob (entity, localEntityId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to update the
		/// reference of the given field of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateUpdateReferenceJob(AbstractEntity entity, Druid localEntityId)
		{
			AbstractPersistenceJob job = null;

			List<Druid> fieldIds = new List<Druid> (
				from field in this.TypeEngine.GetLocalReferenceFields (localEntityId)
				let fieldId = field.CaptionId
				where entity.HasReferenceChanged (fieldId)
				   || this.DataContext.DataSaver.CheckIfFieldMustBeResaved (entity, fieldId)
				let target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ())
				where target == null
				   || this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select fieldId
			);

			if (fieldIds.Any ())
			{
				job = this.CreateReferenceJob (entity, localEntityId, fieldIds, PersistenceJobType.Update);
			}

			return job;
		}

		
		/// <summary>
		/// Creates the appropriate <see cref="AbstractPersistenceJob"/> that must be used to insert
		/// or update the given fields of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> defining the subtype of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="fieldIds">The sequence of <see cref="Druid"/> identifying the fields to insert or update.</param>
		/// <param name="jobType">Tells whether to update or insert the values.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateReferenceJob(AbstractEntity entity, Druid localEntityId, IEnumerable<Druid> fieldIds, PersistenceJobType jobType)
		{
			var fieldIdsWithTargets = fieldIds.ToDictionary
			(
				id => id,
				id => entity.GetField<AbstractEntity> (id.ToResourceId ())
			);

			return new ReferencePersistenceJob (entity, localEntityId, fieldIdsWithTargets, jobType);
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// insert the collections of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateInsertionCollectionJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.TypeEngine.GetCollectionFields(leafEntityId)
				   select this.CreateInsertionCollectionJob (entity, field.CaptionId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to insert the
		/// collection of the given field of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateInsertionCollectionJob(AbstractEntity entity, Druid fieldId)
		{
			CollectionPersistenceJob job = null;

			IEnumerable<AbstractEntity> targets = new List<AbstractEntity>
			(
				from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select target
			);

			if (targets.Any ())
			{
				PersistenceJobType jobType = PersistenceJobType.Insert;

				job = this.CreateCollectionJob (entity, fieldId, targets, jobType);
			}

			return job;
		}


		/// <summary>
		/// Creates the sequence of <see cref="AbstractPersistenceJob"/> that must be executed to
		/// update the collections of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The sequence of <see cref="AbstractPersistenceJob"/>.</returns>
		private IEnumerable<AbstractPersistenceJob> CreateUpdateCollectionJobs(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.TypeEngine.GetCollectionFields(leafEntityId)
				   let fieldId = field.CaptionId
				   where entity.HasCollectionChanged (fieldId)
					  || this.DataContext.DataSaver.CheckIfFieldMustBeResaved (entity, fieldId)
				   select this.CreateUpdateCollectionJob (entity, field.CaptionId);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to update the
		/// collection of the given field of the given <see cref="AbstractEntity"/> in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private AbstractPersistenceJob CreateUpdateCollectionJob(AbstractEntity entity, Druid fieldId)
		{
			IEnumerable<AbstractEntity> targets =
				from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select target;

			PersistenceJobType jobType = PersistenceJobType.Update;

			return this.CreateCollectionJob (entity, fieldId, targets, jobType);
		}


		/// <summary>
		/// Creates the <see cref="AbstractPersistenceJob"/> that must be executed to insert or
		/// update the collection of the given field of the given <see cref="AbstractEntity"/> in
		/// the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field of the <see cref="AbstractEntity"/> whose <see cref="AbstractPersistenceJob"/> to create.</param>
		/// <param name="targets">The sequence of targets of the field.</param>
		/// <param name="jobType">Indicates whether the <see cref="AbstractPersistenceJob"/> is an update or an insert.</param>
		/// <returns>The <see cref="AbstractPersistenceJob"/>.</returns>
		private CollectionPersistenceJob CreateCollectionJob(AbstractEntity entity, Druid fieldId, IEnumerable<AbstractEntity> targets, PersistenceJobType jobType)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.TypeEngine.GetLocalType (leafEntityId, fieldId).CaptionId;

			return new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);
		}


	}


}
