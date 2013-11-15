//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class EventGlyph
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
					r = EventGlyph.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedSquare:
					r = EventGlyph.GetGlyphSquare (rect, 0.25);
					r.Deflate (0.5);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDiamond:
					r = EventGlyph.GetGlyphSquare (rect, 0.35);
					path = EventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDiamond:
					r = EventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = EventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDown:
					r = EventGlyph.GetGlyphSquare (rect, 0.3);
					path = EventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDown:
					r = EventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = EventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledUp:
					r = EventGlyph.GetGlyphSquare (rect, 0.3);
					path = EventGlyph.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedUp:
					r = EventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = EventGlyph.GetUpPath (r);

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
	}
}
