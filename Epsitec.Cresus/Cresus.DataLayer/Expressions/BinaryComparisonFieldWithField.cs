using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class BinaryComparisonFieldWithField : BinaryComparison
	{


		public BinaryComparisonFieldWithField(Field left, BinaryComparator op, Field right)
			: base (left, op)
		{
			this.Right = right;
		}


		public Field Right
		{
			get;
			private set;
		}


	}


}
