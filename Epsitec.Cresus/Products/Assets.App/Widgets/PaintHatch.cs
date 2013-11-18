//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class PaintHatch
	{
		public static void Paint(Graphics graphics, Rectangle rect, Point reference, double alpha)
		{
			//	Dessine des hachures à 45 degrés "/" dans un rectangle.
			//	On s'arrange pour qu'elles soient forcément jointives avec celles
			//	d'autres rectangles.
			rect.Deflate (0.25);  // pour avoir des jointures parfaites
			var path = PaintHatch.GetHatchPath (rect, PaintHatch.distance, reference);

			graphics.AddPath (path);
			graphics.RenderSolid (Color.FromAlphaColor (alpha, ColorManager.GlyphColor));
		}

		//	Importé de Epsitec.Common.Designer.Misc :
		private static Path GetHatchPath(Rectangle rect, double distance, Point reference)
		{
			//	Retourne des hachures à 45 degrés remplissant sans déborder un rectangle.
			//	Une hachure passe toujours par le point de référence.
			Path path = new Path ();

			//	Déplace le point de référence sur le bord gauche du rectangle.
			reference.Y += rect.Left - reference.X;
			reference.X = rect.Left;
			double d = reference.Y - rect.Bottom;

			double v = System.Math.Ceiling (rect.Width/distance) * distance;
			v -= d % distance;

			for (double y=rect.Bottom-v; y<rect.Top; y+=distance)
			{
				double x1 = rect.Left;
				double y1 = y;
				double x2 = rect.Right;
				double y2 = y+rect.Width;

				if (y1 < rect.Bottom)
				{
					x1 += rect.Bottom-y1;
					y1 = rect.Bottom;
				}

				if (y2 > rect.Top)
				{
					x2 -= y2-rect.Top;
					y2 = rect.Top;
				}

				if (x1 < x2)
				{
					path.MoveTo (x1, y1);
					path.LineTo (x2, y2);
				}
			}

			return path;
		}

		private static readonly int distance = 7;
	}
}
