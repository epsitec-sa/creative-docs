﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;


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



		internal override DbConditionCombiner CreateDbSelectCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			throw new System.NotImplementedException ();
		}


	}


}
