//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The HostedDictionary class implements a dictionary of T with notifications
	/// to the host when the contents changes (insertion and removal of items).
	/// </summary>
	/// <typeparam name="K">Type of keys stored in dictionary</typeparam>
	/// <typeparam name="V">Type of values stored in dictionary</typeparam>
	public class HostedDictionary<K, V> : IDictionary<K, V>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedDictionary&lt;K, V&gt;"/> class.
		/// </summary>
		/// <param name="host">The host which must be notified.</param>
		public HostedDictionary(IDictionaryHost<K,V> host)
		{
			this.host = host;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedDictionary&lt;K, V&gt;"/> class.
		/// </summary>
		/// <param name="insertionCallback">The insertion callback.</param>
		/// <param name="removalCallback">The removal callback.</param>
		public HostedDictionary(Callback insertionCallback, Callback removalCallback)
		{
			this.host = new CallbackRelay (insertionCallback, removalCallback);
		}

		/// <summary>
		/// Gets the comparer used for key equality testing.
		/// </summary>
		/// <value>The comparer.</value>
		public IEqualityComparer<K> Comparer
		{
			get
			{
				return this.dictionary.Comparer;
			}
		}

		/// <summary>
		/// Determines whether the dictionary contains the specified value.
		/// </summary>
		/// <param name="value">The value to look for.</param>
		/// <returns>
		/// 	<c>true</c> if the dictionary contains the value; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsValue(V value)
		{
			return this.dictionary.ContainsValue (value);
		}

		#region IDictionary<K,V> Members

		public void Add(K key, V value)
		{
			this.dictionary.Add (key, value);
			this.NotifyInsertion (key, value);
		}

		public bool ContainsKey(K key)
		{
			return this.dictionary.ContainsKey (key);
		}

		public ICollection<K> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public bool Remove(K key)
		{
			V value;

			if (this.dictionary.TryGetValue (key, out value))
			{
				this.dictionary.Remove (key);
				this.NotifyRemoval (key, value);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryGetValue(K key, out V value)
		{
			return this.dictionary.TryGetValue (key, out value);
		}

		public ICollection<V> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public V this[K key]
		{
			get
			{
				return this.dictionary[key];
			}
			set
			{
				if (this.ContainsKey (key))
				{
					if (EqualityComparer<V>.Default.Equals (this.dictionary[key], value) == false)
					{
						this.NotifyRemoval (key, this.dictionary[key]);
						this.dictionary[key] = value;
						this.NotifyInsertion (key, value);
					}
				}
				else
				{
					this.dictionary[key] = value;
					this.NotifyInsertion (key, value);
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<K, V>> Members

		public void Add(KeyValuePair<K, V> item)
		{
			this.Add (item.Key, item.Value);
			this.NotifyInsertion (item.Key, item.Value);
		}

		public void Clear()
		{
			KeyValuePair<K, V>[] array = new KeyValuePair<K, V>[this.dictionary.Count];
			this.CopyTo (array, 0);
			this.dictionary.Clear ();

			for (int i = array.Length-1; i >= 0; i--)
			{
				this.NotifyRemoval (array[i].Key, array[i].Value);
			}
		}

		public bool Contains(KeyValuePair<K, V> item)
		{
			ICollection<KeyValuePair<K, V>> collection = this.dictionary;
			return collection.Contains (item);
		}

		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			ICollection<KeyValuePair<K, V>> collection = this.dictionary;
			collection.CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(KeyValuePair<K, V> item)
		{
			ICollection<KeyValuePair<K, V>> collection = this.dictionary;

			if (collection.Remove (item))
			{
				this.NotifyRemoval (item.Key, item.Value);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<K,V>> Members

		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion
		
		public delegate void Callback(K key, V value);

		#region CallbackRelay Class

		private class CallbackRelay : IDictionaryHost<K,V>
		{
			public CallbackRelay(Callback insertionCallback, Callback removalCallback)
			{
				this.insertionCallback = insertionCallback;
				this.removalCallback = removalCallback;
			}

			#region IDictionaryHost<K,V> Members

			public HostedDictionary<K, V> Items
			{
				get
				{
					throw new System.Exception ("The method or operation is not implemented.");
				}
			}

			public void NotifyDictionaryInsertion(K key, V value)
			{
				this.insertionCallback (key, value);
			}

			public void NotifyDictionaryRemoval(K key, V value)
			{
				this.removalCallback (key, value);
			}

			#endregion

			private Callback insertionCallback;
			private Callback removalCallback;
		}

		#endregion

		protected virtual void NotifyInsertion(K key, V value)
		{
			this.host.NotifyDictionaryInsertion (key, value);
		}
		
		protected virtual void NotifyRemoval(K key, V value)
		{
			this.host.NotifyDictionaryRemoval (key, value);
		}

		private IDictionaryHost<K, V> host;
		private Dictionary<K, V> dictionary = new Dictionary<K, V> ();
	}
}
