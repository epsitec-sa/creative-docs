//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Implementation
{
	using FirebirdSql.Data.FirebirdClient;
	using Epsitec.Cresus.Database;
	using System.IO;
	
	/// <summary>
	/// Implémentation de IDbAbstraction pour Firebird.
	/// </summary>
	public class FirebirdAbstraction : IDbAbstraction
	{
		public FirebirdAbstraction(DbAccess db_access, IDbAbstractionFactory db_factory, EngineType engine_type)
		{
			this.engine_type = engine_type;
			
			switch (this.engine_type)
			{
				case EngineType.Embedded:
					this.server_type = FbServerType.Embedded;
					break;
				
				case EngineType.Server:
					this.server_type = FbServerType.Default;
					break;
				
				default:
					throw new System.ArgumentException ();
			}
			
			this.db_access   = db_access;
			this.db_factory  = db_factory;
			this.db_connection_string = FirebirdAbstraction.MakeStandardConnectionString (db_access, this.MakeDbFileName (db_access), this.server_type);
			
			if (db_access.Create)
			{
				//	Si l'appelant a demandé la création de la base, commence par tenter d'ouvrir une
				//	base existante. Si celle-ci existe, c'est considéré comme une erreur, et on génère
				//	une exception.
				
				bool base_already_exists = false;
				
				try
				{
					this.CreateConnection (true);
					
					//	Si on est arrivé ici, c'est que la base existait déjà... Aïe !
					
					base_already_exists = true;
				}
				catch
				{
					this.CreateDatabase (db_access);
					this.CreateConnection (true);
				}
				
				if (base_already_exists)
				{
					throw new Exceptions.ExistsException (db_access, "Cannot create existing database");
				}
			}
			else
			{
				this.CreateConnection (db_access.CheckConnection);
			}
			
			this.sql_builder = new FirebirdSqlBuilder (this);
			this.sql_engine  = new FirebirdSqlEngine (this);
			this.db_service_tools = new FirebirdServiceTools (this);
		}

		~FirebirdAbstraction()
		{
			this.Dispose (false);
		}
		

		public DbAccess					DbAccess
		{
			get { return this.db_access; }
		}
		
		internal FbServerType			ServerType
		{
			get
			{
				return this.server_type;
			}
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.sql_builder != null)
				{
					this.sql_builder.Dispose ();
					this.sql_builder = null;
				}
				
				if (this.sql_engine != null)
				{
					this.sql_engine.Dispose ();
					this.sql_engine = null;
				}
				
				if (this.db_connection != null)
				{
					this.db_connection.Dispose ();
					this.db_connection = null;
				}
			}
		}
		
		protected virtual void CreateConnection()
		{
			this.db_connection = new FbConnection (this.db_connection_string);
		}
		
		protected virtual void CreateConnection(bool test_if_ok)
		{
			try
			{
				this.CreateConnection ();
				
				if (test_if_ok)
				{
					this.TestConnection ();
				}
			}
			catch
			{
				this.db_connection.Dispose ();
				this.db_connection = null;
				
				throw;
			}
		}
		
		protected virtual void TestConnection()
		{
			switch (this.db_connection.State)
			{
				case System.Data.ConnectionState.Closed:
					this.db_connection.Open ();
					this.db_connection.Close ();
					break;
				
				case System.Data.ConnectionState.Broken:
					this.db_connection.Close ();
					this.db_connection.Open ();
					break;
			}
		}
		
		protected virtual void EnsureConnection()
		{
			if (this.auto_reopen_connection)
			{
				this.auto_reopen_connection = false;
				this.db_connection.Open ();
			}
			else
			{
				switch (this.db_connection.State)
				{
					case System.Data.ConnectionState.Closed:
						this.db_connection.Open ();
						break;
					
					case System.Data.ConnectionState.Broken:
						this.db_connection.Close ();
						this.db_connection.Open ();
						break;
				}
			}
		}
		
		protected virtual void CreateDatabase(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.Create);
			
			//	L'appel FbConnection.CreateDatabase ne sait pas créer le dossier si nécessaire
			string filename = this.MakeDbFileName (db_access);
			System.IO.Directory.CreateDirectory (Path.GetDirectoryName (filename));
			
			string connection = FirebirdAbstraction.MakeCreationConnectionString (db_access, filename, this.server_type);
			
			FbConnection.CreateDatabase (connection, FirebirdAbstraction.fb_page_size, true, false);
		}
		
		internal static string MakeCreationConnectionString(DbAccess db_access, string filename, FbServerType server_type)
		{
			FirebirdAbstraction.ValidateName (db_access, db_access.LoginName);
			FirebirdAbstraction.ValidateName (db_access, db_access.LoginPassword);
			FirebirdAbstraction.ValidateName (db_access, db_access.Server);
			
			FbConnectionStringBuilder cs = new FbConnectionStringBuilder ();
			
			cs.UserID     = db_access.LoginName;
			cs.Password   = db_access.LoginPassword;
			cs.DataSource = db_access.Server;
			cs.Port       = FirebirdAbstraction.fb_port;
			cs.Database   = filename;
			cs.Dialect    = FirebirdAbstraction.fb_dialect;
			cs.Charset    = FirebirdAbstraction.fb_charset;
			cs.ServerType = server_type;
			
			return cs.ConnectionString;
		}
		
		internal static string MakeStandardConnectionString(DbAccess db_access, string filename, FbServerType server_type)
		{
			FirebirdAbstraction.ValidateName (db_access, db_access.LoginName);
			FirebirdAbstraction.ValidateName (db_access, db_access.LoginPassword);
			FirebirdAbstraction.ValidateName (db_access, db_access.Server);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.AppendFormat ("User={0};", db_access.LoginName);
			buffer.AppendFormat ("Password={0};", db_access.LoginPassword);
			buffer.AppendFormat ("Data Source={0};", db_access.Server);
			buffer.AppendFormat ("Database={0};", filename);
			buffer.AppendFormat ("Port={0};", FirebirdAbstraction.fb_port);
			buffer.AppendFormat ("Dialect={0};", FirebirdAbstraction.fb_dialect);
			buffer.AppendFormat ("Packet Size={0};", FirebirdAbstraction.fb_page_size);
			buffer.AppendFormat ("Server Type={0};", server_type == FbServerType.Embedded ? 1 : 0);
			buffer.AppendFormat ("Charset={0};", FirebirdAbstraction.fb_charset);
			
			buffer.Append ("Role=;");
			buffer.Append ("Pooling=true;");
			buffer.Append ("Connection Lifetime=2;");
			
			return buffer.ToString ();
		}
		
		internal virtual string MakeDbFileName(DbAccess db_access)
		{
			FirebirdAbstraction.ValidateName (db_access, db_access.Database);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			switch (this.engine_type)
			{
				case EngineType.Embedded:
					buffer.Append (Common.Support.Globals.Directories.UserAppData);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				case EngineType.Server:
					buffer.Append (FirebirdAbstraction.fb_root_db_path);
					buffer.Append (System.IO.Path.DirectorySeparatorChar);
					break;
				
				default:
					throw new Database.Exceptions.FactoryException (string.Format ("EngineType {0} not supported.", this.engine_type));
			}
			
			buffer.Append (db_access.Database);
			buffer.Append (FirebirdAbstraction.fb_db_extension);
			
			return buffer.ToString ();
		}
		
		
		protected static void ValidateName(DbAccess db_access, string name)
		{
			if (name.Length > 100)
			{
				throw new Exceptions.SyntaxException (db_access, string.Format ("Name is too long (length={0})", name));
			}
			
			if (Epsitec.Common.Support.RegexFactory.AlphaNumName.IsMatch (name) == false)
			{
				throw new Exceptions.SyntaxException (db_access, string.Format ("{0} contains an invalid character", name));
			}
		}
		
		
		#region IDbAbstraction Members
		public IDbAbstractionFactory				Factory
		{
			get { return this.db_factory; }
		}
		
		public System.Data.IDbConnection			Connection
		{
			get { return this.db_connection; }
		}
		
		public ISqlBuilder							SqlBuilder
		{
			get { return this.sql_builder; }
		}

		public ISqlEngine							SqlEngine
		{
			get { return this.sql_engine; }
		}

		public IDbServiceTools						ServiceTools
		{
			get
			{
				return this.db_service_tools;
			}
		}
		
		public bool									IsConnectionOpen
		{
			get
			{
				if (this.db_connection != null)
				{
					return this.db_connection.State != System.Data.ConnectionState.Closed;
				}
				
				return false;
			}
		}
		
		public bool									IsConnectionAlive
		{
			get
			{
				return this.IsConnectionOpen && (this.db_connection.State != System.Data.ConnectionState.Broken);
			}
		}
		
		
		public string[]								UserTableNames
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				using (System.Data.IDbTransaction transaction = this.BeginReadOnlyTransaction ())
				{
					using (System.Data.IDbCommand command = this.NewDbCommand ())
					{
						//	TODO: essayer d'utiliser code de Carlos
						//	DataTable tables = c.GetSchema("Tables", new string[] { null, null, null, "TABLE" });
						
						command.Transaction = transaction;
						command.CommandText = "SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL;";
						command.CommandType = System.Data.CommandType.Text;
						
						using (System.Data.IDataReader reader = command.ExecuteReader ())
						{
							while (reader.Read ())
							{
								string name = reader[0] as string;
								list.Add (name.TrimEnd ());
							}
						}
					}
					
					transaction.Commit ();
				}
				
				string[] names = new string[list.Count];
				list.CopyTo (names);
				return names;
			}
		}
		
		
		public System.Data.IDbCommand NewDbCommand()
		{
			this.EnsureConnection ();
			return this.db_connection.CreateCommand ();
		}
		
		public System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command)
		{
			this.EnsureConnection ();
			System.Diagnostics.Debug.Assert (command.Connection != null);
			System.Diagnostics.Debug.Assert (command.Connection.State != System.Data.ConnectionState.Closed);
			
			FbCommand fb_command = command as FbCommand;
			
			if (fb_command == null)
			{
				throw new Exceptions.GenericException (this.db_access, "Invalid command object");
			}
			
			return new FbDataAdapter (fb_command);
		}
		
		public System.Data.IDbTransaction BeginReadOnlyTransaction()
		{
			this.EnsureConnection ();
			
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Read;
			
			return this.db_connection.BeginTransaction (options);
		}
		
		public System.Data.IDbTransaction BeginReadWriteTransaction()
		{
			this.EnsureConnection ();
			
			FbTransactionOptions options = FbTransactionOptions.Concurrency |
				/**/					   FbTransactionOptions.Wait |
				/**/					   FbTransactionOptions.Write;
			
			return this.db_connection.BeginTransaction (options);
		}
		
		public void ReleaseConnection()
		{
			//	Ferme la connexion à la base de données, en mettant un fanion pour ré-établir
			//	automatiquement la connexion en cas de besoin.
			
			if (this.db_connection.State != System.Data.ConnectionState.Closed)
			{
				this.db_connection.Close ();
				this.auto_reopen_connection = true;
			}
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		private FbServerType			server_type;
		private EngineType				engine_type;
		
		private DbAccess				db_access;
		private IDbAbstractionFactory	db_factory;
		private FbConnection			db_connection;
		private string					db_connection_string;
		private FirebirdSqlBuilder		sql_builder;
		private FirebirdSqlEngine		sql_engine;
		private FirebirdServiceTools	db_service_tools;
		
		private bool					auto_reopen_connection;
		
		protected static int			fb_port				= 3050;
		protected static byte			fb_dialect			= 3;
		protected static short			fb_page_size		= 8192;
		protected static string			fb_charset			= "UNICODE_FSS";
		protected static string			fb_root_path		= @"C:\Program Files\Firebird15";
		protected static string			fb_root_db_path		= @"C:\Program Files\Firebird15\Data\Epsitec";
		protected static string			fb_db_extension		= ".firebird";
	}
}
