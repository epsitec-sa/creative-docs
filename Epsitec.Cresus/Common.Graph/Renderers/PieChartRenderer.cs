//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Graph;
using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;
using System;

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

			int rows = PieChartRenderer.GetRowCount (bounds.Size, this.pies.Count);
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
						pie.Radius = d / 2;
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
			return null;
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
			}
		}

		
		private void PaintLine(IPaintPort port, Data.ChartSeries series, int seriesIndex)
		{
			using (Path path = this.CreateOutlinePath (series, seriesIndex))
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

		private static int GetRowCount(Size size, int count)
		{
			if (count < 1)
            {
				return 1;
            }

			if (size.Width > size.Height)
			{
				return PieChartRenderer.GetStripeCount (size.Width, size.Height, count);
			}
			else
			{
				return (int) System.Math.Ceiling (count / (double) PieChartRenderer.GetStripeCount (size.Height, size.Width, count));
			}
		}


		private static int GetStripeCount(double breadth, double depth, int count)
		{
			int stripeCount = 2;

			while (System.Math.Floor (breadth / count * stripeCount)*stripeCount <= depth)
			{
				stripeCount++;
			}

			return stripeCount-1;
		}

		private List<SeriesPie> pies;
	}
}
