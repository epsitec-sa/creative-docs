//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataSource : IEnumerable<GraphDataSeries>
	{
		public GraphDataSource()
		{
			this.dataSeries = new List<GraphDataSeries> ();
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

		
		public void Add(GraphDataSeries series)
		{
			if (series != null)
			{
				series.DefineDataSource (this);
				this.dataSeries.Add (series);
			}
		}

		public bool Remove(GraphDataSeries series)
		{
			if ((series != null) &&
				(this.dataSeries.Remove (series)))
			{
				series.DefineDataSource (null);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear()
		{
			this.dataSeries.ForEach (x => x.DefineDataSource (null));
			this.dataSeries.Clear ();
		}


		#region IEnumerable<GraphDataSeries> Members

		public IEnumerator<GraphDataSeries> GetEnumerator()
		{
			return this.dataSeries.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.dataSeries.GetEnumerator ();
		}

		#endregion

		
		private List<GraphDataSeries> dataSeries;
	}
}
