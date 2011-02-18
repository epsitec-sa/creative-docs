//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Graph
{
	/// <summary>
	/// The <c>GraphDataSource</c> class stores all <see cref="GraphDataSeries"/> which belong
	/// to a given data source.
	/// </summary>
	public class GraphDataSource : IEnumerable<GraphDataSeries>
	{
		public GraphDataSource(string importConverter, string importConverterMeta)
		{
			this.converterName = importConverter;
			this.converterMeta = importConverterMeta;
			this.dataSeries = new List<GraphDataSeries> ();
			this.categories = new List<GraphDataCategory> ();
		}

		/// <summary>
		/// Gets or sets the name of this source.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the number of series in this source.
		/// </summary>
		/// <value>The number of series in this source.</value>
		public int Count
		{
			get
			{
				return this.dataSeries.Count;
			}
		}

		/// <summary>
		/// Gets the series at the specified index.
		/// </summary>
		/// <value>The series at the specified index</value>
		public GraphDataSeries this[int index]
		{
			get
			{
				return this.dataSeries[index];
			}
		}

		/// <summary>
		/// Gets a list of the data categories.
		/// </summary>
		/// <value>The data categories.</value>
		public IList<GraphDataCategory> Categories
		{
			get
			{
				return this.categories.AsReadOnly ();
			}
		}

		/// <summary>
		/// Adds the specified series.
		/// </summary>
		/// <param name="series">The series.</param>
		public void Add(GraphDataSeries series)
		{
			if (series != null)
			{
				series.DefineDataSource (this);
				this.dataSeries.Add (series);
			}
		}

		/// <summary>
		/// Adds a collection of series.
		/// </summary>
		/// <param name="collection">The collection of series.</param>
		public void AddRange(IEnumerable<GraphDataSeries> collection)
		{
			if (collection != null)
			{
				collection.ForEach (x => this.Add (x));
			}
		}

		/// <summary>
		/// Removes the specified series.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns><c>true</c> if the series was removed; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Clears the source: remove all series.
		/// </summary>
		public void Clear()
		{
			this.dataSeries.ForEach (x => x.DefineDataSource (null));
			this.dataSeries.Clear ();
			this.categories.Clear ();
		}


		/// <summary>
		/// Finds the index of the specified series.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns>The index of the series or <c>-1</c> if it cannot be found.</returns>
		public int IndexOf(GraphDataSeries series)
		{
			if (series == null)
            {
				return -1;
            }

			int index = this.dataSeries.IndexOf (series);

			if (index >= 0)
			{
				return index;
			}
			else
			{
				return this.IndexOf (series.Parent);
			}
		}

		/// <summary>
		/// Renumbers the series by setting <see cref="GraphDataSeries.Index"/> to the series index
		/// in this source.
		/// </summary>
		public void RenumberSeries()
		{
			for (int i = 0; i < this.dataSeries.Count; i++)
			{
				this.dataSeries[i].Index = i;
			}
		}

		/// <summary>
		/// Updates the categories based on the series in this source.
		/// </summary>
		public void UpdateCategories()
		{
			var cat = new HashSet<GraphDataCategory> ();

			this.dataSeries.ForEach (x => cat.Add (this.GetCategory (x)));

			this.categories.Clear ();
			this.categories.AddRange (cat.OrderBy (x => x));
		}


		/// <summary>
		/// Gets the category for the specified series. This will query the converter.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns>The category.</returns>
		public GraphDataCategory GetCategory(GraphDataSeries series)
		{
			this.ResolveConverter ();

			if (this.converter == null)
			{
				return GraphDataCategory.Generic;
			}
			else
			{
				return this.converter.GetCategory (series.ChartSeries);
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


		private void ResolveConverter()
		{
			if (this.converter == null)
			{
				this.converter = ImportConverters.ImportConverter.FindConverter (this.converterName);

				if (this.converter != null)
				{
					this.converter = this.converter.CreateSpecificConverter (this.converterMeta);
				}
			}
		}

		private readonly string converterName;
		private readonly string converterMeta;
		private readonly List<GraphDataSeries> dataSeries;
		private readonly List<GraphDataCategory> categories;

		private ImportConverters.AbstractImportConverter converter;
	}
}
