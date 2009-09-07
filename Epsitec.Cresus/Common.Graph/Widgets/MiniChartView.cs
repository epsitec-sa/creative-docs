//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Widgets
{
	public class MiniChartView : ChartView
	{
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var renderer  = this.Renderer;
			var scale     = this.Scale;
			var rectangle = Rectangle.Deflate (this.Client.Bounds, this.Padding);
			var transform = graphics.Transform;
			int seriesNum = renderer == null ? 0 : renderer.SeriesItems.Count;

			if (seriesNum > 1)
			{
				graphics.RotateTransformDeg (3, rectangle.Center.X, rectangle.Center.Y);
				graphics.AddFilledRectangle (Rectangle.Inflate (rectangle, 1, 1));
				graphics.RenderSolid (Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8));
				graphics.AddFilledRectangle (rectangle);
				graphics.RenderSolid (Color.FromAlphaRgb (0.6, 0.8, 0.8, 0.8));
				graphics.AddFilledRectangle (Rectangle.Deflate (rectangle, 1, 1));
				graphics.RenderSolid (Color.FromBrightness (1.0));

				if (seriesNum > 2)
				{
					graphics.RotateTransformDeg (-3-4, rectangle.Center.X, rectangle.Center.Y);
					graphics.AddFilledRectangle (Rectangle.Inflate (rectangle, 1, 1));
					graphics.RenderSolid (Color.FromAlphaRgb (0.2, 0.8, 0.8, 0.8));
					graphics.AddFilledRectangle (rectangle);
					graphics.RenderSolid (Color.FromAlphaRgb (0.6, 0.8, 0.8, 0.8));
					graphics.AddFilledRectangle (Rectangle.Deflate (rectangle, 1, 1));
					graphics.RenderSolid (Color.FromBrightness (1.0));
				}

				graphics.Transform = transform;
			}

			graphics.AddFilledRectangle (rectangle);
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (Color.FromBrightness (1.0), Color.FromRgb (0.9, 0.9, 0.95));
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (rectangle.Width/100, rectangle.Height/100).Translate (rectangle.Height/2, rectangle.Width/2).RotateDeg (10, rectangle.Center);
			graphics.RenderGradient ();

			if ((renderer != null) &&
				(renderer.SeriesItems.Count > 0))
			{
				graphics.ScaleTransform (scale, scale, 0, 0);
				
				Rectangle paint = rectangle;
				
				paint = Rectangle.Deflate (paint, new Margins (6, 6, 6, 20));
				graphics.AddFilledRectangle (Rectangle.Scale (paint, 1/scale));
				graphics.RenderSolid (Color.FromAlphaRgb (1.0, 1.0, 1.0, 1.0));
				
				paint = Rectangle.Deflate (paint, new Margins (4.5, 9, 9, 5));
				renderer.Render (graphics, Rectangle.Scale (paint, 1/scale));
				
				graphics.Transform = transform;

				Font   font     = Font.GetFont ("Futura", "Condensed Medium");
				double fontSize = 11.0;

				graphics.Color = Color.FromBrightness (0.0);
				graphics.PaintText (rectangle.X, rectangle.Y, rectangle.Width, 20, renderer.SeriesItems.First ().Label, font, fontSize, ContentAlignment.MiddleCenter);
				graphics.RenderSolid ();
			}

			graphics.LineWidth = 1.0;
			graphics.LineJoin = JoinStyle.Miter;
			
			var label = new Rectangle (6, rectangle.Top - 18 - 6, 48, 18);

			graphics.RotateTransformDeg (5, label.Center.X, label.Center.Y);

			MiniChartView.PaintShadow (graphics, label);
			graphics.AddFilledRectangle (label);

			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors (Color.FromRgb (250/255.0, 255/255.0, 124/255.0), Color.FromRgb (255/255.0, 254/255.0, 173/255.0));
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.Transform = Transform.Identity.Scale (1.0, label.Height / 100.0).Translate (label.BottomLeft);
			graphics.RenderGradient ();

			graphics.Color = Color.FromBrightness (0.0);
			graphics.PaintText (label.X, label.Y, label.Width, label.Height, "2008", Font.GetFont ("Futura", "Condensed Medium"), 14.0, ContentAlignment.MiddleCenter);
			graphics.RenderSolid ();

			graphics.Transform = transform;

			graphics.AddRectangle (Rectangle.Deflate (rectangle, 0.5, 0.5));
			graphics.RenderSolid (Color.FromBrightness (0.8));

			if (seriesNum > 1)
			{
				var image = Epsitec.Common.Support.ImageProvider.Default.GetImage ("manifest:Epsitec.Common.Graph.Images.PaperClip.icon", Support.Resources.DefaultManager);
				graphics.PaintImage (image, new Rectangle (rectangle.X + 10, rectangle.Top - 34, 20, 40));
			}
		}


		private static void PaintShadow(Graphics graphics, Rectangle rect)
		{
			rect = Rectangle.Deflate (rect, new Margins (2, 0, 2, 0));
			double[] alpha = new double[] { 0.4, 0.3, 0.2 };

			for (int i = 0; i < alpha.Length; i++)
			{
				rect = Rectangle.Inflate (rect, 1, 1);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha[i], 0.8, 0.8, 0.8));
			}
		}
	}
}
