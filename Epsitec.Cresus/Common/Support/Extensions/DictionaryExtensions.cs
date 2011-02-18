//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

#if DOTNET35
namespace System
{
	public class Tuple<T1, T2>
	{
		public Tuple(T1 item1, T2 item2)
		{
			this.item1 = item1;
			this.item2 = item2;
		}

		public T1 Item1
		{
			get
			{
				return this.item1;
			}
		}

		public T2 Item2
		{
			get
			{
				return this.item2;
			}
		}

		private readonly T1 item1;
		private readonly T2 item2;
	}
}
#endif

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
