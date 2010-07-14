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


		private EntityContext EntityContext
		{
			get;
			set;
		}


		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataContext.DbInfrastructure;
			}
		}


		private SaverQueryGenerator SaverQueryGenerator
		{
			get;
			set;
		}


		public void SaveChanges()
		{
			bool deletedEntities = this.DeleteEntities ();
			bool savedEntities = this.SaveEntities ();

			this.UpdateDataGeneration (deletedEntities || savedEntities);
		}


		private bool DeleteEntities()
		{
			List<AbstractEntity> entitiesToDelete = this.DataContext.GetEntitiesToDelete ().ToList ();
			
			foreach (AbstractEntity entity in entitiesToDelete)
			{
				this.RemoveEntity (entity);
				this.DataContext.MarkAsDeleted (entity);
			}

			this.DataContext.ClearEntitiesToDelete ();

			return entitiesToDelete.Any ();
		}


		private void RemoveEntity(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (entity);

			if (mapping.SerialGeneration != this.EntityContext.DataGeneration)
			{
				mapping.SerialGeneration = this.EntityContext.DataGeneration;

				if (!mapping.RowKey.IsEmpty)
				{
					this.DeleteEntityTargetRelationsInMemory (entity);

					this.ExecuteWithTransaction (transaction =>
					{
						this.SaverQueryGenerator.DeleteEntity (transaction, entity);
					});
				}
			}
		}


		private void DeleteEntityTargetRelationsInMemory(AbstractEntity entity)
		{
			// TODO This method might be optimized by doing the same thing without using the
			// DataBrowser, i.e. by looping over all the managed entities in the DataContext. This
			// would save some queries to the database.
			// Marc

			foreach (var item in this.DataContext.GetReferencers (entity, ResolutionMode.Memory))
			{
				AbstractEntity sourceEntity = item.Item1;
				StructuredTypeField field = this.EntityContext.GetStructuredTypeField (sourceEntity, item.Item2.Fields.First ());

				using (sourceEntity.UseSilentUpdates ())
				{
					switch (field.Relation)
					{
						case FieldRelation.Reference:
						{
							sourceEntity.InternalSetValue (field.Id, null);

							break;
						}
						case FieldRelation.Collection:
						{
							IList collection = sourceEntity.InternalGetFieldCollection (field.Id) as IList;

							while (collection.Contains (entity))
							{
								collection.Remove (entity);
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
		}


		private bool SaveEntities()
		{
			List<AbstractEntity> entitiesToSave = new List<AbstractEntity> (
				from entity in this.DataContext.GetEntitiesModified ()
				where this.DataContext.CheckIfEntityCanBeSaved (entity)
				select entity
			);

			foreach (AbstractEntity entity in entitiesToSave)
			{
				this.SaveEntity (entity);
			}

			return entitiesToSave.Any ();
		}


		private void SaveEntity(AbstractEntity entity)
		{
			if (!this.DataContext.Contains (entity))
			{
				// TODO: Should we propagate the serialization to another DataContext ?
				// Pierre
				return;
			}

			EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (entity);

			if (mapping.SerialGeneration == this.EntityContext.DataGeneration)
			{
				return;
			}

			mapping.SerialGeneration = this.EntityContext.DataGeneration;

			bool isNotPersisted = mapping.RowKey.IsEmpty;

			if (isNotPersisted)
			{
				DbKey newKey = this.ExecuteWithTransaction (transaction =>
				{
					return this.SaverQueryGenerator.GetNewDbKey (transaction, entity);
				});

				this.DataContext.DefineRowKey (mapping, newKey);
			}

			this.SaveTargetsIfNotPersisted (entity);

			if (isNotPersisted)
			{
				this.InsertEntity (entity);
			}
			else
			{
				this.UpdateEntity (entity);
			}
		}


		private void InsertEntity(AbstractEntity entity)
		{
			this.ExecuteWithTransaction (transaction =>
			{
				this.SaverQueryGenerator.InsertEntity (transaction, entity);
			});
		}


		private void UpdateEntity(AbstractEntity entity)
		{
			this.ExecuteWithTransaction (transaction =>
			{
				this.SaverQueryGenerator.UpdateEntity (transaction, entity);
			});
		}


		private void SaveTargetsIfNotPersisted(AbstractEntity source)
		{
			Druid leafEntityId = source.GetEntityStructuredTypeId ();

			var relations = from field in this.EntityContext.GetEntityFieldDefinitions (leafEntityId)
							let rel = field.Relation
							where rel == FieldRelation.Reference || rel == FieldRelation.Collection
							select field;

			foreach (StructuredTypeField field in relations)
			{
				this.SaveTargetsIfNotPersisted (source, field);
			}
		}


		private void SaveTargetsIfNotPersisted(AbstractEntity source, StructuredTypeField field)
		{
			switch (field.Relation)
			{
				case FieldRelation.Reference:
				{
					AbstractEntity target = source.GetField<AbstractEntity> (field.Id);

					if (target != null)
					{
						this.SaveTargetIfNotPersisted (target);
					}

					break;
				}
				case FieldRelation.Collection:
				{
					var targets = from target in source.GetFieldCollection<AbstractEntity> (field.Id)
								  where target != null
								  select target;

					foreach (AbstractEntity target in targets)
					{
						this.SaveTargetIfNotPersisted (target);
					}
					
					break;
				}
				default:
				{
					throw new System.InvalidOperationException ();
				}
			}
		}


		private void SaveTargetIfNotPersisted(AbstractEntity target)
		{
			EntityDataMapping mapping = this.DataContext.GetEntityDataMapping (target);

			if (mapping.RowKey.IsEmpty)
			{
				this.SaveEntity (target);
			}
		}


		private void UpdateDataGeneration(bool containsChanges)
		{
			if (containsChanges)
			{
				this.EntityContext.NewDataGeneration ();
			}
		}


		private void ExecuteWithTransaction(System.Action<DbTransaction> query)
		{
			this.ExecuteWithTransaction<object> (transaction =>
			{
				query (transaction);
				return null;
			});
		}


		private TResult ExecuteWithTransaction<TResult>(System.Func<DbTransaction, TResult> query)
		{
			TResult result;
			
			using (DbTransaction transaction = this.DbInfrastructure.BeginTransaction ())
			{
				try
				{
					result = query (transaction);

					transaction.Commit ();
				}
				catch
				{
					transaction.Rollback ();

					throw;
				}
			}

			return result;
		}


	}


}
