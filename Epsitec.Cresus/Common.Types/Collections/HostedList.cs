//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The HostedList class implements a list of T with notifications to the
	/// host when the contents changes (insertion and removal of items).
	/// </summary>
	/// <typeparam name="T">Type of items stored in list</typeparam>
	public class HostedList<T> : IList<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="host">The host which must be notified.</param>
		public HostedList(IListHost<T> host)
		{
			this.host = host;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedList&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="insertionCallback">The insertion callback.</param>
		/// <param name="removalCallback">The removal callback.</param>
		public HostedList(Callback insertionCallback, Callback removalCallback)
		{
			this.host = new CallbackRelay (insertionCallback, removalCallback);
		}

		/// <summary>
		/// Adds the collection of items to the list.
		/// </summary>
		/// <param name="collection">Items to add</param>
		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add (item);
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

		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			this.NotifyBeforeInsertion (item);
			this.list.Insert (index, item);
			this.NotifyInsertion (item);
		}


		public void RemoveAt(int index)
		{
			T item = this.list[index];
			this.list.RemoveAt (index);
			this.NotifyRemoval (item);
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				if (EqualityComparer<T>.Default.Equals (this.list[index], value) == false)
				{
					this.NotifyBeforeInsertion (value);
					this.NotifyRemoval (this.list[index]);
					this.list[index] = value;
					this.NotifyInsertion (value);
				}
			}
		}

		#endregion
		
		#region ICollection<T> Members

		public void Add(T item)
		{
			this.NotifyBeforeInsertion (item);
			this.list.Add (item);
			this.NotifyInsertion (item);
		}

		public void Clear()
		{
			this.list.Clear ();
			
			T[] array = this.list.ToArray ();

			for (int i = array.Length-1; i >= 0; i--)
			{
				this.NotifyRemoval (array[i]);
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

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(T item)
		{
			if (this.list.Contains (item))
			{
				this.list.Remove (item);
				this.NotifyRemoval (item);
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
		
		public delegate void Callback(T item);

		#region CallbackRelay Class

		private class CallbackRelay : IListHost<T>
		{
			public CallbackRelay(Callback insertionCallback, Callback removalCallback)
			{
				this.insertionCallback = insertionCallback;
				this.removalCallback = removalCallback;
			}
			
			#region IListHost<T> Members

			public HostedList<T> Items
			{
				get
				{
					throw new System.Exception ("The method or operation is not implemented.");
				}
			}
			
			public void NotifyListInsertion(T item)
			{
				this.insertionCallback (item);
			}

			public void NotifyListRemoval(T item)
			{
				this.removalCallback (item);
			}

			#endregion

			private Callback insertionCallback;
			private Callback removalCallback;
		}

		#endregion

		protected virtual void NotifyBeforeInsertion(T item)
		{
		}

		protected virtual void NotifyInsertion(T item)
		{
			this.host.NotifyListInsertion (item);
		}
		
		protected virtual void NotifyRemoval(T item)
		{
			this.host.NotifyListRemoval (item);
		}

		private IListHost<T> host;
		private List<T> list = new List<T> ();
	}
}
