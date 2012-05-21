//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlIndex</c> class represents an index definition for a <see cref="SqlTable"/>.
	/// </summary>
	public sealed class SqlIndex
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlIndex"/> class.
		/// </summary>
		/// <param name="name">The name</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public SqlIndex(string name, IEnumerable<SqlColumn> columns, SqlSortOrder sortOrder)
		{
			this.Name = name;
			this.Columns = columns.ToList ().AsReadOnly ();
			this.SortOrder = sortOrder;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public ReadOnlyCollection<SqlColumn> Columns
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the sort order.
		/// </summary>
		/// <value>The sort order.</value>
		public SqlSortOrder SortOrder
		{
			get;
			private set;
		}

	}

}
