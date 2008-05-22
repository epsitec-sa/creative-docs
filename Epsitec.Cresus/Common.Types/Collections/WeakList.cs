//	Copyright � 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>WeakList</c> stores its items using weak references, yet gives
	/// access to the contents almost like <c>List&lt;T&gt;</c>.
	/// </summary>
	/// <typeparam name="T">The manipulated data type.</typeparam>
	public class WeakList<T> : ICollection<T>, System.Collections.ICollection, System.Collections.IList where T : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WeakList&lt;T&gt;"/> class
		/// with a null list.
		/// </summary>
		public WeakList()
		{
			this.list = new List<Weak<T>> ();
		}

		/// <summary>
		/// Converts the list of weak references to an array of references.
		/// </summary>
		/// <returns>An array of <c>T</c>.</returns>
		public T[] ToArray()
		{
			return this.GetList ().ToArray ();
		}

		/// <summary>
		/// Converts the list of weak references to a list of references.
		/// </summary>
		/// <returns>A list of <c>T</c>.</returns>
		public List<T> ToList()
		{
			return this.GetList ();
		}

		/// <summary>
		/// Finds the first occurrence matching the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The first element or a default value.</returns>
		public T FindFirst(System.Predicate<T> predicate)
		{
			foreach (T item in this)
			{
				if (predicate (item))
				{
					return item;
				}
			}

			return default (T);
		}

		/// <summary>
		/// Find all occurrences matching the specified predicate.
		/// </summary>
		/// <param name="predicate">The predicate.</param>
		/// <returns>The elements.</returns>
		public IEnumerable<T> FindAll(System.Predicate<T> predicate)
		{
			foreach (T item in this)
			{
				if (predicate (item))
				{
					yield return item;
				}
			}
		}

		#region ICollection<T> Members

		public void Add(T item)
		{
			List<T> real = this.GetList ();
			real.Add (item);
			this.UpdateWeakList (real);
		}

		public void Clear()
		{
			this.list.Clear ();
		}

		public bool Contains(T item)
		{
			bool trim = false;

			foreach (Weak<T> weak in this.list)
			{
				if (weak.Target == item)
				{
					return true;
				}
				else if (!trim)
				{
					if (!weak.IsAlive)
					{
						trim = true;
					}
				}
			}

			if (trim)
			{
				this.TrimList ();
			}
			
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			List<T> real = this.GetList ();
			real.CopyTo (array, arrayIndex);
		}

		public int Count
		{
			get
			{
				return this.GetList ().Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			List<T> real = this.GetList ();

			if (real.Remove (item))
			{
				this.UpdateWeakList (real);
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
			return this.GetList ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetList ().GetEnumerator ();
		}

		#endregion

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			System.Collections.ICollection collection = this.GetList ();

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
				return this;
			}
		}

		#endregion

		#region IList Members

		int System.Collections.IList.Add(object value)
		{
			this.Add ((T) value);
			return this.list.Count-1;
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
			throw new System.NotImplementedException ();
		}

		void System.Collections.IList.Insert(int index, object value)
		{
			throw new System.NotImplementedException ();
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
				return false;
			}
		}

		void System.Collections.IList.Remove(object value)
		{
			this.Remove ((T) value);
		}

		void System.Collections.IList.RemoveAt(int index)
		{
			throw new System.NotImplementedException ();
		}

		object System.Collections.IList.this[int index]
		{
			get
			{
				throw new System.NotImplementedException ();
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		#endregion

		private List<T> GetList()
		{
			List<T> real = new List<T> ();

			foreach (Weak<T> item in this.list)
			{
				T target = item.Target;

				if (target != null)
				{
					real.Add (target);
				}
			}

			if (this.list.Count != real.Count)
			{
				this.UpdateWeakList (real);
			}

			return real;
		}

		private void UpdateWeakList(List<T> list)
		{
			this.list.Clear ();

			foreach (T target in list)
			{
				this.list.Add (new Weak<T> (target));
			}
		}

		private void TrimList()
		{
			this.GetList ();
		}

		private List<Weak<T>> list;
	}
}
