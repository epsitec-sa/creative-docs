using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Types.Internal
{
	internal class CollectionViewGroupItems : IList<object>
	{
		public CollectionViewGroupItems(CollectionViewGroup host)
		{
			this.host = host;
		}

		#region IList<object> Members

		public int IndexOf(object item)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		public void Insert(int index, object item)
		{
			throw new System.InvalidOperationException ();
		}

		public void RemoveAt(int index)
		{
			throw new System.InvalidOperationException ();
		}

		public object this[int index]
		{
			get
			{
				throw new Exception ("The method or operation is not implemented.");
			}
			set
			{
				throw new System.InvalidOperationException ();
			}
		}

		#endregion

		#region ICollection<object> Members

		public void Add(object item)
		{
			throw new System.InvalidOperationException ();
		}

		public void Clear()
		{
			throw new System.InvalidOperationException ();
		}

		public bool Contains(object item)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			throw new Exception ("The method or operation is not implemented.");
		}

		public int Count
		{
			get
			{
				throw new Exception ("The method or operation is not implemented.");
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public bool Remove(object item)
		{
			throw new System.InvalidOperationException ();
		}

		#endregion

		#region IEnumerable<object> Members

		public IEnumerator<object> GetEnumerator()
		{
			return this.EnumerateItems ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.EnumerateItems ().GetEnumerator ();
		}

		#endregion

		private IEnumerable<object> EnumerateItems()
		{
			if (this.host.ItemCount == 0)
			{
				return Collections.EmptyEnumerable<object>.Instance;
			}
			else if (this.host.HasSubgroups)
			{
				return this.EnumerateSubgroupItems ();
			}
			else
			{
				return this.host.GetItems ();
			}
		}

		private IEnumerable<object> EnumerateSubgroupItems()
		{
			System.Diagnostics.Debug.Assert (this.host.HasSubgroups);

			foreach (CollectionViewGroup group in this.host.GetSubgroups ())
			{
				if (group.ItemCount > 0)
				{
					foreach (object item in group.Items)
					{
						yield return item;
					}
				}
			}
		}

		private CollectionViewGroup host;
	}
}
