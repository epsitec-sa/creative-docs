using Epsitec.Common.Types.Exceptions;

using System.Collections;
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Support.EntityEngine
{


	/// <summary>
	/// The <see cref="ThreadSafeReadOnlyListProxy{T}"/> is an implementation of <see cref="IList{T}"/>
	/// associated to an entity which will wrap calls to an underlying implementation of list. It
	/// will throw an exception when an attempt to modify the collection is done and it will acquire
	/// the global lock associated with the entity for any read operation on the list.
	/// </summary>
	internal sealed class ThreadSafeReadOnlyListProxy<T> : IList<T> where T : AbstractEntity
	{


		public ThreadSafeReadOnlyListProxy(AbstractEntity entity, IList<T> list)
		{
			this.entity = entity;
			this.list = list;
		}
		

		#region IList<T> Members


		public int IndexOf(T item)
		{
			using (this.entity.LockWrite ())
			{
				return this.list.IndexOf (item);
			}
		}


		public void Insert(int index, T item)
		{
			throw new ReadOnlyException ();
		}


		public void RemoveAt(int index)
		{
			throw new ReadOnlyException ();
		}


		public T this[int index]
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list[index];
				}
			}
			set
			{
				throw new ReadOnlyException ();
			}
		}


		#endregion


		#region ICollection<T> Members


		public void Add(T item)
		{
			throw new ReadOnlyException ();
		}

		public void Clear()
		{
			throw new ReadOnlyException ();
		}


		public bool Contains(T item)
		{
			using (this.entity.LockWrite ())
			{
				return this.list.Contains (item);
			}
		}


		public void CopyTo(T[] array, int arrayIndex)
		{
			using (this.entity.LockWrite ())
			{
				this.list.CopyTo (array, arrayIndex);
			}
		}


		public int Count
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list.Count;
				}
			}
		}


		public bool IsReadOnly
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list.IsReadOnly;
				}
			}
		}


		public bool Remove(T item)
		{
			throw new ReadOnlyException ();
		}


		#endregion


		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			using (this.entity.LockWrite ())
			{
				return new List<T> (this.list).GetEnumerator ();
			}
		}


		#endregion


		#region IEnumerable Members


		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}


		#endregion


		private readonly AbstractEntity entity;


		private readonly IList<T> list;


	}


}
