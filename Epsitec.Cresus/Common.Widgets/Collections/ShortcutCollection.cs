//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Collections
{
	public class ShortcutCollection : System.Collections.IList, System.IDisposable
	{
		public ShortcutCollection()
		{
			this.list = new System.Collections.ArrayList ();
		}
		
		
		public Shortcut							this[int index]
		{
			get
			{
				if (index == -1) return null;
				return this.list[index] as Shortcut;
			}
		}
		
		public Shortcut[]						Values
		{
			get
			{
				return (Shortcut[]) this.list.ToArray (typeof (Shortcut));
			}
		}
		
		
		public void Define(Shortcut value)
		{
			if (this.list.Count == 1)
			{
				Shortcut old = this[0];
				
				if (old.Equals (value))
				{
					return;
				}
			}
			
			this.list.Clear ();
			this.list.Add (value);
			
			this.NotifyCollectionChanged ();
		}
		
		public void Define(Shortcut[] values)
		{
			if ((values == null) &&
				(this.list.Count == 0))
			{
				return;
			}
			
			if ((values != null) &&
				(this.list.Count == values.Length))
			{
				int same = 0;
				
				for (int i = 0; i < values.Length; i++)
				{
					if (this[i].Equals (values[i]))
					{
						same++;
					}
				}
				
				if (same == values.Length)
				{
					return;
				}
			}
			
			this.list.Clear ();
			
			if ((values != null) &&
				(values.Length > 0))
			{
				this.list.AddRange (values);
			}
			
			this.NotifyCollectionChanged ();
		}
		
		public int  Add(Shortcut value)
		{
			int index = this.list.IndexOf (value);
			
			if (index == -1)
			{
				index = this.list.Add (value);
				this.NotifyCollectionChanged ();
			}
			
			return index;
		}
		
		public void AddRange(System.Collections.ICollection values)
		{
			if (values != null)
			{
				foreach (Shortcut shortcut in values)
				{
					this.Add (shortcut);
				}
				
				this.NotifyCollectionChanged ();
			}
		}
		
		public void Remove(Shortcut value)
		{
			this.list.Remove (value);
			this.NotifyCollectionChanged ();
		}
		
		public bool Contains(Shortcut value)
		{
			return this.list.Contains (value);
		}
		
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.list.Clear ();
				this.list = null;
			}
		}
		
		protected virtual void NotifyCollectionChanged()
		{
		}
		
		
		#region IList Members
		public bool								IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool								IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}
		
		
		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		
		public void RemoveAt(int index)
		{
			this.list.RemoveAt (index);
			this.NotifyCollectionChanged ();
		}

		public void Clear()
		{
			this.list.Clear ();
			this.NotifyCollectionChanged ();
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.list.Insert (index, value);
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove (value as Shortcut);
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value as Shortcut);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}

		int System.Collections.IList.Add(object value)
		{
			return this.Add (value as Shortcut);
		}

		#endregion
		
		#region ICollection Members
		public bool								IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public int								Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public object							SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		
		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}
		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion

		private System.Collections.ArrayList	list;
	}
}
