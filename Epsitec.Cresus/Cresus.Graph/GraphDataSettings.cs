//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataSettings
	{
		public GraphDataSettings()
		{
			this.outputSeries = new List<GraphDataSeries> ();
			this.groups = new List<GraphDataGroup> ();
			this.categories = new List<GraphDataCategory> ();
		}

		public List<GraphDataSeries> OutputSeries
		{
			get
			{
				return this.outputSeries;
			}
		}

		public List<GraphDataGroup> Groups
		{
			get
			{
				return this.groups;
			}
		}

		public List<GraphDataCategory> FilterCategories
		{
			get
			{
				return this.categories;
			}
		}

		public string DataSourceName
		{
			get
			{
				return this.dataSourceName;
			}
			set
			{
				this.dataSourceName = value;
			}
		}

		private readonly List<GraphDataSeries> outputSeries;
		private readonly List<GraphDataGroup> groups;
		private readonly List<GraphDataCategory> categories;
		private string dataSourceName;
	}
}
