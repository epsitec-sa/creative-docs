//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		public ReadOnlyDictionary(IDictionary<TKey, TValue> originalDictionary)
		{
			this.dict = originalDictionary ?? ReadOnlyDictionary<TKey, TValue>.empty;
		}

		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return this.dict.Keys;
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.dict[key];
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return this.dict.Values;
			}
		}

		
		public void Add(TKey key, TValue value)
		{
			throw new System.InvalidOperationException ();
		}

		public bool ContainsKey(TKey key)
		{
			return this.dict.ContainsKey (key);
		}

		public bool Remove(TKey key)
		{
			throw new System.InvalidOperationException ();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.dict.TryGetValue (key, out value);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new System.InvalidOperationException ();
		}

		public void Clear()
		{
			throw new System.InvalidOperationException ();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.dict.Contains (item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.dict.CopyTo (array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new System.InvalidOperationException ();
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.dict.GetEnumerator ();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.dict.GetEnumerator ();
		}

		private static readonly Dictionary<TKey, TValue> empty = new Dictionary<TKey, TValue> ();
		
		private readonly IDictionary<TKey, TValue> dict;
	}
}
