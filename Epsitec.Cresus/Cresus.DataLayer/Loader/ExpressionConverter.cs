using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Schema;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>ExpressionConverter</c> class contains methods that are used to convert
	/// <see cref="Expression"/> to <see cref="DbAbstractCondition"/>.
	/// </summary>
	internal class ExpressionConverter
	{



		public ExpressionConverter(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		private DataConverter DataConverter
		{
			get
			{
				return this.DataContext.DataConverter;
			}
		}
		

		/// <summary>
		/// Converts an <see cref="UnaryOperation"/> to an equivalent <see cref="DbAbstractCondition"/>,
		/// using a resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="operation">The <see cref="UnaryOperation"/> to convert.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="operation"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		public DbAbstractCondition CreateDbCondition(UnaryOperation operation, System.Func<Druid, DbTableColumn> columnResolver)
		{
			operation.ThrowIfNull ("operation");
			columnResolver.ThrowIfNull ("columnResolver");

			DbAbstractCondition condition = operation.Expression.CreateDbCondition (this, columnResolver);
			DbConditionModifierOperator op = EnumConverter.ToDbConditionModifierOperator (operation.Operator);

			return new DbConditionModifier (op, condition);
		}


		/// <summary>
		/// Converts an <see cref="BinaryOperation"/> to an equivalent <see cref="DbAbstractCondition"/>,
		/// using a resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="operation">The <see cref="BinaryOperation"/> to convert.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="operation"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		public DbAbstractCondition CreateDbCondition(BinaryOperation operation, System.Func<Druid, DbTableColumn> columnResolver)
		{
			operation.ThrowIfNull ("operation");
			columnResolver.ThrowIfNull ("columnResolver");

			DbAbstractCondition left = operation.Left.CreateDbCondition (this, columnResolver);
			DbAbstractCondition right = operation.Right.CreateDbCondition (this, columnResolver);

			DbConditionCombinerOperator op = EnumConverter.ToDbConditionCombinerOperator (operation.Operator);

			return new DbConditionCombiner (op, left, right);
		}


		/// <summary>
		/// Converts an <see cref="UnaryComparison"/> to an equivalent <see cref="DbAbstractCondition"/>,
		/// using a resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="UnaryComparison"/> to convert.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="comparison"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		public DbAbstractCondition CreateDbCondition(UnaryComparison comparison, System.Func<Druid, DbTableColumn> columnResolver)
		{
			comparison.ThrowIfNull ("comparison");
			columnResolver.ThrowIfNull ("columnResolver");

			DbTableColumn field = this.GetDbTableColumn (comparison.Field, columnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);

			return new DbSimpleCondition (field, op);
		}


		/// <summary>
		/// Converts an <see cref="ComparisonFieldField"/> to an equivalent <see cref="DbAbstractCondition"/>,
		/// using a resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldField"/> to convert.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="comparison"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		public DbAbstractCondition CreateDbCondition(ComparisonFieldField comparison, System.Func<Druid, DbTableColumn> columnResolver)
		{
			comparison.ThrowIfNull ("comparison");
			columnResolver.ThrowIfNull ("columnResolver");

			DbTableColumn left = this.GetDbTableColumn (comparison.Left, columnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);
			DbTableColumn right = this.GetDbTableColumn (comparison.Right, columnResolver);

			return new DbSimpleCondition (left, op, right);
		}


		/// <summary>
		/// Converts an <see cref="ComparisonFieldValue"/> to an equivalent <see cref="DbAbstractCondition"/>,
		/// using a resolver to convert the <see cref="Druid"/> of the fields to the appropriate
		/// <see cref="DbTableColumn"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldValue"/> to convert.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The new <see cref="DbAbstractCondition"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="comparison"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		public DbAbstractCondition CreateDbCondition(ComparisonFieldValue comparison, System.Func<Druid, DbTableColumn> columnResolver)
		{
			comparison.ThrowIfNull ("comparison");
			columnResolver.ThrowIfNull ("columnResolver");

			DbTableColumn left = this.GetDbTableColumn (comparison.Left, columnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);
			object rightValue = comparison.Right.Value;
			DbRawType rightType = EnumConverter.ToDbRawType (comparison.Right.Type);

			object convertedRightValue = this.DataConverter.ToDatabaseValue (rightType, rightValue);
			object convertedRightType = this.DataConverter.ToDatabaseType (rightType);

			return new DbSimpleCondition (left, op, rightValue, rightType);
		}


		/// <summary>
		/// Gets the <see cref="DbTableColumn"/> corresponding to the given <see cref="Field"/> out
		/// of the resolver.
		/// </summary>
		/// <param name="field">The <see cref="Field"/> whose corresponding <see cref="DbTableColumn"/> to get.</param>
		/// <param name="columnResolver">The function used to resolve the <see cref="DbTableColumn"/> given an <see cref="Druid"/>.</param>
		/// <returns>The <see cref="DbTableColumn"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="field"/> is null.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="columnResolver"/> is null.</exception>
		private DbTableColumn GetDbTableColumn(Field field, System.Func<Druid, DbTableColumn> columnResolver)
		{
			field.ThrowIfNull ("field");
			columnResolver.ThrowIfNull ("columnResolver");

			return columnResolver (field.FieldId);
		}


	}


}
