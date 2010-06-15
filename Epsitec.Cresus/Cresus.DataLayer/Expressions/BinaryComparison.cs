using Epsitec.Common.Support;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class BinaryComparison : Comparison
	{


		protected BinaryComparison(Field left, BinaryComparator op) : base ()
		{
			this.Left = left;
			this.Operator = op;
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


	}


}
