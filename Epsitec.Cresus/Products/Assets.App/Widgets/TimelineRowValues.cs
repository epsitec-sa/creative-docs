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
		public TimelineRowValues(TimelineRowDescription row)
			: base (row)
		{
		}


		protected override void Paint(Graphics graphics)
		{
			for (int rank = 0; rank <= this.VisibleCellCount; rank++)
			{
				var cell = this.GetCell (rank);
				if (cell.IsValid)
				{
					var rect = this.GetCellsRect (rank, rank+1);
					bool isHover = (this.hoverRank == rank);

					this.PaintCellBackground (graphics, rect, cell, isHover, rank);
				}
			}

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
									path.MoveTo (lastX.Value+0.5, lastY.Value+0.5);
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

				graphics.AddPath (path);
				graphics.RenderSolid (ColorManager.ValueLineColor);

				foreach (var dot in dots)
				{
					this.PaintDot (graphics, dot);
				}
			}
		}

		private void PaintDot(Graphics graphics, Point dot)
		{
			int x = (int) dot.X;
			int y = (int) dot.Y;
			int s = this.DotSize;

			graphics.AddFilledRectangle (x-s/2, y-s/2, s, s);
			graphics.RenderSolid (ColorManager.GetBackgroundColor ());

			graphics.AddRectangle (x-s/2+0.5, y-s/2+0.5, s, s);
			graphics.RenderSolid (ColorManager.ValueDotColor);
		}

		protected override Color GetCellColor(TimelineCell cell, bool isHover, int index)
		{
			if (cell.IsValid)
			{
				return ColorManager.GetBackgroundColor (isHover);
			}
			else
			{
				return Color.Empty;
			}
		}


		private bool HasMinMax
		{
			get
			{
				return this.min <= this.max;
			}
		}

		private int? GetVerticalPosition(TimelineCell cell)
		{
			if (cell.IsValid && cell.Value.HasValue)
			{
				var factor = (double) ((cell.Value.Value - this.min) / (this.max - this.min));
				return this.DotSize + (int) (factor * (this.ActualHeight - this.DotSize*2));
			}
			else
			{
				return null;
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

				if (cell.IsValid && cell.Value.HasValue)
				{
					this.min = System.Math.Min (this.min, cell.Value.Value);
					this.max = System.Math.Max (this.max, cell.Value.Value);
				}
			}
		}


		private decimal min;
		private decimal max;
	}
}
