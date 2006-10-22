//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	internal class CollectionViewGroupItems : IList<object>
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
