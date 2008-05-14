//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	/// The <c>DataQuery</c> class defines a high level query used by the
	/// <see cref="DataBrowser"/> to define a SQL SELECT statement.
	/// </summary>
	public class DataQuery
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataQuery"/> class.
		/// </summary>
		public DataQuery()
		{
			this.columns = new List<DataQueryColumn> ();
		}

		protected DataQuery(DataQuery originalQuery)
		{
			this.columns  = originalQuery.columns;
			this.Distinct = originalQuery.Distinct;
		}


		/// <summary>
		/// The collection of output fields.
		/// </summary>
		/// <value>The collection of output fields.</value>
		public IList<DataQueryColumn> Columns
		{
			get
			{
				return this.columns;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="DataQuery"/>
		/// will return only distinct values (i.e. remove duplicates).
		/// </summary>
		/// <value><c>true</c> if only distinct values should be returned;
		/// otherwise, <c>false</c>.</value>
		public bool Distinct
		{
			get;
			set;
		}


		/// <summary>
		/// Searches for the specified path and returns the zero-based index to the
		/// first matching column.
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns>The zero-based index to the first matching column; otherwise
		/// <c>-1</c>.</returns>
		public int IndexOf(EntityFieldPath path)
		{
			return this.columns.FindIndex (column => column.FieldPath.Equals (path));
		}

		
		private readonly List<DataQueryColumn> columns;
	}
}
