namespace Epsitec.Cresus.Support
{
	/// <summary>
	/// L'exception ResourceException est lev�e lorsqu'un probl�me survient avec
	/// l'acc�s aux ressources.
	/// </summary>
	public class ResourceException : System.Exception
	{
		public ResourceException()
		{
		}
		
		public ResourceException(string message) : base (message)
		{
		}
		
		public ResourceException(string message, System.Exception inner_exception) : base (message, inner_exception)
		{
		}
	}
}
