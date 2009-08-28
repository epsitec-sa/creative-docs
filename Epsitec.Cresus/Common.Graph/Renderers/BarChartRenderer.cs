//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Renderers
{
	public class BarChartRenderer : AbstractRenderer
	{
		public BarChartRenderer()
		{
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

		public override void EndRender(IPaintPort port)
		{
			base.EndRender (port);
		}

		public override Point GetPoint(int index, double value)
		{
			double offset = value - this.MinValue;
			Point  origin = this.Bounds.Location;

			return new Point (origin.X + index * this.horizontalScale, origin.Y + offset * this.verticalScale);
		}

		public override void UpdateCaptions(IEnumerable<Data.ChartSeries> series)
		{
			this.Captions.Clear ();

			int index = 0;

			foreach (var item in series)
			{
				this.CreateCaption (item, index++);
			}
		}

		public override Path GetDetectionPath(Data.ChartSeries series, int seriesIndex, double detectionRadius)
		{
			return this.CreateSurfacePath (series, seriesIndex);
		}


		protected override void Render(IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex)
		{
			if (series.Values.Count > 1)
			{
				using (Path path = this.CreateSurfacePath (series, seriesIndex))
				{
					this.FindStyle ("line-color").ApplyStyle (seriesIndex, port);
					port.PaintSurface (path);
				}
			}
		}

		private void CreateCaption(Data.ChartSeries series, int seriesIndex)
		{
			this.Captions.AddSample (AbstractRenderer.CleanUpLabel (series.Label),
				(p, r) =>
				{
					using (Path line = new Path ())
					{
						line.MoveTo (r.Left, r.Center.Y);
						line.LineTo (r.Right, r.Center.Y);

						this.FindStyle ("line-color").ApplyStyle (seriesIndex, p);

						p.LineWidth = 4;
						p.LineCap = CapStyle.Butt;
						p.PaintOutline (line);
					}
				});
		}

		
		private Path CreateSurfacePath(Data.ChartSeries series, int seriesIndex)
		{
			double seriesOffset = (1.0 * seriesIndex) / this.SeriesCount;
			
			Path path = new Path ();

			foreach (var item in series.Values)
			{
				int index = this.GetLabelIndex (item.Label);

				System.Diagnostics.Debug.Assert (index >= 0);
				System.Diagnostics.Debug.Assert (index < this.ValueCount);

				var posZero   = this.GetPoint (index, 0.0);
				var posZeroX  = posZero.X;
				var posZeroY  = posZero.Y;
				var posNextX  = this.GetPoint (index+1, 0.0).X;
				var posValueY = this.GetPoint (index, item.Value).Y;

				var width  = posNextX - posZeroX;
				var space  = width * 0.9;
				var step   = space / this.SeriesCount;
				var offset = (width - space) / 2 + step * seriesIndex;

				posZeroX += offset;
				posNextX  = posZeroX + step;

				path.MoveTo (posZeroX, posZeroY);
				path.LineTo (posZeroX, posValueY);
				path.LineTo (posNextX, posValueY);
				path.LineTo (posNextX, posZeroY);
				path.Close ();
			}

			return path;
		}

		

		private double verticalScale;
		private double horizontalScale;
	}
}
