namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'exception DbFactoryException est utilis�e par les gestionnaires de donn�es
	/// universels pour les erreurs internes qui leur sont propres.
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
	}
}
