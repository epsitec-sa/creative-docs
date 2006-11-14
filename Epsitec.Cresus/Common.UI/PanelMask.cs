//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	public class PanelMask : Widgets.Widget
	{
		public PanelMask()
		{
		}

		public PanelMask(Widgets.Widget embedder)
			: base (embedder)
		{
		}

		public Drawing.Rectangle Aperture
		{
			get
			{
				return this.aperture;
			}
			set
			{
				if (this.aperture != value)
				{
					this.Invalidate (Drawing.Rectangle.Union (this.aperture, value));
					this.aperture = value;
				}
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			Drawing.Path pathA = new Drawing.Path (this.Client.Bounds);
			Drawing.Path pathB = new Drawing.Path (this.Aperture);
			
			Drawing.Path shape = Drawing.Path.Combine (pathA, pathB, Drawing.PathOperation.AMinusB);

			graphics.Rasterizer.AddSurface (shape);
			graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8));

			shape.Dispose ();
			pathA.Dispose ();
			pathB.Dispose ();
		}
		
		private Drawing.Rectangle aperture;
	}
}
