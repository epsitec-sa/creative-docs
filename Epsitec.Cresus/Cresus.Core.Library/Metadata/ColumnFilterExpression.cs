//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public abstract class ColumnFilterExpression
	{
		public abstract bool IsValid
		{
			get;
		}

		public abstract Expression GetExpression(ParameterExpression parameter);

		
		public static Expression Comparison(ParameterExpression parameter, ColumnFilterComparisonCode code, Expression expression)
		{
			switch (code)
			{
				case ColumnFilterComparisonCode.Equal:
					return Expression.Equal (parameter, expression);

				case ColumnFilterComparisonCode.NotEqual:
					return Expression.NotEqual (parameter, expression);

				case ColumnFilterComparisonCode.GreaterThan:
					return Expression.GreaterThan (parameter, expression);

				case ColumnFilterComparisonCode.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual (parameter, expression);

				case ColumnFilterComparisonCode.LessThan:
					return Expression.LessThan (parameter, expression);

				case ColumnFilterComparisonCode.LessThanOrEqual:
					return Expression.LessThanOrEqual (parameter, expression);

				case ColumnFilterComparisonCode.Like:
					return Expression.Call (Epsitec.Cresus.Database.SqlMethods.LikeMethodInfo, parameter, expression);

				case ColumnFilterComparisonCode.NotLike:
					return Expression.Not (Expression.Call (Epsitec.Cresus.Database.SqlMethods.LikeMethodInfo, parameter, expression));

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", code.GetQualifiedName ()));
			}
		}

		public static Expression IsNull(ParameterExpression parameter)
		{
			return Expression.Call (Epsitec.Cresus.Database.SqlMethods.IsNullMethodInfo, parameter);
		}

		public static Expression IsNotNull(ParameterExpression parameter)
		{
			return Expression.Call (Epsitec.Cresus.Database.SqlMethods.IsNotNullMethodInfo, parameter);
		}
	}
}
