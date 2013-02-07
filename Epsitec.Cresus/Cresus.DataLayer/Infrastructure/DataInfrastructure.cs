//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Support;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Logging;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.IO;

using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	/// <summary>
	/// The <c>DataInfrastructure</c> class provides an high level access to the data stored in the
	/// database.
	/// </summary>
	/// <remarks>
	/// This class is not completely thread safe. That means that all its members should always be
	/// called by the same thread or the thread safety must be ensured externally. The only two sets
	/// of members that can be called from any thread are the members dealing with the connection (
	/// Connection, OpenConnection, CloseConnection, RefreshConnectionData, KeepConnectionAlive,
	/// KillDeadConnections, AreLocksAvailable and CreateLockTransaction) and the members dealing
	/// with the DataContextPool (DataContextPool, CreateDataContext, DeleteDataContext and
	/// ContainsDataContext).
	/// Note that this class only ensure atomic execution of those methods, but that doesn't protect
	/// the user of this class from messing things up. For instance, it is possible to close the
	/// connection while a DataContext is saving, or to delete the DataContext while it is saving. So
	/// you still have to be careful about what you do when using instances of this class from 
	/// multiple threads.
	/// </remarks>
	public sealed class DataInfrastructure : IIsDisposed
	{
		// TODO Comment this class
		// Marc


		/// <summary>
		/// Creates a new instance of <c>DataInfrastructure</c>.
		/// </summary>
		/// <param name="dbInfrastructure">The <see cref="DbInfrastructure"/> used to query the Database.</param>
		/// <param name="entityEngine">The instance of <see cref="EntityEngine"/> used by this object.</param>
		public DataInfrastructure(DbInfrastructure dbInfrastructure , EntityEngine entityEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			entityEngine.ThrowIfNull ("entityEngine");

			this.entityEngine = entityEngine;
			this.dbInfrastructure = dbInfrastructure;

			this.infoManager = new InfoManager (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);
			this.uidManager = new UidManager (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);
			this.entityModificationLog = new EntityModificationLog (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);
			this.entityDeletionLog = new EntityDeletionLog (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);
			this.connectionManager = new ConnectionManager (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);
			this.lockManager = new LockManager (this.dbInfrastructure, this.entityEngine.ServiceSchemaEngine);

			this.dataContextPool = new DataContextPool ();

			this.connectionLock = new object ();

			this.connection = null;
		}


		/// <summary>
		/// The <see cref="DataContextPool"></see> associated with this instance.
		/// </summary>
		public DataContextPool DataContextPool
		{
			get
			{
				return this.dataContextPool;
			}
		}


		internal EntityEngine EntityEngine
		{
			get
			{
				return this.entityEngine;
			}
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"></see> object used to communicate with the database.
		/// </summary>
		internal DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
		}


		/// <summary>
		/// The <see cref="Connection"/> object that describes the connection of this
		/// instance with the database.
		/// </summary>
		public Connection Connection
		{
			get
			{
				lock (this.connectionLock)
				{
					return this.connection;
				}
			}
		}


		#region IIsDisposed Members


		public bool IsDisposed
		{
			get;
			private set;
		}


		#endregion


		#region IDisposable Members


		public void Dispose()
		{
			this.Dispose (true);
		}


		#endregion


		private void Dispose(bool disposing)
		{
			if (disposing && !this.IsDisposed)
			{
				lock (this.connectionLock)
				{
					if (this.connection != null && this.connection.Status == ConnectionStatus.Open)
					{
						try
						{
							this.connectionManager.CloseConnection (this.connection.Id);
						}
						catch (System.Exception)
						{
						}
					}
				}

				if (this.dataContextPool != null)
				{
					//	Beware: deleting a data context will modify the content of the pool;
					//	don't iterate on the original contents.
					
					var dataContexts = this.dataContextPool.ToArray ();
					
					foreach (DataContext dataContext in dataContexts)
					{
						this.DeleteDataContext (dataContext);
					}

					this.dataContextPool.Dispose ();
				}

				this.IsDisposed = true;
			}
		}


		/// <summary>
		/// Resets all index data related to the given sequence of entity type ids.
		/// </summary>
		/// <remarks>
		/// A call to this method will ensure for instance that the index selectivity is up to date
		/// with respect to the data present in the table and might therefore give a huge boost in
		/// performance. This method should therefore be called if large changes to the data have
		/// been made recently or have been accumulated over time.
		/// </remarks>
		public void ResetIndexes(IEnumerable<Druid> entityTypeIds)
		{
			foreach (var entityTypeId in entityTypeIds)
			{
				var dbTable = this.EntityEngine.EntitySchemaEngine.GetEntityTable (entityTypeId);

				foreach (var dbIndex in dbTable.Indexes)
				{
					this.DbInfrastructure.ResetIndex (dbTable, dbIndex);
				}
			}
		}

		/// <summary>
		/// Enables or disables all index data related to the given sequence of entity type ids.
		/// </summary>
		public void EnableIndexes(IEnumerable<Druid> entityTypeIds, bool enable)
		{
			foreach (var entityTypeId in entityTypeIds)
			{
				var dbTable = this.EntityEngine.EntitySchemaEngine.GetEntityTable (entityTypeId);

				foreach (var dbIndex in dbTable.Indexes)
				{
					this.DbInfrastructure.EnableIndex (dbTable, dbIndex, enable);
				}
			}
		}


		public System.DateTime GetDatabaseTime()
		{
			return this.dbInfrastructure.GetDatabaseTime ();
		}


		public void AddQueryLog(AbstractLog log)
		{
			log.ThrowIfNull ("log");

			this.dbInfrastructure.QueryLogs.Add (log);
		}


		public void RemoveQueryLog(AbstractLog log)
		{
			log.ThrowIfNull ("log");

			this.dbInfrastructure.QueryLogs.Remove (log);
		}


		/// <summary>
		/// Gets an information stored in the database.
		/// </summary>
		/// <param name="key">The key defining the information whose value to get.</param>
		/// <returns>The value of the information corresponding to the given key.</returns>
		public string GetDatabaseInfo(string key)
		{
			this.AssertIsConnected ();

			return this.infoManager.GetInfo (key);
		}


		/// <summary>
		/// Sets the information corresponding to the given key to the given value in the database.
		/// </summary>
		/// <param name="key">The key defining the information whose value to set.</param>
		/// <param name="value">The new value of the information.</param>
		public void SetDatabaseInfo(string key, string value)
		{
			this.AssertIsConnected ();

			this.infoManager.SetInfo (key, value);
		}


		/// <summary>
		/// Creates a new generator for unique ids in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <param name="slots">The definition of the slots of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/>.</returns>
		/// <remarks>
		/// The slots are defined as a sequence of minimum and maximum values, which must be locally
		/// and globally consistent.
		/// </remarks>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="slots"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slots"/> contains overlapping slots.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public UidGenerator CreateUidGenerator(string name, IEnumerable<UidSlot> slots)
		{
			this.AssertIsConnected ();

			List<UidSlot> orderedSlots = slots == null
				? null
				: slots.OrderBy (s => s.MinValue).ToList ();

			return this.uidManager.CreateUidGenerator (name, orderedSlots);
		}


		/// <summary>
		/// Deletes a generator for unique ids from the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public void DeleteUidGenerator(string name)
		{
			this.AssertIsConnected ();

			this.uidManager.DeleteUidGenerator (name);
		}


		/// <summary>
		/// Tells whether a generator for unique ids exists in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns><c>true</c> if a generator with <paramref name="name"/> exists in the database, <c>false</c> if there aren't.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public bool DoesUidGeneratorExists(string name)
		{
			this.AssertIsConnected ();

			return this.uidManager.DoesUidGeneratorExist (name);
		}


		/// <summary>
		/// Gets the <see cref="UidGenerator"/> object used to manipulate a generator of unique ids
		/// in the database.
		/// </summary>
		/// <param name="name">The name of the generator.</param>
		/// <returns>The <see cref="UidGenerator"/> object.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.Exception">If the requested <see cref="UidGenerator"/> does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		public UidGenerator GetUidGenerator(string name)
		{
			this.AssertIsConnected ();

			return this.uidManager.GetUidGenerator (name);
		}


		/// <summary>
		/// Opens the high-level connection with the database: this will create a
		/// new <see cref="Connection"/>.
		/// </summary>
		/// <param name="identity">The user/machine identity.</param>
		/// <exception cref="System.InvalidOperationException">If the connection is already open.</exception>
		public void OpenConnection(string identity)
		{
			lock (this.connectionLock)
			{
				if (this.connection != null && this.connection.Status == ConnectionStatus.Open)
				{
					throw new System.InvalidOperationException ("This instance is already connected.");
				}

				this.connection = this.connectionManager.OpenConnection (identity);
			}
		}


		/// <summary>
		/// Closes the high level connection with the database.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void CloseConnection()
		{
			lock (this.connectionLock)
			{
				this.AssertIsConnected ();

				var id = this.connection.Id;

				this.connectionManager.CloseConnection (id);

				var identity = this.connection.Identity;
				var status = ConnectionStatus.Closed;
				var establishmenthTime = this.connection.EstablishmentTime;
				var refreshTime = this.connection.RefreshTime;

				this.connection = new Connection (id, identity, status, establishmenthTime, refreshTime);
			}
		}


		/// <summary>
		/// Refreshes the data about the connection of this instance.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If the connection has never been opened.</exception>
		public void RefreshConnectionData()
		{
			lock (this.connectionLock)
			{
				if (this.connection == null)
				{
					throw new System.InvalidOperationException ("This instance has never been connected.");
				}

				this.connection = this.connectionManager.GetConnection (this.connection.Id);
			}
		}


		/// <summary>
		/// Notifies the database that this instance of the application is still up and running. 
		/// </summary>
		/// <remarks>
		/// This method is thread safe and thus can be called by any thread, even if the other
		/// methods of the same instance are being called by another thread.
		/// </remarks>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void KeepConnectionAlive()
		{
			lock (this.connectionLock)
			{
				this.AssertIsConnected ();

				this.connectionManager.KeepConnectionAlive (this.connection.Id);
			}
		}


		/// <summary>
		/// Interrupts the connection to the database that are open but inactive for the given
		/// timeout and clean related data such as locks.
		/// </summary>
		/// <remarks>
		/// This method is thread safe and thus can be called by any thread, even if the other
		/// methods of the same instance are being called by another thread.
		/// </remarks>
		/// <param name="timeout">The amount of time after which an open connection should be killed.</param>
		public void KillDeadConnections(System.TimeSpan timeout)
		{
			this.AssertIsConnected ();

			this.connectionManager.KillDeadConnections (timeout);

			this.lockManager.KillDeadLocks ();
		}


		/// <summary>
		/// Tells whether all the given locks are available or not.
		/// </summary>
		/// <param name="lockNames">The name of the locks.</param>
		/// <returns><c>true</c> if all locks are available, <c>false</c> if at least one is not.</returns>
		public bool AreLocksAvailable(IEnumerable<string> lockNames)
		{
			lock (this.connectionLock)
			{
				this.AssertIsConnected ();

				lockNames.ThrowIfNull ("lockNames");

				return this.lockManager.GetLocks (lockNames.ToList ())
					.All (l => l.Owner.Id == this.connection.Id);
			}
		}


		/// <summary>
		/// Creates a new <see cref="LockTransaction"/> for the given locks.
		/// </summary>
		/// <param name="lockNames">The name of the locks to get.</param>
		/// <returns>The new <see cref="LockTransaction"/> object.</returns>
		public LockTransaction CreateLockTransaction(IEnumerable<string> lockNames)
		{
			lock (this.connectionLock)
			{
				this.AssertIsConnected ();

				return new LockTransaction (this.lockManager, this.connection.Id, lockNames);
			}
		}


		internal EntityDeletionEntry CreateEntityDeletionEntry(DbId entityModificationEntryId, Druid entityTypeId, DbId entityId)
		{
			this.AssertIsConnected ();

			return this.entityDeletionLog.CreateEntry (entityModificationEntryId, entityTypeId, entityId);
		}


		internal IEnumerable<EntityDeletionEntry> GetEntityDeletionEntriesNewerThan(DbId minimumId)
		{
			this.AssertIsConnected ();

			return this.entityDeletionLog.GetEntriesNewerThan (minimumId);
		}


		internal EntityModificationEntry CreateEntityModificationEntry()
		{
			lock (this.connectionLock)
			{
				this.AssertIsConnected ();

				return this.entityModificationLog.CreateEntry (this.connection.Id);
			}
		}


		internal EntityModificationEntry GetLatestEntityModificationEntry()
		{
			this.AssertIsConnected ();

			return this.entityModificationLog.GetLatestEntry ();
		}


		public DataContext CreateDataContext(bool enableNullVirtualization = false, bool readOnly = false, bool enableReload = false)
		{
			DataContext dataContext = new DataContext (this, enableNullVirtualization, readOnly, enableReload);

			this.dataContextPool.Add (dataContext);

			return dataContext;
		}


		public void DeleteDataContext(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");
			
			bool removed = this.dataContextPool.Remove (dataContext);

			if (!removed)
			{
				throw new System.ArgumentException ("dataContext is not owned by this instance");
			}

			dataContext.Dispose ();
		}


		public bool ContainsDataContext(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			return this.dataContextPool.Contains (dataContext);
		}


		/// <summary>
		/// Exports a set of <see cref="AbstractEntity"/> to an xml file. The set of exported
		/// <see cref="AbstractEntity"/> is defined by the given <see cref="AbstractEntity"/> and
		/// the given predicate. The algorithm will explore the graph of the <see cref="AbstractEntity"/>
		/// by recursively following all the relations that target an <see cref="AbstractEntity"/>
		/// which satisfies the given predicate, starting with the given <see cref="AbstractEntity"/>.
		/// The resulting subset of the graph will be exported.
		/// </summary>
		/// <param name="file">The file that will contain the exported data.</param>
		/// <param name="dataContext">The <see cref="DataContext"/> that owns the given <see cref="AbstractEntity"/>.</param>
		/// <param name="entities">The collection of entities which will be exported.</param>
		/// <param name="predicate">The predicate used to determine whether to export an <see cref="AbstractEntity"/> or not.</param>
		/// <param name="exportMode">The mode used to export the data.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="dataContext"/> has not been created by this instance.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entities"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="entities"/> contains any entity foreign to <paramref name="dataContext"/>.</exception>
		public void Export(FileInfo file, DataContext dataContext, IEnumerable<AbstractEntity> entities, System.Func<AbstractEntity, bool> predicate = null, ExportationMode exportMode = ExportationMode.PersistedEntities)
		{
			file.ThrowIfNull ("file");
			dataContext.ThrowIfNull ("dataContext");
			dataContext.ThrowIf (d => !this.dataContextPool.Contains (d), "dataContext is not owned by this instance");
			entities.ThrowIfNull ("entity");
			entities.ThrowIf (e => e.Any (x => !dataContext.Contains (x)), "entity is not owned by dataContext.");

			this.AssertIsConnected ();

			if (predicate == null)
			{
				predicate = x => true;
			}

			ImportExportManager.Export (file, dataContext, entities, predicate, exportMode);
		}


		/// <summary>
		/// Imports a set of <see cref="AbstractEntity"/> from an xml file that has been written by
		/// the <see cref="DataInfrastructure.Export"/> method.
		/// </summary>
		/// <param name="file">The file containing the data to import.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void Import(FileInfo file)
		{
			file.ThrowIfNull ("file");

			this.AssertIsConnected ();

			ImportExportManager.Import (file, this);
		}


		/// <summary>
		/// Exports the Epsitec data stored in the database.
		/// </summary>
		/// <param name="file">The file in which to write the data.</param>
		/// <param name="exportMode">The mode used to export the data.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void ExportEpsitecData(FileInfo file, RawExportMode exportMode)
		{
			file.ThrowIfNull ("file");

			this.AssertIsConnected ();

			DbInfrastructure dbInfrastructure = this.dbInfrastructure;
			
			EntityTypeEngine typeEngine = this.EntityEngine.EntityTypeEngine;
			EntitySchemaEngine schemaEngine = this.EntityEngine.EntitySchemaEngine;

			RawEntitySerializer.Export (file, dbInfrastructure, typeEngine, schemaEngine, exportMode);
		}


		/// <summary>
		/// Replaces the Epsitec data stored within the database by the one stored in the given file.
		/// </summary>
		/// <param name="file">The file from which to read the data.</param>
		/// <param name="importMode">The mode used to import the data.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="file"/> is <c>null</c>.</exception>
		public void ImportEpsitecData(FileInfo file, RawImportMode importMode)
		{
			file.ThrowIfNull ("file");

			this.AssertIsConnected ();

			DbId connectionId = this.connection.Id;
			EntityModificationEntry entityModificationEntry = this.entityModificationLog.CreateEntry (connectionId);

			RawEntitySerializer.CleanDatabase (file, this.dbInfrastructure, importMode);
			RawEntitySerializer.Import (file, this.dbInfrastructure, entityModificationEntry, importMode);
		}


		/// <summary>
		/// Asserts that the connection of this instance is open and throws an
		/// <see cref="System.InvalidOperationException"/> otherwise.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If this instance is not connected.</exception>
		private void AssertIsConnected()
		{
			lock (this.connectionLock)
			{
				if (this.connection == null || this.connection.Status != ConnectionStatus.Open)
				{
					throw new System.InvalidOperationException ("This instance is not connected.");
				}
			}
		}


		private readonly DbInfrastructure		dbInfrastructure;
		private readonly EntityEngine			entityEngine;
		private readonly DataContextPool		dataContextPool;
		private readonly InfoManager			infoManager;
		private readonly UidManager				uidManager;
		private readonly EntityModificationLog	entityModificationLog;
		private readonly EntityDeletionLog		entityDeletionLog;
		private readonly ConnectionManager		connectionManager;
		private readonly LockManager			lockManager;

		private readonly object					connectionLock;

		private Connection connection;
	}
}
