using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de IDbAbstraction pour Firebird.
	/// </summary>
	public class FirebirdAbstraction : IDbAbstraction, System.IDisposable
	{
		public FirebirdAbstraction(DbAccess db_access, IDbAbstractionFactory db_factory)
		{
			this.db_factory = db_factory;
			this.db_connection = null;
			this.db_connection_string = this.CreateConnectionString (db_access);
			
			if (db_access.create)
			{
				try
				{
					this.CreateConnection ();
					this.TestConnection ();
				}
				catch
				{
					this.CreateDatabase (db_access);
					this.CreateConnection ();
					this.TestConnection ();
				}
			}
			
			//	TODO: initialisation
			//
			//	1. La connexion est ouverte pour vérifier que la base existe. Il faut voir
			//	   si la connexion doit être conservée dans l'état ouvert.
			//
			//	2. Si la base n'existe pas mais que db_access.create est actif, on crée
			//	   la base de données, puis on ouvre la connexion.
			//
			//	- La propriété 'Connection' doit-elle s'assurer que la connexion est ouverte ?
			//
			//	- Comment sont gérées les transactions ? Je crois que IDbCommand.Transaction
			//	  et IDbConnection.BeginTransaction offrent ce qu'il faut, donc ce n'est pas
			//	  la peine de s'en occuper ici.
			
//-			throw new DbFactoryException ();
		}

		~FirebirdAbstraction()
		{
			this.Dispose (false);
		}
		
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: libère les ressources "managed" => appeler 'Dispose'
			}
			
			//	TODO: libère les ressources non "managed"
		}
		
		protected virtual void CreateConnection()
		{
			this.db_connection = new FbConnection (this.db_connection_string);
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
		
		protected virtual void CreateDatabase(DbAccess db_access)
		{
			System.Diagnostics.Debug.Assert (db_access.create);
			
			FirebirdAbstraction.ValidateName (db_access.login_name);
			FirebirdAbstraction.ValidateName (db_access.login_pwd);
			FirebirdAbstraction.ValidateName (db_access.server);
			
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
			FirebirdAbstraction.ValidateName (db_access.login_name);
			FirebirdAbstraction.ValidateName (db_access.login_pwd);
			FirebirdAbstraction.ValidateName (db_access.server);
			
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
			FirebirdAbstraction.ValidateName (db_access.database);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append (FirebirdAbstraction.fb_root_db_path);
			buffer.Append (System.IO.Path.DirectorySeparatorChar);
			buffer.Append (db_access.database);
			buffer.Append (FirebirdAbstraction.fb_db_extension);
			
			return buffer.ToString ();
		}
		
		
		protected static void ValidateName(string name)
		{
			if (name.Length > 100)
			{
				throw new System.ArgumentException ("Name is too long");
			}
			if (System.Text.RegularExpressions.Regex.IsMatch (name, @"\w") == false)
			{
				throw new System.FormatException (string.Format ("{0} contains an invalid character", name));
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
		
		
		public System.Data.IDbCommand NewDbCommand()
		{
			return this.db_connection.CreateCommand ();
		}
		
		public System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command)
		{
			//	TODO: implémenter new DataAdapter(command)
			return null;
		}

		public void ExtractSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			//	TODO: extraire les paramètres de retour de command et
			//	copier leurs valeurs dans les champs correspondants.
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
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
