//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	public abstract class GenericList<T> : IList<T>, INotifyCollectionChanged
	{
		protected GenericList()
		{
		}
		
		internal abstract void EnsureThatNobodyDerivesTheGenericListClass();
		
		#region IList<T> Members

		public int IndexOf(T item)
		{
			return this.list.IndexOf (item);
		}

		public void Insert(int index, T item)
		{
			this.list.Insert (index, item);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, item));
		}

		public void RemoveAt(int index)
		{
			T value = this.list[index];
			this.list.RemoveAt (index);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, value));
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
				
				this.list[index] = value;
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Replace, index, value, index, oldValue));
			}
		}

		#endregion

		#region ICollection<T> Members

		public void Add(T item)
		{
			int index = this.list.Count;
			this.list.Add (item);
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Add, index, item));
		}

		public void Clear()
		{
			this.list.Clear ();
			this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Reset));
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

		public bool IsReadOnly
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
				this.list.RemoveAt (index);
				this.OnCollectionChanged (new CollectionChangedEventArgs (CollectionChangedAction.Remove, -1, null, index, item));
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
