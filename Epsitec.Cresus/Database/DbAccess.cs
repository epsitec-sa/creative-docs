namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Définition d'un accès à une base de données.
	/// </summary>
	public struct DbAccess
	{
		DbAccess (string provider, string database, string server, string login_name, string login_pwd, bool create)
		{
			this.provider   = provider;
			this.database   = database;
			this.server     = server;
			this.login_name = login_name;
			this.login_pwd  = login_pwd;
			this.create     = create;
		}
		
		public string		provider;
		public string		database;
		public string		server;
		public string		login_name;
		public string		login_pwd;
		public bool			create;
	}
}
