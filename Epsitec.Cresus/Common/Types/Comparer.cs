//	Copyright © 2003-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>Comparer</c> class provides comparison support for two objects.
	/// </summary>
	public static class Comparer
	{
		public static bool Equal(double a, double b, double δ)
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
				return -diff < δ;
			}
			else
			{
				return diff < δ;
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
				System.Array aArray = a as System.Array;
				System.Array bArray = b as System.Array;
				
				return Comparer.Equal (aArray, bArray);
			}

			System.Collections.IEnumerable enumerableA = a as System.Collections.IEnumerable;
			System.Collections.IEnumerable enumerableB = b as System.Collections.IEnumerable;

			if ((enumerableA != null) &&
				(enumerableB != null))
			{
				return Collection.CompareEqual (enumerableA, enumerableB);
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

			bool result;

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}
			
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
			//	textes.

			bool result;

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}
			
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			
			return true;
		}

		public static bool EqualValues<T>(T[] a, T[] b)
			where T : struct, System.IEquatable<T>
		{
			bool result;

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i].Equals (b[i]) == false)
				{
					return false;
				}
			}

			return true;
		}

		public static bool EqualObjects<T>(T[] a, T[] b)
			where T : class
		{
			bool result;

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					if ((a[i] == null) ||
						(a[i].Equals (b[i]) == false))
					{
						return false;
					}
				}
			}

			return true;
		}

		public static bool EqualObjects<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB)
			where T : class
		{
			bool result;

			if (collectionA == collectionB)
			{
				return true;
			}

			ICollection<T> countableA = collectionA as ICollection<T>;
			ICollection<T> countableB = collectionB as ICollection<T>;

			if ((countableA != null) &&
				(countableB != null) &&
				(countableA.Count != countableB.Count))
			{
				return false;
			}

			T[] a = collectionA == null ? new T[0] : collectionA.ToArray ();
			T[] b = collectionB == null ? new T[0] : collectionB.ToArray ();

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[i])
				{
					if ((a[i] == null) ||
						(a[i].Equals (b[i]) == false))
					{
						return false;
					}
				}
			}

			return true;
		}

		public static bool EqualValues<T>(IEnumerable<T> collectionA, IEnumerable<T> collectionB)
			where T : struct
		{
			bool result;

			if (collectionA == collectionB)
			{
				return true;
			}

			ICollection<T> countableA = collectionA as ICollection<T>;
			ICollection<T> countableB = collectionB as ICollection<T>;

			if ((countableA != null) &&
				(countableB != null) &&
				(countableA.Count != countableB.Count))
			{
				return false;
			}

			T[] a = collectionA == null ? new T[0] : collectionA.ToArray ();
			T[] b = collectionB == null ? new T[0] : collectionB.ToArray ();

			if (Comparer.PreCompare (a, b, out result))
			{
				return result;
			}

			for (int i = 0; i < a.Length; i++)
			{
				if (!a[i].Equals (b[i]))
				{
					return false;
				}
			}

			return true;
		}

		private static bool PreCompare<T>(T[] a, T[] b, out bool result)
		{
			if (a == b)
			{
				result = true;
				return true;
			}
			if (a == null)
			{
				result = false;
				return true;
			}
			if (b == null)
			{
				result = false;
				return true;
			}
			if (a.Length != b.Length)
			{
				result = false;
				return true;
			}
			result = false;
			return false;
		}

		public static int Compare(string a, string b, TextComparison comparison)
		{
			if (comparison == TextComparison.Default)
			{
				return string.CompareOrdinal (a, b);
			}
			
			//	Comparing both strings will require that we convert them; rather
			//	than converting the full strings, we start with just a few characters
			//	in order to return quickly if both strings differ at the beginning,
			//	which is highly probable.

			const int probeOffset = 4;
			
			string textA = Comparer.ConvertForComparison (a.Substring (0, System.Math.Min (probeOffset, a.Length)), comparison);
			string textB = Comparer.ConvertForComparison (b.Substring (0, System.Math.Min (probeOffset, b.Length)), comparison);

			int result = string.CompareOrdinal (textA, textB);

			if (result != 0)
			{
				return result;
			}

			textA = Comparer.ConvertForComparison (a.Substring (probeOffset, System.Math.Max (0, a.Length-probeOffset)), comparison);
			textB = Comparer.ConvertForComparison (b.Substring (probeOffset, System.Math.Max (0, b.Length-probeOffset)), comparison);

			return string.CompareOrdinal (textA, textB);
		}

		public static bool Equal(string a, string b, TextComparison comparison)
		{
			return Comparer.Compare (a, b, comparison) == 0;
		}


		private static string ConvertForComparison(string text, TextComparison comparison)
		{
			switch (comparison)
			{
				case TextComparison.Default:
					return text;

				case TextComparison.IgnoreAccents:
					return Converters.TextConverter.StripAccents (text);

				case TextComparison.IgnoreCase:
					return Converters.TextConverter.ConvertToLower (text);

				case TextComparison.IgnoreAccents | TextComparison.IgnoreCase:
					return Converters.TextConverter.ConvertToLowerAndStripAccents (text);

				default:
					throw new System.NotImplementedException (string.Format ("Support for TextComparison.{0} not implemented", comparison));
			}
		}
	}
}
