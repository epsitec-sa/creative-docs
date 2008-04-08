//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting.Settings
{
	/// <summary>
	/// The <c>CollectionSetting</c> class defines how a collection will be
	/// mapped to a list of items, fit for use as rows in a table.
	/// </summary>
	public class CollectionSetting
	{
		public CollectionSetting()
		{
		}



		/// <summary>
		/// Gets or sets the sort comparison used when sorting entities.
		/// </summary>
		/// <value>The sort comparison.</value>
		public System.Comparison<AbstractEntity> SortComparison
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the filter comparison used when preliminarily
		/// filtering entities.
		/// </summary>
		/// <value>The filter comparison.</value>
		public System.Predicate<AbstractEntity> FilterComparison
		{
			get;
			set;
		}
		
		
		public int GroupDepth
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Creates the filtered, sorted (but not grouped) list for the given
		/// input collection.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <returns>The list.</returns>
		public List<AbstractEntity> CreateList(IEnumerable<AbstractEntity> collection)
		{
			List<AbstractEntity> list = new List<AbstractEntity> ();

			if (this.FilterComparison != null)
			{
				foreach (var item in collection)
				{
					if (this.FilterComparison (item))
					{
						list.Add (item);
					}
				}
			}
			else
			{
				list.AddRange (collection);
			}

			if (this.SortComparison != null)
			{
				list.Sort (this.SortComparison);
			}

			return list;
		}
	}
}
