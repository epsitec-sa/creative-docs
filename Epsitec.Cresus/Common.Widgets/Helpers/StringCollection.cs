namespace Epsitec.Common.Widgets.Helpers
{
	public class StringCollection : System.Collections.IList, System.IDisposable
	{
		public StringCollection()
		{
			this.list  = new System.Collections.ArrayList ();
		}
		
		public string							this[int index]
		{
			get
			{
				return this.list[index].ToString ();
			}
		}
		
		
		public int FindExactMatch(string find)
		{
			find = find.ToUpper ();
			
			for (int i = 0; i < this.list.Count; i++)
			{
				string text = this[i].ToUpper ();
				
				if (text == find)
				{
					return i;
				}
			}
			
			return -1;
		}
		
		public int FindStartMatch(string find)
		{
			find = find.ToUpper ();
			
			for (int i = 0; i < this.list.Count; i++)
			{
				string text = this[i].ToUpper ();
				
				if (text.StartsWith (find))
				{
					return i;
				}
			}
			
			return -1;
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
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.HandleInsert (value);
		}

		public void Remove(object value)
		{
			this.HandleRemove (value);
			this.list.Remove (value);
		}

		public bool Contains(object value)
		{
			return this.Contains (value);
		}

		public void Clear()
		{
			foreach (object item in this.list)
			{
				this.HandleRemove (item);
			}
			this.list.Clear ();
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value);
		}

		public int Add(object value)
		{
			int index = this.list.Add (value);
			this.HandleInsert (value);
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
		
		
		
		private System.Collections.ArrayList	list;
	}
}
