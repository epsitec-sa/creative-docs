//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public static class Painter
	{
		public static void PaintLeftToRightGradient(Graphics graphics, Rectangle rect, Color leftColor, Color rightColor)
		{
			Transform ot = graphics.GradientRenderer.Transform;

			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetParameters (-100, 100);
			graphics.GradientRenderer.SetColors (leftColor, rightColor);

			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient ();  // dégradé de gauche à droite

			graphics.GradientRenderer.Transform = ot;
		}

		public static void PaintCircleGradient(Graphics graphics, Rectangle rect, Color borderColor, Color centerColor)
		{
			Transform ot = graphics.GradientRenderer.Transform;

			graphics.GradientRenderer.Fill = GradientFill.Circle;
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.SetColors (borderColor, centerColor);

			Transform t = Transform.Identity;
			Point center = rect.Center;
			t = t.Scale (rect.Width/100/2, rect.Height/100/2);
			t = t.Translate (center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient ();  // dégradé circulaire

			graphics.GradientRenderer.Transform = ot;
		}
	}
}
