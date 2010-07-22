using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryOperation : Expression
	{


		public BinaryOperation(Expression left, BinaryOperator op, Expression right)
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		public Expression Left
		{
			get;
			private set;
		}


		public BinaryOperator Operator
		{
			get;
			private set;
		}


		public Expression Right
		{
			get;
			private set;
		}



		internal override DbAbstractCondition CreateDbAbstractCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbAbstractCondition left = this.Left.CreateDbAbstractCondition (entity, dbTableColumnResolver);
			DbAbstractCondition right = this.Right.CreateDbAbstractCondition (entity, dbTableColumnResolver);

			DbConditionCombinerOperator op = Converter.ToDbConditionCombinerOperator (this.Operator);

			return new DbConditionCombiner (op, left, right);
		}


		internal override IEnumerable<Druid> GetFields()
		{
			IEnumerable<Druid> left = this.Left.GetFields ();
			IEnumerable<Druid> right = this.Right.GetFields ();

			return left.Concat (right);
		}

	}


}
