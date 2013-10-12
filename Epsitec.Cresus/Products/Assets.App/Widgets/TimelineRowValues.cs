//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant des valeurs numériques, généralement des montants.
	/// </summary>
	public class TimelineRowValues : AbstractTimelineRow
	{
		public TimelineRowValues()
		{
			this.min = decimal.MaxValue;
			this.max = decimal.MinValue;
		}


		public void SetCells(TimelineCellValue[] cells)
		{
			this.cells = cells;
			this.InitializeAfterCellsChanged ();
			this.Invalidate ();
		}


		public TimelineValueDisplayMode			ValueDisplayMode;
		public Color?							ValueColor1;
		public Color?							ValueColor2;


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
			var dots = new List<IndexedDot> ();

			if (this.HasMinMax)
			{
				for (int i=0; i<2; i++)
				{
					for (int rank = 0; rank <= this.VisibleCellCount; rank++)
					{
						var cell = this.GetCell (rank);

						if (cell.IsValid)
						{
							var y = this.GetVerticalPosition (cell, i);

							if (y.HasValue)
							{
								var rect = this.GetCellsRect (rank, rank+1);
								int x = (int) rect.Center.X - 1;

								dots.Add (new IndexedDot(new Point (x, y.Value), i));
							}
						}
					}
				}
			}

			this.PaintSurfaces (graphics, dots);
			this.PaintDots     (graphics, dots);
		}

		private void PaintSurfaces(Graphics graphics, List<IndexedDot> dots)
		{
			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Surfaces) != 0 ||
				(this.ValueDisplayMode & TimelineValueDisplayMode.Lines   ) != 0)
			{
				for (int i=0; i<2; i++)
				{
					var points = dots.Where (x => x.Index == i).Select (x => x.Point).ToArray ();
					this.PaintSurfaces (graphics, points, i);
				}
			}
		}

		private void PaintSurfaces(Graphics graphics, Point[] points, int index)
		{
			if (!points.Any ())
			{
				return;
			}

			var path = new Path ();

			path.MoveTo (points[0].X+0.5, -0.5);
			path.LineTo (points[0].X+0.5, points[0].Y+0.5);

			for (int i=1; i<points.Length; i++)
			{
				path.LineTo (points[i].X+0.5, points[i-1].Y+0.5);
				path.LineTo (points[i].X+0.5, points[i].Y+0.5);
			}

			var lastPoint = points.Last ();
			path.LineTo (this.ActualWidth+0.5, lastPoint.Y+0.5);
			path.LineTo (this.ActualWidth+0.5, -0.5);
			path.Close ();

			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Surfaces) != 0)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (this.GetSurfaceColor (index));
			}

			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Lines) != 0)
			{
				graphics.AddPath (path);
				graphics.RenderSolid (this.GetDotColor (index));
			}
		}

		private void PaintDots(Graphics graphics, List<IndexedDot> dots)
		{
			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Dots) != 0)
			{
				foreach (var dot in dots)
				{
					var color = this.GetDotColor (dot.Index);
					this.PaintDot (graphics, dot.Point, color);
				}
			}
		}

		private void PaintDot(Graphics graphics, Point pos, Color color)
		{
			int x = (int) pos.X;
			int y = (int) pos.Y;
			int s = this.DotSize;

			if (false)  // points carrée ?
			{
				graphics.AddFilledRectangle (x-s/2, y-s/2, s, s);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddRectangle (x-s/2+0.5, y-s/2+0.5, s, s);
				graphics.RenderSolid (color);
			}
			else  // points ronds ?
			{
				graphics.AddFilledCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (color);
			}
		}

		private struct IndexedDot
		{
			public IndexedDot(Point point, int index)
			{
				this.Point = point;
				this.Index = index;
			}

			public readonly Point Point;
			public readonly int   Index;
		}

		private Color GetSurfaceColor(int rank)
		{
			return this.GetColor (rank).Alpha (0.15);
		}

		private Color GetDotColor(int rank)
		{
			return this.GetColor (rank);
		}

		private Color GetColor(int rank)
		{
			var color = (rank == 0) ? this.ValueColor1 : this.ValueColor2;

			if (color.HasValue)
			{
				return color.Value;
			}
			else
			{
				return ColorManager.ValueDotColor;
			}
		}
		
		
		private int? GetVerticalPosition(TimelineCellValue cell, int rank)
		{
			if (this.HasMinMax)
			{
				var value = cell.GetValue (rank);

				if (value.HasValue)
				{
					var factor = (double) ((value.Value - this.min) / (this.max - this.min));
					return this.DotSize + (int) (factor * (this.ActualHeight - this.DotSize*2));
				}
			}

			return null;
		}

		private bool HasMinMax
		{
			get
			{
				return this.min < this.max;
			}
		}

		private int DotSize
		{
			get
			{
				return System.Math.Max ((int) (this.ActualHeight * 0.1), 2);
			}
		}

		protected override void InitializeAfterCellsChanged()
		{
			this.min = decimal.MaxValue;
			this.max = decimal.MinValue;

			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);

				if (cell.Value1.HasValue)
				{
					this.min = System.Math.Min (this.min, cell.Value1.Value);
					this.max = System.Math.Max (this.max, cell.Value1.Value);
				}

				if (cell.Value2.HasValue)
				{
					this.min = System.Math.Min (this.min, cell.Value2.Value);
					this.max = System.Math.Max (this.max, cell.Value2.Value);
				}
			}
		}


		private TimelineCellValue GetCell(int rank)
		{
			if (rank < this.VisibleCellCount)
			{
				int index = this.GetListIndex (rank);

				if (index >= 0 && index < this.cells.Length)
				{
					return this.cells[index];
				}
			}

			return new TimelineCellValue ();  // retourne une cellule invalide
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


		private TimelineCellValue[] cells;
		private decimal min;
		private decimal max;
	}
}
