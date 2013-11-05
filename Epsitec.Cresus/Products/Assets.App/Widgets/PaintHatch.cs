//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class PaintHatch
	{
		public static void Paint(Graphics graphics, Rectangle rect)
		{
			//	Dessine des hachures à 45 degrés dans un rectangle. On s'arrange pour
			//	qu'elles soient forcément jointives avec celle du rectangle suivant.
			double d = rect.Height/3.0;  // distance entre 2 hachures

			for (double y=d; y<=rect.Height; y+=d)
			{
				var start = new Point (rect.Left, rect.Top-y);
				var end = PaintHatch.GetHatchEnd (start, rect);
				graphics.AddLine (start, end);
			}

			for (double x=d; x<=rect.Width; x+=d)
			{
				var start = new Point (rect.Left+x, rect.Bottom);
				var end = PaintHatch.GetHatchEnd (start, rect);
				graphics.AddLine (start, end);
			}

			graphics.RenderSolid (Color.FromAlphaColor (0.5, ColorManager.GlyphColor));
		}

		private static Point GetHatchEnd(Point start, Rectangle rect)
		{
			//	Calcule le point d'arrivée de la jointure.
			double x = start.X + rect.Width;
			double y = start.Y + rect.Width;  // juste, mais trop loin

			//	Effectue un clipping dans le rectangle.
			if (x > rect.Right)
			{
				y -= x - rect.Right + 0.5;
				x -= x - rect.Right + 0.5;
			}

			if (y > rect.Top)
			{
				x -= y - rect.Top + 0.5;
				y -= y - rect.Top + 0.5;
			}

			return new Point (x, y);
		}
	}
}
