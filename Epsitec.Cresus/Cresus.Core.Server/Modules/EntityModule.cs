using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Epsitec.Cresus.DataLayer.Context;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	public class EntityModule : CoreModule
	{
		public EntityModule()
			: base ("/entity")
		{
			Post["/{id}"] = parameters =>
			{
				var coreSession = GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				var entityKey = EntityKey.Parse (parameters.id);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var errors = new Dictionary<string, object> ();

				foreach (var key in Request.Form.GetDynamicMemberNames ())
				{
					var k = string.Format ("[{0}]", key);
					var v = Request.Form[key].ToString ().Trim ();
					try
					{
						entity.SetField (k, v);
					}
					catch (System.Exception e)
					{
						errors.Add (key, e.ToString ());
					}
				}
				context.SaveChanges ();

				if (errors.Any ())
				{
					return Response.AsErrorExtJsForm (errors);
				}
				else
				{
					return Response.AsSuccessExtJsForm ();
				}
			};
		}
	}
}
