using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithField : BinaryComparison
	{


		public BinaryComparisonFieldWithField(Field left, BinaryComparator op, Field right)
			: base (left, op)
		{
			this.Right = right;
		}


		public Field Right
		{
			get;
			private set;
		}


		internal override DbSimpleCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = this.Left.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbCompare (this.Operator);
			DbTableColumn right = this.Right.CreateDbTableColumn (entity, dbTableColumnResolver);

			return new DbSimpleCondition (left, op, right);
		}


	}


}
