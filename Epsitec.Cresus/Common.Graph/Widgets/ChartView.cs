//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Widgets
{
	public class ChartView : FrameBox
	{
		public ChartView()
		{
		}


		public Renderers.AbstractRenderer Renderer
		{
			get
			{
				return this.renderer;
			}
			set
			{
				if (this.renderer != value)
				{
					this.renderer = value;
					this.Invalidate ();
				}
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Color.FromBrightness (1));

			if (this.renderer != null)
			{
				Rectangle bounds = this.Client.Bounds;
				bounds = Rectangle.Deflate (bounds, new Margins (60, 30, 30, 30));
				this.renderer.Render (this.renderer.SeriesItems, graphics, bounds);
//-				Rectangle captionsBounds = new Rectangle (graphBounds.Right + 10, graphBounds.Bottom, this.Client.Bounds.Right - graphBounds.Right - 20, graphBounds.Height);
//-				this.renderer.RenderCaptions (graphics, captionsBounds);
			}
		}


		private Renderers.AbstractRenderer renderer;
	}
}
