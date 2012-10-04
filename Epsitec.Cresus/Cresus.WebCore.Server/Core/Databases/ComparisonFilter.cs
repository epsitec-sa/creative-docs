using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal class ComparisonFilter : Filter
	{


		public ComparisonFilter(Column column, BinaryComparator comparator, object value)
			: base (column)
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


		protected override DataExpression ToCondition(EntityColumnMetadata column, AbstractEntity example)
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

