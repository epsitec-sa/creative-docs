//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class PaintEventGlyph
	{
		public static void Paint(Graphics graphics, Rectangle rect, TimelineGlyph glyph)
		{
			//	Dessine le glyph d'un événement.
			Rectangle r;
			Path path;

			switch (glyph)
			{
				case TimelineGlyph.FilledCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.28);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledSquare:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedSquare:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.25);
					r.Deflate (0.5);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.PinnedDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);

					path = PaintEventGlyph.GetPinnedPath (r);
					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDown:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDown:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledUp:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedUp:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledStar:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetStarPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedStar:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetStarPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;
			}
		}

		private static Rectangle GetGlyphSquare(Rectangle rect, double factor)
		{
			int d = (int) (rect.Height * factor);
			int x = (int) (rect.Center.X - d);
			int y = (int) (rect.Center.Y - d);

			return new Rectangle (x, y, d*2, d*2);
		}

		private static Path GetDownPath(Rectangle rect)
		{
			var path = new Path ();

			path.MoveTo (rect.TopLeft);
			path.LineTo (rect.TopRight);
			path.LineTo (rect.Center.X, rect.Bottom);
			path.Close ();

			return path;
		}

		private static Path GetUpPath(Rectangle rect)
		{
			var path = new Path ();

			path.MoveTo (rect.BottomLeft);
			path.LineTo (rect.BottomRight);
			path.LineTo (rect.Center.X, rect.Top);
			path.Close ();

			return path;
		}

		private static Path GetDiamondPath(Rectangle rect)
		{
			var path = new Path ();

			path.MoveTo (rect.Center.X, rect.Top);
			path.LineTo (rect.Right, rect.Center.Y);
			path.LineTo (rect.Center.X, rect.Bottom);
			path.LineTo (rect.Left, rect.Center.Y);
			path.Close ();

			return path;
		}

		private static Path GetStarPath(Rectangle rect)
		{
			var path = new Path ();

			var c = rect.Center;
			var e = new Point (c.X, c.Y+rect.Width*0.7);
			var f = new Point (c.X, c.Y+rect.Width*0.3);
			const int branch = 5*2;

			for (int i=0; i<branch; i++)
			{
				var p = Transform.RotatePointDeg (c, 360.0*i/branch, (i%2 == 0) ? e : f);

				if (i == 0)
				{
					path.MoveTo (p);
				}
				else
				{
					path.LineTo (p);
				}
			}

			path.Close ();

			return path;
		}

		private static Path GetPinnedPath(Rectangle rect)
		{
			var path = new Path ();

			path.AppendCircle (rect.Center, rect.Width*0.15);

			return path;
		}
	}
}
