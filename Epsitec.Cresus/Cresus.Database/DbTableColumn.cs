//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTableColumn</c> class defines a table/column tuple is used
	/// to build queries.
	/// </summary>
	public sealed class DbTableColumn
	{
		public DbTableColumn(DbColumn column)
		{
			this.table = column.Table;
			this.column = column;
		}

		public DbTableColumn(string tableAlias, DbColumn column)
			: this (column)
		{
			this.tableAlias = tableAlias;
		}

		public DbTableColumn(string tableAlias, string columnAlias, DbColumn column)
			: this (column)
		{
			this.tableAlias = tableAlias;
			this.columnAlias = columnAlias;
		}

		public DbTable Table
		{
			get
			{
				return this.table;
			}
		}

		public DbColumn Column
		{
			get
			{
				return this.column;
			}
		}

		public string TableAlias
		{
			get
			{
				return this.tableAlias;
			}
			set
			{
				this.tableAlias = value;
			}
		}

		public string ColumnAlias
		{
			get
			{
				return this.columnAlias;
			}
			set
			{
				this.columnAlias = value;
			}
		}

		private readonly DbTable table;
		private readonly DbColumn column;
		private string tableAlias;
		private string columnAlias;
	}
}
