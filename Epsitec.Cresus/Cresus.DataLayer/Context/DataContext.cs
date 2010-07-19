//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	[System.Diagnostics.DebuggerDisplay ("DataContext #{UniqueId}")]
	public sealed class DataContext : System.IDisposable, IEntityPersistenceManager
	{


		public DataContext(DbInfrastructure infrastructure)
		{
			this.UniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure);
			this.EntityContext = EntityContext.Current;
			this.DataLoader = new DataLoader (this);
			this.DataSaver = new DataSaver (this);

			this.EnableNullVirtualization = false;

			this.entitiesCache = new EntityCache ();
			this.emptyEntities = new HashSet<AbstractEntity> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.entitiesDeleted = new HashSet<AbstractEntity> ();
			
			this.EntityContext.EntityAttached += this.HandleEntityCreated;
			this.EntityContext.PersistenceManagers.Add (this);
		}


		public int UniqueId
		{
			get;
			private set;
		}


		// TODO Make this field internal and apply the changes in core data.
		// Marc

		public EntityContext EntityContext
		{
			get;
			private set;
		}


		internal DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		internal SchemaEngine SchemaEngine
		{
			get;
			private set;
		}


		internal DataLoader DataLoader
		{
			get;
			private set;
		}


		internal DataSaver DataSaver
		{
			get;
			private set;
		}


		public bool EnableNullVirtualization
		{
			get;
			set;
		}


		public TEntity CreateEntity<TEntity>() where TEntity : AbstractEntity, new ()
		{
			return this.EntityContext.CreateEmptyEntity<TEntity> ();
		}


		internal AbstractEntity CreateEntity(Druid entityId)
		{
			return this.EntityContext.CreateEmptyEntity (entityId);
		}


		public TEntity CreateEmptyEntity<TEntity>() where TEntity : AbstractEntity, new ()
		{
			TEntity entity = this.CreateEntity<TEntity> ();

			this.RegisterEmptyEntity (entity);

			return entity;
		}


		public bool Contains(AbstractEntity entity)
		{
			return this.entitiesCache.ContainsEntity (entity);
		}


		public EntityKey? GetEntityKey(AbstractEntity entity)
		{
			return this.entitiesCache.GetEntityKey (entity);
		}


		internal AbstractEntity GetEntity(EntityKey entityKey)
		{
			return this.entitiesCache.GetEntity (entityKey);
		}


		internal void DefineRowKey(AbstractEntity entity, DbKey key)
		{
			this.entitiesCache.DefineRowKey (entity, key);
		}


		public bool ContainsChanges()
		{
			bool containsDeletedEntities = this.entitiesToDelete.Any();
			bool containsChangedEntities = this.GetEntitiesModified ().Any ();

			return containsDeletedEntities || containsChangedEntities;
		}


		public void RegisterEmptyEntity(AbstractEntity entity)
		{
			System.Diagnostics.Debug.WriteLine ("Empty entity registered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());

			if (this.emptyEntities.Add (entity))
			{
				entity.UpdateDataGenerationAndNotifyEntityContextAboutChange ();
			}
		}


		public void UnregisterEmptyEntity(AbstractEntity entity)
		{
			if (this.emptyEntities.Remove (entity))
			{
				System.Diagnostics.Debug.WriteLine ("Empty entity unregistered : " + entity.DebuggerDisplayValue + " #" + entity.GetEntitySerialId ());
				entity.UpdateDataGenerationAndNotifyEntityContextAboutChange ();
			}
		}


		public bool IsRegisteredAsEmptyEntity(AbstractEntity entity)
		{
			return this.emptyEntities.Contains (entity);
		}


		public void UpdateEmptyEntityStatus(AbstractEntity entity, bool isEmpty)
		{
			if (isEmpty)
			{
				this.RegisterEmptyEntity (entity);
			}
			else
			{
				this.UnregisterEmptyEntity (entity);
			}
		}


		// TODO Remove the two methods below which are kind of strange and replace the call by calls
		// to the method just above.
		// Marc


		public bool UpdateEmptyEntityStatus(AbstractEntity entity, params bool[] emptyPredicateResults)
		{
			System.Diagnostics.Debug.Assert (emptyPredicateResults.Length > 0);

			bool result = emptyPredicateResults.All (x => x);

			this.UpdateEmptyEntityStatus (entity, result);

			return result;
		}


		public bool UpdateEmptyEntityStatus<T>(T entity, System.Predicate<T> emptyPredicate) where T : AbstractEntity
		{
			bool result = emptyPredicate (entity);
			
			this.UpdateEmptyEntityStatus (entity, result);

			return result;
		}


		public void DeleteEntity(AbstractEntity entity)
		{
			if (!this.entitiesDeleted.Contains (entity))
			{
				this.entitiesToDelete.Add (entity);
			}
		}


		public bool IsDeleted(AbstractEntity entity)
		{
			return this.entitiesDeleted.Contains (entity) || this.entitiesToDelete.Contains (entity);
		}


		internal IEnumerable<AbstractEntity> GetEntities()
		{
			return this.entitiesCache.GetEntities ();
		}


		internal IEnumerable<AbstractEntity> GetEntitiesModified()
		{
			return from AbstractEntity entity in this.GetEntities ()
				   where entity.GetEntityDataGeneration () >= this.EntityContext.DataGeneration
				   select entity;
		}


		internal IEnumerable<AbstractEntity> GetEntitiesDeleted()
		{
			return this.entitiesDeleted;
		}


		internal IEnumerable<AbstractEntity> GetEntitiesToDelete()
		{
			return this.entitiesToDelete;
		}


		internal void MarkAsDeleted(AbstractEntity entity)
		{
			this.entitiesDeleted.Add (entity);
			this.entitiesCache.Remove (entity);
		}


		internal void ClearEntitiesToDelete()
		{
			this.entitiesToDelete.Clear ();
		}


		public IEnumerable<T> GetByExample<T>(T example) where T : AbstractEntity
		{
			return this.DataLoader.GetByExample<T> (example);
		}


		public IEnumerable<T> GetByRequest<T>(Request request) where T : AbstractEntity
		{
			return this.DataLoader.GetByRequest<T> (request);
		}


		public IEnumerable<System.Tuple<AbstractEntity, EntityFieldPath>> GetReferencers(AbstractEntity target, ResolutionMode resolutionMode = ResolutionMode.Database)
		{
			return this.DataLoader.GetReferencers (target, resolutionMode);
		}


		public AbstractEntity ResolveEntity(EntityKey entityKey)
		{
			AbstractEntity entity = this.entitiesCache.GetEntity (entityKey);

			if (entity == null)
			{
				entity = this.DataLoader.ResolveEntity (entityKey.RowKey, entityKey.EntityId);
			}

			return entity;
		}


		public TEntity ResolveEntity<TEntity>(DbKey rowKey) where TEntity : AbstractEntity, new ()
		{
			Druid entityId = EntityClassFactory.GetEntityId (typeof (TEntity));
			EntityKey entityKey = new EntityKey (entityId, rowKey);

			return (TEntity) this.ResolveEntity (entityKey);
		}


		public void SaveChanges()
		{
			this.DataSaver.SaveChanges ();
		}


		internal bool CheckIfEntityCanBeSaved(AbstractEntity entity)
		{
			bool canBeSaved = entity != null
				&& !this.IsDeleted (entity)
				&& !this.IsRegisteredAsEmptyEntity (entity)
				&& !EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (entity)
				&& !EntityNullReferenceVirtualizer.IsNullEntity (entity);

			return canBeSaved;
		}


		public void CreateSchema<TEntity>() where TEntity : AbstractEntity, new()
		{
			this.SchemaEngine.CreateSchema<TEntity> ();
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.Dipose (true);
		}


		#endregion

		
		private void Dipose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;

				this.EntityContext.EntityAttached -= this.HandleEntityCreated;
				this.EntityContext.PersistenceManagers.Remove (this);
			}
		}


		#region IEntityPersistenceManager Members


		public string GetPersistedId(AbstractEntity entity)
		{
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


		public bool IsPersistent(AbstractEntity entity)
		{
			EntityKey? key = this.GetEntityKey (entity);

			return key != null && !key.Value.RowKey.IsEmpty;
		}


		private void HandleEntityCreated(object sender, EntityContextEventArgs e)
		{
			AbstractEntity entity = e.Entity;

			Druid leafEntityId = entity.GetEntityStructuredTypeId ();
					
			this.OnCreationPatchEntity (entity);
			this.OnCreationRegisterAsEmptyEntity (entity, e);
			this.OnCreationAddToCache (entity);

			try
			{
				this.OnCreationLoadSchema (leafEntityId);
			}
			catch
			{
				this.OnCreationRemoveFromCache (entity);
				throw;
			}
		}


		private void OnCreationPatchEntity(AbstractEntity entity)
		{
			bool entityIsNotPatched = !EntityNullReferenceVirtualizer.IsPatchedEntity (entity);

			if (this.EnableNullVirtualization && entityIsNotPatched)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			}
		}


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

		
		private void OnCreationAddToCache(AbstractEntity entity)
		{
			this.entitiesCache.Add (entity);
		}


		private void OnCreationRemoveFromCache(AbstractEntity entity)
		{
			this.entitiesCache.Remove (entity);
		}


		private void OnCreationLoadSchema(Druid leafEntityId)
		{
			this.SchemaEngine.LoadSchema (leafEntityId);
		}


		private bool isDisposed;


		private readonly EntityCache entitiesCache;


		private readonly HashSet<AbstractEntity> emptyEntities;


		private readonly HashSet<AbstractEntity> entitiesToDelete;


		private readonly HashSet<AbstractEntity> entitiesDeleted;


		private static int nextUniqueId;


	}


}