//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Epsitec.Common.Support.Extensions
{
	public static class ConcurrentDictionaryExtensions
	{
		public static bool Remove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dict, TKey key)
		{
			TValue value;

			return dict.TryRemove (key, out value);
		}
	}
}
