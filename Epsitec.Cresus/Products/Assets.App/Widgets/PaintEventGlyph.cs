//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public static class PaintEventGlyph
	{
		public static void Paint(Graphics graphics, Rectangle rect, TimelineGlyph glyph)
		{
			//	Dessine le glyph d'un événement.
			Rectangle r;
			Path path;

			switch (glyph.Shape)
			{
				case TimelineGlyphShape.FilledCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.28);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.FilledSquare:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedSquare:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.25);
					r.Deflate (0.5);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddRectangle (r);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.FilledDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.PlusDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));

					path = PaintEventGlyph.GetPlusPath (r);
					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.PinnedDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));

					path = PaintEventGlyph.GetPinnedPath (r);
					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedDiamond:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.FilledDown:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedDown:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.FilledUp:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedUp:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.FilledStar:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetStarPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.OutlinedStar:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = PaintEventGlyph.GetStarPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
					break;

				case TimelineGlyphShape.Locked:
					r = PaintEventGlyph.GetGlyphSquare (rect, 0.3);
					path = PaintEventGlyph.GetLockedPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (PaintEventGlyph.MainColor (glyph));
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
			//	Retourne le chemin d'un triangle pointé vers le bas.
			var path = new Path ();

			path.MoveTo (rect.TopLeft);
			path.LineTo (rect.TopRight);
			path.LineTo (rect.Center.X, rect.Bottom);
			path.Close ();

			return path;
		}

		private static Path GetUpPath(Rectangle rect)
		{
			//	Retourne le chemin d'un triangle pointé vers le haut.
			var path = new Path ();

			path.MoveTo (rect.BottomLeft);
			path.LineTo (rect.BottomRight);
			path.LineTo (rect.Center.X, rect.Top);
			path.Close ();

			return path;
		}

		private static Path GetDiamondPath(Rectangle rect)
		{
			//	Retourne le chemin d'un carré sur la pointe.
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
			//	Retourne le chemin d'une étoile à 5 branches.
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

		private static Path GetLockedPath(Rectangle rect)
		{
			//	Retourne le chemin d'un cadenas.
			var path = new Path ();

			//	Forme extérieure (CW).
			path.MoveTo  (PaintEventGlyph.GetPoint (rect, 1, 0));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 1, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 2, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 2, 7));
			path.CurveTo (PaintEventGlyph.GetPoint (rect, 2, 9), PaintEventGlyph.GetPoint (rect, 3, 10), PaintEventGlyph.GetPoint (rect, 5, 10));
			path.CurveTo (PaintEventGlyph.GetPoint (rect, 7, 10), PaintEventGlyph.GetPoint (rect, 8, 9), PaintEventGlyph.GetPoint (rect, 8, 7));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 8, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 9, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 9, 0));
			path.Close ();

			//	Trou de la boucle supérieure (CCW).
			path.MoveTo  (PaintEventGlyph.GetPoint (rect, 3, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 7, 6));
			path.LineTo  (PaintEventGlyph.GetPoint (rect, 7, 7));
			path.CurveTo (PaintEventGlyph.GetPoint (rect, 7, 8), PaintEventGlyph.GetPoint (rect, 6, 9), PaintEventGlyph.GetPoint (rect, 5, 9));
			path.CurveTo (PaintEventGlyph.GetPoint (rect, 4, 9), PaintEventGlyph.GetPoint (rect, 3, 8), PaintEventGlyph.GetPoint (rect, 3, 7));
			path.Close ();

			//	Trou pour la clé (CCW).
			path.MoveTo (PaintEventGlyph.GetPoint (rect, 4, 2));
			path.LineTo (PaintEventGlyph.GetPoint (rect, 6, 2));
			path.LineTo (PaintEventGlyph.GetPoint (rect, 6, 4));
			path.LineTo (PaintEventGlyph.GetPoint (rect, 4, 4));
			path.Close ();

			return path;
		}

		private static Point GetPoint(Rectangle rect, double x, double y)
		{
			//	Retourne un point inscrit dans un espace de coordonnées comprises
			//	entre 0 et 10.
			return new Point (rect.Left + rect.Width*x/10, rect.Bottom + rect.Height*y/10);
		}

		private static Path GetPinnedPath(Rectangle rect)
		{
			//	Retourne le chemin d'un petit cercle, symbolisant l'empreinte de la punaise.
			var path = new Path ();

			path.AppendCircle (rect.Center, rect.Width*0.15);

			return path;
		}

		private static Path GetPlusPath(Rectangle rect)
		{
			//	Retourne le chemin pour un armotissement supplémentaire, sorte de damier
			//	ou noeud papillon évoquant un "+".
			var path = new Path ();

			path.MoveTo (rect.Center.X, rect.Top);
			path.LineTo (rect.Center.X, rect.Bottom);
			path.LineTo (rect.Right, rect.Center.Y);
			path.LineTo (rect.Left, rect.Center.Y);
			path.Close ();

			return path;
		}


		private static Color MainColor(TimelineGlyph glyph)
		{
			switch (glyph.Mode)
			{
				case TimelineGlyphMode.Dimmed:
					return ColorManager.DisableTextColor;

				default:
					return ColorManager.TextColor;
			}
		}
	}
}
