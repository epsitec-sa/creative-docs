//	Copyright © 2006-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>ReadOnlyObservableList</c> represents a read-only wrapper around
	/// an <see cref="T:ObservableList"/>.
	/// </summary>
	/// <typeparam name="T">The manipulated data type.</typeparam>
	public sealed class ReadOnlyObservableList<T> :
		IList<T>,
		INotifyCollectionChanged,
		System.Collections.ICollection,
		System.Collections.IList
	{
		public ReadOnlyObservableList(ObservableList<T> list)
		{
			this.list = list;

			EventRelay.CreateEventRelay (this);
		}

		/// <summary>
		/// Converts the list to an array.
		/// </summary>
		/// <returns>An array of T</returns>
		public T[] ToArray()
		{
			return this.list.ToArray ();
		}

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new System.InvalidOperationException ();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}

		#endregion

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			throw new System.InvalidOperationException ();
		}

		void ICollection<T>.Clear()
		{
			throw new System.InvalidOperationException ();
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

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			throw new System.InvalidOperationException ();
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
			throw new System.InvalidOperationException ();
		}

		void System.Collections.IList.Clear()
		{
			throw new System.InvalidOperationException ();
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
			throw new System.InvalidOperationException ();
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return true;
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				return true;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
			throw new System.InvalidOperationException ();
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this[index];
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}

		#endregion

		private void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
		{
			this.OnCollectionChanged (e);
		}
		
		private void OnCollectionChanged(CollectionChangedEventArgs e)
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

		private class EventRelay
		{
			public static void CreateEventRelay(ReadOnlyObservableList<T> host)
			{
				EventRelay relay = new EventRelay ();
				relay.host = new Weak<ReadOnlyObservableList<T>> (host);
				host.list.CollectionChanged += relay.HandleCollectionChanged;
			}

			private void HandleCollectionChanged(object sender, CollectionChangedEventArgs e)
			{
				ReadOnlyObservableList<T> host = this.host.Target;

				if (host == null)
				{
					ObservableList<T> list = sender as ObservableList<T>;
					list.CollectionChanged -= this.HandleCollectionChanged;
				}
				else
				{
					host.HandleCollectionChanged (sender, e);
				}
			}

			private Weak<ReadOnlyObservableList<T>> host;
		}

		#region INotifyCollectionChanged Members

		/// <summary>
		/// Occurs when the list changes, either by adding or removing items to
		/// the underlying writable list.
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

		private event Epsitec.Common.Support.EventHandler<CollectionChangedEventArgs> collectionChangedEvent;
		private ObservableList<T> list;
	}
}
