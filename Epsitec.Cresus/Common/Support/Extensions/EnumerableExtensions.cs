//	Copyright © 2009-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Gets an instance of <see cref="System.Random"/> local to the calling thread. That means
		/// that no other thread will ever call the same instance and thus the obtained dice can be
		/// used safely.
		/// </summary>
		private static System.Random			Dice
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
		/// Gets the index in <paramref name="sequence"/> of the first item which is equal to
		/// <paramref name="element"/> according to <paramref name="comparer"/>.
		/// </summary>
		/// <remarks>
		/// This linq method executes immediately and will read the whole input sequence
		/// until it finds the given element in it, or until its end if it does not contain the
		/// given element. Therefore it has a O(n) complexity where n is the length of the sequence,
		/// assuming that the comparison function provided is efficient.
		/// </remarks>
		/// <typeparam name="T">The type of the elements in the sequence.</typeparam>
		/// <param name="sequence">The sequence where to search.</param>
		/// <param name="element">The element whose index to find.</param>
		/// <param name="comparer">The function used to compare two elements.</param>
		/// <returns>
		/// The index of the first item which is equal to <paramref name="element"/> in the sequence
		/// or <c>-1</c> if there is none.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sequence"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
		public static int IndexOf<T>(this IEnumerable<T> sequence, T element, System.Func<T, T, bool> comparer)
		{
			sequence.ThrowIfNull ("sequence");
			comparer.ThrowIfNull ("comparer");

			IEqualityComparer<T> iEqualityComparer = new LambdaComparer<T> (comparer);

			return EnumerableExtensions.IndexOf (sequence, element, iEqualityComparer);
		}

		/// <summary>
		/// Appends one or more elements at the end of an <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <remarks>This linq method use deferred execution and will stream the given sequence and
		/// won't keep any copy of it internally. It has a complexity of O(1).</remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to which to append the new elements.</param>
		/// <param name="elements">The elements to append to the sequence.</param>
		/// <returns>A new <see cref="IEnumerable{T}"/> that contains the concatenation of the sequence and the elements.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sequence"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="elements"/> is <c>null</c>.</exception>
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
		/// <remarks>This linq method uses deferred execution but will buffer the input sequence and
		/// has a complexity of O(n) where n is the length of the sequence. 
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to shuffle.</param>
		/// <returns>The shuffled sequence.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sequence"/> is <c>null</c>.</exception>
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
		/// <remarks>This LINQ method will buffer both input sequences and will execute immediately.
		/// Its has a complexity of O(n) where n is the length of the longest input sequence.</remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="first">The first <see cref="IEnumerable{T}"/>.</param>
		/// <param name="second">The second <see cref="IEnumerable{T}"/>.</param>
		/// <returns><c>true</c> if both sequence contain the same set of elements, <c>false</c> if they don't.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="first"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="second"/> is <c>null</c>.</exception>
		public static bool SetEquals<T>(this IEnumerable<T> first, IEnumerable<T> second)
		{
			first.ThrowIfNull ("first");
			second.ThrowIfNull ("second");

			HashSet<T> set1 = new HashSet<T> (first);
			HashSet<T> set2 = new HashSet<T> (second);

			return set1.SetEquals (set2);
		}

#if DOTNET35
#else
		/// <summary>
		/// Splits the given <see cref="IEnumerable{T}"/> in two sequences based on a given predicate.
		/// </summary>
		/// <remarks>
		/// The first item in the result contains the elements of the sequence for which the predicate
		/// is not satisfied. The second item in the result contains the elements of the sequence for
		/// which the predicate is satisfied.
		/// 
		/// This LINQ method will execute immediately and consume the whole input sequence when called.
		/// It has a complexity of O(n) where n is the length of the input sequence.
		/// </remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence to split.</param>
		/// <param name="predicate">The predicate used for the split.</param>
		/// <returns>The two resulting sequences.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sequence"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="predicate"/> is <c>null</c>.</exception>	
		public static System.Tuple<IEnumerable<T>, IEnumerable<T>> Split<T>(this IEnumerable<T> sequence, System.Func<T, bool> predicate)
		{
			sequence.ThrowIfNull ("sequence");
			predicate.ThrowIfNull ("predicate");

			var groups = sequence
				.GroupBy (e => predicate (e))
				.ToDictionary (g => g.Key, g => (IEnumerable<T>) g);

			if (!groups.ContainsKey (true))
			{
				groups[true] = new T[0];
			}

			if (!groups.ContainsKey (false))
			{
				groups[false] = new T[0];
			}

			return System.Tuple.Create (groups[false], groups[true]);
		}
#endif


		/// <summary>
		/// Returns a read only collection that contains the elements of the given
		/// <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <remarks>
		/// This LINQ method will execute immediately and will buffer the whole input sequence. It
		/// has a complexity of O(n) where n is the length of the input sequence.
		/// </remarks>
		/// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
		/// <param name="sequence">The sequence whose elements to put in the collection.</param>
		/// <returns>The new readonly collection.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="sequence"/> is <c>null</c>.</exception>
		public static ReadOnlyCollection<T> AsReadOnlyCollection<T>(this IEnumerable<T> sequence)
		{
			sequence.ThrowIfNull ("sequence");
			
			return sequence.ToList ().AsReadOnly ();
		}


		public static HashSet<T> ToSet<T>(this IEnumerable<T> sequence)
		{
			sequence.ThrowIfNull ("sequence");

			return new HashSet<T> (sequence);
		}


		[System.ThreadStatic]
		private static System.Random dice;
	}
}
