namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// D�finition d'un acc�s � une base de donn�es.
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
