//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// The <c>DataBrowserRow</c> class stores a row from the result set produced
	/// by a <see cref="DataBrowser"/> query.
	/// </summary>
	public sealed class DataBrowserRow
	{
		public DataBrowserRow(DataQuery query, object[] items)
		{
			this.query = query;
			this.items = items;
		}

		/// <summary>
		/// Gets the query, which defines the various columns.
		/// </summary>
		/// <value>The query.</value>
		public DataQuery Query
		{
			get
			{
				return this.query;
			}
		}

		/// <summary>
		/// Gets the object at the specified index.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
		/// <value>The object.</value>
		public object this[int index]
		{
			get
			{
				if ((index < 0) ||
					(index >= this.items.Length))
				{
					throw new System.ArgumentOutOfRangeException ("index", "Index out of range");
				}

				return this.items[index];
			}
		}

		/// <summary>
		/// Gets the object matching the specified path.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the path cannot be found.</exception>
		/// <value>The object.</value>
		public object this[EntityFieldPath path]
		{
			get
			{
				return this[query.IndexOf (path)];
			}
		}

		/// <summary>
		/// Gets the object mathing the specified column.
		/// </summary>
		/// <exception cref="System.ArgumentOutOfRangeException">Thrown when the path cannot be found.</exception>
		/// <value>The object.</value>
		public object this[DataQueryColumn column]
		{
			get
			{
				return this[column.FieldPath];
			}
		}

		/// <summary>
		/// Gets the items, sorted in the same order as the columns defined by
		/// the <see cref="Query"/> property..
		/// </summary>
		/// <value>The items.</value>
		public IEnumerable<object> Items
		{
			get
			{
				return this.items;
			}
		}

		private readonly DataQuery query;
		private readonly object[] items;
	}
}
