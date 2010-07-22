using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Expressions;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal static class ExpressionConversion
	{


		public static DbAbstractCondition CreateDbCondition(this Expression expression, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			if (expression is Operation)
			{
				return (expression as Operation).CreateDbCondition (dbTableColumnResolver);
			}
			else if (expression is Comparison)
			{
				return (expression as Comparison).CreateDbCondition (dbTableColumnResolver);
			}
			else
			{
				throw new System.NotSupportedException ("expression is not supported");
			}
		}


		private static DbAbstractCondition CreateDbCondition(this Operation operation, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			if (operation is UnaryOperation)
			{
				return (operation as UnaryOperation).CreateDbCondition (dbTableColumnResolver);
			}
			else if (operation is BinaryOperation)
			{
				return (operation as BinaryOperation).CreateDbCondition (dbTableColumnResolver);
			}
			else
			{
				throw new System.NotSupportedException ("operation is not supported");
			}
		}


		private static DbAbstractCondition CreateDbCondition(this Comparison comparison, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			if (comparison is UnaryComparison)
			{
				return (comparison as UnaryComparison).CreateDbCondition (dbTableColumnResolver);
			}
			else if (comparison is ComparisonFieldField)
			{
				return (comparison as ComparisonFieldField).CreateDbCondition (dbTableColumnResolver);
			}
			else if (comparison is ComparisonFieldValue)
			{
				return (comparison as ComparisonFieldValue).CreateDbCondition (dbTableColumnResolver);
			}
			else
			{
				throw new System.NotSupportedException ("comparison is not supported");
			}
		}


		private static DbAbstractCondition CreateDbCondition(this UnaryOperation operation, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbAbstractCondition condition = operation.Expression.CreateDbCondition (dbTableColumnResolver);
			DbConditionModifierOperator op = EnumConverter.ToDbConditionModifierOperator (operation.Operator);

			return new DbConditionModifier (op, condition);
		}


		private static DbAbstractCondition CreateDbCondition(this BinaryOperation operation, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbAbstractCondition left = operation.Left.CreateDbCondition (dbTableColumnResolver);
			DbAbstractCondition right = operation.Right.CreateDbCondition (dbTableColumnResolver);

			DbConditionCombinerOperator op = EnumConverter.ToDbConditionCombinerOperator (operation.Operator);

			return new DbConditionCombiner (op, left, right);
		}


		private static DbAbstractCondition CreateDbCondition(this UnaryComparison comparison, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn field = comparison.Field.GetDbTableColumn (dbTableColumnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);

			return new DbSimpleCondition (field, op);
		}


		private static DbAbstractCondition CreateDbCondition(this ComparisonFieldField comparison, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = comparison.Left.GetDbTableColumn (dbTableColumnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);
			DbTableColumn right = comparison.Right.GetDbTableColumn (dbTableColumnResolver);

			return new DbSimpleCondition (left, op, right);
		}


		private static DbAbstractCondition CreateDbCondition(this ComparisonFieldValue comparison, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = comparison.Left.GetDbTableColumn (dbTableColumnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbSimpleConditionOperator (comparison.Operator);
			object rightValue = comparison.Right.Value;
			DbRawType rightType = EnumConverter.ToDbRawType (comparison.Right.Type);

			return new DbSimpleCondition (left, op, rightValue, rightType);
		}


		private static DbTableColumn GetDbTableColumn(this Field field, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			return dbTableColumnResolver (field.FieldId);
		}


	}


}
