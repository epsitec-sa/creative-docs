using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{
	
	
	internal sealed class PersistenceJobGenerator
	{

		public PersistenceJobGenerator(DataContext dataContext)
		{
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


		public IEnumerable<AbstractPersistenceJob> InsertEntity(AbstractEntity entity)
		{
			var jobs1 = this.InsertEntityValues (entity);
			var jobs2 = this.InsertEntityReferences (entity);
			var jobs3 = this.InsertEntityCollections (entity);

			return jobs1.Concat (jobs2).Concat (jobs3);
		}


		public IEnumerable<AbstractPersistenceJob> UpdateEntity(AbstractEntity entity)
		{
			var jobs1 = this.UpdateEntityValues (entity);
			var jobs2 = this.UpdateEntityReferences (entity);
			var jobs3 = this.UpdateEntityCollections (entity);

			return jobs1.Concat (jobs2).Concat (jobs3);
		}


		public AbstractPersistenceJob DeleteEntity(AbstractEntity entity)
		{
			return new DeletePersistenceJob (entity);
		}


		private IEnumerable<AbstractPersistenceJob> InsertEntityValues(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			var localEntityIds = this.EntityContext.GetInheritedEntityIds (leafEntityId);

			return from Druid localEntityId in localEntityIds
				   select this.InsertEntityValues (entity, localEntityId);
		}


		private AbstractPersistenceJob InsertEntityValues(AbstractEntity entity, Druid localEntityId)
		{
			var fieldIds = from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
						   where field.Relation == FieldRelation.None
						   select field.CaptionId;

			return this.CreateValuePersistenceJob(entity, localEntityId, fieldIds, PersistenceJobType.Insert);
		}


		private IEnumerable<AbstractPersistenceJob> UpdateEntityValues(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			var localEntityIds = this.EntityContext.GetInheritedEntityIds (leafEntityId);

			return from Druid localEntityId in localEntityIds
				   select this.UpdateEntityValues (entity, localEntityId);
		}


		private AbstractPersistenceJob UpdateEntityValues(AbstractEntity entity, Druid localEntityId)
		{
			AbstractPersistenceJob job = null;
			
			List<Druid> fieldIds = new List<Druid> (
				from field in this.EntityContext.GetEntityLocalFieldDefinitions (localEntityId)
				let fieldId = field.CaptionId
				where field.Relation == FieldRelation.None
				where entity.HasValueChanged(fieldId)
				select fieldId
			);

			if (fieldIds.Any ())
			{
				job = this.CreateValuePersistenceJob (entity, localEntityId, fieldIds, PersistenceJobType.Update);		
			}

			return job;
		}


		private AbstractPersistenceJob CreateValuePersistenceJob(AbstractEntity entity, Druid localEntityId, IEnumerable<Druid> fieldIds, PersistenceJobType jobType)
		{
			Druid rootEntityId = this.EntityContext.GetRootEntityId (localEntityId);
			bool isRootType = localEntityId == rootEntityId;

			var fieldIdsWithValues = fieldIds.ToDictionary
			(
				id => id,
				id => entity.GetField<object> (id.ToResourceId ())
			);

			return new ValuePersistenceJob (entity, localEntityId, fieldIdsWithValues, isRootType, jobType);
		}


		private IEnumerable<AbstractPersistenceJob> InsertEntityReferences(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				   where field.Relation == FieldRelation.Reference
				   select this.InsertEntityReference (entity, field.CaptionId);
		}


		private AbstractPersistenceJob InsertEntityReference(AbstractEntity entity, Druid fieldId)
		{
			ReferencePersistenceJob job = null;
			
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AbstractEntity target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ());

			if (this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target))
			{
				job = new ReferencePersistenceJob (entity, localEntityId, fieldId, target, PersistenceJobType.Insert);
			}

			return job;
		}


		private IEnumerable<AbstractPersistenceJob> UpdateEntityReferences(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				   let fieldId = field.CaptionId
				   where field.Relation == FieldRelation.Reference
				   where entity.HasReferenceChanged (fieldId)
				   select this.UpdateEntityReference (entity, field.CaptionId);
		}


		private AbstractPersistenceJob UpdateEntityReference(AbstractEntity entity, Druid fieldId)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			AbstractEntity target = entity.GetField<AbstractEntity> (fieldId.ToResourceId ());
			AbstractEntity targetToSave;

			if (this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target))
			{
				targetToSave = target;
			}
			else
			{
				target = null;
			}

			return new ReferencePersistenceJob (entity, localEntityId, fieldId, target, PersistenceJobType.Update);
		}


		private IEnumerable<AbstractPersistenceJob> InsertEntityCollections(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				   where field.Relation == FieldRelation.Collection
				   select this.InsertEntityCollection (entity, field.CaptionId);
		}


		private AbstractPersistenceJob InsertEntityCollection(AbstractEntity entity, Druid fieldId)
		{
			CollectionPersistenceJob job = null;

			var targets = new List<AbstractEntity>
			(
				from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
				where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
				select target
			);

			if (targets.Any ())
			{
				PersistenceJobType jobType = PersistenceJobType.Insert;

				job = this.CreateCollectionPersistenceJob (entity, fieldId, targets, jobType);
			}

			return job;
		}


		private IEnumerable<AbstractPersistenceJob> UpdateEntityCollections(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();

			return from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
				   let fieldId = field.CaptionId
				   where field.Relation == FieldRelation.Collection
				   where entity.HasCollectionChanged (fieldId)
				   select this.UpdateEntityCollection (entity, field.CaptionId);
		}


		private AbstractPersistenceJob UpdateEntityCollection(AbstractEntity entity, Druid fieldId)
		{
			var targets = from target in entity.GetFieldCollection<AbstractEntity> (fieldId.ToResourceId ())
						  where this.DataContext.DataSaver.CheckIfEntityCanBeSaved (target)
						  select target;

			PersistenceJobType jobType = PersistenceJobType.Insert;

			return this.CreateCollectionPersistenceJob (entity, fieldId, targets, jobType);
		}


		private CollectionPersistenceJob CreateCollectionPersistenceJob(AbstractEntity entity, Druid fieldId, IEnumerable<AbstractEntity> targets, PersistenceJobType jobType)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			Druid localEntityId = this.EntityContext.GetLocalEntityId (leafEntityId, fieldId);

			return new CollectionPersistenceJob (entity, localEntityId, fieldId, targets, jobType);
		}     


	}


}
