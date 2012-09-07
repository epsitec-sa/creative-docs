using System;

using System.Collections.Generic;

using System.Linq.Expressions;


public static class LambdaComputer
{


	// The code here has been inspired from this blog post :
	// http://blogs.msdn.com/b/mattwar/archive/2007/08/01/linq-building-an-iqueryable-provider-part-iii.aspx


	public static Expression Compute(Expression expression, Func<Expression, bool> isExpressionComputable)
	{
		// The basic idea here is first to identify the expressions within the given expression that
		// can be computed right now (i.e. captured variables, member access, method calls, etc.)
		// and then to replace them with the result of their execution.

		var finder = new ComputableExpressionFinder (isExpressionComputable);
		var computableExpressions = finder.Find (expression);

		var computer = new ComputableExpressionComputer (computableExpressions);
		return computer.Compute (expression);
	}


	private class ComputableExpressionComputer : ExpressionVisitor
	{


		public ComputableExpressionComputer(ISet<Expression> computableExpressions)
		{
			this.computableExpressions = computableExpressions;
		}


		public Expression Compute(Expression expression)
		{
			return this.Visit (expression);
		}


		public override Expression Visit(Expression expression)
		{
			return this.computableExpressions.Contains (expression)
				? this.Evaluate (expression)
				: base.Visit (expression);
		}


		private Expression Evaluate(Expression expression)
		{
			if (expression == null)
			{
				return expression;
			}
			else if (expression.NodeType == ExpressionType.Constant)
			{
				return expression;
			}
			else
			{
				var value = Expression.Lambda (expression).Compile ().DynamicInvoke (null);
				var type = expression.Type;

				return Expression.Constant (value, type);
			}
		}


		private readonly ISet<Expression> computableExpressions;


	}


	private class ComputableExpressionFinder : ExpressionVisitor
	{


		public ComputableExpressionFinder(Func<Expression, bool> isExpressionComputable)
		{
			this.isExpressionComputable = isExpressionComputable;
		}

		
		internal HashSet<Expression> Find(Expression expression)
		{
			this.isComputableStack = new Stack<bool> ();
			this.isComputableStack.Push (true);

			this.computableExpressions = new HashSet<Expression> ();
			
			this.Visit (expression);
			
			return this.computableExpressions;
		}


		public override Expression Visit(Expression expression)
		{
			if (expression == null)
			{
				return expression;
			}

			// As the ExpressionVisitor class forces us to return an Expression from the Visit
			// method, we can't return a bool which would have been very useful. That's why we use
			// a stack of bools to store the values that should have been returned if we could have
			// done so.

			// If we can't compute the current expression then the parent expression cannot be
			// computed.
			if (!this.isExpressionComputable (expression))
			{
				this.isComputableStack.Pop ();
				this.isComputableStack.Push (false);

				return expression;
			}

			// We start by assuming that the current expression can be computed.
			this.isComputableStack.Push (true);

			// We walk the tree with the children of the current expression to see if they can
			// be computed.
			base.Visit (expression);

			if (!this.isComputableStack.Pop ())
			{
				// If one child of the current expression cannot be computed, which implies that the
				// parent expression cannot be computed.

				this.isComputableStack.Pop ();
				this.isComputableStack.Push (false);
			}
			else
			{
				// All children of the current expression can be computed and so can the current
				// expression, so we add it to the list of expressions that can be computed.

				this.computableExpressions.Add (expression);
			}

			return expression;
		}


		private readonly Func<Expression, bool> isExpressionComputable;


		private HashSet<Expression> computableExpressions;


		private Stack<bool> isComputableStack;


	}


}