using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{
	

	public class UnaryComparison : Comparison
	{


		public UnaryComparison(UnaryComparator op, Field field)
			: base ()
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


		internal override DbCondition CreateDbCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn field = this.Field.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbCompare op = EnumConverter.ToDbCompare (this.Operator);

			return new DbCondition (field, op);
		}


	}


}
