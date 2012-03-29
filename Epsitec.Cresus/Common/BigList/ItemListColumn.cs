//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListColumn
	{
		public ItemListColumn()
		{
			this.layout = new ColumnLayoutInfo (new ColumnDefinition ());
		}
		
		
		public ColumnLayoutInfo					Layout
		{
			get
			{
				return this.layout;
			}
		}

		public FormattedText					Title
		{
			get;
			set;
		}

		public Caption							Caption
		{
			get;
			set;
		}

		public ItemSortOrder					SortOrder
		{
			get
			{
				return this.sortOrder;
			}
			set
			{
				if (this.sortOrder != value)
				{
					this.sortOrder = value;
					this.OnSortOrderChanged ();
				}
			}
		}

		public int								SortIndex
		{
			get
			{
				return this.sortIndex;
			}
			set
			{
				if (this.sortIndex != value)
				{
					this.sortIndex = value;
					this.OnSortIndexChanged ();
				}
			}
		}

		public bool								CanSort
		{
			get;
			set;
		}

		public int								Index
		{
			get;
			set;
		}

		
		public bool Contains(Point pos)
		{
			var x1 = this.layout.Definition.ActualOffset;
			var x2 = this.layout.Definition.ActualWidth + x1;

			x1 -= this.layout.Definition.LeftBorder;
			x2 += this.layout.Definition.RightBorder;

			if ((pos.X >= x1) &&
				(pos.X < x2))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public ItemSortOrder GetActiveSortOrder(bool toggle)
		{
			if (this.CanSort)
			{
				if (toggle)
				{
					switch (this.SortOrder)
					{
						case ItemSortOrder.None:
						case ItemSortOrder.Descending:
							return ItemSortOrder.Ascending;

						case ItemSortOrder.Ascending:
							return ItemSortOrder.Descending;
					}
				}
				else
				{
					switch (this.SortOrder)
					{
						case ItemSortOrder.None:
						case ItemSortOrder.Ascending:
							return ItemSortOrder.Ascending;

						case ItemSortOrder.Descending:
							return ItemSortOrder.Descending;
					}
				}
			}

			return ItemSortOrder.None;
		}



		private void OnSortOrderChanged()
		{
			this.SortOrderChanged.Raise (this);
		}

		private void OnSortIndexChanged()
		{
			this.SortIndexChanged.Raise (this);
		}


		public event EventHandler				SortOrderChanged;
		public event EventHandler				SortIndexChanged;
		
		private readonly ColumnLayoutInfo		layout;
		private ItemSortOrder					sortOrder;
		private int								sortIndex;
	}
}