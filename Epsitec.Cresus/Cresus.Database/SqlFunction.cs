//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>SqlFunction</c> class describes the SQL functions such as
	/// simple mathematic operators (+, -, *, etc.), comparisons
	/// (=, &lt;, &gt;, &lt;&gt;, IS NULL, LIKE, etc.) and tests (IN,
	/// NOT IN, BETWEEN, NOT BETWEEN, EXISTS, NOT EXISTS, etc.).
	/// </summary>
	public sealed class SqlFunction
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlFunction"/> class.
		/// </summary>
		/// <param name="code">The function code.</param>
		/// <param name="fields">The fields.</param>
		public SqlFunction(SqlFunctionCode code, params SqlField[] fields)
		{
			this.code = code;

			int countProvided = fields.Length;
			int countExpected = this.ArgumentCount;

			if (countProvided != countExpected)
			{
				throw new System.ArgumentOutOfRangeException (string.Format ("{0} requires {1} field(s).", code, countExpected));
			}

			switch (countProvided)
			{
				case 0:
					break;
				
				case 1:
					this.a = fields[0];
					break;
				
				case 2:
					this.a = fields[0];
					this.b = fields[1];
					break;
				
				case 3:
					this.a = fields[0];
					this.b = fields[1];
					this.c = fields[2];
					break;
			}
		}


		/// <summary>
		/// Gets the function code.
		/// </summary>
		/// <value>The function code.</value>
		public SqlFunctionCode					Code
		{
			get
			{
				return this.code;
			}
		}

		/// <summary>
		/// Gets the argument count.
		/// </summary>
		/// <value>The argument count.</value>
		public int								ArgumentCount
		{
			get
			{
				switch (this.code)
				{
					//	Math :

					case SqlFunctionCode.MathAdd:
					case SqlFunctionCode.MathSubstract:
					case SqlFunctionCode.MathMultiply:
					case SqlFunctionCode.MathDivide:
						return 2;

					// Comparisons :
					
					case SqlFunctionCode.CompareEqual:
					case SqlFunctionCode.CompareNotEqual:
					case SqlFunctionCode.CompareLessThan:
					case SqlFunctionCode.CompareLessThanOrEqual:
					case SqlFunctionCode.CompareGreaterThan:
					case SqlFunctionCode.CompareGreaterThanOrEqual:
						return 2;

					case SqlFunctionCode.CompareIsNull:
					case SqlFunctionCode.CompareIsNotNull:
						return 1;

					case SqlFunctionCode.CompareLike:
					case SqlFunctionCode.CompareLikeEscape:
					case SqlFunctionCode.CompareNotLike:
					case SqlFunctionCode.CompareNotLikeEscape:
						return 2;

					case SqlFunctionCode.CompareFalse:
					case SqlFunctionCode.CompareTrue:
						return 0;

					//	Sets :

					case SqlFunctionCode.SetIn:
					case SqlFunctionCode.SetNotIn:
						return 2;

					case SqlFunctionCode.SetBetween:
					case SqlFunctionCode.SetNotBetween:
						return 3;

					case SqlFunctionCode.SetExists:
					case SqlFunctionCode.SetNotExists:
						return 1;

					//	Logic :
					
					case SqlFunctionCode.LogicNot:
						return 1;

					case SqlFunctionCode.LogicAnd:
					case SqlFunctionCode.LogicOr:
						return 2;

					//	Other :
					
					case SqlFunctionCode.Substring:
						return 3;

					case SqlFunctionCode.Upper:
						return 1;

					default:
						return 0;
				}
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

		/// <summary>
		/// Gets the C field.
		/// </summary>
		/// <value>The C field.</value>
		public SqlField							C
		{
			get
			{
				return this.c;
			}
		}


		private SqlFunctionCode					code;
		private SqlField						a, b, c;
	}
}
