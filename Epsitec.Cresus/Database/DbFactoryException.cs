namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'exception DbFactoryException est utilis�e par les gestionnaires de donn�es
	/// universels pour les erreurs internes qui leur sont propres.
	/// </summary>
	public class DbFactoryException : DbException
	{
		public DbFactoryException() : base (DbAccess.Empty)
		{
		}
		
		public DbFactoryException(string message) : base (DbAccess.Empty, message)
		{
		}
		
		public DbFactoryException(string message, System.Exception inner_exception) : base (DbAccess.Empty, message, inner_exception)
		{
		}
	}
}
