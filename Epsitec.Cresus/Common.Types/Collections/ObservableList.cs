//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>ObservableList</c> represents a list which provides notifications
	/// when items get added, removed and modified, or when the whole list is
	/// refreshed.
	/// </summary>
	/// <typeparam name="T">The manipulated data type.</typeparam>
	public class ObservableList<T> : IList<T>, INotifyCollectionChanged, System.Collections.ICollection, System.Collections.IList
	{
		public ObservableList()
		{
		}

		/// <summary>
		/// Adds the collection of items to the list.
		/// </summary>
		/// <param name="collection">Items to add</param>
		public void AddRange(IEnumerable<T> collection)
		{
			object[] items = Collection.ToObjectArray (collection);
			
			if (items.Length > 0)
			{
				this.NotifyBeforeChange ();
				int index = this.list.Count;
				this.list.AddRange (collection);
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, items));
			}
		}

		/// <summary>
		/// Converts the list to an array.
		/// </summary>
		/// <returns>An array of T</returns>
		public T[] ToArray()
		{
			return this.list.ToArray ();
		}

		/// <summary>
		/// Sorts the collection using the specified comparer.
		/// </summary>
		/// <param name="comparer">The comparer to use.</param>
		public void Sort(IComparer<T> comparer)
		{
			this.list.Sort (comparer);
		}
		
		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			this.NotifyBeforeChange ();
			this.list.Insert (index, item);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, new object[] { item }));
		}

		public void RemoveAt(int index)
		{
			this.NotifyBeforeChange ();
			T value = this.list[index];
			this.list.RemoveAt (index);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, new object[] { value }));
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				T oldValue = this.list[index];

				if (EqualityComparer<T>.Default.Equals (oldValue, value) == false)
				{
					this.NotifyBeforeChange ();
					this.list[index] = value;
					this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, index, new object[] { value }, index, new object[] { oldValue }));
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		public virtual void Add(T item)
		{
			this.NotifyBeforeChange ();
			int index = this.list.Count;
			this.list.Add (item);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, new object[] { item }));
		}

		public void Clear()
		{
			if (this.list.Count > 0)
			{
				this.NotifyBeforeChange ();
				object[] items = Collection.ToObjectArray (this.list);
				this.list.Clear ();
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, 0, items));
			}
		}

		public bool Contains(T item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			int index = this.list.IndexOf (item);
			if (index >= 0)
			{
				this.NotifyBeforeChange ();
				this.list.RemoveAt (index);
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, new object[] { item }));
				return true;
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.list.GetEnumerator ();
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			System.Collections.ICollection collection = this.list;
			collection.CopyTo (array, index);
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				return this.list;
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			this.Add ((T) value);
			return this.Count-1;
		}

		void System.Collections.IList.Clear()
		{
			this.Clear ();
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains ((T) value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf ((T) value);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.Insert (index, (T) value);
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove ((T) value);
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			this.RemoveAt (index);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				this[index] = (T) value;
			}
		}

		#endregion


		protected virtual void NotifyBeforeChange()
		{
		}
		
		protected virtual void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged (this, e);
			}
		}

		#region INotifyCollectionChanged Members

		public event Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs> CollectionChanged;

		#endregion

		protected List<T> list = new List<T> ();
	}
}
