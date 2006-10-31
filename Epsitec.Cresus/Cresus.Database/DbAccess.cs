//	Copyright � 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		/// <summary>
		/// Initializes a new instance of the <see cref="DbAccess"/> class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <param name="database">The database.</param>
		/// <param name="server">The server.</param>
		/// <param name="loginName">The login name.</param>
		/// <param name="loginPassword">The login password.</param>
		/// <param name="createDatabase">If set to <c>true</c>, requests database creation.</param>
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

		/// <summary>
		/// Gets a value indicating whether this database access is valid.
		/// </summary>
		/// <value><c>true</c> if this database access is valid; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets the provider name.
		/// </summary>
		/// <value>The provider name.</value>
		public string							Provider
		{
			get
			{
				return this.provider;
			}
		}

		/// <summary>
		/// Gets the database name.
		/// </summary>
		/// <value>The database name.</value>
		public string							Database
		{
			get
			{
				return this.database;
			}
		}

		/// <summary>
		/// Gets the server name.
		/// </summary>
		/// <value>The server name.</value>
		public string							Server
		{
			get
			{
				return this.server;
			}
		}

		/// <summary>
		/// Gets the login name.
		/// </summary>
		/// <value>The login name.</value>
		public string							LoginName
		{
			get
			{
				return this.loginName;
			}
		}

		/// <summary>
		/// Gets the login password.
		/// </summary>
		/// <value>The login password.</value>
		public string							LoginPassword
		{
			get
			{
				return this.loginPassword;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the database should be
		/// created.
		/// </summary>
		/// <value><c>true</c> if the database should be created; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Gets or sets a value indicating whether the connection should be checked.
		/// </summary>
		/// <value><c>true</c> if the connection should be checked; otherwise, <c>false</c>.</value>
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
