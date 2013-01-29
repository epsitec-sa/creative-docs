using System;

using System.Linq.Expressions;

namespace Epsitec.Common.Support
{
	public static class LambdaUtils
	{
		public static LambdaExpression Convert<T1, T2>(Expression<Func<T1, T2>> function)
		{
			return (LambdaExpression) function;
		}
	}
}
