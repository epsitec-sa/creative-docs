//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Graph.Data;

namespace Epsitec.Common.Graph.Renderers
{
	public class PieChartRenderer : AbstractRenderer
	{
		public PieChartRenderer()
		{
			this.AdditionalRenderingPasses = 1;
		}

		
		public override void Collect(ChartSeries series)
		{
			base.Collect (series);
			this.pies = null;
		}
		
		public override void BeginRender(IPaintPort port, Rectangle bounds)
		{
			this.UpdatePies ();

			int rows = PieChartRenderer.GetRowCount (bounds.Width, bounds.Height, this.pies.Count);
			int cols = this.pies.Count == 0  ? 1 : (int) System.Math.Ceiling ((double) this.pies.Count / rows);

			double dx = bounds.Width / cols;
			double dy = bounds.Height / rows;
			double d = System.Math.Min (dx, dy);

			int pieIndex = 0;

			for (int i = 0; i < rows; i++)
			{
				var y = bounds.Height - dy/2 - i*dy;

				for (int j = 0; j < cols; j++)
				{
					var x = dx/2 + j*dx;
					var pie = pieIndex < this.pies.Count ? this.pies[pieIndex] : null;

					if (pie != null)
					{
						pie.Center = new Point (x, y);
						pie.Radius = 0.9 * d / 2;
					}

					pieIndex++;
				}
			}

			base.BeginRender (port, bounds);
		}

		public override void BeginPass(IPaintPort port, int pass)
		{
			base.BeginPass (port, pass);
//-			this.BeginLayer (port, PaintLayer.Intermediate);
		}

		public override void EndRender(IPaintPort port)
		{
			base.EndRender (port);
		}

		public override Point GetPoint(int index, double value)
		{
			throw new System.NotImplementedException ();
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
            var path = this.CreateOutlinePath(series, seriesIndex);   
            return path;
		}

        public override Point GetFloatingLabelPosition(Data.ChartSeries series, int seriesIndex)
        {
            foreach (var item in series.Values)
            {
                var label = item.Label;
                var pie = this.pies.FirstOrDefault(x => x.Label == label);

                if ((pie == null) ||
                    (seriesIndex >= pie.Sectors.Count))
                {
                    continue;
                }

                var sector = pie.Sectors[seriesIndex];
                var center = pie.Center + this.Bounds.Location;
                var radius = pie.Radius;

                // N'affiche pas les légendes pour de petits angles
                if ((sector.Angle2 - sector.Angle1) < angleToHide)
                {
                    continue;
                }

                var semiAngle = sector.Angle1 + (sector.Angle2 - sector.Angle1) / 2;

                var txtCenter = center + new Point(radius * System.Math.Cos(Math.DegToRad(semiAngle)), radius * System.Math.Sin(Math.DegToRad(semiAngle)));

                // Cette partie doit être décalée
                if (seriesIndex == this.actualIndex)
                {
                    txtCenter.X += radius * this.radiusProportion * System.Math.Cos(Math.DegToRad(semiAngle));
                    txtCenter.Y += radius * this.radiusProportion * System.Math.Sin(Math.DegToRad(semiAngle));
                }

                return txtCenter;
            }

            return new Point(0, 0);
        }


		protected override void Render(IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex)
		{
			switch (pass)
			{
				case 0:
					this.PaintSurface (port, series, seriesIndex);
					break;
				
				case 1:
					this.PaintOutline (port, series, seriesIndex);
					break;

                default:
                    break;
			}
		}

        public override void HoverIndexChanged(object oldValue, object newValue)
        {
            this.actualIndex = (int) newValue;
        }

		
		private void PaintLine(IPaintPort port, Data.ChartSeries series, int seriesIndex)
        {
            using (Path path = this.CreateOutlinePath(series, seriesIndex))
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

					p.Color = Color.FromBrightness (1);
					p.LineWidth = 1;
					p.LineCap = CapStyle.Butt;
					p.LineJoin = JoinStyle.Miter;
					p.PaintOutline (sample);
				}
			};
		}

		private void PaintSurface(IPaintPort port, Data.ChartSeries series, int seriesIndex)
		{
			if (series.Values.Count > 0)
			{
				using (Path path = this.CreateOutlinePath (series, seriesIndex))
				{
					this.FindStyle ("line-color").ApplyStyle (seriesIndex, port);
					port.PaintSurface (path);
				}
			}
		}

		private void PaintOutline(IPaintPort port, Data.ChartSeries series, int seriesIndex)
		{
			if (series.Values.Count > 0)
			{
                using (Path path = this.CreateOutlinePath (series, seriesIndex))
				{
					port.Color = Color.FromBrightness (1);
					port.LineWidth = 1;
					port.PaintOutline (path);
				}
			}
		}

		private Path CreateOutlinePath(Data.ChartSeries series, int seriesIndex)
		{
			Path path = new Path ();

			foreach (var item in series.Values)
			{
				var label  = item.Label;
				var pie    = this.pies.FirstOrDefault (x => x.Label == label);

				if ((pie == null) ||
					(seriesIndex >= pie.Sectors.Count))
				{
					continue;
				}

				var sector = pie.Sectors[seriesIndex];
				var center = pie.Center + this.Bounds.Location;
				var radius = pie.Radius;

				if ((sector.Angle2 - sector.Angle1) < 0.1)
				{
					continue;
				}

                if (seriesIndex == this.actualIndex)
                {
                    var semiAngle = sector.Angle1 + (sector.Angle2 - sector.Angle1) / 2;
                    center.X += radius * this.radiusProportion * System.Math.Cos(Math.DegToRad(semiAngle));
                    center.Y += radius * this.radiusProportion * System.Math.Sin(Math.DegToRad(semiAngle));
                }
				
				path.MoveTo (center);
				path.ArcToDeg (center, radius, radius, sector.Angle1, sector.Angle2, true);
				path.Close ();
			}

			return path;
		}

        private void UpdatePies()
		{
			if (this.pies == null)
			{
				this.UpdateCollectPies ();
				this.pies.ForEach (pie => pie.UpdateSectors ());
			}
		}

		private void UpdateCollectPies()
		{
			this.pies = new List<SeriesPie> ();
			
			var piesMap = new Dictionary<string, SeriesPie> ();

			foreach (var series in this.SeriesItems)
			{
				foreach (var value in series.Values)
				{
					string label = value.Label;
					SeriesPie pie;

					if (piesMap.TryGetValue (label, out pie) == false)
					{
						pie = piesMap[label] = new SeriesPie (label);
						this.pies.Add (pie);
					}

					pie.Sectors.Add (
						new SeriesSector ()
						{
							Value = value
						});
				}
			}
		}

		class SeriesSector
		{
			public double Angle1
			{
				get;
				set;
			}
			
			public double Angle2
			{
				get;
				set;
			}
			
			public ChartValue Value
			{
				get;
				set;
			}
		}

		class SeriesPie
		{
			public SeriesPie(string label)
			{
				this.label = label;
				this.sectors = new List<SeriesSector> ();
			}

			public string Label
			{
				get
				{
					return this.label;
				}
			}

			public Point Center
			{
				get;
				set;
			}

			public double Radius
			{
				get;
				set;
			}

			public List<SeriesSector> Sectors
			{
				get
				{
					return this.sectors;
				}
			}

			public void UpdateSectors()
			{
				double total = 0;
				
				foreach (var sector in this.sectors)
				{
					total += System.Math.Abs (sector.Value.Value);
				}

				double angle = 0;
				double scale = (total == 0) ? 0 : 360 / total;

				foreach (var sector in this.sectors)
				{
					double offset = System.Math.Abs (sector.Value.Value) * scale;

					sector.Angle1 = angle;
					sector.Angle2 = angle = angle + offset;
				}
			}

            private readonly string label;
			private readonly List<SeriesSector> sectors;
		}

		private static int GetRowCount(double dx, double dy, int count)
		{
			int bestRowCount = 1;
			var bestSize     = 0.0;

			for (int rows = 1; rows < count; rows++)
			{
				int cols = (count + rows - 1) / rows;
				var itemDx = dx / cols;
				var itemDy = dy / rows;
				var itemSize = System.Math.Floor (System.Math.Min (itemDx, itemDy));
				if (itemSize > bestSize)
				{
					bestRowCount = rows;
					bestSize = itemSize;
				}
			}

			return bestRowCount;
		}

        private List<SeriesPie> pies;

        private const int angleToHide = 6;
        private int actualIndex = -1;
        private double radiusProportion = 0.15;
	}
}
