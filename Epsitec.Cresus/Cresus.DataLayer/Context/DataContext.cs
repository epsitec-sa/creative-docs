//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Saver;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <summary>
	/// The <c>DataContext</c> class provides a context in which entities can
	/// be loaded from the database, modified and then saved back.
	/// </summary>
	[System.Diagnostics.DebuggerDisplay ("DataContext #{UniqueId}")]
	public sealed class DataContext : System.IDisposable, IEntityPersistenceManager
	{


		public DataContext(DbInfrastructure infrastructure, bool bulkMode = false)
		{
			this.UniqueId = System.Threading.Interlocked.Increment (ref DataContext.nextUniqueId);
			this.DbInfrastructure = infrastructure;
			this.SchemaEngine = SchemaEngine.GetSchemaEngine (this.DbInfrastructure);
			this.EntityContext = EntityContext.Current;
			this.DataLoader = new DataLoader (this);
			this.DataSaver = new DataSaver (this);

			this.EnableEntityNullReferenceVirtualizer = false;

			this.entityDataCache = new EntityDataCache ();
			this.emptyEntities = new HashSet<AbstractEntity> ();
			this.entitiesToDelete = new HashSet<AbstractEntity> ();
			this.deletedEntities = new HashSet<AbstractEntity> ();
			
			this.EntityContext.EntityAttached += this.HandleEntityCreated;
			this.EntityContext.PersistenceManagers.Add (this);
		}


		internal int UniqueId
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


		internal EntityContext EntityContext
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


		public bool EnableEntityNullReferenceVirtualizer
		{
			get;
			set;
		}


		public TEntity CreateEntity<TEntity>() where TEntity : AbstractEntity, new ()
		{
			return this.EntityContext.CreateEmptyEntity<TEntity> ();
		}


		public TEntity CreateEmptyEntity<TEntity>() where TEntity : AbstractEntity, new ()
		{
			TEntity entity = this.CreateEntity<TEntity> ();

			this.RegisterEmptyEntity (entity);

			return entity;
		}


		internal EntityKey GetEntityKey(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			if (mapping == null)
			{
				return EntityKey.Empty;
			}
			else
			{
				return new EntityKey (mapping.RowKey, entity.GetEntityStructuredTypeId ());
			}
		}


		public bool Contains(AbstractEntity entity)
		{
			return this.entityDataCache.ContainsEntity (entity);
		}


		public bool ContainsChanges()
		{
			bool containsDeletedEntities = this.entitiesToDelete.Any();
			bool containsChangedEntities = this.GetModifiedEntities ().Any ();

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


		public bool UpdateEmptyEntityStatus(AbstractEntity entity, params bool[] emptyPredicateResults)
		{
			System.Diagnostics.Debug.Assert (emptyPredicateResults.Length > 0);

			bool result = emptyPredicateResults.All (x => x);

			this.UpdateEmptyEntityStatus (entity, result);

			return result;
		}

		public bool UpdateEmptyEntityStatusAbstractEntity<T>(T entity, System.Predicate<T> emptyPredicate) where T : AbstractEntity
		{
			bool result = emptyPredicate (entity);
			
			this.UpdateEmptyEntityStatus (entity, result);

			return result;
		}


		public void DeleteEntity(AbstractEntity entity)
		{
			if (!this.deletedEntities.Contains (entity))
			{
				this.entitiesToDelete.Add (entity);
			}
		}


		public bool IsDeleted(AbstractEntity entity)
		{
			return this.deletedEntities.Contains (entity);
		}


		internal IEnumerable<AbstractEntity> GetManagedEntities()
		{
			return this.entityDataCache.GetEntities ();
		}


		internal IEnumerable<AbstractEntity> GetModifiedEntities()
		{
			return from AbstractEntity entity in this.GetManagedEntities ()
				   where entity.GetEntityDataGeneration () >= this.EntityContext.DataGeneration
				   select entity;
		}

		
		internal EntityDataMapping GetEntityDataMapping(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}
			else
			{
				return this.entityDataCache.FindMapping (entity.GetEntitySerialId ());
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.Dipose (true);
		}


		#endregion

		
		private void Dipose(bool disposing)
		{
			if (disposing)
			{
				this.isDisposed = true;
				this.EntityContext.EntityAttached -= this.HandleEntityCreated;
				this.EntityContext.PersistenceManagers.Remove (this);
			}
		}


		#region IEntityPersistenceManager Members


		public string GetPersistedId(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			if (mapping != null && !mapping.RowKey.IsEmpty)
			{
				Druid entityId = entity.GetEntityStructuredTypeId ();
				long  keyId = mapping.RowKey.Id;
				short keyStatus = DbKey.ConvertToIntStatus (mapping.RowKey.Status);

				if (keyStatus == 0)
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}", entityId, keyId);
				}
				else
				{
					return string.Format (System.Globalization.CultureInfo.InvariantCulture, "db:{0}:{1}:{2}", entityId, keyId, keyStatus);
				}
			}

			return null;
		}


		public AbstractEntity GetPeristedEntity(string id)
		{
			if (string.IsNullOrEmpty (id))
			{
				return null;
			}
			else if (id.StartsWith ("db:"))
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

				if (!key.IsEmpty)
				{
					return this.DataLoader.ResolveEntity (key, entityId);
				}
			}

			return null;
		}


		#endregion


		public bool IsPersistent(AbstractEntity entity)
		{
			EntityDataMapping mapping = this.GetEntityDataMapping (entity);

			return mapping != null && !mapping.RowKey.IsEmpty;
		}


		private void HandleEntityCreated(object sender, EntityContextEventArgs e)
		{
			AbstractEntity entity = e.Entity;

			System.Diagnostics.Debug.Assert (this.EntityContext == entity.GetEntityContext ());

			if (this.EnableEntityNullReferenceVirtualizer)
			{
				EntityNullReferenceVirtualizer.PatchNullReferences (entity);
			}

			if (EntityNullReferenceVirtualizer.IsEmptyEntityContext (e.OldContext))
			{
				this.RegisterEmptyEntity (entity);
			}

			Druid entityId = entity.GetEntityStructuredTypeId ();
			Druid rootEntityId = this.EntityContext.GetRootEntityId (entityId);
			EntityDataMapping entityMapping = new EntityDataMapping (entity, entityId, rootEntityId);
			
			long entitySerialId = entity.GetEntitySerialId ();

			this.entityDataCache.Add (entitySerialId, entityMapping);

			try
			{
				this.SchemaEngine.LoadSchema (entityId);
			}
			catch
			{
				this.entityDataCache.Remove (entitySerialId);
				throw;
			}
		}


		private bool isDisposed;


		private readonly EntityDataCache entityDataCache;


		private readonly HashSet<AbstractEntity> emptyEntities;


		private readonly HashSet<AbstractEntity> entitiesToDelete;


		private readonly HashSet<AbstractEntity> deletedEntities;


		private static int nextUniqueId;


	}


}