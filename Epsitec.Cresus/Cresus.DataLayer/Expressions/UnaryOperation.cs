using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class UnaryOperation : Expression
	{


		public UnaryOperation(Expression expression, UnaryOperator op) : this (op, expression)
		{
		}


		public UnaryOperation(UnaryOperator op, Expression expression) : base()
		{
			this.Operator = op;
			this.Expression = expression;
		}


		public UnaryOperator Operator
		{
			get;
			private set;
		}


		public Expression Expression
		{
			get;
			private set;
		}


		internal override DbSelectCondition CreateDbSelectCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			throw new System.NotImplementedException ();
		}


	}


}
