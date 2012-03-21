//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListVerticalContentView : Widget
	{
		public ItemListVerticalContentView()
		{
			this.DefaultLineHeight = 20;
			this.processor = new ItemListVerticalContentViewEventProcessor (this);
		}


		public ItemList							ItemList
		{
			get
			{
				return this.list;
			}
			set
			{
				if (this.list != value)
				{
					this.list = value;
					this.UpdateListHeight ();
				}
			}
		}

		public IItemDataRenderer				ItemRenderer
		{
			get;
			set;
		}

		public int								DefaultLineHeight
		{
			get;
			set;
		}

		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			this.UpdateListHeight ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			this.PaintContents (graphics, clipRect);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (this.processor.ProcessMessage (message, pos))
			{
				message.Consumer = this;
			}
		}
		
		
		private void PaintContents(Graphics graphics, Rectangle clipRect)
		{
			foreach (var row in this.list.VisibleRows)
			{
				var bounds = this.GetRowBounds (row);

				if (bounds.IntersectsWith (clipRect) == false)
				{
					continue;
				}

				var color = Color.FromBrightness ((row.Index & 1) == 0 ? 1.0 : 0.9);

				if (this.list.IsSelected (row.Index))
				{
					color = Color.FromName ("Highlight");
				}

				graphics.AddFilledRectangle (bounds);
				graphics.RenderSolid (color);

				var data  = this.list.Cache.GetItemData (row.Index);
				var state = this.list.GetItemState (row.Index);

				this.ItemRenderer.Render (state, data, graphics, bounds);
			}
		}

		public Rectangle GetRowBounds(ItemListRow row)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;

			double y2 = dy - row.Offset;
			double y1 = y2 - row.Height;

			return new Rectangle (0, y1, dx, y2-y1);
		}

		private void UpdateListHeight()
		{
			if ((this.IsActualGeometryValid) &&
				(this.list != null))
			{
				this.list.VisibleHeight = (int) System.Math.Floor (this.Client.Height);
			}
		}
		
		private ItemList						list;
		private ItemListVerticalContentViewEventProcessor processor;
	}
}
