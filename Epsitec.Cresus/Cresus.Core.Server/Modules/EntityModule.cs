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
				var coreSession = this.GetCoreSession ();
				var context = coreSession.GetBusinessContext ();

				string paramEntityKey = (string) parameters.id;

				var entityKey = EntityKey.Parse (paramEntityKey);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var errors = new Dictionary<string, object> ();
				var memberNames = (IEnumerable<string>) Request.Form.GetDynamicMemberNames ();
				var memberKeys  = memberNames.Where (x => !x.Contains ('_')).ToArray ();

				foreach (var memberKey in memberKeys)
				{
					try
					{
						var value  = Request.Form[memberKey];
						var lambda = Request.Form[string.Concat ("lambda_", (string) memberKey)];

						if (lambda.HasValue)
						{
							var accessor = coreSession.GetPanelFieldAccessor (InvariantConverter.ToInt ((string) lambda.Value));

							if (accessor.IsCollectionType)
							{
								//	TODO: retrouver la liste des entités sélectionnées

								List<AbstractEntity> entities = new List<AbstractEntity> ();
								
								accessor.SetCollection (entity, entities);
							}
							else if (accessor.CanWrite == false)
							{
								errors.Add (memberKey, "The field cannot be written to.");
							}
							else if (accessor.IsEntityType)
							{
								EntityKey      valueEntityKey = EntityKey.Parse (value);
								AbstractEntity valueEntity    = context.DataContext.ResolveEntity (valueEntityKey);

								accessor.SetEntityValue (entity, valueEntity);
							}
							else if (value.HasValue)
							{
								accessor.SetStringValue (entity, value.Value);
							}
							else
							{
								accessor.SetStringValue (entity, null);
							}
						}
						else
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Error: /entity/{0} cannot resolve member {1}", paramEntityKey, memberKey));
						}
					}
					catch (System.Exception e)
					{
						errors.Add (memberKey, e.ToString ());
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
