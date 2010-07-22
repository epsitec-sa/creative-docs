namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithField : Comparison
	{


		public BinaryComparisonFieldWithField(Field left, BinaryComparator op, Field right) : base ()
		{
			this.Left = left;
			this.Operator = op;
			this.Right = right;
		}


		public Field Left
		{
			get;
			private set;
		}


		public BinaryComparator Operator
		{
			get;
			private set;
		}


		public Field Right
		{
			get;
			private set;
		}


	}


}
