using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


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


		protected override DbSimpleCondition CreateDbSimpleCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbTableColumn field = this.Field.CreateDbTableColumn (entity, dbTableColumnResolver);
			DbSimpleConditionOperator op = Converter.ToDbSimpleConditionOperator (this.Operator);

			return new DbSimpleCondition (field, op);
		}


		internal override IEnumerable<Druid> GetFields()
		{
			return new Druid[] { this.Field.FieldId, };
		}

	}


}
