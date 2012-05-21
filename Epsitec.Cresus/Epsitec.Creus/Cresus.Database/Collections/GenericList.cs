//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.GenericList</c> class implements a generic list with named
	/// item access and insertion/removal events.
	/// </summary>
	public class GenericList<T> : ObservableList<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GenericList&lt;T&gt;"/> class.
		/// </summary>
		public GenericList()
		{
		}

		/// <summary>
		/// Gets the item with the specified name.
		/// </summary>
		/// <value>The item if it exists; otherwise, <c>null</c> or the default empty <c>T</c>
		/// if <c>T</c> is a value type.</value>
		public T this[string name]
		{
			get
			{
				int index = this.IndexOf (name);
				
				if (index < 0)
				{
					return default (T);
				}
				else
				{
					return this[index];
				}
			}
		}

		/// <summary>
		/// Removes the item with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Remove(string name)
		{
			int index = this.IndexOf (name);
			
			if (index != -1)
			{
				this.RemoveAt (index);
			}
		}

		/// <summary>
		/// Determines whether the list contains the item with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>
		/// 	<c>true</c> if the list contains the item with the specified name; otherwise, <c>false</c>.
		/// </returns>
		public bool Contains(string name)
		{
			return this.IndexOf (name) != -1;
		}

		/// <summary>
		/// Returns the index of the first item with the specified name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The index of the item or <c>-1</c>.</returns>
		public int IndexOf(string name)
		{
			return this.IndexOf (name, 0);
		}

		/// <summary>
		/// Try to get the value with the given name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		/// 	<c>true</c> if the list contains the item with the specified name; otherwise, <c>faluse</c>.
		/// </returns>
		public bool TryGetValue(string name, out T value)
		{
			int index = this.IndexOf (name);

			if (index < 0)
			{
				value = default (T);
				return false;
			}
			else
			{
				value = this[index];
				return true;
			}
		}

		/// <summary>
		/// Returns the index of the first item with the specified name, starting at
		/// the specified start index.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="start">The start index.</param>
		/// <returns>The index of the item or <c>-1</c>.</returns>
		public virtual int IndexOf(string name, int start)
		{
			return -1;
		}

		protected override void OnCollectionChanged(CollectionChangedEventArgs e)
		{
			base.OnCollectionChanged (e);

			switch (e.Action)
			{
				case CollectionChangedAction.Add:
					foreach (T item in e.NewItems)
					{
						this.NotifyInsertion (item);
					}
					break;

				case CollectionChangedAction.Move:
					break;

				case CollectionChangedAction.Remove:
					foreach (T item in e.OldItems)
					{
						this.NotifyRemoval (item);
					}
					break;

				case CollectionChangedAction.Replace:
					foreach (T item in e.OldItems)
					{
						this.NotifyRemoval (item);
					}
					foreach (T item in e.NewItems)
					{
						this.NotifyInsertion (item);
					}
					break;
				
				default:
					throw new System.NotImplementedException (string.Format ("Action.{0} not implemented", e.Action));
			}
		}

		private void NotifyInsertion(T item)
		{
			if (this.ItemInserted != null)
			{
				this.ItemInserted (this, new ValueEventArgs (item));
			}
		}

		private void NotifyRemoval(T item)
		{
			if (this.ItemRemoved != null)
			{
				this.ItemRemoved (this, new ValueEventArgs (item));
			}
		}

		public event EventHandler<ValueEventArgs> ItemInserted;
		public event EventHandler<ValueEventArgs> ItemRemoved;
	}
}
