//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlColumn</c> class describes a column at the SQL level. Compare
	/// with <see cref="DbColumn"/>.
	/// </summary>
	public sealed class SqlColumn : Common.Types.IName
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		public SqlColumn()
			: this (null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public SqlColumn(string name)
			: this (name, DbRawType.Unknown)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public SqlColumn(string name, DbRawType type)
			: this (name, type, 1, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="length">The length.</param>
		/// <param name="isFixedLength">If set to <c>true</c>, uses a fixed length.</param>
		public SqlColumn(string name, DbRawType type, int length, bool isFixedLength)
			: this (name, type, length, isFixedLength, DbNullability.No)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="nullability">The nullability.</param>
		public SqlColumn(string name, DbRawType type, DbNullability nullability)
			: this (name, type, 1, true, nullability)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="length">The length.</param>
		/// <param name="isFixedLength">If set to <c>true</c>, uses a fixed length.</param>
		/// <param name="nullability">The nullability.</param>
		public SqlColumn(string name, DbRawType type, int length, bool isFixedLength, DbNullability nullability)
		{
			this.Name = name;
			this.SetType (type, length, isFixedLength);
			this.IsNullable = (nullability == DbNullability.Yes);
		}


		/// <summary>
		/// Gets the name of the column.
		/// </summary>
		/// <value>The name of the column.</value>
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
		/// Gets the type of the column.
		/// </summary>
		/// <value>The type of the column.</value>
		public DbRawType						Type
		{
			get
			{
				return this.type;
			}
		}

		/// <summary>
		/// Gets the length of the column.
		/// </summary>
		/// <value>The length of the column.</value>
		public int								Length
		{
			get
			{
				return this.length;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this column is fixed length.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column is fixed length; otherwise, <c>false</c>.
		/// </value>
		public bool								IsFixedLength
		{
			get
			{
				return this.isFixedLength;
			}
		}


		/// <summary>
		/// Gets or sets a value indicating whether this column is nullable.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column is nullable; otherwise, <c>false</c>.
		/// </value>
		public bool								IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}


		/// <summary>
		/// Sets the type.
		/// </summary>
		/// <param name="type">The type.</param>
		public void SetType(DbRawType type)
		{
			this.SetType (type, 1, true);
		}

		/// <summary>
		/// Sets the type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="length">The length.</param>
		/// <param name="isFixedLength">If set to <c>true</c>, this column is fixed length.</param>
		public void SetType(DbRawType type, int length, bool isFixedLength)
		{
			if (length < 1)
			{
				throw new System.ArgumentOutOfRangeException ("Invalid length");
			}

			switch (type)
			{
				case DbRawType.String:
					
					//	This is the only raw type which accepts a length specification.
					//	The byte array does not require it.
					
					break;

				default:
					if ((length != 1) ||
						(!isFixedLength))
					{
						throw new System.ArgumentOutOfRangeException ("Length/type mismatch");
					}
					break;
			}

			this.type          = type;
			this.length        = length;
			this.isFixedLength = isFixedLength;
		}

		private string							name;
		private DbRawType						type;
		private bool							isNullable;
		private bool							isFixedLength;
		private int								length;
	}
}
