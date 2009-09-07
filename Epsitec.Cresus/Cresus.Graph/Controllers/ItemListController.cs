//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Renderers;
using Epsitec.Common.Graph.Widgets;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Data;

namespace Epsitec.Cresus.Graph.Controllers
{
	internal sealed class ItemListController
	{
		public ItemListController(Widget container)
		{
			this.container = container;
			this.items = new List<Widget> ();
			this.originOffset = 0;
			this.overlapX = 4;
			this.overlapY = 4;

			this.container.SizeChanged += (sender, e) => this.Layout ();
		}


		public ItemLayoutMode ItemLayoutMode
		{
			get
			{
				return this.itemLayoutMode;
			}
			set
			{
				if (this.itemLayoutMode != value)
				{
					this.itemLayoutMode = value;
					this.InvalidateLayout ();
				}
			}
		}

		public Widget ActiveItem
		{
			get
			{
				return this.activeItem;
			}
			set
			{
				if (this.activeItem != value)
				{
					this.activeItem = value;
					this.InvalidateLayout ();
				}
			}
		}


		public void Add(Widget item)
		{
			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (item, true);

			item.Anchor = (item.Anchor & AnchorStyles.TopAndBottom) | AnchorStyles.Left;
			item.Parent = this.container;
			item.Clicked += (sender, e) => this.ActiveItem = item;

			this.items.Add (item);
			this.InvalidateLayout ();
		}

		public bool Remove(Widget item)
		{
			int index = this.items.IndexOf (item);

			if (index < 0)
			{
				return false;
			}
			else
			{
				this.items.Remove (item);
				this.InvalidateLayout ();
				
				item.Parent = null;

				//	Removing the active item ? If so, select the next one or, if there
				//	are no following items any more, the previous one, until there are no
				//	more items.

				if (this.activeItem == item)
				{
					if (index >= this.items.Count)
					{
						index = this.items.Count-1;
					}
					if (index < 0)
					{
						this.ActiveItem = null;
					}
					else
					{
						this.ActiveItem = this.items[index];
					}
				}
				
				return true;
			}
		}


		public void Layout()
		{
			switch (this.itemLayoutMode)
			{
				case ItemLayoutMode.Horizontal:
					this.LayoutHorizontal ();
					break;
				
				default:
					throw new System.NotImplementedException ();
			}
		}

		private void LayoutHorizontal()
		{
			double totalWidth = this.items.Aggregate (0.0, this.ComputeWidth);
			double availableWidth = this.container.Client.Size.Width - this.container.Padding.Width;
			double activeBegin = 0;
			double activeEnd = 0;

			//	Make sure that we do not accumulate excess space on the right hand of the
			//	items :
			
			if (totalWidth < this.originOffset + availableWidth)
			{
				this.originOffset = System.Math.Max (0, totalWidth - availableWidth);
			}

			//	Make sure that the active item is fully visible :

			if (this.activeItem != null)
			{
				activeBegin = this.items.TakeWhile (item => item != this.activeItem).Aggregate (0.0, this.ComputeWidth);
				activeEnd   = activeBegin + this.activeItem.PreferredWidth - this.overlapX;

				if (activeEnd > this.originOffset + availableWidth)
				{
					this.originOffset = activeEnd - availableWidth;
				}
				if (activeBegin < this.originOffset)
				{
					this.originOffset = activeBegin;
				}
			}

			double posBegin = -this.originOffset;

			foreach (var item in this.items)
			{
				double posEnd   = posBegin + item.PreferredWidth;
				var    margins  = item.Margins;

				item.Margins = new Margins (posBegin, 0, margins.Top, margins.Bottom);
				item.Visibility = (posEnd > 0) && (posBegin < availableWidth);

				posBegin = posEnd - this.overlapX;
			}
		}

		private double ComputeWidth(double width, Widget item)
		{
			return width + item.PreferredWidth - (width == 0.0 ? 0.0 : this.overlapX);
		}


		private void InvalidateLayout()
		{
			
		}


		private readonly Widget container;
		private readonly List<Widget> items;
		private ItemLayoutMode itemLayoutMode;
		private Widget activeItem;
		private double originOffset;
		private double overlapX;
		private double overlapY;
	}
}
