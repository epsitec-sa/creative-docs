//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;

using System.Threading;

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

			// Here we use an helper method so that the argument check is immediate but the body
			// execution is deferred.
			// Marc

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
		/// has a complexity of O(n) where n is the length of the sequence. 
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to shuffle.</param>
		/// <returns>The shuffled sequence.</returns>
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> sequence)
		{
			sequence.ThrowIfNull ("sequence");

			// Here we use an helper method so that the argument check is immediate but the body
			// execution is deferred.
			// Marc

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
			List<T> elements = sequence.ToList ();

			for (int i = elements.Count - 1; i > 0; i--)
			{
				int j = EnumerableExtensions.Dice.Next (0, i + 1);

				T element1 = elements[i];
				T element2 = elements[j];

				elements[i] = element2;
				elements[j] = element1;
			}

			return elements;
		}

		/// <summary>
		/// Checks that both <see cref="IEnumerable{T}"/> contain the same set of elements, ignoring
		/// duplicates entries in both sequences.
		/// </summary>
		/// <remarks>This LINQ method will buffer both imput sequences and will execute immediately.
		/// Its has a complexity of O(n) where n is the length of the longest input sequence.</remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="first">The first <see cref="IEnumerable{T}"/>.</param>
		/// <param name="second">The second <see cref="IEnumerable{T}"/>.</param>
		/// <returns><c>true</c> if both sequence contain the same set of elements, <c>false</c> if they don't.</returns>
		public static bool SetEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			first.ThrowIfNull ("first");
			second.ThrowIfNull ("second");

			HashSet<T> set1 = new HashSet<T> (first);
			HashSet<T> set2 = new HashSet<T> (second);

			return set1.SetEquals (set2);
		}

		/// <summary>
		/// Gets an instance of <see cref="System.Random"/> local to the calling thread. That means
		/// that no other thread will ever call the same instance and thus the obtained dice can be
		/// used safely.
		/// </summary>
		private static System.Random Dice
		{
			get
			{
				if (EnumerableExtensions.dice == null)
				{
					EnumerableExtensions.dice = new System.Random ();
				}

				return EnumerableExtensions.dice;
			}
		}

		[System.ThreadStatic]
		private static System.Random dice;
        
	}
}
