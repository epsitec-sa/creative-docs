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
			this.items = new List<Data.ChartSeries> ();
		}


		public IList<Data.ChartSeries> Items
		{
			get
			{
				return this.items;
			}
		}


		public void DefineRenderer(Renderers.AbstractRenderer renderer)
		{
			this.renderer = renderer;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Color.FromBrightness (1));

			if (this.renderer != null)
			{
				Rectangle graphBounds = Rectangle.Deflate (this.Client.Bounds, new Margins (40, 160, 25, 40));
				Rectangle captionsBounds = new Rectangle (graphBounds.Right + 10, graphBounds.Bottom, this.Client.Bounds.Right - graphBounds.Right - 20, graphBounds.Height);
				
				this.renderer.Render (this.items, graphics, graphBounds);
				this.renderer.RenderCaptions (graphics, captionsBounds);
			}
		}


		private Renderers.AbstractRenderer renderer;
		private readonly List<Data.ChartSeries> items;
	}
}
