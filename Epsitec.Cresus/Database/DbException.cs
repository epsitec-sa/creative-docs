//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Classe de base pour les exceptions de l'interface avec la base de données.
	/// </summary>
	public class DbException : System.Exception
	{
		public DbException(DbAccess db_access)
		{
			this.db_access = db_access;
		}
		
		public DbException(DbAccess db_access, string message) : base (message)
		{
			this.db_access = db_access;
		}
		
		public DbException(DbAccess db_access, string message, System.Exception inner_exception) : base (message, inner_exception)
		{
			this.db_access = db_access;
		}
		
		
		public DbAccess					DbAccess
		{
			get { return this.db_access; }
		}
		
		
		protected DbAccess				db_access;
	}
}
