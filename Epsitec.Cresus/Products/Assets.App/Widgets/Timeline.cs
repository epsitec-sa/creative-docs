//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public sealed class Timeline : Widget
	{
		public Timeline()
		{
			this.hoverRank = -1;
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
			//	Ligne 1 -> Noms des mois
			//	Ligne 2 -> Numéros de jours
			//	Ligne 3 -> Pastilles des événements
			get
			{
				return (int) (this.ActualBounds.Height / 3);
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

			//	Dessine la ligne 1 (les mois).
			{
				int x = 0;
				int index = 0;
				var lastCell = new TimelineCell ();

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameMonths (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, 2);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellMonth (graphics, rect, lastCell, isHover, index++);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne 2 (les jours).
			{
				int x = 0;
				var lastCell = new TimelineCell ();

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameDays (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, 1);
						bool isHover = (this.hoverRank >= x && this.hoverRank < rank);
						Timeline.PaintCellDay (graphics, rect, lastCell, isHover);
						x = rank;
					}

					lastCell = cell;
				}
			}

			//	Dessine la ligne 3 (les pastilles).
			{
				for (int rank = 0; rank < count; rank++)
				{
					var rect = this.GetCellRect (rank, 0);
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

		private int GetHorizontalPosition(int x)
		{
			return x * this.CellDim;
		}


		private static void PaintCellMonth(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = Timeline.GetMonthBackgroundColor (cell, isHover, index);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			//	Affiche "Septembre 2013", "Sept. 2013", "Septembre" ou "Sept." selon la place disponible.
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
					rect.Deflate (rect.Height * 0.25);
					rect.Inflate (0.5);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (ColorManager.TextColor);
					break;

				case TimelineCellGlyph.OutlinedSquare:
					rect.Deflate (rect.Height * 0.25);

					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (ColorManager.GetBackgroundColor ());
					
					graphics.AddRectangle (rect);
					graphics.RenderSolid (ColorManager.TextColor);
					break;
			}
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

			return new TimelineCell ();
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


		private double							pivot;
		private TimelineCell[]					cells;
		private int								hoverRank;
	}
}