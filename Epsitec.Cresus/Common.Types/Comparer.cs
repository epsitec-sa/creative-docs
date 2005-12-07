//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La classe Comparer permet de comparer deux objets.
	/// </summary>
	public sealed class Comparer
	{
		private Comparer()
		{
		}
		
		
		public static bool Equal(double a, double b, double d)
		{
			//	Compare deux nombres avec une certaine marge d'erreur.
			
			if (a == b)
			{
				return true;
			}
			
			if (double.IsNaN (a) && double.IsNaN (b))
			{
				return true;
			}
			
			if (double.IsNegativeInfinity (a) && double.IsNegativeInfinity (b))
			{
				return true;
			}
			
			if (double.IsPositiveInfinity (a) && double.IsPositiveInfinity (b))
			{
				return true;
			}
			
			double diff = a - b;
			
			if (diff < 0)
			{
				return -diff < d;
			}
			else
			{
				return diff < d;
			}
		}
		
		
		public static bool Equal(object a, object b)
		{
			//	Compare deux objets. Pour les tableaux, compare leur contenu; c'est
			//	la seule différence notable par rapport à la méthode Object.Equals.
			
			if (a == b)
			{
				return true;
			}
			
			if (a == null)
			{
				return Comparer.Equal (b, a);
			}
			
			System.Diagnostics.Debug.Assert (a != null);
			
			if (a.GetType ().IsArray)
			{
				System.Array a_array = a as System.Array;
				System.Array b_array = b as System.Array;
				
				return Comparer.Equal (a_array, b_array);
			}
			
			return a.Equals (b);
		}
		
		public static bool Equal(System.Array a, System.Array b)
		{
			//	Compare deux tableaux d'objets : l'égalité est définie par le
			//	contenu des tableaux.
			
			if (a == b)
			{
				return true;
			}
			
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			
			if (a.Rank != b.Rank)
			{
				return false;
			}
			
			for (int r = 0; r < a.Rank; r++)
			{
				if (a.GetLength (r) != b.GetLength (r))
				{
					return false;
				}
			}
			
			switch (a.Rank)
			{
				case 1:
					for (int i = 0; i < a.GetLength (0); i++)
					{
						if (! Comparer.Equal (a.GetValue (i), b.GetValue (i)))
						{
							return false;
						}
					}
					break;
				
				case 2:
					for (int i = 0; i < a.GetLength (0); i++)
					{
						for (int j = 0; j < a.GetLength (1); j++)
						{
							if (! Comparer.Equal (a.GetValue (i, j), b.GetValue (i, j)))
							{
								return false;
							}
						}
					}
					break;
				
				case 3:
					for (int i = 0; i < a.GetLength (0); i++)
					{
						for (int j = 0; j < a.GetLength (1); j++)
						{
							for (int k = 0; k < a.GetLength (2); k++)
							{
								if (! Comparer.Equal (a.GetValue (i, j, k), b.GetValue (i, j, k)))
								{
									return false;
								}
							}
						}
					}
					break;
				
				default:
					throw new System.ArgumentException ("Invalid rank for arrays.");
			}
			
			return true;
		}
		public static bool Equal(int[] a, int[] b)
		{
			//	Version optimisée pour comparer des tableaux à une dimension de
			//	nombres entiers.
			
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null) ||
				(a.Length != b.Length))
			{
				return false;
			}
			
			System.Diagnostics.Debug.Assert (a.Rank == 1);
			System.Diagnostics.Debug.Assert (b.Rank == 1);
			
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			
			return true;
		}
		
		public static bool Equal(string[] a, string[] b)
		{
			//	Version optimisée pour comparer des tableaux à une dimension de
			//	nombres entiers.
			
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null) ||
				(a.Length != b.Length))
			{
				return false;
			}
			
			System.Diagnostics.Debug.Assert (a.Rank == 1);
			System.Diagnostics.Debug.Assert (b.Rank == 1);
			
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			
			return true;
		}
	}
}
