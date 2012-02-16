using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Globalization;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to update value of an existing entity
	/// </summary>
	public class EntityModule : AbstractCoreSessionModule
	{


		public EntityModule(ServerContext serverContext)
			: base (serverContext, "/entity")
		{
			Post["/{id}"] = parameters => this.ExecuteWithCoreSession (coreSession =>
			{
				var context = coreSession.GetBusinessContext ();

				string paramEntityKey = (string) parameters.id;

				var formData = PostArrayHandler.GetFormWithArrays (Request.Form);

				var entityKey = EntityKey.Parse (paramEntityKey);
				AbstractEntity entity = context.DataContext.ResolveEntity (entityKey);

				var errors = new Dictionary<string, object> ();
				var memberNames = (IEnumerable<string>) formData.GetDynamicMemberNames ();
				var memberKeys  = memberNames.Where (x => !x.Contains ('_')).ToArray (); // We don't want the "lambda_" keys

				foreach (var memberKey in memberKeys)
				{
					try
					{
						var value = formData[memberKey];
						var lambda = formData[Tools.GetLambdaFieldName (memberKey)];

						if (lambda.HasValue)
						{
							var accessor = coreSession.PanelFieldAccessorCache.Get (InvariantConverter.ToInt ((string) lambda.Value));

							if (accessor.IsCollectionType)
							{
								List<AbstractEntity> entities = new List<AbstractEntity> ();
								var collection = ((List<string>) value.Value).Where (x => !string.IsNullOrWhiteSpace (x));

								foreach (string item in collection)
								{
									EntityKey? tmpKey = EntityKey.Parse (item);
									AbstractEntity tmpEntity = context.DataContext.ResolveEntity (tmpKey);
									entities.Add (tmpEntity);
								}

								accessor.SetCollection (entity, entities);
							}
							else if (accessor.IsReadOnly)
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
							Debug.WriteLine (string.Format (CultureInfo.InvariantCulture, "Error: /entity/{0} cannot resolve member {1}", paramEntityKey, memberKey));
						}
					}
					catch (Exception e)
					{
						errors.Add (memberKey, e.ToString ());
					}
				}

				if (errors.Any ())
				{
					context.Discard ();
					return Response.AsCoreError (errors);
				}
				else
				{
					context.SaveChanges ();
					return Response.AsCoreSuccess ();
				}
			});
		}


	}


}
