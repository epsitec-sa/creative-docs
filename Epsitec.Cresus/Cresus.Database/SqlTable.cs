//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlTable</c> class describes a table at the SQL level. Compare
	/// with <see cref="DbTable"/>.
	/// </summary>
	public sealed class SqlTable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTable"/> class.
		/// </summary>
		public SqlTable()
		{
			this.columns = new Collections.SqlColumnList ();
			this.indexes = new List<SqlIndex> ();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTable"/> class.
		/// </summary>
		/// <param name="name">The table name.</param>
		public SqlTable(string name)
			: this ()
		{
			this.Name = name;
		}

		/// <summary>
		/// Gets or sets the table name.
		/// </summary>
		/// <value>The table name.</value>
		public string							Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		/// <summary>
		/// Gets the columns collection.
		/// </summary>
		/// <value>The columns.</value>
		public Collections.SqlColumnList		Columns
		{
			get
			{
				return this.columns;
			}
		}

		public string Comment
		{
			get;
			set;
		}

		/// <summary>
		/// Gets a value indicating whether this table has a primary key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column has a primary key; otherwise, <c>false</c>.
		/// </value>
		public bool								HasPrimaryKey
		{
			get
			{
				return (this.primaryKey != null) && (this.primaryKey.Count > 0);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this table has any foreign keys.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column has any foreign keys; otherwise, <c>false</c>.
		/// </value>
		public bool								HasForeignKeys
		{
			get
			{
				foreach (SqlColumn column in this.columns)
				{
					if (column.IsForeignKey)
					{
						return true;
					}
				}
				
				return false;
			}
		}

		/// <summary>
		/// Gets or sets the primary key. A primary key can span several columns
		/// which are organized to form a tuple. The tuple must be unique in the
		/// database.
		/// </summary>
		/// <value>The primary key.</value>
		public SqlColumn[]						PrimaryKey
		{
			get
			{
				if (this.primaryKey == null)
				{
					return new SqlColumn[0];
				}
				else
				{
					return this.primaryKey.ToArray ();
				}
			}
			set
			{
				if (this.primaryKey == null)
				{
					if (value == null)
					{
						return;
					}

					this.primaryKey = new Collections.SqlColumnList ();
				}

				if ((value == null) ||
					(value.Length == 0))
				{
					this.primaryKey = null;
				}
				else
				{
					this.primaryKey.Clear ();
					this.primaryKey.AddRange (value);
				}
			}
		}

		/// <summary>
		/// Gets the foreign keys.
		/// </summary>
		/// <value>The foreign keys.</value>
		public IEnumerable<SqlColumn>			ForeignKeys
		{
			get
			{
				foreach (SqlColumn column in this.columns)
				{
					if (column.IsForeignKey)
					{
						yield return column;
					}
				}
			}
		}

		/// <summary>
		/// Gets the indexes.
		/// </summary>
		/// <value>The indexes.</value>
		public IEnumerable<SqlIndex>			Indexes
		{
			get
			{
				return this.indexes;
			}
		}

		/// <summary>
		/// Adds an index for this table.
		/// </summary>
		/// <param name="name">The name of the index.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public void AddIndex(string name, SqlSortOrder sortOrder, params SqlColumn[] columns)
		{
			System.Diagnostics.Debug.Assert (sortOrder != SqlSortOrder.None);
			System.Diagnostics.Debug.Assert (columns.Length > 0);

			SqlIndex index = new SqlIndex (name, columns, sortOrder);

			this.indexes.Add (index);
		}

		
		private string							name;
		private readonly Collections.SqlColumnList columns;
		private Collections.SqlColumnList		primaryKey;
		private List<SqlIndex>					indexes;
	}
}
