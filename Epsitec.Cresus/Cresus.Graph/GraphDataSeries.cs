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

		public GraphDataSeries(GraphDataSeries series)
			: this ()
		{
			this.parent = series;
			this.Title = series.Title;
			this.Label = series.Label;
		}
		
		
		public virtual ChartSeries ChartSeries
		{
			get
			{
				var series = this.parent == null ? this.chartSeries : this.parent.ChartSeries;

				if (series == null)
				{
					return new ChartSeries ();
				}
				else
				{
					if (this.NegateValues)
					{
						return new ChartSeries (series.Label, series.Values.Select (x => new ChartValue (x.Label, -x.Value)));
					}
					else
					{
						return new ChartSeries (series.Label, series.Values);
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
			get
			{
				return this.parent == null ? this.source : this.parent.Source;
			}
		}

		public GraphDataSeries Parent
		{
			get
			{
				return this.parent;
			}
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

		public int Index
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Label
		{
			get;
			set;
		}



		internal void DefineDataSource(GraphDataSource source)
		{
			System.Diagnostics.Debug.Assert (this.parent == null);
			System.Diagnostics.Debug.Assert (this.source == null || source == null);

			this.source = source;
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
		private readonly List<GraphDataGroup> dataGroups;
		private readonly GraphDataSeries parent;
		private GraphDataSource source;
	}
}
