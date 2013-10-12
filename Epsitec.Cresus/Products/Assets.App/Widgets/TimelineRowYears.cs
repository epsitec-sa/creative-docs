//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

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

					this.PaintGrid (graphics, rect, index, this.hilitedHoverRank);

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
			//	Dessine le contenu.
			var text = TimelineRowYears.GetCellText (cell);
			var font = Font.DefaultFont;
			graphics.Color = ColorManager.TextColor;
			graphics.PaintText (rect, text, font, this.FontSize, ContentAlignment.MiddleCenter);
		}

		private static bool IsSame(TimelineCellDate c1, TimelineCellDate c2)
		{
			return TimelineCellDate.IsSameYears (c1, c2);
		}

		private static Color GetCellColor(TimelineCellDate cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				if (cell.Date.DayOfWeek == System.DayOfWeek.Saturday ||
					cell.Date.DayOfWeek == System.DayOfWeek.Sunday)
				{
					return ColorManager.GetHolidayColor (isHover);
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

		private static string GetCellText(TimelineCellDate cell)
		{
			//	Retourne le jour sous la forme "1" ou "31".
			//	Voir http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
			if (cell.IsValid)
			{
				return cell.Date.ToString ("yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);
			}
			else
			{
				return null;
			}
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
