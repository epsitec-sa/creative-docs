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
			this.seriesList = new List<Epsitec.Common.Graph.Data.ChartSeries> ();
			this.captions = new CaptionPainter ();
			this.Clear ();
		}


		public int SeriesCount
		{
			get
			{
				return this.seriesList.Count;
			}
		}

		public IList<Data.ChartSeries> SeriesItems
		{
			get
			{
				return this.seriesList.AsReadOnly ();
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

		public int AdditionalRenderingPasses
		{
			get;
			protected set;
		}

		public IEnumerable<string> ValueLabels
		{
			get
			{
				return this.seriesValueLabelsList.Select (x => AbstractRenderer.CleanUpLabel (x));
			}
		}

		public CaptionPainter Captions
		{
			get
			{
				return this.captions;
			}
		}


		public void DefineValueLabels(IEnumerable<string> collection)
		{
			if (this.seriesList.Count > 0)
			{
				throw new System.InvalidOperationException ("Cannot define value labels, there are already some series defined");
			}

			this.seriesValueLabelsList.Clear ();
			this.seriesValueLabelsSet.Clear ();

			foreach (var item in collection)
			{
				this.seriesValueLabelsList.Add (item);
				this.seriesValueLabelsSet.Add (item);
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
			this.seriesList.Clear ();
			
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
			this.seriesList.Add (series);

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

		public void CollectRange(IEnumerable<Data.ChartSeries> collection)
		{
			foreach (var item in collection)
			{
				this.Collect (item);
			}
		}

		public void Render(IEnumerable<Data.ChartSeries> series, IPaintPort port, Rectangle bounds)
		{
			this.UpdateCaptions (series);

			this.BeginRender (port, bounds);

			//	The rendering might take several passes, for instance to paint several layers
			//	and maybe insert a grid between them, or other graphic adornments.
			
			for (int pass = 0; pass < 1+this.AdditionalRenderingPasses; pass++)
			{
				int seriesIndex = 0;

				foreach (var item in series)
				{
					this.Render (port, item, pass, seriesIndex++);
				}
			}

			this.EndRender (port);
		}

		public abstract void UpdateCaptions(IEnumerable<Data.ChartSeries> series);

		public abstract Point GetPoint(int index, double value);

		public abstract Path GetDetectionPath(Data.ChartSeries series, int seriesIndex, double detectionRadius);


		protected abstract void Render(IPaintPort port, Data.ChartSeries series, int pass, int seriesIndex);

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

		protected static string CleanUpLabel(string label)
		{
			int pos = label.LastIndexOf ("¦");

			if (pos < 0)
			{
				return label;
			}
			else
			{
				return label.Substring (pos+1);
			}
		}

		private double minValue;
		private double maxValue;
		private Rectangle bounds;
		
		private readonly HashSet<string> seriesValueLabelsSet;
		private readonly List<string> seriesValueLabelsList;
		private readonly Dictionary<string, Styles.AbstractStyle> styles;
		private readonly List<Adorners.AbstractAdorner> adorners;
		private readonly List<Data.ChartSeries> seriesList;
		private readonly CaptionPainter captions;
	}
}
