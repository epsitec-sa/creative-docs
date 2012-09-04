//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	/// <summary>
	/// The <c>ColumnFilterExpression</c> class is the base class used to represent filters
	/// applied to a column of a table.
	/// </summary>
	public abstract class ColumnFilterExpression : IFilter
	{
		#region IFilter Members

		public abstract bool IsValid
		{
			get;
		}

		public abstract Expression GetExpression(Expression parameter);

		#endregion

		public XElement Save(string xmlNodeName)
		{
			return new XElement (xmlNodeName);
		}

		public static ColumnFilterExpression Restore(XElement xml)
		{
			if (xml == null)
			{
				return null;
			}

			throw new System.NotImplementedException ();
		}


		public static Expression Comparison(Expression parameter, ColumnFilterComparisonCode code, Expression expression)
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

		public static Expression IsNull(Expression parameter)
		{
			return Expression.Call (Epsitec.Cresus.Database.SqlMethods.IsNullMethodInfo, parameter);
		}

		public static Expression IsNotNull(Expression parameter)
		{
			return Expression.Call (Epsitec.Cresus.Database.SqlMethods.IsNotNullMethodInfo, parameter);
		}
	}
}