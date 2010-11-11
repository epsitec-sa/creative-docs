//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> collection, System.Action<T> action)
		{
			foreach (T item in collection)
			{
				action (item);
			}
		}

		public static bool IsEmpty<T>(this IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				return false;
			}

			return true;
		}
		
		public static int IndexOf<T>(this IEnumerable<T> collection, T value, IEqualityComparer<T> comparer = null)
		{
			comparer = comparer ?? EqualityComparer<T>.Default;

			int index = 0;

			foreach (var item in collection)
			{
				if (comparer.Equals (item, value))
				{
					return index;
				}

				index++;
			}

			return -1;
		}


		/// <summary>
		/// Appends one or more elements at the end of an <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to which to append the new elements.</param>
		/// <param name="elements">The elements to append to the sequence.</param>
		/// <returns>A new <see cref="IEnumerable{T}"/> that contains the concatenation of the sequence and the elements.</returns>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, params T[] elements)
		{
			return sequence.Concat (elements);
		}
	}
}
