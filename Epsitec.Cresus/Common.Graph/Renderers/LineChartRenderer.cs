//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Renderers
{
	public class LineChartRenderer : AbstractRenderer
	{
		public LineChartRenderer()
		{
			this.AdditionalRenderingPasses = 1;
		}

		
		public double SurfaceAlpha
		{
			get;
			set;
		}

		
		public override void BeginRender(IPaintPort port, Rectangle bounds)
		{
			this.verticalScale = bounds.Height / (this.MaxValue - this.MinValue);
			this.horizontalScale = bounds.Width / System.Math.Max (1, this.ValueCount - 1);
			
			base.BeginRender (port, bounds);
		}

		protected override void Render(IPaintPort port, Data.ChartSeries series, int pass)
		{
			base.Render (port, series, pass);

			switch (pass)
			{
				case 0:
					if (this.SurfaceAlpha > 0)
					{
						this.PaintSurface (port, series);
					}
					break;
				
				case 1:
					this.PaintLine (port, series);
					break;
			}

			this.PaintLine (port, series);
		}

		private void PaintLine(IPaintPort port, Data.ChartSeries series)
		{
			if (this.CurrentSeriesIndex == 0)
			{
				this.BeginLayer (port, PaintLayer.Intermediate);
			}

			using (Path path = new Path ())
			{
				foreach (var item in series.Values)
				{
					int index = this.GetLabelIndex (item.Label);

					System.Diagnostics.Debug.Assert (index >= 0);
					System.Diagnostics.Debug.Assert (index < this.ValueCount);

					Point pos = this.GetPoint (index, item.Value);

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

		private void PaintSurface(IPaintPort port, Data.ChartSeries series)
		{
			if (series.Values.Count > 1)
			{
				using (Path path = new Path ())
				{
					path.MoveTo (this.GetPoint (0, 0.0));

					foreach (var item in series.Values)
					{
						int index = this.GetLabelIndex (item.Label);

						System.Diagnostics.Debug.Assert (index >= 0);
						System.Diagnostics.Debug.Assert (index < this.ValueCount);

						path.LineTo (this.GetPoint (index, item.Value));
					}

					path.LineTo (path.CurrentPoint.X, this.GetPoint (0, 0.0).Y);
					path.Close ();

					this.FindStyle ("line-color").ApplyStyle (this.CurrentSeriesIndex, port);

					port.Color = Color.FromAlphaColor (this.SurfaceAlpha, port.Color);
					port.PaintSurface (path);
				}
			}
		}

		public override Point GetPoint(int index, double value)
		{
			double offset = value - this.MinValue;
			Point  origin = this.Bounds.Location;

			return new Point (origin.X + index * this.horizontalScale, origin.Y + offset * this.verticalScale);
		}


		private double verticalScale;
		private double horizontalScale;
	}
}
