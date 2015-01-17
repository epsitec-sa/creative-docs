using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec
{
	public class CompositeDictionary : IDictionary<IKey, object>
	{
		public static CompositeDictionary Create(object content)
		{
			var map = content as Dictionary<IKey, object>;
			return map == null ? null : new CompositeDictionary (map);
		}

		public CompositeDictionary()
		{
			this.map = new Dictionary<IKey, object> ();
		}

		public CompositeDictionary(Dictionary<IKey, object> map)
		{
			this.map = map;
		}

		public ICollection<IKey> FirstLevelKeys
		{
			get
			{
				return this.map.Keys;
			}
		}

		public object this[IEnumerable<object> subkeys]
		{
			get
			{
				return this[Key.Create (subkeys)];
			}
			set
			{
				this[Key.Create (subkeys)] = value;
			}
		}

		public object this[params object[] subkeys]
		{
			get
			{
				return this[Key.Create (subkeys)];
			}
			set
			{
				this[Key.Create (subkeys)] = value;
			}
		}

		public bool ContainsKey(IEnumerable<object> subkeys)
		{
			return this.ContainsKey (Key.Create (subkeys));
		}

		public bool ContainsKey(params object[] subkeys)
		{
			return this.ContainsKey (Key.Create (subkeys));
		}

		public bool TryGetValue(IEnumerable<object> subkeys, out object value)
		{
			return this.TryGetValue (Key.Create (subkeys), out value);
		}

		public void Add(IEnumerable<object> subkeys, object value)
		{
			this.Add (Key.Create (subkeys), value);
		}

		public bool Remove(IEnumerable<object> subkeys)
		{
			return this.Remove (Key.Create (subkeys));
		}

		public bool Remove(params object[] subkeys)
		{
			return this.Remove (Key.Create (subkeys));
		}


		#region IDictionary<IKey,object> Members

		public ICollection<IKey> Keys
		{
			get
			{
				return CompositeDictionary.GetKeys (CompositeKey.Empty, this.map).ToList ();
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return CompositeDictionary.Flatten (this.map).ToList ();
			}
		}

		public object this[IKey key]
		{
			get
			{
				object value;
				if (this.TryGetValue (key, out value))
				{
					return value;
				}
				throw new KeyNotFoundException (key.ToString());
			}
			set
			{
				this.Add (key, value);
			}
		}

		public bool ContainsKey(IKey key)
		{
			var map = this.map;
			foreach (var subkey in key)
			{
				if (map == null || !map.ContainsKey (subkey))
				{
					return false;
				}
				map = map[subkey] as Dictionary<IKey, object>;
			}
			return true;
		}

		public bool TryGetValue(IKey key, out object value)
		{
			return this.TryGetValue (key as IEnumerable<IKey>, out value);
		}

		public void Add(IKey key, object value)
		{
			var subkeys = new CompositeDictionary.HeadAndLast (key);
			var map = this.map;
			foreach (var subkey in subkeys.Head)
			{
				map = map.GetOrAdd (subkey, _ => new Dictionary<IKey, object> ()) as Dictionary<IKey, object>;
			}
			map[subkeys.Last] = value;
		}

		public bool Remove(IKey key)
		{
			var subkeys = new CompositeDictionary.HeadAndLast (key);
			object value;
			if (this.TryGetValue (subkeys.Head, out value))
			{
				var map = value as IDictionary<IKey, object>;
				if (map != null)
				{
					return map.Remove (subkeys.Last);
				}
			}
			return false;
		}

		#endregion

		#region ICollection<KeyValuePair<IKey,object>> Members

		public void Add(KeyValuePair<IKey, object> item)
		{
			this.Add (item.Key, item.Value);
		}

		public void Clear()
		{
			this.map.Clear ();
		}

		public bool Contains(KeyValuePair<IKey, object> item)
		{
			object value;
			return this.TryGetValue (item.Key, out value) && object.Equals (value, item.Value);

		}

		public void CopyTo(KeyValuePair<IKey, object>[] array, int arrayIndex)
		{
			this.ToArray ().CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return Keys.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(KeyValuePair<IKey, object> item)
		{
			var subkeys = new CompositeDictionary.HeadAndLast (item.Key);
			object value;
			if (this.TryGetValue (subkeys.Head, out value))
			{
				var map = value as IDictionary<IKey, object>;
				if (map != null && map.TryGetValue (subkeys.Last, out value) && object.Equals (value, item.Value))
				{
					return map.Remove (subkeys.Last);
				}
			}
			return false;
		}

		#endregion

		#region IEnumerable<KeyValuePair<IKey,object>> Members

		public IEnumerator<KeyValuePair<IKey, object>> GetEnumerator()
		{
			return CompositeDictionary.GetKeyValuePairs (CompositeKey.Empty, this.map).GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion


		private class HeadAndLast
		{
			public HeadAndLast(IKey key)
			{
				var array = key.ToArray ();
				var lastIndex = array.Length - 1;
				this.Head = array.Take (lastIndex);
				this.Last = array[lastIndex];
			}

			public IEnumerable<IKey> Head
			{
				get;
				private set;
			}

			public IKey Last
			{
				get;
				private set;
			}
		}

		private static IEnumerable<IKey> GetKeys(ICompositeKey rootKey, IDictionary<IKey, object> map)
		{
			foreach (var kv in map)
			{
				var newMap = kv.Value as IDictionary<IKey, object>;
				var newKey = rootKey.Concat(kv.Key);
				if (newMap == null)
				{
					yield return newKey;
				}
				else
				{
					foreach (var key in CompositeDictionary.GetKeys (newKey, newMap))
					{
						yield return key;
					}
				}
			}
		}

		private static IEnumerable<KeyValuePair<IKey, object>> GetKeyValuePairs(ICompositeKey rootKey, IDictionary<IKey, object> map)
		{
			foreach (var kv in map)
			{
				var newMap = kv.Value as IDictionary<IKey, object>;
				var newKey = rootKey.Concat (kv.Key);
				if (newMap == null)
				{
					yield return new KeyValuePair<IKey, object> (newKey, kv.Value);
				}
				else
				{
					foreach (var kv1 in CompositeDictionary.GetKeyValuePairs (newKey, newMap))
					{
						yield return kv1;
					}
				}
			}
		}

		private bool TryGetValue(IEnumerable<IKey> subkeys, out object value)
		{
			value = null;
			var map = this.map;
			var result = false;
			foreach (var subkey in subkeys)
			{
				if (map == null || !map.TryGetValue (subkey, out value))
				{
					return false;
				}
				map = value as Dictionary<IKey, object>;
				result = true;
			}
			return result;
		}

		private static IEnumerable<object> Flatten(IDictionary<IKey, object> map)
		{
			foreach (var value in map.Values)
			{
				map = value as IDictionary<IKey, object>;
				if (map == null)
				{
					yield return value;
				}
				else
				{
					foreach (var v in CompositeDictionary.Flatten (map))
					{
						yield return v;
					}
				}
			}

		}

		private readonly Dictionary<IKey, object> map;
	}
}
