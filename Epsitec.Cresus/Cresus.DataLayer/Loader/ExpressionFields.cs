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

			if (expression is Operation)
			{
				return (expression as Operation).GetFields ();
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


		private static IEnumerable<Druid> GetFields(this Operation operation)
		{
			if (operation is UnaryOperation)
			{
				return (operation as UnaryOperation).GetFields ();
			}
			else if (operation is BinaryOperation)
			{
				return (operation as BinaryOperation).GetFields ();
			}
			else
			{
				throw new System.NotSupportedException ("operation is not supported");
			}
		}


		private static IEnumerable<Druid> GetFields(this Comparison comparison)
		{
			if (comparison is UnaryComparison)
			{
				return (comparison as UnaryComparison).GetFields ();
			}
			else if (comparison is ComparisonFieldField)
			{
				return (comparison as ComparisonFieldField).GetFields ();
			}
			else if (comparison is ComparisonFieldValue)
			{
				return (comparison as ComparisonFieldValue).GetFields ();
			}
			else
			{
				throw new System.NotSupportedException ("comparison is not supported");
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


		private static IEnumerable<Druid> GetFields(this UnaryComparison comparison)
		{
			yield return comparison.Field.FieldId;
		}


		private static IEnumerable<Druid> GetFields(this ComparisonFieldField comparison)
		{
			yield return comparison.Left.FieldId;
			yield return comparison.Right.FieldId;
		}


		private static IEnumerable<Druid> GetFields(this ComparisonFieldValue comparison)
		{
			yield return comparison.Left.FieldId;
		}


	}


}
