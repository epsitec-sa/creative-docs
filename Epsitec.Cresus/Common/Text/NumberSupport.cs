//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe NumberSupport implémente des méthodes utiles pour la manipulation
	/// de nombres.
	/// </summary>
	public sealed class NumberSupport
	{
		private NumberSupport()
		{
		}
		
		
		public static double Combine(double a, double b)
		{
			//	Combine deux valeurs. Si b est NaN, alors retourne a. Sinon
			//	retourne b.
			
			if (double.IsNaN (b))
			{
				return a;
			}
			else
			{
				return b;
			}
		}
		
		public static bool Equal(double a, double b)
		{
			if (a == b)
			{
				return true;
			}
			
			if (double.IsNaN (a) &&
				double.IsNaN (b))
			{
				return true;
			}
			
			return false;
		}
		
		public static bool Different(double a, double b)
		{
			if (a == b)
			{
				return false;
			}
			
			if (double.IsNaN (a) &&
				double.IsNaN (b))
			{
				return false;
			}
			
			return true;
		}
		
		public static int Compare(double a, double b)
		{
			if (a == b)
			{
				return 0;
			}
			
			if (double.IsNaN (a) &&
				double.IsNaN (b))
			{
				return 0;
			}
			
			if (double.IsNaN (a))
			{
				return -1;
			}
			if (double.IsNaN (b))
			{
				return 1;
			}
			
			return (a < b) ? -1 : 1;
		}
	}
}
