namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// D�finition d'un acc�s � une base de donn�es.
	/// </summary>
	public struct DbAccess
	{
		DbAccess (string provider, string database, string server, string login_name, string login_pwd, bool create)
		{
			this.Provider		= provider;
			this.Database		= database;
			this.Server			= server;
			this.LoginName		= login_name;
			this.LoginPassword	= login_pwd;
			this.Create			= create;
		}
		
		public string						Provider;
		public string						Database;
		public string						Server;
		public string						LoginName;
		public string						LoginPassword;
		public bool							Create;
		
		public static readonly DbAccess		Empty;
	}
}
