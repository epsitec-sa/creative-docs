//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
		
		
		public int  Add(Shortcut value)
		{
			return this.list.Add (value);
		}
		
		public void AddRange(System.Collections.ICollection values)
		{
			if (values != null)
			{
				foreach (Shortcut shortcut in values)
				{
					this.Add (shortcut);
				}
			}
		}
		
		public void Remove(Shortcut value)
		{
			this.list.Remove (value);
		}
		
		public bool Contains(Shortcut value)
		{
			return this.list.Contains (value);
		}
		
		public bool Match(Shortcut value)
		{
			foreach (Shortcut shortcut in this.list)
			{
				if (shortcut.Match (value))
				{
					return true;
				}
			}
			
			return false;
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
		}

		public void Clear()
		{
			this.list.Clear ();
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
