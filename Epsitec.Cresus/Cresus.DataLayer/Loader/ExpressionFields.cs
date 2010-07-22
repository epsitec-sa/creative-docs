using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	internal static class ExpressionFields
	{


		public static IEnumerable<Druid> GetFields(this Expression expression)
		{
			if (expression is UnaryOperation)
			{
				return (expression as UnaryOperation).GetFields ();
			}
			else if (expression is BinaryOperation)
			{
				return (expression as BinaryOperation).GetFields ();
			}
			else if (expression is Comparison)
			{
				return (expression as Comparison).GetFields ();
			}
			else
			{
				throw new System.NotSupportedException ("expression is not supported");
			}
		}


		private static IEnumerable<Druid> GetFields(this UnaryOperation operation)
		{
			return operation.Expression.GetFields ();
		}


		private static IEnumerable<Druid> GetFields(this BinaryOperation operation)
		{
			IEnumerable<Druid> left = operation.Left.GetFields ();
			IEnumerable<Druid> right = operation.Right.GetFields ();

			return left.Concat (right);
		}


		private static IEnumerable<Druid> GetFields(this Comparison comparison)
		{
			if (comparison is UnaryComparison)
			{
				return (comparison as UnaryComparison).GetFields ();
			}
			else if (comparison is BinaryComparison)
			{
				return (comparison as BinaryComparison).GetFields ();
			}
			else
			{
				throw new System.NotSupportedException ("comparison is not supported");
			}
		}


		private static IEnumerable<Druid> GetFields(this UnaryComparison comparison)
		{
			yield return comparison.Field.FieldId;
		}


		private static IEnumerable<Druid> GetFields(this BinaryComparison comparison)
		{
			if (comparison is BinaryComparisonFieldWithField)
			{
				return (comparison as BinaryComparisonFieldWithField).GetFields ();
			}
			else if (comparison is BinaryComparisonFieldWithValue)
			{
				return (comparison as BinaryComparisonFieldWithValue).GetFields ();
			}
			else
			{
				throw new System.NotSupportedException ("comparison is not supported");
			}
		}


		private static IEnumerable<Druid> GetFields(this BinaryComparisonFieldWithField comparison)
		{
			yield return comparison.Left.FieldId;
			yield return comparison.Right.FieldId;
		}


		private static IEnumerable<Druid> GetFields(this BinaryComparisonFieldWithValue comparison)
		{
			yield return comparison.Left.FieldId;
		}


	}


}
