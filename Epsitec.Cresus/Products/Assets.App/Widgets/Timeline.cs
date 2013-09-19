//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public sealed class Timeline : Widget
	{
		public Timeline()
		{
			this.display = TimelineDisplay.Month | TimelineDisplay.Days | TimelineDisplay.Glyphs;
			this.hoverRank = -1;
		}

		public TimelineDisplay					Display
		{
			get
			{
				return this.display;
			}
			set
			{
				if (this.display != value)
				{
					this.display = value;
					this.Invalidate ();
				}
			}
		}

		public double							Pivot
		{
			get
			{
				return this.pivot;
			}
			set
			{
				if (this.pivot != value)
				{
					this.pivot = value;
					this.Invalidate ();
				}
			}
		}

		public int								VisibleCellCount
		{
			get
			{
				return (int) (this.ActualBounds.Width / this.CellDim);
			}
		}

		private int								CellDim
		{
			get
			{
				return (int) (this.ActualBounds.Height / this.LineCount);
			}
		}

		private int								HoverRank
		{
			get
			{
				return this.hoverRank;
			}
			set
			{
				if (this.hoverRank != value)
				{
					this.hoverRank = value;
					this.Invalidate ();
				}
			}
		}

		
		public void SetCells(TimelineCell[] cells)
		{
			this.cells = cells;
			this.Invalidate ();
		}


		protected override void OnClicked(MessageEventArgs e)
		{
			this.OnCellClicked (this.hoverRank);
			base.OnClicked (e);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.HoverRank = this.Detect ((int) e.Point.X);
			base.OnMouseMove (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.HoverRank = -1;
			base.OnExited (e);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			int count = this.VisibleCellCount;
			if (count == 0)
			{
				return;
			}

			//	Dessine la ligne des mois.
			if ((this.display & TimelineDisplay.Month) != 0)
			{
				int line = this.GetLineRank (TimelineDisplay.Month);
				int x = 0;
				int index = 0;
				var lastCell = new TimelineCell ();  // cellule invalide

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameMonths (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, line);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellMonth (graphics, rect, lastCell, isHover, index++);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne des semaines.
			if ((this.display & TimelineDisplay.Weeks) != 0)
			{
				int line = this.GetLineRank (TimelineDisplay.Weeks);
				int x = 0;
				int index = 0;
				var lastCell = new TimelineCell ();  // cellule invalide

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameWeeks (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, line);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellWeek (graphics, rect, lastCell, isHover, index++);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne des jours de la semaine.
			if ((this.display & TimelineDisplay.DaysOfWeek) != 0)
			{
				int line = this.GetLineRank (TimelineDisplay.DaysOfWeek);
				int x = 0;
				var lastCell = new TimelineCell ();  // cellule invalide

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameDays (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, line);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellDaysOfWeek (graphics, rect, lastCell, isHover);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne des jours.
			if ((this.display & TimelineDisplay.Days) != 0)
			{
				int line = this.GetLineRank (TimelineDisplay.Days);
				int x = 0;
				var lastCell = new TimelineCell ();  // cellule invalide

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameDays (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, line);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellDay (graphics, rect, lastCell, isHover);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne des pastilles.
			if ((this.display & TimelineDisplay.Glyphs) != 0)
			{
				int line = this.GetLineRank (TimelineDisplay.Glyphs);

				for (int rank = 0; rank < count; rank++)
				{
					var rect = this.GetCellRect (rank, line);
					var cell = this.GetCell (rank);

					if (cell.IsValid)
					{
						bool isHover = (this.hoverRank == rank);
						Timeline.PaintCellGlyph (graphics, rect, cell, isHover);
					}
				}
			}
		}


		private int Detect(int x)
		{
			int count = this.VisibleCellCount;
			for (int rank = 0; rank < count; rank++)
			{
				int p1 = this.GetHorizontalPosition (rank);
				int p2 = this.GetHorizontalPosition (rank+1);

				if (x >= p1 && x < p2)
				{
					return rank;
				}
			}

			return -1;
		}

		private Rectangle GetCellRect(int x, int y)
		{
			return this.GetCellsRect (x, x+1, y);
		}

		private Rectangle GetCellsRect(int x1, int x2, int y)
		{
			int p1 = this.GetHorizontalPosition (x1);
			int p2 = this.GetHorizontalPosition (x2);

			int dim = this.CellDim;
			return new Rectangle (p1, y*dim, p2-p1, dim);
		}

		private int GetHorizontalPosition(int rank)
		{
			//	Retourne la position horizontale, avec une subile répartition du reste
			//	pour que la cellule de droite touche toujours le bord droite.
			double dim = this.ActualBounds.Width / this.VisibleCellCount;
			return (int) (rank * dim);
		}


		private static void PaintCellMonth(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = Timeline.GetMonthBackgroundColor (cell, isHover, index);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu, plus ou moins détaillé selon la place disponible.
			var font = Font.DefaultFont;

			for (int detailLevel = 3; detailLevel >= 0; detailLevel--)
			{
				var text = Timeline.GetMonthText (cell, detailLevel);
				if (string.IsNullOrEmpty (text))
				{
					break;
				}

				var width = new TextGeometry (0, 0, 1000, 100, text, font, rect.Height*0.6, ContentAlignment.MiddleLeft).Width;
				if (width <= rect.Width)
				{
					graphics.Color = ColorManager.TextColor;
					graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
					break;
				}
			}
		}

		private static void PaintCellWeek(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = Timeline.GetMonthBackgroundColor (cell, isHover, index);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			var text = Timeline.GetWeekText (cell);
			var font = Font.DefaultFont;
			graphics.Color = ColorManager.TextColor;
			graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
		}

		private static void PaintCellDaysOfWeek(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover)
		{
			//	Dessine le fond.
			var color = Timeline.GetDayBackgroundColor (cell, isHover);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			var text = Timeline.GetDaysOfWeekText (cell);
			var font = Font.DefaultFont;
			graphics.Color = ColorManager.TextColor;
			graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
		}

		private static void PaintCellDay(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover)
		{
			//	Dessine le fond.
			var color = Timeline.GetDayBackgroundColor (cell, isHover);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			var text = Timeline.GetDayText (cell);
			var font = Font.DefaultFont;
			graphics.Color = ColorManager.TextColor;
			graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
		}

		private static void PaintCellGlyph(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover)
		{
			//	Dessine le fond.
			var color = Timeline.GetGlyphBackgroundColor (cell, isHover);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			Timeline.PaintCellGlyph (graphics, rect, cell.Glyph);
		}

		private static void PaintCellGlyph(Graphics graphics, Rectangle rect, TimelineCellGlyph type)
		{
			Rectangle r;

			switch (type)
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
					r = Timeline.GetGlyphSquare (rect, 0.25);

					graphics.AddFilledRectangle (r);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineCellGlyph.OutlinedSquare:
					r = Timeline.GetGlyphSquare (rect, 0.25);
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


		private static bool IsSameWeeks(TimelineCell c1, TimelineCell c2)
		{
			int w1 = (c1.IsValid) ? c1.Date.WeekOfYear : -1;
			int w2 = (c2.IsValid) ? c2.Date.WeekOfYear : -1;

			return w1 == w2;
		}

		private static bool IsSameMonths(TimelineCell c1, TimelineCell c2)
		{
			int m1 = (c1.IsValid) ? c1.Date.Month : -1;
			int m2 = (c2.IsValid) ? c2.Date.Month : -1;

			return m1 == m2;
		}

		private static bool IsSameDays(TimelineCell c1, TimelineCell c2)
		{
			int d1 = (c1.IsValid) ? c1.Date.Day : -1;
			int d2 = (c2.IsValid) ? c2.Date.Day : -1;

			return d1 == d2;
		}


		private static Color GetMonthBackgroundColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				return (index%2 == 0) ? ColorManager.GetEvenMonthColor (isHover)
					                  : ColorManager.GetOddMonthColor (isHover);
			}
			else
			{
				return Color.Empty;
			}
		}

		private static Color GetDayBackgroundColor(TimelineCell cell, bool isHover)
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

		private static Color GetGlyphBackgroundColor(TimelineCell cell, bool isHover)
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


		private static string GetMonthText(TimelineCell cell, int detailLevel)
		{
			//	Retourne le mois sous une forme plus ou moins détaillée.
			//	detailLevel = 3 retourne "Septembre 2013"
			//	detailLevel = 2 retourne "Sept. 2013"
			//	detailLevel = 1 retourne "Septembre"
			//	detailLevel = 0 retourne "Sept."
			//	Voir http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
			if (cell.IsValid)
			{
				switch (detailLevel)
				{
					case 3:
						return cell.Date.ToString ("MMMM yyyy", DateTimeFormatInfo.CurrentInfo);

					case 2:
						return cell.Date.ToString ("MMM yyyy", DateTimeFormatInfo.CurrentInfo);

					case 1:
						return cell.Date.ToString ("MMMM", DateTimeFormatInfo.CurrentInfo);

					case 0:
						return cell.Date.ToString ("MMM", DateTimeFormatInfo.CurrentInfo);
				}
			}

			return null;
		}

		private static string GetWeekText(TimelineCell cell)
		{
			//	Retourne le numéro de semaine sous la forme "1" ou "52".
			if (cell.IsValid)
			{
				return cell.Date.WeekOfYear.ToString ();
			}
			else
			{
				return null;
			}
		}

		private static string GetDaysOfWeekText(TimelineCell cell)
		{
			//	Retourne le jour sous la forme "Lu" ou "Ma".
			if (cell.IsValid)
			{
				var text = cell.Date.ToString ("ddd", DateTimeFormatInfo.CurrentInfo);

				if (text.Length > 2)
				{
					text = text.Substring (0, 2);
				}

				return text;
			}
			else
			{
				return null;
			}
		}

		private static string GetDayText(TimelineCell cell)
		{
			//	Retourne le jour sous la forme "1" ou "31".
			if (cell.IsValid)
			{
				return cell.Date.ToString ("dd", DateTimeFormatInfo.CurrentInfo);
			}
			else
			{
				return null;
			}
		}


		private TimelineCell GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCell ();  // retourne une cellule invalide
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


		private int LineCount
		{
			get
			{
				return Timeline.GetLineCount (this.display);
			}
		}

		private int GetLineRank(TimelineDisplay display)
		{
			switch (display)
			{
				case TimelineDisplay.Glyphs:
					return 0;

				case TimelineDisplay.Days:
					return Timeline.GetLineCount (this.display & (TimelineDisplay.Glyphs));

				case TimelineDisplay.DaysOfWeek:
					return Timeline.GetLineCount (this.display & (TimelineDisplay.Glyphs | TimelineDisplay.Days));

				case TimelineDisplay.Weeks:
					return Timeline.GetLineCount (this.display & (TimelineDisplay.Glyphs | TimelineDisplay.Days | TimelineDisplay.DaysOfWeek));

				case TimelineDisplay.Month:
					return Timeline.GetLineCount (this.display & (TimelineDisplay.Glyphs | TimelineDisplay.Days | TimelineDisplay.DaysOfWeek | TimelineDisplay.Weeks));
			}

			return -1;
		}

		private static int GetLineCount(TimelineDisplay display)
		{
			int count = 0;

			if ((display & TimelineDisplay.Month) != 0)
			{
				count++;
			}

			if ((display & TimelineDisplay.Weeks) != 0)
			{
				count++;
			}

			if ((display & TimelineDisplay.DaysOfWeek) != 0)
			{
				count++;
			}

			if ((display & TimelineDisplay.Days) != 0)
			{
				count++;
			}

			if ((display & TimelineDisplay.Glyphs) != 0)
			{
				count++;
			}

			return count;
		}


		#region Events handler
		private void OnCellClicked(int rank)
		{
			if (this.CellClicked != null)
			{
				this.CellClicked (this, rank);
			}
		}

		public delegate void CellClickedEventHandler(object sender, int rank);
		public event CellClickedEventHandler CellClicked;
		#endregion


		private TimelineDisplay					display;
		private double							pivot;
		private TimelineCell[]					cells;
		private int								hoverRank;
	}
}