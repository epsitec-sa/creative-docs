//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;

namespace Epsitec.App.Dolphin
{
	/// <summary>
	/// La classe Geometry contient quelques routines générales.
	/// </summary>
	public class Geometry
	{
		public static void RenderHorizontalGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			//	Peint la surface avec un dégradé horizontal.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetColors(leftColor, rightColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor)
		{
			//	Peint la surface avec un dégradé vertical.
			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(bottomColor, topColor);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		public static void RenderVerticalGradient(Graphics graphics, Rectangle rect, Color bottomColor, Color topColor, int repeat, double middle)
		{
			//	Peint la surface avec un dégradé vertical.
			Color c1 = topColor;
			Color c2 = bottomColor;

			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];

			for ( int i=0 ; i<256 ; i++ )
			{
				double factor = Geometry.GradientRepeatFactor(i/255.0, repeat, middle);
				r[i] = c1.R + (c2.R-c1.R)*factor;
				g[i] = c1.G + (c2.G-c1.G)*factor;
				b[i] = c1.B + (c2.B-c1.B)*factor;
				a[i] = c1.A + (c2.A-c1.A)*factor;
			}

			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Y;
			graphics.GradientRenderer.SetColors(r, g, b, a);
			graphics.GradientRenderer.SetParameters(-100, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			Point center = rect.Center;
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}

		protected static double GradientRepeatFactor(double progress, int repeat, double middle)
		{
			//	Calcule le facteur de progression dans la couleur [0..1].
			//	Si M>0:  P=1-(1-P)^(1+M)
			//	Si M<0:  P=P^(1-M)
			if (repeat > 1)
			{
				int i = (int) (progress*repeat);
				progress = (progress*repeat)%1.0;
				if (i%2 != 0)  progress = 1.0-progress;
			}

			if (middle != 0)
			{
				if (middle > 0.0)
				{
					progress = 1.0-System.Math.Pow(1.0-progress, 1.0+middle);
				}
				else
				{
					progress = System.Math.Pow(progress, 1.0-middle);
				}
			}

			return progress;
		}

		public static void RenderCircularGradient(Graphics graphics, Point center, double radius, Color extColor, Color intColor)
		{
			//	Peint la surface avec un dégradé circulaire.
			Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);

			graphics.FillMode = FillMode.NonZero;
			graphics.GradientRenderer.Fill = GradientFill.Circle;
			graphics.GradientRenderer.SetColors(intColor, extColor);
			graphics.GradientRenderer.SetParameters(0, 100);
			
			Transform ot = graphics.GradientRenderer.Transform;
			Transform t = new Transform();
			t.Scale(rect.Width/100/2, rect.Height/100/2);
			t.Translate(center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient();
			graphics.GradientRenderer.Transform = ot;
		}
	}
}
