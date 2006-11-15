//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.UI
{
	public class PanelMask : Widgets.Widget
	{
		public PanelMask()
		{
		}

		public PanelMask(Widgets.Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
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

		
		public Drawing.Color MaskColor
		{
			get
			{
				return (Drawing.Color) this.GetValue (PanelMask.MaskColorProperty);
			}
			set
			{
				this.SetValue (PanelMask.MaskColorProperty, value);
			}
		}


		protected override void ProcessMessage(Widgets.Message message, Drawing.Point pos)
		{
			message.Consumer = this;
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
#if true
			graphics.AddFilledRectangle (this.Client.Bounds);
			graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.5, 1.0, 0.0, 0.0));
#else
			Drawing.Path pathA = new Drawing.Path (this.Client.Bounds);
			Drawing.Path pathB = new Drawing.Path (this.Aperture);
			Drawing.Path shape = Drawing.Path.Combine (pathA, pathB, Drawing.PathOperation.AMinusB);

			graphics.Rasterizer.AddSurface (shape);
			graphics.RenderSolid (Drawing.Color.FromAlphaRgb (0.5, 1.0, 0.0, 0.0));

			shape.Dispose ();
			pathA.Dispose ();
			pathB.Dispose ();
#endif
		}

		public static readonly DependencyProperty MaskColorProperty = DependencyProperty.Register ("MaskColor", typeof (Drawing.Color), typeof (PanelMask), new Widgets.Helpers.VisualPropertyMetadata (Drawing.Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8), Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));

		private Drawing.Rectangle aperture;
	}
}
