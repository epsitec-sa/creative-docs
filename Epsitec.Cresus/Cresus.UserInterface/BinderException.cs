//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.UserInterface
{
	/// <summary>
	/// La classe BinderException représente une exception liée au DataBinder
	/// ou IBinder.
	/// </summary>
	
	[System.Serializable]
	
	public class BinderException : System.ApplicationException
	{
		public BinderException()
		{
		}
		
		public BinderException(string message) : base (message)
		{
		}
		
		public BinderException(string message, System.Exception inner_exception) : base (message, inner_exception)
		{
		}
	}
}
