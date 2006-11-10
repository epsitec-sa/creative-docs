//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlJoin</c> class describes joins for <c>SqlSelect</c>.
	/// </summary>
	public sealed class SqlJoin
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlJoin"/> class.
		/// </summary>
		/// <param name="code">The join code.</param>
		/// <param name="fields">The fields.</param>
		public SqlJoin(SqlJoinCode code, params SqlField[] fields)
		{
			this.code = code;

			if (fields.Length != 2)
			{
				throw new System.ArgumentOutOfRangeException (string.Format ("Join ({0}) requires 2 fields, got {1}.", code, fields.Length));
			}

			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].FieldType != SqlFieldType.QualifiedName)
				{
					throw new System.ArgumentException (string.Format ("Join argument {0} must be a qualified names (specified {1}).", i, fields[i].FieldType));
				}
			}

			this.a = fields[0];
			this.b = fields[1];
		}


		/// <summary>
		/// Gets the join code.
		/// </summary>
		/// <value>The join code.</value>
		public SqlJoinCode						Code
		{
			get
			{
				return this.code;
			}
		}

		/// <summary>
		/// Gets the A field.
		/// </summary>
		/// <value>The A field.</value>
		public SqlField							A
		{
			get
			{
				return this.a;
			}
		}

		/// <summary>
		/// Gets the B field.
		/// </summary>
		/// <value>The B field.</value>
		public SqlField							B
		{
			get
			{
				return this.b;
			}
		}


		private SqlJoinCode						code;
		private SqlField						a, b;
	}
}
