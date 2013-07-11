﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>DataLoader</c> class is provides the tools used to load from the database the data of
	/// <see cref="AbstractEntity"/>, and to use that data to create instances of them.
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

			Request request = Request.Create (entity, rowKey);

			return this.GetByRequest (request).FirstOrDefault ();
		}


		/// <summary>
		/// Gets the sequence of <see cref="AbstractEntity"/> which corresponds to the given
		/// request.
		/// </summary>
		/// <param name="request">The <see cref="Request"/> defining which <see cref="AbstractEntity"/> to retrieve.</param>
		/// <returns>The <see cref="AbstractEntity"/> which corresponds to the <see cref="Request"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="request"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If request is invalid.</exception>
		public IList<AbstractEntity> GetByRequest(Request request)
		{
			request.ThrowIfNull ("request");
			request.Check (this.DataContext);

			EntityModificationEntry latestEntityModificationEntry;
			IEnumerable<EntityData> entityData;

			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				latestEntityModificationEntry = this.GetLatestEntityModificationEntry ();
				entityData = this.LoaderQueryGenerator.GetEntitiesData (request);

				dbTransaction.Commit ();
			}

			var entities = entityData
				.Select (d => this.DeserializeEntityData (d))
				.ToList ();

			this.AssignModificationEntryIds (latestEntityModificationEntry, entities);

			return entities;
		}


		/// <summary>
		/// Gets the entity keys which corresponds to the given request in the database. The query
		/// is made to the database with the given transaction and will therefore not interfere with
		/// other queries and will be totally independent of them.
		/// </summary>
		/// <param name="request">The request defining which entity keys to get.</param>
		/// <param name="dbTransaction">The transaction to use for the query.</param>
		/// <returns>The entity keys corresponding to the request.</returns>
		public IList<EntityKey> GetEntityKeys(Request request, DbTransaction dbTransaction)
		{
			return this.LoaderQueryGenerator.GetEntityKeys (request, dbTransaction).ToList ();
		}


		/// <summary>
		/// Gets the index of the entity key as it will be in the given request to the database.
		/// </summary>
		/// <remarks>
		/// The order defined by the request must be strict and total, otherwise you will get dummy
		/// results. And anyway, you shouldn't rely on indexes where the order is not strict and
		/// total, because there are no guarantees whatsoever that the rows will always be in the
		/// same order.
		/// </remarks>
		/// <param name="request">The request defining the order of the entity keys.</param>
		/// <param name="entityKey">The entity key whose index to get.</param>
		/// <returns>The index of the given entity key within the result of the request.</returns>
		public int? GetIndex(Request request, EntityKey entityKey)
		{
			return this.LoaderQueryGenerator.GetIndex (request, entityKey);
		}


		/// <summary>
		/// Gets the index of the entity key as it will be in the given request to the database. The
		/// query is made to the database with the given transaction and will therefore not
		/// interfere with other queries and will be totally independent of them.
		/// </summary>
		/// <remarks>See remarks in the other overload of this method.</remarks>
		/// <param name="request">The request defining the order of the entity keys.</param>
		/// <param name="entityKey">The entity key whose index to get.</param>
		/// <param name="dbTransaction">The transaction to use for the query.</param>
		/// <returns>The index of the given entity key within the result of the request.</returns>
		public int? GetIndex(Request request, EntityKey entityKey, DbTransaction dbTransaction)
		{
			return this.LoaderQueryGenerator.GetIndex (request, entityKey, dbTransaction);
		}


		/// <summary>
		/// Gets the number of entities which correspond to the given request in the database.
		/// </summary>
		/// <param name="request">The request defining which entities to count.</param>
		/// <returns>The number of entities that match the given request.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="request"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If request is invalid.</exception>
		public int GetCount(Request request)
		{
			request.ThrowIfNull ("request");
			request.Check (this.DataContext);

			return this.LoaderQueryGenerator.GetCount (request);
		}


		/// <summary>
		/// Gets the number of entities which correspond to the given request in the database. The
		/// query is made to the database with the given transaction and will therefore not
		/// interfere with other queries and will be totally independent of them.
		/// </summary>
		/// <param name="request">The request defining which entities to count.</param>
		/// <param name="dbTransaction">The transaction to use for the query.</param>
		/// <returns>The number of entities that match the given request.</returns>
		public int GetCount(Request request, DbTransaction dbTransaction)
		{
			return this.LoaderQueryGenerator.GetCount (request, dbTransaction);
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

			EntityModificationEntry latestEntityModificationEntry;
			EntityData entityData;

			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				latestEntityModificationEntry = this.GetLatestEntityModificationEntry ();
				entityData = this.LoaderQueryGenerator.GetReferenceField (entity, fieldId);

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

			EntityModificationEntry latestEntityModificationEntry;
			IEnumerable<EntityData> entityData;

			using (DbTransaction dbTransaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				latestEntityModificationEntry = this.GetLatestEntityModificationEntry ();
				entityData = this.LoaderQueryGenerator.GetCollectionField (entity, fieldId);

				dbTransaction.Commit ();
			}

			var entities = entityData.Select (d => this.DeserializeEntityData (d)).ToList ();

			this.AssignModificationEntryIds (latestEntityModificationEntry, entities);

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

			var example = EntityClassFactory.CreateEmptyEntity (entityTypeId);

			Request request = new Request ()
			{
				RootEntity = example,
			};

			request.Conditions.Add
			(
				new BinaryComparison
				(
					InternalField.CreateModificationEntryId (example),
					BinaryComparator.IsGreater,
					new Constant (currentlogId)
				)
			);

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

			EntityKey entityKey = new EntityKey (leafEntityId, rowKey);
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

		private EntityModificationEntry GetLatestEntityModificationEntry()
		{
			return this.DataContext.EnableReload
				? this.DataInfrastructure.GetLatestEntityModificationEntry ()
				: null;
		}

		/// <summary>
		/// Assigns the log ids to the entity types in the associated DataContext.
		/// </summary>
		/// <param name="entityModificationEntry">The log entry that contains the new log id.</param>
		/// <param name="entities">The <see cref="AbstractEntity"/> for which to assign the new log ids.</param>
		private void AssignModificationEntryIds(EntityModificationEntry entityModificationEntry, IEnumerable<AbstractEntity> entities)
		{
			if (!this.DataContext.EnableReload)
			{
				return;
			}

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


	}


}
