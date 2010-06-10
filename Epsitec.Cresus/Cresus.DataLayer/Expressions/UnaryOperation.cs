namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class UnaryOperation : Expression
	{


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


	}


}
