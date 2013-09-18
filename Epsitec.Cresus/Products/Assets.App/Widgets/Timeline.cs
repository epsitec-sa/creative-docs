//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class Timeline : Widget
	{
		public Timeline()
		{
		}

		public double Pivot
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

		public void SetCells(TimelineCell[] cells)
		{
			this.cells = cells;
		}

		public int VisibleCellCount
		{
			get
			{
				return (int) (this.ActualBounds.Width / this.CellDim);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			int count = this.VisibleCellCount;

			//	Dessine la ligne 1 (les mois).
			{
				int x = 0;
				var lastCell = new TimelineCell ();

				for (int rank = 0; rank <= count; rank++)
				{
					var cell = this.GetCell (rank);
					if (!Timeline.IsSameMonths (lastCell, cell) && x != rank)
					{
						var rect = this.GetCellsRect (x, rank, 2);
						this.PaintCellMonth (graphics, rect, lastCell);
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
						this.PaintCellDay (graphics, rect, lastCell);
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
					this.PaintCellBullet (graphics, rect, cell);
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


		private void PaintCellMonth(Graphics graphics, Rectangle rect, TimelineCell cell)
		{
			//	Dessine le fond.
			{
				var color = Color.FromName ("White");
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}

			//	Dessine le cadre.
			{
				var color = Color.FromName ("Black");
				var r = rect;
				r.Inflate (0.5);
				graphics.AddRectangle (r);
				graphics.RenderSolid (color);
			}

			//	Dessine le contenu.
			var text = Timeline.GetCellMonth (cell);
			var font = Font.DefaultFont;
			graphics.PaintText (rect, text, font, rect.Height*0.5, Common.Drawing.ContentAlignment.MiddleCenter);
		}

		private void PaintCellDay(Graphics graphics, Rectangle rect, TimelineCell cell)
		{
			//	Dessine le fond.
			{
				var color = Timeline.GetCellDayColor (cell);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}

			//	Dessine le cadre.
			{
				var color = Color.FromName ("Black");
				var r = rect;
				r.Inflate (0.5);
				graphics.AddRectangle (r);
				graphics.RenderSolid (color);
			}

			//	Dessine le contenu.
			var text = Timeline.GetCellDay (cell);
			var font = Font.DefaultFont;
			graphics.PaintText (rect, text, font, rect.Height*0.5, Common.Drawing.ContentAlignment.MiddleCenter);
		}

		private void PaintCellBullet(Graphics graphics, Rectangle rect, TimelineCell cell)
		{
			//	Dessine le fond.
			{
				var color = Timeline.GetCellBulletColor (cell);
				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (color);
			}

			//	Dessine le cadre.
			{
				var color = Color.FromName ("Black");
				var r = rect;
				r.Inflate (0.5);
				graphics.AddRectangle (r);
				graphics.RenderSolid (color);
			}

			//	Dessine le contenu.
			this.PaintCellBullet (graphics, rect, cell.Type);
		}

		private void PaintCellBullet(Graphics graphics, Rectangle rect, TimelineCellType type)
		{
			var color = Color.FromName ("Black");

			switch (type)
			{
				case TimelineCellType.FilledCircle:
					graphics.AddFilledCircle (rect.Center, rect.Height*0.35);
					graphics.RenderSolid (color);
					break;

				case TimelineCellType.OutlinedCircle:
					graphics.AddCircle (rect.Center, rect.Height*0.35);
					graphics.RenderSolid (color);
					break;

				case TimelineCellType.FilledSquare:
					rect.Deflate (rect.Height * 0.3);
					graphics.AddFilledRectangle (rect);
					graphics.RenderSolid (color);
					break;

				case TimelineCellType.OutlinedSquare:
					rect.Deflate (rect.Height * 0.3);
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

		private static Color GetCellDayColor(TimelineCell cell)
		{
			if (cell.IsValid)
			{
				if (cell.Date.DayOfWeek == DayOfWeek.Saturday ||
					cell.Date.DayOfWeek == DayOfWeek.Sunday)
				{
					return Color.FromName ("LightGray");
				}
				else
				{
					return Color.FromName ("White");
				}
			}
			else
			{
				return Color.Empty;
			}
		}

		private static Color GetCellBulletColor(TimelineCell cell)
		{
			if (cell.IsValid)
			{
				if (cell.IsSelected)
				{
					return Color.FromName ("Gold");
				}
				else
				{
					return Color.FromName ("White");
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

		private int CellDim
		{
			//	Ligne 1 -> Noms des mois
			//	Ligne 2 -> Numéros de jours
			//	Ligne 3 -> Pastilles des événements
			get
			{
				return (int) (this.ActualBounds.Height / 3);
			}
		}

		private double pivot;
		private TimelineCell[] cells;
	}
}