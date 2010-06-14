using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	

	public class UnaryComparison : Comparison
	{


		public UnaryComparison(Field field, UnaryComparator op) : this (op, field)
		{
		}


		public UnaryComparison(UnaryComparator op, Field field) : base ()
		{
			this.Operator = op;
			this.Field = field;
		}


		public UnaryComparator Operator
		{
			get;
			private set;
		}


		public Field Field
		{
			get;
			private set;
		}


		internal override DbSimpleCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn field = this.Field.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbSimpleConditionOperator op = EnumConverter.ToDbCompare (this.Operator);

			return new DbSimpleCondition (field, op);
		}


	}


}
