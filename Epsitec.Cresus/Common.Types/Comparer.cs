//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Comparer permet de comparer deux objets.
	/// </summary>
	public class Comparer
	{
		private Comparer()
		{
		}
		
		public static bool Equal(object a, object b)
		{
			if (a == b)
			{
				return true;
			}
			
			if (a == null)
			{
				return Comparer.Equal (b, a);
			}
			
			System.Diagnostics.Debug.Assert (a != null);
			
			return a.Equals (b);
		}
	}
}
