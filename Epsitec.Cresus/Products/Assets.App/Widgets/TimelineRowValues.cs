//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
			this.Invalidate ();
		}

		public void SetMinMax(decimal min, decimal max)
		{
			//	Le zéro est toujours inclu, pour avoir des courbes comparables.
			this.min = System.Math.Min (min, 0.0m);
			this.max = System.Math.Max (max, 0.0m);
		}


		public TimelineValueDisplayMode			ValueDisplayMode;


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
			var dots = new List<Point> ();

			if (this.HasMinMax)
			{
				//	Compte le nombre de lignes.
				int linesCount = 0;

				for (int rank = -1; rank <= this.VisibleCellCount; rank++)
				{
					var cell = this.GetCell (rank);

					if (cell.IsValid)
					{
						linesCount = System.Math.Max (linesCount, cell.ValueCount);
					}
				}

				//	Dessine les surfaces.
				for (int line=0; line<linesCount; line++)
				{
					dots.Clear ();

					for (int rank = -1; rank <= this.VisibleCellCount; rank++)
					{
						var cell = this.GetCell (rank);

						if (cell.IsValid)
						{
							var y = this.GetVerticalPosition (cell, line);

							if (y.HasValue)
							{
								var rect = this.GetCellsRect (rank, rank+1);
								int x = (int) rect.Center.X - 1;

								dots.Add (new Point (x, y.Value));
							}
						}
					}

					this.PaintSurfaces (graphics, dots, line);
				}

				//	Dessine les points.
				for (int line=0; line<linesCount; line++)
				{
					dots.Clear ();

					for (int rank = -1; rank <= this.VisibleCellCount; rank++)
					{
						var cell = this.GetCell (rank);

						if (cell.IsValid)
						{
							var y = this.GetVerticalPosition (cell, line);

							if (y.HasValue)
							{
								var rect = this.GetCellsRect (rank, rank+1);
								int x = (int) rect.Center.X - 1;

								dots.Add (new Point (x, y.Value));
							}
						}
					}

					this.PaintDots (graphics, dots, line);
				}
			}
		}

		private void PaintSurfaces(Graphics graphics, List<Point> dots, int line)
		{
			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Surfaces) == 0 &&
				(this.ValueDisplayMode & TimelineValueDisplayMode.Lines   ) == 0)
			{
				return;
			}

			if (!dots.Any ())
			{
				return;
			}

			var path = new Path ();

			path.MoveTo (dots[0].X+0.5, -0.5);
			path.LineTo (dots[0].X+0.5, dots[0].Y+0.5);

			for (int i=1; i<dots.Count; i++)
			{
				path.LineTo (dots[i].X+0.5, dots[i-1].Y+0.5);
				path.LineTo (dots[i].X+0.5, dots[i].Y+0.5);
			}

			var lastPoint = dots.Last ();
			path.LineTo (this.ActualWidth+0.5, lastPoint.Y+0.5);
			path.LineTo (this.ActualWidth+0.5, -0.5);
			path.Close ();

			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Surfaces) != 0)
			{
				graphics.AddFilledPath (path);
				graphics.RenderSolid (this.GetSurfaceColor (line));
			}

			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Lines) != 0)
			{
				graphics.AddPath (path);
				graphics.RenderSolid (this.GetDotColor (line));
			}
		}

		private void PaintDots(Graphics graphics, List<Point> dots, int line)
		{
			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Dots) != 0)
			{
				foreach (var dot in dots)
				{
					var color = this.GetDotColor (line);
					this.PaintDot (graphics, dot, color, line);
				}
			}
		}

		private void PaintDot(Graphics graphics, Point pos, Color color, int line)
		{
			int x = (int) pos.X;
			int y = (int) pos.Y;
			int s = this.DotSize;

			if (line == 0)  // points ronds (MainValue) ?
			{
				graphics.AddFilledCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (color);
			}
			else  // points carrés (UserValue) ?
			{
				s++;
				graphics.AddFilledRectangle (x-s/2, y-s/2, s, s);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddRectangle (x-s/2+0.5, y-s/2+0.5, s, s);
				graphics.RenderSolid (color);
			}
		}

		private Color GetSurfaceColor(int line)
		{
			if (line == 0)
			{
				return Color.FromAlphaRgb (0.15, 0.0, 0.0, 0.0);
			}
			else
			{
				double i = (line%2 == 0) ? 0.6 : 0.8;
				return Color.FromAlphaRgb (0.15, i, i, i);
			}
		}

		private Color GetDotColor(int line)
		{
			if (line == 0)
			{
				return Color.FromBrightness (0.2);
			}
			else
			{
				return Color.FromBrightness (0.5);
			}
		}

		
		private int? GetVerticalPosition(TimelineCellValue cell, int line)
		{
			if (this.HasMinMax)
			{
				var value = cell.GetValue (line);

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
				//?return System.Math.Max ((int) (this.ActualHeight * 0.1), 2);
				return 3;
			}
		}


		private TimelineCellValue GetCell(int rank)
		{
			return this.cells.Where (x => x.Rank == rank).FirstOrDefault ();
		}


		private TimelineCellValue[] cells;
		private decimal min;
		private decimal max;
	}
}
