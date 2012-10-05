//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public class LambdaFilter : IFilter
	{
		public LambdaFilter(LambdaExpression lambda)
		{
			this.lambda = lambda;
		}

		#region IFilter Members

		public bool IsValid
		{
			get
			{
				return true;
			}
		}

		public Expression GetExpression(Expression parameter)
		{
			return ExpressionAnalyzer.ReplaceParameter (lambda, parameter as ParameterExpression);
		}

		#endregion


		public static LambdaFilter Create<TEntity>(Expression<System.Func<TEntity, bool>> lambda)
			where TEntity : AbstractEntity
		{
			return new LambdaFilter<TEntity> (lambda);
		}


		private readonly LambdaExpression		lambda;
	}
}
