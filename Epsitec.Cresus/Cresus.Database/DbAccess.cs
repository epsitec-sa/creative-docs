//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbAccess</c> structure defines a database access, which is used
	/// to create a connection string.
	/// </summary>
	[System.Serializable]
	public struct DbAccess
	{
		public DbAccess (string provider, string database, string server, string loginName, string loginPassword, bool createDatabase)
		{
			this.provider		 = provider;
			this.database		 = database;
			this.server			 = server;
			this.loginName		 = loginName;
			this.loginPassword	 = loginPassword;
			this.createDatabase	 = createDatabase;
			this.checkConnection = true;
		}
		
		public bool								IsValid
		{
			get
			{
				return (this.Provider != null) &&
					   (this.Database != null) &&
					   (this.Server != null) &&
					   (this.LoginName != null);
			}
		}

		public string							Provider
		{
			get
			{
				return this.provider;
			}
		}

		public string							Database
		{
			get
			{
				return this.database;
			}
		}

		public string							Server
		{
			get
			{
				return this.server;
			}
		}

		public string							LoginName
		{
			get
			{
				return this.loginName;
			}
		}

		public string							LoginPassword
		{
			get
			{
				return this.loginPassword;
			}
		}

		public bool								CreateDatabase
		{
			get
			{
				return this.createDatabase;
			}
			set
			{
				this.createDatabase = value;
			}
		}

		public bool								CheckConnection
		{
			get
			{
				return this.checkConnection;
			}
			set
			{
				this.checkConnection = value;
			}
		}

		public static readonly DbAccess			Empty;
		
		private string							provider;
		private string							database;
		private string							server;
		private string							loginName;
		private string							loginPassword;
		private bool							createDatabase;
		private bool							checkConnection;
	}
}
