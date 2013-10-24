//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les éventuelles pastilles (glyphs).
	/// </summary>
	public class TimelineRowGlyphs : AbstractTimelineRow, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public void SetCells(TimelineCellGlyph[] cells)
		{
			this.cells = cells;
			this.Invalidate ();

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			if (this.cells == null || this.VisibleCellCount == 0)
			{
				return;
			}

			this.Paint (graphics);
		}

		private void Paint(Graphics graphics)
		{
			int x = 0;
			int index = 0;
			var lastCell = new TimelineCellGlyph ();  // cellule invalide

			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);
				if (!TimelineRowGlyphs.IsSame (lastCell, cell) && x != rank)
				{
					var rect = this.GetCellsRect (x, rank);
					bool isHover = (this.detectedHoverRank >= x && this.detectedHoverRank < rank);

					TimelineRowGlyphs.PaintCellBackground (graphics, rect, lastCell, isHover, index);
					TimelineRowGlyphs.PaintCellForeground (graphics, rect, lastCell, isHover, index);

					this.PaintGrid (graphics, rect, index, this.hilitedHoverRank);

					index++;
					x = rank;
				}

				lastCell = cell;
			}
		}

		private static void PaintCellBackground(Graphics graphics, Rectangle rect, TimelineCellGlyph cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = TimelineRowGlyphs.GetCellColor (cell, isHover, index);

			if (!color.IsBackground ())
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}
		}

		private static void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCellGlyph cell, bool isHover, int index)
		{
			//	Dessine le contenu.
			Rectangle r;
			Path path;

			switch (cell.Glyph)
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
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedSquare:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.25);
					r.Deflate (0.5);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDiamond:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.35);
					path = TimelineRowGlyphs.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDiamond:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.35);
					r.Deflate (0.5);
					path = TimelineRowGlyphs.GetDiamondPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledDown:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.3);
					path = TimelineRowGlyphs.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedDown:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = TimelineRowGlyphs.GetDownPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());

					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.FilledUp:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.3);
					path = TimelineRowGlyphs.GetUpPath (r);

					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineGlyph.OutlinedUp:
					r = TimelineRowGlyphs.GetGlyphSquare (rect, 0.3);
					r.Deflate (0.5);
					path = TimelineRowGlyphs.GetUpPath (r);

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


		private static bool IsSame(TimelineCellGlyph c1, TimelineCellGlyph c2)
		{
			return TimelineCellGlyph.IsSameGlyphs (c1, c2);
		}

		private static Color GetCellColor(TimelineCellGlyph cell, bool isHover, int index)
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
				//-else if (cell.Glyph != TimelineGlyph.Empty)
				//-{
				//-	return ColorManager.EditSinglePropertyColor;
				//-}
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


		private TimelineCellGlyph GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCellGlyph ();  // retourne une cellule invalide
		}

		private int GetListIndex(int rank)
		{
			if (rank >= 0 && rank < this.cells.Length)
			{
				int offset = (int) ((double) (this.cells.Length - this.VisibleCellCount) * this.pivot);
				return rank + offset;
			}
			else
			{
				return -1;
			}
		}


		#region IToolTipHost Members
		public object GetToolTipCaption(Point pos)
		{
			if (this.detectedHoverRank >= 0 && this.detectedHoverRank < this.cells.Length)
			{
				return this.cells[this.detectedHoverRank].Tooltip;
			}
			else
			{
				return null;
			}
		}
		#endregion


		private TimelineCellGlyph[] cells;
	}
}
