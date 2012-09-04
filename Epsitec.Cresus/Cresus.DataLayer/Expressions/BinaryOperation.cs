using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>BinaryOperation</c> class represent a logical operation on two <see cref="DataExpression"/>,
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
		public BinaryOperation(DataExpression left, BinaryOperator op, DataExpression right)
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
		public DataExpression Left
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
		public DataExpression Right
		{
			get;
			private set;
		}


		internal override SqlFunction CreateSqlCondition(SqlFieldBuilder builder)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Operator),
				SqlField.CreateFunction (this.Left.CreateSqlCondition (builder)),
				SqlField.CreateFunction (this.Right.CreateSqlCondition (builder))
			);
		}


		internal override void CheckFields(FieldChecker checker)
		{
			this.Left.CheckFields (checker);
			this.Right.CheckFields (checker);
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			this.Left.AddEntities (entities);
			this.Right.AddEntities (entities);
		}


	}


}
