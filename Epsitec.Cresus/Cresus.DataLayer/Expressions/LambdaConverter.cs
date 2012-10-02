using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;

using System;

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;

using System.Reflection;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public class LambdaConverter : ExpressionVisitor
	{


		public static DataExpression Convert<T>(DataContext dataContext, T entity, Expression<Func<T, bool>> lambda)
			where T : AbstractEntity
		{
			var e = (AbstractEntity) entity;
			var l = (LambdaExpression) lambda;

			return LambdaConverter.Convert (dataContext, e, l);
		}


		public static DataExpression Convert(DataContext dataContext, AbstractEntity entity, LambdaExpression lambda)
		{
			LambdaConverter.Check (dataContext, entity, lambda);

			var computedExpression = LambdaComputer.Compute (lambda.Body, LambdaConverter.IsExpressionComputable);
			var dataExpression = new LambdaConverter (dataContext, entity).Convert (computedExpression);

			return dataExpression;
		}


		private static bool IsExpressionComputable(Expression expression)
		{
			// Expressions that are the parameter of the lambda expression cannot be computed.
			if (expression.NodeType == ExpressionType.Parameter)
			{
				return false;
			}

			// We don't want to compute the calls to methods declared in SqlMethods because these
			// are "fake" method calls that are to be translated into DataExpressions.
			if (expression is MethodCallExpression)
			{
				var method = ((MethodCallExpression) expression).Method;

				if (method.DeclaringType == typeof (SqlMethods))
				{
					return false;
				}
			}

			return true;
		}


		private static void Check(DataContext dataContext, AbstractEntity entity, LambdaExpression lambda)
		{
			dataContext.ThrowIfNull ("dataContext");
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


		public LambdaConverter(DataContext dataContext, AbstractEntity entity)
		{
			this.dataContext = dataContext;
			this.entity = entity;
			this.results = new Stack<object> ();
		}


		public DataExpression Convert(Expression expression)
		{
			this.Visit (expression);

			return this.GetResult ();
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

			object result;

			if (this.IsUnaryOperator (nodeType))
			{
				result = new UnaryOperation
				(
					this.GetUnaryOperator (nodeType),
					(DataExpression) operand
				);
			}
			else if (nodeType == ExpressionType.Convert || nodeType == ExpressionType.ConvertChecked)
			{
				// In this case, we have a type conversion. So we skip the conversion and return the
				// value to be converted. This behavior works for simple case. However, it might not
				// work for more complex cases. If they happen, this behavior should be changed.

				result = operand;
			}
			else
			{
				throw new NotSupportedException ();
			}
			
			return this.PushAndReturn (node, result);
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

			return this.ConvertBinaryComparison (leftOperand, nodeType, rightOperand);
		}


		private DataExpression ConvertRegularComparison(BinaryExpression node)
		{
			var left = this.VisitAndPop (node.Left);
			var nodeType = node.NodeType;
			var right = this.VisitAndPop (node.Right);

			if (this.IsBinaryComparator (nodeType))
			{
				return this.ConvertBinaryComparison (left, nodeType, right);
			}
			else if (this.IsBinaryOperator (nodeType))
			{
				return this.ConvertBinaryOperation (left, nodeType, right);
			}
			
			throw new NotSupportedException ();
		}


		private DataExpression ConvertBinaryComparison(object left, ExpressionType nodeType, object right)
		{
			// Here we swap both sides if the left one is null, so it makes the null special case
			// below a lot easier.
			var swap = this.IsNullConstant (left);

			if (swap)
			{
				var tmp = left;
				left = right;
				right = tmp;
			}

			// Because of how the NULL stuff totally sucks in SQL, we must have two special cases
			// here to convert stuff like x == null, null == x, x != null and null != x to IS
			// NULL OR IS NOT NULL SQL clauses.
			var isNullConstant = this.IsNullConstant (right);

			if (isNullConstant && nodeType == ExpressionType.Equal)
			{
				return new UnaryComparison
				(
					(EntityField) left,
					UnaryComparator.IsNull
				);
			}
			else if (isNullConstant && nodeType == ExpressionType.NotEqual)
			{
				return new UnaryComparison
				(
					(EntityField) left,
					UnaryComparator.IsNotNull
				);
			}
			else
			{
				return new BinaryComparison
				(
					(Value) left,
					this.GetBinaryComparator (nodeType),
					(Value) right
				);
			}
		}


		private DataExpression ConvertBinaryOperation(object left, ExpressionType nodeType, object right)
		{
			return new BinaryOperation
			(
				(DataExpression) left,
				this.GetBinaryOperator (nodeType),
				(DataExpression) right
			);
		}


		private bool IsNullConstant(object o)
		{
			var constant = o as Constant;

			return constant != null && constant.Value == null;
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

			if (expression is InternalField)
			{
				var internalField = (InternalField) expression;

				entity = internalField.Entity;
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
			return this.PushAndReturn (node, InternalField.CreateId (this.entity));
		}


		protected override Expression VisitConstant(ConstantExpression node)
		{
			object value = node.Value;

			// If we have an entity at this point, we must get its row key and use that as a
			// constant, since we have a lambda like x => x == myEntity and we want to check for
			// equality based on the value of the entity key of myEntity.

			if (value is AbstractEntity)
			{
				var entityKey = this.dataContext.GetNormalizedEntityKey ((AbstractEntity) value);

				if (!entityKey.HasValue)
				{
					throw new NotSupportedException ("Entity used as constant has no entity key.");
				}

				value = entityKey.Value.RowKey;
			}

			return this.PushAndReturn (node, new Constant (value));
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


		private readonly DataContext dataContext;


		private readonly AbstractEntity entity;


		private readonly Stack<object> results;


	}


}
