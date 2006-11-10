//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTable"/> class.
		/// </summary>
		/// <param name="name">The table name.</param>
		public SqlTable(string name)
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
		public Collections.SqlColumns			Columns
		{
			get
			{
				return this.columns;
			}
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
		/// Gets or sets the primary key. A primary key can span several columns
		/// which are organised to form a tuple. The tuple must be unique in the
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

					this.primaryKey = new Collections.SqlColumns ();
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


		private string							name;
		private Collections.SqlColumns			columns = new Collections.SqlColumns ();
		private Collections.SqlColumns			primaryKey;
	}
}
