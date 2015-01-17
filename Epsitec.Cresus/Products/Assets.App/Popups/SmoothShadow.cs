//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public static class SmoothShadow
	{
		public static void Paint(Graphics graphics, Rectangle rect)
		{
			//	Dessine une jolie ombre douce.
			//	L'ombre est plus discrète si le rectangle est petit.

			double factor = rect.Height / 200;  // plus discrète si petit

			factor = System.Math.Min (factor, 1.0);
			factor = System.Math.Max (factor, 0.5);

			for (double step=0.0; step<=1.0; step+=0.02)  // 0..1
			{
				var r = rect;
				r.Offset (0, -20*factor);
				r.Inflate (-20*factor + step*80.0*factor);

				var path = SmoothShadow.GetPathRoundRectangle (r, (10.0 + step*80.0)*factor);
				var alpha = System.Math.Pow (1.0-step, 2.0) * 0.04;

				graphics.AddFilledPath (path);
				graphics.RenderSolid (Color.FromAlphaRgb (alpha, 0.0, 0.0, 0.0));
			}
		}

		private static Path GetPathRoundRectangle(Rectangle rect, double radius)
		{
			//	Crée le chemin d'un rectangle à coins arrondis.
			double ox = rect.Left;
			double oy = rect.Bottom;
			double dx = rect.Width;
			double dy = rect.Height;

			radius = System.Math.Max (radius, 0.1);

			var path = new Path ();
			path.MoveTo (ox+radius+0.5, oy+0.5);
			path.LineTo (ox+dx-radius-0.5, oy+0.5);
			path.CurveTo (ox+dx-0.5, oy+0.5, ox+dx-0.5, oy+radius+0.5);
			path.LineTo (ox+dx-0.5, oy+dy-radius-0.5);
			path.CurveTo (ox+dx-0.5, oy+dy-0.5, ox+dx-radius-0.5, oy+dy-0.5);
			path.LineTo (ox+radius+0.5, oy+dy-0.5);
			path.CurveTo (ox+0.5, oy+dy-0.5, ox+0.5, oy+dy-radius-0.5);
			path.LineTo (ox+0.5, oy+radius+0.5);
			path.CurveTo (ox+0.5, oy+0.5, ox+radius+0.5, oy+0.5);
			path.Close ();

			return path;
		}
	}
}