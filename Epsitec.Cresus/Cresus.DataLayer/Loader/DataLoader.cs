using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{

	
	/// <summary>
	/// The <c>DataLoader</c> class is provides the tools used to load from the database the data of
	/// <see cref="AbstractEntity"/>.
	/// </summary>
	internal sealed class DataLoader
	{


		/// <summary>
		/// Builds a new <c>DataLoader</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> associated with this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public DataLoader(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
			this.DataContext = dataContext;
			this.LoaderQueryGenerator = new LoaderQueryGenerator (dataContext);
		}


		/// <summary>
		/// The <see cref="DataContext"/> associated with this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="LoaderQueryGenerator"/> used by this instance to generate queries.
		/// </summary>
		private LoaderQueryGenerator LoaderQueryGenerator
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
		/// The <see cref="DbInfrastructure"/> used by this instance.
		/// </summary>
		private DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.DataInfrastructure.DbInfrastructure;
			}
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> defined by the given <see cref="DbKey"/> and entity
		/// id out of the database.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/>.</param>
		/// <param name="rowKey">The <see cref="DbKey"/> defining the id of the <see cref="AbstractEntity"/>.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="entityId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="rowKey"/> is empty.</exception>
		public AbstractEntity ResolveEntity(Druid entityId, DbKey rowKey)
		{
			entityId.ThrowIf (id => id.IsEmpty, "entityId cannot be empty");
			rowKey.ThrowIf (key => key.IsEmpty, "rowKey cannot be empty");
			
			AbstractEntity entity = EntityClassFactory.CreateEmptyEntity (entityId);

			Request request = new Request ()
			{
				RootEntity = entity,
				RootEntityKey = rowKey,
			};

			return this.GetByRequest<AbstractEntity> (request).FirstOrDefault ();
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractEntity"/> of type <typeparamref name="T"/> which
		/// corresponds to the given example.
		/// </summary>
		/// <typeparam name="T">The type of the <see cref="AbstractEntity"/> to retrieve.</typeparam>
		/// <param name="example">The example describing the <see cref="AbstractEntity"/> to retrieve.</param>
		/// <returns>The <see cref="AbstractEntity"/> which corresponds to the example.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="example"/> is <c>null</c>.</exception>
		public IEnumerable<T> GetByExample<T>(T example) where T : AbstractEntity
		{
			example.ThrowIfNull ("example");

			Request request = new Request ()
			{
				RootEntity = example,
				RequestedEntity = example,
			};

			return this.GetByRequest<T> (request);
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractEntity"/> of type <typeparamref name="T"/> which
		/// corresponds to the given request.
		/// </summary>
		/// <typeparam name="T">The type of the <see cref="AbstractEntity"/> to retrieve.</typeparam>
		/// <param name="request">The <see cref="Request"/> defining which <see cref="AbstractEntity"/> to retrieve.</param>
		/// <returns>The <see cref="AbstractEntity"/> which corresponds to the <see cref="Request"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="request"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If request.RootEntity is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If request.RequestedEntity is <c>null</c>.</exception>
		public IEnumerable<T> GetByRequest<T>(Request request) where T : AbstractEntity
		{
			request.ThrowIfNull ("request");
			request.RootEntity.ThrowIfNull ("request.RootEntity");
			request.RequestedEntity.ThrowIfNull ("request.RequestedEntity");

			this.CheckForCycles (request.RootEntity);
			this.CheckForForeignEntities (request.RootEntity);

			EntityModificationEntry latestEntityModificationEntry;
			IEnumerable<EntityData> entityData;

			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				latestEntityModificationEntry = this.DataInfrastructure.GetLatestEntityModificationEntry ();
				entityData = this.LoaderQueryGenerator.GetEntitiesData (request);

				dbTransaction.Commit ();
			}

			List<T> entities = entityData.Select (d => (T) this.DeserializeEntityData (d)).ToList ();

			this.AssignModificationEntryIds (latestEntityModificationEntry, entities);

			return entities;
		}


		/// <summary>
		/// Gets the value of a value field of an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose value to get.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field whose value to get.</param>
		/// <returns>The value of the field of the <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public object ResolveValueField(AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty");

			return this.LoaderQueryGenerator.GetValueField (entity, fieldId);
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> which is the target of a reference field of another
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose reference field to resolve.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field </param>
		/// <returns>The <see cref="AbstractEntity"/> which is the target.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public AbstractEntity ResolveReferenceField(AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty");

			EntityData entityData;
			EntityModificationEntry latestEntityModificationEntry;
			
			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				entityData = this.LoaderQueryGenerator.GetReferenceField (entity, fieldId);
				latestEntityModificationEntry = this.DataInfrastructure.GetLatestEntityModificationEntry ();

				dbTransaction.Commit ();
			}

			AbstractEntity target = null;

			if (entityData != null)
			{
				target = this.DeserializeEntityData (entityData);

				this.AssignModificationEntryIds (latestEntityModificationEntry, new List<AbstractEntity> () { target });
			}

			return target;
		}


		/// <summary>
		/// Gets the collection of <see cref="AbstractEntity"/> which are the target of a collection
		/// field of another <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose collection field to resolve.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining the field </param>
		/// <returns>The <see cref="AbstractEntity"/> which are the targets.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public IEnumerable<AbstractEntity> ResolveCollectionField(AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty");

			IEnumerable<EntityData> entityData;
			EntityModificationEntry latestModificationEntry;
			
			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				entityData = this.LoaderQueryGenerator.GetCollectionField (entity, fieldId);
				latestModificationEntry = this.DataInfrastructure.GetLatestEntityModificationEntry ();

				dbTransaction.Commit ();
			}
			
			var entities = entityData.Select (d => this.DeserializeEntityData (d)).ToList ();

			this.AssignModificationEntryIds (latestModificationEntry, entities);

			return entities;
		}


		/// <summary>
		/// Reloads the data of all the out dated <see cref="AbstractEntity"/> of the given type in
		/// the associated <see cref="DataContext"/>.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> that defined the type of the <see cref="AbstractEntity"/> to reload.</param>
		/// <param name="currentlogId">The log id of the <see cref="AbstractEntity"/> of the given types in the <see cref="DataContext"/>.</param>
		/// <param name="newLogId">The current log id.</param>
		/// <returns><c>true</c> if an <see cref="AbstractEntity"/> has been reloaded, <c>false</c> if none have been reloaded.</returns>
		public bool ReloadOutDatedEntities(Druid entityTypeId, long currentlogId, long newLogId)
		{
			bool modifications = false;

			Request request = new Request ()
			{
				RootEntity = EntityClassFactory.CreateEmptyEntity (entityTypeId),
				RequestedEntityMinimumLogId = currentlogId + 1,
			};

			var result = this.LoaderQueryGenerator.GetEntitiesData (request);

			foreach (EntityData entityData in result)
			{
				bool modified = this.DeserializeEntityData (entityData, newLogId);

				modifications = modifications || modified;
			}

			this.DataContext.DefineEntityModificationEntryId (entityTypeId, newLogId);

			return modifications;
		}


		/// <summary>
		/// Builds the <see cref="AbstractEntity"/> which corresponds to the given
		/// <see cref="EntityData"/>.
		/// </summary>
		/// <param name="entityData">The <see cref="EntityData"/> containing the data of the <see cref="AbstractEntity"/>.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entityData"/> is <c>null</c>.</exception>
		public AbstractEntity DeserializeEntityData(EntityData entityData)
		{
			entityData.ThrowIfNull ("entityData");

			Druid leafEntityId = entityData.LeafEntityId;
			DbKey rowKey = entityData.RowKey;

			EntityKey entityKey = new EntityKey(leafEntityId, rowKey);
			AbstractEntity entity = this.DataContext.GetEntity (entityKey);

			if (entity == null)
			{
				entity = this.DataContext.SerializationManager.Deserialize (entityData);
			}

			return entity;
		}


		private bool DeserializeEntityData(EntityData entityData, long newLogId)
		{
			bool modifications = false;

			Druid leafEntityId = entityData.LeafEntityId;
			DbKey rowKey = entityData.RowKey;
			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);

			AbstractEntity entity = this.DataContext.GetEntity (entityKey);

			if (entity != null)
			{
				long? oldEntityLogId = this.DataContext.GetEntityModificationEntryId (entity);
				long newEntityLogId = entityData.LogId;

				if (!oldEntityLogId.HasValue || oldEntityLogId.Value < newEntityLogId)
				{
					this.DataContext.SerializationManager.Deserialize (entity, entityData);
					
					entity.ResetDataGeneration ();

					this.DataContext.NotifyEntityChanged (entity, EntityChangedEventSource.Reload, EntityChangedEventType.Updated);

					modifications = true;
				}

				this.DataContext.DefineEntityModificationEntryId (entity, newLogId);
			}

			return modifications;
		}

		/// <summary>
		/// Assigns the log ids to the entity types in the associated DataContext.
		/// </summary>
		/// <param name="entityModificationEntry">The log entry that contains the new log id.</param>
		/// <param name="entities">The <see cref="AbstractEntity"/> for which to assign the new log ids.</param>
		private void AssignModificationEntryIds(EntityModificationEntry entityModificationEntry, IEnumerable<AbstractEntity> entities)
		{
			var entityTypeIds = entities
				.Select (e => e.GetEntityStructuredTypeId ())
				.Distinct ()
				.Where (d => !this.DataContext.GetEntityModificationEntryId (d).HasValue);

			long entryId = entityModificationEntry == null ? 1 : entityModificationEntry.EntryId.Value;

			foreach (Druid entityTypeId in entityTypeIds)
			{
				this.DataContext.DefineEntityModificationEntryId (entityTypeId, entryId);
			}

			foreach (AbstractEntity entity in entities)
			{
				this.DataContext.DefineEntityModificationEntryId (entity, entryId);
			}
		}


		private void CheckForCycles(AbstractEntity entity)
		{
			if (!this.DataContext.IsPersistent (entity))
			{
				ISet<AbstractEntity> currentEntities = new HashSet<AbstractEntity> ();
				ISet<AbstractEntity> entitiesChecked = new HashSet<AbstractEntity> ();

				this.CheckForCycles (currentEntities, entitiesChecked, entity);
			}
		}


		private void CheckForCycles(ISet<AbstractEntity> currentEntities, ISet<AbstractEntity> entitiesChecked, AbstractEntity entity)
		{
			if (currentEntities.Contains (entity))
			{
				throw new System.ArgumentException ("Cycles are not allowed in requests.");
			}

			if (!entitiesChecked.Contains (entity))
			{
				currentEntities.Add (entity);

				var targets = this.GetDefinedChildren (entity)
					.Where (e => !this.DataContext.IsPersistent (e))
					.ToList ();

				foreach (var target in targets)
				{
					this.CheckForCycles (currentEntities, entitiesChecked, target);
				}

				currentEntities.Remove (entity);
				entitiesChecked.Add (entity);
			}
		}


		private void CheckForForeignEntities(AbstractEntity entity)
		{
			HashSet<AbstractEntity> entitiesChecked = new HashSet<AbstractEntity> ();
			HashSet<AbstractEntity> entitiesToCheck = new HashSet<AbstractEntity> ()
			{
				entity,
			};

			while (entitiesToCheck.Count > 0)
			{
				var entityToCheck = entitiesToCheck.First ();

				entitiesToCheck.Remove (entityToCheck);

				if (this.DataContext.IsForeignEntity (entityToCheck))
				{
					throw new System.ArgumentException ("Usage of a foreign entity in a request is not allowed.");
				}

				entitiesChecked.Add (entityToCheck);

				if (!this.DataContext.IsPersistent (entityToCheck))
				{
					var targets = this.GetDefinedChildren (entityToCheck)
						.Where (e => !entitiesChecked.Contains (e));

					entitiesToCheck.AddRange (targets);
				}
			}
		}


		private IEnumerable<AbstractEntity> GetDefinedChildren(AbstractEntity entity)
		{
			EntityTypeEngine entityTypeEngine = this.DataInfrastructure.EntityEngine.EntityTypeEngine;

			Druid entityTypeId = entity.GetEntityStructuredTypeId ();

			var referenceTargets = entityTypeEngine
				.GetReferenceFields (entityTypeId)
				.Where (f => entity.IsFieldNotEmpty (f.Id))
				.Select (f => entity.GetField<AbstractEntity> (f.CaptionId.ToResourceId ()));

			var collectionTargets = entityTypeEngine
				.GetCollectionFields (entityTypeId)
				.Where (f => entity.IsFieldNotEmpty (f.Id))
				.SelectMany (f => entity.GetFieldCollection<AbstractEntity> (f.CaptionId.ToResourceId ()));

			return referenceTargets.Concat (collectionTargets);
		}

		
	}


}
