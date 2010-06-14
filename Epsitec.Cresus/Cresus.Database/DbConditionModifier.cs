using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{


	public class DbConditionModifier : DbAbstractCondition
	{


		public DbConditionModifier(DbConditionModifierOperator op, DbAbstractCondition condition) : base ()
		{
			this.Operator = op;
			this.Condition = condition;
		}


		public DbConditionModifierOperator Operator
		{
			get;
			private set;
		}


		public DbAbstractCondition Condition
		{
			get;
			private set;
		}


		internal override IEnumerable<DbTableColumn> Columns
		{
			get
			{
				return this.Condition.Columns;
			}
		}


		internal override void ReplaceTableColumns(System.Func<DbTableColumn, DbTableColumn> replaceOperation)
		{
			this.Condition.ReplaceTableColumns (replaceOperation);
		}


		internal override SqlField CreateSqlField()
		{
			SqlField field = this.Condition.CreateSqlField ();
			SqlFunctionCode op = this.ToSqlFunctionType (this.Operator);

			SqlFunction function = new SqlFunction (op, field);

			return SqlField.CreateFunction (function);
		}


		private SqlFunctionCode ToSqlFunctionType(DbConditionModifierOperator op)
		{
			switch (op)
			{
				case DbConditionModifierOperator.Not:
					return SqlFunctionCode.LogicNot;
			}

			throw new System.ArgumentException ("Unsupported comparison: " + op);
		}


	}


}
