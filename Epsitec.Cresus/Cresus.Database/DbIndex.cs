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
	public sealed class DbIndex
	{
		public DbIndex(SqlSortOrder sortOrder, params DbColumn[] columns)
		{
			this.sortOrder = sortOrder;
			this.columns = (DbColumn[]) columns.Clone ();
		}

		public SqlSortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}

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
