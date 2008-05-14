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
		/// <summary>
		/// Initializes a new instance of the <see cref="DataBrowserRow"/> class.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="values">The values.</param>
		public DataBrowserRow(DataQueryResult query, object[] values)
		{
			this.query  = query;
			this.values = values;
			this.keys   = DataBrowserRow.emptyKeys;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBrowserRow"/> class.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="row">The row which contains the values and the keys.</param>
		public DataBrowserRow(DataQueryResult query, DbReader.RowData row)
		{
			this.query = query;
			this.values = row.Values;
			this.keys  = row.Keys;

			System.Diagnostics.Debug.Assert (this.query.EntityIds.Count == this.keys.Length);
		}

		/// <summary>
		/// Gets the query, which defines the various columns.
		/// </summary>
		/// <value>The query.</value>
		public DataQueryResult Query
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
					(index >= this.values.Length))
				{
					throw new System.ArgumentOutOfRangeException ("index", "Index out of range");
				}

				return this.values[index];
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
		/// Gets the value count.
		/// </summary>
		/// <value>The value count.</value>
		public int ValueCount
		{
			get
			{
				return this.values.Length;
			}
		}

		/// <summary>
		/// Gets the key count (number of entity keys for the data in the row).
		/// </summary>
		/// <value>The key count.</value>
		public int KeyCount
		{
			get
			{
				return this.keys.Length;
			}
		}

		/// <summary>
		/// Gets the values, sorted in the same order as the columns defined by
		/// the <see cref="Query"/> property..
		/// </summary>
		/// <value>The values.</value>
		public IList<object> Values
		{
			get
			{
				return this.values;
			}
		}

		/// <summary>
		/// Gets the keys, sorted in the same order as the entity ids in the
		/// <see cref="Query"/> property.
		/// </summary>
		/// <value>The keys.</value>
		public IList<DbKey> Keys
		{
			get
			{
				return this.keys;
			}
		}

		private static readonly DbKey[] emptyKeys = new DbKey[0];

		private readonly DataQueryResult query;
		private readonly object[] values;
		private readonly DbKey[] keys;
	}
}
