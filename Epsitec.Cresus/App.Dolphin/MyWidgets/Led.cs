//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule une diode LED rouge.
	/// </summary>
	public class Led : Widget
	{
		public Led() : base()
		{
		}

		public Led(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);
			double dim = System.Math.Min(23, System.Math.Min(rect.Width, rect.Height));
			rect = new Rectangle(rect.Center.X-dim/2, rect.Center.Y-dim/2, dim, dim);

			double radiusExt = dim/2;
			double radiusInt = radiusExt*0.75;

			Rectangle rectInt = new Rectangle(rect.Center.X-radiusInt, rect.Center.Y-radiusInt, radiusInt*2, radiusInt*2);

			if (this.ActiveState == ActiveState.Yes)
			{
				//	Dessine le tout.
				graphics.AddFilledCircle(rect.Center, radiusExt);
				Geometry.RenderVerticalGradient(graphics, rect, Color.FromRgb(0.4, 0.2, 0.2), Color.FromRgb(1.0, 0.8, 0.8));

				//	Dessine l'intérieur allumé.
				graphics.AddFilledCircle(rect.Center, radiusInt);
				graphics.RenderSolid(DolphinApplication.ColorHilite);

				//	Dessine le reflet.
				Point rc = new Point(rect.Center.X-3, rect.Center.Y+3);
				graphics.AddFilledCircle(rc, 5);
				Geometry.RenderCircularGradient(graphics, rc, 5, Color.FromAlphaRgb(0, 1.0, 1.0, 1.0), DolphinApplication.FromBrightness(1.0));
			}
			else
			{
				//	Dessine le tout.
				graphics.AddFilledCircle(rect.Center, radiusExt);
				Geometry.RenderVerticalGradient(graphics, rect, DolphinApplication.FromBrightness(0.6), DolphinApplication.FromBrightness(1.0));

				//	Dessine l'intérieur éteint.
				graphics.AddFilledCircle(rect.Center, radiusInt);
				Geometry.RenderVerticalGradient(graphics, rectInt, DolphinApplication.FromBrightness(0.4), DolphinApplication.FromBrightness(0.7));

				//	Dessine le reflet.
				Point rc = new Point(rect.Center.X-3, rect.Center.Y+3);
				graphics.AddFilledCircle(rc, 7);
				Geometry.RenderCircularGradient(graphics, rc, 7, Color.FromAlphaRgb(0, 1.0, 1.0, 1.0), DolphinApplication.FromBrightness(1.0));
			}

			//	Dessine les cercles.
			graphics.AddCircle(rect.Center, radiusExt);
			graphics.AddCircle(rect.Center, radiusInt);
			graphics.RenderSolid(DolphinApplication.FromBrightness(0));
		}

	}
}
