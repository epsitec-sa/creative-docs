//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityCollectionProxy</c> 
	/// </summary>
	/// <typeparam name="T">The type of the list items.</typeparam>
	public sealed class EntityCollectionProxy<T> : IList<T>, System.Collections.IList, IEntityCollection where T : AbstractEntity
	{
		public EntityCollectionProxy(string containerFieldId, AbstractEntity container)
		{
			this.containerFieldId = containerFieldId;
			this.container = container;
		}

		public EntityCollectionProxy(System.Collections.IList entityCollection)
		{
			this.entityCollection = entityCollection;
		}

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

		public void ResetCopyOnWrite()
		{
			IEntityCollection collection = this.EntityCollection as IEntityCollection;

			if (collection != null)
			{
				collection.ResetCopyOnWrite ();
			}
		}

		public void CopyOnWrite()
		{
			IEntityCollection collection = this.EntityCollection as IEntityCollection;
			
			if (collection != null)
			{
				collection.CopyOnWrite ();
			}
		}

		public bool UsesCopyOnWriteBehavior
		{
			get
			{
				IEntityCollection collection = this.EntityCollection as IEntityCollection;

				if (collection != null)
				{
					return collection.UsesCopyOnWriteBehavior;
				}
				else
				{
					return false;
				}
			}
		}

		#endregion

		private readonly string containerFieldId;
		private readonly AbstractEntity container;
		private readonly System.Collections.IList entityCollection;
	}
}
