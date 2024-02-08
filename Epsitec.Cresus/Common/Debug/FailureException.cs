//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Debug
{
	/// <summary>
	/// La classe FailureException permet de signaler un échec général.
	/// </summary>
	public class FailureException : System.ApplicationException
	{
		public FailureException()
		{
		}
		
		public FailureException(string message) : base (message)
		{
		}
	}
}
