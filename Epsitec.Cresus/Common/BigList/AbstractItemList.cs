//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	/// <summary>
	/// The <c>AbstractItemList</c> class is the lowest level abstract class for the item list
	/// classes, which implement <see cref="IItemList"/>.
	/// </summary>
	public abstract class AbstractItemList : IItemList
	{
		protected AbstractItemList(ItemCache cache, IList<ItemListMark> marks, ItemListSelection selection)
		{
			this.cache = cache;
			this.marks = marks;
			this.selection = selection;
		}


		#region IItemList Members

		public ItemListFeatures					Features
		{
			get
			{
				return this.cache.Features;
			}
		}

		public IList<ItemListMark>				Marks
		{
			get
			{
				return this.marks;
			}
		}

		public int								Count
		{
			get
			{
				return this.cache.ItemCount;
			}
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

		public ItemListSelection				Selection
		{
			get
			{
				return this.selection;
			}
		}

		public ItemCache						Cache
		{
			get
			{
				return this.cache;
			}
		}

		public abstract void Reset();

		#endregion


		protected virtual void SetActiveIndex(int index)
		{
			if ((index < 0) ||
					(index >= this.Count))
			{
				throw new System.ArgumentOutOfRangeException ("index", "Index out of bounds");
			}

			this.activeIndex = index;
		}

		protected virtual void ClearActiveIndex()
		{
			this.activeIndex = -1;
		}

		protected virtual void SetFocusedIndex(int index)
		{
			if ((index < 0) ||
					(index >= this.Count))
			{
				throw new System.ArgumentOutOfRangeException ("index", "Index out of bounds");
			}

			this.focusedIndex = index;
		}

		protected virtual void ClearFocusedIndex()
		{
			this.focusedIndex = -1;
		}

		
		private readonly ItemCache				cache;
		private readonly IList<ItemListMark>	marks;
		private readonly ItemListSelection		selection;
		private int								activeIndex;
		private int								focusedIndex;
	}
}
