//	Copyright © 2012-2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

            try
            {
                var name = "x";
                var @param = Expression.Parameter (entity.GetType (), name);
                var body = filter.GetExpression (entity, @param);

                request.AddCondition (dataContext, name, entity, body);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine ("Request.AddCondition: " + ex.Message);
            }
		}

		public static void AddCondition(this Epsitec.Cresus.DataLayer.Loader.Request request, DataContext dataContext, string entityName, AbstractEntity entity, Expression expression)
		{
			if (expression == null)
			{
				return;
			}

            try
            {
                request.Conditions.Add (LambdaConverter.Convert (dataContext, entityName, entity, expression));
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine ("Request.AddCondition: " + ex.Message);
            }
        }
    }
}
