//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/11/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// D�finition d'un acc�s � une base de donn�es.
	/// </summary>
	
	[System.Serializable]
	
	public struct DbAccess
	{
		public DbAccess (string provider, string database, string server, string login_name, string login_pwd, bool create)
		{
			this.Provider		= provider;
			this.Database		= database;
			this.Server			= server;
			this.LoginName		= login_name;
			this.LoginPassword	= login_pwd;
			this.Create			= create;
		}
		
		
		public bool							IsValid
		{
			get { return (this.Provider != null) && (this.Database != null) && (this.Server != null) && (this.LoginName != null); }
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
