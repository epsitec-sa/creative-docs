//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Renderers
{
	public abstract class AbstractRenderer
	{
		protected AbstractRenderer()
		{
			this.seriesValueLabelsSet = new HashSet<string> ();
			this.seriesValueLabelsList = new List<string> ();
			this.styles = new Dictionary<string, Styles.AbstractStyle> ();
			this.adorners = new List<Adorners.AbstractAdorner> ();
			this.Clear ();
		}


		public int SeriesCount
		{
			get
			{
				return this.seriesCount;
			}
		}

		public int ValueCount
		{
			get
			{
				return this.seriesValueLabelsList.Count;
			}
		}

		public double MinValue
		{
			get
			{
				return this.minValue;
			}
		}

		public double MaxValue
		{
			get
			{
				return this.maxValue;
			}
		}

		public Rectangle Bounds
		{
			get
			{
				return this.bounds;
			}
		}

		public int CurrentSeriesIndex
		{
			get
			{
				return this.seriesRendered;
			}
		}

		public int AdditionalRenderingPasses
		{
			get;
			protected set;
		}

		public IEnumerable<string> ValueLabels
		{
			get
			{
				return this.seriesValueLabelsList;
			}
		}

		
		public void AddStyle(Styles.AbstractStyle style)
		{
			this.styles.Add (style.Name, style);
		}

		public void AddAdorner(Adorners.AbstractAdorner adorner)
		{
			this.adorners.Add (adorner);
		}

		public Styles.AbstractStyle FindStyle(string name)
		{
			Styles.AbstractStyle style;

			this.styles.TryGetValue (name, out style);

			return style;
		}

		public void ClipRange(double minValue, double maxValue)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		public virtual void Clear()
		{
			this.seriesCount = 0;
			this.seriesRendered = -1;

			this.minValue = double.MaxValue;
			this.maxValue = double.MinValue;
			this.seriesValueLabelsSet.Clear ();
			this.seriesValueLabelsList.Clear ();
		}

		public virtual void BeginRender(IPaintPort port, Rectangle bounds)
		{
			this.bounds = bounds;
			this.BeginLayer (port, PaintLayer.Background);
		}

		public virtual void EndRender(IPaintPort port)
		{
			this.BeginLayer (port, PaintLayer.Foreground);
		}

		public virtual void Collect(Data.ChartSeries series)
		{
			this.seriesCount++;

			int index = 0;

			foreach (var item in series.Values)
			{
				string label = item.Label;
				double value = item.Value;

				if (this.seriesValueLabelsSet.Contains (label))
				{
					index = this.GetLabelIndex (label) + 1;
				}
				else
				{
					this.seriesValueLabelsList.Insert (index++, label);
					this.seriesValueLabelsSet.Add (label);
				}

				if (value < this.minValue)
				{
					this.minValue = value;
				}
				if (value > this.maxValue)
				{
					this.maxValue = value;
				}
			}

			System.Diagnostics.Debug.Assert (this.seriesValueLabelsList.Count == this.seriesValueLabelsSet.Count);
		}

		public void Render(IEnumerable<Data.ChartSeries> series, IPaintPort port, Rectangle bounds)
		{
			this.BeginRender (port, bounds);

			//	The rendering might take several passes, for instance to paint several layers
			//	and maybe insert a grid between them, or other graphic adornments.
			
			for (int pass = 0; pass < this.AdditionalRenderingPasses+1; pass++)
			{
				this.seriesRendered = -1;

				foreach (var item in series)
				{
					this.seriesRendered++;
					this.Render (port, item, pass);
				}
			}

			this.EndRender (port);
		}

		public virtual void RenderCaptions(IPaintPort port, Rectangle bounds)
		{
		}

		public abstract Point GetPoint(int index, double value);


		protected abstract void Render(IPaintPort port, Data.ChartSeries series, int pass);

		protected void BeginLayer(IPaintPort port, PaintLayer layer)
		{
			foreach (var adorner in this.adorners)
			{
				adorner.Paint (port, this, layer);
			}
		}

		protected int GetLabelIndex(string label)
		{
			return this.seriesValueLabelsList.IndexOf (label);
		}

		
		private int seriesCount;
		private int seriesRendered;
		private double minValue;
		private double maxValue;
		private Rectangle bounds;
		
		private readonly HashSet<string> seriesValueLabelsSet;
		private readonly List<string> seriesValueLabelsList;
		private readonly Dictionary<string, Styles.AbstractStyle> styles;
		private readonly List<Adorners.AbstractAdorner> adorners;
	}
}
