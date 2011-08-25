//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core.Server.AdditionalResponses;
using Epsitec.Cresus.Core.Server.NancyComponents;
using Epsitec.Cresus.DataLayer.Context;
using Nancy;

namespace Epsitec.Cresus.Core.Server.Modules
{
	/// <summary>
	/// Used to update value of an existing entity
	/// </summary>
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
				var memberKeys  = memberNames.Where (x => !x.Contains ('_')).ToArray (); // We don't want the "lambda_" keys

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
								var collection = ((List<string>) value.Value).Where (x => !string.IsNullOrWhiteSpace (x));
								
								foreach (string item in collection)
								{
									EntityKey? tmpKey = EntityKey.Parse (item);
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
					context.Discard ();
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
