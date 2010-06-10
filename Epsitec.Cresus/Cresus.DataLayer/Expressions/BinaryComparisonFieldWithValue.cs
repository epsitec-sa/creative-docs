using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithValue : BinaryComparison
	{


		public BinaryComparisonFieldWithValue(Field left, BinaryComparator op, Constant right)
			: base (left, op)
		{
			this.Right = right;
		}


		public Constant Right
		{
			get;
			private set;
		}

		
		internal override DbCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = this.Left.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbCompare op = EnumConverter.ToDbCompare (this.Operator);
			object rightValue = this.Right.Value;
			DbRawType rightType = EnumConverter.ToDbRawType (this.Right.Type);

			return new DbCondition (left, op, rightValue, rightType);
		}


	}


}
