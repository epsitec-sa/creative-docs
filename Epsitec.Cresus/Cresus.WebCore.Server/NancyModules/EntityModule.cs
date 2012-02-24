using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface;
using Epsitec.Cresus.WebCore.Server.UserInterface.PanelFieldAccessor;

using Nancy;

using System;

using System.Collections.Generic;

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
			Post["/{id}"] = p => this.ExecuteWithCoreSession (cs => this.Update (cs, p));
		}


		private Response Update(CoreSession coreSession, dynamic parameters)
		{
			var businessContext = coreSession.GetBusinessContext ();

			var errors = new Dictionary<string, object> ();

			AbstractEntity entity = EntityModule.ResolveEntity (businessContext, parameters.id);
			DynamicDictionary formData = PostArrayHandler.GetFormWithArrays (Request.Form);

			// We filter the form to exclude the fields used to store the lambda keys.
			var memberNames = formData.GetDynamicMemberNames ().Where (x => !Tools.IsLambdaFieldName (x));

			using (businessContext.Bind (entity))
			{
				foreach (var memberName in memberNames)
				{
					try
					{
						var value = formData[memberName];
						var panelFieldAccessor = EntityModule.GetPanelFieldAccessor (coreSession, formData, memberName);

						EntityModule.SetValue (businessContext, panelFieldAccessor, entity, value);
					}
					catch (Exception e)
					{
						errors.Add (memberName, e.ToString ());
					}
				}

				businessContext.ApplyRulesToRegisteredEntities (RuleType.Update);

				if (errors.Any ())
				{
					// TODO This is probably a bad idea to discard the changes like this, as the
					// business context is never reset. Either This should be changed, or the
					// business context should be reset or something.

					businessContext.Discard ();
					return Response.AsCoreError (errors);
				}
				else
				{
					businessContext.SaveChanges ();
					return Response.AsCoreSuccess ();
				}
			}
		}


		private static AbstractEntity ResolveEntity(BusinessContext businessContext, string entityId)
		{
			var entityKey = EntityKey.Parse (entityId);

			return businessContext.DataContext.ResolveEntity (entityKey);
		}


		private static AbstractPanelFieldAccessor GetPanelFieldAccessor(CoreSession coreSession, DynamicDictionary formData, string memberName)
		{
			var lambdaFieldName = Tools.GetLambdaFieldName (memberName);
			var lambdaFieldValue = formData[lambdaFieldName];
			var accessorId = (string) lambdaFieldValue.Value;

			return coreSession.PanelFieldAccessorCache.Get (accessorId);
		}


		private static void SetValue(BusinessContext businessContext, AbstractPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, DynamicDictionaryValue value)
		{
			var entityListPanelFieldAccessor = panelFieldAccessor as EntityListPanelFieldAccessor;
			var entityPanelFieldAccessor = panelFieldAccessor as EntityPanelFieldAccessor;
			var stringPanelFieldAccessor = panelFieldAccessor as StringPanelFieldAccessor;

			if (entityListPanelFieldAccessor != null)
			{
				var castedValue = (IEnumerable<string>) value.Value;

				EntityModule.SetValueForCollection (businessContext, entityListPanelFieldAccessor, entity, castedValue);
			}
			else if (entityPanelFieldAccessor != null)
			{
				var castedValue = (string) value;

				EntityModule.SetValueForEntity (businessContext, entityPanelFieldAccessor, entity, castedValue);
			}
			else if (stringPanelFieldAccessor != null)
			{
				var castedValue = (string) value;

				EntityModule.SetValueForString (stringPanelFieldAccessor, entity, castedValue);
			}
		}


		private static void SetValueForCollection(BusinessContext businessContext, EntityListPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, IEnumerable<string> targetEntityIds)
		{
			var targetEntities = targetEntityIds
				.Where (id => !string.IsNullOrWhiteSpace (id))
				.Select (id => EntityModule.ResolveEntity (businessContext, id))
				.ToList ();

			panelFieldAccessor.SetCollection (entity, targetEntities);
		}


		private static void SetValueForEntity(BusinessContext businessContext, EntityPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string targetEntityId)
		{
			var targetEntity = string.IsNullOrEmpty (targetEntityId)
				? null
				: EntityModule.ResolveEntity (businessContext, targetEntityId);

			panelFieldAccessor.SetEntityValue (entity, targetEntity);
		}


		private static void SetValueForString(StringPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string fieldValue)
		{
			// NOTE Here we interpret empty strings as if they where null strings. The problem is
			// that we can't make the difference as a text field does not provide any way to tell if
			// its input is the null string or the empty one. So here I made the choice to always
			// interpret empty strings as null strings.
			// I do the conversion here explicitely because the underlying Marshaler embedded in
			// the PanelFieldAccessor does not make this conversion automatically and that's
			// probably a good thing.

			var value = string.IsNullOrEmpty (fieldValue)
			    ? null
			    : fieldValue;

			panelFieldAccessor.SetString (entity, value);
		}


	}


}
