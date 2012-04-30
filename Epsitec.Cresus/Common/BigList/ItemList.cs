//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public interface IItemList
	{
		ItemListFeatures Features
		{
			get;
		}

		IList<ItemListMark> Marks
		{
			get;
		}

		int Count
		{
			get;
		}

		int ActiveIndex
		{
			get;
			set;
		}

		int FocusedIndex
		{
			get;
			set;
		}

		int SelectedItemCount
		{
			get;
		}

		ItemCache Cache
		{
			get;
		}
	}

	public abstract class ItemList : IItemList
	{
		protected ItemList(List<ItemListMark> marks = null, ItemListFeatures features = null)
		{
			this.visibleRows = new List<ItemListRow> ();
			this.marks       = marks ?? new List<ItemListMark> ();
			this.features    = features ?? new ItemListFeatures ()
			{
				SelectionMode = ItemSelectionMode.ExactlyOne,
			};
		}


		public ItemListFeatures					Features
		{
			get
			{
				return this.features;
			}
		}

		public IList<ItemListMark>				Marks
		{
			get
			{
				return this.marks;
			}
		}

		public abstract int						Count
		{
			get;
		}

		public int								ActiveIndex
		{
			get
			{
				return this.activeIndex;
			}
			set
			{
				if (value == -1)
				{
					this.ClearActiveIndex ();
				}
				else
				{
					this.SetActiveIndex (value);
				}
			}
		}

		public int								FocusedIndex
		{
			get
			{
				return this.focusedIndex;
			}
			set
			{
				if (value == -1)
				{
					this.ClearFocusedIndex ();
				}
				else
				{
					this.SetFocusedIndex (value);
				}
			}
		}

		public int								VisibleIndex
		{
			get
			{
				return this.visibleIndex;
			}
			set
			{
				var count = this.Count;

				if ((value >= count) &&
					(value > 0))
				{
					value = count-1;
				}

				if (this.visibleIndex != value)
				{
					this.SetVisibleIndex (value);
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

		public IList<ItemListRow>				VisibleRows
		{
			get
			{
				return this.visibleRows.AsReadOnly ();
			}
		}

		public int								SelectedItemCount
		{
			get
			{
				return this.selectedItemCount;
			}
		}

		public abstract ItemCache				Cache
		{
			get;
		}


		private bool SetVisibleIndex(int index)
		{
			var oldVisibleIndex  = this.visibleIndex;
			var oldVisibleOffset = this.visibleOffset;
			
			this.InternalSetVisibleIndex (index);

			return oldVisibleIndex  != this.visibleIndex
				|| oldVisibleOffset != this.visibleOffset;
		}

		private bool SetActiveIndex(int index)
		{
			if ((index < 0) ||
				(index >= this.Count))
			{
				return false;
			}

			var oldActiveIndex = this.activeIndex;
			
			this.activeIndex = index;

			return oldActiveIndex != this.activeIndex;
		}

		private bool ClearActiveIndex()
		{
			if (this.activeIndex == -1)
			{
				return false;
			}

			this.activeIndex = -1;
			return true;
		}

		private bool SetFocusedIndex(int index)
		{
			if ((index < 0) ||
				(index >= this.Count))
			{
				return false;
			}

			var oldFocusedIndex = this.focusedIndex;

			this.focusedIndex = index;

			return oldFocusedIndex != this.focusedIndex;
		}

		private bool ClearFocusedIndex()
		{
			if (this.focusedIndex == -1)
			{
				return false;
			}

			this.focusedIndex = -1;
			return true;
		}

		public void Reset()
		{
			this.ResetCache ();

			this.ClearActiveIndex ();
			this.ClearFocusedIndex ();

			this.SetVisibleIndex (0);
		}

		public bool Select(int index, ItemSelection selection)
		{
			if ((index < 0) ||
				(index >= this.Count))
			{
				return false;
			}

			if (selection == ItemSelection.Toggle)
			{
				selection = this.IsSelected (index) ? ItemSelection.Deselect : ItemSelection.Select;
			}
			
			if (selection == ItemSelection.Deselect)
			{
				switch (this.features.SelectionMode)
				{
					case ItemSelectionMode.ExactlyOne:
					case ItemSelectionMode.None:
						return false;

					case ItemSelectionMode.Multiple:
					case ItemSelectionMode.ZeroOrOne:
						return this.DeselectOne (index);

					case ItemSelectionMode.OneOrMore:
						if (this.selectedItemCount < 2)
						{
							return false;
						}
						else
						{
							return this.DeselectOne (index);
						}
				}
			}

			if (selection == ItemSelection.Select)
			{
				switch (this.features.SelectionMode)
				{
					case ItemSelectionMode.ZeroOrOne:
					case ItemSelectionMode.ExactlyOne:
						if (this.SelectOne (index))
						{
							if (this.selectedItemCount > 1)
							{
								this.DeselectAllButOne (index);
							}
							return true;
						}
						return false;

					case ItemSelectionMode.OneOrMore:
					case ItemSelectionMode.Multiple:
						return this.SelectOne (index);

					case ItemSelectionMode.None:
						return false;
				}
			}

			throw new System.InvalidOperationException (string.Format ("Select does not understand {0}", selection.GetQualifiedName ()));
		}

		public bool IsSelected(int index)
		{
			if (index < 0)
			{
				return false;
			}
			else
			{
				return this.Cache.GetItemState (index, ItemStateDetails.Flags).Selected;
			}
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
				rows = this.GetVisibleRowsEndingWith (this.Count-1);
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
		}

		public int GetFirstFullyVisibleIndex()
		{
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

		public ItemState GetItemState(int index)
		{
			return this.Cache.GetItemState (index, ItemStateDetails.Full);
		}

		public ItemHeight GetItemHeight(int index)
		{
			return this.Cache.GetItemHeight (index);
		}

		public void SetItemState(int index, ItemState state)
		{
			this.Cache.SetItemState (index, state, ItemStateDetails.Full);
		}

		public void SetItemHeight(int index, int height)
		{
			var state = this.GetItemState (index);
			state.Height = height;
			this.SetItemState (index, state);
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

		
		
		protected void ResetCache()
		{
			this.Cache.Reset ();
		}



		private bool DeselectAllButOne(int index)
		{
			int changes = 0;

			for (int i = 0; i < this.Count; i++)
			{
				if (i == index)
				{
					continue;
				}

				if (this.ChangeFlagState (i, x => x.Select (ItemSelection.Deselect), ItemStateDetails.IgnoreNull))
				{
					this.selectedItemCount--;
					changes++;
				}
			}

			return changes > 0;
		}

		private bool SelectOne(int index)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Select)))
			{
				this.selectedItemCount++;
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool DeselectOne(int index)
		{
			if (this.ChangeFlagState (index, x => x.Select (ItemSelection.Deselect)))
			{
				this.selectedItemCount--;
				return true;
			}
			else
			{
				return false;
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

		private bool GetFlagState(int index, System.Predicate<ItemState> predicate)
		{
			return predicate (this.Cache.GetItemState (index, ItemStateDetails.Flags));
		}

		private bool ChangeFlagState(int index, System.Action<ItemState> action, ItemStateDetails flags = ItemStateDetails.None)
		{
			flags &= ItemStateDetails.FlagMask;

			var state = this.Cache.GetItemState (index, ItemStateDetails.Flags | flags);

			if (state == null)
			{
				return false;
			}
			
			var copy  = state.Clone ();

			action (copy);

			if (state.Equals (copy))
			{
				return false;
			}

			this.Cache.SetItemState (index, copy, ItemStateDetails.Flags);

			return true;
		}

		private List<ItemListRow> GetVisibleRowsStartingWith(int index, int startOffset = 0)
		{
			var rows  = new List<ItemListRow> ();
			int count = this.Count;

			if (count == 0)
			{
				return rows;
			}

			while ((startOffset > 0) && (index > 0))
			{
				startOffset -= this.GetItemHeight (--index).TotalHeight;
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

				var height = this.GetItemHeight (index);
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
			var rows  = new List<ItemListRow> ();
			int count = this.Count;

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
				var height = this.GetItemHeight (index);
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

				rows = new List<ItemListRow> (ItemList.ShiftRows (rows, total - this.visibleHeight));

				var last = rows.Last ();

				offset = total;
				index  = last.Index + 1;

				//	...and fill the list with as many items as possible.

				while ((total < this.visibleHeight) && (index < count))
				{
					var height = this.GetItemHeight (index);
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


		private static IEnumerable<ItemListRow> ShiftRows(IEnumerable<ItemListRow> rows, int offset)
		{
			return rows.Select (x => new ItemListRow (x.Index, x.Offset + offset, x.Height, x.IsLast));
		}


		protected readonly ItemListFeatures		features;

		private List<ItemListRow>				visibleRows;
		private readonly List<ItemListMark>		marks;
		private int								activeIndex;
		private int								focusedIndex;
		private int								visibleIndex;
		private int								visibleOffset;
		private int								visibleHeight;
		private int								selectedItemCount;
	}
}