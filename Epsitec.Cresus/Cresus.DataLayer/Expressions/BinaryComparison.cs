namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparison : Expression
	{


		public BinaryComparison(Value left, BinaryComparator op, Value right) : base ()
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		public Value Left
		{
			get;
			private set;
		}


		public BinaryComparator Operator
		{
			get;
			private set;
		}


		public Value Right
		{
			get;
			private set;
		}


	}


}
