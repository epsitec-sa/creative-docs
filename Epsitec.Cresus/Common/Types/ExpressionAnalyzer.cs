//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>ExpressionAnalyzer</c> takes a lambda expression and returns runtime information
	/// about it (for instance the property info if the lambda is like <c>x =&gt; x.Foo</c>).
	/// </summary>
	public static class ExpressionAnalyzer
	{
		/// <summary>
		/// Gets the property info for the specified expression, which must be a simply lambda
		/// accessor (such as <c>x =&gt; x.Foo</c>).
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The matching property info; otherwise, <c>null</c>.</returns>
		public static PropertyInfo GetLambdaPropertyInfo(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Lambda)
			{
				return ExpressionAnalyzer.GetLambdaPropertyInfo (expression as LambdaExpression);
			}
			else
			{
				return null;
			}
		}

#if DOTNET35
/* Nothing */
#else

		/// <summary>
		/// Creates a setter expression based on a getter expression.
		/// </summary>
		/// <param name="getterExpression">The getter expression.</param>
		/// <returns>The corresponding setter expression.</returns>
		public static LambdaExpression CreateSetter(LambdaExpression getterExpression)
		{
			if (getterExpression.Body.NodeType == ExpressionType.Parameter)
			{
				return null;
			}
			
			var lambdaMember = (MemberExpression) getterExpression.Body;
			var propertyInfo = lambdaMember.Member as System.Reflection.PropertyInfo;

			var fieldType    = getterExpression.ReturnType;
			var sourceType   = getterExpression.Parameters[0].Type;

			var sourceParameterExpression = getterExpression.Parameters[0];
			var valueParameterExpression  = Expression.Parameter (fieldType, "value");

			var expressionBlock =
				Expression.Block (
					Expression.Assign (
						Expression.Property (lambdaMember.Expression, lambdaMember.Member.Name),
						valueParameterExpression));

			var setterLambda = Expression.Lambda (expressionBlock, sourceParameterExpression, valueParameterExpression);

			return setterLambda;
		}
#endif

		private static PropertyInfo GetLambdaPropertyInfo(LambdaExpression expression)
		{
			var body = expression.Body;

			if (body.NodeType == ExpressionType.MemberAccess)
			{
				MemberExpression member = body as MemberExpression;
				PropertyInfo propertyInfo = member.Member as PropertyInfo;
					
				if ((propertyInfo != null) &&
					(propertyInfo.CanRead))
				{
					return propertyInfo;
				}
			}

			return null;
		}
	}
}
