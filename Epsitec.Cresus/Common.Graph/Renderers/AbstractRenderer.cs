﻿//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;

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
			this.seriesOriginals = new List<Data.ChartSeries> ();
			this.seriesList = new List<Data.ChartSeries> ();
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
				if (this.AlwaysIncludeZero)
				{
					return System.Math.Min (this.minValue, 0.0);
				}
				else
				{
					return this.minValue;
				}
			}
		}

		public double MaxValue
		{
			get
			{
				if (this.AlwaysIncludeZero)
				{
					return System.Math.Max (this.maxValue, 0.0);
				}
				else
				{
					return this.maxValue;
				}
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

		public IEnumerable<KeyValuePair<string, Styles.AbstractStyle>> Styles
		{
			get
			{
				return this.styles;
			}
		}

		public CaptionPainter Captions
		{
			get
			{
				return this.captions;
			}
		}

		public virtual Adorners.HorizontalAxisMode HorizontalAxisMode
		{
			get
			{
				return Adorners.HorizontalAxisMode.Ticks;
			}
		}

		public bool AlwaysIncludeZero
		{
			get;
			set;
		}

		public ChartSeriesRenderingMode ChartSeriesRenderingMode
		{
			get;
			set;
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

		public virtual void BeginPass(IPaintPort port, int pass)
		{
		}

		public virtual void EndRender(IPaintPort port)
		{
			this.BeginLayer (port, PaintLayer.Foreground);
		}

		public virtual void Collect(Data.ChartSeries series)
		{
			if (series == null)
			{
				return;
			}

			this.seriesOriginals.Add (series);
			this.seriesList.Add (series = this.GetPreprocessedSeries (series));

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

		private Data.ChartSeries GetPreprocessedSeries(Data.ChartSeries series)
		{
			switch (this.ChartSeriesRenderingMode)
			{
				case ChartSeriesRenderingMode.Separate:
					return series;

				case ChartSeriesRenderingMode.Stacked:
					return new Data.ChartSeries (series.Label, AbstractRenderer.GetStackedValues (this.seriesOriginals.Select (x => x.Values)));

				default:
					throw new System.InvalidOperationException ();
			}
		}

		private static IList<Data.ChartValue> GetStackedValues(IEnumerable<IList<Data.ChartValue>> collection)
		{
			var lists = new List<IList<Data.ChartValue>> (collection);
			var labelsDict = new Dictionary<string, double> ();

			foreach (var list in lists)
			{
				foreach (var item in list)
				{
					double value;

					if (labelsDict.TryGetValue (item.Label, out value))
					{
						labelsDict[item.Label] = value + item.Value;
					}
					else
					{
						labelsDict.Add (item.Label, item.Value);
					}
				}
			}

			return new List<Data.ChartValue> (
				from kv in labelsDict
				let key = kv.Key
				let value = kv.Value
				orderby key ascending
				select new Data.ChartValue (key, value));
		}


		public void CollectRange(IEnumerable<Data.ChartSeries> collection)
		{
			collection.ForEach (item => this.Collect (item));
		}

		public void Render(IPaintPort port, Rectangle bounds)
		{
			this.Render (this.SeriesItems, port, bounds);
		}

		public void Render(IEnumerable<Data.ChartSeries> series, IPaintPort port, Rectangle bounds)
		{
			this.UpdateCaptions (series);

			this.BeginRender (port, bounds);

			int start     = 0;
			int increment = 1;

			switch (this.ChartSeriesRenderingMode)
			{
				case ChartSeriesRenderingMode.Stacked:
					series    = series.Reverse ();
					increment = -1;
					start     = series.Count () - 1;
					break;
			}
			
			//	The rendering might take several passes, for instance to paint several layers
			//	and maybe insert a grid between them, or other graphic adornments.

			if (series.IsEmpty ())
			{
				//	Do nothing if there is no value in the series.
			}
			else
			{
				for (int pass = 0; pass < 1+this.AdditionalRenderingPasses; pass++)
				{
					this.BeginPass (port, pass);

					int seriesIndex = start;

					foreach (var item in series)
					{
						this.Render (port, item, pass, seriesIndex);
						seriesIndex += increment;
					}
				}
			}

			this.EndRender (port);
		}

		public abstract void UpdateCaptions(IEnumerable<Data.ChartSeries> series);

		public abstract Point GetPoint(int index, double value);

		public Point AdjustPoint(Point point)
		{
			return Point.GridAlign (point, 0.5, 1.0);
		}

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
			int pos = label.LastIndexOf (Data.DataCube.LabelSortPrefixSeparator);

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
		private readonly List<Data.ChartSeries> seriesOriginals;
		private readonly List<Data.ChartSeries> seriesList;
		private readonly CaptionPainter captions;
	}
}
