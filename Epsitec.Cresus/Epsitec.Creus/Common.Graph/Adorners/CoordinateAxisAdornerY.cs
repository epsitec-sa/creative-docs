//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public class CoordinateAxisAdornerY : AbstractCoordinateAxisAdorner
	{
		public CoordinateAxisAdornerY()
		{
		}
		
		public override void Paint(IPaintPort port, Epsitec.Common.Graph.Renderers.AbstractRenderer renderer, PaintLayer layer)
		{
			throw new System.NotImplementedException ();
		}

		public void PaintLabels(IPaintPort port, Renderers.AbstractRenderer renderer)
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

		public void PaintTicks(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;
			Point     origin = renderer.GetPoint (0, 0.0);
			
			port.LineWidth = 1;
			port.Color = Color.FromBrightness (0);

			this.PaintVerticalAxis (port, bounds.Left, bounds.Bottom, bounds.Top + this.extraLength,
				path =>
				{
					bool first = true;

					foreach (Point point in CoordinateAxisAdornerY.GetVerticalAxisTicks (renderer, this.GetOptimalVerticalValueCount (bounds.Height)))
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
		}

		public void PaintGrid(Path path, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;

			foreach (Point point in CoordinateAxisAdornerY.GetVerticalAxisTicks (renderer, this.GetOptimalVerticalValueCount (bounds.Height)))
			{
				path.MoveTo (bounds.Left, point.Y);
				path.LineTo (bounds.Right + this.extraLength / 2, point.Y);
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

		private void PaintVerticalAxis(IPaintPort port, double x, double y1, double y2, System.Action<Path> addTicks)
		{
			using (Path path = new Path ())
			{
				path.MoveTo (x, y1);
				path.LineTo (x, y2);
				path.MoveTo (x - this.arrowBreadth/2, y2 - this.arrowLength);
				path.LineTo (x, y2);
				path.LineTo (x + this.arrowBreadth/2, y2 - this.arrowLength);

				if ((this.tickLength > 0) &&
					(this.VisibleTicks))
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

		private static IEnumerable<Point> GetVerticalAxisTicks(Renderers.AbstractRenderer renderer, int optimalVerticalValueCount)
		{
			foreach (var value in CoordinateAxisAdorner.GetLog10Values (renderer.MinValue, renderer.MaxValue, optimalVerticalValueCount))
			{
				yield return renderer.GetPoint (0, value);
			}
		}
	}
}
