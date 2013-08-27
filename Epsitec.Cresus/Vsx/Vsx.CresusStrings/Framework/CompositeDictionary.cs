using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec
{
	public class CompositeDictionary : IDictionary<ICompositeKey, object>
	{
		public CompositeDictionary()
		{
			this.map = new Dictionary<IKey, object> ();
		}

		public CompositeDictionary(Dictionary<IKey, object> map)
		{
			this.map = map;
		}

		public ReadOnlyDictionary<IKey, object> Map
		{
			get
			{
				return new ReadOnlyDictionary<IKey, object> (this.map);
			}
		}

		#region IDictionary<ICompositeKey,object> Members

		public ICollection<ICompositeKey> Keys
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

		public object this[ICompositeKey key]
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

		public bool ContainsKey(ICompositeKey key)
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

		public bool TryGetValue(ICompositeKey key, out object value)
		{
			return this.TryGetValue (key as IEnumerable<IKey>, out value);
		}

		public void Add(ICompositeKey key, object value)
		{
			var subkeys = new CompositeDictionary.HeadAndLast (key);
			var map = this.map;
			foreach (var subkey in subkeys.Head)
			{
				map = map.GetOrAdd (subkey, _ => new Dictionary<IKey, object> ()) as Dictionary<IKey, object>;
			}
			map[subkeys.Last] = value;
		}

		public bool Remove(ICompositeKey key)
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

		#region ICollection<KeyValuePair<ICompositeKey,object>> Members

		public void Add(KeyValuePair<ICompositeKey, object> item)
		{
			this.Add (item.Key, item.Value);
		}

		public void Clear()
		{
			this.map.Clear ();
		}

		public bool Contains(KeyValuePair<ICompositeKey, object> item)
		{
			object value;
			return this.TryGetValue (item.Key, out value) && object.Equals (value, item.Value);

		}

		public void CopyTo(KeyValuePair<ICompositeKey, object>[] array, int arrayIndex)
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

		public bool Remove(KeyValuePair<ICompositeKey, object> item)
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

		#region IEnumerable<KeyValuePair<ICompositeKey,object>> Members

		public IEnumerator<KeyValuePair<ICompositeKey, object>> GetEnumerator()
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
			public HeadAndLast(ICompositeKey key)
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

		private static IEnumerable<ICompositeKey> GetKeys(ICompositeKey rootKey, IDictionary<IKey, object> map)
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

		private static IEnumerable<KeyValuePair<ICompositeKey, object>> GetKeyValuePairs(ICompositeKey rootKey, IDictionary<IKey, object> map)
		{
			foreach (var kv in map)
			{
				var newMap = kv.Value as IDictionary<IKey, object>;
				var newKey = rootKey.Concat (kv.Key);
				if (newMap == null)
				{
					yield return new KeyValuePair<ICompositeKey, object> (newKey, kv.Value);
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
			foreach (var subkey in subkeys)
			{
				if (map == null || !map.TryGetValue (subkey, out value))
				{
					return false;
				}
				map = value as Dictionary<IKey, object>;
			}
			return true;
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
