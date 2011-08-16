using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
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

				string paramEntityKey = (string) parameters.id;
//				string paramLambdaId  = (string) parameters.lambda;

//				var accessor = coreSession.GetPanelFieldAccessor (InvariantConverter.ToInt (paramLambdaId));

				var entityKey = EntityKey.Parse (paramEntityKey);
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

				if (errors.Any ())
				{
					context.Discard (); // TODO check this
					return Response.AsCoreError (errors);
				}
				else
				{
					context.SaveChanges ();
					return Response.AsCoreSuccess ();
				}
			};
		}
	}
}
