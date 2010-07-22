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

	}


}
