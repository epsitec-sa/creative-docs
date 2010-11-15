using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;

using Epsitec.Cresus.DataLayer.Context;
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
		/// The <see cref="EntityContext"/> associated with this instance.
		/// </summary>
		private EntityContext EntityContext
		{
			get
			{
				return this.DataContext.EntityContext;
			}
		}


		/// <summary>
		/// The <see cref="LoaderQueryGenerator"/> used by this instance to generate queries.
		/// </summary>
		private LoaderQueryGenerator LoaderQueryGenerator
		{
			get;
			set;
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

			return from data in this.LoaderQueryGenerator.GetEntitiesData (request)
				   select (T) this.DeserializeEntityData (data);
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

			EntityData targetData = this.LoaderQueryGenerator.GetReferenceField (entity, fieldId);

			AbstractEntity target = null;

			if (targetData != null)
			{
				target = this.DeserializeEntityData (targetData);
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

			return from data in this.LoaderQueryGenerator.GetCollectionField (entity, fieldId)
				   select this.DeserializeEntityData (data);
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

			EntityKey entityKey = EntityKey.CreateNormalizedEntityKey (this.EntityContext, leafEntityId, rowKey);

			AbstractEntity entity = this.DataContext.GetEntity (entityKey);

			if (entity == null)
			{
				entity = this.DataContext.SerializationManager.Deserialize (entityData);
			}

			return entity;
		}


		/// <summary>
		/// Erases the data of the given <see cref="AbstractEntity"/> and replaces it by its data as
		/// stored in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose data to reload.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public void ReloadEntity(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");
			
			Druid entityId = entity.GetEntityStructuredTypeId ();

			Request request = new Request ()
			{
				RootEntity = EntityClassFactory.CreateEmptyEntity (entityId),
				RootEntityKey = this.DataContext.GetNormalizedEntityKey (entity).Value.RowKey
			};

			EntityData data = this.LoaderQueryGenerator.GetEntitiesData (request).FirstOrDefault ();

			if (data != null)
			{
				this.DataContext.SerializationManager.Deserialize (entity, data);
				this.DataContext.DefineLogSequenceNumber (entity, data.LogSequenceNumber);

				this.DataContext.NotifyEntityChanged (entity, EntityChangedEventSource.Reload, EntityChangedEventType.Updated);
			}
			else
			{
				this.DataContext.RemoveAllReferences (entity, EntityChangedEventSource.Internal);
				this.DataContext.MarkAsDeleted (entity);

				this.DataContext.NotifyEntityChanged (entity, EntityChangedEventSource.Reload, EntityChangedEventType.Deleted);
			}
		}


		/// <summary>
		/// Erases the data of the given field of the given <see cref="AbstractEntity"/> and replaces
		/// it by its data as stored in the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose field to reload.</param>
		/// <param name="fieldId">The <see cref="Druid"/> defining which field to reload.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public void ReloadEntityField(AbstractEntity entity, Druid fieldId)
		{
			entity.ThrowIfNull ("entity");
			fieldId.ThrowIf (id => id.IsEmpty, "fieldId cannot be empty");
			
			this.DataContext.SerializationManager.ReplaceFieldByProxy (entity, fieldId);

			this.DataContext.NotifyEntityChanged (entity, EntityChangedEventSource.Reload, EntityChangedEventType.Updated);			
		}


		/// <summary>
		/// Gets the value of the sequence number of the log entry that is currently associated with the
		/// <see cref="AbstractEntity"/> defined by the given <see cref="EntityKey"/>.
		/// </summary>
		/// <param name="normalizedEntityKey">The normalized <see cref="EntityKey"/> defining the <see cref="AbstractEntity"/>.</param>
		/// <returns>The value of the sequence number of the log entry.</returns>
		public long? GetLogSequenceNumberForEntity(EntityKey normalizedEntityKey)
		{
			return this.LoaderQueryGenerator.GetLogSequenceNumberForEntity (normalizedEntityKey);
		}


	}


}
