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
		}


		public ItemList ItemList
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

		public IItemDataRenderer ItemRenderer
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

		private void PaintContents(Graphics graphics, Rectangle clipRect)
		{
			double dx = this.Client.Width;
			double dy = this.Client.Height;

			foreach (var row in this.list.VisibleRows)
			{
				double y2 = dy - row.Offset;
				double y1 = y2 - row.Height;

				if ((clipRect.Bottom >= y2) ||
					(clipRect.Top <= y1))
				{
					continue;
				}

				graphics.AddFilledRectangle (0, y1, dx, y2-y1);
				graphics.RenderSolid (Color.FromBrightness ((row.Index & 1) == 0 ? 1.0 : 0.9));

				var data = this.list.Cache.GetItemData (row.Index);

				this.ItemRenderer.Render (data, graphics, new Rectangle (0, y1, dx, y2-y1));
			}
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
	}
}
