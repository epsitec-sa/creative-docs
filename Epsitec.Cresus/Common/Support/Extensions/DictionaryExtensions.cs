//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;
using System.Collections.Generic;


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

		/// <summary>
		/// Returns a read only dictionary that contains the elements of the given
		/// <see cref="IDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <remarks>
		/// This LINQ method will execute immediately and has a complexity of O(1).
		/// </remarks>
		/// <typeparam name="TKey">The type of the key elements.</typeparam>
		/// <typeparam name="TValue">The type of the value elements.</typeparam>
		/// <param name="sequence">The dictionary of which to obtain a read only instance.>/param>
		/// <returns>The new readonly dictionary.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dictionary"/> is <c>null</c>.</exception>
		public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
		{
			dictionary.ThrowIfNull ("dictionary");

			return new ReadOnlyDictionary<TKey, TValue> (dictionary);
		}
	}
}
