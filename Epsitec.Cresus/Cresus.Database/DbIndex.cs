//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbIndex</c> class represents an index definition for a <see cref="DbTable"/>.
	/// </summary>
	public sealed class DbIndex
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbIndex"/> class.
		/// </summary>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public DbIndex(SqlSortOrder sortOrder, params DbColumn[] columns)
		{
			this.sortOrder = sortOrder;
			this.columns = (DbColumn[]) columns.Clone ();
		}

		/// <summary>
		/// Gets the sort order.
		/// </summary>
		/// <value>The sort order.</value>
		public SqlSortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		/// <summary>
		/// Gets the columns.
		/// </summary>
		/// <value>The columns.</value>
		public DbColumn[] Columns
		{
			get
			{
				return (DbColumn[]) this.columns.Clone ();
			}
		}

		private readonly SqlSortOrder			sortOrder;
		private readonly DbColumn[]				columns;
	}
}
