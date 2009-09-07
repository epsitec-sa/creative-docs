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
			this.scale = 1.0;
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

		public double Scale
		{
			get
			{
				return this.scale;
			}
			set
			{
				if (this.scale != value)
				{
					this.scale = value;
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
				var transform = graphics.Transform;
				graphics.ScaleTransform (this.scale, this.scale, 0, 0);
				Rectangle paint = Rectangle.Deflate (this.Client.Bounds, this.Padding);
				this.renderer.Render (graphics, Rectangle.Scale (paint, 1/this.scale));
				graphics.Transform = transform;
			}
		}


		private Renderers.AbstractRenderer renderer;
		private double scale;
	}
}
