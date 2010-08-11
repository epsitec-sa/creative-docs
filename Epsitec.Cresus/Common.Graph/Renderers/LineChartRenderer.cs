//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;

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


        public override void BeginRender (IPaintPort port, Rectangle portSize, Rectangle portBounds)
		{
			this.verticalScale = portBounds.Height / (this.MaxValue - this.MinValue);
			this.horizontalScale = portBounds.Width / System.Math.Max (1, this.ValueCount - 1);

			if (this.MaxValue == this.MinValue)
			{
				this.verticalScale = 0;
			}
			
			base.BeginRender (port, portSize, portBounds);
		}

		public override void BeginPass(IPaintPort port, int pass)
		{
			base.BeginPass (port, pass);
			
			if (pass == 1)
			{
				this.BeginLayer (port, PaintLayer.Intermediate);
			}
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
			using (Path outline = this.CreateOutlinePath (series))
			{
				Path path = new Path ();

				path.Append (outline, detectionRadius * 2, CapStyle.Round, JoinStyle.Round, 5.0, 1.0);

				return path;
			}
		}

        /// <summary>
        /// Does not show these captions
        /// </summary>
        public override SeriesCaptionPosition GetSeriesCaptionPosition (Data.ChartSeries series, int seriesIndex)
        {
            return new SeriesCaptionPosition ()
            {
                ShowCaption = false
            };
        }

		protected override void Render(IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex)
		{
			switch (pass)
			{
				case 0:
					this.PaintSurface (port, series, seriesIndex);
					break;
				
				case 1:
					this.PaintLine (port, series, seriesIndex);
					break;
			}
		}

		
		private void PaintLine(IPaintPort port, Data.ChartSeries series, int seriesIndex)
		{
			using (Path path = this.CreateOutlinePath (series))
			{
				this.FindStyle ("line-color").ApplyStyle (seriesIndex, port);
				
				port.LineWidth = 2;
				port.PaintOutline (path);
			}
		}

		protected override System.Action<IPaintPort, Rectangle> CreateCaptionSamplePainter(Data.ChartSeries series, int seriesIndex)
		{
			return (p, r) =>
				{
					using (Path line = new Path ())
					{
						line.MoveTo (r.Left, r.Center.Y);
						line.LineTo (r.Right, r.Center.Y);

						this.FindStyle ("line-color").ApplyStyle (seriesIndex, p);

						p.LineWidth = 2;
						p.LineCap = CapStyle.Butt;
						p.PaintOutline (line);
					}
				};
		}

		private void PaintSurface(IPaintPort port, Data.ChartSeries series, int seriesIndex)
		{
			if ((series.Values.Count > 0) &&
				(this.SurfaceAlpha > 0))
			{
				using (Path path = this.CreateSurfacePath (series))
				{
					this.FindStyle ("line-color").ApplyStyle (seriesIndex, port);

					port.Color = Color.FromAlphaColor (this.SurfaceAlpha, port.Color);
					port.PaintSurface (path);
				}
			}
		}

		private Path CreateOutlinePath(Data.ChartSeries series)
		{
			Path path = new Path ();

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
				path.LineTo (this.GetPoint (1, series.Values[0].Value));
			}

			return path;
		}
		
		private Path CreateSurfacePath(Data.ChartSeries series)
		{
			Path path = new Path ();

			path.MoveTo (this.GetPoint (0, 0.0));

			foreach (var item in series.Values)
			{
				int index = this.GetLabelIndex (item.Label);

				System.Diagnostics.Debug.Assert (index >= 0);
				System.Diagnostics.Debug.Assert (index < this.ValueCount);

				path.LineTo (this.GetPoint (index, item.Value));
			}

			if (series.Values.Count == 1)
			{
				path.LineTo (this.GetPoint (1, series.Values[0].Value));
			}
			
			path.LineTo (path.CurrentPoint.X, this.GetPoint (0, 0.0).Y);
			path.Close ();

			return path;
		}

		

		private double verticalScale;
		private double horizontalScale;
	}
}
