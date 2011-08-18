using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Epsitec.Cresus.DataLayer.Context;
using Nancy;
using Epsitec.Cresus.Core.Server.NancyComponents;

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

				var formData = PostArrayHandler.GetFormWithArrays (Request.Form);

				var entityKey = EntityKey.Parse (paramEntityKey);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var errors = new Dictionary<string, object> ();
				var memberNames = (IEnumerable<string>) formData.GetDynamicMemberNames ();
				var memberKeys  = memberNames.Where (x => !x.Contains ('_')).ToArray ();

				foreach (var memberKey in memberKeys)
				{
					try
					{
						var value  = formData[memberKey];
						var lambda = formData[PanelBuilder.GetLambdaFieldName ((string) memberKey)];

						if (lambda.HasValue)
						{
							var accessor = coreSession.GetPanelFieldAccessor (InvariantConverter.ToInt ((string) lambda.Value));

							if (accessor.IsCollectionType)
							{
								List<AbstractEntity> entities = new List<AbstractEntity> ();

								var collectionItems = value.Value;
								var collectionNames = (IEnumerable<string>) collectionItems.GetDynamicMemberNames ();
								var notNullNames = collectionNames.Where (x => !string.IsNullOrEmpty (x) && !string.IsNullOrEmpty (collectionItems[x]));

								foreach (string item in notNullNames)
								{
									EntityKey tmpKey = EntityKey.Parse (collectionItems[item]);
									AbstractEntity tmpEntity = context.DataContext.ResolveEntity (tmpKey);
									entities.Add (tmpEntity);
								}

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
