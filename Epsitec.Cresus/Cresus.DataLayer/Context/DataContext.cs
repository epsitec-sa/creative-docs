//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Schema;

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
		public DataContext(DbInfrastructure infrastructure)
		{
			this.UniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);
			this.IsDisposed = false;
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure);
			this.EntityContext = new EntityContext ();
			this.DataLoader = new DataLoader (this);
			this.DataSaver = new DataSaver (this);

			this.EnableNullVirtualization = false;

			this.entitiesCache = new EntityCache ();
			this.emptyEntities = new HashSet<AbstractEntity> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.entitiesDeleted = new HashSet<AbstractEntity> ();

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
		/// The event that is fired when an <see cref="AbstractEntity"/> managed by this instance is
		/// created, updated or deleted.
		/// </summary>
		public event EventHandler<EntityEventArgs> EntityEvent
		{
			add
			{
				lock (this.eventLock)
				{
					this.entityEvent += value;
				}
			}
			remove
			{
				lock (this.eventLock)
				{
					this.entityEvent -= value;
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
		/// Gets or sets the value that tells if the <see cref="AbstractEntity"/> created by this
		/// <see cref="DataContext"/> can be null virtualized with the
		/// <see cref="EntityNullReferenceVirtualizer"/>.
		/// </summary>
		public bool EnableNullVirtualization
		{
			get;
			set;
		}


		/// <summary>
		/// Creates a new <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> associated
		/// with this instance.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to create.</typeparam>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public TEntity CreateEntity<TEntity>() where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();

			TEntity entity = this.EntityContext.CreateEmptyEntity<TEntity> ();

			this.FireEntityEvent (entity, EntityEventSource.External, EntityEventType.Created);
			
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
		public TEntity CreateEmptyEntity<TEntity>() where TEntity : AbstractEntity, new ()
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
		/// <remarks>
		/// Be careful with this feature, because it might create inconsistencies. If an
		/// <see cref="AbstractEntity"/> is targeted by another and is registered as empty, the relation
		/// between them won't be persisted to the database, then the first <see cref="AbstractEntity"/>
		/// is unregistered as empty, so it will be persisted to the database, but no the relation because
		/// the second <see cref="AbstractEntity"/> wouldn't have changed.
		/// </remarks>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register as empty.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void RegisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			if (this.emptyEntities.Add (entity))
			{
				System.Diagnostics.Debug.WriteLine ("Empty entity registered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());

				entity.UpdateDataGenerationAndNotifyEntityContextAboutChange ();
			}
		}


		/// <summary>
		/// Unregisters an <see cref="AbstractEntity"/> as empty, which means that if it was registered
		/// as empty, it will now be considered as a normal <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to unregister as empty.</param>
		/// <remarks>See the remarks in <see cref="RegisterEmptyEntity"/>.</remarks>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void UnregisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			if (this.emptyEntities.Remove (entity))
			{
				System.Diagnostics.Debug.WriteLine ("Empty entity unregistered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());
				
				entity.UpdateDataGenerationAndNotifyEntityContextAboutChange ();
			}
		}


		/// <summary>
		/// Sets the registered empty status of an <see cref="AbstractEntity"/> to a given value.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register or unregister as empty.</param>
		/// <param name="isEmpty">A <see cref="bool"/> indicating whether to register or unregister is at empty.</param>
		/// <remarks>See the remarks in <see cref="RegisterEmptyEntity"/>.</remarks>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void UpdateEmptyEntityStatus(AbstractEntity entity, bool isEmpty)
		{
			this.AssertDataContextIsNotDisposed ();

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
		/// Gets the <see cref="AbstractEntity"/> managed by this instance.
		/// </summary>
		/// <returns>The <see cref="AbstractEntity"/> managed by this instance.</returns>
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

			this.FireEntityEvent (entity, EntityEventSource.External, EntityEventType.Deleted);
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
		/// Gets the <see cref="AbstractEntity"/> corresponding to a <see cref="EntityKey"/>. This
		/// method looks in the cache and then queries the database.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> defining the <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public AbstractEntity ResolveEntity(EntityKey entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			AbstractEntity entity = this.entitiesCache.GetEntity (entityKey);

			if (entity == null)
			{
				entity = this.DataLoader.ResolveEntity (entityKey);
			}

			return entity;
		}

		
		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> with a given
		/// <see cref="DbKey"/>. This method looks in the cache and then queries the database.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to get.</typeparam>
		/// <param name="rowKey">The <see cref="DbKey"/> of the <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/></returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public TEntity ResolveEntity<TEntity>(DbKey rowKey) where TEntity : AbstractEntity, new ()
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
		public IEnumerable<TEntity> GetByExample<TEntity>(TEntity example) where TEntity : AbstractEntity
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
		public IEnumerable<TEntity> GetByRequest<TEntity>(Request request) where TEntity : AbstractEntity
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

			IEnumerable<AbstractSynchronisationJob> jobs = this.DataSaver.SaveChanges ();
			
			DataContextPool.Instance.Synchronize (this, jobs);
		}


		/// <summary>
		/// Creates and persists the schema describing an <see cref="AbstractEntity"/> to the
		/// database. The schema for the <see cref="AbstractEntity"/> is created with all the
		/// dependencies.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> whose schema to create.</typeparam>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public void CreateSchema<TEntity>() where TEntity : AbstractEntity, new()
		{
			this.AssertDataContextIsNotDisposed ();

			this.SchemaEngine.CreateSchema<TEntity> ();
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="AbstractSynchronisationJob"/>
		/// to the current instance if it is relevant.
		/// </summary>
		/// <param name="job">The <see cref="AbstractSynchronisationJob"/> whose modification to apply.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void Synchronize(AbstractSynchronisationJob job)
		{
			this.AssertDataContextIsNotDisposed ();

			if (this.Contains (job.EntityKey))
			{
				// Here is a little trick : job is cast to dynamic, which allows us to simulate a
				// double dispatch call to this.Synchronize(...). That is cool, because that means
				// that we have 4 overloads for this.Synchronize(...) and we are able to call the
				// appropriate one without knowing the concrete type of job at compile time or by
				// testing it with the 'is' keyword at runtime.
				// Marc

				this.Synchronize ((dynamic) job);
			}
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="DeleteSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="DeleteSynchronizationJob"/> whose modification to apply.</param>
		private void Synchronize(DeleteSynchronizationJob job)
		{
			AbstractEntity entity = this.GetEntity (job.EntityKey);

			this.DataSaver.DeleteEntityTargetRelationsInMemory (entity, EntityEventSource.Synchronization);

			this.FireEntityEvent (entity, EntityEventSource.Synchronization, EntityEventType.Deleted);
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="ValueSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ValueSynchronizationJob"/> whose modification to apply.</param>
		private void Synchronize(ValueSynchronizationJob job)
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

			this.FireEntityEvent (entity, EntityEventSource.Synchronization, EntityEventType.Updated);
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="ReferenceSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ReferenceSynchronizationJob"/> whose modification to apply.</param>
		private void Synchronize(ReferenceSynchronizationJob job)
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

						if (job.NewValue.HasValue)
						{
							fieldValue = new EntityKeyProxy (this, job.NewValue.Value);
						}
						else
						{
							fieldValue = null;
						}

						entity.InternalSetValue (fieldId, fieldValue);
					}
				}
			}

			this.FireEntityEvent (entity, EntityEventSource.Synchronization, EntityEventType.Updated);
		}


		/// <summary>
		/// Applies the modifications described by the given <see cref="CollectionSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="CollectionSynchronizationJob"/> whose modification to apply.</param>
		private void Synchronize(CollectionSynchronizationJob job)
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

						foreach (EntityKey entityKey in job.NewValues)
						{
							object proxy = new EntityKeyProxy (this, entityKey);

							collection.Add (proxy);
						}
					}
				}
			}

			this.FireEntityEvent (entity, EntityEventSource.Synchronization, EntityEventType.Updated);
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
		private void HandleEntityChanged(object sender, EntityChangedEventArgs args)
		{
			this.FireEntityEvent (args.Entity, EntityEventSource.External, EntityEventType.Updated);
		}


		/// <summary>
		/// Fires an event through the appropriate event handler with the given parameters, that will
		/// notify the listeners about the changes.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the event.</param>
		/// <param name="source">The source of the event.</param>
		/// <param name="type">The type of the event.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void FireEntityEvent(AbstractEntity entity, EntityEventSource source, EntityEventType type)
		{
			this.AssertDataContextIsNotDisposed ();

			EventHandler<EntityEventArgs> handler;

			lock (this.eventLock)
			{
				handler = this.entityEvent;
			}

			if (handler != null)
			{
				EntityEventArgs eventArgs = new EntityEventArgs (entity, type, source);

				handler (this, eventArgs);
			}
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


		/// <summary>
		/// The object used as a lock for the events.
		/// </summary>
		private readonly object eventLock;


		/// <summary>
		/// The event handler used to fire events related to the <see cref="AbstractEntity"/>
		/// managed by this instance.
		/// </summary>
		private EventHandler<EntityEventArgs> entityEvent;


		/// <summary>
		/// The next unique id which will be used for the next instance of <c>DataContext</c>.
		/// </summary>
		private static int nextUniqueId;


	}


}