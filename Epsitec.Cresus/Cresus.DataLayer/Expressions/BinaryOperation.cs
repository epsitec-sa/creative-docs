using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryOperation</c> class represent a logical operation on two <see cref="Expression"/>,
	/// such as ((a = b) and (c = d)).
	/// </summary>
	public class BinaryOperation : Operation
	{


		/// <summary>
		/// Builds a new <c>BinaryOperation</c>.
		/// </summary>
		/// <param name="left">The left side of the <see cref="BinaryOperator"/>.</param>
		/// <param name="op">The <see cref="BinaryOperator"/> to apply to the left and right argument.</param>
		/// <param name="right">The right side of the <see cref="BinaryOperator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="left"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="right"/> is null.</exception>
		public BinaryOperation(Expression left, BinaryOperator op, Expression right)
		{
			left.ThrowIfNull ("left");
			right.ThrowIfNull ("right");
			
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		/// <summary>
		/// The left side of the <c>Expression</c>.
		/// </summary>
		public Expression Left
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="BinaryOperator"/> of the <c>Expression</c>.
		/// </summary>
		public BinaryOperator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// The right side of the <c>Expression</c>.
		/// </summary>
		public Expression Right
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
			IEnumerable<Druid> left = this.Left.GetFields ();
			IEnumerable<Druid> right = this.Right.GetFields ();

			return left.Concat (right).Distinct ();
		}


		internal override SqlFunction CreateSqlCondition(System.Func<DbRawType, DbSimpleType, DbNumDef, object, SqlField> sqlConstantResolver, System.Func<Druid, SqlField> sqlColumnResolver)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				SqlField.CreateFunction (this.Left.CreateSqlCondition (sqlConstantResolver, sqlColumnResolver)),
				SqlField.CreateFunction (this.Right.CreateSqlCondition (sqlConstantResolver, sqlColumnResolver))
			);
		}


	}


}
