namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'exception DbFormatException représente une erreur de format (type invalide,
	/// etc.) qui n'est pas encore forcément lié à une base.
	/// </summary>
	public class DbFormatException : DbException
	{
		public DbFormatException() : base (DbAccess.Empty)
		{
		}
		
		public DbFormatException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public DbFormatException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
		
		public DbFormatException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbFormatException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
	}
}
