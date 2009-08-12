//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Graph.Renderers
{
	public abstract class AbstractRenderer
	{
		protected AbstractRenderer()
		{
			this.seriesValuesLabelsSet = new HashSet<string> ();
			this.seriesValuesLabelsList = new List<string> ();
			this.Clear ();
		}


		public int SeriesCount
		{
			get
			{
				return this.seriesCount;
			}
		}

		public int ValuesCount
		{
			get
			{
				return this.seriesValuesLabelsList.Count;
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

		public void ClipRange(double minValue, double maxValue)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}

		public virtual void Clear()
		{
			this.seriesCount = 0;
			this.minValue = double.MaxValue;
			this.maxValue = double.MinValue;
			this.seriesValuesLabelsSet.Clear ();
			this.seriesValuesLabelsList.Clear ();
		}

		public virtual void BeginRender()
		{
			this.seriesRendered = -1;
		}

		public virtual void EndRender()
		{
		}

		public virtual void Collect(Data.ChartSeries series)
		{
			this.seriesCount++;

			int index = 0;

			foreach (var item in series.Values)
			{
				string label = item.Label;
				double value = item.Value;

				if (this.seriesValuesLabelsSet.Contains (label))
				{
					index = this.GetLabelIndex (label) + 1;
				}
				else
				{
					this.seriesValuesLabelsList.Insert (index++, label);
					this.seriesValuesLabelsSet.Add (label);
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

			System.Diagnostics.Debug.Assert (this.seriesValuesLabelsList.Count == this.seriesValuesLabelsSet.Count);
		}

		public virtual void Render(Drawing.IPaintPort port, Data.ChartSeries series)
		{
		}


		protected int GetLabelIndex(string label)
		{
			return this.seriesValuesLabelsList.IndexOf (label);
		}

		private int seriesCount;
		private int seriesRendered;
		private double minValue;
		private double maxValue;
		
		private readonly HashSet<string> seriesValuesLabelsSet;
		private readonly List<string> seriesValuesLabelsList;
	}
}
