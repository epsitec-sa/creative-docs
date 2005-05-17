//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				
				if ((b_array == null) ||
					(a_array.Rank != b_array.Rank))
				{
					return false;
				}
				
				for (int r = 0; r < a_array.Rank; r++)
				{
					if (a_array.GetLength (r) != b_array.GetLength (r))
					{
						return false;
					}
				}
				
				switch (a_array.Rank)
				{
					case 1:
						for (int i = 0; i < a_array.GetLength (0); i++)
						{
							if (! Comparer.Equal (a_array.GetValue (i), b_array.GetValue (i)))
							{
								return false;
							}
						}
						break;
					
					case 2:
						for (int i = 0; i < a_array.GetLength (0); i++)
						{
							for (int j = 0; j < a_array.GetLength (1); j++)
							{
								if (! Comparer.Equal (a_array.GetValue (i, j), b_array.GetValue (i, j)))
								{
									return false;
								}
							}
						}
						break;
					
					case 3:
						for (int i = 0; i < a_array.GetLength (0); i++)
						{
							for (int j = 0; j < a_array.GetLength (1); j++)
							{
								for (int k = 0; k < a_array.GetLength (2); k++)
								{
									if (! Comparer.Equal (a_array.GetValue (i, j, k), b_array.GetValue (i, j, k)))
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
			
			return a.Equals (b);
		}
	}
}
