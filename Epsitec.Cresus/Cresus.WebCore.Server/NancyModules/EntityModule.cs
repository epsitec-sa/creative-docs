﻿using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
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

			// NOTE Here we need to process the form to transform the arrays which are in separated
			// fields to put them back in the same field.

			DynamicDictionary formData = FormCollectionEmbedder.DecodeFormWithCollections (Request.Form);

			using (businessContext.Bind (entity))
			{
				foreach (var panelFieldAccessorId in formData.GetDynamicMemberNames ())
				{
					try
					{
						var value = formData[panelFieldAccessorId];
						var panelFieldAccessor = coreSession.PanelFieldAccessorCache.Get (panelFieldAccessorId);

						EntityModule.SetValue (businessContext, panelFieldAccessor, entity, value);
					}
					catch (Exception e)
					{
						errors.Add (panelFieldAccessorId, e.ToString ());
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


		private static void SetValue(BusinessContext businessContext, AbstractPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, DynamicDictionaryValue value)
		{
			switch (panelFieldAccessor.FieldType)
			{
				case FieldType.EntityCollection:
					{
						var castedValue = (IEnumerable<string>) value.Value;
						var castedPanelFieldAccessor = (EntityListPanelFieldAccessor) panelFieldAccessor;
			
						EntityModule.SetValueForEntityCollectionField (businessContext, castedPanelFieldAccessor, entity, castedValue);
						
						break;
					}
				case FieldType.EntityReference:
					{
						var castedValue = (string) value;
						var castedPanelFieldAccessor = (EntityPanelFieldAccessor) panelFieldAccessor;
			
						EntityModule.SetValueForEntityReferenceField (businessContext, castedPanelFieldAccessor, entity, castedValue);
						
						break;
					}
				case FieldType.Enumeration:
					{
						var castedValue = (string) value;
						var castedPanelFieldAccessor = (StringPanelFieldAccessor) panelFieldAccessor;

						EntityModule.SetValueForEnumerationField (castedPanelFieldAccessor, entity, castedValue);

						break;
					}
				case FieldType.Date:
					{
						var castedValue = (string) value;
						var castedPanelFieldAccessor = (StringPanelFieldAccessor) panelFieldAccessor;

						EntityModule.SetValueForDateField (castedPanelFieldAccessor, entity, castedValue);

						break;
					}
				case FieldType.Text:
					{
						var castedValue = (string) value;
						var castedPanelFieldAccessor = (StringPanelFieldAccessor) panelFieldAccessor;

						EntityModule.SetValueForTextField (castedPanelFieldAccessor, entity, castedValue);

						break;
					}
				default:
					throw new NotImplementedException ();
			}
		}


		private static void SetValueForEntityCollectionField(BusinessContext businessContext, EntityListPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, IEnumerable<string> targetEntityIds)
		{
			var targetEntities = targetEntityIds
				.Where (id => !string.IsNullOrWhiteSpace (id))
				.Select (id => EntityModule.ResolveEntity (businessContext, id))
				.ToList ();

			panelFieldAccessor.SetCollection (entity, targetEntities);
		}


		private static void SetValueForEntityReferenceField(BusinessContext businessContext, EntityPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string targetEntityId)
		{
			var targetEntity = string.IsNullOrEmpty (targetEntityId)
				? null
				: EntityModule.ResolveEntity (businessContext, targetEntityId);

			panelFieldAccessor.SetEntityValue (entity, targetEntity);
		}


		private static void SetValueForEnumerationField(StringPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string fieldValue)
		{
			EntityModule.SetValueForTextField (panelFieldAccessor, entity, fieldValue);
		}


		private static void SetValueForDateField(StringPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string fieldValue)
		{
			EntityModule.SetValueForTextField (panelFieldAccessor, entity, fieldValue);
		}


		private static void SetValueForTextField(StringPanelFieldAccessor panelFieldAccessor, AbstractEntity entity, string fieldValue)
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
