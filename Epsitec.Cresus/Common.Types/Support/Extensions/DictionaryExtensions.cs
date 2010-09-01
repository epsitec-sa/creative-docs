//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.Extensions
{
	public static class DictionaryExtensions
	{
		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> set, IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			foreach (KeyValuePair<TKey, TValue> item in collection)
			{
				set.Add (item.Key, item.Value);
			}
		}

		public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> set, IEnumerable<System.Tuple<TKey, TValue>> collection)
		{
			foreach (System.Tuple<TKey, TValue> item in collection)
			{
				set.Add (item.Item1, item.Item2);
			}
		}
	}
}
