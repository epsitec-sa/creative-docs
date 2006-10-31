//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;
using FirebirdSql.Data.FirebirdClient;

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

			this.dbConnectionString = FirebirdAbstraction.MakeConnectionString (this.dbAccess, this.MakeDbFilePath (), this.serverType);
			
			if (this.dbAccess.Create)
			{
				//	Si l'appelant a demandé la création de la base, commence par tenter d'ouvrir une
				//	base existante. Si celle-ci existe, c'est considéré comme une erreur, et on génère
				//	une exception.
				
				bool dbAlreadyExists = false;
				
				try
				{
					this.CreateConnection (true);
					
					//	Si on est arrivé ici, c'est que la base existait déjà... Aïe !
					
					dbAlreadyExists = true;
				}
				catch
				{
					this.CreateDatabase ();
					this.CreateConnection (true);
				}
				
				if (dbAlreadyExists)
				{
					throw new Exceptions.ExistsException (this.dbAccess, "Cannot create existing database");
				}
			}
			else
			{
				this.CreateConnection (this.dbAccess.CheckConnection);
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
		
		
		private void CreateConnection()
		{
			this.dbConnection = new FbConnection (this.dbConnectionString);
		}

		private void CreateConnection(bool testConnection)
		{
			try
			{
				this.CreateConnection ();
				
				if (testConnection)
				{
					this.TestConnection ();
				}
			}
			catch
			{
				this.dbConnection.Dispose ();
				this.dbConnection = null;
				
				throw;
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
			System.Diagnostics.Debug.Assert (this.dbAccess.Create);
			
			//	L'appel FbConnection.CreateDatabase ne sait pas créer le dossier, si nécessaire.
			//	Il faut donc que nous le créions nous-même s'il n'existe pas encore.
			
			string path = this.MakeDbFilePath ();
			System.IO.Directory.CreateDirectory (System.IO.Path.GetDirectoryName (path));

			string connection = FirebirdAbstraction.MakeConnectionStringForDbCreation (this.dbAccess, path, this.serverType);
			
			FbConnection.CreateDatabase (connection, FirebirdAbstraction.fbPageSize, true, false);
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
			
			return buffer.ToString ();
		}
		
		public string MakeDbFilePath()
		{
			FirebirdAbstraction.ValidateName (this.dbAccess, this.dbAccess.Database);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			switch (this.engineType)
			{
				case EngineType.Embedded:
					buffer.Append (Common.Support.Globals.Directories.UserAppData);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				case EngineType.Server:
					buffer.Append (FirebirdAbstraction.fbRootDbPath);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				default:
					throw new Database.Exceptions.FactoryException (string.Format ("EngineType {0} not supported.", this.engineType));
			}

			buffer.Append (this.dbAccess.Database);
			buffer.Append (FirebirdAbstraction.fbDbFileExtension);
			
			return buffer.ToString ();
		}
		
		
		private static void ValidateName(DbAccess dbAccess, string name)
		{
			if (name.Length > 100)
			{
				throw new Exceptions.SyntaxException (dbAccess, string.Format ("Name is to long (length={0})", name));
			}
			
			if (Epsitec.Common.Support.RegexFactory.AlphaNumName.IsMatch (name) == false)
			{
				throw new Exceptions.SyntaxException (dbAccess, string.Format ("{0} contains an invalid character", name));
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
		
		
		public string[]								UserTableNames
		{
			get
			{
				List<string> list = new List<string> ();
				
				System.Data.DataTable tables = this.dbConnection.GetSchema ("Tables", new string[] { null, null, null, "TABLE" });

				foreach (System.Data.DataRow row in tables.Rows)
				{
					list.Add (row["TABLE_NAME"] as string);
				}

				return list.ToArray ();
			}
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
		
		public System.Data.IDbTransaction BeginReadOnlyTransaction()
		{
			this.EnsureConnection ();
			
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Read;
			
			return this.dbConnection.BeginTransaction (options);
		}
		
		public System.Data.IDbTransaction BeginReadWriteTransaction()
		{
			this.EnsureConnection ();
			
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Write;
			
			return this.dbConnection.BeginTransaction (options);
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
		private static readonly string			fbCharset			= "UNICODE_FSS";
		private static readonly string			fbRootDbPath		= @"C:\Program Files\Firebird15\Data\Epsitec";
		private static readonly string			fbDbFileExtension	= ".firebird";
	}
}
