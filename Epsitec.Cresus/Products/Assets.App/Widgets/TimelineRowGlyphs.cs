//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class TimelineRowGlyphs : AbstractTimelineRow
	{
		public TimelineRowGlyphs(TimelineDisplay display)
			: base (display)
		{
		}


		protected override void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le contenu.
			Rectangle r;

			switch (cell.Glyph)
			{
				case TimelineCellGlyph.FilledCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.28);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineCellGlyph.OutlinedCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineCellGlyph.FilledSquare:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineCellGlyph.OutlinedSquare:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.25);
					r.Deflate (0.5);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddRectangle (r);
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

		protected override Color GetCellColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				if (cell.IsSelected)
				{
					return ColorManager.SelectionColor;
				}
				else if (isHover)
				{
					return ColorManager.HoverColor;
				}
				else
				{
					return ColorManager.GetBackgroundColor (isHover);
				}
			}
			else
			{
				return Color.Empty;
			}
		}
	}
}
