//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// L'exception DataException est levée lorsqu'un problème survient avec
	/// l'accès aux ressources.
	/// </summary>
	public class DataException : System.Exception
	{
		public DataException()
		{
		}
		
		public DataException(string message) : base (message)
		{
		}
		
		public DataException(string message, System.Exception inner_exception) : base (message, inner_exception)
		{
		}
	}
}
