//	Copyright © 2007-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityCollectionProxy</c> class is a proxy to an <see cref="EntityCollection"/>.
	/// It is returned by <see cref="AbstractEntity"/> when the requested collection
	/// type does not match the real one or if the real collection is still unchanged
	/// and in copy-on-write mode.
	/// </summary>
	/// <typeparam name="T">The type of the list items.</typeparam>
	public sealed class EntityCollectionProxy<T> : IList<T>, System.Collections.IList, IEntityCollection
		where T : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityCollectionProxy&lt;T&gt;"/> class.
		/// This constructor is used when the container defines a copy-on-write
		/// collection which might be replaced by a writable copy, yet the user
		/// should not be aware of the change.
		/// </summary>
		/// <param name="containerFieldId">The container field id.</param>
		/// <param name="container">The container.</param>
		public EntityCollectionProxy(string containerFieldId, AbstractEntity container)
		{
			this.containerFieldId = containerFieldId;
			this.container = container;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityCollectionProxy&lt;T&gt;"/> class.
		/// This constructor is used when the collection requested does not
		/// implement the requested <see cref="IList&lt;T&gt;"/> interface,
		/// yet the user should be able to access it through it.
		/// </summary>
		/// <param name="entityCollection">The entity collection.</param>
		public EntityCollectionProxy(System.Collections.IList entityCollection)
		{
			this.entityCollection = entityCollection;
		}

		/// <summary>
		/// Gets the real entity collection for which this proxy stands in.
		/// </summary>
		/// <value>The real entity collection.</value>
		private System.Collections.IList EntityCollection
		{
			get
			{
				return this.entityCollection ?? this.container.InternalGetValue (this.containerFieldId) as System.Collections.IList;
			}
		}
		
		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.EntityCollection.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			this.EntityCollection.Insert (index, item);
		}

		public void RemoveAt(int index)
		{
			this.EntityCollection.RemoveAt (index);
		}

		public T this[int index]
		{
			get
			{
				return this.EntityCollection[index] as T;
			}
			set
			{
				this.EntityCollection[index] = value;
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			this.EntityCollection.Add (item);
		}

		public void Clear()
		{
			this.EntityCollection.Clear ();
		}

		public bool Contains(T item)
		{
			return this.EntityCollection.Contains (item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			this.EntityCollection.CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.EntityCollection.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.EntityCollection.IsReadOnly;
			}
		}

		public bool Remove(T item)
		{
			System.Collections.IList list = this.EntityCollection;
			
			if (list.Contains (item))
			{
				list.Remove (item);
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
			foreach (T item in this.EntityCollection)
			{
				yield return item;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.EntityCollection.GetEnumerator ();
		}

		#endregion

		#region IList Members

		public int Add(object value)
		{
			return this.EntityCollection.Add (value);
		}

		public bool Contains(object value)
		{
			return this.EntityCollection.Contains (value);
		}

		public int IndexOf(object value)
		{
			return this.EntityCollection.IndexOf (value);
		}

		public void Insert(int index, object value)
		{
			this.EntityCollection.Insert (index, value);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public void Remove(object value)
		{
			this.EntityCollection.Remove (value);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.EntityCollection[index];
			}
			set
			{
				this.EntityCollection[index] = value;
			}
		}

		#endregion

		#region ICollection Members

		public void CopyTo(System.Array array, int index)
		{
			this.EntityCollection.CopyTo (array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return this.EntityCollection.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this.EntityCollection.SyncRoot;
			}
		}

		#endregion

		#region IEntityCollection Members

		/// <summary>
		/// Resets the collection to the unchanged copy on write state.
		/// </summary>
		public void ResetCopyOnWrite()
		{
			IEntityCollection collection = this.EntityCollection as IEntityCollection;

			if (collection != null)
			{
				collection.ResetCopyOnWrite ();
			}
		}

		/// <summary>
		/// Copies the collection to a writable instance if the collection is
		/// still in the unchanged copy on write state.
		/// </summary>
		public void CopyOnWrite()
		{
			IEntityCollection collection = this.EntityCollection as IEntityCollection;
			
			if (collection != null)
			{
				collection.CopyOnWrite ();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this collection will create a copy
		/// before being modified.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the collection has the copy on write state; otherwise, <c>false</c>.
		/// </value>
		public bool HasCopyOnWriteState
		{
			get
			{
				IEntityCollection collection = this.EntityCollection as IEntityCollection;

				if (collection != null)
				{
					return collection.HasCopyOnWriteState;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// Gets the type of the items stored in this collection.
		/// </summary>
		/// <returns>The type of the items.</returns>
		System.Type IEntityCollection.GetItemType()
		{
			return typeof (T);
		}


		public System.IDisposable DisableNotifications()
		{
			var collection = this.EntityCollection as EntityCollection;

			System.Diagnostics.Debug.Assert (collection != null);

			return collection.DisableNotifications ();
		}


		/// <summary>
		/// Temporarily disables all change notifications. Any changes which
		/// happen until <c>Dispose</c> is called on the returned object will
		/// not generate events.
		/// </summary>
		/// <returns>
		/// An object you will have to <c>Dispose</c> in order to re-enable
		/// the notifications.
		/// </returns>
		public System.IDisposable SuspendNotifications()
		{
			var collection = this.EntityCollection as EntityCollection;

			System.Diagnostics.Debug.Assert (collection != null);

			return collection.SuspendNotifications ();
		}

		#endregion

		#region INotifyCollectionChangedProvider Members

		/// <summary>
		/// Gets the <see cref="INotifyCollectionChanged"/> interface which can
		/// be used to get the <c>CollectionChanged</c> events for the source.
		/// </summary>
		/// <returns>
		/// The <see cref="INotifyCollectionChanged"/> interface.
		/// </returns>
		public INotifyCollectionChanged GetNotifyCollectionChangedSource()
		{
			INotifyCollectionChangedProvider provider = this.EntityCollection as INotifyCollectionChangedProvider;

			System.Diagnostics.Debug.Assert (provider != null);
			System.Diagnostics.Debug.Assert (provider.GetNotifyCollectionChangedSource () != null);
			
			return provider.GetNotifyCollectionChangedSource ();
		}

		#endregion

		private readonly string containerFieldId;
		private readonly AbstractEntity container;
		private readonly System.Collections.IList entityCollection;
	}
}
