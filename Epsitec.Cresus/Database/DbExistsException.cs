namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Exception signalant que la base de données existe déjà.
	/// </summary>
	public class DbExistsException : DbException
	{
		public DbExistsException(DbAccess db_access) : base (db_access)
		{
		}
		
		public DbExistsException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbExistsException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
	}
}
