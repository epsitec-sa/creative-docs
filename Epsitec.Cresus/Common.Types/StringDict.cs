//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La class StringDict implémente un dictionnaire de clefs/valeurs de
	/// type string.
	/// </summary>
	public class StringDict : System.Collections.IDictionary, Types.IStringDict
	{
		public StringDict()
		{
		}
		
		public StringDict(Types.IStringDict model) : this ()
		{
			StringDict.Copy (model, this);
		}
		
		
		#region IDictionary Members
		public bool								IsReadOnly
		{
			get
			{
				return this.hash.IsReadOnly;
			}
		}
		
		public bool								IsFixedSize
		{
			get
			{
				return this.hash.IsFixedSize;
			}
		}
		
		System.Collections.ICollection			System.Collections.IDictionary.Values
		{
			get
			{
				return this.hash.Values;
			}
		}
		
		System.Collections.ICollection			System.Collections.IDictionary.Keys
		{
			get
			{
				return this.hash.Keys;
			}
		}
		
		object									System.Collections.IDictionary.this[object key]
		{
			get
			{
				return this.hash[key];
			}
			set
			{
				this.hash[key] = value;
			}
		}

		
		System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator()
		{
			return this.hash.GetEnumerator ();
		}

		
		void System.Collections.IDictionary.Add(object key, object value)
		{
			this.hash.Add (key, value);
		}
		
		void System.Collections.IDictionary.Remove(object key)
		{
			this.hash.Remove (key);
		}

		bool System.Collections.IDictionary.Contains(object key)
		{
			return this.hash.Contains (key);
		}
		#endregion

		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return this.hash.IsSynchronized;
			}
		}
		
		public object							SyncRoot
		{
			get
			{
				return this.hash.SyncRoot;
			}
		}
		
		public void CopyTo(System.Array array, int index)
		{
			this.hash.CopyTo (array, index);
		}
		#endregion

		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.hash.GetEnumerator ();
		}
		#endregion

		#region IStringDict Members
		public string[]							Keys
		{
			get
			{
				string[] keys = new string[this.hash.Count];
				this.hash.Keys.CopyTo (keys, 0);
				return keys;
			}
		}
		
		public string							this[string key]
		{
			get
			{
				return this.hash[key] as string;
			}
			set
			{
				this.hash[key] = value;
			}
		}
		
		public void Add(string key, string value)
		{
			this.hash.Add (key, value);
		}

		public void Remove(string key)
		{
			this.hash.Remove (key);
		}
		
		public bool Contains(string key)
		{
			return this.hash.Contains (key);
		}
		#endregion
		
		public int								Count
		{
			get
			{
				return this.hash.Count;
			}
		}
		
		
		public void Clear()
		{
			this.hash.Clear ();
		}
		
		
		public static void Copy(Types.IStringDict model, Types.IStringDict target)
		{
			string[] keys = model.Keys;
			
			for (int i = 0; i < keys.Length; i++)
			{
				target.Add (keys[i], model[keys[i]]);
			}
		}
		
		
		protected System.Collections.Hashtable	hash = new System.Collections.Hashtable ();
	}
}
