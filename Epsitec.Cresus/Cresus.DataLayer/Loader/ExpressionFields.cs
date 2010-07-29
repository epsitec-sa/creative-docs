using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Loader
{


	/// <summary>
	/// The <c>ExpressionFields</c> class provides extension methods that can retrieve the set of
	/// <see cref="Druid"/> of the <see cref="Field"/> present in an <see cref="Expression"/>.
	/// </summary>
	internal static class ExpressionFields
	{


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="Expression"/>.
		/// </summary>
		/// <param name="expression">The <see cref="Expression"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="expression"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this Expression expression)
		{
			expression.ThrowIfNull ("expression");

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


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="Operation"/>.
		/// </summary>
		/// <param name="operation">The <see cref="Operation"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="operation"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this Operation operation)
		{
			operation.ThrowIfNull ("operation");
			
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


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="Comparison"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="Comparison"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="comparison"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this Comparison comparison)
		{
			comparison.ThrowIfNull ("comparison");

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


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="UnaryOperation"/>.
		/// </summary>
		/// <param name="operation">The <see cref="UnaryOperation"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="operation"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this UnaryOperation operation)
		{
			operation.ThrowIfNull ("operation");

			return operation.Expression.GetFields ();
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="BinaryOperation"/>.
		/// </summary>
		/// <param name="operation">The <see cref="BinaryOperation"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="operation"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this BinaryOperation operation)
		{
			operation.ThrowIfNull ("operation");

			IEnumerable<Druid> left = operation.Left.GetFields ();
			IEnumerable<Druid> right = operation.Right.GetFields ();

			return left.Concat (right).Distinct ();
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="UnaryComparison"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="UnaryComparison"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="comparison"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this UnaryComparison comparison)
		{
			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			comparison.ThrowIfNull ("comparison");

			return ExpressionFields.GetFieldsHelper (comparison);
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="UnaryComparison"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="UnaryComparison"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		private static IEnumerable<Druid> GetFieldsHelper(UnaryComparison comparison)
		{
			yield return comparison.Field.FieldId;
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="ComparisonFieldField"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldField"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="comparison"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this ComparisonFieldField comparison)
		{
			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			comparison.ThrowIfNull ("comparison");

			return ExpressionFields.GetFieldsHelper (comparison);
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="ComparisonFieldField"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldField"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		private static IEnumerable<Druid> GetFieldsHelper(ComparisonFieldField comparison)
		{
			Druid id1 = comparison.Left.FieldId;
			Druid id2 = comparison.Right.FieldId;

			if (id1 == id2)
			{
				yield return id1;
			}
			else
			{
				yield return id1;
				yield return id2;
			}
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="ComparisonFieldValue"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldValue"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="comparison"/> is null.</exception>
		public static IEnumerable<Druid> GetFields(this ComparisonFieldValue comparison)
		{
			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			comparison.ThrowIfNull ("comparison");

			return ExpressionFields.GetFieldsHelper (comparison);
		}


		/// <summary>
		/// Gets the set of <see cref="Druid"/> of the <see cref="Field"/> present in an
		/// <see cref="ComparisonFieldValue"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="ComparisonFieldValue"/> whose <see cref="Druid"/> to get.</param>
		/// <returns>The set of <see cref="Druid"/>.</returns>
		private static IEnumerable<Druid> GetFieldsHelper(ComparisonFieldValue comparison)
		{
			yield return comparison.Left.FieldId;
		}


	}


}
