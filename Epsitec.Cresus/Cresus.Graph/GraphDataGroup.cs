//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

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
