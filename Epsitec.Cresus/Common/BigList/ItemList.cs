//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public abstract class ItemList
	{
		protected ItemList()
		{
		}

		
		public ItemSelectionMode SelectionMode
		{
			get;
			set;
		}

		public int ActiveIndex
		{
			get
			{
				return this.activeIndex;
			}
			set
			{
				if (this.activeIndex != value)
				{
					this.SetActiveIndex (value);
				}
			}
		}

		public int VisibleIndex
		{
			get
			{
				return this.visibleIndex;
			}
			set
			{
				if (this.visibleIndex != value)
				{
					this.SetVisibleIndex (value);
				}
			}
		}

		public int VisibleOffset
		{
			get
			{
				return this.visibleOffset;
			}
		}
		
		
		public void Select(int index, bool isSelected)
		{
		}

		public void ScrollRelative(int offset)
		{
		}


		protected abstract int GetItemHeight(int index);


		private void SetActiveIndex(int index)
		{
			this.activeIndex = index;
		}

		private void SetVisibleIndex(int index)
		{
			this.visibleIndex = index;
		}

		
		private int activeIndex;
		private int visibleIndex;
		private int visibleOffset;
	}
}
