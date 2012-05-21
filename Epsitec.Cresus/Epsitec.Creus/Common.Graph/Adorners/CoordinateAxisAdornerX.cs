//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public class CoordinateAxisAdornerX : AbstractCoordinateAxisAdorner
	{
		public CoordinateAxisAdornerX()
		{
		}
		
		public override void Paint(IPaintPort port, Epsitec.Common.Graph.Renderers.AbstractRenderer renderer, PaintLayer layer)
		{
			throw new System.NotImplementedException ();
		}

		public void PaintLabels(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			var horizontalAxisMode = renderer.HorizontalAxisMode;

			if (renderer.ValueCount > 0)
			{
				Rectangle bounds = renderer.Bounds;

				var  style = this.Style;
				port.Color = style.FontColor;

				double max = renderer.MaxValue;
				double min = renderer.MinValue;

				int count = horizontalAxisMode == HorizontalAxisMode.Ticks ? renderer.ValueCount-1 : renderer.ValueCount;

				double dx = bounds.Width / count;
				double x  = bounds.Left;
				double y  = (0.0 <= max) && (0.0 >= min) ? renderer.GetPoint (0, 0.0).Y : bounds.Bottom;
				double h  = style.Font.LineHeight * style.FontSize;

				if (horizontalAxisMode == HorizontalAxisMode.Ranges)
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

		public void PaintTicks(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;
			Point     origin = renderer.GetPoint (0, 0.0);
			
			port.LineWidth = 1;
			port.Color = Color.FromBrightness (0);

			double max = renderer.MaxValue;
			double min = renderer.MinValue;

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

		public void PaintGrid(Path path, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;
			
			foreach (Point point in this.GetHorizontalAxisTicks (renderer))
			{
				path.MoveTo (point.X, bounds.Bottom);
				path.LineTo (point.X, bounds.Top + this.extraLength / 2);
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

		private IEnumerable<Point> GetHorizontalAxisTicks(Renderers.AbstractRenderer renderer)
		{
			var horizontalAxisMode = renderer.HorizontalAxisMode;
			int count = renderer.ValueCount;

			if (horizontalAxisMode == HorizontalAxisMode.Ranges)
			{
				count++;
			}

			for (int i = 0; i < count; i++)
			{
				yield return renderer.GetPoint (i, 0.0);
			}
		}
	}
}
