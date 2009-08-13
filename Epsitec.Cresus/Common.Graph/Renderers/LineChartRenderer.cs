﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Renderers
{
	public class LineChartRenderer : AbstractRenderer
	{
		public override void BeginRender(Rectangle bounds)
		{
			base.BeginRender (bounds);

			this.verticalScale = bounds.Height / (this.MaxValue - this.MinValue);
			this.horizontalScale = bounds.Width / System.Math.Max (1, this.ValuesCount - 1); 
		}

		public override void EndRender()
		{
			base.EndRender ();
		}

		public override void Render(IPaintPort port, Data.ChartSeries series)
		{
			base.Render (port, series);

			int valuesCount = this.ValuesCount;

			using (Path path = new Path ())
			{
				foreach (var item in series.Values)
				{
					int index = this.GetLabelIndex (item.Label);

					System.Diagnostics.Debug.Assert (index >= 0);
					System.Diagnostics.Debug.Assert (index < this.ValuesCount);

					Point pos = this.GetLocation (index, item.Value);

					if (path.IsEmpty)
					{
						path.MoveTo (pos);
					}
					else
					{
						path.LineTo (pos);
					}
				}

				if (series.Values.Count == 1)
				{
					path.LineTo (path.CurrentPoint.X + port.LineWidth, path.CurrentPoint.Y);
				}

				this.FindStyle ("line-color").ApplyStyle (this.CurrentSeriesIndex, port);
				port.LineWidth = 2;
				port.PaintOutline (path);
			}
		}

		private Point GetLocation(int index, double value)
		{
			double offset = value - this.MinValue;
			Point  origin = this.Bounds.Location;

			return new Point (origin.X + index * this.horizontalScale, origin.Y + offset * this.verticalScale);
		}


		private double verticalScale;
		private double horizontalScale;
	}
}
