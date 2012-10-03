//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Metadata
{
	public static class RequestExtensions
	{
		public static void AddCondition(this Epsitec.Cresus.DataLayer.Loader.Request request, DataContext dataContext, AbstractEntity entity, IFilter filter)
		{
			if (filter == null)
			{
				return;
			}

			var name = "x";
			var @param = Expression.Parameter (entity.GetType (), name);
			var body   = filter.GetExpression (@param);

			request.AddCondition (dataContext, name, entity, body);
		}

		public static void AddCondition(this Epsitec.Cresus.DataLayer.Loader.Request request, DataContext dataContext, string entityName, AbstractEntity entity, Expression expression)
		{
			if (expression == null)
			{
				return;
			}

			request.Conditions.Add (LambdaConverter.Convert (dataContext, entityName, entity, expression));
		}
	}
}
