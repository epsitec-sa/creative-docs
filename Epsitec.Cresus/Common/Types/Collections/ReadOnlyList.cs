//	Copyright © 2006-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>ReadOnlyList</c> represents a read-only wrapper around an instance
	/// implementing <see cref="T:System.Collections.Generic.IList"/>.
	/// </summary>
	/// <typeparam name="T">The manipulated data type.</typeparam>
	public class ReadOnlyList<T> : IList<T>, System.Collections.ICollection, System.Collections.IList
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyList&lt;T&gt;"/> class
		/// with a null list.
		/// </summary>
		public ReadOnlyList()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="list">The list which will be wrapped into a read only shell.</param>
		public ReadOnlyList(IList<T> list)
		{
			this.list = list;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="array">The array which will be wrapped into a read only shell.</param>
		public ReadOnlyList(T[] array)
		{
			this.list = array;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="collection">The collection which will be made available as a read-only list.</param>
		public ReadOnlyList(IEnumerable<T> collection)
		{
			this.list = new List<T> (collection);
		}

		/// <summary>
		/// Gets a value indicating whether the list is null.
		/// </summary>
		/// <value><c>true</c> if the list is null; otherwise, <c>false</c>.</value>
		public bool IsNull
		{
			get
			{
				return this.list == null;
			}
		}
		
		/// <summary>
		/// Converts the list to an array.
		/// </summary>
		/// <returns>An array of T</returns>
		public T[] ToArray()
		{
			return Collection.ToArray (this.list);
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
				return this.list == null ? 0 : this.list.Count;
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
			for (int i = 0; i < this.list.Count; i++)
			{
				array.SetValue (this.list[i], index+i);
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

		/// <summary>
		/// An empty read only list.
		/// </summary>
		public static readonly ReadOnlyList<T> Empty = new ReadOnlyList<T> (new List<T> ());

		private IList<T> list;
	}
}
