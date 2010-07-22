﻿using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using System.Collections.Generic;


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


		internal override DbAbstractCondition CreateDbAbstractCondition(AbstractEntity entity, System.Func<Druid, DbTableColumn> dbTableColumnResolver)
		{
			DbAbstractCondition condition = this.Expression.CreateDbAbstractCondition (entity, dbTableColumnResolver);
			DbConditionModifierOperator op = Converter.ToDbConditionModifierOperator (this.Operator);

			return new DbConditionModifier (op, condition);
		}


		internal override IEnumerable<Druid> GetFields()
		{
			return this.Expression.GetFields ();
		}


	}


}
