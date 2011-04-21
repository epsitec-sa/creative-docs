using Epsitec.Common.Types.Exceptions;

using System.Collections;


namespace Epsitec.Common.Support.EntityEngine
{


	/// <summary>
	/// The <see cref="ThreadSafeReadOnlyListProxy"/> is an implementation of <see cref="IList"/>
	/// associated to an entity which will wrap calls to an underlying implementation of list. It
	/// will throw an exception when an attempt to modify the collection is done and it will acquire
	/// the global lock associated with the entity for any read operation on the list.
	/// </summary>
	internal sealed class ThreadSafeReadOnlyListProxy : IList
	{


		public ThreadSafeReadOnlyListProxy(AbstractEntity entity, IList list)
		{
			this.entity = entity;
			this.list = list;
		}
		

		#region IList Members


		public int Add(object value)
		{
			throw new ReadOnlyException ();
		}


		public void Clear()
		{
			throw new ReadOnlyException ();
		}


		public bool Contains(object value)
		{
			using (this.entity.LockWrite ())
			{
				return this.list.Contains (value);
			}
		}


		public int IndexOf(object value)
		{
			using (this.entity.LockWrite ())
			{
				return this.list.IndexOf (value);
			}
		}


		public void Insert(int index, object value)
		{
			throw new ReadOnlyException ();
		}


		public bool IsFixedSize
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list.IsFixedSize;
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


		public void Remove(object value)
		{
			throw new ReadOnlyException ();
		}


		public void RemoveAt(int index)
		{
			throw new ReadOnlyException ();
		}


		public object this[int index]
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


		#region ICollection Members

		public void CopyTo(System.Array array, int index)
		{
			using (this.entity.LockWrite ())
			{
				this.list.CopyTo (array, index);
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


		public bool IsSynchronized
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list.IsSynchronized;
				}
			}
		}


		public object SyncRoot
		{
			get
			{
				using (this.entity.LockWrite ())
				{
					return this.list.SyncRoot;
				}
			}
		}


		#endregion


		#region IEnumerable Members


		public IEnumerator GetEnumerator()
		{
			using (this.entity.LockWrite ())
			{
				return new ArrayList (this.list).GetEnumerator ();
			}
		}


		#endregion


		private readonly AbstractEntity entity;


		private readonly IList list;

	
	}


}
