//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les années.
	/// Par exemple "2012", "2013", "2014".
	/// </summary>
	public class TimelineRowYears : AbstractTimelineRow
	{
		public void SetCells(TimelineCellDate[] cells)
		{
			this.cells = cells;
			this.Invalidate ();
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
			var lastCell = new TimelineCellDate ();  // cellule invalide

			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);
				if (!TimelineRowYears.IsSame (lastCell, cell) && x != rank)
				{
					var rect = this.GetCellsRect (x, rank);
					bool isHover = (this.detectedHoverRank >= x && this.detectedHoverRank < rank);

					this.PaintCellBackground (graphics, rect, lastCell, isHover, index);
					this.PaintCellForeground (graphics, rect, lastCell, isHover, index);

					this.PaintGrid (graphics, rect, index, this.hilitedHoverRank, false, 0.0);

					index++;
					x = rank;
				}

				lastCell = cell;
			}
		}

		private void PaintCellBackground(Graphics graphics, Rectangle rect, TimelineCellDate cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = TimelineRowYears.GetCellColor (cell, isHover, index);

			if (!color.IsBackground ())
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}
		}

		private void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCellDate cell, bool isHover, int index)
		{
			//	Dessine le contenu, plus ou moins détaillé selon la place disponible.
			var text = this.GetCellText (rect, cell);

			if (!string.IsNullOrEmpty (text))
			{
				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, text, font, this.FontSize, ContentAlignment.MiddleCenter);
			}
		}

		private static bool IsSame(TimelineCellDate c1, TimelineCellDate c2)
		{
			return TimelineCellDate.IsSameYears (c1, c2);
		}

		private static Color GetCellColor(TimelineCellDate cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				return ColorManager.GetBackgroundColor (false);
			}
			else
			{
				return Color.Empty;
			}
		}

		private string GetCellText(Rectangle rect, TimelineCellDate cell)
		{
			//	Retourne l'année sous la forme "2014" ou "14".
			if (cell.IsValid)
			{
				var font = Font.DefaultFont;

				int detailLevel = 0;
				while (true)
				{
					var text = cell.Date.ToYear (detailLevel--);
					if (string.IsNullOrEmpty (text))
					{
						break;
					}

					var width = text.GetTextWidth (font, this.FontSize);
					if (width <= rect.Width)
					{
						return text;
					}
				}
			}

			return null;
		}


		private TimelineCellDate GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCellDate ();  // retourne une cellule invalide
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


		private TimelineCellDate[] cells;
	}
}
