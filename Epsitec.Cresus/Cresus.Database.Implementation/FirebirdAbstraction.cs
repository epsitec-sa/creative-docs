//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

using FirebirdSql.Data.FirebirdClient;

using System.Collections.Generic;
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
				if ((testConnection) &&
					(ignoreErrors) &&
					(this.dbAccess.IsLocalHost))
				{
					if (!FirebirdAbstractionFactory.GetDatabaseFilePaths (this.dbAccess).Any (path => System.IO.File.Exists (path)))
                    {
						//	We know that we will fail - no need to try, the file is
						//	missing.

						return;
                    }
				}

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
			
			//	L'appel FbConnection.CreateDatabase ne sait pas créer le dossier, si nécessaire.
			//	Il faut donc que nous le créions nous-même s'il n'existe pas encore.

			string path = this.GetDbFilePath ();
			System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (path));

			string connection = FirebirdAbstraction.MakeConnectionStringForDbCreation (this.dbAccess, path, this.serverType);
			
			FbConnection.CreateDatabase (connection, FirebirdAbstraction.fbPageSize, true, false);
		}


		public void DropDatabase()
		{
			string path = this.GetDbFilePath ();
			string connection = FirebirdAbstraction.MakeConnectionString (this.dbAccess, path, this.serverType);

			// HACK This waits a little bit of time, hoping that at the end, there is no transaction
			// or lock around and that the drop can be done. It would be nice to do this in a proper
			// way.
			System.Threading.Thread.Sleep (5000);

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
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.AppendFormat ("User={0};", dbAccess.LoginName);
			buffer.AppendFormat ("Password={0};", dbAccess.LoginPassword);
			buffer.AppendFormat ("Data Source={0};", dbAccess.Server);
			buffer.AppendFormat ("Database={0};", path);
			buffer.AppendFormat ("Port={0};", FirebirdAbstraction.fbPort);
			buffer.AppendFormat ("Dialect={0};", FirebirdAbstraction.fbDialect);
			buffer.AppendFormat ("Packet Size={0};", FirebirdAbstraction.fbPageSize);
			buffer.AppendFormat ("Server Type={0};", serverType == FbServerType.Embedded ? 1 : 0);
			buffer.AppendFormat ("Charset={0};", FirebirdAbstraction.fbCharset);
			
			buffer.Append ("Role=;");
			buffer.Append ("Pooling=true;");
			buffer.Append ("Connection Lifetime=2;");

			if (serverType == FbServerType.Embedded)
			{
				FirebirdAbstraction.LoadFirebirdEmbedded ();
			}
			
			return buffer.ToString ();
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

			// TODO This check is too restrictive, it should be relaxed. In the meantime, I disabled
			// it.
			//if (RegexFactory.AlphaNumDotName2.IsMatch (name) == false)
			//{
			//    throw new Exceptions.SyntaxException (dbAccess, string.Format ("{0} contains an invalid character", name));
			//}
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
			
			System.Data.DataTable tables = this.dbConnection.GetSchema ("Tables", new string[] { null, null, null, "TABLE" });

			foreach (System.Data.DataRow row in tables.Rows)
			{
				list.Add (row["TABLE_NAME"] as string);
			}

			return list.ToArray ();
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
		
		// TODO The options given to the BeginTransaction(...) method are wrong and cause a deadlock
		// when more than one DataContext tries to write in the database. It should be something like
		// Concurrency | Protected | Wait | Read and Concurrency | Protected | Wait | Write, but I
		// couldn't manage to make it run correctly.
		// Marc


		public System.Data.IDbTransaction BeginReadOnlyTransaction()
		{
			this.EnsureConnection ();
			
#if false
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Read;
			
			return this.dbConnection.BeginTransaction (options);
#else
			return this.dbConnection.BeginTransaction (
				new FbTransactionOptions ()
				{
					TransactionBehavior = FbTransactionBehavior.Concurrency | FbTransactionBehavior.Wait | FbTransactionBehavior.Read
				});
#endif
		}

		/// <summary>
		/// Begins a read-write transaction.
		/// </summary>
		/// <returns>The database transaction object.</returns>
		public System.Data.IDbTransaction BeginReadWriteTransaction()
		{
			this.EnsureConnection ();
			
#if false
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Write;
			
			return this.dbConnection.BeginTransaction (options);
#else
			return this.dbConnection.BeginTransaction (
				new FbTransactionOptions ()
				{
					TransactionBehavior = FbTransactionBehavior.Concurrency | FbTransactionBehavior.Wait | FbTransactionBehavior.Write
				});
#endif
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
			
			path = Globals.Directories.CommonAppData;
			path = System.IO.Path.GetDirectoryName (path);
			path = System.IO.Path.GetDirectoryName (path);
			path = System.IO.Path.Combine (path, "Epsitec");
			path = System.IO.Path.Combine (path, "Firebird Databases");

			FirebirdAbstraction.fbRootDbPath = path;

			System.Diagnostics.Debug.WriteLine ("Database path : " + path);
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
