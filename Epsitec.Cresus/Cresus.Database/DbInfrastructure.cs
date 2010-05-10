//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbInfrastructure</c> class provides support for the database
	/// infrastructure needed by CRESUS (internal tables, metadata, etc.)
	/// </summary>
	public sealed class DbInfrastructure : DependencyObject, System.IDisposable
	{
		public DbInfrastructure()
		{
			this.localizations    = "fr";
			this.liveTransactions = new List<DbTransaction> ();
			this.releaseRequested = new List<IDbAbstraction> ();

			this.SchemasCache = new Dictionary<DbTable, System.Data.DataTable> ();
		}


		/// <summary>
		/// Gets a value indicating whether the connection is open.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the connection is open; otherwise, <c>false</c>.
		/// </value>
		public bool								IsConnectionOpen
		{
			get
			{
				return (this.abstraction != null) && (this.abstraction.IsConnectionOpen);
			}
		}
		
		public string[]							DefaultLocalizations
		{
			get
			{
				if (string.IsNullOrEmpty (this.localizations))
				{
					return new string[0];
				}
				else
				{
					return this.localizations.Split ('/');
				}
			}
			set
			{
				if ((value == null) ||
					(value.Length == 0))
				{
					this.localizations = null;
				}
				else
				{
					this.localizations = string.Join ("/", value);
				}
			}
		}
		
		public ISqlBuilder						DefaultSqlBuilder
		{
			get
			{
				return this.abstraction.SqlBuilder;
			}
		}
		
		public ISqlEngine						DefaultSqlEngine
		{
			get
			{
				return this.sqlEngine;
			}
		}
		
		public IDbAbstraction					DefaultDbAbstraction
		{
			get
			{
				return this.abstraction;
			}
		}
		
		public DbContext						DefaultContext
		{
			get
			{
				return DbContext.Current;
			}
		}

		public DbTransaction					DefaultLiveTransaction
		{
			get
			{
				return this.FindLiveTransaction (this.abstraction);
			}
		}

		public ITypeConverter					Converter
		{
			get
			{
				return this.converter;
			}
		}
		
		public DbTransaction[]					LiveTransactions
		{
			get
			{
				lock (this.liveTransactions)
				{
					return this.liveTransactions.ToArray ();
				}
			}
		}
		
		public DbLogger							Logger
		{
			get
			{
				return this.logger;
			}
		}
		
		public DbClientManager					ClientManager
		{
			get
			{
				if (this.LocalSettings.IsServer)
				{
					return this.clientManager;
				}
				else
				{
					return null;
				}
			}
		}

		public DbAccess							Access
		{
			get
			{
				return this.access;
			}
		}
		
		public bool								IsInGlobalLock
		{
			get
			{
				return this.globalLock.IsWriterLockHeld;
			}
		}
		
		public Settings.Globals					GlobalSettings
		{
			get
			{
				return this.globals;
			}
		}
		
		public Settings.Locals					LocalSettings
		{
			get
			{
				return this.locals;
			}
		}


		public Dictionary<DbTable, System.Data.DataTable> SchemasCache
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates a new database using the specified database access. This
		/// is only possible if the <c>DbInfrastructure</c> is not yet connected
		/// to any database.
		/// </summary>
		/// <param name="access">The database access.</param>
		public bool CreateDatabase(DbAccess access)
		{
			return this.CreateDatabase (access, false);
		}

		/// <summary>
		/// Creates a new database using the specified database access. This
		/// is only possible if the <c>DbInfrastructure</c> is not yet connected
		/// to any database.
		/// </summary>
		/// <param name="access">The database access.</param>
		/// <param name="isServer">If set to <c>true</c> creates the database for a server.</param>
		public bool CreateDatabase(DbAccess access, bool isServer)
		{
			if (this.access.IsValid)
			{
				throw new Exceptions.GenericException (this.access, "A database already exists for this DbInfrastructure");
			}
			
			this.access = access;
			this.access.CreateDatabase = true;

			if (this.InitializeDatabaseAbstraction () == false)
			{
				this.access = new DbAccess ();
				return false;
			}
			
			this.types.RegisterTypes ();
			
			//	The database is now ready to be filled. At this point, it is totally
			//	empty of any tables.
			
			System.Diagnostics.Debug.Assert (this.abstraction.QueryUserTableNames ().Length == 0);
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				BootHelper helper = new BootHelper (this, transaction);

				//	Create the tables required for our own metadata management. The
				//	created tables must be commited before they can be populated.

				helper.CreateTableTableDef ();
				helper.CreateTableColumnDef ();
				helper.CreateTableTypeDef ();
				helper.CreateTableLog ();
				helper.CreateTableRequestQueue ();
				helper.CreateTableClientDef ();
				
				transaction.Commit ();
			}
			
			//	TODO: handle clientId
			this.clientId = 1;
			
			//	Fill the tables with the initial metadata.
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetupTables (transaction);
				
				transaction.Commit ();
			}

			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				Settings.Globals.CreateTable (this, transaction, Settings.Globals.Name, DbElementCat.Internal, DbRevisionMode.IgnoreChanges, DbReplicationMode.Automatic);
				Settings.Locals.CreateTable (this, transaction, Settings.Locals.Name, DbElementCat.Internal, DbRevisionMode.IgnoreChanges, DbReplicationMode.None);
				
				transaction.Commit ();
			}
			
			//	Create the default values for the global and local settings :
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				Settings.Globals globals = new Settings.Globals (this, transaction);
				Settings.Locals  locals  = new Settings.Locals (this, transaction);
				
				locals.ClientId = this.clientId;
				locals.IsServer = isServer;

				globals.PersistToBase (transaction);
				locals.PersistToBase (transaction);

				transaction.Commit ();
			}
			
			//	The database is ready. Start using it...
			
			this.StartUsingDatabase ();

			return true;
		}

		/// <summary>
		/// Attaches to an existing database. This is only possible if the
		/// <c>DbInfrastructure</c> is not yet connected to any database.
		/// </summary>
		/// <param name="access">The database access.</param>
		public bool AttachToDatabase(DbAccess access)
		{
			if (this.access.IsValid)
			{
				throw new Exceptions.GenericException (this.access, "Database already attached");
			}
			
			this.access = access;
			this.access.CreateDatabase = false;

			if (this.InitializeDatabaseAbstraction () == false)
			{
				this.access = new DbAccess ();
				return false;
			}
			
			//	The database must have user tables (at least, it should have our metadata
			//	tables)...

			if (this.abstraction.QueryUserTableNames ().Length == 0)
			{
				throw new Exceptions.EmptyDatabaseException (this.access);
			}
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableLog));
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableTableDef));
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableColumnDef));
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableTypeDef));
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableClientDef));
				this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableRequestQueue));
				
				this.types.ResolveTypes (transaction);
				
				transaction.Commit ();
			}
			
			this.StartUsingDatabase ();

			return true;
		}

		/// <summary>
		/// Sets up a roaming database. Call <c>AttachToDatabase</c> first.
		/// </summary>
		/// <param name="clientId">The current client id.</param>
		public void SetupRoamingDatabase(int clientId)
		{
			if (this.clientId != clientId)
			{
				using (DbTransaction transaction = this.BeginTransaction ())
				{
					//	The last ID stored in the log is considered to be the active id at the
					//	synchronization point (setting up a roaming database requires an image
					//	of the server database to start its operations).
					
					DbId lastServerId = this.logger.CurrentId;
					
					//	Since this is a fresh roaming database, all table ids will be reset to
					//	one (1) :
					
					this.ResetColumnData (transaction, this.internalTables[Tags.TableTableDef], Tags.ColumnNextId, DbId.CreateId (1, clientId));
					
					//	Clear tables which should not be replicated between the server and the
					//	client databases :
					
					this.ClearTable (transaction, this.internalTables[Tags.TableRequestQueue]);
					this.ClearTable (transaction, this.internalTables[Tags.TableClientDef]);
					
					//	Update the logger in order to use the new client id instead of the
					//	id found in the copied database :
					
					this.logger.Detach ();
					
					this.logger = new DbLogger ();
					this.logger.DefineClientId (clientId);
					this.logger.DefineInitialLogId (1);
					this.logger.Attach (this, this.internalTables[Tags.TableLog]);
					this.logger.CreateInitialEntry (transaction);
					
					//	Define local settings based on the client :
					
					this.LocalSettings.ClientId  = clientId;
					this.LocalSettings.IsServer  = false;
					this.LocalSettings.SyncLogId = lastServerId.Value;
					
					this.LocalSettings.PersistToBase (transaction);
					
					transaction.Commit ();
					
					this.clientId = clientId;
				}
			}
		}

		/// <summary>
		/// Clears the table by removing all rows.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		private void ClearTable(DbTransaction transaction, DbTable table)
		{
			transaction.SqlBuilder.RemoveData (table.GetSqlName (), null);
			this.ExecuteNonQuery (transaction);
		}

		/// <summary>
		/// Resets the data in the specified column to the specified value.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="data">The data.</param>
		private void ResetColumnData(DbTransaction transaction, DbTable table, string columnName, DbId data)
		{
			DbColumn              column = table.Columns[columnName];
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();
			
			fields.Add (column.GetSqlName (), this.CreateSqlFieldFromAdoValue (column, data));
			
			transaction.SqlBuilder.UpdateData (table.GetSqlName (), fields, null);
			
			this.ExecuteNonQuery (transaction);
		}

		/// <summary>
		/// Releases the database connection. If transactions are still active
		/// for this database, the connection will be released autmatically
		/// when the transactions finish.
		/// </summary>
		public void ReleaseConnection()
		{
			this.ReleaseConnection (this.abstraction);
		}

		/// <summary>
		/// Releases the database connection. If transactions are still active
		/// for this database, the connection will be released autmatically
		/// when the transactions finish.
		/// </summary>
		/// <param name="abstraction">The database abstraction.</param>
		private void ReleaseConnection(IDbAbstraction abstraction)
		{
			lock (this.liveTransactions)
			{
				foreach (DbTransaction item in this.liveTransactions)
				{
					if (item.Database == abstraction)
					{
						this.releaseRequested.Add (abstraction);
						return;
					}
				}
			}				
			
			abstraction.ReleaseConnection ();
		}

		/// <summary>
		/// Creates the database access.
		/// </summary>
		/// <param name="name">The database file name.</param>
		/// <returns>The database access.</returns>
		public static DbAccess CreateDatabaseAccess(string name)
		{
			return new DbAccess ("Firebird", name, "localhost", "sysdba", "masterkey", false);
		}

		/// <summary>
		/// Creates the database access.
		/// </summary>
		/// <param name="provider">The database provider.</param>
		/// <param name="name">The database file name.</param>
		/// <returns>The database access.</returns>
		public static DbAccess CreateDatabaseAccess(string provider, string name)
		{
			return new DbAccess (provider, name, "localhost", "sysdba", "masterkey", false);
		}

		/// <summary>
		/// Creates a new database abstraction. This will create a new connection
		/// with the database.
		/// </summary>
		/// <returns>The database abstraction.</returns>
		public IDbAbstraction CreateDatabaseAbstraction()
		{
			IDbAbstraction abstraction = DbFactory.CreateDatabaseAbstraction (this.access);

			if (abstraction.IsConnectionInitialized == false)
			{
				//	Could not open connection and configured to not throw any
				//	exceptions...

				abstraction.Dispose ();
				abstraction = null;
			}
			
			return abstraction;
		}

		/// <summary>
		/// Begins a read and write transaction for the default database abstraction.
		/// </summary>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction()
		{
			return this.BeginTransaction (DbTransactionMode.ReadWrite);
		}

		/// <summary>
		/// Begins the specified transaction for the default database abstraction.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction(DbTransactionMode mode)
		{
			return this.BeginTransaction (mode, this.abstraction);
		}

		/// <summary>
		/// Begins a transaction for the specified database abstraction.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <param name="abstraction">The database abstraction.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction(DbTransactionMode mode, IDbAbstraction abstraction)
		{
			System.Diagnostics.Debug.Assert (abstraction != null);
			
			//	We currently allow a single transaction per database abstraction,
			//	because some ADO.NET providers do not support cascaded transactions.
			
			DbTransaction transaction = null;
			
			//	Make sure we can get a lock on the database. If not, this means
			//	that someone holds a global lock and we may not access the database
			//	at all, even within a transaction. This is the case when restoring
			//	a database, for instance.
			
			this.DatabaseLock (abstraction);
			
			try
			{
				switch (mode)
				{
					case DbTransactionMode.ReadOnly:
						transaction = new DbTransaction (abstraction.BeginReadOnlyTransaction (), abstraction, this, mode);
						break;
					
					case DbTransactionMode.ReadWrite:
						transaction = new DbTransaction (abstraction.BeginReadWriteTransaction (), abstraction, this, mode);
						break;
					
					default:
						throw new System.ArgumentOutOfRangeException ("mode", mode, string.Format ("Transaction mode {0} not supported", mode.ToString ()));
				}
			}
			catch
			{
				this.DatabaseUnlock (abstraction);
				throw;
			}
			
			return transaction;
		}

		/// <summary>
		/// Inherits the live transaction or begins a transaction. When inheriting,
		/// the transaction modes must be compatible.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction InheritOrBeginTransaction(DbTransactionMode mode)
		{
			DbTransaction live = this.DefaultLiveTransaction;

			if (live == null)
			{
				return this.BeginTransaction (mode);
			}
			else
			{
				if ((mode == DbTransactionMode.ReadWrite) &&
					(live.IsReadOnly))
				{
					throw new System.InvalidOperationException ("Cannot begin read/write transaction from inherited read-only transaction");
				}
				
				return new DbTransaction (live);
			}
		}

		/// <summary>
		/// Creates the select condition for active rows.
		/// </summary>
		/// <returns>The select condition.</returns>
		public DbSelectCondition CreateSelectCondition()
		{
			return this.CreateSelectCondition (DbSelectRevision.LiveActive);
		}

		/// <summary>
		/// Creates the select condition for the specified revision.
		/// </summary>
		/// <param name="revision">The revision.</param>
		/// <returns>The select condition.</returns>
		public DbSelectCondition CreateSelectCondition(DbSelectRevision revision)
		{
			return new DbSelectCondition (this.Converter, revision);
		}

		/// <summary>
		/// Creates a minimal database table definition. This will only contain
		/// the basic id and status columns required by <c>DbInfrastructure</c>.
		/// </summary>
		/// <param name="name">The table name.</param>
		/// <param name="category">The category.</param>
		/// <param name="revisionMode">The revision mode.</param>
		/// <returns>The database table definition.</returns>
		public DbTable CreateDbTable(string name, DbElementCat category, DbRevisionMode revisionMode)
		{
			switch (category)
			{
				case DbElementCat.Internal:
					throw new Exceptions.GenericException (this.access, string.Format ("Users may not create internal tables (table '{0}')", name));
				
				case DbElementCat.ManagedUserData:
					return this.CreateTable(name, category, revisionMode, DbReplicationMode.Automatic);
				
				default:
					throw new Exceptions.GenericException (this.access, string.Format ("Unsupported category {0} specified for table '{1}'", category, name));
			}
		}

		/// <summary>
		/// Creates a minimal database table definition. This will only contain
		/// the basic id and status columns required by <c>DbInfrastructure</c>.
		/// </summary>
		/// <param name="captionId">The table caption id.</param>
		/// <param name="category">The category.</param>
		/// <param name="revisionMode">The revision mode.</param>
		/// <returns>The database table definition.</returns>
		public DbTable CreateDbTable(Druid captionId, DbElementCat category, DbRevisionMode revisionMode)
		{
			switch (category)
			{
				case DbElementCat.Internal:
					throw new Exceptions.GenericException (this.access, string.Format ("Users may not create internal tables (table '{0}')", captionId));

				case DbElementCat.ManagedUserData:
					return this.CreateTable (captionId, category, revisionMode, DbReplicationMode.Automatic);

				default:
					throw new Exceptions.GenericException (this.access, string.Format ("Unsupported category {0} specified for table '{1}'", category, captionId));
			}
		}

		/// <summary>
		/// Registers a new table for this database. This creates both the
		/// metadata and the database table itself.
		/// </summary>
		/// <param name="table">The table.</param>
		public void RegisterNewDbTable(DbTable table)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RegisterNewDbTable (transaction, table);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Registers a new table for this database. This creates both the
		/// metadata and the database table itself.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		public void RegisterNewDbTable(DbTransaction transaction, DbTable table)
		{
			this.RegisterDbTable (transaction, table, false);
		}

		/// <summary>
		/// Registers a known table for this database. This call is reserved for
		/// the replication service which must be able to create tables in the
		/// database for which there already exists metadata.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		public void RegisterKnownDbTable(DbTransaction transaction, DbTable table)
		{
			this.RegisterDbTable (transaction, table, true);
		}

		/// <summary>
		/// Unregisters a table from the database. The metadata is updated but
		/// the real database table is not dropped for security reasons.
		/// </summary>
		/// <param name="table">The table.</param>
		public void UnregisterDbTable(DbTable table)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.UnregisterDbTable (transaction, table);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Unregisters a table from the database. The metadata is updated but
		/// the real database table is not dropped for security reasons.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		public void UnregisterDbTable(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			this.CheckForKnownTable (transaction, table);
			
			DbKey oldKey = table.Key;
			DbKey newKey = new DbKey (oldKey.Id, DbRowStatus.Deleted);
			
			this.UpdateKeyInRow (transaction, Tags.TableTableDef, oldKey, newKey);
		}

		/// <summary>
		/// Resolves the database table definition with the specified name. This
		/// will return the same object when called multiple times with the same
		/// name, unless the cache is cleared with <c>ClearCaches</c>.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The table definition.</returns>
		public DbTable ResolveDbTable(string tableName)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable value = this.ResolveDbTable (transaction, tableName);
				transaction.Commit ();
				return value;
			}
		}

		/// <summary>
		/// Resolves the database table definition with the specified key. This
		/// will return the same object when called multiple times with the same
		/// key, unless the cache is cleared with <c>ClearCaches</c>.
		/// </summary>
		/// <param name="key">The key to the table metadata.</param>
		/// <returns>The table definition.</returns>
		public DbTable ResolveDbTable(DbKey key)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable value = this.ResolveDbTable (transaction, key);
				transaction.Commit ();
				return value;
			}
		}

		/// <summary>
		/// Resolves the database table definition with the specified name. This
		/// will return the same object when called multiple times with the same
		/// name, unless the cache is cleared with <c>ClearCaches</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The table definition.</returns>
		public DbTable ResolveDbTable(DbTransaction transaction, string tableName)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			DbKey key = this.FindDbTableKey (transaction, tableName);
			return this.ResolveDbTable (transaction, key);
		}

		/// <summary>
		/// Resolves the database table definition with the specified key. This
		/// will return the same object when called multiple times with the same
		/// key, unless the cache is cleared with <c>ClearCaches</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="key">The key to the table metadata.</param>
		/// <returns>The table definition.</returns>
		public DbTable ResolveDbTable(DbTransaction transaction, DbKey key)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			if (key.IsEmpty)
			{
				return null;
			}

			lock (this.tableCache)
			{
				DbTable table = this.tableCache[key];
				
				if (table == null)
				{
					List<DbTable> tables = this.LoadDbTable (transaction, key, DbRowSearchMode.LiveActive);
					
					if (tables.Count > 0)
					{
						System.Diagnostics.Debug.Assert (tables.Count == 1);
						System.Diagnostics.Debug.Assert (tables[0].Key == key);
						System.Diagnostics.Debug.Assert (this.tableCache[key] == tables[0]);
						
						table = tables[0];
					}
				}
				
				return table;
			}
		}

		/// <summary>
		/// Resolves the database table definition with the specified id. This
		/// will return the same object when called multiple times with the same
		/// id, unless the cache is cleared with <c>ClearCaches</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableId">The table id.</param>
		/// <returns>The table definition.</returns>
		public DbTable ResolveDbTable(DbTransaction transaction, Druid tableId)
		{
			DbTable templateTable = new DbTable (tableId);
			return this.ResolveDbTable (transaction, templateTable.Name);
		}

		/// <summary>
		/// Finds all live database table definitions belonging to the specified
		/// category (either internal or user data).
		/// </summary>
		/// <param name="category">The table category.</param>
		/// <returns>The table definitions or an empty array.</returns>
		public DbTable[] FindDbTables(DbElementCat category)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTable[] value = this.FindDbTables (transaction, category);
				transaction.Commit ();
				return value;
			}
		}

		/// <summary>
		/// Finds all live database table definitions belonging to the specified
		/// category (either internal or user data).
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="category">The table category.</param>
		/// <returns>The table definitions or an empty array.</returns>
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category)
		{
			return this.FindDbTables (transaction, category, DbRowSearchMode.LiveActive);
		}

		/// <summary>
		/// Finds the database table definitions belonging to the specified
		/// category (either internal or user data).
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="category">The table category.</param>
		/// <param name="rowSearchMode">The row search mode.</param>
		/// <returns>The table definitions or an empty array.</returns>
		public DbTable[] FindDbTables(DbTransaction transaction, DbElementCat category, DbRowSearchMode rowSearchMode)
		{
			List<DbTable> list = this.LoadDbTable (transaction, DbKey.Empty, rowSearchMode);
			
			if (category != DbElementCat.Any)
			{
				list.RemoveAll
					(
						delegate (DbTable table)
						{
							return table.Category != category;
						}
					);
			}
			
			return list.ToArray ();
		}

		/// <summary>
		/// Clears the table and type caches. This will force a reload of the
		/// table definitions and type definitions.
		/// </summary>
		public void ClearCaches()
		{
			lock (this.tableCache)
			{
				this.tableCache.ClearCache ();
			}
			lock (this.typeCache)
			{
				this.typeCache.ClearCache ();
			}
		}

		/// <summary>
		/// Registers the column relations by generating the associated database
		/// metadata. Every foreign key found in the table generates one relation.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		public void RegisterColumnRelations(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			foreach (DbColumn column in table.Columns)
			{
				if (column.ColumnClass == DbColumnClass.RefId)
				{
					string targetTableName = column.TargetTableName;
					
					if (! string.IsNullOrEmpty (targetTableName))
					{
						DbTable targetTable = this.ResolveDbTable (transaction, targetTableName);
						
						if (targetTable == null)
						{
							string message = string.Format ("Table '{0}' referenced from '{1}.{2}' not found in database", targetTableName, table.Name, column.Name);
							throw new Exceptions.GenericException (this.access, message);
						}
						
						DbKey sourceTableKey  = table.Key;
						DbKey sourceColumnKey = column.Key;
						DbKey targetTableKey  = targetTable.Key;
						
						if (sourceTableKey.IsEmpty)
						{
							string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{1}'", targetTableName, table.Name, column.Name);
							throw new Exceptions.GenericException (this.access, message);
						}

						if (sourceColumnKey.IsEmpty)
						{
							string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered column '{2}'", targetTableName, table.Name, column.Name);
							throw new Exceptions.GenericException (this.access, message);
						}

						if (targetTableKey.IsEmpty)
						{
							string message = string.Format ("Reference of '{0}' from '{1}.{2}' specifies unregistered table '{0}'", targetTableName, table.Name, column.Name);
							throw new Exceptions.GenericException (this.access, message);
						}
						
						//	We have found a valid relation between the source column
						//	and the arget table. Update it in our metadata:
						
						this.UpdateColumnRelation (transaction, sourceTableKey, sourceColumnKey, targetTableKey);
					}
				}
			}
		}

		/// <summary>
		/// Registers a new type and stores it into the database metadata.
		/// </summary>
		/// <param name="typeDef">The type definition.</param>
		public void RegisterNewDbType(DbTypeDef typeDef)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RegisterNewDbType (transaction, typeDef);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Registers a new type and stores it into the database metadata. If
		/// a type with the same name already exists in the database, this will
		/// throw an exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		public void RegisterNewDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			this.CheckForUnknownType (transaction, typeDef);

			long tableId = this.NewRowIdInTable (transaction, this.internalTables[Tags.TableTypeDef], 1);

			typeDef.DefineKey (new DbKey (tableId));
			this.InsertTypeDefRow (transaction, typeDef);
		}

		/// <summary>
		/// Unregisters the type from the database. This will not drop the
		/// type but mark it as deleted for security reasons.
		/// </summary>
		/// <param name="typeDef">The type definition.</param>
		public void UnregisterDbType(DbTypeDef typeDef)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.UnregisterDbType (transaction, typeDef);
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Unregisters the type from the database. This will not drop the
		/// type but mark it as deleted for security reasons.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		public void UnregisterDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			this.CheckForKnownType (transaction, typeDef);
			
			DbKey oldKey = typeDef.Key;
			DbKey newKey = new DbKey (oldKey.Id, DbRowStatus.Deleted);
			
			this.UpdateKeyInRow (transaction, Tags.TableTypeDef, oldKey, newKey);
		}

		/// <summary>
		/// Resolves a type definition from its name.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The type definition or <c>null</c>.</returns>
		public DbTypeDef ResolveDbType(string typeName)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTypeDef value = this.ResolveDbType (transaction, typeName);
				transaction.Commit ();
				return value;
			}
		}

		/// <summary>
		/// Resolves a type definition from its name.
		/// </summary>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The type definition or <c>null</c>.</returns>
		public DbTypeDef ResolveLoadedDbType(string typeName)
		{
			DbTypeDef value;

			if (this.internalTypes.TryGetValue (typeName, out value))
			{
				return value;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Resolves a type definition from its name.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeName">Name of the type.</param>
		/// <returns>The type definition or <c>null</c>.</returns>
		public DbTypeDef ResolveDbType(DbTransaction transaction, string typeName)
		{
			DbTypeDef value;

			if (this.internalTypes.TryGetValue (typeName, out value))
			{
				return value;
			}

			System.Diagnostics.Debug.Assert (transaction != null);
			
			DbKey key = this.FindDbTypeKey (transaction, typeName);
			value = this.ResolveDbType (transaction, key);
			
			return value;
		}

		/// <summary>
		/// Resolves a type definition from an <see cref="INamedType"/> instance.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="type">The type object.</param>
		/// <returns>The type definition or <c>null</c>.</returns>
		public DbTypeDef ResolveDbType(DbTransaction transaction, INamedType type)
		{
			if (type == null)
			{
				return null;
			}

			DbTypeDef templateType = new DbTypeDef (type);
			return this.ResolveDbType (transaction, templateType.Name);
		}

		/// <summary>
		/// Resolves a type definition from its key.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="key">The metadata key for the type.</param>
		/// <returns>The type definition or <c>null</c>.</returns>
		private DbTypeDef ResolveDbType(DbTransaction transaction, DbKey key)
		{
			if (key.IsEmpty)
			{
				return null;
			}

			lock (this.typeCache)
			{
				DbTypeDef typeDef = this.typeCache[key];
				
				if (typeDef == null)
				{
					List<DbTypeDef> types = this.LoadDbType (transaction, key, DbRowSearchMode.LiveActive);
					
					if (types.Count > 0)
					{
						System.Diagnostics.Debug.Assert (types.Count == 1);
						System.Diagnostics.Debug.Assert (types[0].Key == key);
						System.Diagnostics.Debug.Assert (this.typeCache[key] == types[0]);

						typeDef = types[0] as DbTypeDef;
					}
				}
				
				return typeDef;
			}
		}

		/// <summary>
		/// Finds all the live type definitions.
		/// </summary>
		/// <returns>The type definitions.</returns>
		public DbTypeDef[] FindDbTypes()
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbTypeDef[] value = this.FindDbTypes (transaction);
				transaction.Commit ();
				return value;
			}
		}

		/// <summary>
		/// Finds all the live type definitions.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The type definitions.</returns>
		public DbTypeDef[] FindDbTypes(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			return this.FindDbTypes (transaction, DbRowSearchMode.LiveActive);
		}

		/// <summary>
		/// Finds all the type definitions using a specific search mode.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="rowSearchMode">The row search mode.</param>
		/// <returns>The type definitions.</returns>
		public DbTypeDef[] FindDbTypes(DbTransaction transaction, DbRowSearchMode rowSearchMode)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			return this.LoadDbType (transaction, DbKey.Empty, rowSearchMode).ToArray ();
		}

		/// <summary>
		/// Creates an SQL field definining a constant value for a given column.
		/// The value is automatically converted from an ADO.NET representation
		/// to the internal data type.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="value">The ADO.NET compatible value.</param>
		/// <returns>The SQL field.</returns>
		public SqlField CreateSqlFieldFromAdoValue(DbColumn column, object value)
		{
			DbTypeDef type    = column.Type;
			DbRawType rawType = type.RawType;

			value = TypeConverter.ConvertToInternal (this.converter, value, rawType);
			
			SqlField field = SqlField.CreateConstant (value, rawType);
			field.Alias = column.Name;
			return field;
		}

		/// <summary>
		/// Creates an SQL field definining a constant value for a given column.
		/// The value is automatically converted from an ADO.NET representation
		/// to the internal data type.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <param name="value">The ADO.NET compatible value.</param>
		/// <returns>The SQL field.</returns>
		public SqlField CreateSqlFieldFromAdoValue(SqlColumn column, object value)
		{
			DbRawType rawType = column.Type;

			value = TypeConverter.ConvertToInternal (this.converter, value, rawType);

			SqlField field = SqlField.CreateConstant (value, rawType);
			field.Alias = column.Name;
			return field;
		}

		/// <summary>
		/// Creates an the empty SQL field defining a constant.
		/// </summary>
		/// <param name="column">The column.</param>
		/// <returns>The SQL field.</returns>
		public SqlField CreateEmptySqlField(DbColumn column)
		{
			SqlField field = SqlField.CreateConstant (null, column.Type.RawType);
			field.Alias = column.GetSqlName ();
			return field;
		}

		/// <summary>
		/// Creates a table definition with the minimum id, status and log columns.
		/// </summary>
		/// <param name="name">The table name.</param>
		/// <param name="category">The table category.</param>
		/// <param name="revisionMode">The table revision mode.</param>
		/// <param name="replicationMode">The table replication mode.</param>
		/// <returns></returns>
		internal DbTable CreateTable(string name, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
			DbTable table = new DbTable (name);

			this.DefineBasicTable (table, category, revisionMode, replicationMode);

			return table;
		}

		/// <summary>
		/// Creates a table definition with the minimum id, status and log columns.
		/// </summary>
		/// <param name="captionId">The table caption id.</param>
		/// <param name="category">The table category.</param>
		/// <param name="revisionMode">The table revision mode.</param>
		/// <param name="replicationMode">The table replication mode.</param>
		/// <returns></returns>
		internal DbTable CreateTable(Druid captionId, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
			DbTable table = new DbTable (captionId);

			this.DefineBasicTable (table, category, revisionMode, replicationMode);

			return table;
		}

		private void DefineBasicTable(DbTable table, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
			System.Diagnostics.Debug.Assert (revisionMode != DbRevisionMode.Unknown);
			System.Diagnostics.Debug.Assert (replicationMode != DbReplicationMode.Unknown);

			DbTypeDef typeDef = this.internalTypes[Tags.TypeKeyId];

			DbColumn colId   = new DbColumn (Tags.ColumnId, this.internalTypes[Tags.TypeKeyId], DbColumnClass.KeyId, DbElementCat.Internal, DbRevisionMode.Immutable);
			DbColumn colStat = new DbColumn (Tags.ColumnStatus, this.internalTypes[Tags.TypeKeyStatus], DbColumnClass.KeyStatus, DbElementCat.Internal);
			DbColumn colLog  = new DbColumn (Tags.ColumnRefLog, this.internalTypes[Tags.TypeKeyId], DbColumnClass.RefInternal, DbElementCat.Internal);

			table.DefineCategory (category);
			table.DefineRevisionMode (revisionMode);
			table.DefineReplicationMode (replicationMode);

			table.Columns.Add (colId);
			table.Columns.Add (colStat);
			table.Columns.Add (colLog);

			table.PrimaryKeys.Add (colId);
		}

		/// <summary>
		/// Registers a table for this database. This creates both the metadata and
		/// the database table itself (if needed), but does not initialize the
		/// relations between the columns and their target tables. See also the
		/// <see cref="RegisterColumnRelations"/> method.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="checkForKnownTable">If set to <c>true</c>, checks that the table is indeed known.</param>
		private void RegisterDbTable(DbTransaction transaction, DbTable table, bool checkForKnownTable)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			this.CheckForRegisteredTypes (transaction, table);
			
			if (checkForKnownTable)
			{
				this.CheckForKnownTable (transaction, table);
			}
			else
			{
				this.CheckForUnknownTable (transaction, table);
				
				long tableId  = this.NewRowIdInTable (transaction, this.internalTables[Tags.TableTableDef], 1);
				long columnId = this.NewRowIdInTable (transaction, this.internalTables[Tags.TableColumnDef], table.Columns.Count);
				
				//	Create the table description in the CR_TABLE_DEF table :
				
				table.DefineKey (new DbKey (tableId));
				table.UpdatePrimaryKeyInfo ();
				
				this.InsertTableDefRow (transaction, table);
				
				//	Create the column descriptions in the CR_COLUMN_DEF table :
				
				for (int i = 0; i < table.Columns.Count; i++)
				{
					table.Columns[i].DefineKey (new DbKey (columnId + i));
					this.InsertColumnDefRow (transaction, table, table.Columns[i]);
				}
			}
			
			//	Create the table itself :
			
			SqlTable sqlTable = table.CreateSqlTable (this.converter);
			
			transaction.SqlBuilder.InsertTable (sqlTable);
			this.ExecuteSilent (transaction);

			if (!string.IsNullOrEmpty (table.Comment))
			{
				transaction.SqlBuilder.SetTableComment (sqlTable.Name, sqlTable.Comment);
				this.ExecuteSilent (transaction);
			}

			foreach (SqlColumn column in sqlTable.Columns)
			{
				if (!string.IsNullOrEmpty (column.Comment))
				{
					transaction.SqlBuilder.SetTableColumnComment (sqlTable.Name, column.Name, column.Comment);
					this.ExecuteSilent (transaction);
				}
			}

			//	Create the revision tracking table, if needed :

			if (table.RevisionMode == DbRevisionMode.TrackChanges)
			{
				this.CreateRevisionTrackingTable (transaction, table);
			}

			//	Create relation tables if virtual columns exist in the source
			//	table :

			foreach (DbColumn column in table.Columns)
			{
				if (column.Cardinality != DbCardinality.None)
				{
					this.CreateRelationTable (transaction, table, column);
				}
			}
		}

		/// <summary>
		/// Creates the revision tracking table for a given table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		private void CreateRevisionTrackingTable(DbTransaction transaction, DbTable table)
		{
			SqlTable sqlTable;

			sqlTable = new SqlTable (table.GetRevisionTableName ());
			sqlTable.Comment = "Revision table for " + table.Comment;
			sqlTable.Columns.Add (new SqlColumn (Tags.ColumnRefId, DbKey.RawTypeForId, DbNullability.No));
			sqlTable.Columns.Add (new SqlColumn (Tags.ColumnRefModel, DbKey.RawTypeForId, DbNullability.No));

			transaction.SqlBuilder.InsertTable (sqlTable);
			this.ExecuteSilent (transaction);

			if (!string.IsNullOrEmpty (table.Comment))
			{
				transaction.SqlBuilder.SetTableComment (sqlTable.Name, sqlTable.Comment);
				this.ExecuteSilent (transaction);
			}
		}

		/// <summary>
		/// Creates the relation table for the specified source column.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="sourceTable">The source table.</param>
		/// <param name="sourceColumn">The source column.</param>
		private void CreateRelationTable(DbTransaction transaction, DbTable sourceTable, DbColumn sourceColumn)
		{
			DbTable relationTable = DbTable.CreateRelationTable (this, sourceTable, sourceColumn);

			this.RegisterNewDbTable (transaction, relationTable);
			/*
			transaction.SqlBuilder.InsertTable (relationTable.CreateSqlTable (this.converter));
			this.ExecuteSilent (transaction);
			 */
		}

		/// <summary>
		/// Checks that all types used by the column definitions for the specified
		/// table are properly registered. Otherwise, throws an exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// /// <exception cref="Exceptions.GenericException">Thrown if a type is not registered.</exception>
		private void CheckForRegisteredTypes(DbTransaction transaction, DbTable table)
		{
			foreach (DbColumn column in table.Columns)
			{
				if (column.Cardinality == DbCardinality.None)
				{
					DbTypeDef typeDef = column.Type;

					System.Diagnostics.Debug.Assert (typeDef != null);

					if (typeDef.Key.IsEmpty)
					{
						string message = string.Format ("Unregistered type '{0}' used in table '{1}', column '{2}'.",
							/* */						typeDef.Name, table.Name, column.Name);

						throw new Exceptions.GenericException (this.access, message);
					}
				}
			}
		}

		/// <summary>
		/// Checks that the specified type is not yet known. Otherwise, throws an
		/// exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		/// <exception cref="Exceptions.GenericException">Thrown if the type already exists.</exception>
		private void CheckForUnknownType(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (typeDef != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, typeDef.Name) > 0)
			{
				string message = string.Format ("Type {0} already exists in database.", typeDef.Name);
				throw new Exceptions.GenericException (this.access, message);
			}
		}

		/// <summary>
		/// Checks that the specified type is known. Otherwise, throws an exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		/// <exception cref="Exceptions.GenericException">Thrown if the type does not exist.</exception>
		private void CheckForKnownType(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (typeDef != null);
			
			if (this.CountMatchingRows (transaction, Tags.TableTypeDef, Tags.ColumnName, typeDef.Name) == 0)
			{
				string message = string.Format ("Type {0} does not exist in database.", typeDef.Name);
				throw new Exceptions.GenericException (this.access, message);
			}
		}

		/// <summary>
		/// Checks that the specified table is not yet known. Otherwise, throws an
		/// exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <exception cref="Exceptions.GenericException">Thrown if the table already exists.</exception>
		private void CheckForUnknownTable(DbTransaction transaction, DbTable table)
		{
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) > 0)
			{
				string message = string.Format ("Table {0} already exists in database.", table.Name);
				throw new Exceptions.GenericException (this.access, message);
			}
		}

		/// <summary>
		/// Checks that the specified table is known. Otherwise, throws an exception.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <exception cref="Exceptions.GenericException">Thrown if the table does not exist.</exception>
		private void CheckForKnownTable(DbTransaction transaction, DbTable table)
		{
			if (this.CountMatchingRows (transaction, Tags.TableTableDef, Tags.ColumnName, table.Name) == 0)
			{
				string message = string.Format ("Table {0} does not exist in database.", table.Name);
				throw new Exceptions.GenericException (this.access, message);
			}
		}

		/// <summary>
		/// Acquires the global lock on the database (this is connection
		/// independent; all database connections created through this
		/// <c>DbInfrastructure</c> will be locked).
		/// Neither <c>GlobalLock</c> nor <c>DatabaseLock</c> is allowed
		/// until <c>GlobalUnlcok</c> is called.
		/// </summary>
		/// <returns>An object which should be used in a <c>using</c> block.</returns>
		public System.IDisposable GlobalLock()
		{
			this.globalLock.AcquireWriterLock (this.lockTimeout);

			return new GlobalLockHelper (this);
		}

		/// <summary>
		/// Releases the global lock from the database. See <see cref="GlobalLock"/>.
		/// </summary>
		private void GlobalUnlock()
		{
			this.globalLock.ReleaseWriterLock ();
		}


		private class GlobalLockHelper : System.IDisposable
		{
			public GlobalLockHelper(DbInfrastructure infrastructure)
			{
				this.infrastructure = infrastructure;
			}

			~GlobalLockHelper()
			{
				throw new System.InvalidOperationException ("Caller of GlobalLock forgot to call Dispose");
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}

			#endregion

			private void Dispose(bool disposing)
			{
				this.infrastructure.GlobalUnlock ();
			}

			readonly DbInfrastructure infrastructure;
		}


		/// <summary>
		/// Locks a specific database connection. This prevents that the global
		/// lock gets locked until this database connection is unlocked again.
		/// </summary>
		/// <param name="database">The database abstraction.</param>
		internal void DatabaseLock(IDbAbstraction database)
		{
			this.globalLock.AcquireReaderLock (this.lockTimeout);
			
			if (System.Threading.Monitor.TryEnter (database, this.lockTimeout) == false)
			{
				this.globalLock.ReleaseReaderLock ();
				throw new Exceptions.DeadLockException (this.access, "Cannot lock database.");
			}
		}

		/// <summary>
		/// Unlocks a specific database connection.
		/// </summary>
		/// <param name="database">The database abstraction.</param>
		internal void DatabaseUnlock(IDbAbstraction database)
		{
			this.globalLock.ReleaseReaderLock ();
			System.Threading.Monitor.Exit (database);
		}

		/// <summary>
		/// Notifies that a transaction begins. This is called by <c>DbTransaction</c>
		/// when a new transaction object is created. Checks that there is at most
		/// one active transaction for every database abstraction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal void NotifyBeginTransaction(DbTransaction transaction)
		{
			IDbAbstraction abstraction = transaction.Database;
			
			lock (this.liveTransactions)
			{
				foreach (DbTransaction item in this.liveTransactions)
				{
					if (item.Database == abstraction)
					{
						throw new Exceptions.GenericException (this.access, string.Format ("Nested transactions not supported."));
					}
				}
				
				this.liveTransactions.Add (transaction);
			}
		}

		/// <summary>
		/// Notifies that the transaction ended. This is called by <c>DbTransaction</c>
		/// when a transaction is committed, rolled back or disposed.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal void NotifyEndTransaction(DbTransaction transaction)
		{
			IDbAbstraction abstraction = transaction.Database;
			
			this.DatabaseUnlock (abstraction);

			bool release = false;
			
			lock (this.liveTransactions)
			{
				if (this.liveTransactions.Remove (transaction) == false)
				{
					throw new Exceptions.GenericException (this.access, string.Format ("Ending wrong transaction."));
				}
				
				if (this.releaseRequested.Contains (abstraction))
				{
					this.releaseRequested.Remove (abstraction);
					release = true;
				}
			}

			if (release)
			{
				this.ReleaseConnection (abstraction);
			}
		}

		/// <summary>
		/// Finds the live transaction for the specified database abstraction.
		/// </summary>
		/// <param name="abstraction">The database abstraction.</param>
		/// <returns>The live transaction or <c>null</c>.</returns>
		internal DbTransaction FindLiveTransaction(IDbAbstraction abstraction)
		{
			lock (this.liveTransactions)
			{
				foreach (DbTransaction item in this.liveTransactions)
				{
					if (item.Database == abstraction)
					{
						return item;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Executes the specified rich command. This is called by <c>DbRichCommand</c>
		/// when the command is initially created through its <c>CreateFromTables</c>
		/// method.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="command">The command.</param>
		internal void Execute(DbTransaction transaction, DbRichCommand command)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			this.sqlEngine.Execute (command, this, transaction);
		}

		/// <summary>
		/// Silently executes the command attached to the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>Zero if no command was executed.</returns>
		public int ExecuteSilent(DbTransaction transaction)
		{
			return this.ExecuteSilent (transaction, transaction.SqlBuilder);
		}

		/// <summary>
		/// Silently executes the command defined by the SQL command builder.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <returns>Zero if no command was executed.</returns>
		public int ExecuteSilent(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return 0;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				int result;
				this.sqlEngine.Execute (command, DbCommandType.Silent, count, out result);
				return result;
			}
		}

		/// <summary>
		/// Executes the command attached to the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The first column of the first row returned by the command or <c>null</c>.</returns>
		public object ExecuteScalar(DbTransaction transaction)
		{
			return this.ExecuteScalar (transaction, transaction.SqlBuilder);
		}

		/// <summary>
		/// Executes the command defined by the SQL command builder.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <returns>The first column of the first row returned by the command or <c>null</c> if no command was executed.</returns>
		public object ExecuteScalar(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sqlEngine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}

		/// <summary>
		/// Executes the command attached to the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The number of rows affected or <c>null</c> if no command was executed.</returns>
		public object ExecuteNonQuery(DbTransaction transaction)
		{
			return this.ExecuteNonQuery (transaction, transaction.SqlBuilder);
		}

		/// <summary>
		/// Executes the command defined by the SQL command builder.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <returns>The number of rows affected or <c>null</c> if no command was executed.</returns>
		public object ExecuteNonQuery(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				object data;
				
				this.sqlEngine.Execute (command, DbCommandType.NonQuery, count, out data);
				
				return data;
			}
		}

		/// <summary>
		/// Executes the command attached to the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The data set or <c>null</c> if no command was executed.</returns>
		public System.Data.DataSet ExecuteRetData(DbTransaction transaction)
		{
			return this.ExecuteRetData (transaction, transaction.SqlBuilder);
		}
		
		/// <summary>
		/// Executes the command defined by the SQL command builder.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <returns>The data set or <c>null</c> if no command was executed.</returns>
		public System.Data.DataSet ExecuteRetData(DbTransaction transaction, ISqlBuilder builder)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			int count = builder.CommandCount;
			
			if (count < 1)
			{
				return null;
			}
			
			using (System.Data.IDbCommand command = builder.CreateCommand (transaction.Transaction))
			{
				System.Data.DataSet data;
				
				this.sqlEngine.Execute (command, DbCommandType.ReturningData, count, out data);
				
				return data;
			}
		}

		/// <summary>
		/// Executes a SELECT command.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="query">The SELECT query.</param>
		/// <param name="minRows">The minimum number of rows expected.</param>
		/// <returns>The data set.</returns>
		public System.Data.DataTable ExecuteSqlSelect(DbTransaction transaction, SqlSelect query, int minRows)
		{
			return this.ExecuteSqlSelect (transaction, transaction.SqlBuilder, query, minRows);
		}

		/// <summary>
		/// Executes a SELECT command.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <param name="query">The SELECT query.</param>
		/// <param name="minRows">The minimum number of rows expected.</param>
		/// <returns>The data set.</returns>
		/// <exception cref="Exceptions.GenericException">Thrown if the query failed or returned less rows than expected.</exception>
		public System.Data.DataTable ExecuteSqlSelect(DbTransaction transaction, ISqlBuilder builder, SqlSelect query, int minRows)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (builder != null);
			
			builder.SelectData (query);
			
			System.Data.DataSet dataSet;
			System.Data.DataTable dataTable;
			
			dataSet = this.ExecuteRetData (transaction);
			
			if ((dataSet == null) ||
				(dataSet.Tables.Count != 1))
			{
				throw new Exceptions.GenericException (this.access, string.Format ("Query failed"));
			}
			
			dataTable = dataSet.Tables[0];
			
			if (dataTable.Rows.Count < minRows)
			{
				throw new Exceptions.GenericException (this.access, string.Format ("Query returned too few rows; expected {0}, found {1}", minRows, dataTable.Rows.Count));
			}
			
			return dataTable;
		}

		/// <summary>
		/// Finds the key for the specified table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The table name.</param>
		/// <returns>The key to the table metadata.</returns>
		public DbKey FindDbTableKey(DbTransaction transaction, string name)
		{
			return this.FindLiveKey (this.FindDbKeys (transaction, Tags.TableTableDef, name));
		}

		/// <summary>
		/// Finds the key for the specified type.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The type name.</param>
		/// <returns>The key to the type metadata.</returns>
		public DbKey FindDbTypeKey(DbTransaction transaction, string name)
		{
			return this.FindLiveKey (this.FindDbKeys (transaction, Tags.TableTypeDef, name));
		}

		/// <summary>
		/// Finds the first live key in the collection.
		/// </summary>
		/// <param name="keys">The keys.</param>
		/// <returns>The live key or <c>DbKey.Empty</c>.</returns>
		internal DbKey FindLiveKey(IEnumerable<DbKey> keys)
		{
			foreach (DbKey key in keys)
			{
				switch (key.Status)
				{
					case DbRowStatus.Live:
					case DbRowStatus.Copied:
						return key;
				}
			}
			
			return DbKey.Empty;
		}

		/// <summary>
		/// Finds the keys for the named rows in the specified table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="rowName">Name of the row or rows.</param>
		/// <returns>The keys.</returns>
		internal IEnumerable<DbKey> FindDbKeys(DbTransaction transaction, string tableName, string rowName)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", Tags.ColumnId));
			query.Fields.Add ("T_STAT",	SqlField.CreateName ("T", Tags.ColumnStatus));
			
			query.Tables.Add ("T", SqlField.CreateName (tableName));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, SqlField.CreateName ("T", Tags.ColumnName), SqlField.CreateConstant (rowName, DbRawType.String)));
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 0);
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long  id     = InvariantConverter.ToLong (row["T_ID"]);
				short status = InvariantConverter.ToShort (row["T_STAT"]);
				
				yield return new DbKey (id, DbKey.ConvertFromIntStatus (status));
			}
		}

		/// <summary>
		/// Counts the rows of the specified table which have a matching value in
		/// a given column.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="nameColumn">Name of the column.</param>
		/// <param name="value">The value.</param>
		/// <returns>The number of matching rows.</returns>
		public int CountMatchingRows(DbTransaction transaction, string tableName, string nameColumn, string value)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("N", new SqlAggregate (SqlAggregateFunction.Count, SqlField.CreateAll ()));
			query.Tables.Add ("T", SqlField.CreateName (tableName));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, SqlField.CreateName ("T", nameColumn), SqlField.CreateConstant (value, DbRawType.String)));

			DbInfrastructure.AddKeyExtraction (query.Conditions, "T", DbRowSearchMode.LiveActive);
			
			transaction.SqlBuilder.SelectData (query);
			
			return InvariantConverter.ToInt (this.ExecuteScalar (transaction));
		}

		/// <summary>
		/// Updates the specified row to use a new key.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="oldKey">The old key of the row.</param>
		/// <param name="newKey">The new key of the row.</param>
		private void UpdateKeyInRow(DbTransaction transaction, string tableName, DbKey oldKey, DbKey newKey)
		{
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();
			Collections.SqlFieldList conds  = new Collections.SqlFieldList ();
			
			fields.Add (Tags.ColumnId,     SqlField.CreateConstant (newKey.Id,             DbKey.RawTypeForId));
			fields.Add (Tags.ColumnStatus, SqlField.CreateConstant (newKey.IntStatus,      DbKey.RawTypeForStatus));
			fields.Add (Tags.ColumnRefLog, SqlField.CreateConstant (this.logger.CurrentId, DbKey.RawTypeForId));
			
			DbInfrastructure.AddKeyExtraction (conds, tableName, oldKey);
			
			transaction.SqlBuilder.UpdateData (tableName, fields, conds);
			
			int numRowsAffected = InvariantConverter.ToInt (this.ExecuteNonQuery (transaction));
			
			if (numRowsAffected != 1)
			{
				throw new Exceptions.GenericException (this.access, string.Format ("Update of row {0} in table {1} produced {2} updates.", oldKey, tableName, numRowsAffected));
			}
		}

		/// <summary>
		/// Updates the next column id in the table definition metadata. This
		/// is used when a new column is created to derive the column key.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="nextId">The next column id.</param>
		internal void UpdateTableNextId(DbTransaction transaction, DbTable table, DbId nextId)
		{
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();
			Collections.SqlFieldList conds  = new Collections.SqlFieldList ();
			
			fields.Add (Tags.ColumnNextId, SqlField.CreateConstant (nextId, DbKey.RawTypeForId));
			
			DbInfrastructure.AddKeyExtraction (conds, Tags.TableTableDef, table.Key);
			
			transaction.SqlBuilder.UpdateData (Tags.TableTableDef, fields, conds);
			this.ExecuteSilent (transaction);
		}

		/// <summary>
		/// Loads the table definitions based on the metadata table key and the
		/// specified search mode.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="key">The table key or <c>DbKey.Empty</c> to load all table definitions based on the search mode.</param>
		/// <param name="rowSearchMode">The search mode (live, deleted, etc.) if the key is set to <c>DbKey.Empty</c>, ignored otherwise.</param>
		/// <returns>The table definitions.</returns>
		public List<DbTable> LoadDbTable(DbTransaction transaction, DbKey key, DbRowSearchMode rowSearchMode)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			//	We will build a join in order to query both the table definition
			//	and the column definitions with a single SQL request.
			
			SqlSelect query = new SqlSelect ();
			
			//	Table related informations :
			
			query.Fields.Add ("T_ID",     SqlField.CreateName ("T_TABLE", Tags.ColumnId));
			query.Fields.Add ("T_NAME",   SqlField.CreateName ("T_TABLE", Tags.ColumnName));
			query.Fields.Add ("T_D_NAME", SqlField.CreateName ("T_TABLE", Tags.ColumnDisplayName));
			query.Fields.Add ("T_INFO",   SqlField.CreateName ("T_TABLE", Tags.ColumnInfoXml));
			
			//	Column related informations :
			
			query.Fields.Add ("C_ID",     SqlField.CreateName ("T_COLUMN", Tags.ColumnId));
			query.Fields.Add ("C_NAME",   SqlField.CreateName ("T_COLUMN", Tags.ColumnName));
			query.Fields.Add ("C_D_NAME", SqlField.CreateName ("T_COLUMN", Tags.ColumnDisplayName));
			query.Fields.Add ("C_INFO",   SqlField.CreateName ("T_COLUMN", Tags.ColumnInfoXml));
			query.Fields.Add ("C_TYPE",   SqlField.CreateName ("T_COLUMN", Tags.ColumnRefType));
			query.Fields.Add ("C_TARGET", SqlField.CreateName ("T_COLUMN", Tags.ColumnRefTarget));
			
			//	Tables to query :
			
			query.Tables.Add ("T_TABLE",  SqlField.CreateName (Tags.TableTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (Tags.TableColumnDef));
			
			if (key.IsEmpty)
			{
				//	Extract all tables and columns...
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TABLE", rowSearchMode);
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, "T_TABLE");
			}
			else
			{
				//	Extract only matching tables...
				
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TABLE", key);
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, key);
			}
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 1);
			
			long          rowId   = -1;
			List<DbTable> tables  = new List<DbTable> ();
			DbTable		  dbTable = null;
			bool          recycle = false;
			
			//	Analyse the returned rows which are expected to be sorted first
			//	by table definitions and second by column definitions.
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long currentRowId = InvariantConverter.ToLong (row["T_ID"]);
				
				if (rowId != currentRowId)
				{
					//	Found a new table definition :
					
					rowId   = currentRowId;
					dbTable = null;
					
					string tableInfo = InvariantConverter.ToString (row["T_INFO"]);
					string tableName = InvariantConverter.ToString (row["T_NAME"]);
					string tableDisplayName = InvariantConverter.ToString (row["T_D_NAME"]);
					DbKey  tableKey  = key.IsEmpty ? new DbKey (rowId) : key;
					
					dbTable = this.tableCache[tableKey];
					
					if (dbTable == null)
					{
						//	The table is not yet loaded in the cache, so deserialize
						//	it and initialize it, then put it into the cache :
						
						dbTable = DbTools.DeserializeFromXml<DbTable> (tableInfo);
						recycle = false;

						dbTable.DefineDisplayName (tableDisplayName);
						dbTable.DefineKey (tableKey);
						
						if ((tableKey.Status == DbRowStatus.Live) ||
							(tableKey.Status == DbRowStatus.Copied))
						{
							this.tableCache[tableKey] = dbTable;
						}
					}
					else
					{
						System.Diagnostics.Debug.WriteLine (string.Format ("Recycling known table {0}", dbTable.Name));
						recycle = true;
					}
					
					tables.Add (dbTable);
				}
				
				if (recycle)
				{
					continue;
				}
				
				//	Every row defines one column :

				long   typeDefId  = InvariantConverter.ToLong (row["C_TYPE"]);
				long   columnId   = InvariantConverter.ToLong (row["C_ID"]);
				string columnName = InvariantConverter.ToString (row["C_NAME"]);
				string columnDisplayName = InvariantConverter.ToString (row["C_D_NAME"]);
				string columnInfo = InvariantConverter.ToString (row["C_INFO"]);
				string targetName = null;

				if (InvariantConverter.IsNotNull (row["C_TARGET"]))
				{
					//	Resolve the reference to the target table; this won't
					//	produce an endless loop if both tables refer to each
					//	other, as the tables get cached even if they are not
					//	yet fully initialized...
					
					DbKey   targetKey   = new DbKey (InvariantConverter.ToLong (row["C_TARGET"]));
					DbTable targetTable = this.ResolveDbTable (transaction, targetKey);

					targetName = targetTable.Name;
				}

				DbKey     typeDefKey = new DbKey (typeDefId);
				DbTypeDef typeDef    = typeDefId == 0 ? null : this.ResolveDbType (transaction, typeDefKey);

				if (typeDefId != 0)
				{
					if (typeDef == null)
					{
						throw new Exceptions.GenericException (this.access, string.Format ("Missing type for column '{0}' in table '{1}'", columnName, dbTable.Name));
					}

					System.Diagnostics.Debug.Assert (typeDef.Key == typeDefKey);
				}

				DbColumn dbColumn = DbTools.DeserializeFromXml<DbColumn> (columnInfo);

				dbColumn.DefineDisplayName (columnDisplayName);
				dbColumn.DefineKey (new DbKey (columnId));
				dbColumn.DefineType (typeDef);
				dbColumn.DefineTargetTableName (targetName ?? dbColumn.TargetTableName);
				
				dbTable.Columns.Add (dbColumn);
				
				if (dbColumn.IsPrimaryKey)
				{
					dbTable.PrimaryKeys.Add (dbColumn);
				}
			}
			
			return tables;
		}

		/// <summary>
		/// Loads the type definitions based on the metadata type key and the
		/// specified search mode.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeKey">The type key or <c>DbKey.Empty</c> to load all type definitions based on the search mode.</param>
		/// <param name="rowSearchMode">The search mode (live, deleted, etc.) if the key is set to <c>DbKey.Empty</c>, ignored otherwise.</param>
		/// <returns>The type definitions.</returns>
		public List<DbTypeDef> LoadDbType(DbTransaction transaction, DbKey typeKey, DbRowSearchMode rowSearchMode)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",     SqlField.CreateName ("T_TYPE", Tags.ColumnId));
			query.Fields.Add ("T_NAME",   SqlField.CreateName ("T_TYPE", Tags.ColumnName));
			query.Fields.Add ("T_D_NAME", SqlField.CreateName ("T_TYPE", Tags.ColumnDisplayName));
			query.Fields.Add ("T_INFO",   SqlField.CreateName ("T_TYPE", Tags.ColumnInfoXml));
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (Tags.TableTypeDef));
			
			if (typeKey.IsEmpty)
			{
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TYPE", rowSearchMode);
			}
			else
			{
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TYPE", typeKey);
			}
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 1);
			List<DbTypeDef> types = new List<DbTypeDef> ();
			
			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long   typeId          = InvariantConverter.ToLong (row["T_ID"]);
				string typeName        = InvariantConverter.ToString (row["T_NAME"]);
				string typeDisplayName = InvariantConverter.ToString (row["T_D_NAME"]);
				string typeInfo        = InvariantConverter.ToString (row["T_INFO"]);
				
				DbTypeDef typeDef = this.typeCache[typeKey];

				if (typeDef == null)
				{
					typeDef = DbTools.DeserializeFromXml<DbTypeDef> (typeInfo);

					typeDef.DefineDisplayName (typeDisplayName);
					typeDef.DefineKey (new DbKey (typeId));

					this.typeCache[typeKey] = typeDef;
				}
				
				types.Add (typeDef);
			}
			
			return types;
		}

		/// <summary>
		/// Gets the next row id in the specified table. This does not allocate
		/// any keys.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <returns>The next row id.</returns>
		internal long NextRowIdInTable(DbTransaction transaction, DbTable table)
		{
			return this.NewRowIdInTable (transaction, table, 0);
		}

		/// <summary>
		/// Gets the first available row id in the specified table and allocates
		/// room for the specified number of keys.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="numKeys">The number of keys to allocate.</param>
		/// <returns>The first row id of a contiguous range.</returns>
		public long NewRowIdInTable(DbTransaction transaction, DbTable table, int numKeys)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (numKeys >= 0);
			
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();
			Collections.SqlFieldList conds  = new Collections.SqlFieldList ();
			
			SqlField fieldNextId = SqlField.CreateName (Tags.ColumnNextId);
			SqlField fieldConstN = SqlField.CreateConstant (numKeys, DbRawType.Int32);
			
			fields.Add (Tags.ColumnNextId, new SqlFunction (SqlFunctionCode.MathAdd, fieldNextId, fieldConstN));
			
			DbInfrastructure.AddKeyExtraction (conds, Tags.TableTableDef, table.Key);
			
			if (numKeys > 0)
			{
				transaction.SqlBuilder.UpdateData (Tags.TableTableDef, fields, conds);
				this.ExecuteSilent (transaction);
			}
			
			SqlSelect query = new SqlSelect ();

			System.Diagnostics.Debug.Assert (conds.Count == 1);

			query.Fields.Add (fieldNextId);
			query.Tables.Add (SqlField.CreateName (Tags.TableTableDef));
			query.Conditions.Add (conds[0]);
			
			transaction.SqlBuilder.SelectData (query);
			
			return InvariantConverter.ToLong (this.ExecuteScalar (transaction)) - numKeys;
		}

		/// <summary>
		/// Adds a SELECT extraction condition for a key in a table.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		private static void AddKeyExtraction(Collections.SqlFieldList conditions, string tableName, DbKey key)
		{
			SqlField nameColId  = SqlField.CreateName (tableName, Tags.ColumnId);
			SqlField constantId = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, nameColId, constantId));
		}

		/// <summary>
		/// Adds a SELECT extraction condition for a key in a target table
		/// matching a foreign key defined by the source table and column.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="sourceTableName">Name of the source table.</param>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <param name="targetTableName">Name of the target table.</param>
		private static void AddKeyExtraction(Collections.SqlFieldList conditions, string sourceTableName, string sourceColumnName, string targetTableName)
		{
			SqlField targetColumnId = SqlField.CreateName (targetTableName, Tags.ColumnId);
			SqlField sourceColumnId = SqlField.CreateName (sourceTableName, sourceColumnName);

			conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, sourceColumnId, targetColumnId));
		}

		/// <summary>
		/// Adds a SELECT extraction condition for a key matching a foreign
		/// key defined by the source table and column.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="sourceTableName">Name of the source table.</param>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <param name="key">The key.</param>
		private static void AddKeyExtraction(Collections.SqlFieldList conditions, string sourceTableName, string sourceColumnName, DbKey key)
		{
			SqlField sourceColId = SqlField.CreateName (sourceTableName, sourceColumnName);
			SqlField constantId  = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, sourceColId, constantId));
		}

		/// <summary>
		/// Adds a SELECT extraction condition for any keys in a table matching
		/// the search mode (live, deleted, etc.)
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="searchMode">The search mode (live, deleted, etc.).</param>
		private static void AddKeyExtraction(Collections.SqlFieldList conditions, string tableName, DbRowSearchMode searchMode)
		{
			SqlFunctionCode function;
			DbRowStatus     status;
			
			//	See the definitions of DbRowStatus and DbRowSearchMode...
			
			switch (searchMode)
			{
				case DbRowSearchMode.Copied:		status = DbRowStatus.Copied;		function = SqlFunctionCode.CompareEqual;	break;
				case DbRowSearchMode.Live:			status = DbRowStatus.Live;			function = SqlFunctionCode.CompareEqual;	break;
				case DbRowSearchMode.LiveActive:	status = DbRowStatus.ArchiveCopy;	function = SqlFunctionCode.CompareLessThan;	break;
				case DbRowSearchMode.ArchiveCopy:	status = DbRowStatus.ArchiveCopy;	function = SqlFunctionCode.CompareEqual;	break;
				case DbRowSearchMode.LiveAll:		status = DbRowStatus.Deleted;		function = SqlFunctionCode.CompareLessThan;	break;
				case DbRowSearchMode.Deleted:		status = DbRowStatus.Deleted;		function = SqlFunctionCode.CompareEqual;	break;
				
				case DbRowSearchMode.All:
					return;
				
				default:
					throw new System.ArgumentException (string.Format ("Search mode {0} not supported", searchMode), "searchMode");
			}
			
			SqlField nameStatus  = SqlField.CreateName (tableName, Tags.ColumnStatus);
			SqlField constStatus = SqlField.CreateConstant (DbKey.ConvertToIntStatus (status), DbKey.RawTypeForStatus);
			
			conditions.Add (new SqlFunction (function, nameStatus, constStatus));
		}

		/// <summary>
		/// Starts using the database. This loads the global and local settings
		/// and instanciates the client manager.
		/// </summary>
		private void StartUsingDatabase()
		{
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				this.globals = new Settings.Globals (this, transaction);
				this.locals  = new Settings.Locals (this, transaction);
				
				this.clientId = this.locals.ClientId;
				this.SetupLogger (transaction);

				this.clientManager = new DbClientManager ();

				this.clientManager.Attach (this, this.ResolveDbTable (transaction, Tags.TableClientDef));
				this.clientManager.LoadFromBase (transaction);
				
				transaction.Commit ();
			}
		}

		/// <summary>
		/// Sets up the database logger.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		private void SetupLogger(DbTransaction transaction)
		{
			this.logger = new DbLogger ();
			this.logger.DefineClientId (this.clientId);
			this.logger.Attach (this, this.internalTables[Tags.TableLog]);
			this.logger.ResetCurrentLogId (transaction);
		}

		/// <summary>
		/// Sets up the metadata table definitions by filling them with the
		/// defaults required by an empty database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		private void SetupTables(DbTransaction transaction)
		{
			long typeKeyId   = DbId.CreateId (1, this.clientId);
			long tableKeyId  = DbId.CreateId (1, this.clientId);
			long columnKeyId = DbId.CreateId (1, this.clientId);
			long clientKeyId = DbId.CreateId (1, this.clientId);

			System.Diagnostics.Debug.Assert (this.logger == null);
			
			//	Minimal logger definition. We must be able to access to the
			//	logger's CurrentId property, that's all :
			
			this.logger = new DbLogger ();
			this.logger.DefineClientId (this.clientId);
			this.logger.DefineInitialLogId (1);
			
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId == 1);
			
			//	First, fill the type table so that we can reference them from
			//	the column definition table :
			
			foreach (DbTypeDef typeDef in this.internalTypes)
			{
				typeDef.DefineKey (new DbKey (typeKeyId++));
				this.InsertTypeDefRow (transaction, typeDef);
			}
			
			//	Then, fill the table definition table and the column definition
			//	table :
			
			foreach (DbTable table in this.internalTables)
			{
				table.DefineKey (new DbKey (tableKeyId++));
				table.UpdatePrimaryKeyInfo ();
				
				this.InsertTableDefRow (transaction, table);
				
				foreach (DbColumn column in table.Columns)
				{
					column.DefineKey (new DbKey (columnKeyId++));
					this.InsertColumnDefRow (transaction, table, column);
				}
			}
			
			//	At last, fill in the relations :
			//	
			//	- A column definition refers to its containing table definition.
			//	- A column definition refers to a type definition.
			//	- A column definition refers to a target table definition if
			//	  this is a foreign key.
			
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef, Tags.ColumnRefTable,  Tags.TableTableDef);
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef, Tags.ColumnRefType,   Tags.TableTypeDef);
			this.UpdateColumnRelation (transaction, Tags.TableColumnDef, Tags.ColumnRefTarget, Tags.TableTableDef);
			
			this.UpdateTableNextId (transaction, this.internalTables[Tags.TableTableDef], tableKeyId);
			this.UpdateTableNextId (transaction, this.internalTables[Tags.TableColumnDef], columnKeyId);
			this.UpdateTableNextId (transaction, this.internalTables[Tags.TableTypeDef], typeKeyId);
			this.UpdateTableNextId (transaction, this.internalTables[Tags.TableClientDef], clientKeyId);
			
			//	Now that everything is properly defined, we may attach the
			//	logger to the database :
			
			this.logger.Attach (this, this.internalTables[Tags.TableLog]);
			this.logger.CreateInitialEntry (transaction);
			
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId == 1);
			System.Diagnostics.Debug.Assert (this.NextRowIdInTable (transaction, this.internalTables[Tags.TableLog]) == DbId.CreateId (2, this.clientId));
		}

		/// <summary>
		/// Updates the column relation information. This will record the relation
		/// in the CR_COLUMN_DEF table by specifying the target table for a given
		/// source column.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="sourceTableKey">The source table key.</param>
		/// <param name="sourceColumnKey">The source column key.</param>
		/// <param name="targetTableKey">The target table key.</param>
		private void UpdateColumnRelation(DbTransaction transaction, DbKey sourceTableKey, DbKey sourceColumnKey, DbKey targetTableKey)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			System.Diagnostics.Debug.Assert (sourceTableKey  != null);
			System.Diagnostics.Debug.Assert (sourceColumnKey != null);
			System.Diagnostics.Debug.Assert (targetTableKey  != null);
			
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();
			Collections.SqlFieldList conds  = new Collections.SqlFieldList ();
			
			fields.Add (Tags.ColumnRefTarget, SqlField.CreateConstant (targetTableKey.Id, DbKey.RawTypeForId));

			DbInfrastructure.AddKeyExtraction (conds, Tags.TableColumnDef, sourceColumnKey);
			
			transaction.SqlBuilder.UpdateData (Tags.TableColumnDef, fields, conds);
			this.ExecuteSilent (transaction);
		}

		/// <summary>
		/// Updates the column relation information. This will record the relation
		/// in the CR_COLUMN_DEF table by specifying the target table for a given
		/// source column.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="sourceTableName">Name of the source table.</param>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <param name="targetTableName">Name of the target table.</param>
		private void UpdateColumnRelation(DbTransaction transaction, string sourceTableName, string sourceColumnName, string targetTableName)
		{
			DbTable  source = this.internalTables[sourceTableName];
			DbTable  target = this.internalTables[targetTableName];
			DbColumn column = source.Columns[sourceColumnName];
			
			this.UpdateColumnRelation (transaction, source.Key, column.Key, target.Key);
		}


		/// <summary>
		/// Inserts a type definition row into the CR_TYPE_DEF table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		private void InsertTypeDefRow(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			
			DbTable typeDefTable = this.internalTables[Tags.TableTypeDef];
			
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();

			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnId],          typeDef.Key.Id));
			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnStatus],      typeDef.Key.IntStatus));
			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnRefLog],      this.logger.CurrentId));
			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnName],        typeDef.Name));
			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnDisplayName], typeDef.DisplayName));
			fields.Add (this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnInfoXml],     DbTools.GetCompactXml (typeDef)));
			
			transaction.SqlBuilder.InsertData (typeDefTable.GetSqlName (), fields);
			this.ExecuteSilent (transaction);
		}

		/// <summary>
		/// Inserts a table definition row into the CR_TABLE_DEF table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		private void InsertTableDefRow(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			
			DbTable tableDefTable = this.internalTables[Tags.TableTableDef];
			
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();

			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnId],		    table.Key.Id));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnStatus],      table.Key.IntStatus));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnRefLog],      this.logger.CurrentId));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnName],        table.Name));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnDisplayName], table.DisplayName));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnInfoXml],     DbTools.GetCompactXml (table)));
			fields.Add (this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnNextId],      DbId.CreateId (1, this.clientId)));
			
			transaction.SqlBuilder.InsertData (tableDefTable.GetSqlName (), fields);
			this.ExecuteSilent (transaction);
		}

		/// <summary>
		/// Inserts a column definition row into the CR_COLUMN_DEF table. This
		/// does not generate the relation between the column and the target
		/// table, if any.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		/// <param name="column">The column definition.</param>
		private void InsertColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (this.logger.CurrentId.LocalId > 0);
			
			DbTable columnDefTable = this.internalTables[Tags.TableColumnDef];
			
			Collections.SqlFieldList fields = new Collections.SqlFieldList ();

			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnId],          column.Key.Id));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnStatus],      column.Key.IntStatus));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefLog],      this.logger.CurrentId));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnName],        column.Name));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnDisplayName], column.DisplayName));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnInfoXml],     DbTools.GetCompactXml (column)));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefTable],    table.Key.Id));
			fields.Add (this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefType],     column.Type == null ? 0 : column.Type.Key.Id));
			
			transaction.SqlBuilder.InsertData (columnDefTable.GetSqlName (), fields);
			this.ExecuteSilent (transaction);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.globalLock != null)
				{
					this.globalLock.ReleaseLock ();
					this.globalLock = null;
				}
				
				if (this.logger != null)
				{
					this.logger.Detach ();
					this.logger = null;
				}
				
				if (this.abstraction != null)
				{
					this.abstraction.Dispose ();
					
					System.Diagnostics.Debug.Assert (this.abstraction.IsConnectionOpen == false);
					
					this.abstraction = null;
					this.sqlEngine   = null;
					this.converter   = null;
				}
			}
			
			System.Diagnostics.Debug.Assert (this.sqlEngine == null);
			System.Diagnostics.Debug.Assert (this.converter == null);
		}

		#region Initialisation

		/// <summary>
		/// Initializes the database abstraction.
		/// </summary>
		/// <returns><c>true</c> if the connection was successfully established;
		/// otherwise, <c>false</c>.</returns>
		private bool InitializeDatabaseAbstraction()
		{
			this.abstraction = this.CreateDatabaseAbstraction ();

			if (this.abstraction == null)
			{
				return false;
			}
			else
			{
				this.types       = new TypeHelper (this);
				this.sqlEngine   = this.abstraction.SqlEngine;
				this.converter   = this.abstraction.Factory.TypeConverter;

				System.Diagnostics.Debug.Assert (this.sqlEngine != null);
				System.Diagnostics.Debug.Assert (this.converter != null);

				this.abstraction.SqlBuilder.AutoClear = true;

				return true;
			}
		}

		#endregion
		
		#region BootHelper Class
		
		private sealed class BootHelper
		{
			public BootHelper(DbInfrastructure infrastructure, DbTransaction transaction)
			{
				this.infrastructure = infrastructure;
				this.transaction    = transaction;

				System.Diagnostics.Debug.Assert (this.infrastructure.types.KeyId.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.KeyStatus.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.NullableKeyId.IsNullable == true);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.Name.IsNullable == false);
				System.Diagnostics.Debug.Assert (this.infrastructure.types.InfoXml.IsNullable == false);
			}
			
			public void CreateTableTableDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableTableDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 DbColumnClass.KeyId,		DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 DbColumnClass.KeyStatus,	DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 DbColumnClass.RefInternal, DbElementCat.Internal),
						new DbColumn (Tags.ColumnName,		  types.Name,		 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnDisplayName, types.Name,		 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnNextId,	  types.KeyId,		 DbColumnClass.RefInternal, DbElementCat.Internal)
					};

				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableColumnDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableColumnDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		   DbColumnClass.KeyId,		  DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	   DbColumnClass.KeyStatus,   DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		   DbColumnClass.RefInternal, DbElementCat.Internal),
						new DbColumn (Tags.ColumnName,		  types.Name,		   DbColumnClass.Data,		  DbElementCat.Internal),
						new DbColumn (Tags.ColumnDisplayName, types.Name,		   DbColumnClass.Data,        DbElementCat.Internal),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	   DbColumnClass.Data,		  DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefTable,	  types.KeyId,         DbColumnClass.RefId,		  DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefType,	  types.KeyId,         DbColumnClass.RefId,		  DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefTarget,	  types.NullableKeyId, DbColumnClass.RefId,		  DbElementCat.Internal)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableTypeDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableTypeDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		 DbColumnClass.KeyId,		DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	 DbColumnClass.KeyStatus,	DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		 DbColumnClass.RefInternal, DbElementCat.Internal),
						new DbColumn (Tags.ColumnName,		  types.Name,		 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnDisplayName, types.Name,		 DbColumnClass.Data,        DbElementCat.Internal),
						new DbColumn (Tags.ColumnInfoXml,	  types.InfoXml,	 DbColumnClass.Data,		DbElementCat.Internal)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableLog()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableLog);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		DbColumnClass.KeyId, DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnDateTime,	  types.DateTime,	DbColumnClass.Data,  DbElementCat.Internal, DbRevisionMode.Immutable)
					};
				
				//	TODO: add a column recording the nature of the change and the author of the change...
				
				this.CreateTable (table, columns, DbReplicationMode.Manual);
			}
			
			public void CreateTableRequestQueue()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableRequestQueue);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,		  types.KeyId,		  DbColumnClass.KeyId,		 DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnStatus,	  types.KeyStatus,	  DbColumnClass.KeyStatus,	 DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefLog,	  types.KeyId,		  DbColumnClass.RefInternal, DbElementCat.Internal),
						new DbColumn (Tags.ColumnReqExState,  types.ReqExecState, DbColumnClass.Data,		 DbElementCat.Internal),
						new DbColumn (Tags.ColumnReqData,	  types.ReqData,	  DbColumnClass.Data,		 DbElementCat.Internal),
						new DbColumn (Tags.ColumnDateTime,    types.DateTime,     DbColumnClass.Data,		 DbElementCat.Internal)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.None);
			}
			
			public void CreateTableClientDef()
			{
				TypeHelper types   = this.infrastructure.types;
				DbTable    table   = new DbTable (Tags.TableClientDef);
				DbColumn[] columns = new DbColumn[]
					{
						new DbColumn (Tags.ColumnId,			types.KeyId,	 DbColumnClass.KeyId,		DbElementCat.Internal, DbRevisionMode.Immutable),
						new DbColumn (Tags.ColumnStatus,		types.KeyStatus, DbColumnClass.KeyStatus,	DbElementCat.Internal),
						new DbColumn (Tags.ColumnRefLog,		types.KeyId,	 DbColumnClass.RefInternal, DbElementCat.Internal),
						new DbColumn (Tags.ColumnClientId,		types.KeyId,	 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnClientName,	types.Name,      DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnClientSync,	types.KeyId,	 DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnClientCreDate,	types.DateTime,  DbColumnClass.Data,		DbElementCat.Internal),
						new DbColumn (Tags.ColumnClientConDate,	types.DateTime,  DbColumnClass.Data,		DbElementCat.Internal)
					};
				
				this.CreateTable (table, columns, DbReplicationMode.None);
			}
			
			private void CreateTable(DbTable table, DbColumn[] columns, DbReplicationMode replicationMode)
			{
				for (int i = 0; i < columns.Length; i++)
				{
					columns[i].DefineCategory (DbElementCat.Internal);
				}
				
				table.Columns.AddRange (columns);
				
				table.DefineCategory (DbElementCat.Internal);
				table.DefinePrimaryKey (columns[0]);
				table.DefineReplicationMode (replicationMode);
				
				this.infrastructure.internalTables.Add (table);
				
				SqlTable sqlTable = table.CreateSqlTable (this.infrastructure.converter);
				this.infrastructure.DefaultSqlBuilder.InsertTable (sqlTable);
				this.infrastructure.ExecuteSilent (this.transaction);
			}
		
			private DbInfrastructure			infrastructure;
			private DbTransaction				transaction;
		}
		
		#endregion
		
		#region TypeHelper Class
		
		private sealed class TypeHelper
		{
			public TypeHelper(DbInfrastructure infrastructure)
			{
				this.infrastructure = infrastructure;
			}
			
			public DbTypeDef					KeyId
			{
				get
				{
					return this.numTypeKeyId;
				}
			}

			public DbTypeDef					NullableKeyId
			{
				get
				{
					return this.numTypeNullableKeyId;
				}
			}

			public DbTypeDef					KeyStatus
			{
				get
				{
					return this.numTypeKeyStatus;
				}
			}

			public DbTypeDef					ReqExecState
			{
				get
				{
					return this.numTypeReqExState;
				}
			}

			public DbTypeDef					CollectionRank
			{
				get
				{
					return this.numTypeCollectionRank;
				}
			}


			public DbTypeDef					DateTime
			{
				get
				{
					return this.otherTypeDateTime;
				}
			}

			public DbTypeDef					ReqData
			{
				get
				{
					return this.otherTypeReqData;
				}
			}

			public DbTypeDef					Name
			{
				get
				{
					return this.strTypeName;
				}
			}

			public DbTypeDef					InfoXml
			{
				get
				{
					return this.strTypeInfoXml;
				}
			}

			public DbTypeDef					DictKey
			{
				get
				{
					return this.strTypeDictKey;
				}
			}

			public DbTypeDef					DictValue
			{
				get
				{
					return this.strTypeDictValue;
				}
			}

			public void RegisterTypes()
			{
				this.InitializeNumTypes ();
				this.InitializeStrTypes ();
				this.InitializeOtherTypes ();
				
				this.AssertAllTypesReady ();
			}
			
			public void ResolveTypes(DbTransaction transaction)
			{
				this.numTypeKeyId          = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyId);
				this.numTypeNullableKeyId  = this.infrastructure.ResolveDbType (transaction, Tags.TypeNullableKeyId);
				this.numTypeKeyStatus      = this.infrastructure.ResolveDbType (transaction, Tags.TypeKeyStatus);
				this.numTypeReqExState     = this.infrastructure.ResolveDbType (transaction, Tags.TypeReqExState);
				this.numTypeCollectionRank = this.infrastructure.ResolveDbType (transaction, Tags.TypeCollectionRank);
				
				this.strTypeName           = this.infrastructure.ResolveDbType (transaction, Tags.TypeName);
				this.strTypeInfoXml        = this.infrastructure.ResolveDbType (transaction, Tags.TypeInfoXml);
				this.strTypeDictKey        = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictKey);
				this.strTypeDictValue      = this.infrastructure.ResolveDbType (transaction, Tags.TypeDictValue);
				
				this.otherTypeDateTime     = this.infrastructure.ResolveDbType (transaction, Tags.TypeDateTime);
				this.otherTypeReqData      = this.infrastructure.ResolveDbType (transaction, Tags.TypeReqData);
				
				this.infrastructure.internalTypes.Add (this.numTypeKeyId);
				this.infrastructure.internalTypes.Add (this.numTypeNullableKeyId);
				this.infrastructure.internalTypes.Add (this.numTypeKeyStatus);
				this.infrastructure.internalTypes.Add (this.numTypeReqExState);
				this.infrastructure.internalTypes.Add (this.numTypeCollectionRank);
				
				this.infrastructure.internalTypes.Add (this.strTypeName);
				this.infrastructure.internalTypes.Add (this.strTypeInfoXml);
				this.infrastructure.internalTypes.Add (this.strTypeDictKey);
				this.infrastructure.internalTypes.Add (this.strTypeDictValue);
				
				this.infrastructure.internalTypes.Add (this.otherTypeDateTime);
				this.infrastructure.internalTypes.Add (this.otherTypeReqData);
				
				this.AssertAllTypesReady ();
			}

			private void InitializeNumTypes()
			{
				this.numTypeKeyId          = new DbTypeDef (Res.Types.Num.KeyId);
				this.numTypeNullableKeyId  = new DbTypeDef (Res.Types.Num.NullableKeyId);
				this.numTypeKeyStatus      = new DbTypeDef (Res.Types.Num.KeyStatus);
				this.numTypeReqExState     = new DbTypeDef (Res.Types.Num.ReqExecState);
				this.numTypeCollectionRank = new DbTypeDef (Res.Types.Num.CollectionRank);
			
				this.infrastructure.internalTypes.Add (this.numTypeKeyId);
				this.infrastructure.internalTypes.Add (this.numTypeNullableKeyId);
				this.infrastructure.internalTypes.Add (this.numTypeKeyStatus);
				this.infrastructure.internalTypes.Add (this.numTypeReqExState);
				this.infrastructure.internalTypes.Add (this.numTypeCollectionRank);
			}

			private void InitializeOtherTypes()
			{
				this.otherTypeDateTime = new DbTypeDef (Res.Types.Other.DateTime);
				this.otherTypeReqData  = new DbTypeDef (Res.Types.Other.ReqData);
		
				this.infrastructure.internalTypes.Add (this.otherTypeDateTime);
				this.infrastructure.internalTypes.Add (this.otherTypeReqData);
			}

			private void InitializeStrTypes()
			{
				this.strTypeName      = new DbTypeDef (Res.Types.Str.Name);
				this.strTypeInfoXml   = new DbTypeDef (Res.Types.Str.InfoXml);
				this.strTypeDictKey   = new DbTypeDef (Res.Types.Str.Dict.Key);
				this.strTypeDictValue = new DbTypeDef (Res.Types.Str.Dict.Value);
			
				this.infrastructure.internalTypes.Add (this.strTypeName);
				this.infrastructure.internalTypes.Add (this.strTypeInfoXml);
				this.infrastructure.internalTypes.Add (this.strTypeDictKey);
				this.infrastructure.internalTypes.Add (this.strTypeDictValue);
			}

			private void AssertAllTypesReady()
			{
				System.Diagnostics.Debug.Assert (this.numTypeKeyId != null);
				System.Diagnostics.Debug.Assert (this.numTypeNullableKeyId != null);
				System.Diagnostics.Debug.Assert (this.numTypeKeyStatus != null);
				System.Diagnostics.Debug.Assert (this.numTypeReqExState != null);
				System.Diagnostics.Debug.Assert (this.numTypeCollectionRank != null);
				
				System.Diagnostics.Debug.Assert (this.strTypeName != null);
				System.Diagnostics.Debug.Assert (this.strTypeInfoXml != null);
				System.Diagnostics.Debug.Assert (this.strTypeDictKey != null);
				System.Diagnostics.Debug.Assert (this.strTypeDictValue != null);
				
				System.Diagnostics.Debug.Assert (this.otherTypeDateTime != null);
				System.Diagnostics.Debug.Assert (this.otherTypeReqData != null);
			}

			private DbInfrastructure			infrastructure;

			private DbTypeDef					numTypeKeyId;
			private DbTypeDef					numTypeNullableKeyId;
			private DbTypeDef					numTypeKeyStatus;
			private DbTypeDef					numTypeReqExState;
			private DbTypeDef					numTypeCollectionRank;

			private DbTypeDef					otherTypeDateTime;
			private DbTypeDef					otherTypeReqData;

			private DbTypeDef					strTypeName;
			private DbTypeDef					strTypeInfoXml;
			private DbTypeDef					strTypeDictKey;
			private DbTypeDef					strTypeDictValue;
		}
		
		#endregion


		private DbAccess						access;
		private IDbAbstraction					abstraction;

		private ISqlEngine						sqlEngine;
		private ITypeConverter					converter;
		
		private TypeHelper						types;
		private DbLogger						logger;
		private DbClientManager					clientManager;
		
		private Settings.Globals				globals;
		private Settings.Locals					locals;

		private Collections.DbTableList			internalTables = new Collections.DbTableList ();
		private Collections.DbTypeDefList			internalTypes = new Collections.DbTypeDefList ();

		private int								clientId;
		
		private string							localizations;

		private Cache.DbTypeDefs				typeCache = new Cache.DbTypeDefs ();
		private Cache.DbTables					tableCache = new Cache.DbTables ();

		private List<DbTransaction>				liveTransactions;
		private List<IDbAbstraction>			releaseRequested;
		
		private int								lockTimeout = 15000;
		System.Threading.ReaderWriterLock		globalLock = new System.Threading.ReaderWriterLock ();
	}
}
