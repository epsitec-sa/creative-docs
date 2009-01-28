//	Copyright � 2006-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>ObservableList</c> represents a list which provides notifications
	/// when items get added, removed and modified, or when the whole list is
	/// refreshed.
	/// </summary>
	/// <typeparam name="T">The manipulated data type.</typeparam>
	public class ObservableList<T> : AbstractObservableList, IList<T>, INotifyCollectionChanged, System.Collections.ICollection, System.Collections.IList, IReadOnly, IReadOnlyLock
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableList&lt;T&gt;"/> class.
		/// </summary>
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
				ObservableList<T> that = this.NotifyBeforeChange ();
				int index = that.list.Count;
				that.list.AddRange (collection);
				that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, items));
			}
		}

		/// <summary>
		/// Adds the collection of items to the list.
		/// </summary>
		/// <param name="collection">Items to add.</param>
		public void AddRange(params T[] collection)
		{
			this.AddRange ((IEnumerable<T>) collection);
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
			ObservableList<T> that = this.NotifyBeforeChange ();

			object[] oldItems = Collection.ToObjectArray (that.list);

			that.list.Sort (comparer);

			object[] newItems = Collection.ToObjectArray (that.list);

			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, 0, newItems, 0, oldItems));
		}

		/// <summary>
		/// Sorts the collection using the specified comparison.
		/// </summary>
		/// <param name="comparison">The comparison to use.</param>
		public void Sort(System.Comparison<T> comparison)
		{
			ObservableList<T> that = this.NotifyBeforeChange ();

			object[] oldItems = Collection.ToObjectArray (that.list);

			that.list.Sort (comparison);

			object[] newItems = Collection.ToObjectArray (that.list);

			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, 0, newItems, 0, oldItems));
		}

		/// <summary>
		/// Replaces the list with the contents of the specified collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		public void ReplaceWithRange(IEnumerable<T> collection)
		{
			ObservableList<T> that = this.NotifyBeforeChange ();

			object[] oldItems = Collection.ToObjectArray (that.list);
			object[] newItems = Collection.ToObjectArray (collection);

			that.list.Clear ();
			that.list.AddRange (collection);

			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, 0, newItems, 0, oldItems));
		}

		/// <summary>
		/// Temporarily disables all change notifications. Any changes which
		/// happen until <c>Dispose</c> is called on the returned object will
		/// not generate events.
		/// </summary>
		/// <returns>An object you will have to <c>Dispose</c> in order to re-enable
		/// the notifications.</returns>
		public System.IDisposable DisableNotifications()
		{
			System.Threading.Interlocked.Increment (ref this.silent);
			return new ReEnabler (this);
		}

		/// <summary>
		/// Makes this collection read only. Any further modification is prohibited
		/// and will throw an exception.
		/// </summary>
		public void Lock()
		{
			this.isReadOnly = true;
		}

		internal void Unlock()
		{
			this.isReadOnly = false;
		}

		/// <summary>
		/// Gets target for the specified collection changing event handler.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The target handler instance or <c>null</c>.</returns>
		public override object GetCollectionChangingTarget(int index)
		{
			Support.EventHandler handler;

			lock (this.list)
			{
				handler = this.collectionChangingEvent;
			}

			if (handler != null)
			{
				System.Delegate[] delegates = handler.GetInvocationList ();

				if ((index >= 0) &&
					(index < delegates.Length))
				{
					return delegates[index].Target;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets target for the specified collection changed event handler.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>The target handler instance or <c>null</c>.</returns>
		public override object GetCollectionChangedTarget(int index)
		{
			Support.EventHandler<CollectionChangedEventArgs> handler;

			lock (this.list)
			{
				handler = this.collectionChangedEvent;
			}

			if (handler != null)
			{
				System.Delegate[] delegates = handler.GetInvocationList ();

				if ((index >= 0) &&
					(index < delegates.Length))
				{
					return delegates[index].Target;
				}
			}

			return null;
		}

		#region IReadOnlyLock Members

		void IReadOnlyLock.Lock()
		{
			this.Lock ();
		}

		void IReadOnlyLock.Unlock()
		{
			this.Unlock ();
		}

		#endregion

		#region IReadOnly Members

		bool IReadOnly.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		#endregion

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			ObservableList<T> that = this.NotifyBeforeChange ();
			that.list.Insert (index, item);
			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, new object[] { item }));
		}

		public void RemoveAt(int index)
		{
			ObservableList<T> that = this.NotifyBeforeChange ();
			T value = that.list[index];
			that.list.RemoveAt (index);
			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, new object[] { value }));
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
					ObservableList<T> that = this.NotifyBeforeChange ();
					that.NotifyBeforeSet (index, oldValue, value);
					that.list[index] = value;
					that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, index, new object[] { value }, index, new object[] { oldValue }));
				}
			}
		}

		#endregion

		#region ICollection<T> Members

		public virtual void Add(T item)
		{
			ObservableList<T> that = this.NotifyBeforeChange ();
			int index = that.list.Count;
			that.list.Add (item);
			that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, new object[] { item }));
		}

		public void Clear()
		{
			if (this.list.Count > 0)
			{
				ObservableList<T> that = this.NotifyBeforeChange ();
				object[] items = Collection.ToObjectArray (that.list);
				that.list.Clear ();
				that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, 0, items));
			}
		}

		public bool Contains(T item)
		{
			return this.list.Contains (item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.list.CopyTo (array, arrayIndex);
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
				return this.isReadOnly;
			}
		}

		public bool Remove(T item)
		{
			int index = this.list.IndexOf (item);
			if (index >= 0)
			{
				ObservableList<T> that = this.NotifyBeforeChange ();
				that.list.RemoveAt (index);
				that.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, new object[] { item }));
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

		public object SyncRoot
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

		/// <summary>
		/// Silently replaces the item. This will not generate any event and
		/// will not go through the <see cref="GetWorkingList"/> method either.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="item">The item.</param>
		protected void PatchItem(int index, T item)
		{
			this.list[index] = item;
		}

		protected virtual void NotifyBeforeSet(int index, T oldValue, T newValue)
		{
		}

		/// <summary>
		/// Notifies the event handlers before the list changes. This calls
		/// <see cref="GetWorkingList"/> to decide on which list the changes
		/// will be applied and then ensures that the collection is writable.
		/// </summary>
		/// <returns>The writable list to use.</returns>
		protected ObservableList<T> NotifyBeforeChange()
		{
			ObservableList<T> that = this.GetWorkingList ();

			if (that.IsReadOnly)
			{
				throw new System.InvalidOperationException ("Read-only list may not be changed");
			}

			that.OnCollectionChanging ();

			return that;
		}

		/// <summary>
		/// Gets the observable list on which to apply modifications.
		/// This method should be overridden if a copy on write protection
		/// needs to be implemented for this list.
		/// </summary>
		/// <returns>The list to use.</returns>
		protected virtual ObservableList<T> GetWorkingList()
		{
			return this;
		}

		protected virtual void OnCollectionChanging()
		{
			if (this.silent == 0)
			{
				Epsitec.Common.Support.EventHandler handler;

				lock (this.list)
				{
					handler = this.collectionChangingEvent;
				}

				if (handler != null)
				{
					handler (this);
				}
			}
		}

		protected virtual void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			if (this.silent == 0)
			{
				Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs> handler;

				lock (this.list)
				{
					handler = this.collectionChangedEvent;
				}

				if (handler != null)
				{
					handler (this, e);
				}
			}
		}

		#region INotifyCollectionChanged Members

		/// <summary>
		/// Occurs when the list changes, either by adding or removing items.
		/// <remarks>Subscribing to this event is thread safe.</remarks>
		/// </summary>
		public event Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs> CollectionChanged
		{
			add
			{
				lock (this.list)
				{
					this.collectionChangedEvent += value;
				}
			}
			remove
			{
				lock (this.list)
				{
					this.collectionChangedEvent -= value;
				}
			}
		}

		#endregion

		/// <summary>
		/// Occurs before the collection changes. There is no way to prevent
		/// the change; the event is only informative.
		/// </summary>
		public event Epsitec.Common.Support.EventHandler CollectionChanging
		{
			add
			{
				lock (this.list)
				{
					this.collectionChangingEvent += value;
				}
			}
			remove
			{
				lock (this.list)
				{
					this.collectionChangingEvent -= value;
				}
			}
		}

		#region ReEnabler Class

		private class ReEnabler : System.IDisposable
		{
			public ReEnabler(ObservableList<T> list)
			{
				this.list = list;
			}

			#region IDisposable Members

			public void Dispose()
			{
				System.Threading.Interlocked.Decrement (ref this.list.silent);
			}

			#endregion

			ObservableList<T> list;
		}

		#endregion

		private Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs> collectionChangedEvent;
		private Epsitec.Common.Support.EventHandler collectionChangingEvent;

		private readonly List<T> list = new List<T> ();
		private int silent;
		private bool isReadOnly;
	}
}
