using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Impl�mentation de IDbAbstraction pour Firebird.
	/// </summary>
	public class FirebirdAbstraction : IDbAbstraction, System.IDisposable
	{
		public FirebirdAbstraction(DbAccess db_access, IDbAbstractionFactory db_factory)
		{
			this.db_access  = db_access;
			this.db_factory = db_factory;
			this.db_connection_string = this.CreateConnectionString (db_access);
			
			if (db_access.create)
			{
				//	Si l'appelant a demand� la cr�ation de la base, commence par tenter d'ouvrir une
				//	base existante. Si celle-ci existe, c'est consid�r� comme une erreur, et on g�n�re
				//	une exception.
				
				bool base_already_exists = false;
				
				try
				{
					this.CreateConnection (true);
					
					//	Si on est arriv� ici, c'est que la base existait d�j�... A�e !
					
					base_already_exists = true;
				}
				catch
				{
					this.CreateDatabase (db_access);
					this.CreateConnection (true);
				}
				
				if (base_already_exists)
				{
					throw new DbExistsException (db_access, "Cannot create existing database");
				}
			}
			else
			{
				this.CreateConnection (true);
			}
		}

		~FirebirdAbstraction()
		{
			this.Dispose (false);
		}
		
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: lib�re les ressources "managed" => appeler 'Dispose'
			}
			
			//	TODO: lib�re les ressources non "managed"
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
		
		protected virtual void CreateDatabase(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.create);
			
			FirebirdAbstraction.ValidateName (db_access, db_access.login_name);
			FirebirdAbstraction.ValidateName (db_access, db_access.login_pwd);
			FirebirdAbstraction.ValidateName (db_access, db_access.server);
			
			FbConnection.CreateDatabase (db_access.server,
				/**/					 FirebirdAbstraction.fb_port,
				/**/					 this.CreateDbFileName (db_access),
				/**/					 db_access.login_name,
				/**/					 db_access.login_pwd,
				/**/					 FirebirdAbstraction.fb_dialect,
				/**/					 true, // <- true means synchronous writes on server
				/**/					 FirebirdAbstraction.fb_page_size,
				/**/					 FirebirdAbstraction.fb_charset);
		}
		
		
		protected virtual string CreateConnectionString(DbAccess db_access)
		{
			FirebirdAbstraction.ValidateName (db_access, db_access.login_name);
			FirebirdAbstraction.ValidateName (db_access, db_access.login_pwd);
			FirebirdAbstraction.ValidateName (db_access, db_access.server);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.AppendFormat ("User={0};", db_access.login_name);
			buffer.AppendFormat ("Password={0};", db_access.login_pwd);
			buffer.AppendFormat ("DataSource={0};", db_access.server);
			buffer.AppendFormat ("Database={0};", this.CreateDbFileName (db_access));
			buffer.AppendFormat ("Port={0};", FirebirdAbstraction.fb_port);
			buffer.AppendFormat ("Dialect={0};", FirebirdAbstraction.fb_dialect);
			buffer.AppendFormat ("Packet Size={0};", FirebirdAbstraction.fb_page_size);
			buffer.AppendFormat ("Charset={0};", FirebirdAbstraction.fb_charset);
			
			buffer.Append ("Role=;");
			buffer.Append ("Pooling=true;");
			buffer.Append ("Connection Lifetime=60;");
			
			return buffer.ToString ();
		}
		
		protected virtual string CreateDbFileName(DbAccess db_access)
		{
			FirebirdAbstraction.ValidateName (db_access, db_access.database);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (FirebirdAbstraction.fb_root_db_path);
			buffer.Append (System.IO.Path.DirectorySeparatorChar);
			buffer.Append (db_access.database);
			buffer.Append (FirebirdAbstraction.fb_db_extension);
			
			return buffer.ToString ();
		}
		
		
		protected static void ValidateName(DbAccess db_access, string name)
		{
			if (name.Length > 100)
			{
				throw new DbSyntaxException (db_access, string.Format ("Name is too long (length={0})", name));
			}
			if (System.Text.RegularExpressions.Regex.IsMatch (name, @"\w") == false)
			{
				throw new DbSyntaxException (db_access, string.Format ("{0} contains an invalid character", name));
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
			get { return null; }
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
				
				using (System.Data.IDbTransaction transaction = this.BeginTransaction ())
				{
					using (System.Data.IDbCommand command = this.NewDbCommand ())
					{
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
			return this.db_connection.CreateCommand ();
		}
		
		public System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command)
		{
			System.Diagnostics.Debug.Assert (command.Connection != null);
			System.Diagnostics.Debug.Assert (command.Connection.State != System.Data.ConnectionState.Closed);
			
			FbCommand fb_command = command as FbCommand;
			
			if (fb_command == null)
			{
				throw new DbException (this.db_access, "Invalid command object");
			}
			
			return new FbDataAdapter (fb_command);
		}
		
		public System.Data.IDbTransaction BeginTransaction()
		{
			this.EnsureConnection ();
			return this.db_connection.BeginTransaction ();
		}
		
		#endregion

#if false
		public void GetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			FbCommand fb_command = command as FbCommand;
			
			for (int i = 0; i < fb_command.Parameters.Count; i++)
			{
				FbParameter param = fb_command.Parameters[i] as FbParameter;
				
				System.Diagnostics.Debug.Assert (param != null);
				System.Diagnostics.Debug.WriteLine (param.ParameterName + ": " + param.FbDbType.ToString ());
				
				switch (param.Direction)
				{
					case System.Data.ParameterDirection.Input:
						break;
					
					case System.Data.ParameterDirection.InputOutput:
					case System.Data.ParameterDirection.Output:
						break;
					
					case System.Data.ParameterDirection.ReturnValue:
						break;
				}
			}
		}
#endif
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		private DbAccess							db_access;
		private IDbAbstractionFactory				db_factory;
		private FbConnection						db_connection;
		private string								db_connection_string;
		
		protected static int						fb_port				= 3050;
		protected static byte						fb_dialect			= 3;
		protected static short						fb_page_size		= 8192;
		protected static string						fb_charset			= "UNICODE_FSS";
		protected static string						fb_root_path		= @"C:\Program Files\Firebird15";
		protected static string						fb_root_db_path		= @"C:\Program Files\Firebird15\Data\Epsitec";
		protected static string						fb_db_extension		= ".firebird";
	}
}
