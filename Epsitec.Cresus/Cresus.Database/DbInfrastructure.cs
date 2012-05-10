//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database.Collections;
using Epsitec.Cresus.Database.Exceptions;
using Epsitec.Cresus.Database.Logging;

using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Epsitec.Cresus.Database
{
	// TODO Split DbKeysCache in two, one for the types, and one for the tables?
	// Marc

	/// <summary>
	/// The <c>DbInfrastructure</c> class provides support for the database
	/// infrastructure needed by CRESUS (internal tables, metadata, etc.)
	/// <remarks>
	/// This class is not thread safe. However, the methods used to create transactions are thread
	/// safe enough to ensure that a transaction can be created only one thread at a time and must
	/// be disposed/rolled back/committed in order for another thread to obtain a transaction.
	/// Therefore, as all the methods provided by this class that make a request to the database
	/// require a transaction, they are indirectly thread safe because they cannot be called without
	/// having a valid transaction.
	/// So, you can consider the methods BeginTransaction(...), InheritOrBeginTransaction(...),
	/// ExecuteNonQuery(...), ExecuteOutputParameters(...), ExecuteRetData(...), ExecuteScalar(...),
	/// ExecuteSilent(...) and ExecuteSqlSelect(...) to be thread safe, as long as the transaction
	/// objects are used on the same thread as they are created. Any other member of this class
	/// should not be considered as thread safe.
	/// </remarks>
	/// </summary>
	public sealed class DbInfrastructure : DependencyObject, System.IDisposable
	{
		public DbInfrastructure()
		{
			internalTypes = new DbTypeDefList ();
			internalTables = new DbTableList ();
			this.QueryLogs = new HashSet<AbstractLog> ();

			this.liveTransactions = new List<DbTransaction> ();
			this.releaseRequested = new List<IDbAbstraction> ();

			this.dbKeysCache = new Dictionary<string, Dictionary<string, DbKey[]>> ();
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
				return this.globalLock.IsWriteLockHeld;
			}
		}

		internal TypeHelper						TypeManager
		{
			get
			{
				return this.types;
			}
		}

		public ISet<AbstractLog>				QueryLogs
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
			if (this.access.IsValid)
			{
				throw new Exceptions.GenericException (this.access, "A database already exists for this DbInfrastructure");
			}
			
			this.access = access;
			this.access.CreateDatabase = true;

			if (this.InitializeDatabaseAbstraction () == false)
			{
				this.access = DbAccess.Empty;
				return false;
			}
			
			this.types.RegisterTypes ();
			
			//	The database is now ready to be filled. At this point, it is totally
			//	empty of any tables.
			
			System.Diagnostics.Debug.Assert (this.abstraction.QueryUserTableNames ().Length == 0);

			List<DbTable> tableCore = BootHelper.CreateCoreTables (this).ToList ();
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				//	Create the tables required for our own metadata management. The
				//	created tables must be committed before they can be populated.

				BootHelper.RegisterTables (this, transaction, tableCore);
				
				transaction.Commit ();
			}
			
			//	Fill the tables with the initial metadata.
			
			using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetupTables (transaction);
				
				transaction.Commit ();
			}

			return true;
		}

		public void AttachToDatabase(DbAccess access)
		{
			if (this.access.IsValid)
			{
				throw new System.InvalidOperationException ("Database is already attached.");
			}

			try
			{
				this.ConnectToDatabase (access);
				this.LoadCoreTables ();
			}
			catch
			{
				this.access = DbAccess.Empty;
				throw;
			}
		}

		private void ConnectToDatabase(DbAccess access)
		{
			bool success;

			try
			{
				this.access = access;
				this.access.CreateDatabase = false;

				success = this.InitializeDatabaseAbstraction ();
			}
			catch (System.Exception e)
			{
				throw new System.Exception ("Cannot connect to database", e);
			}

			if (!success)
			{
				throw new System.Exception ("Cannot connect to database");
			}
		}

		private void LoadCoreTables()
		{
			try
			{
				using (DbTransaction transaction = this.BeginTransaction (DbTransactionMode.ReadOnly))
				{
					this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableTableDef));
					this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableColumnDef));
					this.internalTables.Add (this.ResolveDbTable (transaction, Tags.TableTypeDef));

					this.types.ResolveTypes (transaction);

					transaction.Commit ();
				}
			}
			catch (System.Exception ex)
			{
				throw new Exceptions.GenericException (this.access, "Cannot load core tables.", ex);
			}

			bool success = this.CheckCoreTables ();
			
			if (!success)
			{
				throw new Exceptions.IncompatibleDatabaseException (this.access, "Incompatible core tables.");
			}
		}

		private bool CheckCoreTables()
		{
			// TODO This check is based only on the meta data found in CR_TABLE_DEF and CR_COLUMN_DEF
			// therefore, if the meta data is correct but does not match the real state of the tables
			// in the database (that is, a table has been modified without the meta data being updated)
			// we won't detect the problem.
			// Marc

			List<DbTable> expectedTables = BootHelper.CreateCoreTables (this).ToList ();

			BootHelper.UpdateCoreTableRelations (expectedTables[0], expectedTables[1], expectedTables[2]);

			foreach (DbTable table in expectedTables)
			{
				table.UpdatePrimaryKeyInfo ();
			}

			return expectedTables.All (t => this.internalTables[t.Name] != null)
				&& DbSchemaChecker.CheckSchema (this, expectedTables);
		}

		/// <summary>
		/// Releases the database connection. If transactions are still active
		/// for this database, the connection will be released automatically
		/// when the transactions finish.
		/// </summary>
		public void ReleaseConnection()
		{
			this.ReleaseConnection (this.abstraction);
		}

		/// <summary>
		/// Releases the database connection. If transactions are still active
		/// for this database, the connection will be released automatically
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

		public static void DropDatabase(DbAccess dbAccess)
		{
			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.DropDatabase ();
			}
		}

		public static bool CheckDatabaseExistence(DbAccess dbAccess)
		{
			string provider = dbAccess.Provider;
			string database = dbAccess.Database;
			string server   = dbAccess.Server;
			string user     = dbAccess.LoginName;
			string password = dbAccess.LoginPassword;

			DbAccess testDbAccess = new DbAccess (provider, database, server, user, password, false)
			{
				CheckConnection               = false,
				CreateDatabase                = false,
				IgnoreInitialConnectionErrors = true,
			};

			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (testDbAccess))
			{
				return idbAbstraction.ServiceTools.CheckExistence ();
			}
		}

		public static void BackupDatabase(DbAccess dbAccess, string remoteFilePath)
		{
			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.ServiceTools.Backup (remoteFilePath);
			}
		}

		public static void RestoreDatabase(DbAccess dbAccess, string remoteFilePath)
		{
			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.ServiceTools.Restore (remoteFilePath);
			}
		}


		/// <summary>
		/// Creates the database access.
		/// </summary>
		/// <param name="name">The database file name.</param>
		/// <param name="host">The host where the database is.</param>
		/// <returns>The database access.</returns>
		public static DbAccess CreateDatabaseAccess(string name, string host = "localhost")
		{
			ExceptionThrower.ThrowIfNullOrEmpty (name, "name");
			ExceptionThrower.ThrowIfNullOrEmpty (host, "host");

			return new DbAccess ("Firebird", name, host, "sysdba", "masterkey", false);
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
		/// Begins the specified transaction for the default database abstraction locking the
		/// specified given <see cref="DbTable"/>.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <param name="tablesToLock">The <see cref="DbTable"/> to lock during the transaction.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction(DbTransactionMode mode, IEnumerable<DbTable> tablesToLock)
		{
			return this.BeginTransaction (mode, tablesToLock, this.abstraction);
		}
		
		/// <summary>
		/// Begins a transaction for the specified database abstraction.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <param name="abstraction">The database abstraction.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction(DbTransactionMode mode, IDbAbstraction abstraction)
		{
			return this.BeginTransaction (mode, new List<DbTable> (), abstraction);
		}

		/// <summary>
		/// Begins the specified transaction for the specified database abstraction locking the
		/// specified <see cref="DbTable"/>.
		/// </summary>
		/// <param name="mode">The transaction mode.</param>
		/// <param name="tablesToLock">The <see cref="DbTable"/> to lock during the transaction.</param>
		/// <param name="abstraction">The database abstraction.</param>
		/// <returns>The transaction.</returns>
		public DbTransaction BeginTransaction(DbTransactionMode mode, IEnumerable<DbTable> tablesToLock, IDbAbstraction abstraction)
		{
			System.Diagnostics.Debug.Assert (abstraction != null);
			System.Diagnostics.Debug.Assert (tablesToLock != null);

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
						transaction = new DbTransaction (abstraction.BeginReadOnlyTransaction (tablesToLock), abstraction, this, mode);
						break;

					case DbTransactionMode.ReadWrite:
						transaction = new DbTransaction (abstraction.BeginReadWriteTransaction (tablesToLock), abstraction, this, mode);
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
			this.DatabaseLock (this.abstraction);

			try
			{
				DbTransaction live = this.FindLiveTransaction (this.abstraction);

				if (live == null)
				{
					return this.BeginTransaction (mode);
				}
				else
				{
					if (mode == DbTransactionMode.ReadWrite && live.IsReadOnly)
					{
						throw new System.InvalidOperationException ("Cannot begin read/write transaction from inherited read-only transaction");
					}

					this.DatabaseLock (this.abstraction);

					try
					{
						return new DbTransaction (live);
					}
					catch
					{
						this.DatabaseUnlock (this.abstraction);
						throw;
					}
				}
			}
			finally
			{
				this.DatabaseUnlock (this.abstraction);
			}
		}


		/// <summary>
		/// Creates a minimal database table definition. This will only contain
		/// the basic id and status columns required by <c>DbInfrastructure</c>.
		/// </summary>
		/// <param name="name">The table name.</param>
		/// <param name="category">The category.</param>
		/// <param name="autoIncrementedId">Tells whether the id of the table must be auto incremented or not.</param>
		/// <returns>The database table definition.</returns>
		public DbTable CreateDbTable(string name, DbElementCat category, bool autoIncrementedId)
		{
			switch (category)
			{
				case DbElementCat.Internal:
					throw new Exceptions.GenericException (this.access, string.Format ("Users may not create internal tables (table '{0}')", name));
				
				case DbElementCat.ManagedUserData:
					return this.CreateTable(name, category, autoIncrementedId);
				
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
		/// <param name="autoIncrementedId">Tells whether the id of the table must be auto incremented or not.</param>
		/// <returns>The database table definition.</returns>
		public DbTable CreateDbTable(Druid captionId, DbElementCat category, bool autoIncrementedId)
		{
			switch (category)
			{
				case DbElementCat.Internal:
					throw new Exceptions.GenericException (this.access, string.Format ("Users may not create internal tables (table '{0}')", captionId));

				case DbElementCat.ManagedUserData:
					return this.CreateTable (captionId, category, autoIncrementedId);

				default:
					throw new Exceptions.GenericException (this.access, string.Format ("Unsupported category {0} specified for table '{1}'", category, captionId));
			}
		}

		public void AddTable(DbTable table)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.AddTable (transaction, table);

				transaction.Commit ();
			}
		}

		public void AddTable(DbTransaction transaction, DbTable table)
		{
			// TODO Ensure that the table is not a relation table?
			// Marc

			this.CheckForRegisteredTypes (transaction, table);
			this.CheckForUnknownTable (transaction, table);
			this.CheckForRelations (table, this.FindDbTables (DbElementCat.Any).Append (table).ToList ());

			this.AddTableInternal (transaction, table);
		}

		public void AddTables(IEnumerable<DbTable> tables)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.AddTables (transaction, tables);

				transaction.Commit ();
			}
		}

		public void AddTables(DbTransaction transaction, IEnumerable<DbTable> tables)
		{
			// TODO Ensure that the table is not a relation table?
			// Marc

			var existingTables = this.FindDbTables (DbElementCat.Any);
			var newTables = tables.ToList ();

			foreach (DbTable table in newTables)
			{
				this.CheckForRegisteredTypes (transaction, table);
				this.CheckForUnknownTable (transaction, table);
				this.CheckForRelations (table, existingTables.Concat (newTables).ToList ());
			}
			
			this.AddTablesInternal (transaction, newTables);
		}

		public void RemoveTable(DbTable table)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RemoveTable (transaction, table);

				transaction.Commit ();
			}
		}

		public void RemoveTable(DbTransaction transaction, DbTable table)
		{
			// TODO Ensure that the table is not a relation table?
			// Marc

			// TODO Do not remove the inward relation columns but throw an exception if the table
			// to be removed is targeted by a relation table?
			// Marc

			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			if (internalTable == null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " is not defined.");
			}

			this.RemoveTableInternal (transaction, internalTable);
		}

		public void AddColumnToTable(DbTable table, DbColumn column)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.AddColumnToTable (transaction, table, column);

				transaction.Commit ();
			}
		}

		public void AddColumnToTable(DbTransaction transaction, DbTable table, DbColumn column)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			if (internalTable == null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " is not defined.");
			}

			DbColumn internalColumn = internalTable.Columns[column.Name];

			if (internalColumn != null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " has alreary a column " + column.Name + ".");
			}

			if (column.IsPrimaryKey)
			{
				throw new GenericException (this.access, "New colum " + column.Name + " for table " + table.Name + " cannot be a primary key.");
			}

			if (column.ColumnClass == DbColumnClass.KeyId)
			{
				throw new GenericException (this.access, "New colum " + column.Name + " for table " + table.Name + " has an invalid column class.");
			}

			// TODO Mutate the dbColumn object and the dbTable object?
			// Marc

			switch (column.Cardinality)
			{
				case DbCardinality.None:
					this.AddConcreteColumnToTable (transaction, internalTable, column);
					break;

				case DbCardinality.Reference:
				case DbCardinality.Collection:
					this.AddRelationTableAndColumn (transaction, internalTable, column);
					break;

				default:
					throw new System.NotImplementedException ();
			}

			this.RemoveFromCache (internalTable);
		}

		public void RemoveColumnFromTable(DbTable table, DbColumn column)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RemoveColumnFromTable (transaction, table, column);

				transaction.Commit ();
			}
		}

		public void RemoveColumnFromTable(DbTransaction transaction, DbTable table, DbColumn column)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			if (internalTable == null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " is not defined.");
			}

			DbColumn internalColumn = internalTable.Columns[column.Name];

			if (internalColumn == null)
			{
				throw new GenericException (this.access, "The column " + column.Name + " is not defined for table " + table.Name + ".");
			}

			if (column.IsPrimaryKey)
			{
				throw new GenericException (this.access, "Colum " + column.Name + " of table " + table.Name + " cannot be removed because it is a primary key.");
			}

			if (column.ColumnClass == DbColumnClass.KeyId)
			{
				throw new GenericException (this.access, "Colum " + column.Name + " of table " + table.Name + " cannot be removed because of its status.");
			}

			if (table.Indexes.SelectMany (i => i.Columns).Contains (column))
			{
				throw new GenericException (this.access, "Colum " + column.Name + " of table " + table.Name + " cannot be removed because it part of an index.");
			}

			// TODO Add a check on the "foreign keys" of other tables? This is probably not necessary
			// because the column can be referenced only if it is a primary key, and we check for that.
			// Marc

			switch (column.Cardinality)
			{
				case DbCardinality.None:
					this.RemoveConcreteColumnFromTable (transaction, internalTable, internalColumn);
					break;

				case DbCardinality.Reference:
				case DbCardinality.Collection:
					this.RemoveRelationTableAndColumn (transaction, internalColumn);
					break;

				default:
					throw new System.NotImplementedException ();
			}

			this.RemoveFromCache (internalTable);
		}

		public void AddIndexToTable(DbTable table, DbIndex index)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.AddIndexToTable (transaction, table, index);

				transaction.Commit ();
			}
		}

		public void AddIndexToTable(DbTransaction transaction, DbTable table, DbIndex index)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			this.CheckUnexistingIndex (internalTable, index);
			
			this.AddIndexInternal (transaction, internalTable, index);
			this.RemoveFromCache (internalTable);
		}

		public void ResetIndex(DbTable table, DbIndex index)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.ResetIndex (transaction, table, index);

				transaction.Commit ();
			}
		}

		public void ResetIndex(DbTransaction transaction, DbTable table, DbIndex index)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			this.CheckExistingIndex (internalTable, index);

			this.ResetIndexInternal (transaction, internalTable, index);
		}

		public void RemoveIndexFromTable(DbTable table, DbIndex index)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RemoveIndexFromTable (transaction, table, index);

				transaction.Commit ();
			}
		}

		public void RemoveIndexFromTable(DbTransaction transaction, DbTable table, DbIndex index)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			this.CheckExistingIndex (internalTable, index);

			this.RemoveIndexInternal (transaction, internalTable, index);
			this.RemoveFromCache (internalTable);
		}

		public void SetTableComment(DbTransaction transaction, DbTable table, string comment)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			this.SetTableCommentInternal (transaction, internalTable, comment);

			this.RemoveFromCache (internalTable);
		}

		private void CheckUnexistingIndex(DbTable table, DbIndex index)
		{
			// TODO Implement this method.
			// Marc
		}

		private void CheckExistingIndex(DbTable table, DbIndex index)
		{
			// TODO Implement this method.
			// Marc
		}

		public void SetColumnAutoIncrementValue(DbTable table, DbColumn column, long value)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetColumnAutoIncrementValue (transaction, table, column, value);

				transaction.Commit ();
			}
		}

		public void SetColumnAutoIncrementValue(DbTransaction transaction, DbTable table, DbColumn column, long value)
		{
			value.ThrowIf (v => v < 0, "value is lower than zero");

			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			if (internalTable == null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " is not defined.");
			}

			DbColumn internalColumn = internalTable.Columns[column.Name];

			if (internalColumn == null)
			{
				throw new GenericException (this.access, "The column " + column.Name + " is not defined for table " + table.Name + ".");
			}

			if (!internalColumn.IsAutoIncremented)
			{
				throw new GenericException (this.access, "The column " + column.Name + " is not autoincremented");
			}

			internalColumn.AutoIncrementStartValue = value;

			this.DropAutoIncrementFromColumn (transaction, internalTable, internalColumn);
			this.InsertAutoIncrementForColumn (transaction, internalTable, internalColumn);
		}

		public void RenameTableColumn(DbTable table, DbColumn column, string newName)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RenameTableColumn (transaction, table, column, newName);

				transaction.Commit ();
			}
		}

		public void RenameTableColumn(DbTransaction transaction, DbTable table, DbColumn column, string newName)
		{
			DbTable internalTable = this.ResolveDbTable (transaction, table.Name);

			if (internalTable == null)
			{
				throw new GenericException (this.access, "Table " + table.Name + " is not defined.");
			}

			DbColumn internalColumn = internalTable.Columns[column.Name];

			if (internalColumn == null)
			{
				throw new GenericException (this.access, "The column " + column.Name + " is not defined for table " + table.Name + ".");
			}

			if (internalColumn.Cardinality == DbCardinality.Collection)
			{
				throw new GenericException (this.access, "Cannot rename collection columns!");
			}

			if (internalTable.Columns[newName] != null)
			{
				throw new GenericException (this.access, "There is already a column with name " + newName + " in table " + table.Name + ".");
			}

			DbColumn columnWithOldName = internalColumn;
			DbColumn columnWithNewName = (DbColumn) internalColumn.Clone ();

			columnWithNewName.DefineCaptionId (Druid.Empty);
			columnWithNewName.DefineDisplayName (newName);

			this.UpdateColumnDefRow (transaction, table, columnWithNewName);
			this.RenameColumn (transaction, table, columnWithOldName, columnWithNewName);
			this.RemoveFromCache (internalTable);
		}

		private void AddTableInternal(DbTransaction transaction, DbTable table)
		{
			this.AddConcreteTable (transaction, table);
			this.AddRelationTablesAndColumns (transaction, table);
			this.RemoveFromCache (table);
		}

		private void AddTablesInternal(DbTransaction transaction, List<DbTable> newTables)
		{
			foreach (DbTable table in newTables)
			{
				this.AddConcreteTable (transaction, table);
			}

			foreach (DbTable table in newTables)
			{
				this.AddRelationTablesAndColumns (transaction, table);
			}

			foreach (DbTable table in newTables)
			{
				this.RemoveFromCache (table);
			}
		}


		private void RemoveTableInternal(DbTransaction transaction, DbTable internalTable)
		{
			this.RemoveRelationTablesAndColumns (transaction, internalTable);
			this.RemoveConcreteTable (transaction, internalTable);

			this.RemoveFromCache (internalTable);
		}

		private void AddConcreteTable(DbTransaction transaction, DbTable table)
		{
			this.RegisterTable (transaction, table);
			this.InsertTable (transaction, table);
		}

		private void RemoveConcreteTable(DbTransaction transaction, DbTable table)
		{
			this.DropTable (transaction, table);
			this.UnregisterTable (transaction, table);
		}

		private void AddConcreteColumnToTable(DbTransaction transaction, DbTable table, DbColumn column)
		{
			this.InsertConcreteColumnToTable (transaction, table, column);
			this.RegisterConcreteTableColumn (transaction, table, column);
		}

		private void RemoveConcreteColumnFromTable(DbTransaction transaction, DbTable table, DbColumn column)
		{
			this.DropConcreteColumnFromTable (transaction, table, column);
			this.UnregisterConcreteTableColumn (transaction, column);
		}

		private void AddRelationTablesAndColumns(DbTransaction transaction, DbTable table)
		{
			foreach (DbColumn column in table.Columns.Where (c => c.Cardinality != DbCardinality.None))
			{
				this.AddRelationTableAndColumn (transaction, table, column);
			}
		}

		private void AddRelationTableAndColumn(DbTransaction transaction, DbTable table, DbColumn column)
		{
			this.AddRelationTable (transaction, table, column);
			this.RegisterRelationTableColumn (transaction, table, column);
		}
		
		private void AddRelationTable(DbTransaction transaction, DbTable table, DbColumn column)
		{
			DbTable relationTable = DbTable.CreateRelationTable (this, table, column);

			this.AddTableInternal (transaction, relationTable);
		}

		private void RemoveRelationTablesAndColumns(DbTransaction transaction, DbTable table)
		{
			var relationColumnsIn = this.FindInwardRelationColumns (table).ToList ();
			var relationColumnsOut = this.FindOutwardRelationColumns (table).ToList ();

			foreach (DbColumn column in relationColumnsIn.Concat (relationColumnsOut))
			{
				this.RemoveRelationTableAndColumn (transaction, column);
			}
		}

		private void RemoveRelationTableAndColumn(DbTransaction transaction, DbColumn column)
		{
			this.RemoveRelationTable (transaction, column);
			this.UnregisterRelationTableColumn (transaction, column);
			this.RemoveFromCache (column.Table);
		}
		
		private void RemoveRelationTable(DbTransaction transaction, DbColumn column)
		{
			DbTable sourceTable = column.Table;

			string relationTableName = sourceTable.GetRelationTableName (column);
			DbTable relationTable = this.ResolveDbTable (relationTableName);

			this.RemoveTableInternal (transaction, relationTable);
		}

		private void AddIndexInternal(DbTransaction transaction, DbTable table, DbIndex index)
		{
			table.Indexes.Add (index);

			SqlTable sqlTable = table.CreateSqlTable (this.converter);
			SqlIndex sqlIndex = sqlTable.Indexes.Where (i => i.Name == index.Name).Single ();

			this.UpdateTableDefRow (transaction, table);
			this.InsertIndex (transaction, sqlTable, sqlIndex);
		}

		private void ResetIndexInternal(DbTransaction transaction, DbTable table, DbIndex index)
		{
			SqlTable sqlTable = table.CreateSqlTable (this.converter);
			SqlIndex sqlIndex = sqlTable.Indexes.Where (i => i.Name == index.Name).Single ();

			this.ResetIndex (transaction, sqlIndex);
		}

		private void RemoveIndexInternal(DbTransaction transaction, DbTable table, DbIndex index)
		{
			SqlTable sqlTable = table.CreateSqlTable (this.converter);
			SqlIndex sqlIndex = sqlTable.Indexes.Where (i => i.Name == index.Name).Single ();

			int indexOfIndex = table.Indexes.IndexOf (index, INameComparer.Instance);
			table.Indexes.RemoveAt (indexOfIndex);

			this.UpdateTableDefRow (transaction, table);
			this.DropIndex (transaction, sqlIndex);
		}

		private void SetTableCommentInternal(DbTransaction transaction, DbTable table, string comment)
		{
			table.Comment = comment;

			this.UpdateTableDefRow (transaction, table);
			this.SetTableComment (transaction, table);
		}

		private void RegisterTable(DbTransaction transaction, DbTable table)
		{
			table.UpdatePrimaryKeyInfo ();
			DbKey tableKey = this.InsertTableDefRow (transaction, table);
			table.DefineKey (tableKey);

			foreach (DbColumn column in table.Columns.Where (c => c.Cardinality == DbCardinality.None))
			{
				this.RegisterConcreteTableColumn (transaction, table, column);
			}
		}

		private void UnregisterTable(DbTransaction transaction, DbTable table)
		{
			this.DeleteColumnDefRows (transaction, table);
			this.DeleteTableDefRow (transaction, table);
		}
		
		private void RegisterConcreteTableColumn(DbTransaction transaction, DbTable table, DbColumn column)
		{
			DbKey columnKey = this.InsertColumnDefRow (transaction, table, column);

			column.DefineKey (columnKey);
		}

		private void UnregisterConcreteTableColumn(DbTransaction dbTransaction, DbColumn dbColumn)
		{
			this.DeleteColumnDefRow (dbTransaction, dbColumn);
		}

		private void RegisterRelationTableColumn(DbTransaction transaction, DbTable table, DbColumn column)
		{
			switch (column.ColumnClass)
			{
				// TODO Should it include other classes in this case?
				// Marc

				case DbColumnClass.RefId:
				{
					string targetTableName = column.TargetTableName;

					DbTable targetTable = this.ResolveDbTable (transaction, targetTableName);

					DbKey columnKey = this.InsertRelationColumnDefRow (transaction, table, column, targetTable);
					column.DefineKey (columnKey);	
				}
				break;

				default:
				{
					DbKey columnKey = this.InsertColumnDefRow (transaction, table, column);
					column.DefineKey (columnKey);
				}
				break;
			}
		}

		public void UnregisterRelationTableColumn(DbTransaction transaction, DbColumn column)
		{
			this.DeleteColumnDefRow (transaction, column);
		}

		private void InsertTable(DbTransaction transaction, DbTable table)
		{
			SqlTable sqlTable = table.CreateSqlTable (this.converter);

			transaction.SqlBuilder.InsertTable (sqlTable);
			this.ExecuteSilent (transaction);

			foreach (DbColumn dbColumn in table.Columns.Where (c => c.Cardinality == DbCardinality.None))
			{
				this.InsertAutoIncrementForColumn (transaction, table, dbColumn);
				this.InsertAutoTimeStampForColumn (transaction, table, dbColumn);
			}

			foreach (SqlIndex sqlIndex in sqlTable.Indexes)
			{
				this.InsertIndex (transaction, sqlTable, sqlIndex);
			}
		}

		private void InsertConcreteColumnToTable(DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumn)
		{
			string dbTableName = dbTable.GetSqlName ();
			SqlColumn sqlColumn = dbColumn.CreateSqlColumn (this.converter);

			dbTransaction.SqlBuilder.InsertTableColumns (dbTableName, new SqlColumn[] { sqlColumn });
			this.ExecuteSilent (dbTransaction);

			this.InsertAutoIncrementForColumn (dbTransaction, dbTable, dbColumn);
			this.InsertAutoTimeStampForColumn (dbTransaction, dbTable, dbColumn);
		}

		private void InsertAutoIncrementForColumn(DbTransaction transaction, DbTable dbTable, DbColumn dbColumn)
		{
			if (dbColumn.IsAutoIncremented)
			{
				string tableName = dbTable.GetSqlName ();
				string columnName = dbColumn.GetSqlName ();

				transaction.SqlBuilder.SetAutoIncrementOnTableColumn (tableName, columnName, dbColumn.AutoIncrementStartValue);
				this.ExecuteSilent (transaction);
			}
		}

		private void InsertAutoTimeStampForColumn(DbTransaction transaction, DbTable dbTable, DbColumn dbColumn)
		{
			bool autoTimeStampOnInsert = dbColumn.IsAutoTimeStampOnInsert;
			bool autoTimeStampOnUpdate = dbColumn.IsAutoTimeStampOnUpdate;

			if (autoTimeStampOnInsert || autoTimeStampOnUpdate)
			{
				string tableName = dbTable.GetSqlName ();
				string columnName = dbColumn.GetSqlName ();

				transaction.SqlBuilder.SetAutoTimeStampOnTableColumn (tableName, columnName, autoTimeStampOnInsert, autoTimeStampOnUpdate);
				this.ExecuteSilent (transaction);
			}
		}

		private void InsertIndex(DbTransaction transaction, SqlTable table, SqlIndex index)
		{
			transaction.SqlBuilder.CreateIndex (table.Name, index);

			this.ExecuteSilent (transaction);
		}

		private void DropTable(DbTransaction dbTransaction, DbTable dbTable)
		{
			SqlTable sqlTable = dbTable.CreateSqlTable (this.converter);

			foreach (SqlIndex index in sqlTable.Indexes)
			{
				this.DropIndex (dbTransaction, index);
			}
			
			foreach (DbColumn dbColumn in dbTable.Columns.Where (c => c.Cardinality == DbCardinality.None))
			{
				this.DropAutoIncrementFromColumn (dbTransaction, dbTable, dbColumn);
				this.DropAutoTimeStampFromColumn (dbTransaction, dbTable, dbColumn);
			}
			
			dbTransaction.SqlBuilder.RemoveTable (sqlTable);
			this.ExecuteSilent (dbTransaction);
		}

		private void DropConcreteColumnFromTable(DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumn)
		{
			this.DropAutoIncrementFromColumn (dbTransaction, dbTable, dbColumn);
			this.DropAutoTimeStampFromColumn (dbTransaction, dbTable, dbColumn);

			string dbTableName = dbTable.GetSqlName ();
			SqlColumn sqlColumn = dbColumn.CreateSqlColumn (this.converter);

			dbTransaction.SqlBuilder.RemoveTableColumns (dbTableName, new SqlColumn[] { sqlColumn });
			this.ExecuteSilent (dbTransaction);
		}
		
		private void DropAutoIncrementFromColumn(DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumn)
		{
			if (dbColumn.IsAutoIncremented)
			{
				string tableName = dbTable.GetSqlName ();
				string columnName = dbColumn.GetSqlName ();

				dbTransaction.SqlBuilder.DropAutoIncrementOnTableColumn (tableName, columnName);
				this.ExecuteSilent (dbTransaction);
			}
		}
		
		private void DropAutoTimeStampFromColumn(DbTransaction dbTransaction, DbTable dbTable, DbColumn dbColumn)
		{
			bool autoTimeStampOnInsert = dbColumn.IsAutoTimeStampOnInsert;
			bool autoTimeStampOnUpdate = dbColumn.IsAutoTimeStampOnUpdate;

			if (autoTimeStampOnInsert || autoTimeStampOnUpdate)
			{
				string tableName = dbTable.GetSqlName ();
				string columnName = dbColumn.GetSqlName ();

				dbTransaction.SqlBuilder.DropAutoTimeStampOnTableColumn (tableName, columnName);
				this.ExecuteSilent (dbTransaction);
			}
		}

		private void DropIndex(DbTransaction transaction, SqlIndex index)
		{
			transaction.SqlBuilder.DropIndex (index);
			this.ExecuteSilent (transaction);
		}

		private void ResetIndex(DbTransaction transaction, SqlIndex index)
		{
			transaction.SqlBuilder.ResetIndex (index);
			this.ExecuteSilent (transaction);
		}

		private void RenameColumn(DbTransaction transaction, DbTable table, DbColumn columnWithOldName, DbColumn columnWithNewName)
		{
			string tableName = table.GetSqlName ();
			string oldColumnName = columnWithOldName.GetSqlName ();
			string newColumnName = columnWithNewName.GetSqlName ();

			transaction.SqlBuilder.RenameTableColumn (tableName, oldColumnName, newColumnName);
			this.ExecuteSilent (transaction);
		}

		private void SetTableComment(DbTransaction transaction, DbTable table)
		{
			string tableName = table.GetSqlName ();
			string comment = table.Comment;

			transaction.SqlBuilder.SetTableComment (tableName, comment);
			this.ExecuteSilent (transaction);
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
					List<DbTable> tables = this.LoadDbTable (transaction, key);
					
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

		public DbTable ResolveDbTable(Druid tableId)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction(DbTransactionMode.ReadOnly))
			{
				DbTable table = this.ResolveDbTable (transaction, tableId);

				transaction.Commit ();

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
			List<DbTable> list = this.LoadDbTable (transaction, DbKey.Empty);
			
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

		internal IEnumerable<DbTable> FindBuiltInDbTables()
		{
			return this.internalTables;
		}

		private IEnumerable<DbColumn> FindInwardRelationColumns(DbTable dbTable)
		{
			// This request might be optimized because it looks at all the table in the database. It
			// might be more efficient to execute an appropriate SQL request or to use the method that
			// gets the source reference resolvers.
			// Marc

			return this.FindDbTables (DbElementCat.Any)
				.SelectMany (t => t.Columns)
				.Where (c => c.Cardinality != DbCardinality.None)
				.Where (c => c.TargetTableName == dbTable.Name);
		}

		private IEnumerable<DbColumn> FindOutwardRelationColumns(DbTable dbTable)
		{
			return dbTable.Columns.Where (c => c.Cardinality != DbCardinality.None);
		}

		public void AddType(DbTypeDef type)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.AddType (transaction, type);

				transaction.Commit ();
			}
		}

		public void AddType(DbTransaction transaction, DbTypeDef type)
		{
			this.CheckForUnknownType (transaction, type);

			this.RegisterDbType (transaction, type);
		}

		public void RemoveType(DbTypeDef type)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.RemoveType (transaction, type);

				transaction.Commit ();
			}
		}

		public void RemoveType(DbTransaction transaction, DbTypeDef type)
		{
			this.CheckForKnownType (transaction, type);
			this.CheckForUnusedType (transaction, type);
			this.CheckForNotBuiltInType (type);

			this.UnregisterDbType (transaction, type);

			this.RemoveFromCache (type);
		}

		private void RegisterDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			DbKey typeKey = this.InsertTypeDefRow (transaction, typeDef);

			typeDef.DefineKey (typeKey);
		}

		private void UnregisterDbType(DbTransaction transaction, DbTypeDef typeDef)
		{
			this.DeleteTypeDefRow (transaction, typeDef);
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

		public DbTypeDef ResolveDbType(INamedType type)
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction(DbTransactionMode.ReadOnly))
			{
				DbTypeDef typeDef = this.ResolveDbType (transaction, type);

				transaction.Commit ();

				return typeDef;
			}
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
					List<DbTypeDef> types = this.LoadDbType (transaction, key);
					
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
			return this.LoadDbType (transaction, DbKey.Empty).ToArray ();
		}

		internal IEnumerable<DbTypeDef> FindBuiltInDbTypes()
		{
			return this.internalTypes;
		}

		private void RemoveFromCache(DbTable table)
		{
			this.tableCache[table.Key] = null;

			if (this.dbKeysCache.ContainsKey (Tags.TableTableDef))
			{
				this.dbKeysCache[Tags.TableTableDef].Remove (table.Name);
			}
		}

		private void RemoveFromCache(DbTypeDef type)
		{
			this.typeCache[type.Key] = null;

			if (this.dbKeysCache.ContainsKey (Tags.TableTypeDef))
			{
				this.dbKeysCache[Tags.TableTableDef].Remove (type.Name);
			}
		}

		/// <summary>
		/// Clears the table and type caches. This will force a reload of the
		/// table definitions and type definitions.
		/// </summary>
		public void ClearCaches()
		{
			lock (this.dbKeysCache)
			{
				this.dbKeysCache.Clear ();
			}
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
			field.Alias = column.GetSqlName ();
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
		/// Creates a table definition with the minimum id and status columns.
		/// </summary>
		/// <param name="name">The table name.</param>
		/// <param name="category">The table category.</param>
		/// <param name="autoIncrementedId">Tells whether the id of the table must be auto incremented or not.</param>
		/// <returns></returns>
		internal DbTable CreateTable(string name, DbElementCat category, bool autoIncrementedId)
		{
			DbTable table = new DbTable (name);

			this.DefineBasicTable (table, category,  autoIncrementedId);

			return table;
		}

		/// <summary>
		/// Creates a table definition with the minimum id and status columns.
		/// </summary>
		/// <param name="captionId">The table caption id.</param>
		/// <param name="category">The table category.</param>
		/// <param name="autoIncrementedId">Tells whether the id of the table must be auto incremented or not.</param>
		/// <returns></returns>
		internal DbTable CreateTable(Druid captionId, DbElementCat category, bool autoIncrementedId)
		{
			DbTable table = new DbTable (captionId);

			this.DefineBasicTable (table, category, autoIncrementedId);

			return table;
		}

		private void DefineBasicTable(DbTable table, DbElementCat category, bool autoIncrementedId)
		{
			DbColumn colId = new DbColumn (Tags.ColumnId, this.internalTypes[Tags.TypeKeyId], DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = autoIncrementedId,
				AutoIncrementStartValue = 0,
			};
			table.DefineCategory (category);

			table.Columns.Add (colId);

			table.PrimaryKeys.Add (colId);
			table.UpdatePrimaryKeyInfo ();
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
			foreach (DbColumn column in table.Columns.Where (c => c.Cardinality == DbCardinality.None))
			{
				DbTypeDef typeDef = column.Type;

				if (typeDef.Key.IsEmpty)
				{
					DbTypeDef myTypeDef = this.ResolveDbType (transaction, typeDef.Name);

					if (myTypeDef == null)
					{
						string message = string.Format ("Unregistered type '{0}' used in table '{1}', column '{2}'.",
							/* */						typeDef.Name, table.Name, column.Name);

						throw new Exceptions.GenericException (this.access, message);
					}
					else
					{
						typeDef.DefineKey (myTypeDef.Key);
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

		private void CheckForUnusedType(DbTransaction transaction, DbTypeDef type)
		{
			if (this.CountMatchingRows (transaction, Tags.TableColumnDef, Tags.ColumnRefType, type.Key.ToString ()) > 0)
			{
				string message = string.Format ("Type {0} is used in database.", type.Name);

				throw new Exceptions.GenericException (this.access, message);
			}
		}

		private void CheckForNotBuiltInType(DbTypeDef type)
		{
			if (this.FindBuiltInDbTypes ().Any (t => t.Name == type.Name))
			{
				string message = string.Format ("Type {0} is a built in type.", type.Name);

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

		private void CheckForRelations(DbTable table, List<DbTable> tables)
		{
			IEnumerable<DbColumn> relationColumns = table.Columns.Where (c => c.Cardinality != DbCardinality.None);

			foreach (DbColumn column in relationColumns)
			{
				string targetTableName = column.TargetTableName;

				if (!tables.Any (t => t.Name == targetTableName))
				{
					string message = string.Format ("Column {0} of table {1} targets a tables which does not exist.", column.Name, table.Name);

					throw new Exceptions.GenericException (this.access, message);
				}
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
			return TimedReaderWriterLock.LockWrite (this.globalLock, this.lockTimeout);
		}

		/// <summary>
		/// Locks a specific database connection. This prevents that the global
		/// lock gets locked until this database connection is unlocked again.
		/// </summary>
		/// <param name="database">The database abstraction.</param>
		internal void DatabaseLock(IDbAbstraction database)
		{
			bool success = this.globalLock.TryEnterReadLock (this.lockTimeout);

			if (!success)
			{
				throw new LockTimeoutException ();
			}

			if (Monitor.TryEnter (database, this.lockTimeout) == false)
			{
				this.globalLock.ExitReadLock ();
				throw new Exceptions.DeadLockException (this.access, "Cannot lock database.");
			}
		}

		/// <summary>
		/// Unlocks a specific database connection.
		/// </summary>
		/// <param name="database">The database abstraction.</param>
		internal void DatabaseUnlock(IDbAbstraction database)
		{
			Monitor.Exit (database);
			this.globalLock.ExitReadLock ();
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

			this.DatabaseUnlock (abstraction);		
		}


		internal void NotifyEndInheritedTransaction(DbTransaction transaction)
		{
			IDbAbstraction abstraction = transaction.Database;

			this.DatabaseUnlock (abstraction);
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
				int data;

				if (!this.IsLogEnabled ())
				{
					data = this.ExecuteSilent (count, command);
				}
				else
				{
					System.DateTime startTime = System.DateTime.Now;
					System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew ();

					data = this.ExecuteSilent (count, command);
					watch.Stop ();

					this.Log (l => l.AddEntry (command, startTime, watch.Elapsed, data));
				}

				return data;
			}
		}
		
		private int ExecuteSilent(int count, System.Data.IDbCommand command)
		{
			int result;

			this.sqlEngine.Execute (command, DbCommandType.Silent, count, out result);

			return result;
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

				if (!this.IsLogEnabled ())
				{
					data = this.ExecuteScalar (count, command);
				}
				else
				{
					System.DateTime startTime = System.DateTime.Now;
					System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew ();

					data = this.ExecuteScalar (count, command);
					watch.Stop ();

					this.Log (l => l.AddEntry (command, startTime, watch.Elapsed, data));
				}

				return data;
			}
		}

		private object ExecuteScalar(int count, System.Data.IDbCommand command)
		{
			object data;

			this.sqlEngine.Execute (command, DbCommandType.ReturningData, count, out data);

			return data;
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

				if (!this.IsLogEnabled ())
				{
					data = this.ExecuteNonQuery (count, command);
				}
				else
				{
					System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
					System.DateTime startTime = System.DateTime.Now;

					watch.Start ();
					data = this.ExecuteNonQuery (count, command);
					watch.Stop ();

					this.Log (l => l.AddEntry (command, startTime, watch.Elapsed, data));
				}

				return data;
			}
		}
		
		private object ExecuteNonQuery(int count, System.Data.IDbCommand command)
		{
			object data;

			this.sqlEngine.Execute (command, DbCommandType.NonQuery, count, out data);

			return data;
		}

		/// <summary>
		/// Executes the command attached to the transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The values of the output parameters of the command.</returns>
		public IList<object> ExecuteOutputParameters(DbTransaction transaction)
		{
			return this.ExecuteOutputParameters (transaction, transaction.SqlBuilder);
		}

		/// <summary>
		/// Executes the command defined by the SQL command builder.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="builder">The SQL command builder.</param>
		/// <returns>The values of the output parameters of the command.</returns>
		public IList<object> ExecuteOutputParameters(DbTransaction transaction, ISqlBuilder builder)
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
				IList<object> data;

				if (!this.IsLogEnabled ())
				{
					data = this.ExecuteOutputParameters (count, command);
				}
				else
				{
					System.DateTime startTime = System.DateTime.Now;
					System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew ();
					
					data = this.ExecuteOutputParameters (count, command);
					watch.Stop ();

					this.Log (l => l.AddEntry (command, startTime, watch.Elapsed, data));
				}

				return data;
			}
		}
		
		private IList<object> ExecuteOutputParameters(int count, System.Data.IDbCommand command)
		{
			IList<object> data;

			this.sqlEngine.Execute (command, DbCommandType.NonQuery, count, out data);

			return data;
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

				if (!this.IsLogEnabled ())
				{
					data = this.ExecuteRetData (count, command);
				}
				else
				{
					System.DateTime startTime = System.DateTime.Now;
					System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew ();

					data = this.ExecuteRetData (count, command);
					watch.Stop ();

					this.Log (l => l.AddEntry (command, startTime, watch.Elapsed, data));
				}

				return data;
			}
		}
		
		private System.Data.DataSet ExecuteRetData(int count, System.Data.IDbCommand command)
		{
			System.Data.DataSet data;

			this.sqlEngine.Execute (command, DbCommandType.ReturningData, count, out data);

			return data;
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
		
		public System.DateTime GetDatabaseTime()
		{
			using (DbTransaction transaction = this.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				transaction.SqlBuilder.GetCurrentTimeStamp ();

				object databaseTime = this.ExecuteScalar (transaction);

				transaction.Commit ();

				return (System.DateTime) databaseTime;
			}
		}


		private bool IsLogEnabled()
		{
			return this.QueryLogs.Any ();
		}


		private void Log(System.Action<AbstractLog> log)
		{
			foreach (AbstractLog queryLog in this.QueryLogs)
			{
				log (queryLog);
			}
		}

		
		/// <summary>
		/// Finds the key for the specified table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The table name.</param>
		/// <returns>The key to the table metadata.</returns>
		public DbKey FindDbTableKey(DbTransaction transaction, string name)
		{
			return this.FindDbKeys (transaction, Tags.TableTableDef, name).FirstOrDefault ();
		}

		/// <summary>
		/// Finds the key for the specified type.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="name">The type name.</param>
		/// <returns>The key to the type metadata.</returns>
		public DbKey FindDbTypeKey(DbTransaction transaction, string name)
		{
			return this.FindDbKeys (transaction, Tags.TableTypeDef, name).FirstOrDefault ();
		}

		internal IEnumerable<DbKey> FindDbKeys(DbTransaction transaction, string tableName, string rowName)
		{
			DbKey[] dbKeys;
			
			if (this.dbKeysCache.ContainsKey (tableName) && this.dbKeysCache[tableName].ContainsKey (rowName))
			{
				dbKeys = this.dbKeysCache[tableName][rowName];
			}
			else
			{
				dbKeys = this.FindDbKeysWithQuery (transaction, tableName, rowName).ToArray ();

				if (dbKeys.Length > 0)
				{
					if (!this.dbKeysCache.ContainsKey (tableName))
					{
						this.dbKeysCache[tableName] = new Dictionary<string, DbKey[]> ();
					}

					this.dbKeysCache[tableName][rowName] = dbKeys;
				}
			}

			return dbKeys;
		}

		/// <summary>
		/// Finds the keys for the named rows in the specified table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="rowName">Name of the row or rows.</param>
		/// <returns>The keys.</returns>
		internal IEnumerable<DbKey> FindDbKeysWithQuery(DbTransaction transaction, string tableName, string rowName)
		{
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",   SqlField.CreateName ("T", Tags.ColumnId));
			
			query.Tables.Add ("T", SqlField.CreateName (tableName));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, SqlField.CreateName ("T", Tags.ColumnName), SqlField.CreateConstant (rowName, DbRawType.String)));
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 0);

			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long  id = InvariantConverter.ToLong (row["T_ID"]);

				yield return new DbKey (id);
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

			transaction.SqlBuilder.SelectData (query);
			
			return InvariantConverter.ToInt (this.ExecuteScalar (transaction));
		}

		private void DeleteRow(DbTransaction transaction, string tableName, DbKey key)
		{
			SqlFieldList conditions  = new SqlFieldList ();

			DbInfrastructure.AddKeyExtraction (conditions, tableName, key);

			transaction.SqlBuilder.RemoveData (tableName, conditions);

			int numRowsAffected = InvariantConverter.ToInt (this.ExecuteNonQuery (transaction));

			if (numRowsAffected != 1)
			{
				throw new Exceptions.GenericException (this.access, string.Format ("Delete of row {0} in table {1} produced {2} deletions.", key, tableName, numRowsAffected));
			}
		}

		/// <summary>
		/// Loads the table definitions based on the metadata table key and the
		/// specified search mode.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="key">The table key or <c>DbKey.Empty</c> to load all table definitions based on the search mode.</param>
		/// <returns>The table definitions.</returns>
		public List<DbTable> LoadDbTable(DbTransaction transaction, DbKey key)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			//	We will build a join in order to query both the table definition
			//	and the column definitions with a single SQL request.

			SqlSelect query = new SqlSelect ();

			//	Table related informations :

			query.Fields.Add ("T_ID", SqlField.CreateName ("T_TABLE", Tags.ColumnId));
			query.Fields.Add ("T_NAME", SqlField.CreateName ("T_TABLE", Tags.ColumnName));
			query.Fields.Add ("T_D_NAME", SqlField.CreateName ("T_TABLE", Tags.ColumnDisplayName));
			query.Fields.Add ("T_INFO", SqlField.CreateName ("T_TABLE", Tags.ColumnInfoXml));

			query.OrderBy.Add ("T_ID", SqlField.CreateName ("T_TABLE", Tags.ColumnId), SqlSortOrder.Ascending);

			//	Column related informations :

			query.Fields.Add ("C_ID", SqlField.CreateName ("T_COLUMN", Tags.ColumnId));
			query.Fields.Add ("C_NAME", SqlField.CreateName ("T_COLUMN", Tags.ColumnName));
			query.Fields.Add ("C_D_NAME", SqlField.CreateName ("T_COLUMN", Tags.ColumnDisplayName));
			query.Fields.Add ("C_INFO", SqlField.CreateName ("T_COLUMN", Tags.ColumnInfoXml));
			query.Fields.Add ("C_TYPE", SqlField.CreateName ("T_COLUMN", Tags.ColumnRefType));
			query.Fields.Add ("C_TARGET", SqlField.CreateName ("T_COLUMN", Tags.ColumnRefTarget));

			query.OrderBy.Add ("C_ID", SqlField.CreateName ("T_COLUMN", Tags.ColumnId), SqlSortOrder.Ascending);

			//	Tables to query :

			query.Tables.Add ("T_TABLE", SqlField.CreateName (Tags.TableTableDef));
			query.Tables.Add ("T_COLUMN", SqlField.CreateName (Tags.TableColumnDef));

			if (key.IsEmpty)
			{
				//	Extract all tables and columns...

				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, "T_TABLE");
			}
			else
			{
				//	Extract only matching tables...

				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TABLE", key);
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_COLUMN", Tags.ColumnRefTable, key);
			}

			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 0);

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

						this.tableCache[tableKey] = dbTable;
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
		/// <param name="key">The type key or <c>DbKey.Empty</c> to load all type definitions based on the search mode.</param>
		/// <returns>The type definitions.</returns>
		public List<DbTypeDef> LoadDbType(DbTransaction transaction, DbKey key)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID",     SqlField.CreateName ("T_TYPE", Tags.ColumnId));
			query.Fields.Add ("T_NAME",   SqlField.CreateName ("T_TYPE", Tags.ColumnName));
			query.Fields.Add ("T_D_NAME", SqlField.CreateName ("T_TYPE", Tags.ColumnDisplayName));
			query.Fields.Add ("T_INFO",   SqlField.CreateName ("T_TYPE", Tags.ColumnInfoXml));
			
			query.Tables.Add ("T_TYPE", SqlField.CreateName (Tags.TableTypeDef));
			
			if (!key.IsEmpty)
			{
				DbInfrastructure.AddKeyExtraction (query.Conditions, "T_TYPE", key);
			}
			
			System.Data.DataTable dataTable = this.ExecuteSqlSelect (transaction, query, 0);
			List<DbTypeDef> types = new List<DbTypeDef> ();

			foreach (System.Data.DataRow row in dataTable.Rows)
			{
				long   typeId          = InvariantConverter.ToLong (row["T_ID"]);
				string typeName        = InvariantConverter.ToString (row["T_NAME"]);
				string typeDisplayName = InvariantConverter.ToString (row["T_D_NAME"]);
				string typeInfo        = InvariantConverter.ToString (row["T_INFO"]);
				DbKey  typeKey		   = new DbKey (typeId);

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
		/// Adds a SELECT extraction condition for a key in a table.
		/// </summary>
		/// <param name="conditions">The conditions.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="key">The key.</param>
		private static void AddKeyExtraction(SqlFieldList conditions, string tableName, DbKey key)
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
		private static void AddKeyExtraction(SqlFieldList conditions, string sourceTableName, string sourceColumnName, string targetTableName)
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
		private static void AddKeyExtraction(SqlFieldList conditions, string sourceTableName, string sourceColumnName, DbKey key)
		{
			SqlField sourceColId = SqlField.CreateName (sourceTableName, sourceColumnName);
			SqlField constantId  = SqlField.CreateConstant (key.Id, DbKey.RawTypeForId);

			conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, sourceColId, constantId));
		}

		/// <summary>
		/// Sets up the metadata table definitions by filling them with the
		/// defaults required by an empty database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		private void SetupTables(DbTransaction transaction)
		{
			//	First, fill the type table so that we can reference them from
			//	the column definition table :
			
			foreach (DbTypeDef typeDef in this.internalTypes)
			{
				DbKey dbKey = this.InsertTypeDefRow (transaction, typeDef);
				typeDef.DefineKey (dbKey);
			}
			
			//	Then, fill the table definition table and the column definition
			//	table :
			
			foreach (DbTable table in this.internalTables)
			{
				table.UpdatePrimaryKeyInfo ();
				DbKey tableDbKey = this.InsertTableDefRow (transaction, table);
				table.DefineKey (tableDbKey);
				
				foreach (DbColumn column in table.Columns)
				{
					DbKey columnDbKey = this.InsertColumnDefRow (transaction, table, column);
					column.DefineKey (columnDbKey);
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
		}

		/// <summary>
		/// Updates the column relation information. This will record the relation
		/// in the CR_COLUMN_DEF table by specifying the target table for a given
		/// source column.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="sourceColumnKey">The source column key.</param>
		/// <param name="targetTableKey">The target table key.</param>
		private void UpdateColumnRelation(DbTransaction transaction, DbKey sourceColumnKey, DbKey targetTableKey)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			System.Diagnostics.Debug.Assert (sourceColumnKey != null);
			System.Diagnostics.Debug.Assert (targetTableKey  != null);

			SqlFieldList fields = new SqlFieldList ();
			SqlFieldList conds  = new SqlFieldList ();

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

			this.UpdateColumnRelation (transaction, column.Key, target.Key);
		}
		
		/// <summary>
		/// Inserts a type definition row into the CR_TYPE_DEF table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="typeDef">The type definition.</param>
		private DbKey InsertTypeDefRow(DbTransaction transaction, DbTypeDef typeDef)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			DbTable typeDefTable = this.internalTables[Tags.TableTypeDef];

			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnName], typeDef.Name),
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnDisplayName], typeDef.DisplayName),
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnInfoXml], DbTools.GetCompactXml (typeDef)),
			};

			SqlFieldList fieldsToReturn = new SqlFieldList ()
			{
				new SqlField() { Alias = typeDefTable.Columns[Tags.ColumnId].GetSqlName (), },
			};

			transaction.SqlBuilder.InsertData (typeDefTable.GetSqlName (), fieldsToInsert, fieldsToReturn);
			object data = this.ExecuteScalar (transaction);

			return new DbKey (new DbId ((long) data));
		}
		
		private void UpdateTypeDefRow(DbTransaction transaction, DbTypeDef typeDef)
		{
			DbTable typeDefTable = this.internalTables[Tags.TableTypeDef];
			string tableName = typeDefTable.GetSqlName ();
			
			SqlFieldList fieldsToUpdate = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnName], typeDef.Name),
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnDisplayName], typeDef.DisplayName),
				this.CreateSqlFieldFromAdoValue (typeDefTable.Columns[Tags.ColumnInfoXml], DbTools.GetCompactXml (typeDef)),
			};

			SqlFieldList conditions = new SqlFieldList ();
			DbInfrastructure.AddKeyExtraction (conditions, tableName, typeDef.Key);

			transaction.SqlBuilder.UpdateData (tableName, fieldsToUpdate, conditions);
			this.ExecuteNonQuery (transaction);
		}
		
		private void DeleteTypeDefRow(DbTransaction transaction, DbTypeDef typeDef)
		{
			DbKey key = typeDef.Key;

			this.DeleteRow (transaction, Tags.TableTypeDef, key);
		}
		
		/// <summary>
		/// Inserts a table definition row into the CR_TABLE_DEF table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		private DbKey InsertTableDefRow(DbTransaction transaction, DbTable table)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			DbTable tableDefTable = this.internalTables[Tags.TableTableDef];

			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnName],        table.Name),
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnDisplayName], table.DisplayName),
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnInfoXml],     DbTools.GetCompactXml (table)),
			};

			SqlFieldList fieldsToReturn = new SqlFieldList ()
			{
				new SqlField() { Alias = tableDefTable.Columns[Tags.ColumnId].GetSqlName (), },
			};

			transaction.SqlBuilder.InsertData (tableDefTable.GetSqlName (), fieldsToInsert, fieldsToReturn);
			object data = this.ExecuteScalar (transaction);

			return new DbKey (new DbId ((long) data));
		}

		private void UpdateTableDefRow(DbTransaction transaction, DbTable table)
		{
			DbTable tableDefTable = this.internalTables[Tags.TableTableDef];
			string tableName = tableDefTable.GetSqlName ();
			
			SqlFieldList fields = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnName], table.Name),
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnDisplayName], table.DisplayName),
				this.CreateSqlFieldFromAdoValue (tableDefTable.Columns[Tags.ColumnInfoXml], DbTools.GetCompactXml (table)),
			};

			SqlFieldList conditions = new SqlFieldList ();
			DbInfrastructure.AddKeyExtraction (conditions, tableName, table.Key);

			transaction.SqlBuilder.UpdateData (tableName, fields, conditions);
			this.ExecuteNonQuery (transaction);
		}
		
		private void DeleteTableDefRow(DbTransaction transaction, DbTable table)
		{
			DbKey key = table.Key;

			this.DeleteRow (transaction, Tags.TableTableDef, key);
		}

		/// <summary>
		/// Inserts a column definition row into the CR_COLUMN_DEF table. This
		/// does not generate the relation between the column and the target
		/// table, if any.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		/// <param name="column">The column definition.</param>
		private DbKey InsertColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			DbTable columnDefTable = this.internalTables[Tags.TableColumnDef];

			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnName],        column.Name),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnDisplayName], column.DisplayName),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnInfoXml],     DbTools.GetCompactXml (column)),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefTable],    table.Key.Id),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefType],     column.Type == null ? 0 : column.Type.Key.Id),
			};

			SqlFieldList fieldsToReturn = new SqlFieldList ()
			{
				new SqlField() { Alias = columnDefTable.Columns[Tags.ColumnId].GetSqlName (), },
			};

			transaction.SqlBuilder.InsertData (columnDefTable.GetSqlName (), fieldsToInsert, fieldsToReturn);
			object data = this.ExecuteScalar (transaction);

			return new DbKey (new DbId ((long) data));
		}

		private DbKey InsertRelationColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column, DbTable targetTable)
		{
			System.Diagnostics.Debug.Assert (transaction != null);

			DbTable columnDefTable = this.internalTables[Tags.TableColumnDef];

			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnName], column.Name),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnDisplayName], column.DisplayName),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnInfoXml], DbTools.GetCompactXml (column)),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefTable], table.Key.Id),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefType], column.Type == null ? 0 : column.Type.Key.Id),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefTarget], targetTable.Key.Id),
			};

			SqlFieldList fieldsToReturn = new SqlFieldList ()
			{
				new SqlField() { Alias = columnDefTable.Columns[Tags.ColumnId].GetSqlName (), },
			};

			transaction.SqlBuilder.InsertData (columnDefTable.GetSqlName (), fieldsToInsert, fieldsToReturn);
			object data = this.ExecuteScalar (transaction);

			return new DbKey (new DbId ((long) data));
		}

		private void UpdateColumnDefRow(DbTransaction transaction, DbTable table, DbColumn column)
		{
			DbTable columnDefTable = this.internalTables[Tags.TableColumnDef];
			string tableName = columnDefTable.GetSqlName ();

			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnName], column.Name),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnDisplayName], column.DisplayName),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnInfoXml], DbTools.GetCompactXml (column)),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefTable], table.Key.Id),
				this.CreateSqlFieldFromAdoValue (columnDefTable.Columns[Tags.ColumnRefType], column.Type == null ? 0 : column.Type.Key.Id),
			};

			SqlFieldList conditions = new SqlFieldList ();
			DbInfrastructure.AddKeyExtraction (conditions, tableName, column.Key);

			transaction.SqlBuilder.UpdateData (columnDefTable.GetSqlName (), fieldsToInsert, conditions);
			this.ExecuteNonQuery (transaction);
		}	

		private void DeleteColumnDefRow(DbTransaction transaction, DbColumn column)
		{
			SqlFieldList conditions = new SqlFieldList ()
			{
				SqlField.CreateFunction
				(
					new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						SqlField.CreateName (Tags.TableColumnDef, Tags.ColumnId),
						SqlField.CreateConstant (column.Key.Id, DbKey.RawTypeForId)
					)
				),
			};

			transaction.SqlBuilder.RemoveData (Tags.TableColumnDef, conditions);
			this.ExecuteNonQuery (transaction);
		}
		
		private void DeleteColumnDefRows(DbTransaction transaction, DbTable table)
		{
			SqlFieldList conditions = new SqlFieldList ()
			{
				SqlField.CreateFunction
				(
					new SqlFunction
					(
						SqlFunctionCode.CompareEqual,
						SqlField.CreateName (Tags.TableColumnDef, Tags.ColumnRefTable),
						SqlField.CreateConstant (table.Key.Id, DbKey.RawTypeForId)
					)
				),
			};

			transaction.SqlBuilder.RemoveData (Tags.TableColumnDef, conditions);
			this.ExecuteNonQuery (transaction);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.globalLock != null)
				{
					this.globalLock.Dispose ();
					this.globalLock = null;
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

			base.Dispose (disposing);
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
		
		private static class BootHelper
		{

			public static void RegisterTables(DbInfrastructure infrastructure, DbTransaction transaction, IEnumerable<DbTable> tables)
			{
				BootHelper.RegisterToDatabase (infrastructure, transaction, tables);
				BootHelper.RegisterToDbInfrastructure (infrastructure, tables);
			}

			private static void RegisterToDatabase(DbInfrastructure infrastructure, DbTransaction transaction, IEnumerable<DbTable> tables)
			{
				foreach (DbTable table in tables)
				{
					infrastructure.InsertTable (transaction, table);
				}
			}

			private static void RegisterToDbInfrastructure(DbInfrastructure infrastructure, IEnumerable<DbTable> tables)
			{
				infrastructure.internalTables.AddRange (tables);
			}

			public static IEnumerable<DbTable> CreateCoreTables(DbInfrastructure infrastructure)
			{
				yield return BootHelper.CreateTableTableDef (infrastructure);
				yield return BootHelper.CreateTableColumnDef (infrastructure);
				yield return BootHelper.CreateTableTypeDef (infrastructure);
			}

			public static void UpdateCoreTableRelations(DbTable tableDef, DbTable columnDef, DbTable typeDef)
			{
				columnDef.Columns[Tags.ColumnRefTable].DefineTargetTableName (tableDef.GetSqlName ());
				columnDef.Columns[Tags.ColumnRefType].DefineTargetTableName (typeDef.GetSqlName ());
				columnDef.Columns[Tags.ColumnRefTarget].DefineTargetTableName (tableDef.GetSqlName ());
			}

			private static DbTable CreateTableTableDef(DbInfrastructure infrastructure)
			{
				TypeHelper types = infrastructure.types;

				DbTable table = new DbTable (Tags.TableTableDef);
				table.DefineCategory (DbElementCat.Internal);

				DbColumn[] columns = new DbColumn[]
				{
					new DbColumn(Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal) { IsAutoIncremented = true },
					new DbColumn(Tags.ColumnName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnDisplayName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnInfoXml, types.InfoXml, DbColumnClass.Data, DbElementCat.Internal),
				};

				table.DefineCategory (DbElementCat.Internal);
				table.Columns.AddRange (columns);
				table.DefinePrimaryKey (columns[0]);

				return table;
			}

			private static DbTable CreateTableColumnDef(DbInfrastructure infrastructure)
			{
				TypeHelper types = infrastructure.types;

				DbTable table = new DbTable (Tags.TableColumnDef);
				table.DefineCategory (DbElementCat.Internal);

				DbColumn[] columns = new DbColumn[]
				{
					new DbColumn(Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal) { IsAutoIncremented = true },
					new DbColumn(Tags.ColumnName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnDisplayName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnInfoXml, types.InfoXml, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnRefTable, types.KeyId, DbColumnClass.RefId, DbElementCat.Internal),
					new DbColumn(Tags.ColumnRefType, types.KeyId, DbColumnClass.RefId, DbElementCat.Internal),
					new DbColumn(Tags.ColumnRefTarget, types.KeyId, DbColumnClass.RefId, DbElementCat.Internal) { IsNullable = true },
				};

				table.DefineCategory (DbElementCat.Internal);
				table.Columns.AddRange (columns);
				table.DefinePrimaryKey (columns[0]);

				return table;
			}

			private static DbTable CreateTableTypeDef(DbInfrastructure infrastructure)
			{
				TypeHelper types = infrastructure.types;

				DbTable table = new DbTable (Tags.TableTypeDef);
				table.DefineCategory (DbElementCat.Internal);

				DbColumn[] columns = new DbColumn[]
				{
					new DbColumn(Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal) { IsAutoIncremented = true },
					new DbColumn(Tags.ColumnName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnDisplayName, types.Name, DbColumnClass.Data, DbElementCat.Internal),
					new DbColumn(Tags.ColumnInfoXml, types.InfoXml, DbColumnClass.Data, DbElementCat.Internal),
				};

				table.DefineCategory (DbElementCat.Internal);
				table.Columns.AddRange (columns);
				table.DefinePrimaryKey (columns[0]);

				return table;
			}

		}
		
		#endregion
		
		#region TypeHelper Class
		
		internal sealed class TypeHelper
		{

			// TODO I'm not convinced that how I implemented the stuff for the default types is the
			// most elegant one. It might require some kind of refactoring, or maybe the usage of
			// resources. I don't really know.
			// Marc

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

			public DbTypeDef					DefaultInteger
			{
				get
				{
					return this.defaultInteger;
				}
			}

			public DbTypeDef					DefaultLongInteger
			{
				get
				{
					return this.defaultLongInteger;
				}
			}

			public DbTypeDef					DefaultString
			{
				get
				{
					return this.defaultString;
				}
			}
			
			public void RegisterTypes()
			{
				this.InitializeNumTypes ();
				this.InitializeStrTypes ();
				this.InitializeOtherTypes ();
				this.InitializeDefaultTypes ();
				
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

				this.defaultInteger		   = this.infrastructure.ResolveDbType (transaction, new DbTypeDef (IntegerType.Default).Name);
				this.defaultLongInteger	   = this.infrastructure.ResolveDbType (transaction, new DbTypeDef (LongIntegerType.Default).Name);
				this.defaultString		   = this.infrastructure.ResolveDbType (transaction, new DbTypeDef (StringType.Default).Name);
				
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

				this.infrastructure.internalTypes.Add (this.defaultInteger);
				this.infrastructure.internalTypes.Add (this.defaultLongInteger);
				this.infrastructure.internalTypes.Add (this.defaultString);
				
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

			public void InitializeDefaultTypes()
			{
				//var defaultStringType = Epsitec.Common.Types.Res.Types.Default.String;

				this.defaultInteger		= new DbTypeDef (IntegerType.Default);
				this.defaultLongInteger = new DbTypeDef (LongIntegerType.Default);
				this.defaultString      = new DbTypeDef (StringType.Default);

				this.infrastructure.internalTypes.Add (this.defaultInteger);
				this.infrastructure.internalTypes.Add (this.defaultLongInteger);
				this.infrastructure.internalTypes.Add (this.defaultString);
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
				
				System.Diagnostics.Debug.Assert (this.defaultInteger != null);
				System.Diagnostics.Debug.Assert (this.defaultLongInteger != null);
				System.Diagnostics.Debug.Assert (this.defaultString != null);
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

			private DbTypeDef					defaultInteger;
			private DbTypeDef					defaultLongInteger;
			private DbTypeDef					defaultString;
		}
		
		#endregion
		
		private DbAccess						access;
		private IDbAbstraction					abstraction;

		private ISqlEngine						sqlEngine;
		private ITypeConverter					converter;
		
		private TypeHelper						types;

		private readonly DbTableList internalTables;
		private readonly DbTypeDefList internalTypes;

		private readonly Cache.DbTypeDefs		typeCache = new Cache.DbTypeDefs ();
		private readonly Cache.DbTables			tableCache = new Cache.DbTables ();
		private Dictionary<string, Dictionary<string, DbKey[]>> dbKeysCache;

		private List<DbTransaction>				liveTransactions;
		private List<IDbAbstraction>			releaseRequested;

		private System.TimeSpan					lockTimeout = System.TimeSpan.FromSeconds (15);
		private ReaderWriterLockSlim			globalLock = new ReaderWriterLockSlim (LockRecursionPolicy.SupportsRecursion);

	}
}
