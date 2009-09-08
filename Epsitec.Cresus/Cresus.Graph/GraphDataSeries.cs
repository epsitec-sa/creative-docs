//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Graph.Data;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataSeries
	{
		public GraphDataSeries()
		{
			this.dataGroups = new List<GraphDataGroup> ();
		}

		public GraphDataSeries(ChartSeries series)
			: this ()
		{
			this.chartSeries = series;
		}

		
		public ChartSeries ChartSeries
		{
			get
			{
				if (this.chartSeries == null)
				{
					return new ChartSeries ();
				}
				else
				{
					if (this.NegateValues)
					{
						return new ChartSeries (this.chartSeries.Label, this.chartSeries.Values.Select (x => new ChartValue (x.Label, -x.Value)));
					}
					else
					{
						return new ChartSeries (this.chartSeries.Label, this.chartSeries.Values);
					}
				}
			}
		}

		public bool IsSelected
		{
			get;
			set;
		}

		public bool NegateValues
		{
			get;
			set;
		}

		public GraphDataSource Source
		{
			get;
			private set;
		}

		public IEnumerable<GraphDataGroup> Groups
		{
			get
			{
				return this.dataGroups;
			}
		}

		public int GroupCount
		{
			get
			{
				return this.dataGroups.Count;
			}
		}


		internal void DefineDataSource(GraphDataSource source)
		{
			System.Diagnostics.Debug.Assert (this.Source == null || source == null);

			this.Source = source;
		}

		internal void AddDataGroup(GraphDataGroup group)
		{
			System.Diagnostics.Debug.Assert (this.dataGroups.Contains (group) == false);

			this.dataGroups.Add (group);
		}

		internal void RemoveDataGroup(GraphDataGroup group)
		{
			System.Diagnostics.Debug.Assert (this.dataGroups.Contains (group));

			this.dataGroups.Remove (group);
		}


		protected ChartSeries chartSeries;
		private List<GraphDataGroup> dataGroups;
	}
}
