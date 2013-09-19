//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

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
		}


		protected override void OnMouseMove(MessageEventArgs e)
		{
			this.HoverRank = (int) (e.Point.X / this.CellDim);
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
						this.PaintCellMonth (graphics, rect, lastCell, isHover, index++);
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
						this.PaintCellDay (graphics, rect, lastCell, isHover);
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
						this.PaintCellGlyph (graphics, rect, cell, isHover);
					}
				}
			}
		}


		private Rectangle GetCellRect(int x, int y)
		{
			return this.GetCellsRect (x, x+1, y);
		}

		private Rectangle GetCellsRect(int x1, int x2, int y)
		{
			int dim = this.CellDim;
			return new Rectangle (x1*dim, y*dim, (x2-x1)*dim, dim);
		}


		private void PaintCellMonth(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover, int index)
		{
			//	Dessine le fond.
			var color = Timeline.GetCellMonthColor (cell, isHover, index);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			var text = Timeline.GetCellMonth (cell);
			var font = Font.DefaultFont;

			var width = new TextGeometry (0, 0, 1000, 100, text, font, rect.Height*0.6, ContentAlignment.MiddleLeft).Width;
			if (width < rect.Width)
			{
				graphics.Color = Color.FromBrightness (0.2);
				graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
			}
		}

		private void PaintCellDay(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover)
		{
			//	Dessine le fond.
			var color = Timeline.GetCellDayColor (cell, isHover);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			var text = Timeline.GetCellDay (cell);
			var font = Font.DefaultFont;
			graphics.Color = Color.FromBrightness (0.2);
			graphics.PaintText (rect, text, font, rect.Height*0.6, ContentAlignment.MiddleCenter);
		}

		private void PaintCellGlyph(Graphics graphics, Rectangle rect, TimelineCell cell, bool isHover)
		{
			//	Dessine le fond.
			var color = Timeline.GetCellBulletColor (cell, isHover);
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine le contenu.
			this.PaintCellGlyph (graphics, rect, cell.Glyph);
		}

		private void PaintCellGlyph(Graphics graphics, Rectangle rect, TimelineCellGlyph type)
		{
			var color = Color.FromBrightness (0.2);

			switch (type)
			{
				case TimelineCellGlyph.FilledCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (color);
					break;

				case TimelineCellGlyph.OutlinedCircle:
					graphics.AddCircle (rect.Center, rect.Height*0.25);
					graphics.RenderSolid (color);
					break;

				case TimelineCellGlyph.FilledSquare:
					rect.Deflate (rect.Height * 0.25);
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (color);
					break;

				case TimelineCellGlyph.OutlinedSquare:
					rect.Deflate (rect.Height * 0.25);
					graphics.AddRectangle (rect);
					graphics.RenderSolid (color);
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


		private static Color GetCellMonthColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				if (isHover)
				{
					return Color.FromName ("LightBlue");
				}
				else
				{
					return Color.FromBrightness (index%2 == 0 ? 0.95 : 0.90);
				}
			}
			else
			{
				return Color.Empty;
			}
		}

		private static Color GetCellDayColor(TimelineCell cell, bool isHover)
		{
			if (cell.IsValid)
			{
				if (isHover)
				{
					return Color.FromName ("LightBlue");
				}
				else
				{
					if (cell.Date.DayOfWeek == System.DayOfWeek.Saturday ||
					cell.Date.DayOfWeek == System.DayOfWeek.Sunday)
					{
						return Color.FromBrightness (0.95);
					}
					else
					{
						return Color.FromBrightness (1.0);
					}
				}
			}
			else
			{
				return Color.Empty;
			}
		}

		private static Color GetCellBulletColor(TimelineCell cell, bool isHover)
		{
			if (cell.IsValid)
			{
				if (isHover)
				{
					return Color.FromName ("LightBlue");
				}
				else
				{
					if (cell.IsSelected)
					{
						return Color.FromName ("Gold");
					}
					else
					{
						return Color.FromBrightness (1.0);
					}
				}
			}
			else
			{
				return Color.Empty;
			}
		}


		private static string GetCellMonth(TimelineCell cell)
		{
			if (cell.IsValid)
			{
				return cell.Date.ToString ("MMM yyyy", DateTimeFormatInfo.CurrentInfo);
			}
			else
			{
				return null;
			}
		}

		private static string GetCellDay(TimelineCell cell)
		{
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

		private double							pivot;
		private TimelineCell[]					cells;
		private int								hoverRank;
	}
}