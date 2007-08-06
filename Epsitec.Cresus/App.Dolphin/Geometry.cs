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
