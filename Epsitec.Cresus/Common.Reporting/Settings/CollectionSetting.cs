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
		public System.Comparison<AbstractEntity> Sort
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the filter predicate used when preliminarily
		/// filtering entities.
		/// </summary>
		/// <value>The filter predicate.</value>
		public System.Predicate<AbstractEntity> Filter
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

			if (this.Filter != null)
			{
				foreach (var item in collection)
				{
					if (this.Filter (item))
					{
						list.Add (item);
					}
				}
			}
			else
			{
				list.AddRange (collection);
			}

			if (this.Sort != null)
			{
				list.Sort (this.Sort);
			}

			return list;
		}
	}
}
