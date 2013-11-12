//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant les noms des mois et années.
	/// Par exemple "Septembre 2013", "Octobre 2013".
	/// Si la place manque, le nom du mois est compacté et l'année
	/// éventuellement supprimée.
	/// </summary>
	public class TimelineRowMonths : AbstractTimelineRow
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
				if (!TimelineRowMonths.IsSame (lastCell, cell) && x != rank)
				{
					var rect = this.GetCellsRect (x, rank);
					bool isHover = (this.detectedHoverRank >= x && this.detectedHoverRank < rank);

					this.PaintCellBackground (graphics, rect, lastCell, isHover, index);
					this.PaintCellForeground (graphics, rect, lastCell, x == rank-1);

					this.PaintGrid (graphics, rect, index, this.hilitedHoverRank, 0.0);

					index++;
					x = rank;
				}

				lastCell = cell;
			}
		}

		private void PaintCellBackground(Graphics graphics, Rectangle rect, TimelineCellDate cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = TimelineRowMonths.GetCellColor (cell, isHover, index);

			if (!color.IsBackground ())
			{
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}
		}

		private void PaintCellForeground(Graphics graphics, Rectangle rect, TimelineCellDate cell, bool square)
		{
			//	Dessine le contenu, plus ou moins détaillé selon la place disponible.
			var text = this.GetCellText (rect, cell, square);

			if (!string.IsNullOrEmpty (text))
			{
				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, text, font, this.FontSize, ContentAlignment.MiddleCenter);
			}
		}

		private string GetCellText(Rectangle rect, TimelineCellDate cell, bool square)
		{
			//	Retourne le texte, plus ou moins détaillé selon la place disponible.
			if (square)
			{
				//	Si la cellule est carrée (donc la plus petite taille possible),
				//	on évite d'essayer de caser des textes abrégés. Ceci est nécessaire
				//	pour uniformiser les textes et éviter des mélanges de chiffres et
				//	de lettres tels que "1 fév. 3 4...".
				return TimelineRowMonths.GetMonthText (cell, 0);
			}
			else
			{
				var font = Font.DefaultFont;

				for (int detailLevel = 4; detailLevel >= 0; detailLevel--)
				{
					var text = TimelineRowMonths.GetMonthText (cell, detailLevel);
					if (string.IsNullOrEmpty (text))
					{
						break;
					}

					var width = Helpers.Text.GetTextWidth (text, font, this.FontSize);
					if (width <= rect.Width)
					{
						return text;
					}
				}
			}

			return null;
		}

		private static bool IsSame(TimelineCellDate c1, TimelineCellDate c2)
		{
			return TimelineCellDate.IsSameMonths (c1, c2);
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

		private static string GetMonthText(TimelineCellDate cell, int detailLevel)
		{
			//	Retourne le mois sous une forme plus ou moins détaillée.
			//	detailLevel = 4 retourne "Septembre 2013"
			//	detailLevel = 3 retourne "Sept. 2013"
			//	detailLevel = 2 retourne "Septembre"
			//	detailLevel = 1 retourne "Sept."
			//	detailLevel = 0 retourne "9"
			//	Voir http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
			if (cell.IsValid)
			{
				switch (detailLevel)
				{
					case 4:
						return cell.Date.ToString ("MMMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 3:
						return cell.Date.ToString ("MMM yyyy", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 2:
						return cell.Date.ToString ("MMMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 1:
						return cell.Date.ToString ("MMM", System.Globalization.DateTimeFormatInfo.CurrentInfo);

					case 0:
						return cell.Date.Month.ToString (System.Globalization.DateTimeFormatInfo.CurrentInfo);
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
