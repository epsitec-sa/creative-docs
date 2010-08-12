//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;

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

		public override Adorners.HorizontalAxisMode HorizontalAxisMode
		{
			get
			{
				return Adorners.HorizontalAxisMode.Ranges;
			}
		}

		
		public override void BeginRender(IPaintPort port, Rectangle portSize, Rectangle portBounds)
		{
			this.verticalScale = portBounds.Height / (this.MaxValue - this.MinValue);
			this.horizontalScale = portBounds.Width / System.Math.Max (1, this.ValueCount);
			
			base.BeginRender (port, portSize, portBounds);
		}

		public override void BeginPass(IPaintPort port, int pass)
		{
			base.BeginPass (port, pass);
			
			this.BeginLayer (port, PaintLayer.Intermediate);
		}

		public override void EndRender(IPaintPort port)
		{
			base.EndRender (port);
		}

		public override Point GetPoint(int index, double value)
		{
			double offset = value - this.MinValue;
			Point  origin = this.Bounds.Location;

			return this.AdjustPoint (new Point (origin.X + index * this.horizontalScale, origin.Y + offset * this.verticalScale));
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

        /// <summary>
        /// Returns the caption position, centered on the bar.
        /// Position depending on the value sign:
        /// - On top for positive values
        /// - On bottom for negative values
        /// </summary>
        public override SeriesCaptionPosition GetSeriesCaptionPosition (Data.ChartSeries series, int seriesIndex)
        {
            if (series.Values.Count != 1)
                return new SeriesCaptionPosition () { ShowCaption = false };

            using (Path path = this.CreateSurfacePath (series, seriesIndex))
            {
                PathElement[] elems;
                Point[] points;

                path.GetElements (out elems, out points);

                double minX = double.MaxValue;
                double maxX = 0;
                double minMaxY = 0;

                // Take every point and get the extremums
                foreach (var p in points)
                {
                    // Some points are not correct
                    if (p.X > 10)
                        minX = System.Math.Min (minX, p.X);

                    maxX = System.Math.Max (maxX, p.X);
                    minMaxY = series.Values[0].Value >= 0 ? System.Math.Max (minMaxY, p.Y) : System.Math.Min (minMaxY, p.Y);
                }

                // Return data
                var point = new Point ()
                {
                    X = minX + (maxX - minX) / 2,
                    Y = minMaxY
                };
                ContentAlignment aligment;

                // Point depends on the value sign
                if (minMaxY >= 0)
                {
                    point.Y += BarChartRenderer.captionVerticalOffset;
                    aligment = ContentAlignment.BottomCenter;
                }
                else
                {
                    point.Y -= BarChartRenderer.captionVerticalOffset;
                    aligment = ContentAlignment.TopCenter;
                }

                return new SeriesCaptionPosition ()
                {
                    Position = point,
                    Alignment = aligment
                };
            }
        }

		protected override void Render(IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex)
		{
			if (series.Values.Count > 0)
			{
				using (Path path = this.CreateSurfacePath (series, seriesIndex))
				{
					this.FindStyle ("line-color").ApplyStyle (seriesIndex, port);
					
					port.PaintSurface (path);
					
					port.Color = this.GetOutlineColor (port.Color);
					port.LineWidth = 1;
					port.LineCap = CapStyle.Butt;
					port.LineJoin = JoinStyle.Miter;
					port.PaintOutline (path);
				}
			}
		}

		protected override System.Action<IPaintPort, Rectangle> CreateCaptionSamplePainter(Data.ChartSeries series, int seriesIndex)
		{
			return (p, r) =>
				{
					using (Path sample = new Path ())
					{
						this.FindStyle ("line-color").ApplyStyle (seriesIndex, p);
						
						double x1 = r.Left;
						double y1 = r.Center.Y - 4;
						double x2 = r.Right;
						double y2 = r.Center.Y + 4;
						
						sample.MoveTo (x1, y1);
						sample.LineTo (x2, y1);
						sample.LineTo (x2, y2);
						sample.LineTo (x1, y2);
						sample.Close ();

						p.PaintSurface (sample);

						p.Color = this.GetOutlineColor (p.Color);
						p.LineWidth = 2;
						p.LineCap = CapStyle.Butt;
						p.LineJoin = JoinStyle.Miter;
						p.PaintOutline (sample);
					}
				};
		}

		private Color GetOutlineColor(Color surfaceColor)
		{
			return Color.Mix (surfaceColor, Color.FromBrightness (0), 0.2);
		}

		
		private Path CreateSurfacePath(Data.ChartSeries series, int seriesIndex)
		{
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
				var space  = width * 0.9 - 4;
				var step   = space / this.SeriesCount;
				var offset = (width - space) / 2 + step * seriesIndex;

				switch (this.ChartSeriesRenderingMode)
				{
					case ChartSeriesRenderingMode.Stacked:
						step = space;
						offset = (width - space) / 2;
						break;
				}

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



        private static readonly int captionVerticalOffset = 5;
        private double verticalScale;
		private double horizontalScale;
	}
}
