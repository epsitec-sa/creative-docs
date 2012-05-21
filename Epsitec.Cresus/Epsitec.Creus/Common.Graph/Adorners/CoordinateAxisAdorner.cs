//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public class CoordinateAxisAdorner : AbstractCoordinateAxisAdorner
	{
		public CoordinateAxisAdorner()
		{
		}


		public AbstractCoordinateAxisAdorner HorizontalAxisAdorner
		{
			get
			{
				return this.horizontalAxisAdorner;
			}
		}

		public AbstractCoordinateAxisAdorner VerticalAxisAdorner
		{
			get
			{
				return this.verticalAxisAdorner;
			}
		}


		public override Styles.CaptionStyle Style
		{
			get
			{
				return base.Style;
			}
			set
			{
				base.Style = value;
				this.horizontalAxisAdorner.Style = value;
				this.verticalAxisAdorner.Style = value;
			}
		}

		public override Color GridColor
		{
			get
			{
				return base.GridColor;
			}
			set
			{
				base.GridColor = value;
				this.horizontalAxisAdorner.GridColor = value;
				this.verticalAxisAdorner.GridColor = value;
			}
		}


		public override double GridLineWidth
		{
			get
			{
				return base.GridLineWidth;
			}
			set
			{
				base.GridLineWidth = value;
				this.horizontalAxisAdorner.GridLineWidth = value;
				this.verticalAxisAdorner.GridLineWidth = value;
			}
		}

		public override bool VisibleGrid
		{
			get
			{
				return base.VisibleGrid;
			}
			set
			{
				base.VisibleGrid = value;
				this.horizontalAxisAdorner.VisibleGrid = value;
				this.verticalAxisAdorner.VisibleGrid = value;
			}
		}

		public override bool VisibleLabels
		{
			get
			{
				return base.VisibleLabels;
			}
			set
			{
				base.VisibleLabels = value;
				this.horizontalAxisAdorner.VisibleLabels = value;
				this.verticalAxisAdorner.VisibleLabels = value;
			}
		}

		public override bool VisibleTicks
		{
			get
			{
				return base.VisibleTicks;
			}
			set
			{
				base.VisibleTicks = value;
				this.horizontalAxisAdorner.VisibleTicks = value;
				this.verticalAxisAdorner.VisibleTicks = value;
			}
		}

		
		public override void Paint(IPaintPort port, Renderers.AbstractRenderer renderer, PaintLayer layer)
		{
			if (layer != PaintLayer.Intermediate)
			{
				return;
			}

			if ((this.horizontalAxisAdorner.VisibleGrid) ||
				(this.verticalAxisAdorner.VisibleGrid))
            {
				this.PaintGrid (port, renderer);
            }

			if (this.horizontalAxisAdorner.VisibleLabels)
			{
				this.horizontalAxisAdorner.PaintLabels (port, renderer);
			}
			if (this.verticalAxisAdorner.VisibleLabels)
			{
				this.verticalAxisAdorner.PaintLabels (port, renderer);
			}

			if (this.horizontalAxisAdorner.VisibleTicks)
			{
				this.horizontalAxisAdorner.PaintTicks (port, renderer);
			}
			if (this.verticalAxisAdorner.VisibleTicks)
			{
				this.verticalAxisAdorner.PaintTicks (port, renderer);
			}
		}

		private void PaintGrid(IPaintPort port, Renderers.AbstractRenderer renderer)
		{
			Rectangle bounds = renderer.Bounds;

			if ((this.horizontalAxisAdorner.VisibleGrid) &&
				(this.verticalAxisAdorner.VisibleGrid) &&
				(this.GridLineWidth == this.horizontalAxisAdorner.GridLineWidth) &&
				(this.GridLineWidth == this.verticalAxisAdorner.GridLineWidth) &&
				(this.GridColor == this.horizontalAxisAdorner.GridColor) &&
				(this.GridColor == this.verticalAxisAdorner.GridColor))
			{
				//	Both horizontal and vertical grid lines have the same color and
				//	width : optimize and paint the outline in one pass; this avoids
				//	intersection artifacts.

				port.LineWidth = this.GridLineWidth;
				port.Color     = this.GridColor;

				using (Path path = new Path ())
				{
					this.horizontalAxisAdorner.PaintGrid (path, renderer);
					this.verticalAxisAdorner.PaintGrid (path, renderer);

					port.PaintOutline (path);
				}
			}
			else
			{
				//	Draw horizontal lines, then vertical lines. Each with their
				//	own setting.

				if (this.horizontalAxisAdorner.VisibleGrid)
				{
					port.LineWidth = this.horizontalAxisAdorner.GridLineWidth;
					port.Color     = this.horizontalAxisAdorner.GridColor;

					using (Path path = new Path ())
					{
						this.horizontalAxisAdorner.PaintGrid (path, renderer);

						port.PaintOutline (path);
					}
				}

				if (this.verticalAxisAdorner.VisibleGrid)
				{
					port.LineWidth = this.verticalAxisAdorner.GridLineWidth;
					port.Color     = this.verticalAxisAdorner.GridColor;

					using (Path path = new Path ())
					{
						this.verticalAxisAdorner.PaintGrid (path, renderer);

						port.PaintOutline (path);
					}
				}
			}
		}

		internal static IEnumerable<double> GetLog10Values(double min, double max, int optimalVerticalValueCount)
		{
			double range     = max - min;

			if (range <= 0)
			{
				yield return min;
				yield break;
			}

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


		private readonly CoordinateAxisAdornerX		horizontalAxisAdorner = new CoordinateAxisAdornerX ();
		private readonly CoordinateAxisAdornerY		verticalAxisAdorner   = new CoordinateAxisAdornerY ();
	}
}
