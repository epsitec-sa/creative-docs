using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


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


		protected override DbSimpleCondition CreateDbSimpleCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn left = this.Left.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbSimpleConditionOperator op = OperatorConverter.ToDbSimpleConditionOperator (this.Operator);
			DbTableColumn right = this.Right.CreateDbTableColumn (entity, dbTableColumnResolver);

			return new DbSimpleCondition (left, op, right);
		}


		internal override IEnumerable<Druid> GetFields()
		{
			return new Druid[] { this.Left.FieldId, this.Right.FieldId, };
		}


	}


}
