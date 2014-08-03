//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Core.Helpers;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Ligne de Timeline affichant des valeurs numériques, généralement des montants.
	/// </summary>
	public class TimelineRowValues : AbstractTimelineRow, Epsitec.Common.Widgets.Helpers.IToolTipHost
	{
		public TimelineRowValues()
		{
			this.fieldNames = new List<string> ();

			this.min = decimal.MaxValue;
			this.max = decimal.MinValue;

			this.hoverLine = -1;
			this.hoverRank = -1;

			ToolTip.Default.RegisterDynamicToolTipHost (this);  // pour voir les tooltips dynamiques
		}


		public List<string>						FieldNames
		{
			get
			{
				return this.fieldNames;
			}
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


		#region IToolTipHost Members
		public object GetToolTipCaption(Point pos)
		{
			if (this.hoverRank != -1 && this.hoverLine != -1)
			{
				var cell = this.GetCell (this.hoverRank);

				if (cell.IsValid)
				{
					var value = cell.GetValue (this.hoverLine);

					if (value.HasValue)
					{
						var amount = TypeConverters.AmountToString (value.Value);

						if (this.hoverLine >= 0 && this.hoverLine < this.fieldNames.Count)
						{
							var fieldName = this.fieldNames[this.hoverLine];
							return string.Format ("{0} {1}", fieldName, amount);
						}
						else
						{
							return amount;
						}
					}
				}
			}

			return null;
		}
		#endregion


		protected override void OnMouseMove(MessageEventArgs e)
		{
			if (this.DetectDot (e.Point))
			{
				this.Invalidate ();
			}

			base.OnMouseMove (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.hoverLine = -1;
			this.hoverRank = -1;
			this.Invalidate ();

			base.OnExited (e);
		}

		private bool DetectDot(Point pos)
		{
			var hoverRank = -1;
			var hoverLine = -1;

			for (int line=0; line<this.linesCount; line++)
			{
				for (int rank = -1; rank <= this.VisibleCellCount; rank++)
				{
					var center = this.GetDotCenter (rank, line);

					if (!center.IsZero)
					{
						var d = Point.Distance (pos, center);
						if (d <= 4)
						{
							hoverRank = rank;
							hoverLine = line;
						}
					}
				}
			}

			bool changing = this.hoverRank != hoverRank
						 || this.hoverLine != hoverLine;

			if (changing)
			{
				this.hoverRank = hoverRank;
				this.hoverLine = hoverLine;
			}

			return changing;
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
			var dots = new List<Point> ();

			if (this.HasMinMax)
			{
				//	Compte le nombre de lignes.
				this.linesCount = 0;

				for (int rank = -1; rank <= this.VisibleCellCount; rank++)
				{
					var cell = this.GetCell (rank);

					if (cell.IsValid)
					{
						this.linesCount = System.Math.Max (this.linesCount, cell.ValueCount);
					}
				}

				//	Dessine les surfaces.
				for (int line=0; line<this.linesCount; line++)
				{
					dots.Clear ();

					for (int rank = -1; rank <= this.VisibleCellCount; rank++)
					{
						var center = this.GetDotCenter (rank, line);

						if (!center.IsZero)
						{
							dots.Add (center);
						}
					}

					this.PaintSurfaces (graphics, dots, line);
				}

				//	Dessine les points.
				for (int line=0; line<this.linesCount; line++)
				{
					dots.Clear ();

					for (int rank = -1; rank <= this.VisibleCellCount; rank++)
					{
						var center = this.GetDotCenter (rank, line);

						if (!center.IsZero)
						{
							dots.Add (center);
						}
					}

					this.PaintDots (graphics, dots, line);
				}
			}
		}

		private Point GetDotCenter(int rank, int line)
		{
			var cell = this.GetCell (rank);

			if (cell.IsValid)
			{
				var y = this.GetVerticalPosition (cell, line);

				if (y.HasValue)
				{
					var rect = this.GetCellsRect (rank, rank+1);
					int x = (int) rect.Center.X - 1;

					return new Point (x, y.Value);
				}
			}

			return Point.Zero;
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
				var color = this.GetSurfaceColor (line);
				if (!color.IsEmpty)
				{
					graphics.AddFilledPath (path);
					graphics.RenderSolid (color);
				}
			}

			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Lines) != 0)
			{
				graphics.AddPath (path);
				graphics.RenderSolid (this.GetLineColor (line));
			}
		}

		private void PaintDots(Graphics graphics, List<Point> dots, int line)
		{
			if ((this.ValueDisplayMode & TimelineValueDisplayMode.Dots) != 0)
			{
				foreach (var dot in dots)
				{
					this.PaintDot (graphics, dot, line);
				}
			}
		}

		private void PaintDot(Graphics graphics, Point pos, int line)
		{
			int x = (int) pos.X;
			int y = (int) pos.Y;
			int s = this.DotSize;

			if (line == 0)  // points ronds (MainValue) ?
			{
				graphics.AddFilledCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (this.GetDotFillColor (line));

				graphics.AddCircle (x+0.5, y+0.5, s-1.0);
				graphics.RenderSolid (this.GetLineColor (line));
			}
			else  // points carrés (UserValue) ?
			{
				s++;
				graphics.AddFilledRectangle (x-s/2, y-s/2, s, s);
				graphics.RenderSolid (this.GetDotFillColor (line));

				graphics.AddRectangle (x-s/2+0.5, y-s/2+0.5, s, s);
				graphics.RenderSolid (this.GetLineColor (line));
			}
		}

		private Color GetLineColor(int line)
		{
			if (this.hoverLine == -1)
			{
				if (line == 0)
				{
					return Color.FromAlphaRgb (0.8, 0.0, 0.0, 0.0);
				}
				else
				{
					return Color.FromAlphaRgb (0.2, 0.0, 0.0, 0.0);
				}
			}
			else
			{
				if (line == this.hoverLine)
				{
					return Color.FromAlphaRgb (0.8, 0.0, 0.0, 0.0);
				}
				else
				{
					return Color.FromAlphaRgb (0.2, 0.0, 0.0, 0.0);
				}
			}
		}

		private Color GetSurfaceColor(int line)
		{
			if (line == this.hoverLine)
			{
				return Color.FromAlphaColor (0.4, ColorManager.SelectionColor);
			}
			else
			{
				if (line == 0 && this.hoverLine == -1)
				{
					return Color.FromAlphaRgb (0.15, 0.0, 0.0, 0.0);
				}
				else
				{
					return Color.Empty;
				}
			}
		}

		private Color GetDotFillColor(int line)
		{
			if (line == this.hoverLine)
			{
				return ColorManager.SelectionColor;
			}
			else
			{
				return ColorManager.GetBackgroundColor ();
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


		private readonly List<string>			fieldNames;

		private TimelineCellValue[]				cells;
		private decimal							min;
		private decimal							max;
		private int								linesCount;
		private int								hoverRank;
		private int								hoverLine;
	}
}
