using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;

using System.Reflection;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	internal sealed class LambdaConverter : ExpressionVisitor
	{


		public static DataExpression Convert<T>(T entity, Expression<Func<T, bool>> lambda)
			where T : AbstractEntity
		{
			return LambdaConverter.Convert ((AbstractEntity) entity, (LambdaExpression) lambda);
		}


		public static DataExpression Convert(AbstractEntity entity, LambdaExpression lambda)
		{
			LambdaConverter.Check (entity, lambda);

			var converter = new LambdaConverter (entity);

			converter.Visit (lambda.Body);

			return converter.GetResult ();
		}


		private static void Check(AbstractEntity entity, LambdaExpression lambda)
		{
			entity.ThrowIfNull ("entity");
			lambda.ThrowIfNull ("lambda");

			if (lambda.Parameters.Count != 1)
			{
				throw new ArgumentException ("Wrong number of arguments in lambda");
			}

			var entityType = entity.GetType ();
			var parameterType = lambda.Parameters[0].Type;

			if (!parameterType.IsSubclassOf (typeof(AbstractEntity)))
			{
				var message = "Wrong type for lambda parameter";

				throw new ArgumentException (message);
			}

			if (!parameterType.IsAssignableFrom (entityType))
			{
				var message = "Type mismatch between entity type and lambda parameter type.";

				throw new ArgumentException (message);
			}

			if (lambda.ReturnType != typeof (bool))
			{
				throw new ArgumentException ("Wrong return type for lambda");
			}
		}


		public LambdaConverter(AbstractEntity entity)
		{
			this.entity = entity;
			this.results = new Stack<object> ();
		}


		public DataExpression GetResult()
		{
			if (this.results.Count != 1)
			{
				throw new Exception ();
			}

			return (DataExpression) this.PeekResult ();
		}


		private void PushResult(object result)
		{
			this.results.Push (result);
		}


		private object PopResult()
		{
			return this.results.Pop ();
		}


		private object PeekResult()
		{
			return this.results.Peek ();
		}


		private object VisitAndPop(Expression node)
		{
			this.Visit (node);
			return this.PopResult ();
		}


		private Expression PushAndReturn(Expression node, object dataExpression)
		{
			this.PushResult (dataExpression);
			return node;
		}


		protected override Expression VisitUnary(UnaryExpression node)
		{
			var nodeType = node.NodeType;
			var operand = this.VisitAndPop (node.Operand);

			DataExpression dataExpression;

			if (this.IsUnaryOperator (nodeType))
			{
				dataExpression = new UnaryOperation
				(
					this.GetUnaryOperator (nodeType),
					(DataExpression) operand
				);
			}
			else
			{
				throw new NotSupportedException ();
			}
			
			return this.PushAndReturn (node, dataExpression);
		}


		private bool IsUnaryOperator(ExpressionType type)
		{
			return type == ExpressionType.Not;
		}


		private UnaryOperator GetUnaryOperator(ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.Not:
					return UnaryOperator.Not;

				default:
					throw new NotSupportedException ();
			}
		}


		protected override Expression VisitBinary(BinaryExpression node)
		{
			// Here we have a fugly special case because of the string comparisons. .NET won't
			// allow us to create expressions like ("foo" > "bar") because the > operator is not
			// defined for strings. Therefore, if we want to have order comparisons between
			// strings, we have to use something like (SqlMethods.CompareTo("Foo", "Bar") > 0)
			// and interpret them as something like "Foo" > "Bar". Here we look at such special
			// cases and if this is one, we bypass the regular logic and convert it directly.

			var dataExpression = this.IsStringComparison (node)
				? this.ConvertStringComparison (node)
				: this.ConvertRegularComparison (node);

			return this.PushAndReturn (node, dataExpression);
		}


		private bool IsStringComparison(BinaryExpression node)
		{
			var case1 = this.IsCallToCompareTo (node.Left) && this.IsZeroConstant (node.Right);
			var case2 = this.IsZeroConstant (node.Left) && this.IsCallToCompareTo (node.Right);

			return (case1 || case2) && this.IsBinaryComparator (node.NodeType);
		}


		private bool IsZeroConstant(Expression node)
		{
			return node.NodeType == ExpressionType.Constant
				&& ((ConstantExpression) node).Type == typeof (int)
				&& (int) ((ConstantExpression) node).Value == 0;
		}


		private bool IsCallToCompareTo(Expression node)
		{
			return node.NodeType == ExpressionType.Call
				&& ((MethodCallExpression) node).Method == SqlMethods.CompareToMethodInfo;
		}


		private DataExpression ConvertStringComparison(BinaryExpression node)
		{
			var left= node.Left;
			var nodeType = node.NodeType;
			var right = node.Right;

			// If we have a string comparison in the "wrong" order (the zero constant on the left
			// and the call to CompareTo on the right), we swap the sides of the expression so later
			// on we can assume that it is in the order that we expect.

			var swap = this.IsZeroConstant (left)
				&& this.IsBinaryComparator (nodeType)
				&& this.IsCallToCompareTo (right);

			if (swap)
			{
				nodeType = this.RevertBinaryComparator (nodeType);
				var tmp = left;
				left = right;
				right = tmp;
			}

			// If we have a string comparison in the "right" order, we replace it by an equivalent
			// comparison.

			var isStringCompareTo = this.IsCallToCompareTo (left)
				&& this.IsBinaryComparator (nodeType)				
				&& this.IsZeroConstant (right);

			if (!isStringCompareTo)
			{
				throw new NotSupportedException ();
			}

			var callToCompareTo = (MethodCallExpression) left;

			var leftOperand = this.VisitAndPop (callToCompareTo.Arguments[0]);
			var rightOperand = this.VisitAndPop (callToCompareTo.Arguments[1]);

			return new BinaryComparison
			(
				(Value) leftOperand,
				this.GetBinaryComparator (nodeType),
				(Value) rightOperand
			);
		}



		private DataExpression ConvertRegularComparison(BinaryExpression node)
		{
			var left = this.VisitAndPop (node.Left);
			var nodeType = node.NodeType;
			var right = this.VisitAndPop (node.Right);

			if (this.IsBinaryComparator (nodeType))
			{
				return new BinaryComparison
				(
					(Value) left,
					this.GetBinaryComparator (nodeType),
					(Value) right
				);
			}
			else if (this.IsBinaryOperator (nodeType))
			{
				return new BinaryOperation
				(
					(DataExpression) left,
					this.GetBinaryOperator (nodeType),
					(DataExpression) right
				);
			}
			
			throw new NotSupportedException ();
		}


		private bool IsBinaryComparator(ExpressionType type)
		{
			return type == ExpressionType.Equal
				|| type == ExpressionType.NotEqual
				|| type == ExpressionType.GreaterThan
				|| type == ExpressionType.GreaterThanOrEqual
				|| type == ExpressionType.LessThan
				|| type == ExpressionType.LessThanOrEqual;
		}


		private BinaryComparator GetBinaryComparator(ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.Equal:
					return BinaryComparator.IsEqual;
					
				case ExpressionType.NotEqual:
					return BinaryComparator.IsNotEqual;
					
				case ExpressionType.GreaterThan:
					return BinaryComparator.IsGreater;
					
				case ExpressionType.GreaterThanOrEqual:
					return BinaryComparator.IsGreaterOrEqual;
					
				case ExpressionType.LessThan:
					return BinaryComparator.IsLower;
					
				case ExpressionType.LessThanOrEqual:
					return BinaryComparator.IsLowerOrEqual;

				default:
					throw new NotSupportedException ();
			}
		}


		private ExpressionType RevertBinaryComparator(ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.Equal:
					return ExpressionType.Equal;

				case ExpressionType.NotEqual:
					return ExpressionType.NotEqual;

				case ExpressionType.GreaterThan:
					return ExpressionType.LessThan;

				case ExpressionType.GreaterThanOrEqual:
					return ExpressionType.LessThanOrEqual;

				case ExpressionType.LessThan:
					return ExpressionType.GreaterThan;

				case ExpressionType.LessThanOrEqual:
					return ExpressionType.GreaterThanOrEqual;

				default:
					throw new NotSupportedException ();
			}
		}


		private bool IsBinaryOperator(ExpressionType type)
		{
			return type == ExpressionType.AndAlso
				|| type == ExpressionType.OrElse;
		}


		private BinaryOperator GetBinaryOperator(ExpressionType type)
		{
			switch (type)
			{
				case ExpressionType.AndAlso:
					return BinaryOperator.And;

				case ExpressionType.OrElse:
					return BinaryOperator.Or;

				default:
					throw new NotSupportedException ();
			}
		}


		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var method = node.Method;
			var arguments = node.Arguments
				.Select (a => this.VisitAndPop (a))
				.ToList ();

			object expression;

			if (method == SqlMethods.LikeMethodInfo)
			{
				expression = new BinaryComparison
				(
					(Value) arguments[0],
					BinaryComparator.IsLike,
					(Value) arguments[1]
				);
			}
			else if (method == SqlMethods.EscapedLikeMethodInfo)
			{
				expression = new BinaryComparison
				(
					(Value) arguments[0],
 					BinaryComparator.IsLikeEscape,
					(Value) arguments[1]
				);
			}
			else if (method == SqlMethods.IsNullMethodInfo)
			{
				expression = new UnaryComparison
				(
					(EntityField) arguments[0],
					UnaryComparator.IsNull
				);
			}
			else if (method == SqlMethods.IsNotNullMethodInfo)
			{
				expression = new UnaryComparison
				(
					(EntityField) arguments[0],
					UnaryComparator.IsNotNull
				);
			}
			else if (method == SqlMethods.CompareToMethodInfo)
			{
				// This method is a special case used to compare strings. It is processed beforehand
				// in the VisitBinary method. So in theory this case should never happen and we
				// throw an exception just to make sure that if it does happen, we'll know it.

				throw new NotSupportedException ();
			}
			else
			{
				throw new NotSupportedException ();
			}

			return this.PushAndReturn (node, expression);
		}


		protected override Expression VisitMember(MemberExpression node)
		{
			// TODO Manage EntityCollectionFields.

			// Here we interpret the result of the expression on which the member access is done. We
			// can have the simple case where it is an entity or another member access on an entity
			// which returns an entity.
			// Of course there are lots of other cases that might be possible, but for now that's
			// the only ones that we allow.

			var expression = this.VisitAndPop (node.Expression);

			AbstractEntity entity;

			if (expression is AbstractEntity)
			{
				entity = (AbstractEntity) expression;
			}
			else if (expression is ReferenceField)
			{
				var referenceField = (ReferenceField) expression;
				var referenceFieldId = referenceField.FieldId.ToResourceId ();

				entity = referenceField.Entity.GetField<AbstractEntity> (referenceFieldId);
			}
			else
			{
				throw new NotImplementedException ();
			}

			// Here we build the result of the current expression transformation based on the kind
			// of member that we have.
			// For now we don't support the case of collection fields.

			var propertyInfo = (PropertyInfo) node.Member;
			var field = EntityInfo.GetStructuredTypeField (propertyInfo);
			var fieldId = field.CaptionId;

			EntityField entityField;

			switch (field.Relation)
			{
				case FieldRelation.None:
					entityField = new ValueField (entity, fieldId);
					break;

				case FieldRelation.Reference:
					entityField = new ReferenceField (entity, fieldId);
					break;

				case FieldRelation.Collection:
				default:
					throw new NotSupportedException ();
			}

			return this.PushAndReturn (node, entityField);
		}


		protected override Expression VisitParameter(ParameterExpression node)
		{
			// TODO Manage the cases where we want to return an InternalEntityField with an id.

			return this.PushAndReturn (node, this.entity);
		}


		protected override Expression VisitConstant(ConstantExpression node)
		{
			var constant = new Constant (node.Value);

			return this.PushAndReturn (node, constant);
		}


		protected override CatchBlock VisitCatchBlock(CatchBlock node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitBlock(BlockExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitConditional(ConditionalExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitDebugInfo(DebugInfoExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitDefault(DefaultExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitDynamic(DynamicExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override ElementInit VisitElementInit(ElementInit node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitExtension(Expression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitGoto(GotoExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitIndex(IndexExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitInvocation(InvocationExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitLabel(LabelExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override LabelTarget VisitLabelTarget(LabelTarget node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitLambda<T>(Expression<T> node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitListInit(ListInitExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitLoop(LoopExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
		{
			throw new NotSupportedException ();
		}


		protected override MemberBinding VisitMemberBinding(MemberBinding node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitMemberInit(MemberInitExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
		{
			throw new NotSupportedException ();
		}


		protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitNew(NewExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitNewArray(NewArrayExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitSwitch(SwitchExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override SwitchCase VisitSwitchCase(SwitchCase node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitTry(TryExpression node)
		{
			throw new NotSupportedException ();
		}


		protected override Expression VisitTypeBinary(TypeBinaryExpression node)
		{
			throw new NotSupportedException ();
		}


		private readonly AbstractEntity entity;


		private readonly Stack<object> results;


	}


}
