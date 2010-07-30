using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

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


		public void SaveChanges()
		{
			bool containsChanges;

			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				bool deletedEntities = this.DeleteEntities (transaction);
				bool savedEntities = this.SaveEntities (transaction);

				containsChanges = deletedEntities || savedEntities;

				transaction.Commit ();
			}

			if (containsChanges)
			{
				this.UpdateDataGeneration ();
			}
		}


		private bool DeleteEntities(DbTransaction transaction)
		{
			List<AbstractEntity> entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.RemoveEntity (transaction, entity);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();

			return entitiesToDelete.Any ();
		}


		private void RemoveEntity(DbTransaction transaction, AbstractEntity entity)
		{
			this.DeleteEntityTargetRelationsInMemory (entity);

			if (this.DataContext.IsPersistent (entity))
			{
				this.SaverQueryGenerator.DeleteEntity (transaction, entity);
			}
		}


		private void DeleteEntityTargetRelationsInMemory(AbstractEntity target)
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
						this.DeleteEntityTargetRelationInMemory (source, fieldPaths[leafInheritedId], target);
					}
				}
			}
		}


		private void DeleteEntityTargetRelationInMemory(AbstractEntity source, Druid fieldId, AbstractEntity target)
		{
			StructuredTypeField field = this.EntityContext.GetStructuredTypeField (source, fieldId.ToResourceId ());

			using (source.UseSilentUpdates ())
			{
				switch (field.Relation)
				{
					case FieldRelation.Reference:
					{
						if (source.InternalGetValue (field.Id) == target)
						{
							source.InternalSetValue (field.Id, null);
						}

						break;
					}
					case FieldRelation.Collection:
					{
						IList collection = source.InternalGetFieldCollection (field.Id) as IList;

						while (collection.Contains (target))
						{
							collection.Remove (target);
						}

						break;
					}
					default:
					{
						throw new System.InvalidOperationException ();
					}
				}
			}
		}


		private bool SaveEntities(DbTransaction transaction)
		{
			List<AbstractEntity> entitiesToSave = new List<AbstractEntity> (
				from entity in this.DataContext.GetEntitiesModified ()
				where this.CheckIfEntityCanBeSaved (entity)
				select entity
			);

			HashSet<AbstractEntity> savedEntities = new HashSet<AbstractEntity> ();

			foreach (AbstractEntity entity in entitiesToSave)
			{
				this.SaveEntity (transaction, savedEntities, entity);
			}

			foreach (AbstractEntity entity in savedEntities)
			{
				entity.SetModifiedValuesAsOriginalValues ();
			}

			return entitiesToSave.Any ();
		}


		private void SaveEntity(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, AbstractEntity entity)
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
				return;
			}

			bool isPersisted = this.DataContext.IsPersistent (entity);

			if (!isPersisted)
			{
				DbKey newKey = this.SaverQueryGenerator.GetNewDbKey (transaction, entity);

				this.DataContext.DefineRowKey (entity, newKey);
			}

			this.SaveTargetsIfNotPersisted (transaction, savedEntities, entity);

			if (isPersisted)
			{
				this.UpdateEntity (transaction, entity);
			}
			else
			{
				this.InsertEntity (transaction, entity);
			}
		}


		private void InsertEntity(DbTransaction transaction, AbstractEntity entity)
		{
			this.SaverQueryGenerator.InsertEntity (transaction, entity);
		}


		private void UpdateEntity(DbTransaction transaction, AbstractEntity entity)
		{
			this.SaverQueryGenerator.UpdateEntity (transaction, entity);
		}


		private void SaveTargetsIfNotPersisted(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, AbstractEntity source)
		{
			Druid leafEntityId = source.GetEntityStructuredTypeId ();

			var relations = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
							let rel = field.Relation
							where rel == FieldRelation.Reference || rel == FieldRelation.Collection
							select field;

			foreach (StructuredTypeField field in relations)
			{
				this.SaveTargetsIfNotPersisted (transaction, savedEntities, source, field);
			}
		}


		private void SaveTargetsIfNotPersisted(DbTransaction transaction, HashSet<AbstractEntity> savedEntities, AbstractEntity source, StructuredTypeField field)
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
				this.SaveEntity (transaction, savedEntities, target);
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
