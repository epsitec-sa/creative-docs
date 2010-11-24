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
		/// <remarks>This linq method use deferred execution and will stream the given sequence and
		/// won't keep any copy of it internally.</remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to which to append the new elements.</param>
		/// <param name="elements">The elements to append to the sequence.</param>
		/// <returns>A new <see cref="IEnumerable{T}"/> that contains the concatenation of the sequence and the elements.</returns>
		public static IEnumerable<T> Append<T>(this IEnumerable<T> sequence, params T[] elements)
		{
			sequence.ThrowIfNull ("sequence");
			elements.ThrowIfNull ("elements");

			return EnumerableExtensions.AppendInternal<T> (sequence, elements);
		}

		/// <summary>
		/// Helper method for <see cref="EnumerableExtensions.Append"/> that will perform the real
		/// work.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to which to append the new elements.</param>
		/// <param name="elements">The elements to append to the sequence.</param>
		/// <returns>A new <see cref="IEnumerable{T}"/> that contains the concatenation of the sequence and the elements.</returns>
		private static IEnumerable<T> AppendInternal<T>(IEnumerable<T> sequence, T[] elements)
		{
			return sequence.Concat (elements);
		}

		/// <summary>
		/// Shuffles the given <see cref="IEnumerable{T}"/>, that is, enumerates it in a random
		/// order.
		/// </summary>
		/// <remarks>This linq method use deferred execution but will buffer the input sequence and
		/// has a complexity of O(n log n).
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to shuffle.</param>
		/// <returns>The shuffled sequence.</returns>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sequence)
		{
			sequence.ThrowIfNull ("sequence");

			return EnumerableExtensions.ShuffleInternal<T> (sequence);
		}

		/// <summary>
		/// Helper method for <see cref="EnumerableExtensions.Shuffle"/> that will perform the real
		/// work.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to shuffle.</param>
		/// <returns>The shuffled sequence.</returns>
		private static IEnumerable<T> ShuffleInternal<T>(IEnumerable<T> sequence)
		{
			return sequence.OrderBy (e => EnumerableExtensions.dice.NextDouble ());
		}

		[System.ThreadStatic]
		private static System.Random dice = new System.Random ();

            
	}
}
