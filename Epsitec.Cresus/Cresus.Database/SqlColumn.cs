//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System;

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
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public SqlColumn(string name, DbRawType type)
			: this (name, type, false)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="isNullable">The nullability.</param>
		public SqlColumn(string name, DbRawType type, bool isNullable)
			: this (name, type, isNullable, 1, true, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="isNullable">The nullability.</param>
		/// <param name="length">The length.</param>
		/// <param name="isFixedLength">If set to <c>true</c>, uses a fixed length.</param>
		public SqlColumn(string name, DbRawType type, bool isNullable, int length, bool isFixedLength)
			: this (name, type, isNullable, length, isFixedLength, null, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="isNullable">The nullability.</param>
		/// <param name="length">The length.</param>
		/// <param name="isFixedLength">If set to <c>true</c>, uses a fixed length.</param>
		/// <param name="encoding">The encoding of the text.</param>
		/// <param name="collation">The collation of the text.</param>
		public SqlColumn(string name, DbRawType type, bool isNullable, int length, bool isFixedLength, DbCharacterEncoding? encoding, DbCollation? collation)
		{
			length.ThrowIf (l => l < 1, "Invalid length");

			if (type != DbRawType.String)
			{
				//	This is the only raw type which accepts a length specification. The byte array
				// does not require it.

				if (length != 1 || !isFixedLength)
				{
					throw new ArgumentOutOfRangeException ("Length/type mismatch");
				}
			}

			this.name = name;
			this.type = type;
			this.isNullable = isNullable;
			this.length = length;
			this.isFixedLength = isFixedLength;
			this.encoding = encoding;
			this.collation = collation;
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
		/// Gets a value indicating whether this column is nullable.
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
		}

		/// <summary>
		/// Gets the character encoding (if this column defines a string).
		/// </summary>
		/// <value>The character encoding.</value>
		public DbCharacterEncoding?				Encoding
		{
			get
			{
				return this.encoding;
			}
		}

		/// <summary>
		/// Gets the collation (if this column defines a string).
		/// </summary>
		/// <value>The collation.</value>
		public DbCollation?						Collation
		{
			get
			{
				return this.collation;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this column defines a foreign key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance column defines a foreign key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsForeignKey
		{
			get;
			set;
		}

		/// <summary>
		/// The comment associated with this instance.
		/// </summary>
		public string							Comment
		{
			get;
			set;
		}

		private readonly string					name;
		private readonly DbRawType				type;
		private readonly bool					isNullable;
		private readonly bool					isFixedLength;
		private readonly DbCharacterEncoding?	encoding;
		private readonly DbCollation?			collation;
		private readonly int					length;
	}
}
