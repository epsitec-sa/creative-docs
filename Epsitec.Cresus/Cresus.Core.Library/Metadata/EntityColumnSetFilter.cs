using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Expressions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public class EntityColumnSetFilter : EntityColumnFilter
	{
		public EntityColumnSetFilter(SetComparator comparator, IEnumerable<object> values)
		{
			this.comparator = comparator;
			this.values = values.ToList ();
		}

		public SetComparator Comparator
		{
			get
			{
				return this.comparator;
			}
		}

		public IEnumerable<object> Values
		{
			get
			{
				return this.values;
			}
		}

		public override Expression ToCondition(EntityColumn column, AbstractEntity example)
		{
			var fieldEntity = column.GetLeafEntity (example, NullNodeAction.CreateMissing);
			var fieldId = column.GetLeafFieldId ();

			var fieldNode = new ValueField (fieldEntity, fieldId);
			
			// We must do all this weird stuff because of the behavior of NULL values in SQL, which
			// implies that if we simply translate this to SQL will give incorrect results in the
			// case where we have NULL in the set. If we have NULL in the set, the rows where the
			// column is NULL will never be returned.

			// Firstly, we convert the sequence of values into a sequence of expression using the
			// following rule.
			// - if the value is not null, we generate "column = value"
			// - if the value is null, we generate "column IS NULL"
			var expressions = this.Values.Select (v => v == null
				? (Expression) new UnaryComparison (fieldNode, UnaryComparator.IsNull)
				: new BinaryComparison (fieldNode, BinaryComparator.IsEqual, new Constant (v))
			);

			// Now we join all these expressions together with OR statements in order to get a
			// single expression that returns true if the column is in the set. For instance we
			// could get ((column = 'A') OR (column IS NULL) OR (column = 'B') OR ...)
			var expression = expressions.Aggregate
			(
				(e1, e2) => new BinaryOperation (e1, BinaryOperator.Or, e2)
			);

			// Now if what we want is a condition that check that the element is not in the set, we
			// must negate the whole expression.
			if (this.Comparator == SetComparator.NotIn)
			{
				// So here we basically negate the expression, which is straightforward.
				expression = new UnaryOperation (UnaryOperator.Not, expression);

				// And now is the last trick that we must make. If all the values in the set are
				// not null, our expression must return true for the rows where the value of the
				// column is NULL. In this case, the expression that we have is equivalent to
				// NOT ((NULL = 'A') OR (NULL = 'B') OR ...) which evaluates to NULL, which not TRUE
				// which implies the row will not be in the result set. Because of that, we must add
				// a check that checks if the column is null and return TRUE in this case. So we
				// generate an expression like that :
				// ((NOT ((column = 'A') OR (column = 'B') OR ...)) OR column IS NULL)
				if (this.values.All (v => v != null))
				{
					expression = new BinaryOperation
					(
						expression,
						BinaryOperator.Or,
						new UnaryComparison (fieldNode, UnaryComparator.IsNull)
					);
				}
			}

			return expression;
		}

		private readonly SetComparator comparator;
		private readonly IEnumerable<object> values;
	}
}
