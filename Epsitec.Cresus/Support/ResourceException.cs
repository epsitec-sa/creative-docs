namespace Epsitec.Cresus.Support
{
	/// <summary>
	/// L'exception ResourceException est levée lorsqu'un problème survient avec
	/// l'accès aux ressources.
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
