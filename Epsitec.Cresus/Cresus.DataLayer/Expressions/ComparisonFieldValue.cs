using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>ComparisonFieldValue</c> class represents a comparison between a
	/// <see cref="Field"/> and a <see cref="Constant"/>, such as (a = 3).
	/// </summary>
	public class ComparisonFieldValue : Comparison
	{


		/// <summary>
		/// Builds a new <c>ComparisonFieldValue</c>.
		/// </summary>
		/// <param name="left">The <see cref="Field"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <param name="op">The <see cref="BinaryComparator"/> used by the <c>ComparisonFieldValue</c>.</param>
		/// <param name="right">The <see cref="Constant"/> on the left of the <see cref="BinaryComparator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public ComparisonFieldValue(Field left, BinaryComparator op, Constant right) : base ()
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");

			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public Field Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryComparator"/> of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public BinaryComparator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>ComparisonFieldValue</c>.
		/// </summary>
		public Constant Right
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the sequence of field ids that are used in this instance.
		/// </summary>
		/// <returns>The sequence of field ids that are used in this instance.</returns>
		internal override IEnumerable<Druid> GetFields()
		{
			yield return this.Left.FieldId;
		}


		/// <summary>
		/// Creates an <see cref="SqlFunction"/> that corresponds to this instance.
		/// </summary>
		/// <param name="sqlConstantResolver">A function used to build the <see cref="SqlField"/> that represent constants.</param>
		/// <param name="sqlColumnResolver">A function used to build the <see cref="SqlField"/> that represent columns in a table.</param>
		/// <returns>The new <see cref="SqlFunction"/>.</returns>
		internal override SqlFunction CreateSqlCondition(System.Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> sqlConstantResolver, System.Func<Druid, SqlField> sqlColumnResolver)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				this.Left.CreateSqlField (sqlColumnResolver),
				this.Right.CreateSqlField (sqlConstantResolver)
			);
		}


	}


}
