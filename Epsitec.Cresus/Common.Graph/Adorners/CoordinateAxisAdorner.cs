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
			this.verticalLabelFont = Font.GetFont ("Arial", "Regular");
		}

		public override void Paint(IPaintPort port, Renderers.AbstractRenderer renderer, PaintLayer layer)
		{
			if (layer != PaintLayer.Intermediate)
			{
				return;
			}

			Rectangle bounds = renderer.Bounds;
			Point     origin = renderer.GetPoint (0, 0.0);

			double max = renderer.MaxValue;
			double min = renderer.MinValue;

			this.PaintGrid (port, renderer, bounds);
			this.PaintVerticalAxisLabels (port, renderer, bounds);

			port.LineWidth = 1;
			port.Color = Color.FromBrightness (0);

			this.PaintVerticalAxis (port, bounds.Left, bounds.Bottom, bounds.Top + this.extraLength,
				path =>
				{
					bool first = true;

					foreach (Point point in CoordinateAxisAdorner.GetVerticalAxisTicks (renderer))
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
						
						foreach (Point point in CoordinateAxisAdorner.GetHorizontalAxisTicks (renderer))
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

		private void PaintGrid(IPaintPort port, Renderers.AbstractRenderer renderer, Rectangle bounds)
		{
			port.LineWidth = 0.5;
			port.Color = Color.FromBrightness (0);

			using (Path path = new Path ())
			{
				foreach (Point point in CoordinateAxisAdorner.GetHorizontalAxisTicks (renderer))
				{
					path.MoveTo (point.X, bounds.Bottom);
					path.LineTo (point.X, bounds.Top + this.extraLength / 2);
				}
				
				foreach (Point point in CoordinateAxisAdorner.GetVerticalAxisTicks (renderer))
				{
					path.MoveTo (bounds.Left, point.Y);
					path.LineTo (bounds.Right + this.extraLength / 2, point.Y);
				}

				port.PaintOutline (path);
			}
		}

		private void PaintVerticalAxisLabels(IPaintPort port, Renderers.AbstractRenderer renderer, Rectangle bounds)
		{
			foreach (var value in CoordinateAxisAdorner.GetLog10Values (renderer.MinValue, renderer.MaxValue))
			{
				Point pos = renderer.GetPoint (0, value);
				string text = string.Format (System.Globalization.CultureInfo.CurrentCulture, "{0}  ", value);
				double len  = this.verticalLabelFont.GetTextAdvance (text) * this.verticalLabelFontSize;

				port.PaintText (pos.X - len, pos.Y, text, this.verticalLabelFont, this.verticalLabelFontSize);
			}
		}


		private static IEnumerable<Point> GetHorizontalAxisTicks(Renderers.AbstractRenderer renderer)
		{
			int count = renderer.ValuesCount;

			for (int i = 0; i < count; i++)
			{
				yield return renderer.GetPoint (i, 0.0);
			}
		}

		private static IEnumerable<Point> GetVerticalAxisTicks(Renderers.AbstractRenderer renderer)
		{
			foreach (var value in CoordinateAxisAdorner.GetLog10Values (renderer.MinValue, renderer.MaxValue))
			{
				yield return renderer.GetPoint (0, value);
			}
		}

		private static IEnumerable<double> GetLog10Values(double min, double max)
		{
			double range = max - min;
			double logRange = System.Math.Floor (System.Math.Log10 (range));
			double adjRange = System.Math.Pow (10, logRange);

			for (int i = 0; ; i++)
			{
				double value = adjRange * i/10;

				if (value > max)
				{
					break;
				}

				if (value >= min)
				{
					yield return value;
				}
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


		private readonly double extraLength = 12;
		private readonly double arrowLength = 4;
		private readonly double arrowBreadth = 3;
		private readonly double tickLength = 4;
		private readonly Font verticalLabelFont;
		private readonly double verticalLabelFontSize = 9;
	}
}
