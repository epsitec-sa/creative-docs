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
	internal sealed class ItemListController<T> : IEnumerable<T> where T : Widget
	{
		public ItemListController()
		{
			this.items = new List<T> ();
			this.originOffset = 0;
			this.overlapX = 4;
			this.overlapY = 4;
			this.Anchor = AnchorStyles.TopLeft;
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
					this.DisposeScroller ();
					this.InvalidateLayout ();
				}
			}
		}

		public T ActiveItem
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

		public T this[int index]
		{
			get
			{
				return this.items[index];
			}
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public AnchorStyles Anchor
		{
			get;
			set;
		}


		public void SetupUI(Widget container)
		{
			this.container = container;
			this.container.SizeChanged += (sender, e) => this.Layout ();
			this.Layout ();
		}

		public void UpdateLayout()
		{
			this.Layout ();
		}
		
		public void Add(T item)
		{
			Epsitec.Common.Widgets.Layouts.LayoutEngine.SetIgnoreMeasure (item, true);

			item.Anchor = this.Anchor;
			item.Parent = this.container;
			item.Index  = this.items.Count;
			item.Clicked += (sender, e) => this.ActiveItem = item;

			this.items.Add (item);
			this.InvalidateLayout ();
		}

		public bool Remove(T item)
		{
			if (item == null)
			{
				return false;
			}

			int index = this.items.IndexOf (item);

			if (index < 0)
			{
				return false;
			}
			else
			{
				this.items.Remove (item);
				this.InvalidateLayout ();

				item.Dispose ();

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

		public T Find(System.Predicate<T> predicate)
		{
			return this.items.Find (predicate);
		}

		public int IndexOf(T item)
		{
			return this.items.IndexOf (item);
		}
		
		public void Clear()
		{
			this.items.ForEach (item => item.Dispose ());
			this.items.Clear ();
			this.activeItem = null;

			this.InvalidateLayout ();
		}

		public void DefineOffset(double offset)
		{
			if (this.manualOffset != offset)
			{
				this.manualOffset = offset;
				this.InvalidateLayout ();
			}
		}

		
		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator ();
		}

		#endregion


		private void Layout()
		{
			if (this.container != null)
			{
				switch (this.itemLayoutMode)
				{
					case ItemLayoutMode.Horizontal:
						this.LayoutHorizontal ();
						break;

					case ItemLayoutMode.Flow:
						this.LayoutFlow ();
						break;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		private void LayoutFlow()
		{
			if (this.scroller == null)
			{
				Margins padding = new Margins ();

				this.scroller = new VScroller ()
				{
					Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom,
					Margins = new Margins (0, padding.Right, padding.Top, padding.Bottom),
					Parent = this.container,
					IsInverted = true,
					MinValue = 0.0M,
					MaxValue = 1.0M,
					SmallChange = 76,
					LargeChange = 76,
				};

				this.scroller.ValueChanged +=
					delegate
					{
						this.originOffset = -this.scroller.DoubleValue;
						this.InvalidateLayout ();
					};
			}

			double availableWidth  = this.container.Client.Size.Width - this.container.Padding.Width - this.scroller.PreferredWidth;
			double availableHeight = this.container.Client.Size.Height - this.container.Padding.Height;

			if ((availableWidth == this.cachedSize.Width) &&
				(availableHeight == this.cachedSize.Height))
			{
				return;
			}
			else
			{
				this.cachedSize = new Size (availableWidth, availableHeight);
			}
			
			double posBeginX  = 0;
			double posBeginY  = 0;
			double lineHeight = 0;
			double posMaxY    = 0;

			foreach (var item in this.items)
			{
				double posEndX  = posBeginX + item.PreferredWidth;

				if (posEndX > availableWidth)
				{
					posBeginX  = 0;
					posEndX    = item.PreferredWidth;
					posBeginY += lineHeight;
					lineHeight = 0;
				}

				lineHeight = System.Math.Max (lineHeight, item.PreferredHeight - this.overlapY);

				double posEndY  = posBeginY + item.PreferredHeight - this.overlapY;
				var    margins  = item.Margins;

				posMaxY = System.Math.Max (posMaxY, posEndY);

				item.Margins = new Margins (posBeginX, margins.Right, posBeginY + this.originOffset, margins.Bottom);
				item.Visibility = (posEndY + this.originOffset > 0) && (posBeginY + this.originOffset < availableHeight);

				posBeginX = posEndX - this.overlapX;
			}

			if (posMaxY > availableHeight)
			{
				this.scroller.Enable = true;
				this.scroller.VisibleRangeRatio = (decimal) (availableHeight / posMaxY);
				this.scroller.MaxValue = (decimal) (posMaxY - availableHeight);
				this.scroller.SmallChange = (decimal) (this.items.First ().PreferredHeight - this.overlapY);
				this.scroller.LargeChange = (decimal) (availableHeight);
			}
			else
			{
				this.scroller.Enable = false;
			}
		}

		private void LayoutHorizontal()
		{
			double availableWidth = this.container.Client.Size.Width - this.container.Padding.Width;
			double totalWidth = this.items.Aggregate (0.0, this.ComputeWidth);
			double startOffset;
			double buttonOffset = 0;

			if (totalWidth > availableWidth)
			{
				//	No room for all items.

				this.CreateHorizontalScrollButtons ();

				this.scrollPlus.ZOrder = 1;
				this.scrollMinus.ZOrder = 0;
				
				this.scrollMinus.Visibility = true;
				this.scrollPlus.Visibility  = true;
				
				buttonOffset = this.scrollPlus.PreferredWidth;
				availableWidth -= 2 * buttonOffset;
			}
			else
			{
				if (this.scrollMinus != null)
				{
					this.scrollMinus.Visibility = false;
					this.scrollPlus.Visibility  = false;
				}
			}
			
			if (this.AdjustHorizontalOffset (availableWidth, totalWidth, out startOffset))
			{
				//	Nothing changed (same availableWidth, same startOffset)
				return;
			}
			
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

				item.Margins = new Margins (buttonOffset + startOffset + posBegin, 0, margins.Top, margins.Bottom);
				item.Visibility = (posEnd > 0) && (posBegin < availableWidth);

				posBegin = posEnd - this.overlapX;
			}
		}

		
		private void CreateHorizontalScrollButtons()
		{
			if (this.scrollMinus == null)
			{
				this.scrollMinus = new Button ()
				{
					Parent = this.container,
					Anchor = AnchorStyles.Left | AnchorStyles.TopAndBottom,
					PreferredWidth = 16,
					ButtonStyle = ButtonStyle.Flat,
					Margins = new Margins (-1, 0, 4, 4),
					AutoFocus = false,
					Text = "&lt;",
					AutoEngage = true,
					AutoRepeat = true,
				};

				this.scrollPlus = new Button ()
				{
					Parent = this.container,
					Anchor = AnchorStyles.Right | AnchorStyles.TopAndBottom,
					PreferredWidth = 16,
					ButtonStyle = ButtonStyle.Flat,
					Margins = new Margins (0, -1, 4, 4),
					AutoFocus = false,
					Text = "&gt;",
					AutoEngage = true,
					AutoRepeat = true,
				};

				this.scrollMinus.Engaged      += delegate
				{
					this.ScrollHorizontalLayout (-1);
				};
				
				this.scrollMinus.StillEngaged += delegate
				{
					this.ScrollHorizontalLayout (-1);
				};
				
				this.scrollPlus.Engaged       += delegate
				{
					this.ScrollHorizontalLayout (1);
				};
				
				this.scrollPlus.StillEngaged  += delegate
				{
					this.ScrollHorizontalLayout (1);
				};
			}
		}

		private void ScrollHorizontalLayout(int increment)
		{
			this.activeItem = null;

			var item = this.items.FirstOrDefault ();
			
			if (item != null)
			{
				this.originOffset = System.Math.Max (0, this.originOffset + (item.PreferredWidth - this.overlapX) * increment);
				this.InvalidateLayout ();
			}
		}

		/// <summary>
		/// Adjusts the horizontal starting offset based on the manual offset (i.e. where the
		/// elements should start) and on the available space.
		/// </summary>
		/// <param name="availableWidth">Available width.</param>
		/// <param name="totalWidth">Total width.</param>
		/// <param name="start">The start offset.</param>
		/// <returns><c>true</c> if neither the available width, not the start offset changed.</returns>
		private bool AdjustHorizontalOffset(double availableWidth, double totalWidth, out double start)
		{
			start = this.manualOffset;
			double end = this.manualOffset + totalWidth;

			if (end > availableWidth)
			{
				end = availableWidth;
				start = end - totalWidth;
			}
			if (start < 0)
			{
				start = 0;
			}

			availableWidth -= start;

			if (new Size (availableWidth, start) == this.cachedSize)
			{
				return true;
			}
			else
			{
				this.cachedSize = new Size (availableWidth, start);
				return false;
			}
		}

		private double ComputeWidth(double width, Widget item)
		{
			return width + item.PreferredWidth - (width == 0.0 ? 0.0 : this.overlapX);
		}

		private void DisposeScroller()
		{
			if (this.scroller != null)
			{
				this.scroller.Dispose ();
				this.scroller = null;
			}
		}


		private void InvalidateLayout()
		{
			this.cachedSize = Size.Empty;
			Epsitec.Common.Widgets.Application.QueueAsyncCallback (this.Layout);
		}


		private Widget container;
		private AbstractScroller scroller;
		private readonly List<T> items;
		private ItemLayoutMode itemLayoutMode;
		private T activeItem;
		private double originOffset;
		private double manualOffset;
		private double overlapX;
		private double overlapY;
		private Size cachedSize;
		private Button scrollMinus;
		private Button scrollPlus;
	}
}
