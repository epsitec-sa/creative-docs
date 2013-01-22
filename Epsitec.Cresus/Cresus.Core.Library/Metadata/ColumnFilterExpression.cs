//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

[assembly:XmlNodeClass (typeof (ColumnFilterExpression))]

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>ColumnFilterExpression</c> class is the base class used to represent filters
	/// applied to a column of a table.
	/// </summary>
	public abstract class ColumnFilterExpression : IFilter, IXmlNodeClass
	{
		#region IFilter Members

		public abstract bool IsValid
		{
			get;
		}

		public abstract Expression GetExpression(AbstractEntity example, Expression parameter);

		#endregion

		#region IXmlNodeClass Members

		public abstract XElement Save(string xmlNodeName);

		#endregion

		public static ColumnFilterExpression Restore(XElement xml)
		{
			return XmlNodeClassFactory.Restore<ColumnFilterExpression> (xml);
		}
		
		public static Expression Compare(Expression parameter, ColumnFilterComparisonCode code, Expression expression)
		{
			if (parameter.Type != expression.Type)
			{
				expression = ColumnFilterExpression.Convert (expression, parameter.Type);
			}

			if (ColumnFilterExpression.IsStringType (parameter.Type))
			{
				return ColumnFilterExpression.CompareStrings (parameter, code, expression);
			}

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
				
				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", code.GetQualifiedName ()));
			}
		}

		
		private static Expression CompareStrings(Expression parameter, ColumnFilterComparisonCode code, Expression expression)
		{
			if (parameter.Type != typeof (string))
			{
				parameter = ColumnFilterExpression.Convert (parameter, typeof (string));
				expression = ColumnFilterExpression.Convert (expression, typeof (string));
			}

			switch (code)
			{
				case ColumnFilterComparisonCode.Equal:
					return Expression.Equal (parameter, expression);

				case ColumnFilterComparisonCode.NotEqual:
					return Expression.NotEqual (parameter, expression);

				case ColumnFilterComparisonCode.GreaterThan:
					return Expression.GreaterThan (Expression.Call (SqlMethods.CompareToMethodInfo, parameter, expression), zero);

				case ColumnFilterComparisonCode.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual (Expression.Call (SqlMethods.CompareToMethodInfo, parameter, expression), zero);

				case ColumnFilterComparisonCode.LessThan:
					return Expression.LessThan (Expression.Call (SqlMethods.CompareToMethodInfo, parameter, expression), zero);

				case ColumnFilterComparisonCode.LessThanOrEqual:
					return Expression.LessThanOrEqual (Expression.Call (SqlMethods.CompareToMethodInfo, parameter, expression), zero);

				case ColumnFilterComparisonCode.Contains:
				case ColumnFilterComparisonCode.StartsWith:
					return Expression.Call (SqlMethods.LikeMethodInfo, parameter, expression);

				case ColumnFilterComparisonCode.ContainsEscaped:
				case ColumnFilterComparisonCode.StartsWithEscaped:
					return Expression.Call (SqlMethods.EscapedLikeMethodInfo, parameter, expression);

				case ColumnFilterComparisonCode.NotContains:
				case ColumnFilterComparisonCode.NotStartsWith:
					return Expression.Not (Expression.Call (SqlMethods.LikeMethodInfo, parameter, expression));

				case ColumnFilterComparisonCode.NotContainsEscaped:
				case ColumnFilterComparisonCode.NotStartsWithEscaped:
					return Expression.Not (Expression.Call (SqlMethods.EscapedLikeMethodInfo, parameter, expression));

				default:
					throw new System.NotSupportedException (string.Format ("{0} not supported", code.GetQualifiedName ()));
			}
		}

		private static bool IsStringType(System.Type type)
		{
			return type == typeof (string)
				|| type == typeof (FormattedText)
				|| type == typeof (FormattedText?);
		}

		private static Expression Convert(Expression expression, System.Type type)
		{
			var declaringType = typeof (SqlMethods);
			var methodName = SqlMethods.ConvertMethodInfo.Name;
			var genericArguments = new System.Type[] { expression.Type, type };

			return Expression.Call (declaringType, methodName, genericArguments, expression);
		}


		private static readonly Expression zero = Expression.Constant (0);
	}
}