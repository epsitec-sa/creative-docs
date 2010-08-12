using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	/// <summary>
	/// The <c>UnaryOperation</c> class represents a logical operation on a single
	/// <see cref="Expression"/>, such as (Not (a > b)).
	/// </summary>
	public class UnaryOperation : Operation
	{
		

		/// <summary>
		/// Creates a new <c>UnaryOperation</c>.
		/// </summary>
		/// <param name="op">The operation to apply to the <see cref="Expression"/>.</param>
		/// <param name="expression">The <see cref="Expression"/> on which to apply the <see cref="UnaryOperator"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="expression"/> is null.</exception>
		public UnaryOperation(UnaryOperator op, Expression expression) : base()
		{
			expression.ThrowIfNull ("expression");
			
			this.Operator = op;
			this.Expression = expression;
		}
         

		/// <summary>
		/// Gets the <see cref="UnaryOperator"/> of the current instance.
		/// </summary>
		public UnaryOperator Operator
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="Expression"/> of the current instance.
		/// </summary>
		public Expression Expression
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
			return ExpressionFields.GetFields (this);
		}


		/// <summary>
		/// Converts this instance to an equivalent <see cref="DbAbstractCondition"/>, using a
		/// resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="expressionConverter">The <see cref="ExpressionConverter"/> used to convert this instance.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="expressionConverter"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is <c>null</c>.</exception>
		internal override DbAbstractCondition CreateDbCondition(ExpressionConverter expressionConverter, System.Func<Druid, DbTableColumn> columnResolver)
		{
			expressionConverter.ThrowIfNull ("expressionConverter");
			columnResolver.ThrowIfNull ("columnResolver");

			return expressionConverter.CreateDbCondition (this, columnResolver);
		}


	}


}
