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
			this.Padding = new Margins (60, 30, 30, 30);
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
				this.renderer.Render (this.renderer.SeriesItems, graphics, Rectangle.Deflate (this.Client.Bounds, this.Padding));
			}
		}


		private Renderers.AbstractRenderer renderer;
	}
}
