//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>DataContext</c> class is responsible of the mapping between the object model and the
	/// relational model of the <see cref="AbstractEntity"/>. It is therefore the designated entry
	/// point for everything which is related to them.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay ("DataContext #{UniqueId}")]
	public sealed class DataContext : System.IDisposable, IEntityPersistenceManager
	{
		/// <summary>
		/// Creates a new <c>DataContext</c>.
		/// </summary>
		/// <param name="infrastructure">The <see cref="DbInfrastructure"/> that will be used to talk to the database.</param>
		/// <param name="enableNullVirtualization">Tells whether to enable the virtualization of null <see cref="AbstractEntity"/> or not.</param>
		public DataContext(DbInfrastructure infrastructure, bool enableNullVirtualization = false)
		{
			this.UniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);
			this.IsDisposed = false;
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure);
			this.EntityContext = new EntityContext ();
			this.DataLoader = new DataLoader (this);
			this.DataSaver = new DataSaver (this);
			this.SerializationManager = new EntitySerializationManager (this);
			this.DataConverter = new DataConverter (this);

			this.EnableNullVirtualization = enableNullVirtualization;

			this.entitiesCache = new EntityCache ();
			this.emptyEntities = new HashSet<AbstractEntity> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.entitiesDeleted = new HashSet<AbstractEntity> ();
			this.fieldsToResave = new Dictionary<AbstractEntity, HashSet<Druid>> ();

			this.eventLock = new object ();

			this.EntityContext.EntityAttached += this.HandleEntityCreated;
			this.EntityContext.EntityChanged += this.HandleEntityChanged;
			this.EntityContext.PersistenceManagers.Add (this);
		}

		/// <summary>
		/// Destructor for the <c>DataContext</c>.
		/// </summary>
		~DataContext()
		{
			this.Dipose (false);
		}


		/// <summary>
		/// Gets the unique id of the current instance.
		/// </summary>
		public int UniqueId
		{
			get;
			private set;
		}

		/// <summary>
		/// Tells whether this instance is disposed or not.
		/// </summary>
		public bool IsDisposed
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the name of the <c>DataContext</c>. This is useful when
		/// debugging.
		/// </summary>
		/// <value>The name of the <c>DataContext</c>.</value>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// The event is fired when an <see cref="AbstractEntity"/> managed by this instance is
		/// created, updated or deleted.
		/// </summary>
		public event EventHandler<EntityChangedEventArgs> EntityChanged
		{
			add
			{
				lock (this.eventLock)
				{
					this.entityChanged += value;
				}
			}
			remove
			{
				lock (this.eventLock)
				{
					this.entityChanged -= value;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="EntityContext"/> associated with this instance.
		/// </summary>
		[System.Obsolete ("This field should be used only internally")]
		public EntityContext EntityContext
		{
			// TODO Make this field internal and apply the changes in core data and remove the
			// obsolete tag.
			// Marc

			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DbInfrastructure"/> associated with this instance.
		/// </summary>
		internal DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="SchemaEngine"/> associated with this instance.
		/// </summary>
		internal SchemaEngine SchemaEngine
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DataLoader"/> associated with this instance.
		/// </summary>
		internal DataLoader DataLoader
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DataSaver"/> associated with this instance.
		/// </summary>
		internal DataSaver DataSaver
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="SerializationManager"/> associated with this instance.
		/// </summary>
		internal EntitySerializationManager SerializationManager
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="DataConverter"/> associated with this instance.
		/// </summary>
		internal DataConverter DataConverter
		{
			get;
			private set;
		}
		

		/// <summary>
		/// Gets or sets the value that tells if the <see cref="AbstractEntity"/> created by this
		/// <see cref="DataContext"/> can be null virtualized with the
		/// <see cref="EntityNullReferenceVirtualizer"/>.
		/// </summary>
		public bool EnableNullVirtualization
		{
			get;
			private set;
		}

		/// <summary>
		/// Creates a new <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> associated
		/// with this instance. This methods fires an event indicating that the <see cref="AbstractEntity"/>
		/// has been created.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to create.</typeparam>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public TEntity CreateEntity<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();

			TEntity entity = this.EntityContext.CreateEmptyEntity<TEntity> ();

			this.NotifyEntityChanged (entity, EntityChangedEventSource.External, EntityChangedEventType.Created);

			entity.UpdateDataGeneration ();

			return entity;
		}


		/// <summary>
		/// Creates a new <see cref="AbstractEntity"/> with the type given by <paramref name="entityId"/>
		/// associated with this instance.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> of the type of the <see cref="AbstractEntity"/> to create.</param>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal AbstractEntity CreateEntity(Druid entityId)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.EntityContext.CreateEmptyEntity (entityId);
		}


		/// <summary>
		/// Creates a new <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> associated
		/// with this instance and registers it as an empty one.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to create.</typeparam>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public TEntity CreateEmptyEntity<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();

			TEntity entity = this.CreateEntity<TEntity> ();

			this.RegisterEmptyEntity (entity);

			return entity;
		}


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is managed by this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check if it is managed by this instance.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> is managed by this instance, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public bool Contains(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.ContainsEntity (entity);
		}

		/// <summary>
		/// Tells whether this instance manages an <see cref="AbstractEntity"/> corresponding to a
		/// given <see cref="EntityKey"/>.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> to check if the corresponding <see cref="AbstractEntity"/> is managed by this instance.</param>
		/// <returns><c>true</c> if the corresponding <see cref="AbstractEntity"/> is managed by this instance, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal bool Contains(EntityKey entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetEntity (entityKey) != null;
		}

		/// <summary>
		/// Gets the <see cref="EntityKey"/> associated with an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="EntityKey"/> to get.</param>
		/// <returns>The <see cref="EntityKey"/> or <c>null</c> if there is none defined in this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public EntityKey? GetEntityKey(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetEntityKey (entity);
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> given by <paramref name="entityKey"/> out of the
		/// cache of this instance. This method does not query the database.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> defining the <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal AbstractEntity GetEntity(EntityKey entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetEntity (entityKey);
		}


		/// <summary>
		/// Associates <paramref name="entity"/> with <paramref name="key"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="DbKey"/> to define.</param>
		/// <param name="key">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void DefineRowKey(AbstractEntity entity, DbKey key)
		{
			this.AssertDataContextIsNotDisposed ();

			this.entitiesCache.DefineRowKey (entity, key);
		}


		/// <summary>
		/// Tells whether the <see cref="AbstractEntity"/> managed by this instance have been
		/// modified since the last call to <see cref="SaveChanges"/>.
		/// </summary>
		/// <returns><c>true</c> if there is any modification, <c>false</c> if there isn't any.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public bool ContainsChanges()
		{
			this.AssertDataContextIsNotDisposed ();

			bool containsDeletedEntities = this.entitiesToDelete.Any ();
			bool containsChangedEntities = this.GetEntitiesModified ().Any ();

			return containsDeletedEntities || containsChangedEntities;
		}


		/// <summary>
		/// Registers an <see cref="AbstractEntity"/> as empty, which means that it will be as if it
		/// didn't exist when saving this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register as empty.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> has already been persisted.</exception>
		public void RegisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			entity.ThrowIfNull ("entity");
			entity.ThrowIf (e => this.IsPersistent (e), "entity");

			if (this.emptyEntities.Add (entity))
			{
				System.Diagnostics.Debug.WriteLine ("Empty entity registered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());

				entity.UpdateDataGeneration ();
				this.ResaveReferencingFields (entity);
			}
		}


		/// <summary>
		/// Unregisters an <see cref="AbstractEntity"/> as empty, which means that if it was registered
		/// as empty, it will now be considered as a normal <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to unregister as empty.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> has already been persisted.</exception>
		public void UnregisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			entity.ThrowIfNull ("entity");

			if (this.emptyEntities.Remove (entity))
			{
				entity.ThrowIf (e => this.IsPersistent (e), "entity");
				
				System.Diagnostics.Debug.WriteLine ("Empty entity unregistered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());
				
				entity.UpdateDataGeneration ();
				this.ResaveReferencingFields (entity);
			}
		}



		/// <summary>
		/// Sets the registered empty status of an <see cref="AbstractEntity"/> to a given value, if
		/// the given <see cref="AbstractEntity"/> is not persistent.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register or unregister.</param>
		/// <param name="isEmpty">A <see cref="bool"/> indicating the future empty status of the <see cref="AbstractEntity"/>.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void UpdateEmptyEntityStatus(AbstractEntity entity, bool isEmpty)
		{
			this.AssertDataContextIsNotDisposed ();

			if (this.IsPersistent (entity))
            {
				return;
            }
			
			if (isEmpty)
			{
				this.RegisterEmptyEntity (entity);
			}
			else
			{
				this.UnregisterEmptyEntity (entity);
			}
		}
		

		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is registered as an empty one within this
		/// instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose empty status to check.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> is registered as empty, <c>false</c> if it isn't.</returns>
		/// <remarks>See the remarks in <see cref="RegisterEmptyEntity"/>.</remarks>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public bool IsRegisteredAsEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.emptyEntities.Contains (entity);
		}


		/// <summary>
		/// Deletes an <see cref="AbstractEntity"/> from the database.
		/// </summary>
		/// <remarks>
		/// Take care with this feature, because it is subtle. It will remove the <see cref="AbstractEntity"/>
		/// from the database during the call to <see cref="SaveChanges"/>. In the meantime, it will
		/// still be considered as a normal <see cref="AbstractEntity"/>. Furthermore, even after it
		/// will have been removed from the database, if you have a reference to it somewhere, you
		/// will be able to use it as a normal <see cref="AbstractEntity"/>, except that nothing will
		/// be persisted to the database.
		/// </remarks>
		/// <param name="entity">The <see cref="AbstractEntity"/> to delete.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void DeleteEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			if (!this.entitiesDeleted.Contains (entity))
			{
				this.entitiesToDelete.Add (entity);
			}
		}


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is deleted in this instance or if it will
		/// be deleted in the next call to <see cref="SaveChanges"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check if it has been deleted.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> has been deleted, <c>false</c> if it hasn't.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public bool IsDeleted(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesDeleted.Contains (entity) || this.entitiesToDelete.Contains (entity);
		}


		/// <summary>
		/// Gets the entities of the specified type.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <returns>The collection of entities of the given type, managed by this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public IEnumerable<TEntity> GetEntities<TEntity>()
			where TEntity : AbstractEntity
		{
			foreach (var entity in this.GetEntities ())
			{
				var expectedEntity = entity as TEntity;

				if (expectedEntity != null)
				{
					yield return expectedEntity;
				}
			}
		}

		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> managed by this instance.
		/// </summary>
		/// <returns>The collection of <see cref="AbstractEntity"/> managed by this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal IEnumerable<AbstractEntity> GetEntities()
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetEntities ();
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> managed by this instance and which contain changes
		/// since the last call to <see cref="SaveChanges"/>.
		/// </summary>
		/// <returns>The modified <see cref="AbstractEntity"/> managed by this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal IEnumerable<AbstractEntity> GetEntitiesModified()
		{
			this.AssertDataContextIsNotDisposed ();

			return from AbstractEntity entity in this.GetEntities ()
				   where entity.GetEntityDataGeneration () >= this.EntityContext.DataGeneration
				   select entity;
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> that have been deleted by this instance.
		/// </summary>
		/// <returns>The <see cref="AbstractEntity"/> that have been deleted by this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal IEnumerable<AbstractEntity> GetEntitiesDeleted()
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesDeleted;
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> which must be deleted during the next call to
		/// <see cref="SaveChanges"/>.
		/// </summary>
		/// <returns>The <see cref="AbstractEntity"/> which must be deleted.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal IEnumerable<AbstractEntity> GetEntitiesToDelete()
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesToDelete;
		}


		/// <summary>
		/// Mark an <see cref="AbstractEntity"/> as deleted, which removes it from this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to mark as deleted.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void MarkAsDeleted(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			this.entitiesDeleted.Add (entity);
			this.entitiesCache.Remove (entity);

			this.NotifyEntityChanged (entity, EntityChangedEventSource.External, EntityChangedEventType.Deleted);
		}


		/// <summary>
		/// Clears the list of the <see cref="AbstractEntity"/> which are to be deleted.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void ClearEntitiesToDelete()
		{
			this.AssertDataContextIsNotDisposed ();

			this.entitiesToDelete.Clear ();
		}

		/// <summary>
		/// Notifies this instance that all the fields referencing an <see cref="AbstractEntity"/>
		/// must be persisted again, even if their value has not changed.
		/// </summary>
		/// <param name="target">The <see cref="AbstractEntity"/> whose referencing fields must be saved again.</param>
		private void ResaveReferencingFields(AbstractEntity target)
		{
			foreach (var item in this.GetPossibleReferencers (target))
			{
				AbstractEntity source = item.Key;
				Druid fieldId = item.Value;

				this.ResaveReferencingField (source, fieldId, target);
			}
		}

		/// <summary>
		/// Notifies this instance that the given field of the given <see cref="AbstractEntity"/> must
		/// be persisted again if it references another <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="source">The <see cref="AbstractEntity"/> that contains the referencing field.</param>
		/// <param name="fieldId">The <see cref="Druid"/> that identifies the referencing field.</param>
		/// <param name="target">The <see cref="AbstractEntity"/> targeted by the referencing field.</param>
		private void ResaveReferencingField(AbstractEntity source, Druid fieldId, AbstractEntity target)
		{
			StructuredTypeField field = this.EntityContext.GetStructuredTypeField (source, fieldId.ToResourceId ());

			bool found;

			switch (field.Relation)
			{
				case FieldRelation.Reference:
					found = source.InternalGetValue (field.Id) == target;
					break;

				case FieldRelation.Collection:
					IList collection = source.InternalGetFieldCollection (field.Id) as IList;
					found = collection.Contains (target);
					break;

				default:
					throw new System.InvalidOperationException ();
			}

			if (found)
			{
				this.ResaveReferencingField (source, fieldId);
			}
		}


		/// <summary>
		/// Notifies this instance that the given field of the given <see cref="AbstractEntity"/>
		/// must be saved again.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> that contains the field.</param>
		/// <param name="fieldId">The field that must be saved again.</param>
		private void ResaveReferencingField(AbstractEntity entity, Druid fieldId)
		{
			entity.UpdateDataGeneration ();

			if (!this.fieldsToResave.ContainsKey (entity))
			{
				this.fieldsToResave[entity] = new HashSet<Druid> ();
			}

			this.fieldsToResave[entity].Add (fieldId);
		}

		/// <summary>
		/// Gets the fields that must be persisted again for each <see cref="AbstractEntity"/>.
		/// </summary>
		/// <returns>The mapping between the <see cref="AbstractEntity"/> and their fields that must be saved again.</returns>
		internal Dictionary<AbstractEntity, HashSet<Druid>> GetFieldsToResave()
		{
			this.AssertDataContextIsNotDisposed ();

			return this.fieldsToResave;
		}

		/// <summary>
		/// Resets the list of fields that must be persisted again.
		/// </summary>
		internal void ClearFieldsToResave()
		{
			this.fieldsToResave.Clear ();
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> corresponding to a <see cref="EntityKey"/>. This
		/// method looks in the cache and then queries the database.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> defining the <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public AbstractEntity ResolveEntity(EntityKey entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			AbstractEntity entity = this.GetEntity (entityKey);

			if (entity == null)
			{
				return this.DataLoader.ResolveEntity (entityKey);
			}
			else
			{
				return entity;
			}
		}

		
		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> with a given
		/// <see cref="DbKey"/>. This method looks in the cache and then queries the database.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to get.</typeparam>
		/// <param name="rowKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/></returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public TEntity ResolveEntity<TEntity>(DbKey rowKey)
			where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();

			Druid entityId = EntityClassFactory.GetEntityId (typeof (TEntity));
			EntityKey entityKey = new EntityKey (entityId, rowKey);

			return (TEntity) this.ResolveEntity (entityKey);
		}

		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which match the
		/// given example.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to return.</typeparam>
		/// <param name="example">The <see cref="AbstractEntity"/> to use as an example.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given example.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public IEnumerable<TEntity> GetByExample<TEntity>(TEntity example)
			where TEntity : AbstractEntity
		{
			this.AssertDataContextIsNotDisposed ();

			return this.DataLoader.GetByExample<TEntity> (example);
		}

		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which correspond
		/// to the given <see cref="Request"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to return.</typeparam>
		/// <param name="request">The <see cref="Request"/> to execute against the database.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given <see cref="Request"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public IEnumerable<TEntity> GetByRequest<TEntity>(Request request)
			where TEntity : AbstractEntity
		{
			this.AssertDataContextIsNotDisposed ();

			return this.DataLoader.GetByRequest<TEntity> (request);
		}

		/// <summary>
		/// Persists all the changes that have been made to the <see cref="AbstractEntity"/> managed
		/// by this instance to the database.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void SaveChanges()
		{
			this.AssertDataContextIsNotDisposed ();

			IEnumerable<AbstractSynchronizationJob> jobs = this.DataSaver.SaveChanges ();
			
			DataContextPool.Instance.Synchronize (this, jobs);
		}

		/// <summary>
		/// Creates and persists the schema describing an <see cref="AbstractEntity"/> to the
		/// database. The schema for the <see cref="AbstractEntity"/> is created with all the
		/// dependencies.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> whose schema to create.</typeparam>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void CreateSchema<TEntity>()
			where TEntity : AbstractEntity, new()
		{
			this.AssertDataContextIsNotDisposed ();

			this.SchemaEngine.CreateSchema<TEntity> ();
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="DeleteSynchronizationJob"/>
		/// to the current instance if it is relevant.
		/// </summary>
		/// <param name="job">The <see cref="DeleteSynchronizationJob"/> whose modification to apply.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="job"/> is <c>null</c>.</exception>
		internal void Synchronize(DeleteSynchronizationJob job)
		{
			job.ThrowIfNull ("job");
			this.AssertDataContextIsNotDisposed ();

			if (this.Contains (job.EntityKey))
			{
				AbstractEntity entity = this.GetEntity (job.EntityKey);

				this.RemoveAllReferences (entity, EntityChangedEventSource.Synchronization);

				this.NotifyEntityChanged (entity, EntityChangedEventSource.Synchronization, EntityChangedEventType.Deleted);
			}
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="ValueSynchronizationJob"/>
		/// to the current instance if it is relevant.
		/// </summary>
		/// <param name="job">The <see cref="ValueSynchronizationJob"/> whose modification to apply.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="job"/> is <c>null</c>.</exception>
		internal void Synchronize(ValueSynchronizationJob job)
		{
			job.ThrowIfNull ("job");
			this.AssertDataContextIsNotDisposed ();

			if (this.Contains (job.EntityKey))
			{
				AbstractEntity entity = this.GetEntity (job.EntityKey);

				using (entity.UseSilentUpdates ())
				{
					using (entity.DefineOriginalValues ())
					{
						using (entity.DisableEvents ())
						{
							string fieldId = job.FieldId.ToResourceId ();
							object fieldValue = job.NewValue;

							entity.InternalSetValue (fieldId, fieldValue);
						}
					}
				}

				this.NotifyEntityChanged (entity, EntityChangedEventSource.Synchronization, EntityChangedEventType.Updated);
			}
		}
		

		/// <summary>
		/// Applies the modifications described by the given <see cref="ReferenceSynchronizationJob"/>
		/// to the current instance if it is relevant.
		/// </summary>
		/// <param name="job">The <see cref="ReferenceSynchronizationJob"/> whose modification to apply.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="job"/> is <c>null</c>.</exception>
		internal void Synchronize(ReferenceSynchronizationJob job)
		{
			job.ThrowIfNull ("job");
			this.AssertDataContextIsNotDisposed ();

			if (this.Contains (job.EntityKey))
			{
				AbstractEntity entity = this.GetEntity (job.EntityKey);

				using (entity.UseSilentUpdates ())
				{
					using (entity.DefineOriginalValues ())
					{
						using (entity.DisableEvents ())
						{
							string fieldId = job.FieldId.ToResourceId ();
							object fieldValue;

							if (job.NewTargetKey.HasValue)
							{
								fieldValue = new EntityKeyProxy (this, job.NewTargetKey.Value);
							}
							else
							{
								fieldValue = null;
							}

							entity.InternalSetValue (fieldId, fieldValue);
						}
					}
				}

				this.NotifyEntityChanged (entity, EntityChangedEventSource.Synchronization, EntityChangedEventType.Updated);
			}
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="CollectionSynchronizationJob"/>
		/// to the current instance if it is relevant.
		/// </summary>
		/// <param name="job">The <see cref="CollectionSynchronizationJob"/> whose modification to apply.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="job"/> is <c>null</c>.</exception>
		internal void Synchronize(CollectionSynchronizationJob job)
		{
			job.ThrowIfNull ("job");
			this.AssertDataContextIsNotDisposed ();

			if (this.Contains (job.EntityKey))
			{
				AbstractEntity entity = this.GetEntity (job.EntityKey);

				using (entity.UseSilentUpdates ())
				{
					using (entity.DefineOriginalValues ())
					{
						using (entity.DisableEvents ())
						{
							string fieldId = job.FieldId.ToResourceId ();

							IList collection = entity.InternalGetFieldCollection (fieldId);

							collection.Clear ();

							foreach (EntityKey entityKey in job.NewTargetKeys)
							{
								object proxy = new EntityKeyProxy (this, entityKey);

								collection.Add (proxy);
							}
						}
					}
				}

				this.NotifyEntityChanged (entity, EntityChangedEventSource.Synchronization, EntityChangedEventType.Updated);
			}
		}


		/// <summary>
		/// Removes all references to an <see cref="AbstractEntity"/> from all the <see cref="AbstractEntity"/>
		/// managed by this instance.
		/// </summary>
		/// <param name="target">The <see cref="AbstractEntity"/> whose references to remove.</param>
		/// <param name="eventSource">The source that must be used for the event to be fired.</param>
		public void RemoveAllReferences(AbstractEntity target, EntityChangedEventSource eventSource)
		{
			foreach (var item in this.GetPossibleReferencers (target))
			{
				AbstractEntity source = item.Key;
				Druid fieldId = item.Value;

				this.RemoveReference (source, fieldId, target, eventSource);
			}
		}


		/// <summary>
		/// Removes the relation with an <see cref="AbstractEntity"/> in another a field of another
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="source">The <see cref="AbstractEntity"/> whose reference or collection field to clean up.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field to clean up.</param>
		/// <param name="target">The <see cref="AbstractEntity"/> that must be removed from the field.</param>
		/// <param name="eventSource">The source that must be used for the event to be fired.</param>
		private void RemoveReference(AbstractEntity source, Druid fieldId, AbstractEntity target, EntityChangedEventSource eventSource)
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
				this.NotifyEntityChanged (source, eventSource, EntityChangedEventType.Updated);
			}
		}


		private IEnumerable<KeyValuePair<AbstractEntity, Druid>> GetPossibleReferencers(AbstractEntity target)
		{
			// This method will probably be too slow for a high number of managed entities, therefore
			// it would be nice to optimize it, either by keeping somewhere a list of entities targeting
			// other entities, or by looping only on a subset of entities, i.e only on the location
			// entities if we look for an entity which can be targeted only by a location.
			// Marc

			Druid leafTargetEntityId = target.GetEntityStructuredTypeId ();

			var fieldPaths = this.EntityContext.GetInheritedEntityIds (leafTargetEntityId)
				.SelectMany (id => this.DbInfrastructure.GetSourceReferences (id))
				.GroupBy (fp => fp.EntityId, fp => Druid.Parse (fp.Fields[0]))
				.ToDictionary (g => g.Key, g => g.ToList ());

			foreach (AbstractEntity source in this.GetEntities ())
			{
				Druid leafSourceEntityId = source.GetEntityStructuredTypeId ();
				var sourceInheritedIds = this.EntityContext.GetInheritedEntityIds (leafSourceEntityId);

				foreach (Druid localSourceId in sourceInheritedIds)
				{
					if (fieldPaths.ContainsKey (localSourceId))
					{
						foreach (Druid fieldId in fieldPaths[localSourceId])
						{
							yield return new KeyValuePair<AbstractEntity, Druid> (source, fieldId);
						}
					}
				}
			}
		}


		#region IDisposable Members


		/// <summary>
		/// Release all the resources kept by this instance.
		/// </summary>
		public void Dispose()
		{
			this.Dipose (true);
		}


		#endregion

		
		/// <summary>
		/// Release all the resources kept by this instance.
		/// </summary>
		/// <param name="disposing"></param>
		private void Dipose(bool disposing)
		{
			if (!this.IsDisposed && disposing)
			{
				this.EntityContext.EntityAttached -= this.HandleEntityCreated;
				this.EntityContext.EntityChanged -= this.HandleEntityChanged;
				this.EntityContext.PersistenceManagers.Remove (this);

				// TODO Remove this instance from the DataContextPool singleton if it is present?
				// Marc
			}

			this.IsDisposed = true;
		}


		/// <summary>
		/// Throws an <see cref="System.ObjectDisposedException"/> if this instance has been
		/// disposed.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		private void AssertDataContextIsNotDisposed()
		{
			if (this.IsDisposed)
			{
				throw new System.ObjectDisposedException ("DataContext #" + this.UniqueId);
			}
		}


		#region IEntityPersistenceManager Members


		public string GetPersistedId(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			string persistedId = null;
			
			if (this.IsPersistent (entity))
			{
				Druid leafEntityId = entity.GetEntityStructuredTypeId ();
				DbKey key = this.GetEntityKey (entity).Value.RowKey;

				if (key.Status == DbRowStatus.Live)
				{
					persistedId = string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}", leafEntityId, key.Id);
				}
				else
				{
					persistedId = string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}:{2}", leafEntityId, key.Id, (short) key.Status);
				}
			}

			return persistedId;
		}


		public AbstractEntity GetPeristedEntity(string id)
		{
			this.AssertDataContextIsNotDisposed ();

			AbstractEntity entity = null;

			if (!string.IsNullOrEmpty (id) && id.StartsWith ("db:"))
			{
				string[] args = id.Split (':');

				DbKey key = DbKey.Empty;
				Druid entityId = Druid.Empty;

				long  keyId;
				short keyStatus;

				switch (args.Length)
				{
					case 3:
						entityId = Druid.Parse (args[1]);

						if ((entityId.IsValid) &&
							(long.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyId)))
						{
							key = new DbKey (keyId);
						}
						break;

					case 4:
						entityId = Druid.Parse (args[1]);

						if ((entityId.IsValid) &&
							(long.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyId)) &&
							(short.TryParse (args[3], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyStatus)))
						{
							key = new DbKey (keyId, DbKey.ConvertFromIntStatus (keyStatus));
						}
						break;
				}

				if (!key.IsEmpty && !entityId.IsEmpty)
				{
					EntityKey entityKey = new EntityKey (entityId, key);

					entity = this.ResolveEntity (entityKey);
					;
				}
			}

			return entity;
		}


		#endregion


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> has been persisted to the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose persistence to check.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> has been persisted to the database, <c>false</c> if it hasn't.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public bool IsPersistent(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			EntityKey? key = this.GetEntityKey (entity);

			return key.HasValue && !key.Value.RowKey.IsEmpty;
		}


		/// <summary>
		/// Handles the creation of an <see cref="AbstractEntity"/> by the <see cref="EntityContext"/>
		/// associated with this instance.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The argument of the event.</param>
		private void HandleEntityCreated(object sender, EntityContextEventArgs e)
		{
			AbstractEntity entity = e.Entity;

			this.OnCreationPatchEntity (entity);
			this.OnCreationRegisterAsEmptyEntity (entity, e);
			this.OnCreationAddToCache (entity);

			try
			{
				this.OnCreationLoadSchema (entity);
			}
			catch
			{
				this.OnCreationRemoveFromCache (entity);
				throw;
			}
		}


		/// <summary>
		/// Patches the given <see cref="AbstractEntity"/> if it has to be patched by the
		/// <see cref="EntityNullReferenceVirtualizer"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to pacth.</param>
		private void OnCreationPatchEntity(AbstractEntity entity)
		{
			bool entityIsNotPatched = !EntityNullReferenceVirtualizer.IsPatchedEntity (entity);

			if (this.EnableNullVirtualization && entityIsNotPatched)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			}
		}


		/// <summary>
		/// Registers the given <see cref="AbstractEntity"/> as an empty one if it must be.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register as empty.</param>
		/// <param name="e">The arguments of the creation event.</param>
		private void OnCreationRegisterAsEmptyEntity(AbstractEntity entity, EntityContextEventArgs e)
		{
			bool entityWasNullVirtualized = EntityNullReferenceVirtualizer.IsEmptyEntityContext (e.OldContext);
			bool entityIsNullVirtualized = EntityNullReferenceVirtualizer.IsNullEntity (entity);
			bool entityIsStillUnchanged = EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (entity);

			if (entityWasNullVirtualized && (entityIsNullVirtualized || entityIsStillUnchanged))
			{
				this.RegisterEmptyEntity (entity);
			}
		}

		
		/// <summary>
		/// Adds the given <see cref="AbstractEntity"/> to the cache of this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to add to the cache.</param>
		private void OnCreationAddToCache(AbstractEntity entity)
		{
			this.entitiesCache.Add (entity);
		}


		/// <summary>
		/// Removes the given <see cref="AbstractEntity"/> to the cache of this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to remove from the cache.</param>
		private void OnCreationRemoveFromCache(AbstractEntity entity)
		{
			this.entitiesCache.Remove (entity);
		}


		/// <summary>
		/// Loads the schema of the given <see cref="AbstractEntity"/> in the <see cref="SchemaEngine"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose schema to load.</param>
		private void OnCreationLoadSchema(AbstractEntity entity)
		{
			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
			
			this.SchemaEngine.LoadSchema (leafEntityId);
		}


		/// <summary>
		/// Handles the events fired by the <see cref="EntityContext"/> associated with this
		/// instance and redirects them to the appropriate event handler.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="args">The data about the event.</param>
		private void HandleEntityChanged(object sender, Epsitec.Common.Support.EntityEngine.EntityFieldChangedEventArgs args)
		{
			this.NotifyEntityChanged (args.Entity, EntityChangedEventSource.External, EntityChangedEventType.Updated);
		}


		/// <summary>
		/// Fires an event through the appropriate event handler with the given parameters, that will
		/// notify the listeners about the changes.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the event.</param>
		/// <param name="source">The source of the event.</param>
		/// <param name="type">The type of the event.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void NotifyEntityChanged(AbstractEntity entity, EntityChangedEventSource source, EntityChangedEventType type)
		{
			this.AssertDataContextIsNotDisposed ();

			EventHandler<EntityChangedEventArgs> handler;

			lock (this.eventLock)
			{
				handler = this.entityChanged;
			}

			if (handler != null)
			{
				EntityChangedEventArgs eventArgs = new EntityChangedEventArgs (entity, type, source);

				handler (this, eventArgs);
			}
		}


		/// <summary>
		/// Copies an <see cref="AbstractEntity"/> present in a <see cref="DataContext"/> in another
		/// <see cref="DataContext"/>. What happens is exactly the same as if the
		/// <see cref="AbstractEntity"/> would have been loaded from the database, except that its
		/// data comes from another <see cref="DataContext"/>.
		/// </summary>
		/// <param name="sender">The <see cref="DataContext"/> that manages the <see cref="AbstractEntity"/> to copy.</param>
		/// <param name="entity">The <see cref="AbstractEntity"/> to copy.</param>
		/// <param name="receiver">The <see cref="DataContext"/> in which the <see cref="AbstractEntity"/> must be copied.</param>
		/// <returns>The copy of the <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sender"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="receiver"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not managed by <paramref name="sender"/>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is not persistent.</exception>
		/// <exception cref="System.ObjectDisposedException">If <paramref name="sender"/> has been disposed.</exception>
		/// <exception cref="System.ObjectDisposedException">If <paramref name="receiver"/> has been disposed.</exception>
		public static TEntity CopyEntity<TEntity>(DataContext sender, TEntity entity, DataContext receiver) where TEntity : AbstractEntity
		{
			sender.ThrowIfNull ("sender");
			receiver.ThrowIfNull ("receiver");

			sender.AssertDataContextIsNotDisposed ();
			receiver.AssertDataContextIsNotDisposed ();
			
			entity.ThrowIfNull ("entity");
			entity.ThrowIf (e => !sender.Contains (e), "entity is not managed by sender.");
			entity.ThrowIf (e => !sender.IsPersistent (e), "entity is not persistent.");
						
			EntityData data = sender.SerializationManager.Serialize (entity);
			
			return (TEntity) receiver.DataLoader.ResolveEntity (data);
		}
		

		/// <summary>
		/// The cache used to cache the <see cref="AbstractEntity"/> managed by this instance.
		/// </summary>
		private readonly EntityCache entitiesCache;


		/// <summary>
		/// The <see cref="AbstractEntity"/> which are registered as empty.
		/// </summary>
		private readonly HashSet<AbstractEntity> emptyEntities;


		/// <summary>
		/// The <see cref="AbstractEntity"/> which are to be deleted.
		/// </summary>
		private readonly HashSet<AbstractEntity> entitiesToDelete;


		/// <summary>
		/// The <see cref="AbstractEntity"/> which have been deleted.
		/// </summary>
		private readonly HashSet<AbstractEntity> entitiesDeleted;


		///// <summary>
		///// Stores the mapping between the <see cref="AbstractEntity"/> and their fields that must be
		///// saved again event if their value has not changed.
		///// </summary>
		private readonly Dictionary<AbstractEntity, HashSet<Druid>> fieldsToResave;


		/// <summary>
		/// The object used as a lock for the events.
		/// </summary>
		private readonly object eventLock;


		/// <summary>
		/// The event handler used to fire events related to the <see cref="AbstractEntity"/>
		/// managed by this instance.
		/// </summary>
		private EventHandler<EntityChangedEventArgs> entityChanged;


		/// <summary>
		/// The next unique id which will be used for the next instance of <c>DataContext</c>.
		/// </summary>
		private static int nextUniqueId;
	}
}