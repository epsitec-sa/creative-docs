//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The Collection class provides a few useful methods to operate on
	/// enumerable collections.
	/// </summary>
	public class Collection
	{
		public static int Count<T>(IEnumerable<T> collection)
		{
			int n = 0;

			foreach (T item in collection)
			{
				n++;
			}

			return n;
		}

		public static bool IsEmpty(System.Collections.IEnumerable collection)
		{
			if (collection == null)
			{
				return true;
			}

			System.Collections.IEnumerator enumerator = collection.GetEnumerator ();

			if (enumerator.MoveNext ())
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static T Extract<T>(IEnumerable<T> collection, int index)
		{
			int n = 0;

			foreach (T item in collection)
			{
				if (index == n)
				{
					return item;
				}
				
				n++;
			}

			return default (T);
		}

		public static List<T> ToList<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();
			
			if (collection != null)
			{
				list.AddRange (collection);
			}
			
			return list;
		}

		public static bool TryGetFirst(System.Collections.IEnumerable collection, out object firstItem)
		{
			foreach (object item in collection)
			{
				firstItem = item;
				return true;
			}

			firstItem = null;
			return false;
		}

		public static T GetFirst<T>(IEnumerable<T> collection, T defaultValue)
		{
			T firstItem;

			if (Collection.TryGetFirst (collection, out firstItem))
			{
				return firstItem;
			}
			else
			{
				return defaultValue;
			}
		}

		public static T GetFirst<T>(IEnumerable<T> collection)
		{
			T firstItem;
			
			if (Collection.TryGetFirst (collection, out firstItem))
			{
				return firstItem;
			}

			throw new System.ArgumentOutOfRangeException ();
		}

		public static bool TryGetFirst<T>(IEnumerable<T> collection, out T firstItem)
		{
			foreach (T item in collection)
			{
				firstItem = item;
				return true;
			}

			firstItem = default (T);
			return false;
		}
		
		public static T GetLast<T>(IList<T> list)
		{
			if (list == null)
			{
				throw new System.ArgumentOutOfRangeException ();
			}

			int n = list.Count;

			if (n < 1)
			{
				throw new System.ArgumentOutOfRangeException ();
			}

			return list[n-1];
		}
		
		public static T GetLast<T>(IList<T> list, T defaultValue)
		{
			if (list == null)
			{
				return defaultValue;
			}

			int n = list.Count;

			if (n < 1)
			{
				return defaultValue;
			}

			return list[n-1];
		}

		public static T[] StripLast<T>(T[] array)
		{
			if (array.Length < 2)
			{
				return new T[0];
			}
			else
			{
				T[] copy = new T[array.Length-1];
				System.Array.Copy (array, 0, copy, 0, array.Length-1);
				return copy;
			}
		}

		public static void RemoveDuplicates<T>(IList<T> list) where T : System.IEquatable<T>
		{
			for (int index = 0; index < list.Count-1; index++)
			{
				T a = list[index];

				for (int j = 1; j+index < list.Count; )
				{
					T b = list[index+j];

					if (a.Equals (b))
					{
						list.RemoveAt (index+j);
					}
					else
					{
						j++;
					}
				}
			}
		}

		public static void RemoveDuplicatesInSortedList<T>(IList<T> list) where T : System.IEquatable<T>
		{
			int index = 0;

			while (index < list.Count-1)
			{
				T a = list[index];
				T b = list[index+1];

				if (a.Equals (b))
				{
					list.RemoveAt (index+1);
				}
				else
				{
					index++;
				}
			}
		}

		public static T[] ToArray<T>(IList<T> list)
		{
			T[] array = new T[list.Count];
			list.CopyTo (array, 0);
			return array;
		}

		public static T[] ToArray<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();

			if (collection != null)
			{
				list.AddRange (collection);
			}

			return list.ToArray ();
		}

		public static T[] ToArray<T>(System.Collections.IEnumerable collection)
		{
			List<T> list = new List<T> ();

			if (collection != null)
			{
				foreach (T item in collection)
				{
					list.Add (item);
				}
			}

			return list.ToArray ();
		}

		public static object[] ToObjectArray<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();

			if (collection != null)
			{
				list.AddRange (collection);
			}

			object[] items = new object[list.Count];

			for (int i = 0; i < list.Count; i++)
			{
				items[i] = list[i];
			}

			return items;
		}

		public static T[] ToSortedArray<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> ();

			if (collection != null)
			{
				list.AddRange (collection);
			}

			T[] array = list.ToArray ();
			System.Array.Sort (array);
			return array;
		}

		public static IEnumerable<T> Single<T>(T item)
		{
			yield return item;
		}

		/// <summary>
		/// Determines whether the collection contains an item based on a predicate.
		/// </summary>
		/// <typeparam name="T">The item type.</typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the searched item; otherwise, <c>false</c>.
		/// </returns>
		public static bool Contains<T>(IEnumerable<T> collection, System.Predicate<T> predicate)
		{
			return (Collection.FindIndex (collection, predicate) < 0) ? false : true;
		}

		public static bool ContainsAll<T>(IEnumerable<T> collection, ICollection<T> values)
		{
			List<T> ignore = new List<T> ();
			
			foreach (T item in collection)
			{
				if (ignore.Contains (item))
				{
					continue;
				}
				if (values.Contains (item))
				{
					ignore.Add (item);
				}
			}

			return ignore.Count == values.Count;
		}

		public static bool TryFind<T>(IEnumerable<T> collection, System.Predicate<T> predicate, out T result)
		{
			foreach (T item in collection)
			{
				if (predicate (item))
				{
					result = item;
					return true;
				}
			}

			result = default (T);
			return false;
		}

		public static int FindIndex<T>(IEnumerable<T> collection, System.Predicate<T> predicate)
		{
			int index = 0;

			foreach (T item in collection)
			{
				if (predicate (item))
				{
					return index;
				}

				index++;
			}

			return -1;
		}

		public static IEnumerable<T> Filter<T>(IEnumerable<T> collection, System.Predicate<T> predicate)
		{
			foreach (T item in collection)
			{
				if (predicate.Invoke (item))
				{
					yield return item;
				}
			}
		}

		public static IEnumerable<object> EnumerateObjects(System.Collections.IEnumerable collection)
		{
			foreach (object item in collection)
			{
				yield return item;
			}
		}

		public static IEnumerable<T> Reverse<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T> (collection);
			list.Reverse ();
			return list;
		}

		public static IEnumerable<T> Enumerate<T>(IEnumerable<T> collection, System.Comparison<T> comparer)
		{
			if (comparer == null)
			{
				return collection;
			}
			else
			{
				List<T> list = new List<T> (collection);
				list.Sort (comparer);
				return list;
			}
		}

		/// <summary>
		/// Compares two collections for equality. All items in the collection
		/// must compare equal (using their <c>IEquatable.Equals</c> method).
		/// </summary>
		/// <typeparam name="T">Type of the elements.</typeparam>
		/// <param name="a">The first collection.</param>
		/// <param name="b">The second collection.</param>
		/// <returns>
		/// 	<c>true</c> if both collections have an equal content; otherwise, <c>false</c>.
		/// </returns>
		public static bool CompareEqual<T>(ICollection<T> a, ICollection<T> b) where T : System.IEquatable<T>
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}
			
			int n = a.Count;
			
			if (n != b.Count)
			{
				return false;
			}

			IEnumerable<T> enumerableA = a as IEnumerable<T>;
			IEnumerable<T> enumerableB = b as IEnumerable<T>;

			return Collection.CompareEqualNotChecked (enumerableA, enumerableB);
		}

		/// <summary>
		/// Compares two collections for equality. All items in the collection
		/// must compare equal (using their <c>IEquatable.Equals</c> method).
		/// </summary>
		/// <typeparam name="T">Type of the elements.</typeparam>
		/// <param name="a">The first collection.</param>
		/// <param name="b">The second collection.</param>
		/// <returns>
		/// 	<c>true</c> if both collections have an equal content; otherwise, <c>false</c>.
		/// </returns>
		public static bool CompareEqual<T>(IEnumerable<T> a, IEnumerable<T> b) where T : System.IEquatable<T>
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			return Collection.CompareEqualNotChecked (a, b);
		}

		public static bool CompareEqual(System.Collections.IEnumerable a, System.Collections.IEnumerable b)
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			return Collection.CompareEqualNotChecked (a, b);
		}

		/// <summary>
		/// Compares two collections for equality. All items in the collection
		/// must compare equal (using a provided comparison method).
		/// </summary>
		/// <typeparam name="T">Type of the elements.</typeparam>
		/// <param name="a">The first collection.</param>
		/// <param name="b">The second collection.</param>
		/// <param name="predicate">The comparison method.</param>
		/// <returns>
		/// 	<c>true</c> if both collections have an equal content; otherwise, <c>false</c>.
		/// </returns>
		public static bool CompareEqual<T>(IEnumerable<T> a, IEnumerable<T> b, Predicate<T> predicate)
		{
			if (a == b)
			{
				return true;
			}
			if ((a == null) ||
				(b == null))
			{
				return false;
			}

			return Collection.CompareEqualNotChecked (a, b, predicate);
		}

		
		private static bool CompareEqualNotChecked<T>(IEnumerable<T> a, IEnumerable<T> b) where T : System.IEquatable<T>
		{
			return Collection.CompareEqualNotChecked (a, b,
				delegate (T value1, T value2)
				{
					return value1.Equals (value2);
				});
		}

		private static bool CompareEqualNotChecked<T>(IEnumerable<T> a, IEnumerable<T> b, Predicate<T> predicate)
		{
			IEnumerator<T> enumeratorA = a.GetEnumerator ();
			IEnumerator<T> enumeratorB = b.GetEnumerator ();

			while (true)
			{
				bool okA = enumeratorA.MoveNext ();
				bool okB = enumeratorB.MoveNext ();

				if (okA != okB)
				{
					return false;
				}
				if (okA == false)
				{
					return true;
				}

				T value1 = enumeratorA.Current;
				T value2 = enumeratorB.Current;

				if ((value1 == null) &&
					(value2 == null))
				{
					continue;
				}
				if (value1 == null)
				{
					return false;
				}

				if (!predicate (value1, value2))
				{
					return false;
				}
			}
		}

		private static bool CompareEqualNotChecked(System.Collections.IEnumerable a, System.Collections.IEnumerable b)
		{
			System.Collections.IEnumerator enumeratorA = a.GetEnumerator ();
			System.Collections.IEnumerator enumeratorB = b.GetEnumerator ();

			while (true)
			{
				bool okA = enumeratorA.MoveNext ();
				bool okB = enumeratorB.MoveNext ();

				if (okA != okB)
				{
					break;
				}
				if (okA == false)
				{
					return true;
				}

				object value1 = enumeratorA.Current;
				object value2 = enumeratorB.Current;

				if (!Comparer.Equal (value1, value2))
				{
					break;
				}
			}
			
			return false;
		}


		public delegate bool Predicate<T>(T a, T b);
	}
}
