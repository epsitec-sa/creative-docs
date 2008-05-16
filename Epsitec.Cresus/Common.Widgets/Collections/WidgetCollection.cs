//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	public class WidgetCollection<T> : System.Collections.IList where T : Widget
	{
		public WidgetCollection(IWidgetCollectionHost<T> host)
		{
			this.host = host;
			this.list = new List<T> ();
		}
		
		
		public T								this[int index]
		{
			get
			{
				return this.list[index];
			}
		}
		
		public T								this[string name]
		{
			get
			{
				foreach (T item in this.list)
				{
					if (item.Name == name)
					{
						return item;
					}
				}
				return null;
			}
		}
		
		public bool								AutoEmbedding
		{
			get
			{
				return this.auto_embedding;
			}
			set
			{
				this.auto_embedding = value;
			}
		}
		
		
		public void AddRange(IEnumerable<T> widgets)
		{
			this.list.AddRange (widgets);
			
			foreach (T widget in widgets)
			{
				this.HandleInsert (widget);
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
				this.list[index] = value as T;
			}
		}

		public void RemoveAt(int index)
		{
			T item = this.list[index] as T;
			this.HandleRemove (item);
			this.list.RemoveAt (index);
			this.HandlePostRemove (item);
		}

		public void Insert(int index, object value)
		{
			this.list.Insert (index, value as T);
			this.HandleInsert (value as T);
		}

		public void Remove(object value)
		{
			this.HandleRemove (value as T);
			this.list.Remove (value as T);
			this.HandlePostRemove (value as T);
		}

		public bool Contains(object value)
		{
			return this.list.Contains (value as T);
		}

		public void Clear()
		{
			T[] widgets = this.list.ToArray ();
			
			for (int i = 0; i < widgets.Length; i++)
			{
				this.HandleRemove (widgets[i]);
			}
			
			this.list.Clear ();
			
			for (int i = 0; i < widgets.Length; i++)
			{
				this.HandlePostRemove (widgets[i]);
			}
		}

		public int IndexOf(object value)
		{
			return this.list.IndexOf (value as T);
		}

		public int Add(object value)
		{
			if (value == null)
			{
				throw new System.ArgumentNullException ();
			}
			
			if (value is T)
			{
				int index = this.list.Count;
				this.list.Add (value as T);
				this.HandleInsert (value as T);
				return index;
			}
			
			throw new System.ArgumentException (string.Format ("Expecting {0}, got {1} instead", typeof (T).Name, value.GetType ().Name));
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion
		
		#region ICollection Members
		public bool IsSynchronized
		{
			get
			{
				return false;
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
			System.Collections.IList list = this.list;
			list.CopyTo (array, index);
		}

		public object SyncRoot
		{
			get
			{
				System.Collections.IList list = this.list;
				return list.SyncRoot;
			}
		}

		#endregion
		
		#region IEnumerable Members
		public System.Collections.IEnumerator GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		#endregion
		
		protected void HandleInsert(T item)
		{
			if (this.auto_embedding)
			{
				Widget embedder = this.host as Widget;
				
				System.Diagnostics.Debug.Assert (embedder != null);
				System.Diagnostics.Debug.Assert (item.Parent == null);
				
				item.SetEmbedder (embedder);
			}
			
			this.RenumberItems ();
			this.host.NotifyInsertion (item);
		}
		
		protected void HandleRemove(T item)
		{
			this.host.NotifyRemoval (item);
			this.RenumberItems ();
		}
		
		protected void HandlePostRemove(T item)
		{
			item.Index = -1;
			this.host.NotifyPostRemoval (item);
		}
		
		protected void RenumberItems()
		{
			for (int i = 0; i < this.list.Count; i++)
			{
				this.list[i].Index = i;
			}
		}
		
		
		readonly List<T>						list;
		readonly IWidgetCollectionHost<T>		host;
		bool									auto_embedding;
	}
}
