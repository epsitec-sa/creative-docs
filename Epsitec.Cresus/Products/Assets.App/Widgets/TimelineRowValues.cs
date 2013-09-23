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


		public TimelineValueDisplayMode ValueDisplayMode;


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
			graphics.AddFilledRectangle (new Rectangle (Point.Zero, this.ActualSize));
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());

			if (this.HasMinMax)
			{
				var path = new Path ();
				var dots = new List<Point> ();
				int? lastX = null;
				int? lastY = null;

				for (int rank = 0; rank <= this.VisibleCellCount; rank++)
				{
					var cell = this.GetCell (rank);

					if (cell.IsValid)
					{
						var y = this.GetVerticalPosition (cell);

						if (y.HasValue)
						{
							var rect = this.GetCellsRect (rank, rank+1);
							int x = (int) rect.Center.X - 1;

							if (lastX.HasValue && lastY.HasValue)
							{
								if (path.IsEmpty)
								{
									path.MoveTo (lastX.Value+0.5, -0.5);
									path.LineTo (lastX.Value+0.5, lastY.Value+0.5);
								}

								path.LineTo (x+0.5, lastY.Value+0.5);
								path.LineTo (x+0.5, y.Value+0.5);
							}

							dots.Add (new Point (x, y.Value));

							lastX = x;
							lastY = y;
						}
					}
				}

				path.LineTo (this.ActualWidth+0.5, lastY.Value+0.5);
				path.LineTo (this.ActualWidth+0.5, -0.5);
				path.Close ();

				if ((this.ValueDisplayMode & TimelineValueDisplayMode.Surfaces) != 0)
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (ColorManager.ValueSurfaceColor);
				}

				if ((this.ValueDisplayMode & TimelineValueDisplayMode.Lines) != 0)
				{
					graphics.AddPath (path);
					graphics.RenderSolid (ColorManager.ValueDotColor);
				}

				if ((this.ValueDisplayMode & TimelineValueDisplayMode.Dots) != 0)
				{
					foreach (var dot in dots)
					{
						this.PaintDot (graphics, dot);
					}
				}
			}
		}

		private void PaintDot(Graphics graphics, Point dot)
		{
			int x = (int) dot.X;
			int y = (int) dot.Y;
			int s = this.DotSize;

			if (false)  // points carrée ?
			{
				graphics.AddFilledRectangle (x-s/2, y-s/2, s, s);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddRectangle (x-s/2+0.5, y-s/2+0.5, s, s);
				graphics.RenderSolid (ColorManager.ValueDotColor);
			}
			else  // points ronds ?
			{
				graphics.AddFilledCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (ColorManager.GetBackgroundColor ());

				graphics.AddCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (ColorManager.ValueDotColor);
			}
		}
		
		
		private int? GetVerticalPosition(TimelineCellValue cell)
		{
			if (cell.IsValid && this.HasMinMax)
			{
				var factor = (double) ((cell.Value.Value - this.min) / (this.max - this.min));
				return this.DotSize + (int) (factor * (this.ActualHeight - this.DotSize*2));
			}
			else
			{
				return null;
			}
		}

		private bool HasMinMax
		{
			get
			{
				return this.min <= this.max;
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

				if (cell.IsValid)
				{
					this.min = System.Math.Min (this.min, cell.Value.Value);
					this.max = System.Math.Max (this.max, cell.Value.Value);
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
