//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class HostedList<T> : IList<T>
	{
		public HostedList(IListHost<T> host)
		{
			this.host = host;
		}
		public HostedList(Callback insertionCallback, Callback removalCallback)
		{
			this.host = new CallbackRelay (insertionCallback, removalCallback);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				this.Add (item);
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
			this.NotifyInsertion (item);
		}


		public void RemoveAt(int index)
		{
			T item = this.list[index];
			this.NotifyRemoval (item);
			this.list.RemoveAt (index);
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
					this.NotifyRemoval (this.list[index]);
					this.list[index] = value;
					this.NotifyInsertion (this.list[index]);
				}
			}
		}

		#endregion
		
		#region ICollection<T> Members

		public void Add(T item)
		{
			this.list.Add (item);
			this.NotifyInsertion (item);
		}

		public void Clear()
		{
			T[] array = this.list.ToArray ();

			for (int i = array.Length-1; i >= 0; i--)
			{
				this.NotifyRemoval (array[i]);
			}
			
			this.list.Clear ();
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
				this.NotifyRemoval (item);
				this.list.Remove (item);
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

		private void NotifyInsertion(T item)
		{
			this.host.NotifyListInsertion (item);
		}
		private void NotifyRemoval(T item)
		{
			this.host.NotifyListRemoval (item);
		}

		private IListHost<T> host;
		private List<T> list = new List<T> ();
	}
}
