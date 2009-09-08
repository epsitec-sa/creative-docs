//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Graph.Data;

namespace Epsitec.Cresus.Graph
{
	public sealed class GraphDataGroup : System.IDisposable
	{
		public GraphDataGroup()
		{
			this.dataSeries = new List<GraphDataSeries> ();
			this.syntheticDataSeries = new List<GraphSyntheticDataSeries> ();
		}


		public string Name
		{
			get;
			set;
		}

		public int Count
		{
			get
			{
				return this.dataSeries.Count;
			}
		}

		public IEnumerable<GraphDataSeries> InputDataSeries
		{
			get
			{
				return this.dataSeries;
			}
		}

		public IEnumerable<GraphSyntheticDataSeries> SyntheticDataSeries
		{
			get
			{
				return this.syntheticDataSeries;
			}
		}

		
		public void Add(GraphDataSeries series)
		{
			if (series != null)
			{
				series.AddDataGroup (this);
				this.dataSeries.Add (series);
			}
		}

		public bool Remove(GraphDataSeries series)
		{
			if ((series != null) &&
				(this.dataSeries.Remove (series)))
			{
				series.RemoveDataGroup (this);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			this.dataSeries.ForEach (x => x.RemoveDataGroup (this));
			this.dataSeries.Clear ();
		}

		public bool Contains(GraphDataSeries series)
		{
			return this.dataSeries.Contains (series);
		}


		public void ClearSyntheticDataSeries()
		{
			this.syntheticDataSeries.Clear ();
		}

		public GraphSyntheticDataSeries AddSyntheticDataSeries(string label, System.Func<IList<ChartValue>, ChartValue> function)
		{
			var series = this.Synthesize (label, function);
			this.syntheticDataSeries.Add (series);
			return series;
		}


		private GraphSyntheticDataSeries Synthesize(string label, System.Func<IList<ChartValue>, ChartValue> function)
		{
			int n = this.dataSeries.Count;
			int m = this.dataSeries[0].ChartSeries.Values.Count;
			
			ChartValue[] inputVector = new ChartValue[n];
			ChartValue[] outputVector = new ChartValue[m];

			List<ChartValue>[] values = new List<ChartValue>[n];

			for (int i = 0; i < n; i++)
			{
				var source = this.dataSeries[i].ChartSeries.Values;

				if (source.Count != m)
				{
					throw new System.NotSupportedException ("Ragged input not supported");
				}
				
				values[i] = new List<ChartValue> (source);
			}

			for (int j = 0; j < m; j++)
			{
				for (int i = 0; i < n; i++)
				{
					inputVector[i] = values[i][j];
				}

				outputVector[j] = function (inputVector);
			}

			return new GraphSyntheticDataSeries (this, new ChartSeries (label, outputVector))
			{
				Title = this.Name,
				Label = label,
			};
		}


		#region IDisposable Members

		public void Dispose()
		{
			this.Clear ();
		}

		#endregion

		
		private List<GraphDataSeries> dataSeries;
		private List<GraphSyntheticDataSeries> syntheticDataSeries;
	}
}
