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
	}
}
