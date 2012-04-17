//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class IListExtensions
	{
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				list.Add (item);
			}
		}

		public static void AddRange(this System.Collections.IList list, System.Collections.IEnumerable collection)
		{
			foreach (object item in collection)
			{
				list.Add (item);
			}
		}

		public static System.IDisposable SuspendNotifications(this System.Collections.IList list)
		{
			ISuspendCollectionChanged collection = list as ISuspendCollectionChanged;

			if (collection == null)
			{
				return null;
			}
			else
			{
				return collection.SuspendNotifications ();
			}
		}

		public static System.IDisposable SuspendNotifications<T>(this IList<T> list)
			where T : AbstractEntity, new()
		{
			ISuspendCollectionChanged collection = list as ISuspendCollectionChanged;

			if (collection == null)
			{
				return null;
			}
			else
			{
				return collection.SuspendNotifications ();
			}
		}

		/// <summary>
		/// Inserts the given element at the given position within the given list. The main
		/// difference with the regular Insert method is that if the list is too small, it is
		/// expanded with default values to the appropriate size before the element is added at its
		/// end.
		/// </summary>
		public static void InsertAtIndex<T>(this IList<T> list, int index, T value)
		{
			list.ThrowIfNull ("list");
			index.ThrowIf (i => i < 0, "index is smaller that zero");

			if (index <= list.Count)
			{
				list.Insert (index, value);
			}
			else
			{
				while (list.Count < index)
				{
					list.Add (default (T));
				}

				list.Add (value);
			}
		}

		/// <summary>
		/// Gets a random element out of <paramref name="list"/>.
		/// </summary>
		/// <typeparam name="T">The type of the elements within <paramref name="list"/></typeparam>
		/// <param name="list">The <see cref="List{T}"/> of which to get a random element.</param>
		/// <returns>A randomly chosen element of <paramref name="list"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="list"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="list"/> is empty.</exception>
		public static T GetRandomElement<T>(this IList<T> list)
		{
			list.ThrowIfNull ("sequence");
			list.ThrowIf (l => l.Count == 0, "list is empty");

			int index = IListExtensions.Dice.Next (0, list.Count);

			return list[index];
		}

		private static System.Random Dice
		{
			get
			{
				if (IListExtensions.dice == null)
				{
					IListExtensions.dice = new System.Random ();
				}

				return IListExtensions.dice;
			}
		}

		[System.ThreadStatic]
		private static System.Random dice;
	}
}
