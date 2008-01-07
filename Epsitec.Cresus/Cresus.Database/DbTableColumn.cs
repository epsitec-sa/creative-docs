//	Copyright © 2007-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTableColumn</c> class defines a table/column tuple is used
	/// to build queries.
	/// </summary>
	public sealed class DbTableColumn : System.IEquatable<DbTableColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTableColumn"/> class.
		/// </summary>
		/// <param name="column">The column definition.</param>
		public DbTableColumn(DbColumn column)
		{
			this.table = column.Table;
			this.column = column;
			this.tableAlias = this.table.Name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTableColumn"/> class.
		/// </summary>
		/// <param name="tableAlias">The table alias.</param>
		/// <param name="column">The column definition.</param>
		public DbTableColumn(string tableAlias, DbColumn column)
			: this (column)
		{
			this.tableAlias = tableAlias;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTableColumn"/> class.
		/// </summary>
		/// <param name="tableAlias">The table alias.</param>
		/// <param name="columnAlias">The column alias.</param>
		/// <param name="column">The column definition.</param>
		public DbTableColumn(string tableAlias, string columnAlias, DbColumn column)
			: this (column)
		{
			this.tableAlias = tableAlias;
			this.columnAlias = columnAlias;
		}

		/// <summary>
		/// Gets the table definition.
		/// </summary>
		/// <value>The table definition.</value>
		public DbTable Table
		{
			get
			{
				return this.table;
			}
		}

		/// <summary>
		/// Gets the column definition.
		/// </summary>
		/// <value>The column definition.</value>
		public DbColumn Column
		{
			get
			{
				return this.column;
			}
		}

		/// <summary>
		/// Gets or sets the table alias.
		/// </summary>
		/// <value>The table alias.</value>
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

		/// <summary>
		/// Gets or sets the column alias.
		/// </summary>
		/// <value>The column alias.</value>
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

		#region IEquatable<DbTableColumn> Members

		public bool Equals(DbTableColumn other)
		{
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}

			return (this.table == other.table)
				&& (this.column == other.column)
				&& (this.tableAlias == other.tableAlias)
				&& (this.columnAlias == other.columnAlias);
		}

		#endregion

		public override bool Equals(object obj)
		{
			return base.Equals (obj as DbTableColumn);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			buffer.Append (this.table.Name);
			buffer.Append (".");
			buffer.Append (this.column.Name);
			
			if ((this.tableAlias != null) ||
				(this.columnAlias != null))
			{
				buffer.Append (" AS ");
				buffer.Append (this.tableAlias ?? "<null>");
				buffer.Append (".");
				buffer.Append (this.columnAlias ?? "<null>");
			}

			return buffer.ToString ();
		}

		public static bool operator==(DbTableColumn a, DbTableColumn b)
		{
			if (object.ReferenceEquals (a, b))
			{
				return true;
			}
			else if (object.ReferenceEquals (a, null))
			{
				return false;
			}
			else
			{
				return a.Equals (b);
			}
		}

		public static bool operator!=(DbTableColumn a, DbTableColumn b)
		{
			return !(a == b);
		}

		private readonly DbTable table;
		private readonly DbColumn column;
		private string tableAlias;
		private string columnAlias;
	}
}
