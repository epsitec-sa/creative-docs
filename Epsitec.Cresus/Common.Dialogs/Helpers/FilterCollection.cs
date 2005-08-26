//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Dialogs.Helpers
{
	public class FilterCollection : System.Collections.IList, System.IDisposable
	{
		public FilterCollection(IFilterCollectionHost host)
		{
			this.host  = host;
			this.list  = new System.Collections.ArrayList ();
		}
		
		
		public FilterItem				this[int index]
		{
			get
			{
				if (index == -1) return null;
				return this.list[index] as FilterItem;
			}
		}
		
		public string					FileDialogFilter
		{
			get
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				for (int i = 0; i < this.list.Count; i++)
				{
					if (i > 0)
					{
						buffer.Append ("|");
					}
					
					buffer.Append (this[i].FileDialogFilter);
				}
				
				return buffer.ToString ();
			}
		}
		
		public int Add(FilterItem item)
		{
			int index = this.list.Add (item);
			this.HandleInsert (item);
			this.HandleChange ();
			return index;
		}
		
		public int Add(string name, string caption, string filter)
		{
			return this.Add (new FilterItem (name, caption, filter));
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
		public bool IsReadOnly
		{
			get
			{
				return false;
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
			object item = this.list[index];
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.HandleChange ();
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.HandleInsert (value);
			this.HandleChange ();
		}

		public void Remove(object value)
		{
			int index = this.list.IndexOf (value);
			if (index >= 0)
			{
				this.HandleRemove (value);
				this.list.RemoveAt (index);
				this.HandleChange ();
			}
		}

		public bool Contains(object value)
		{
			return this.list.Contains (value);
		}

		public void Clear()
		{
			foreach (object item in this.list)
			{
				this.HandleRemove (item);
			}
			this.list.Clear ();
			this.HandleChange ();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}

		public int Add(object value)
		{
			int index = this.list.Add (value);
			
			this.HandleInsert (value);
			this.HandleChange ();
			
			return index;
		}

		public bool IsFixedSize
		{
			get
			{
				return this.list.IsFixedSize;
			}
		}
		#endregion
		
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return this.list.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void CopyTo(System.Array array, int index)
		{
			this.list.CopyTo (array, index);
		}

		public object SyncRoot
		{
			get
			{
				return this.list.SyncRoot;
			}
		}

		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		protected virtual void HandleInsert(object item)
		{
		}
		
		protected virtual void HandleRemove(object item)
		{
		}
		
		protected virtual void HandleChange()
		{
			if (this.host != null)
			{
				this.host.FilterCollectionChanged ();
			}
		}
		
		
		private IFilterCollectionHost			host;
		private System.Collections.ArrayList	list;
	}
}
