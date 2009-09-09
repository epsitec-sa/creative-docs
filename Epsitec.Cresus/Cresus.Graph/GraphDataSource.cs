//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	public class GraphDataSource : IEnumerable<GraphDataSeries>
	{
		public GraphDataSource(string importConverter)
		{
			this.converterName = importConverter;
			this.dataSeries = new List<GraphDataSeries> ();
			this.categories = new List<GraphDataCategory> ();
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

		public GraphDataSeries this[int index]
		{
			get
			{
				return this.dataSeries[index];
			}
		}

		public IList<GraphDataCategory> Categories
		{
			get
			{
				return this.categories.AsReadOnly ();
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


		public void UpdateCategories()
		{
			var cat = new HashSet<GraphDataCategory> ();

			this.dataSeries.ForEach (x => cat.Add (this.GetCategory (x)));

			this.categories.Clear ();
			this.categories.AddRange (cat.OrderBy (x => x));
		}

		
		public GraphDataCategory GetCategory(GraphDataSeries series)
		{
			var converter = ImportConverters.ImportConverter.FindConverter (this.converterName);

			if (converter == null)
			{
				return GraphDataCategory.Generic;
			}
			else
			{
				return converter.GetCategory (series.ChartSeries);
			}
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

		
		private readonly string converterName;
		private readonly List<GraphDataSeries> dataSeries;
		private readonly List<GraphDataCategory> categories;
	}
}
