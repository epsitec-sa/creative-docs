//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Internal
{
	/// <summary>
	/// The <c>CollectionViewGroupItems</c> class is an internal class used by
	/// <see cref="CollectionViewGroup"/> to iterate over its items, pretending
	/// that all items are flat, even when they are stored within multiple
	/// subgroups.
	/// </summary>
	internal class CollectionViewGroupItems : IList<object>, System.Collections.IList
	{
		public CollectionViewGroupItems(CollectionViewGroup host)
		{
			this.host = host;
		}

		#region IList<object> Members

		public int IndexOf(object item)
		{
			int index = 0;
			
			foreach (object x in this.EnumerateItems ())
			{
				if (item == x)
				{
					return index;
				}
				index++;
			}
			
			return -1;
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
				CollectionViewGroup group = this.host;

				if ((index < group.ItemCount) &&
					(CollectionViewGroupItems.FindItemGroup (ref group, ref index)))
				{
					return group.GetItems ()[index];
				}
				else
				{
					throw new System.ArgumentOutOfRangeException ("index");
				}
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
			foreach (object x in this.EnumerateItems ())
			{
				if (x == item)
				{
					return true;
				}
			}
			
			return false;
		}

		public void CopyTo(object[] array, int arrayIndex)
		{
			foreach (object x in this.EnumerateItems ())
			{
				array[arrayIndex++] = x;
			}
		}

		public int Count
		{
			get
			{
				return this.host.ItemCount;
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
			return this.Contains (value);
		}

		int System.Collections.IList.IndexOf(object value)
		{
			return this.IndexOf (value);
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

		#region ICollection Members

		void System.Collections.ICollection.CopyTo(System.Array array, int index)
		{
			throw new System.InvalidOperationException ();
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
				return null;
			}
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

		private static bool FindItemGroup(ref CollectionViewGroup group, ref int index)
		{
			while (true)
			{
				if (index < group.ItemCount)
				{
					if (group.HasSubgroups)
					{
						Collections.ObservableList<CollectionViewGroup> subgroups = group.GetSubgroups ();
						group = subgroups[0];
					}
					else
					{
						return true;
					}
				}
				else
				{
					index -= group.ItemCount;

				moveUpOneGroup:

					CollectionViewGroup parent = group.ParentGroup;

					if (parent == null)
					{
						return false;
					}

					Collections.ObservableList<CollectionViewGroup> subgroups = parent.GetSubgroups ();
					int groupIndex = subgroups.IndexOf (group) + 1;

					System.Diagnostics.Debug.Assert (groupIndex > 0);
					System.Diagnostics.Debug.Assert (groupIndex < subgroups.Count+1);

					if (groupIndex == subgroups.Count)
					{
						group = parent;
						goto moveUpOneGroup;
					}
					else
					{
						group = subgroups[groupIndex];
					}
				}
			}
		}

		private CollectionViewGroup host;
	}
}
