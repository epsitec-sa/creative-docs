//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Widgets.Collections
{
	/// <summary>
	/// The <c>WidgetCollection</c> class implements a collection of specific
	/// widgets, which provides notifications to the host when changes happen.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class WidgetCollection<T> : IList<T>, System.Collections.IList where T : Widget
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
		
		public virtual bool						AutoEmbedding
		{
			get
			{
				return true;
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

		public T[] ToArray()
		{
			return this.list.ToArray ();
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			this.list.Insert (index, item);
			this.HandleInsert (item);
		}

		T IList<T>.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new System.NotSupportedException ();
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			if (item == null)
			{
				throw new System.ArgumentNullException ();
			}
			if (this.list.Contains (item))
			{
				throw new System.InvalidOperationException ("Duplicate insertion");
			}
			
			this.list.Add (item);
			this.HandleInsert (item);
		}

		public bool Contains(T item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo (array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (this.list.Contains (item))
			{
				this.HandleRemove (item);
				this.list.Remove (item);
				this.HandlePostRemove (item);
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion
		
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

		void System.Collections.IList.Insert(int index, object value)
		{
			this.list.Insert (index, value as T);
			this.HandleInsert (value as T);
		}

		void System.Collections.IList.Remove(object value)
		{
			this.HandleRemove (value as T);
			this.list.Remove (value as T);
			this.HandlePostRemove (value as T);
		}

		bool System.Collections.IList.Contains(object value)
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

		int System.Collections.IList.IndexOf(object value)
		{
			return this.list.IndexOf (value as T);
		}

		int System.Collections.IList.Add(object value)
		{
			T item = value as T;
			
			if (value == null)
			{
				throw new System.ArgumentNullException ();
			}
			if (item == null)
			{
				throw new System.InvalidCastException ();
			}
			
			int index = this.list.Count;
			this.list.Add (item);
			this.HandleInsert (item);
			return index;
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion
		
		#region ICollection Members

		bool System.Collections.ICollection.IsSynchronized
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

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			System.Collections.IList list = this.list;
			list.CopyTo (array, index);
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				System.Collections.IList list = this.list;
				return list.SyncRoot;
			}
		}

		#endregion
		
		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}
		
		#endregion
		
		protected void HandleInsert(T item)
		{
			if (this.AutoEmbedding)
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
	}
}
