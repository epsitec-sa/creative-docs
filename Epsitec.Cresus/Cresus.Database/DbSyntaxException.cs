//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// Exception signalant que la base de données a détecté une erreur de syntaxe
	/// (au niveau des arguments de connexion, dans une requête SQL, dans un nom
	/// de table, etc.)
	/// </summary>
	public class DbSyntaxException : DbException
	{
		public DbSyntaxException(DbAccess db_access) : base (db_access)
		{
		}
		
		public DbSyntaxException(DbAccess db_access, string message) : base (db_access, message)
		{
		}
		
		public DbSyntaxException(DbAccess db_access, string message, System.Exception inner_exception) : base (db_access, message, inner_exception)
		{
		}
	}
}
