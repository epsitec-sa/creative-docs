//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public class CoordinateAxisAdorner : AbstractAdorner
	{
		public CoordinateAxisAdorner()
		{
			this.GridColor = Color.FromBrightness (0);
			this.GridLineWidth = 0.5;
		}

		public Styles.CaptionStyle Style
		{
			get
			{
				return this.style ?? CoordinateAxisAdorner.defaultStyle;
			}
			set
			{
				this.style = value;
			}
		}
		
		public Color GridColor
		{
			get;
			set;
		}
		
		public double GridLineWidth
		{
			get;
			set;
		}

		private HorizontalAxisMode HorizontalAxisMode
		{
			get;
			set;
		}

		public override void Paint(IPaintPort port, Renderers.AbstractRenderer renderer, PaintLayer layer)
		{
			if (layer != PaintLayer.Intermediate)
			{
				return;
			}

			this.HorizontalAxisMode = renderer.HorizontalAxisMode;

			Rectangle bounds = renderer.Bounds;
			Point     origin = renderer.GetPoint (0, 0.0);

			double max = renderer.MaxValue;
			double min = renderer.MinValue;

			this.PaintGrid (port, renderer);
			this.PaintVerticalAxisLabels (port, renderer);
			this.PaintHorizontalAxisLabels (port, renderer);

			port.LineWidth = 1;
			port.Color = Color.FromBrightness (0);

			this.PaintVerticalAxis (port, bounds.Left, bounds.Bottom, bounds.Top + this.extraLength,
				path =>
				{
					bool first = true;

					foreach (Point point in CoordinateAxisAdorner.GetVerticalAxisTicks (renderer, this.GetOptimalVerticalValueCount (bounds.Height)))
					{
						if (first)
						{
							first = false;
						}
						else
						{
							path.MoveTo (point.X - this.tickLength/2, point.Y);
							path.LineTo (point.X + this.tickLength/2, point.Y);
						}
					}
				});

			if ((0.0 <= max) &&
				(min <= 0.0))
			{
				this.PaintHorizonalAxis (port, bounds.Left, origin.Y, bounds.Right + this.extraLength,
					path =>
					{
						bool first = true;
						
						foreach (Point point in this.GetHorizontalAxisTicks (renderer))
						{
							if (first)
							{
								first = false;
							}
							else
							{
								path.MoveTo (point.X, point.Y - this.tickLength/2);
								path.LineTo (point.X, point.Y + this.tickLength/2);
							}
						}
					});
			}
		}

		private void PaintGrid(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;

			if (this.GridLineWidth > 0)
			{
				port.LineWidth = this.GridLineWidth;
				port.Color     = this.GridColor;

				using (Path path = new Path ())
				{
					foreach (Point point in this.GetHorizontalAxisTicks (renderer))
					{
						path.MoveTo (point.X, bounds.Bottom);
						path.LineTo (point.X, bounds.Top + this.extraLength / 2);
					}

					foreach (Point point in CoordinateAxisAdorner.GetVerticalAxisTicks (renderer, this.GetOptimalVerticalValueCount (bounds.Height)))
					{
						path.MoveTo (bounds.Left, point.Y);
						path.LineTo (bounds.Right + this.extraLength / 2, point.Y);
					}

					port.PaintOutline (path);
				}
			}
		}

		private void PaintHorizontalAxisLabels(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			if (renderer.ValueCount > 0)
			{
				Rectangle bounds = renderer.Bounds;
				
				var  style = this.Style;
				port.Color = style.FontColor;

				double max = renderer.MaxValue;
				double min = renderer.MinValue;

				int count = this.HorizontalAxisMode == HorizontalAxisMode.Ticks ? renderer.ValueCount-1 : renderer.ValueCount;

				double dx = bounds.Width / count;
				double x  = bounds.Left;
				double y  = (0.0 <= max) && (0.0 >= min) ? renderer.GetPoint (0, 0.0).Y : bounds.Bottom;
				double h  = style.Font.LineHeight * style.FontSize;

				if (this.HorizontalAxisMode == HorizontalAxisMode.Ranges)
				{
					x += dx/2;
				}
				
				foreach (var text in renderer.ValueLabels)
				{
					double len = style.Font.GetTextAdvance (text) * style.FontSize;
					port.PaintText (x - len/2, y - h, text, style.Font, style.FontSize);
					x += dx;
				}
			}
		}

		private void PaintVerticalAxisLabels(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;

			var  style = this.Style;
			port.Color = style.FontColor;

			foreach (var value in CoordinateAxisAdorner.GetLog10Values (renderer.MinValue, renderer.MaxValue, this.GetOptimalVerticalValueCount (bounds.Height)))
			{
				Point pos = renderer.GetPoint (0, value);
				string text = string.Format (System.Globalization.CultureInfo.CurrentCulture, "{0}  ", value);
				double len  = style.Font.GetTextAdvance (text) * style.FontSize;

				port.PaintText (pos.X - len, pos.Y, text, style.Font, style.FontSize);
			}
		}

		private void PaintHorizonalAxis(IPaintPort port, double x1, double y, double x2, System.Action<Path> addTicks)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x1, y);
				path.LineTo (x2, y);
				path.MoveTo (x2 - this.arrowLength, y - this.arrowBreadth/2);
				path.LineTo (x2, y);
				path.LineTo (x2 - this.arrowLength, y + this.arrowBreadth/2);

				if (this.tickLength > 0)
				{
					if (addTicks != null)
					{
						addTicks (path);
					}
				}

				port.PaintOutline (path);
			}
		}

		private void PaintVerticalAxis(IPaintPort port, double x, double y1, double y2, System.Action<Path> addTicks)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y1);
				path.LineTo (x, y2);
				path.MoveTo (x - this.arrowBreadth/2, y2 - this.arrowLength);
				path.LineTo (x, y2);
				path.LineTo (x + this.arrowBreadth/2, y2 - this.arrowLength);

				if (this.tickLength > 0)
				{
					if (addTicks != null)
					{
						addTicks (path);
					}
				}

				port.PaintOutline (path);
			}
		}

		private int GetOptimalVerticalValueCount(double height)
		{
			var style = this.Style;
			double lineHeight = style.Font.LineHeight * style.FontSize * 1.2;
			double ratio = height / lineHeight;

			if (ratio > 25)
			{
				return 20;
			}
			else if (ratio > 10)
			{
				return 10;
			}
			else if (ratio > 5)
			{
				return 5;
			}
			else if (ratio > 2)
			{
				return 2;
			}
			else
			{
				return 1;
			}
		}

		private IEnumerable<Point> GetHorizontalAxisTicks(Renderers.AbstractRenderer renderer)
		{
			int count = renderer.ValueCount;

			if (this.HorizontalAxisMode == HorizontalAxisMode.Ranges)
			{
				count++;
			}

			for (int i = 0; i < count; i++)
			{
				yield return renderer.GetPoint (i, 0.0);
			}
		}

		private static IEnumerable<Point> GetVerticalAxisTicks(Renderers.AbstractRenderer renderer, int optimalVerticalValueCount)
		{
			foreach (var value in CoordinateAxisAdorner.GetLog10Values (renderer.MinValue, renderer.MaxValue, optimalVerticalValueCount))
			{
				yield return renderer.GetPoint (0, value);
			}
		}

		private static IEnumerable<double> GetLog10Values(double min, double max, int optimalVerticalValueCount)
		{
			double range     = max - min;
			double logRange  = System.Math.Floor (System.Math.Log10 (range));
			double adjRange  = System.Math.Pow (10, logRange);
			double increment = adjRange / optimalVerticalValueCount;
			double value     = 0;

			int tickRatio = (int)(range / increment / optimalVerticalValueCount);

			if (tickRatio >= 4)
			{
				increment *= 5;
			}
			else if (tickRatio > 2)
			{
				increment *= 2.5;
			}

			while (value > min)
			{
				value -= increment;
			}

			while (value <= max)
			{
				if (value >= min)
				{
					yield return value;
				}

				value += increment;
			}
		}


		private static readonly Styles.CaptionStyle	defaultStyle = new Styles.CaptionStyle ();
		
		private readonly double extraLength = 12;
		private readonly double arrowLength = 4;
		private readonly double arrowBreadth = 3;
		private readonly double tickLength = 4;

		private Styles.CaptionStyle			style;
	}
}
