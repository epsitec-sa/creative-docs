//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Définition d'un accès à une base de données.
	/// </summary>
	
	[System.Serializable]
	
	public struct DbAccess
	{
		public DbAccess (string provider, string database, string server, string login_name, string login_pwd, bool create)
		{
			this.Provider		 = provider;
			this.Database		 = database;
			this.Server			 = server;
			this.LoginName		 = login_name;
			this.LoginPassword	 = login_pwd;
			this.Create			 = create;
			this.CheckConnection = true;
		}
		
		
		public bool							IsValid
		{
			get
			{
				return (this.Provider != null) &&
					   (this.Database != null) &&
					   (this.Server != null) &&
					   (this.LoginName != null);
			}
		}
		
		
		public string						Provider;
		public string						Database;
		public string						Server;
		public string						LoginName;
		public string						LoginPassword;
		public bool							Create;
		public bool							CheckConnection;
		
		public static readonly DbAccess		Empty;
	}
}
