namespace Epsitec.Common.Widgets.Helpers
{
	public class WidgetCollection : System.Collections.IList, System.IDisposable
	{
		public WidgetCollection(IWidgetCollectionHost host)
		{
			this.host = host;
			this.list = new System.Collections.ArrayList ();
		}
		
		public Widget this[int index]
		{
			get
			{
				return this.list[index] as Widget;
			}
		}
		
		public Widget this[string name]
		{
			get
			{
				foreach (Widget item in this.list)
				{
					if (item.Name == name)
					{
						return item;
					}
				}
				return null;
			}
		}
		
		public void Dispose()
		{
			System.Diagnostics.Debug.Assert (this.list.Count == 0);
			
			this.host = null;
			this.list = null;
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
			Widget item = this.list[index] as Widget;
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.RenumberItems ();
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value);
			this.HandleInsert (value as Widget);
			this.RenumberItems ();
		}

		public void Remove(object value)
		{
			this.HandleRemove (value as Widget);
			this.list.Remove (value);
			this.RenumberItems ();
		}

		public bool Contains(object value)
		{
			return this.Contains (value);
		}

		public void Clear()
		{
			foreach (Widget item in this.list)
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
			if (value is Widget)
			{
				int index = this.list.Add (value);
				this.HandleInsert (value as Widget);
				this.RenumberItems ();
				return index;
			}
			
			throw new System.ArgumentException ("Expecting Widget, got " + value.GetType ().Name);
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
		
		protected void HandleInsert(Widget item)
		{
			this.host.NotifyInsertion (item);
		}
		
		protected void HandleRemove(Widget item)
		{
			this.host.NotifyRemoval (item);
		}
		
		protected void RenumberItems()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				Widget item = this.list[i] as Widget;
				item.Index = i;
			}
		}
		
		
		private System.Collections.ArrayList	list;
		private IWidgetCollectionHost			host;
	}
}
