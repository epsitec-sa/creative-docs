using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.DataLayer.Expressions;

using System;

namespace Epsitec.Cresus.Core.Metadata
{
	public class EntityColumnComparisonFilter : EntityColumnFilter
	{
		public EntityColumnComparisonFilter(BinaryComparator comparator, object value)
		{
			this.comparator = comparator;
			this.value = value;
		}

		public BinaryComparator Comparator
		{
			get
			{
				return this.comparator;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public override Expression ToCondition(EntityColumn column, AbstractEntity example)
		{
			var fieldEntity = column.GetLeafEntity (example, NullNodeAction.CreateMissing);
			var fieldId = column.GetLeafFieldId ();

			var fieldNode = new ValueField (fieldEntity, fieldId);

			if (this.Value == null)
			{
				UnaryComparator unaryComparator;

				switch (this.comparator)
				{
					case BinaryComparator.IsEqual:
					case BinaryComparator.IsLike:
					case BinaryComparator.IsLikeEscape:
						unaryComparator = UnaryComparator.IsNull;
						break;

					case BinaryComparator.IsNotEqual:
					case BinaryComparator.IsNotLike:
					case BinaryComparator.IsNotLikeEscape:
						unaryComparator = UnaryComparator.IsNotNull;
						break;

					default:
						throw new NotSupportedException ();
				}

				return new UnaryComparison (fieldNode, unaryComparator);
			}
			else
			{
				var constantNode = new Constant (this.Value);

				return new BinaryComparison (fieldNode, this.Comparator, constantNode);
			}	
		}

		private readonly BinaryComparator comparator;
		private readonly object value;
	}
}
