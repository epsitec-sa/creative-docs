//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class SqlIndex
	{
		public SqlIndex(SqlSortOrder sortOrder, params SqlColumn[] columns)
		{
			this.sortOrder = sortOrder;
			this.columns = (SqlColumn[]) columns.Clone ();
		}

		public SqlSortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

		public SqlColumn[] Columns
		{
			get
			{
				return (SqlColumn[]) this.columns.Clone ();
			}
		}


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
