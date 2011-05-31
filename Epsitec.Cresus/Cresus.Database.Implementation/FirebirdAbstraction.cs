//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using FirebirdSql.Data.FirebirdClient;

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdAbstraction</c> class implements <c>IDbAbstraction</c> for Firebird.
	/// </summary>
	internal sealed class FirebirdAbstraction : IDbAbstraction, System.IDisposable
	{
		public FirebirdAbstraction(DbAccess dbAccess, IDbAbstractionFactory dbFactory, EngineType engineType)
		{
			switch (engineType)
			{
				case EngineType.Server:
					this.serverType = FbServerType.Default;
					this.engineType = engineType;
					break;

				case EngineType.Embedded:
					FirebirdAbstraction.LoadFirebirdEmbedded ();
					this.serverType = FbServerType.Embedded;
					this.engineType = engineType;
					break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			this.dbAccess  = dbAccess;
			this.dbFactory = dbFactory;

			string path = this.GetDbFilePath ();

			this.dbConnectionString = FirebirdAbstraction.MakeConnectionString (this.dbAccess, path, this.serverType);
			
			if (this.dbAccess.CreateDatabase)
			{
				//	Si l'appelant a demandé la création de la base, commence par tenter d'ouvrir une
				//	base existante. Si celle-ci existe, c'est considéré comme une erreur, et on génère
				//	une exception.
				
				this.CreateConnection (ignoreErrors: true);
				
				if (this.dbConnection == null)
				{
					this.CreateDatabase ();
					this.CreateConnection ();
				}
				else
				{
					throw new Exceptions.ExistsException (this.dbAccess, "Cannot create existing database, it already exists");
				}
			}
			else
			{
				this.CreateConnection (testConnection: this.dbAccess.CheckConnection, ignoreErrors: this.dbAccess.IgnoreInitialConnectionErrors);
			}
			
			this.sqlBuilder = new FirebirdSqlBuilder (this);
			this.sqlEngine  = new FirebirdSqlEngine (this);
			this.dbServiceTools = new FirebirdServiceTools (this);
		}

		public DbAccess							DbAccess
		{
			get
			{
				return this.dbAccess;
			}
		}

		public FbServerType						ServerType
		{
			get
			{
				return this.serverType;
			}
		}
		
		
		private void CreateConnection(bool testConnection = true, bool ignoreErrors = false)
		{
			try
			{
				this.dbConnection = new FbConnection (this.dbConnectionString);
				
				if (testConnection)
				{
					this.TestConnection ();
				}
			}
			catch
			{
				if (this.dbConnection != null)
				{
					this.dbConnection.Dispose ();
					this.dbConnection = null;
				}

				if (ignoreErrors)
				{
					//	Swallow exception silently
				}
				else
				{
					throw;
				}
			}
		}

		private void TestConnection()
		{
			switch (this.dbConnection.State)
			{
				case System.Data.ConnectionState.Closed:
					this.dbConnection.Open ();
					this.dbConnection.Close ();
					break;
				
				case System.Data.ConnectionState.Broken:
					this.dbConnection.Close ();
					this.dbConnection.Open ();
					break;
			}
		}

		private void EnsureConnection()
		{
			if (this.autoReopenConnection)
			{
				this.autoReopenConnection = false;
				this.dbConnection.Open ();
			}
			else
			{
				switch (this.dbConnection.State)
				{
					case System.Data.ConnectionState.Closed:
						this.dbConnection.Open ();
						break;
					
					case System.Data.ConnectionState.Broken:
						this.dbConnection.Close ();
						this.dbConnection.Open ();
						break;
				}
			}

			System.Diagnostics.Debug.Assert (this.dbConnection.State == System.Data.ConnectionState.Open);
		}

		private void CreateDatabase()
		{
			System.Diagnostics.Debug.Assert (this.dbAccess.CreateDatabase);
			
			string path = this.GetDbFilePath ();

			string connection = FirebirdAbstraction.MakeConnectionStringForDbCreation (this.dbAccess, path, this.serverType);
			
			FbConnection.CreateDatabase (connection, FirebirdAbstraction.fbPageSize, true, false);
		}


		public void DropDatabase()
		{
			string path = this.GetDbFilePath ();
			string connection = FirebirdAbstraction.MakeConnectionString (this.dbAccess, path, this.serverType);
			
			FbConnection.DropDatabase (connection);
		}


		internal string GetDbFilePath()
		{
			return FirebirdAbstraction.MakeDbFilePath (this.dbAccess, this.engineType);
		}
		
		public static string MakeConnectionStringForDbCreation(DbAccess dbAccess, string path, FbServerType serverType)
		{
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.LoginName);
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.LoginPassword);
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.Server);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder ();

			cs.UserID     = dbAccess.LoginName;
			cs.Password   = dbAccess.LoginPassword;
			cs.DataSource = dbAccess.Server;
			cs.Port       = FirebirdAbstraction.fbPort;
			cs.Database   = path;
			cs.Dialect    = FirebirdAbstraction.fbDialect;
			cs.Charset    = FirebirdAbstraction.fbCharset;
			cs.ServerType = serverType;

			return cs.ConnectionString;
		}
		
		public static string MakeConnectionString(DbAccess dbAccess, string path, FbServerType serverType)
		{
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.LoginName);
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.LoginPassword);
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.Server);

			FbConnectionStringBuilder cs = new FbConnectionStringBuilder
			{
				UserID				= dbAccess.LoginName,
				Password			= dbAccess.LoginPassword,
				DataSource			= dbAccess.Server,
				Database			= path,
				Port				= FirebirdAbstraction.fbPort,
				Dialect				= FirebirdAbstraction.fbDialect,
				PacketSize			= FirebirdAbstraction.fbPageSize,
				ServerType			= serverType,
				Charset				= FirebirdAbstraction.fbCharset,
				Pooling				= false,
				ConnectionLifeTime	= 0,			
			};

			return cs.ConnectionString;
		}

		[System.Runtime.InteropServices.DllImport ("Kernel32.dll")]
		private static extern System.IntPtr LoadLibrary(string fullpath);
		
		[System.Runtime.InteropServices.DllImport ("Kernel32.dll")]
		private static extern bool SetDllDirectory(string pathName);
		
		private static void LoadFirebirdEmbedded()
		{
			if (FirebirdAbstraction.fbEmbeddedLoaded)
			{
				return;
			}
			
			try
			{
				string   dllName    = "fbembed.dll";
				string   dllPath    = null;
				string[] probePaths = new string[]
				{
					Epsitec.Common.Support.Globals.Directories.ExecutableRoot,
					Epsitec.Common.Support.Globals.Directories.InitialDirectory,
				};

				foreach (string path in probePaths)
				{
					if (System.IO.File.Exists (System.IO.Path.Combine (path, dllName)))
					{
						dllPath = path;
						break;
					}
				}

				if (dllPath == null)
				{
					System.Diagnostics.Debug.Fail ("Could not locate fbembed.dll");
				}
				else
				{
					FirebirdAbstraction.SetDllDirectory (dllPath);
				}

				FirebirdAbstraction.fbEmbeddedLoaded = true;
			}
			catch
			{
				//	Never mind if we cannot set the DLL directory; this probably means that we
				//	are running on a system older than XP SP1 or Server 2003.
			}
		}
		
		public static string MakeDbFilePath(DbAccess dbAccess, EngineType engineType)
		{
			FirebirdAbstraction.ValidateName (dbAccess, dbAccess.Database);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			switch (engineType)
			{
				case EngineType.Embedded:
					buffer.Append (Globals.Directories.UserAppData);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				case EngineType.Server:
					buffer.Append (FirebirdAbstraction.fbRootDbPath);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				default:
					throw new Database.Exceptions.FactoryException (string.Format ("EngineType {0} not supported.", engineType));
			}

			buffer.Append (dbAccess.Database);
			buffer.Append (FirebirdAbstraction.fbDbFileExtension);
			
			return buffer.ToString ();
		}
		
		
		private static void ValidateName(DbAccess dbAccess, string name)
		{
			if (name.Length > 100)
			{
				throw new Exceptions.SyntaxException (dbAccess, string.Format ("Name is to long (length={0})", name));
			}
		}

		#region IDisposable Members

		void System.IDisposable.Dispose()
		{
			if (this.sqlBuilder != null)
			{
				this.sqlBuilder.Dispose ();
				this.sqlBuilder = null;
			}

			if (this.sqlEngine != null)
			{
				this.sqlEngine.Dispose ();
				this.sqlEngine = null;
			}

			if (this.dbConnection != null)
			{
				this.dbConnection.Dispose ();
				this.dbConnection = null;
			}
		}

		#endregion

		#region IDbAbstraction Members
		
		public IDbAbstractionFactory				Factory
		{
			get
			{
				return this.dbFactory;
			}
		}

		public System.Data.IDbConnection			Connection
		{
			get
			{
				return this.dbConnection;
			}
		}

		public ISqlBuilder							SqlBuilder
		{
			get
			{
				return this.sqlBuilder;
			}
		}

		public ISqlEngine							SqlEngine
		{
			get
			{
				return this.sqlEngine;
			}
		}

		public IDbServiceTools						ServiceTools
		{
			get
			{
				return this.dbServiceTools;
			}
		}

		public bool									IsConnectionInitialized
		{
			get
			{
				return this.dbConnection != null;
			}
		}
		
		public bool									IsConnectionOpen
		{
			get
			{
				if (this.dbConnection != null)
				{
					return this.dbConnection.State != System.Data.ConnectionState.Closed;
				}
				else
				{
					return false;
				}
			}
		}
		
		public bool									IsConnectionAlive
		{
			get
			{
				return this.IsConnectionOpen && (this.dbConnection.State != System.Data.ConnectionState.Broken);
			}
		}
		
		
		public string[] QueryUserTableNames()
		{
			this.EnsureConnection ();
			
			List<string> list = new List<string> ();
			
			// TODO Why is there some null elements in the string array argument of this call ?
			// Marc

			System.Data.DataTable tables = this.dbConnection.GetSchema ("Tables", new string[] { null, null, null, "TABLE" });

			foreach (System.Data.DataRow row in tables.Rows)
			{
				list.Add (row["TABLE_NAME"] as string);
			}

			return list.ToArray ();
		}

		public string QueryDatabaseFolderPath()
		{
			return FirebirdAbstraction.fbRootDbPath;
		}
		
		public System.Data.IDbCommand NewDbCommand()
		{
			this.EnsureConnection ();
			
			return this.dbConnection.CreateCommand ();
		}
		
		public System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command)
		{
			this.EnsureConnection ();
			
			System.Diagnostics.Debug.Assert (command.Connection != null);
			System.Diagnostics.Debug.Assert (command.Connection.State != System.Data.ConnectionState.Closed);
			
			FbCommand fbCommand = command as FbCommand;
			
			if (fbCommand == null)
			{
				throw new Exceptions.GenericException (this.dbAccess, "Invalid command object");
			}
			
			return new FbDataAdapter (fbCommand);
		}


		/// <summary>
		/// Begins a read only transaction.
		/// </summary>
		/// <returns>The database transaction object.</returns>
		public System.Data.IDbTransaction BeginReadOnlyTransaction()
		{
			return this.BeginReadOnlyTransaction (new List<DbTable> ());
		}

		/// <summary>
		/// Begins a read only transaction that locks the given <see cref="DbTable"/> for shared
		/// read access.
		/// </summary>
		/// <param name="tablesToLock">The <see cref="DbDable"/> to lock.</param>
		/// <returns>The database transaction object.</returns>
		public System.Data.IDbTransaction BeginReadOnlyTransaction(IEnumerable<DbTable> tablesToLock)
		{
			this.EnsureConnection ();

			FbTransactionOptions options = new FbTransactionOptions ()
			{
				TransactionBehavior = FbTransactionBehavior.Concurrency
									| FbTransactionBehavior.Wait
									| FbTransactionBehavior.Read,
				LockTables = this.GetLockTables (tablesToLock, FbTransactionBehavior.LockRead | FbTransactionBehavior.Shared),
			};

			return this.dbConnection.BeginTransaction (options);
		}

		/// <summary>
		/// Begins a read-write transaction.
		/// </summary>
		/// <returns>The database transaction object.</returns>
		public System.Data.IDbTransaction BeginReadWriteTransaction()
		{
			return this.BeginReadWriteTransaction (new List<DbTable> ());
		}

		/// <summary>
		/// Begins a read-write transaction that locks the given <see cref="DbTable"/> for exclusive
		/// write access.
		/// </summary>
		/// <param name="tablesToLock">The <see cref="DbDable"/> to lock.</param>
		/// <returns>The database transaction object.</returns>
		public System.Data.IDbTransaction BeginReadWriteTransaction(IEnumerable<DbTable> tablesToLock)
		{
			this.EnsureConnection ();

			FbTransactionOptions options = new FbTransactionOptions ()
			{
				TransactionBehavior = FbTransactionBehavior.Concurrency
									| FbTransactionBehavior.Wait
									| FbTransactionBehavior.Write,
				LockTables = this.GetLockTables (tablesToLock, FbTransactionBehavior.LockWrite | FbTransactionBehavior.Exclusive),
			};

			return this.dbConnection.BeginTransaction (options);
		}

		/// <summary>
		/// Builds the table locking argument that must be given to an <see cref="FbTransactionBehavior"/>
		/// object in order to configure a <see cref="FbTransaction"/>.
		/// </summary>
		/// <param name="tablesToLock">The <see cref="DbTable"/> to lock.</param>
		/// <param name="behavior">The mode to used to lock the <see cref="DbTable"/>.</param>
		/// <returns>The object that must be used to configure the <see cref="FbTransaction"/>.</returns>
		private IDictionary<string, FbTransactionBehavior> GetLockTables(IEnumerable<DbTable> tablesToLock, FbTransactionBehavior behavior)
		{
			Dictionary<string, FbTransactionBehavior> locks = new Dictionary<string, FbTransactionBehavior> ();

			foreach (DbTable table in tablesToLock)
			{
				locks[table.GetSqlName ()] = behavior;
			}

			return locks;
		}

		public void ReleaseConnection()
		{
			//	Ferme la connexion à la base de données, en mettant un fanion pour ré-établir
			//	automatiquement la connexion en cas de besoin.
			
			if (this.dbConnection.State != System.Data.ConnectionState.Closed)
			{
				this.dbConnection.Close ();
				this.autoReopenConnection = true;
			}
		}
		
		#endregion

		static FirebirdAbstraction()
		{
			string path;

			path = System.Environment.GetFolderPath (System.Environment.SpecialFolder.CommonApplicationData);
			path = System.IO.Path.Combine (path, "Epsitec");
			path = System.IO.Path.Combine (path, "Firebird Databases");

			FirebirdAbstraction.fbRootDbPath = path;
		}
		
		
		private FbServerType					serverType;
		private EngineType						engineType;
										
		private DbAccess						dbAccess;
		private IDbAbstractionFactory			dbFactory;
		private FbConnection					dbConnection;
		private string							dbConnectionString;
		private FirebirdSqlBuilder				sqlBuilder;
		private FirebirdSqlEngine				sqlEngine;
		private FirebirdServiceTools			dbServiceTools;
										
		private bool							autoReopenConnection;
										
		private static readonly int				fbPort				= 3050;
		private static readonly byte			fbDialect			= 3;
		private static readonly short			fbPageSize			= 8192;
		private static readonly string			fbCharset			= "UTF8";
		private static readonly string			fbRootDbPath;
		private static readonly string			fbDbFileExtension	= ".firebird";

		private static bool						fbEmbeddedLoaded;
	}
}
