//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 09/10/2003

namespace Epsitec.Common.Support
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
			System.Diagnostics.Debug.WriteLine ("Raising ResourceException: " + message);
		}
		
		public ResourceException(string message, System.Exception inner_exception) : base (message, inner_exception)
		{
			System.Diagnostics.Debug.WriteLine ("Raising ResourceException: " + message + "/" + inner_exception.Message);
		}
	}
}
