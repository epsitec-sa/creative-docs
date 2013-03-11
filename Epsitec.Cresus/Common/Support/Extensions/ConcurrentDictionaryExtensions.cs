//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Epsitec.Common.Support.Extensions
{
	public static class ConcurrentDictionaryExtensions
	{
		/// <summary>
		/// Removes the specified key from the concurrent dictionary.
		/// </summary>
		/// <typeparam name="TKey">The type of the key.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="dict">The concurrent dictionary.</param>
		/// <param name="key">The key.</param>
		/// <returns><c>true</c> if the key could be removed; otherwise, <c>false</c>.</returns>
		public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
		{
			TValue value;

			return dict.TryRemove (key, out value);
		}
	}
}
