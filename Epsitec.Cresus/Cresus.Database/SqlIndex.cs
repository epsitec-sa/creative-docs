//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

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
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public SqlIndex(SqlSortOrder sortOrder, params SqlColumn[] columns)
		{
			this.sortOrder = sortOrder;
			this.columns = (SqlColumn[]) columns.Clone ();
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
		public SqlColumn[] Columns
		{
			get
			{
				return (SqlColumn[]) this.columns.Clone ();
			}
		}

		/// <summary>
		/// Returns a <see cref="string"/> that represents the current index..
		/// </summary>
		/// <returns>
		/// A string that represents the current index.
		/// </returns>
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (SqlColumn column in this.columns)
			{
				if (buffer.Length > 0)
				{
					buffer.Append ("_");
				}

				buffer.Append (column.Name);
			}

			return buffer.ToString ();
		}

		private readonly SqlSortOrder			sortOrder;
		private readonly SqlColumn[]			columns;
	}
}
