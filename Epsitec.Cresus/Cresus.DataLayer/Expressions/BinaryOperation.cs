namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryOperation : Operation
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


	}


}
