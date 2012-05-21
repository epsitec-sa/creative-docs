//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListVisibleFrame
	{
		internal ItemListVisibleFrame(ItemList itemList)
		{
			this.itemList = itemList;
			this.visibleRows = new List<ItemListRow> ();
		}


		public int								VisibleIndex
		{
			get
			{
				return this.visibleIndex;
			}
			set
			{
				var count = this.itemList.ItemCount;

				if ((value >= count) &&
					(value > 0))
				{
					value = count-1;
				}

				if (this.visibleIndex != value)
				{
					this.SetVisibleIndex (value);
					this.OnVisibleContentChanged ();
				}
			}
		}

		public int								VisibleOffset
		{
			get
			{
				return this.visibleOffset;
			}
		}

		public int								VisibleHeight
		{
			get
			{
				return this.visibleHeight;
			}
			set
			{
				if (this.visibleHeight != value)
				{
					this.visibleHeight = value;
					this.MoveVisibleContent (0);
					this.OnVisibleContentChanged ();
				}
			}
		}

		public int								VisibleCount
		{
			get
			{
				return this.visibleRows.Count;
			}
		}

		public int								FullyVisibleCount
		{
			get
			{
				var count = this.VisibleCount;

				if (count > 1)
				{
					var last = this.visibleRows[count-1];
					var end  = last.Offset + last.Height.TotalHeight;

					if (end > this.visibleHeight)
					{
						count--;
					}

					var first = this.visibleRows[0];

					if (first.Offset < 0)
					{
						count--;
					}
				}

				return count;
			}
		}


		public IList<ItemListRow>				VisibleRows
		{
			get
			{
				return this.visibleRows.AsReadOnly ();
			}
		}


		public void Reset()
		{
			this.visibleRows.Clear ();

			this.visibleOffset = 0;
			this.visibleIndex  = -1;
		}


		public void SyncVisibleIndex(int index)
		{
			if ((this.visibleRows.Count > 0) &&
				(this.visibleRows[0].Index == index))
			{
				return;
			}

			this.visibleRows.Clear ();
			this.visibleIndex = this.itemList.ItemCount;

			this.SetVisibleIndex (index);
			this.OnVisibleContentChanged ();
		}

		public void MoveVisibleContent(int distance)
		{
			List<ItemListRow> rows;

			if (distance == System.Int32.MaxValue)
			{
				rows = this.GetVisibleRowsStartingWith (0);
			}
			else if (distance == System.Int32.MinValue)
			{
				rows = this.GetVisibleRowsEndingWith (this.itemList.ItemCount-1);
			}
			else
			{
				rows = this.GetVisibleRowsStartingWith (this.visibleIndex, this.visibleOffset + distance);
			}
			
			var row  = rows.FirstOrDefault ();

			if (row == null)
			{
				this.visibleIndex  = 0;
				this.visibleOffset = 0;
				this.visibleRows   = rows;
			}
			else
			{
				this.visibleIndex  = row.Index;
				this.visibleOffset = row.Offset;
				this.visibleRows   = rows;
			}

			this.OnVisibleContentChanged ();
		}

		public int GetFirstFullyVisibleIndex()
		{
			this.RefreshIfNeeded ();

			var row = this.visibleRows.FirstOrDefault (x => x.Offset >= 0)
				   ?? this.visibleRows.FirstOrDefault ();

			if (row == null)
			{
				return -1;
			}
			else
			{
				return row.Index;
			}
		}

		public int GetLastFullyVisibleIndex()
		{
			this.RefreshIfNeeded ();

			var row = this.visibleRows.LastOrDefault (x => (x.Offset + x.Height.TotalHeight) <= this.visibleHeight)
				   ?? this.visibleRows.LastOrDefault ();

			if (row == null)
			{
				return -1;
			}
			else
			{
				return row.Index;
			}
		}

		public ItemListMarkOffset GetOffset(ItemListMark mark)
		{
			if (mark == null)
			{
				return ItemListMarkOffset.Empty;
			}

			var shift = mark.Attachment == ItemListMarkAttachment.After;
			var index = mark.Index;

		again:

			var row = this.visibleRows.FirstOrDefault (x => x.Index == index);

			if (row == null)
			{
				if (shift)
				{
					index += 1;
					shift  = false;
					goto again;
				}

				if (index < this.visibleIndex)
				{
					return ItemListMarkOffset.Before;
				}
				if (index >= this.visibleIndex + this.VisibleCount)
				{
					return ItemListMarkOffset.After;
				}

				return ItemListMarkOffset.Empty;
			}

			int offset;

			if (shift)
			{
				offset = row.Offset + row.Height.TotalHeight;
			}
			else
			{
				offset = row.Offset;
			}

			if (offset < -mark.Breadth)
			{
				return ItemListMarkOffset.Before;
			}
			if (offset > this.visibleHeight + mark.Breadth)
			{
				return ItemListMarkOffset.After;
			}

			return new ItemListMarkOffset (offset);
		}


		internal bool SetVisibleIndex(int index)
		{
			var oldVisibleIndex  = this.visibleIndex;
			var oldVisibleOffset = this.visibleOffset;
			
			this.InternalSetVisibleIndex (index);

			return oldVisibleIndex  != this.visibleIndex
				|| oldVisibleOffset != this.visibleOffset;
		}


		private void RefreshIfNeeded()
		{
			if (this.visibleIndex < 0)
			{
				this.visibleIndex = 0;
				this.InternalSetVisibleIndex (0);
			}
		}

		private void InternalSetVisibleIndex(int index)
		{
			if (this.visibleHeight == 0)
			{
				this.visibleIndex  = 0;
				this.visibleOffset = 0;
				this.visibleRows   = new List<ItemListRow> ();

				return;
			}


			var rows = this.visibleRows;
			var row  = rows.Find (x => x.Index == index);

			if (row == null)
			{
				if (index > this.visibleIndex)
				{
					rows = this.GetVisibleRowsEndingWith (index);
					row  = rows.Find (x => x.Index == index);
				}
				else
				{
					rows = this.GetVisibleRowsStartingWith (index);
					row  = rows.Find (x => x.Index == index);
				}
			}

			int offset = 0;

			if (row != null)
			{
				if (row.Offset < 0)
				{
					rows = this.GetVisibleRowsStartingWith (index);
					row  = rows.Find (x => x.Index == index);
				}
				else if (row.Offset + row.Height.TotalHeight > this.visibleHeight)
				{
					rows = this.GetVisibleRowsEndingWith (index);
					row  = rows.Find (x => x.Index == index);
				}

				offset = row.Offset;
			}

			this.visibleRows   = rows;
			this.visibleIndex  = index;
			this.visibleOffset = offset;
		}

		private List<ItemListRow> GetVisibleRowsStartingWith(int index, int startOffset = 0)
		{
			this.RefreshIfNeeded ();

			var rows  = new List<ItemListRow> ();
			int count = this.itemList.ItemCount;

			if (count == 0)
			{
				return rows;
			}

			while ((startOffset > 0) && (index > 0))
			{
				startOffset -= this.itemList.GetItemHeight (--index).TotalHeight;
			}

			if (index < 0)
			{
				index = 0;
				startOffset = 0;
			}
			if (index >= count)
			{
				index = count-1;
			}

			int start  = index;
			int offset = startOffset < 0 ? startOffset : 0;

			//	Assign each row an offset and a height, until we fill all the available space.

			while (offset < this.visibleHeight)
			{
				if (index >= count)
				{
					//	We could not fill the available space.
#if false
					if (start == 0)
					{
						//	We already started at the beginning of the collection, stop here;
						//	this will result in an incomplete list of rows. We can't do better.

						break;
					}
#endif

					//	Since we did not start at the beginning of the collection, try to
					//	generate a list with the last item of the collection aligned with the
					//	end of the available space:

					return this.GetVisibleRowsEndingWith (count-1);
				}

				var height = this.itemList.GetItemHeight (index);
				var total  = height.TotalHeight;

				if ((total > 0) && (offset + total > 0))
				{
					rows.Add (new ItemListRow (index, offset, height, index == count-1));
				}

				offset += total;
				index  += 1;
			}

			return rows;
		}

		private List<ItemListRow> GetVisibleRowsEndingWith(int index)
		{
			this.RefreshIfNeeded ();

			var rows  = new List<ItemListRow> ();
			int count = this.itemList.ItemCount;

			if (count == 0)
			{
				return rows;
			}
			if (index < 0)
			{
				index = 0;
			}
			if (index >= count)
			{
				index = count-1;
			}

			int offset = this.visibleHeight;
			int total  = 0;

			//	Assign each row an offset and a height, until we fill all the available space.

			while ((index >= 0) && (offset > 0))
			{
				var height = this.itemList.GetItemHeight (index);
				var localH = height.TotalHeight;

				offset -= localH;

				if ((offset <= 0) &&
					(rows.Count == 0))
				{
					//	Special case: there is just one item, which does not fit into the
					//	available space. Position it so that the end of the item will be
					//	visible.

					rows.Add (new ItemListRow (index, 0, height, index == count-1));
					return rows;
				}

				if (localH > 0)
				{
					rows.Insert (0, new ItemListRow (index, offset, height, index == count-1));
				}

				total += localH;
				index -= 1;
			}

			if ((total < this.visibleHeight) &&
				(rows.Count > 0))
			{
				//	There were not enough items to fill the available space. Shift the rows
				//	so that the first is aligned with the starting offset...

				rows = new List<ItemListRow> (ItemListVisibleFrame.ShiftRows (rows, total - this.visibleHeight));

				var last = rows.Last ();

				offset = total;
				index  = last.Index + 1;

				//	...and fill the list with as many items as possible.

				while ((offset < this.visibleHeight) && (index < count))
				{
					var height = this.itemList.GetItemHeight (index);
					var localH = height.TotalHeight;

					if (localH > 0)
					{
						rows.Add (new ItemListRow (index, offset, height, index == count-1));
					}

					offset += localH;
					index  += 1;
				}
			}

			return rows;
		}


		private void OnVisibleContentChanged()
		{
			var handler = this.VisibleContentChanged;
			handler.Raise (this);
		}

		
		private static IEnumerable<ItemListRow> ShiftRows(IEnumerable<ItemListRow> rows, int offset)
		{
			return rows.Select (x => new ItemListRow (x.Index, x.Offset + offset, x.Height, x.IsLast));
		}


		public event EventHandler				VisibleContentChanged;

		private readonly ItemList				itemList;

		private List<ItemListRow>				visibleRows;
		private int								visibleIndex;
		private int								visibleOffset;
		private int								visibleHeight;
	}
}