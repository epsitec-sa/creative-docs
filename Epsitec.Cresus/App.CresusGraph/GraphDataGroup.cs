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

		public int Index
		{
			get;
			set;
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
				return this.syntheticDataSeries.Where (x => x.Enabled);
			}
		}

		public GraphDataSource Source
		{
			get;
			set;
		}

		public string DefaultFunctionName
		{
			get;
			set;
		}


		public void Add(GraphDataSeries series)
		{
			if (series != null)
			{
				series.AddDataGroup (this);
				this.dataSeries.Add (series);
				this.Invalidate ();
			}
		}

		public bool Remove(GraphDataSeries series)
		{
			if ((series != null) &&
				(this.dataSeries.Remove (series)))
			{
				series.RemoveDataGroup (this);
				this.Invalidate ();
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
			this.Invalidate ();
		}

		public bool Contains(GraphDataSeries series)
		{
			return this.dataSeries.Contains (series);
		}


		public void ClearSyntheticDataSeries()
		{
			this.syntheticDataSeries.ForEach (x => x.Enabled = false);
		}

		public GraphSyntheticDataSeries AddSyntheticDataSeries(string functionName)
		{
			var series = this.syntheticDataSeries.Find (x => x.FunctionName == functionName);

			if (series == null)
			{
				series = new GraphSyntheticDataSeries (this, functionName);

				series.DefineDataSource (this.Source);

				this.syntheticDataSeries.Add (series);
			}
			else
			{
				this.syntheticDataSeries.Remove (series);
				this.syntheticDataSeries.Add (series);
			}

//-			series.Title   = this.Name;
			series.Enabled = true;

			return series;
		}

		public void RemoveSyntheticDataSeries(string functionName)
		{
			var series = this.syntheticDataSeries.Find (x => x.FunctionName == functionName);

			if (series != null)
			{
				series.Enabled = false;
			}
		}

		public GraphSyntheticDataSeries GetSyntheticDataSeries(string functionName)
		{
			if (string.IsNullOrEmpty (functionName))
			{
				return null;
			}

			var series = this.syntheticDataSeries.Find (x => x.FunctionName == functionName);

			if (series == null)
			{
				series = this.AddSyntheticDataSeries (functionName);
			}

			return series;
		}

		public void Invalidate()
		{
			this.syntheticDataSeries.ForEach (x => x.Invalidate ());
		}

		
		internal ChartSeries SynthesizeChartSeries(string label, System.Func<IList<double>, double> function)
		{
			int n = this.dataSeries.Count;
			int m = n == 0 ? 0 : this.dataSeries[0].ChartSeries.Values.Count;
			
			double[]     inputVector  = new double[n];
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
					inputVector[i] = values[i][j].Value;
				}

				outputVector[j] = new ChartValue (values[0][j].Label, function (inputVector));
			}

			return new ChartSeries (label, outputVector);
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
