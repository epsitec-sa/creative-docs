//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityCollection</c> class is used to store and represent lists
	/// of data for collection fields in a parent entity.
	/// </summary>
	/// <typeparam name="T">The type of the list items.</typeparam>
	public sealed class EntityCollection<T> : ObservableList<object>, IList<T>, System.Collections.IList, IEntityCollection, INotifyCollectionChangedProvider where T : AbstractEntity
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityCollection&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="containerFieldId">The container field id.</param>
		/// <param name="container">The container entity.</param>
		/// <param name="copyOnWrite">If set to <c>true</c>, the list will be set into "copy on write" mode.</param>
		public EntityCollection(string containerFieldId, AbstractEntity container, bool copyOnWrite)
		{
			this.containerFieldId = containerFieldId;
			this.container = container;
			this.state = copyOnWrite ? State.CopyOnWrite : State.Default;
		}

		/// <summary>
		/// Gets the observable list on which to apply modifications.
		/// If in "copy on write" mode, this will return a writable copy of 
		/// the list instead of the list itself.
		/// </summary>
		/// <returns>The list to use.</returns>
		protected override ObservableList<object> GetWorkingList()
		{
			if (this.container.IsDefiningOriginalValues)
			{
				//	The container is in the special "original value definition mode"
				//	and we need not to worry about copy on write.

				return this;
			}
			else
			{
				//	If this list is in copy on write mode, then we ask the container
				//	to generate a copy, which will be stored in the modified data
				//	version of the container field :

				switch (this.state)
				{
					case State.CopyOnWrite:
						this.state = State.Copied;
						return this.container.CopyFieldCollection<T> (this.containerFieldId, this);

					case State.Copied:
						throw new System.InvalidOperationException ("Copied collection may not be changed");

					case State.Default:
						return this;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		/// <summary>
		/// Called when the collection is changing.
		/// </summary>
		protected override void OnCollectionChanging()
		{
			base.OnCollectionChanging ();

			this.container.UpdateDataGeneration ();
		}

		#region IList<T> Members

		int IList<T>.IndexOf(T item)
		{
			return this.IndexOf (item);
		}

		void IList<T>.Insert(int index, T item)
		{
			this.Insert (index, item);
		}

		void IList<T>.RemoveAt(int index)
		{
			this.RemoveAt (index);
		}

		T IList<T>.this[int index]
		{
			get
			{
				return this.Promote (index, this[index]);
			}
			set
			{
				this[index] = value;
			}
		}

		#endregion

		private T Promote(int index, object item)
		{
			T promotedItem = item as T;

			if (promotedItem != null)
			{
				return promotedItem;
			}

			IEntityProxy proxy = item as IEntityProxy;

			if (proxy != null)
			{
				promotedItem = proxy.PromoteToRealInstance () as T;

				if (promotedItem != null)
				{
					this.PatchItem (index, promotedItem);

					return promotedItem;
				}
			}

			throw new System.NotImplementedException ();
		}

		#region ICollection<T> Members

		void ICollection<T>.Add(T item)
		{
			this.Add (item);
		}

		void ICollection<T>.Clear()
		{
			this.Clear ();
		}

		bool ICollection<T>.Contains(T item)
		{
			return this.Contains (item);
		}

		void ICollection<T>.CopyTo(T[] array, int arrayIndex)
		{
			object[] items = this.ToArray ();

			for (int i = 0; i < items.Length; i++)
			{
				array[arrayIndex+i] = this.Promote (i, items[i]);
			}
		}

		int ICollection<T>.Count
		{
			get
			{
				return this.Count;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		bool ICollection<T>.Remove(T item)
		{
			return this.Remove (item);
		}

		#endregion

		#region IEnumerable<T> Members

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			int index = 0;
			foreach (T item in this)
			{
				yield return this.Promote (index++, item);
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			this.Add (value);
			return this.Count-1;
		}

		void System.Collections.IList.Clear()
		{
			this.Clear ();
		}

		bool System.Collections.IList.Contains(object value)
		{
			return this.Contains (value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf (value);
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			this.Insert (index, value);
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				return this.IsReadOnly;
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
			this.Remove (value);
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			this.RemoveAt (index);
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				return this.Promote (index, this[index]);
			}
			set
			{
				this[index] = value;
			}
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int arrayIndex)
		{
			object[] items = this.ToArray ();

			for (int i = 0; i < items.Length; i++)
			{
				array.SetValue (this.Promote (i, items[i]), arrayIndex+i);
			}
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
				return this.SyncRoot;
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			int index = 0;
			foreach (T item in this)
			{
				yield return this.Promote (index++, item);
			}
		}

		#endregion

		#region IEntityCollection Members

		/// <summary>
		/// Resets the collection to the unchanged copy on write state.
		/// </summary>
		public void ResetCopyOnWrite()
		{
			this.state = State.CopyOnWrite;
		}

		/// <summary>
		/// Copies the collection to a writable instance if the collection is
		/// still in the unchanged copy on write state.
		/// </summary>
		void IEntityCollection.CopyOnWrite()
		{
			if (this.state == State.CopyOnWrite)
			{
				this.state = State.Copied;
				this.container.CopyFieldCollection<T> (this.containerFieldId, this);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this collection will create a copy
		/// before being modified.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the collection has the copy on write state; otherwise, <c>false</c>.
		/// </value>
		bool IEntityCollection.HasCopyOnWriteState
		{
			get
			{
				return this.state == State.CopyOnWrite;
			}
		}

		System.Type IEntityCollection.GetItemType()
		{
			return typeof (T);
		}
		
		#endregion

		#region INotifyCollectionChangedProvider Members

		public INotifyCollectionChanged GetNotifyCollectionChangedSource()
		{
			return this;
		}

		#endregion

		#region Private State Enumeration

		private enum State
		{
			Default,
			CopyOnWrite,
			Copied
		}

		#endregion

		private readonly string containerFieldId;
		private readonly AbstractEntity container;
		private State state;
	}
}
