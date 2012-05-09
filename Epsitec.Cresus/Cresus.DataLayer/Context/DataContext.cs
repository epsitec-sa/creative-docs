//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Serialization;

using System.Collections;
using System.Collections.Generic;

using System.Linq;

using System.Threading;


namespace Epsitec.Cresus.DataLayer.Context
{
	/// <summary>
	/// The <c>DataContext</c> class is responsible of the mapping between the object model and the
	/// relational model of the <see cref="AbstractEntity"/>. It is therefore the designated entry
	/// point for everything which is related to them.
	/// </summary>
	/// 
	/// <remarks>
	/// This class is partially thread safe. Instances of this class which are not readonly are not
	/// thread-safe and should therefore only be accessed by a single thread at a time. Some Members
	/// are thread safe for readonly instances. They are roughly all the public members of this
	/// class that are available in readonly mode. The mechanism used is a reader writer lock per
	/// instance. Therefore, several threads can access members that do not modify the internal
	/// state at the same time. The access is exclusive for members that might modify the internal 
	/// state.
	/// 
	/// Members that have a shared access are the following:
	/// - DataContextPool
	/// - EnableNullVirtualization
	/// - Contains (the public overload)
	/// - ContainsChanges
	/// - GetEntities
	/// - GetNormalizedEntityKey
	/// - GetPersistedId
	/// - IsDeleted
	/// - IsForeignEntity
	/// - IsPersistent
	/// - IsReadOnly
	/// - IsRegisteredAsEmptyEntity
	/// - Name (only the getter, the setter isn't thread safe).
	/// - UniqueId
	/// 
	/// Members that have an exclusive access are the following:
	/// - GetByExample
	/// - GetByRequest
	/// - GetPersistedEntity
	/// - Reload	
	/// - ResolveEntity (all three overloads)
	///
	/// Members that concerns more than one instance of DataContext
	/// - CopyEntity (exclusive access to both instances of DataContext, the sender first and then
	///   the receiver)
	/// - GetLocalEntity (shared access to sender DataContext and then exclusive access to receiver
	///   DataContext)
	///   
	/// Event member
	/// - EntityChanged
	///   
	/// Two members might result in calls to more than one instance of DataContext : CopyEntity and
	/// GetLocalEntity. The instances of DataContext are accessed one after the other and thus both
	/// are never locked at the same time, which helps avoids deadlocks.
	/// 
	/// The mechanism used for the synchronization of the data between the instances of DataContext
	/// registered to a DataContextPool is not thread safe. Therefore all dataContexts from the same
	/// DataContextPool must be readonly if they are accessed by more than one thread. Otherwise,
	/// the consistency of the data within the instances might become corrupted while the data is
	/// synchronized.
	/// 
	/// The mechanism used by the proxies (CollectionFieldProxy, KeyedCollectionFieldProxy,
	/// ReferenceFieldProxy, KeyedReferenceFieldProxy and ValueFieldProxy) are also thread safe for
	/// the entities owned by a readonly instance of DataContext, so that when those proxies are
	/// resolved, the consistency of the data is ensured.
	/// 
	/// The event member EntityChanged is not fully thread safe. Registration and unregistration to
	/// it is thread safe and can be done by any thread at any time. However, it might happen that
	/// the event handler calls a method which has been unregistered from the event. Therefore, the
	/// methods that registers to this event must be able to handle the case when they are called
	/// after they have been unregistered if the DataContext and its entities are used by more than
	/// one thread. In addition, events might be fired while the instance of DataContext is in read
	/// or write lock. That means that dead locks are possible if the action to take when an event
	/// is fired on one DataContext might lock another one and vice versa.
	/// So unless you know exactly what you are doing and can garantee that what you do won't cause
	/// any deadlock, it is safer to avoid to use this event in a DataContext that is used by more
	/// than one thread.
	/// </remarks>
	[System.Diagnostics.DebuggerDisplay ("DataContext #{UniqueId}")]
	public sealed class DataContext : IEntityPersistenceManager, IIsDisposed, IReadOnly
	{
		/// <summary>
		/// Creates a new <c>DataContext</c>.
		/// </summary>
		/// <param name="infrastructure">The <see cref="DbInfrastructure"/> that will be used to talk to the database.</param>
		/// <param name="enableNullVirtualization">Tells whether to enable the virtualization of null <see cref="AbstractEntity"/> or not.</param>
		/// <param name="isReadOnly">Tells whether the new instance will contain only read only entities.</param>
		internal DataContext(DataInfrastructure infrastructure, bool enableNullVirtualization = false, bool isReadOnly = false)
		{
			this.uniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);
			this.DataInfrastructure = infrastructure;
			this.EntityContext = new EntityContext ();
			this.DataLoader = new DataLoader (this);
			this.DataSaver = new DataSaver (this);
			this.SerializationManager = new EntitySerializationManager (this);
			this.DataConverter = new DataConverter (this);

			this.IsReadOnly = isReadOnly;
			this.EnableNullVirtualization = enableNullVirtualization;

			this.entitiesCache = new EntityCache (this.TypeEngine);
			this.emptyEntities = new HashSet<AbstractEntity> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.entitiesDeleted = new HashSet<AbstractEntity> ();
			this.fieldsToResave = new Dictionary<AbstractEntity, HashSet<Druid>> ();

			this.dataContextLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion);
			this.lockTimeOut = System.TimeSpan.FromSeconds (15);

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


		#region IReadOnly Members


		/// <summary>
		/// Tells whether this instance contains only read only entities.
		/// </summary>
		public bool IsReadOnly
		{
			get;
			private set;
		}


		#endregion


		/// <summary>
		/// Gets the unique id of the current instance.
		/// </summary>
		public long								UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		/// <summary>
		/// Gets or sets the name of the <c>DataContext</c>. This is useful when
		/// debugging.
		/// </summary>
		/// <value>The name of the <c>DataContext</c>.</value>
		public string							Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the <see cref="EntityContext"/> associated with this instance.
		/// </summary>
		internal EntityContext					EntityContext
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DataInfrastructure"/> associated with this instance.
		/// </summary>
		internal DataInfrastructure				DataInfrastructure
		{
			get;
			private set;
		}

		public DataContextPool					DataContextPool
		{
			get
			{
				return this.DataInfrastructure.DataContextPool;
			}
		}

		private DbInfrastructure				DbInfrastructure
		{
			get
			{
				return this.DataInfrastructure.DbInfrastructure;
			}
		}

		private EntityTypeEngine				TypeEngine
		{
			get
			{
				return this.DataInfrastructure.EntityEngine.EntityTypeEngine;
			}
		}

		/// <summary>
		/// Gets the <see cref="DataLoader"/> associated with this instance.
		/// </summary>
		internal DataLoader						DataLoader
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DataSaver"/> associated with this instance.
		/// </summary>
		internal DataSaver						DataSaver
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="SerializationManager"/> associated with this instance.
		/// </summary>
		internal EntitySerializationManager		SerializationManager
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the <see cref="DataConverter"/> associated with this instance.
		/// </summary>
		internal DataConverter					DataConverter
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the value that tells if the <see cref="AbstractEntity"/> created by this
		/// <see cref="DataContext"/> can be null virtualized with the
		/// <see cref="EntityNullReferenceVirtualizer"/>.
		/// </summary>
		public bool								EnableNullVirtualization
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
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		public TEntity CreateEntity<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();
			this.ThrowIfReadOnly ();

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
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		public AbstractEntity CreateEntity(Druid entityId)
		{
			this.AssertDataContextIsNotDisposed ();
			this.ThrowIfReadOnly ();

			AbstractEntity entity = this.EntityContext.CreateEmptyEntity (entityId);

			this.NotifyEntityChanged (entity, EntityChangedEventSource.External, EntityChangedEventType.Created);

			entity.UpdateDataGeneration ();

			return entity;
		}


		/// <summary>
		/// Creates a new <see cref="AbstractEntity"/> of type <typeparamref name="TEntity"/> associated
		/// with this instance and registers it as an empty one.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to create.</typeparam>
		/// <returns>The new <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		public TEntity CreateEntityAndRegisterAsEmpty<TEntity>()
			where TEntity : AbstractEntity, new ()
		{
			this.AssertDataContextIsNotDisposed ();
			this.ThrowIfReadOnly ();

			TEntity entity = this.CreateEntity<TEntity> ();

			this.RegisterEmptyEntity (entity);

			return entity;
		}


		public T UnwrapNullEntity<T>(T entity) where T : AbstractEntity
		{
			return EntityNullReferenceVirtualizer.UnwrapNullEntity (entity);
		}


		public T WrapNullEntity<T>(T entity) where T : AbstractEntity, new ()
		{
			EntityContext realEntityContext = this.EntityContext;

			return EntityNullReferenceVirtualizer.WrapNullEntity (realEntityContext, entity);
		}


		public T CreateNullEntity<T>(bool freeze = false) where T : AbstractEntity, new ()
		{
			EntityContext realEntityContext = this.EntityContext;

			return EntityNullReferenceVirtualizer.CreateEmptyEntity<T> (realEntityContext, freeze);
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

			using (this.LockRead ())
			{
				return this.entitiesCache.ContainsEntity (entity);
			}
		}


		/// <summary>
		/// Gets the local copy of the specified entity. If it already belongs to this <c>DataContext</c>,
		/// the entity will be returned unchanged. Otherwise, it will be loaded into the context. If
		/// the entity was not persisted to the base, it cannot be made local and <c>null</c> will
		/// be returned instead.
		/// </summary>
		/// <typeparam name="T">The type of the entity.</typeparam>
		/// <param name="entity">The entity, which might be <c>null</c> or might not be persisted yet.</param>
		/// <returns>The entity, as loaded in the current context if possible; otherwise, <c>null</c>.</returns>
		public T GetLocalEntity<T>(T entity) where T : AbstractEntity
		{
			var entityKey = DataContextPool.GetEntityKey (entity);

			if (entityKey.HasValue)
			{
				using (this.LockWrite ())
				{
					T localEntity;

					if (this.Contains (entity))
					{
						localEntity = entity;
					}
					else
					{
						localEntity = (T) this.ResolveEntity (entityKey);
					}

					return localEntity;
				}
			}
			else if (EntityNullReferenceVirtualizer.IsNullEntity (entity))
			{
				return null;
			}
			else
			{
				System.Diagnostics.Debug.Assert (this.IsForeignEntity (entity) == false, "Unpersisted entities cannot be resolved to local entities");
				return null;
			}
		}


		/// <summary>
		/// Gets the normalized <see cref="EntityKey"/> associated with an <see cref="AbstractEntity"/>.
		/// Normalized means that the <see cref="Druid"/> of the type is the root type of the
		/// <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose <see cref="EntityKey"/> to get.</param>
		/// <returns>The <see cref="EntityKey"/> or <c>null</c> if there is none defined in this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public EntityKey? GetNormalizedEntityKey(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);

			using (this.LockRead ())
			{
				return this.entitiesCache.GetEntityKey (entity);
			}
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
		/// Associates a new log id to an entity type id.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> that defines the entity type id.</param>
		/// <param name="logId">The new log id.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void DefineEntityModificationEntryId(Druid entityTypeId, long logId)
		{
			this.AssertDataContextIsNotDisposed ();

			this.entitiesCache.DefineEntityModificationEntryId (entityTypeId, logId);
		}


		/// <summary>
		/// Associates a new log id to an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to which associate the log id.</param>
		/// <param name="logId">The new log id.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		internal void DefineEntityModificationEntryId(AbstractEntity entity, long logId)
		{
			this.AssertDataContextIsNotDisposed ();

			this.entitiesCache.DefineEntityModificationEntryId (entity, logId);
		}

		
		/// <summary>
		/// Gets the log id associated to an entity type id.
		/// </summary>
		/// <param name="entityTypeId">The <see cref="Druid"/> that defines the entity type id.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <returns>The log id.</returns>
		internal long? GetEntityModificationEntryId(Druid entityTypeId)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetLogId (entityTypeId);
		}


		/// <summary>
		/// Gets the log id associated to an <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose log id to get.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <returns>The log id.</returns>
		internal long? GetEntityModificationEntryId(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetLogId (entity);
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

			using (this.LockRead ())
			{
				bool containsDeletedEntities = this.entitiesToDelete.Any ();
				bool containsChangedEntities = this.GetEntitiesModified ().Any ();

				return containsDeletedEntities || containsChangedEntities;
			}
		}


		/// <summary>
		/// Registers an <see cref="AbstractEntity"/> as empty, which means that it will be as if it
		/// didn't exist when saving this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to register as empty.</param>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If <paramref name="entity"/> is in read only mode.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> has already been persisted.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public void RegisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);
			this.ThrowIfReadOnly ();

			entity.ThrowIfNull ("entity");
			entity.ThrowIf (e => this.IsPersistent (e), "entity");
			entity.AssertIsNotReadOnly ();

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
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If <paramref name="entity"/> is in read only mode.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> has already been persisted.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public void UnregisterEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);
			this.ThrowIfReadOnly ();

			entity.ThrowIfNull ("entity");
			entity.AssertIsNotReadOnly ();

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
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If <paramref name="entity"/> is in read only mode.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public void UpdateEmptyEntityStatus(AbstractEntity entity, bool isEmpty)
		{
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
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public bool IsRegisteredAsEmptyEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);

			using (this.LockRead ())
			{
				return this.emptyEntities.Contains (entity);
			}
		}


		/// <summary>
		/// Deletes an <see cref="AbstractEntity"/> from the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to delete.</param>
		/// <returns><c>true</c> if the entity has been added to the list of pending deletes; otherwise, <c>false</c>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		///   
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		///   
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		///   
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If <paramref name="entity"/> is in read only mode.</exception>
		///   
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		/// <remarks>
		/// Take care with this feature, because it is subtle. It will remove the <see cref="AbstractEntity"/>
		/// from the database during the call to <see cref="SaveChanges"/>. In the meantime, it will
		/// still be considered as a normal <see cref="AbstractEntity"/>. Furthermore, even after it
		/// will have been removed from the database, if you have a reference to it somewhere, you
		/// will be able to use it as a normal <see cref="AbstractEntity"/>, except that nothing will
		/// be persisted to the database.
		/// </remarks>
		public bool DeleteEntity(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.ThrowIfReadOnly ();
			
			entity.ThrowIfNull ("entity");
			entity.AssertIsNotReadOnly ();
			this.AssertEntityIsNotForeign (entity);

			if (this.entitiesDeleted.Contains (entity))
			{
				return false;
			}
			else
			{
				this.entitiesToDelete.Add (entity);
				return true;
			}
		}


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is deleted in this instance or if it will
		/// be deleted in the next call to <see cref="SaveChanges"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check if it has been deleted.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> has been deleted, <c>false</c> if it hasn't.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public bool IsDeleted(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);

			using (this.LockRead ())
			{
				return this.entitiesDeleted.Contains (entity) || this.entitiesToDelete.Contains (entity);
			}
		}
		

		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> managed by this instance.
		/// </summary>
		/// <returns>The collection of <see cref="AbstractEntity"/> managed by this instance.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public IList<AbstractEntity> GetEntities()
		{
			this.AssertDataContextIsNotDisposed ();

			using (this.LockRead ())
			{
				return this.entitiesCache.GetEntities ()
					.Except (this.entitiesDeleted)
					.ToList ();
			}
		}


		/// <summary>
		/// Gets the sequence of <see cref="Druid"/> that defines the types of the <see cref="AbstractEntity"/>
		/// managed by this instance.
		/// </summary>
		/// <returns>The sequence of <see cref="Druid"/> for the types.</returns>
		private IEnumerable<Druid> GetManagedEntityTypeIds()
		{
			return this.entitiesCache.GetEntityTypeIds ();
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
			this.entitiesToDelete.Remove (entity);
			this.emptyEntities.Remove (entity);
			this.fieldsToResave.Remove (entity);
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
			Druid entityTypeId = source.GetEntityStructuredTypeId ();
			StructuredTypeField field = this.TypeEngine.GetField (entityTypeId, fieldId);

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
		/// Gets the <see cref="AbstractEntity"/> corresponding to a <see cref="DbKey"/> and a type
		/// of <see cref="AbstractEntity"/>. This method looks in the cache and then queries the
		/// database.
		/// </summary>
		/// <param name="entityId">The <see cref="Druid"/> defining the type of the <see cref="AbstractEntity"/> to get.</param>
		/// <param name="rowKey">The <see cref="DbKey"/> defining which <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public AbstractEntity ResolveEntity(Druid entityId, DbKey rowKey)
		{
			this.AssertDataContextIsNotDisposed ();

			EntityKey entityKey = new EntityKey (entityId, rowKey);

			// Here we first look in the cache and if we find nothing (the call to this.GetEntity(...)
			// returns null) we look in the database. Thanks to the ?? operator, we can express that
			// in a very concise way.
			// Marc

			using (this.LockWrite ())
			{
				return this.GetEntity (entityKey)
					?? this.DataLoader.ResolveEntity (entityId, rowKey);
			}
		}


		/// <summary>
		/// Gets the <see cref="AbstractEntity"/> corresponding to an <see cref="EntityKey"/>. This
		/// method looks in the cache and then queries the database.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> defining which <see cref="AbstractEntity"/> to get.</param>
		/// <returns>The <see cref="AbstractEntity"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		public AbstractEntity ResolveEntity(EntityKey? entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			if ((entityKey.HasValue == false) ||
				(entityKey.Value.IsEmpty))
			{
				return null;
			}

			return this.ResolveEntity (entityKey.Value.EntityId, entityKey.Value.RowKey);
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

			Druid entityId = EntityInfo.GetTypeId (typeof (TEntity));

			return (TEntity) this.ResolveEntity (entityId, rowKey);
		}


		/// <summary>
		/// Refreshes the <see cref="AbstractEntity"/> that are managed by this instance and that
		/// have been modified or deleted in the database since they have been loaded in memory. This
		/// method will thus erase all modifications done in memory for the <see cref="AbstractEntity"/>
		/// that have been modified or deleted in the database. The other <see cref="AbstractEntity"/>
		/// won't be touched.
		/// </summary>
		/// <returns><c>true</c> if a modification occured, <c>false</c> if none occured.</returns>
		public bool Reload()
		{
			this.AssertDataContextIsNotDisposed ();

			bool changes;

			using (this.LockWrite ())
			{
				using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					bool deletions = this.DeleteDeletedEnties ();
					bool modifications = this.ReloadOutDatedEntities ();

					changes = deletions || modifications;
				}
			}

			return changes;
		}


		/// <summary>
		/// Deletes the <see cref="AbstractEntity"/> that are managed by this instance and that have
		/// been deleted in the database after they have been loaded in memory.
		/// </summary>
		/// <returns><c>true</c> if an <see cref="AbstractEntity"/> has been deleted, <c>false</c> if none where deleted.</returns>
		private bool DeleteDeletedEnties()
		{
			long id = System.Math.Max (this.entitiesCache.GetMinimumLogId () ?? 1, 1);

			var deletedEntities = new List<AbstractEntity>
			(
				from entry in this.DataInfrastructure.GetEntityDeletionEntriesNewerThan (new DbId (id))
				let entityTypeId = entry.EntityTypeId
				let rowKey = new DbKey (entry.EntityId)
				let entityKey = new EntityKey (entityTypeId, rowKey)
				let entity = this.GetEntity (entityKey)
				where entity != null
				select entity
			);

			this.CleanDeletedEntities (deletedEntities, EntityChangedEventSource.Reload, EntityChangedEventSource.Reload);

			return deletedEntities.Any ();
		}


		/// <summary>
		/// Reloads the data of the <see cref="AbstractEntity"/> that are managed by this instance
		/// and that have been modified in the database after they have been loaded in memory.
		/// </summary>
		/// <returns><c>true</c> if an <see cref="AbstractEntity"/> has been modified, <c>false</c> if none where modified.</returns>
		private bool ReloadOutDatedEntities()
		{
			bool modifications = false;
			
			EntityModificationEntry latestEntityModificationEntry = this.DataInfrastructure.GetLatestEntityModificationEntry ();

			foreach (Druid entityTypeId in this.GetManagedEntityTypeIds ().ToList ())
			{
				long currentEntryId = this.GetEntityModificationEntryId (entityTypeId).Value;
				long lastEntryId = latestEntityModificationEntry.EntryId.Value;

				bool modificationsForTypeId = this.DataLoader.ReloadOutDatedEntities (entityTypeId, currentEntryId, lastEntryId);

				modifications = modifications || modificationsForTypeId;
			}

			return modifications;
		}


		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which match the
		/// given example.
		/// </summary>
		/// <param name="example">The <see cref="AbstractEntity"/> to use as an example.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given example.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="example"/> is managed by another <see cref="DataContext"/>.</exception>
		/// <exception cref="System.InvalidOperationException">If <paramref name="example"/> contains an <see cref="AbstractEntity"/> managed by another <see cref="DataContext"/>.</exception>
		public IList<AbstractEntity> GetByExample(AbstractEntity example)
		{
			var request = Request.Create (example);

			return this.GetByRequest (request);
		}


		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which match the
		/// given example.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to return.</typeparam>
		/// <param name="example">The <see cref="AbstractEntity"/> to use as an example.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given example.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="example"/> is managed by another <see cref="DataContext"/>.</exception>
		/// <exception cref="System.InvalidOperationException">If <paramref name="example"/> contains an <see cref="AbstractEntity"/> managed by another <see cref="DataContext"/>.</exception>
		public IList<TEntity> GetByExample<TEntity>(TEntity example)
			where TEntity : AbstractEntity
		{
			var request = Request.Create (example);

			return this.GetByRequest<TEntity> (request);
		}


		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which correspond
		/// to the given <see cref="Request"/>.
		/// </summary>
		/// <param name="request">The <see cref="Request"/> to execute against the database.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given <see cref="Request"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="request"/> contains <see cref="AbstractEntity"/> is managed by another <see cref="DataContext"/>.</exception>
		/// <exception cref="System.InvalidOperationException">If <paramref name="request"/> contains an <see cref="AbstractEntity"/> managed by another <see cref="DataContext"/>.</exception>
		public IList<AbstractEntity> GetByRequest(Request request)
		{
			this.AssertDataContextIsNotDisposed ();

			using (this.LockWrite ())
			{
				return this.DataLoader.GetByRequest (request);
			}
		}


		/// <summary>
		/// Queries the database to retrieve all the <see cref="AbstractEntity"/> which correspond
		/// to the given <see cref="Request"/>.
		/// </summary>
		/// <typeparam name="TEntity">The type of the <see cref="AbstractEntity"/> to return.</typeparam>
		/// <param name="request">The <see cref="Request"/> to execute against the database.</param>
		/// <returns>The <see cref="AbstractEntity"/> which match the given <see cref="Request"/>.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="request"/> contains <see cref="AbstractEntity"/> is managed by another <see cref="DataContext"/>.</exception>
		/// <exception cref="System.InvalidOperationException">If <paramref name="request"/> contains an <see cref="AbstractEntity"/> managed by another <see cref="DataContext"/>.</exception>
		public IList<TEntity> GetByRequest<TEntity>(Request request)
			where TEntity : AbstractEntity
		{
			return this.GetByRequest (request)
				.Cast<TEntity> ()
				.ToList ();
		}


		/// <summary>
		/// Queries the database for the number of entities which match the given example.
		/// </summary>
		public int GetCount(AbstractEntity example)
		{
			var request = Request.Create (example);

			return this.GetCount (request);
		}


		/// <summary>
		/// Queries the database for the number of entities which match the given request.
		/// </summary>
		public int GetCount(Request request)
		{
			this.AssertDataContextIsNotDisposed ();

			using (this.LockRead ())
			{
				return this.DataLoader.GetCount (request);
			}
		}


		public RequestView GetRequestView(Request request)
		{
			return new RequestView (this, request);
		}


		/// <summary>
		/// Persists all the changes that have been made to the <see cref="AbstractEntity"/> managed
		/// by this instance to the database.
		/// </summary>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If this instance is in read only mode.</exception>
		public void SaveChanges()
		{
			this.AssertDataContextIsNotDisposed ();
			this.ThrowIfReadOnly ();

			IEnumerable<AbstractSynchronizationJob> jobs = this.DataSaver.SaveChanges ();
			
			this.DataContextPool.Synchronize (this, jobs);
		}


		/// <summary>
		/// Tells whether this instance manages an <see cref="AbstractEntity"/> corresponding to a
		/// given <see cref="EntityKey"/>.
		/// </summary>
		/// <param name="entityKey">The <see cref="EntityKey"/> to check if the corresponding <see cref="AbstractEntity"/> is managed by this instance.</param>
		/// <returns><c>true</c> if the corresponding <see cref="AbstractEntity"/> is managed by this instance, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		private bool Contains(EntityKey entityKey)
		{
			this.AssertDataContextIsNotDisposed ();

			return this.entitiesCache.GetEntity (entityKey) != null;
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
				var deletedEntity = this.GetEntity (job.EntityKey);
				var deletedEntities = new AbstractEntity[] { deletedEntity };

				// TODO Here we could have a significant performance benefit by executing all the
				// delete synchronization jobs at once.
				this.CleanDeletedEntities(deletedEntities, EntityChangedEventSource.Synchronization, EntityChangedEventSource.Synchronization);
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
				using (entity.DefineOriginalValues ())
				using (entity.DisableEvents ())
				using (entity.DisableReadOnlyChecks ())
				{
					string fieldId = job.FieldId.ToResourceId ();
					object fieldValue = job.NewValue;

					entity.InternalSetValue (fieldId, fieldValue);
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
				using (entity.DefineOriginalValues ())
				using (entity.DisableEvents ())
				using (entity.DisableReadOnlyChecks ())
				{
					object fieldValue;

					Druid fieldId = job.FieldId;
					EntityKey? targetKey = job.NewTargetKey;

					if (targetKey.HasValue)
					{
						fieldValue = new KeyedReferenceFieldProxy (this, entity, fieldId, targetKey.Value);
					}
					else
					{
						fieldValue = null;
					}

					entity.InternalSetValue (fieldId.ToResourceId (), fieldValue);
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
				using (entity.DefineOriginalValues ())
				using (entity.DisableEvents ())
				using (entity.DisableReadOnlyChecks ())
				{
					Druid fieldId = job.FieldId;

					object collection;

					if (job.NewTargetKeys.Any ())
					{
						collection = new KeyedCollectionFieldProxy (this, entity, fieldId, job.NewTargetKeys);
					}
					else
					{
						collection = UndefinedValue.Value;
					}

					entity.InternalSetValue (fieldId.ToResourceId (), collection);
				}

				this.NotifyEntityChanged (entity, EntityChangedEventSource.Synchronization, EntityChangedEventType.Updated);
			}
		}


		/// <summary>
		/// Cleans all the stuff associated with entities that have been deleted.
		/// </summary>
		/// <param name="deletedEntities">The <see cref="AbstractEntity"/> that have been deleted.</param>
		/// <param name="referenceRemovalSource">The source of the event that will be fired for any removal of reference towards a deleted entity.</param>
		/// <param name="deletionSource">The source of the event that will be fired for all deleted entities.</param>
		internal void CleanDeletedEntities(IEnumerable<AbstractEntity> deletedEntities, EntityChangedEventSource referenceRemovalSource, EntityChangedEventSource deletionSource)
		{
			this.RemoveAllReferences (deletedEntities, referenceRemovalSource);

			foreach (AbstractEntity entity in deletedEntities)
			{
				this.MarkAsDeleted (entity);
				this.NotifyEntityChanged (entity, deletionSource, EntityChangedEventType.Deleted);
			}
		}


		/// <summary>
		/// Removes all references to a sequence of <see cref="AbstractEntity"/> from all the
		/// <see cref="AbstractEntity"/> managed by this instance.
		/// </summary>
		/// <param name="targets">The sequence of <see cref="AbstractEntity"/> whose references to remove.</param>
		/// <param name="eventSource">The source that must be used for the event to be fired.</param>
		internal void RemoveAllReferences(IEnumerable<AbstractEntity> targets, EntityChangedEventSource eventSource)
		{
			var targetSet = new HashSet<AbstractEntity> (targets);

			var fields = targets
				.Select (e => e.GetEntityStructuredTypeId ())
				.Distinct ()
				.SelectMany (id => this.TypeEngine.GetReferencingFields (id).Select (i => System.Tuple.Create (i.Key, i.Value)))
				.Distinct ()
				.GroupBy (t => t.Item1)
				.ToDictionary (g => g.Key, g => g.SelectMany (t => t.Item2).ToList ());

			foreach (var source in this.GetEntities ())
			{
				var updated = false;

				Druid leafSourceEntityId = source.GetEntityStructuredTypeId ();
				var localSourceEntityTypes = this.TypeEngine.GetBaseTypes (leafSourceEntityId);

				using (source.UseSilentUpdates ())
				using (source.DisableEvents ())
				using (source.DisableReadOnlyChecks ())
				{
					foreach (var localSourceEntityType in localSourceEntityTypes)
					{
						List<StructuredTypeField> localFields;
						
						if (fields.TryGetValue (localSourceEntityType, out localFields))
						{
							foreach (var field in localFields)
							{
								var fieldUpdated = this.RemoveReference (source, field, targetSet, eventSource);

								updated = updated || fieldUpdated;
							}
						}
					}
				}

				if (updated)
				{
					this.NotifyEntityChanged (source, eventSource, EntityChangedEventType.Updated);
				}
			}
		}


		/// <summary>
		/// Removes any relation with a sequence of <see cref="AbstractEntity"/> that are referenced
		/// in a field of another <see cref="AbstractEntity"/>.
		/// </summary>
		/// <param name="source">The <see cref="AbstractEntity"/> whose reference or collection field to clean up.</param>
		/// <param name="field">The <see cref="StructuredTypeField"/> of the field to clean up.</param>
		/// <param name="targets">The sequence of <see cref="AbstractEntity"/> that must be removed from the field.</param>
		/// <param name="eventSource">The source that must be used for the event to be fired.</param>
		internal bool RemoveReference(AbstractEntity source, StructuredTypeField field, HashSet<AbstractEntity> targets, EntityChangedEventSource eventSource)
		{
			bool updated = false;

			switch (field.Relation)
			{
				case FieldRelation.Reference:

					var target = source.InternalGetValue (field.Id) as AbstractEntity;

					if (target != null && targets.Contains (target))
					{
						source.InternalSetValue (field.Id, null);

						updated = true;
					}

					break;

				case FieldRelation.Collection:

					IList collection = source.InternalGetFieldCollection (field.Id);

					for (int i = 0; i < collection.Count; i++)
					{
						var item = collection[i] as AbstractEntity;

						if (item != null && targets.Contains (item))
						{
							collection.RemoveAt (i);

							i--;

							updated = true;
						}
					}

					break;

				default:

					throw new System.InvalidOperationException ();
			}

			return updated;
		}


		private IEnumerable<KeyValuePair<AbstractEntity, Druid>> GetPossibleReferencers(AbstractEntity target)
		{
			// TODO This method will probably be too slow for a high number of managed entities,
			// therefore it would be nice to optimize it, either by keeping somewhere a list of
			// entities targeting other entities, or by looping only on a subset of entities, i.e 
			// only on the location entities if we look for an entity which can be targeted only by
			// a location.
			// Marc

			Druid leafTargetEntityId = target.GetEntityStructuredTypeId ();

			var fields = this.TypeEngine.GetReferencingFields (leafTargetEntityId);

			IEnumerable<AbstractEntity> entities = this.GetEntities ();

			foreach (AbstractEntity source in entities)
			{
				Druid leafSourceEntityId = source.GetEntityStructuredTypeId ();
				var localSourceEntityTypes = this.TypeEngine.GetBaseTypes (leafSourceEntityId);

				foreach (StructuredType localSourceEntityType in localSourceEntityTypes)
				{
					if (fields.ContainsKey (localSourceEntityType))
					{
						foreach (StructuredTypeField field in fields[localSourceEntityType])
						{
							yield return new KeyValuePair<AbstractEntity, Druid> (source, field.CaptionId);
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


		#region IIsDisposed Members


		/// <summary>
		/// Tells whether this instance was disposed or not.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
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
				this.dataContextLock.Dispose ();
			}

			this.isDisposed = true;
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


		/// <summary>
		/// Throws an <see cref="System.ArgumentException"/> if the given <see cref="AbstractEntity"/>
		/// is managed by another <see cref="DataContext"/>.
		/// </summary>
		/// <param name="entity">The entity whose presence in this instance to check.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		private void AssertEntityIsNotForeign(AbstractEntity entity)
		{
			if (this.IsForeignEntity (entity))
			{
				throw new System.ArgumentException ("entity is managed by another DataContext.");
			}
		}		
		

		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> is managed by another
		/// <see cref="DataContext"/>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to check if it is foreign.</param>
		/// <returns>
		/// <c>true</c> if the <see cref="AbstractEntity"/> is managed by another
		/// <see cref="DataContext"/>, <c>false</c> if it is not managed by another
		/// <see cref="DataContext"/>.
		/// </returns>
		internal bool IsForeignEntity(AbstractEntity entity)
		{
			// TODO Remove 'entity != null' when adding argument checks to this class. Now it is a
			// trick that avoid to break everything when these methods are called with null.
			// Marc

			return entity != null
				&& entity.DataContextId.HasValue
				&& entity.DataContextId.Value != this.UniqueId;
		}


		#region IEntityPersistenceManager Members


		public string GetPersistedId(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);

			using (this.LockRead ())
			{
				string persistedId = null;

				if (this.IsPersistent (entity))
				{
					Druid leafEntityId = entity.GetEntityStructuredTypeId ();
					DbKey key = this.GetNormalizedEntityKey (entity).Value.RowKey;

					persistedId = string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}", leafEntityId, key.Id);
				}

				return persistedId;
			}
		}


		public AbstractEntity GetPersistedEntity(string id)
		{
			this.AssertDataContextIsNotDisposed ();

			using (this.LockWrite ())
			{
				AbstractEntity entity = null;

				if (!string.IsNullOrEmpty (id) && id.StartsWith ("db:"))
				{
					string[] args = id.Split (':');

					DbKey key = DbKey.Empty;
					Druid entityId = Druid.Empty;

					entityId = Druid.Parse (args[1]);

					long  keyId;

					if ((entityId.IsValid) &&
					(long.TryParse (args[2], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out keyId)))
					{
						key = new DbKey (keyId);
					}

					if (!key.IsEmpty && !entityId.IsEmpty)
					{
						entity = this.ResolveEntity (entityId, key);
					}
				}

				return entity;
			}
		}


		#endregion


		/// <summary>
		/// Tells whether an <see cref="AbstractEntity"/> has been persisted to the database.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> whose persistence to check.</param>
		/// <returns><c>true</c> if the <see cref="AbstractEntity"/> has been persisted to the database, <c>false</c> if it hasn't.</returns>
		/// <exception cref="System.ObjectDisposedException">If this instance has been disposed.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entity"/> is managed by another <see cref="DataContext"/>.</exception>
		public bool IsPersistent(AbstractEntity entity)
		{
			this.AssertDataContextIsNotDisposed ();
			this.AssertEntityIsNotForeign (entity);

			EntityKey? key = this.GetNormalizedEntityKey (entity);

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

			this.OnCreationAssignToDataContext (entity);
			this.OnCreationPatchEntity (entity);
			this.OnCreationRegisterAsEmptyEntity (entity, e);
			this.OnCreationFreezeEntity (entity);
			this.OnCreationAddToCache (entity);
		}


		/// <summary>
		/// Sets the <see cref="AbstractEntity.DataContextId"/> property so that it is assigned to
		/// this instance.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to assign to this instance.</param>
		private void OnCreationAssignToDataContext(AbstractEntity entity)
		{
			if (entity.DataContextId.HasValue)
			{
				throw new System.InvalidOperationException (string.Format ("The entity of type {0} is already assigned to a DataContext #{1}", entity.GetType ().FullName, entity.DataContextId.Value));
			}

			entity.DataContextId = this.UniqueId;
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
		/// Freezes the given <see cref="AbstractEntity"/> if this instance is in read only mode.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> to freeze.</param>
		private void OnCreationFreezeEntity(AbstractEntity entity)
		{
			if (this.IsReadOnly)
			{
				entity.Freeze ();

				entity.DefineLockFunctions (this.LockRead, this.LockWrite);
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

			var eventArgs = new EntityChangedEventArgs (entity, type, source);

			this.EntityChanged.Raise (this, eventArgs);
		}


		internal System.IDisposable LockRead()
		{
			return this.IsReadOnly
				? TimedReaderWriterLock.LockRead (this.dataContextLock, this.lockTimeOut)
				: null;
		}

		internal System.IDisposable LockWrite()
		{
			return this.IsReadOnly
				? TimedReaderWriterLock.LockWrite (this.dataContextLock, this.lockTimeOut)
				: null;
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
		/// <exception cref="Epsitec.Common.Types.Exceptions.ReadOnlyException">If <paramref name="sender"/> is not in read only mode but <paramref name="receiver"/> is.</exception>
		/// <exception cref="System.ObjectDisposedException">If <paramref name="sender"/> has been disposed.</exception>
		/// <exception cref="System.ObjectDisposedException">If <paramref name="receiver"/> has been disposed.</exception>
		public static TEntity CopyEntity<TEntity>(DataContext sender, TEntity entity, DataContext receiver) where TEntity : AbstractEntity
		{
			sender.ThrowIfNull ("sender");
			receiver.ThrowIfNull ("receiver");

			sender.AssertDataContextIsNotDisposed ();
			receiver.AssertDataContextIsNotDisposed ();

			if (!sender.IsReadOnly)
			{
				receiver.ThrowIfReadOnly ();
			}

			if (sender == receiver)
			{
				return entity;
			}

			EntityKey entityKey;
			Druid entityTypeId;
			long senderEntityLogId;
			EntityData data;

			using (sender.LockWrite ())
			{
				entity.ThrowIfNull ("entity");
				entity.ThrowIf (e => !sender.Contains (e), "entity is not managed by sender.");
				entity.ThrowIf (e => !sender.IsPersistent (e), "entity is not persistent.");

				entityKey = sender.GetNormalizedEntityKey (entity).Value;
				entityTypeId = entity.GetEntityStructuredTypeId ();

				senderEntityLogId = sender.entitiesCache.GetLogId (entity).Value;
				data = sender.SerializationManager.Serialize (entity, senderEntityLogId);
			}

			using (receiver.LockWrite ())
			{
				TEntity receiverEntity = (TEntity) receiver.GetEntity (entityKey);

				if (receiverEntity != null)
				{
					return receiverEntity;
				}

				TEntity copiedEntity = (TEntity) receiver.DataLoader.DeserializeEntityData (data);

				receiver.entitiesCache.DefineEntityModificationEntryId (copiedEntity, senderEntityLogId);

				long? receiverLogId = receiver.entitiesCache.GetLogId (entityTypeId);

				if (!receiverLogId.HasValue || receiverLogId.Value > senderEntityLogId)
				{
					receiver.entitiesCache.DefineEntityModificationEntryId (entityTypeId, senderEntityLogId);
				}

				return copiedEntity;
			}
		}


		/// <summary>
		/// The event is fired when an <see cref="AbstractEntity"/> managed by this instance is
		/// created, updated or deleted.
		/// </summary>
		public event EventHandler<EntityChangedEventArgs> EntityChanged;


		private static long							nextUniqueId;					//	next unique ID
		private readonly long						uniqueId;						//	unique ID associated with this instance


		private readonly ReaderWriterLockSlim		dataContextLock;				//  lock used to access thread safe methods in the DataContext
		private readonly System.TimeSpan			lockTimeOut;					//  maximum time that we can wait for locks.


		private readonly EntityCache				entitiesCache;					//	entities managed by this instance
		private readonly HashSet<AbstractEntity>	emptyEntities;					//	entities registered as empty
		private readonly HashSet<AbstractEntity>	entitiesToDelete;				//	entities to be deleted
		private readonly HashSet<AbstractEntity>	entitiesDeleted;				//	entities which have been deleted
		private readonly Dictionary<AbstractEntity, HashSet<Druid>> fieldsToResave;	//	mapping between entities and their fields that must be re-saved, even if their value has not changed


		private bool								isDisposed;
	}
}
