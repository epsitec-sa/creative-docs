﻿using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.CoreServer;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Epsitec.Cresus.WebCore.Server.UserInterface.PropertyAccessor;

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
			Post["/edit/{id}"] = p => this.ExecuteWithCoreSession (cs => this.Edit (cs, p));
			Post["/autoCreate"] = _ => this.ExecuteWithCoreSession (cs => this.AutoCreateNullEntity (cs));
		}


		private Response Edit(CoreSession coreSession, dynamic parameters)
		{
			var businessContext = coreSession.GetBusinessContext ();
			var propertyAccessorCache = coreSession.PropertyAccessorCache;

			DynamicDictionary form = Request.Form;
			var propertyAccessorsWithValues = EntityModule.GetPropertyAccessorsWithValues (businessContext, propertyAccessorCache, form)
				.ToList ();

			var entity = Tools.ResolveEntity (businessContext, (string) parameters.id);

			var invalidItems = propertyAccessorsWithValues
				.Where (i => !i.Item1.CheckValue (entity, i.Item2))
				.ToList ();

			if (invalidItems.Any ())
			{
				var errors = invalidItems.ToDictionary (i => i.Item1.Id, i => "Invalid value");

				return CoreResponse.AsError (errors);
			}
			else
			{
				using (businessContext.Bind (entity))
				{
					foreach (var propertyAccessorWithValue in propertyAccessorsWithValues)
					{
						var propertyAccessor = propertyAccessorWithValue.Item1;
						var value = propertyAccessorWithValue.Item2;

						propertyAccessor.SetValue (entity, value);
					}

					businessContext.SaveChanges ();
					return CoreResponse.AsSuccess ();
				}
			}
		}


		private Response AutoCreateNullEntity(CoreSession coreSession)
		{
			// NOTE Should we add some locking here in order to ensure that we don't have two
			// clients that auto create an entity at the same time ?

			var businessContext = coreSession.GetBusinessContext ();
			var autoCreatorCache = coreSession.AutoCreatorCache;

			var entity = Tools.ResolveEntity (businessContext, (string) Request.Form.entityId);
			var autoCreator = autoCreatorCache.Get ((string) Request.Form.autoCreatorId);

			var child = autoCreator.Execute (businessContext, entity);

			businessContext.SaveChanges ();

			var entityId = Tools.GetEntityId (businessContext, child);

			return CoreResponse.AsSuccess (entityId);
		}


		private static IEnumerable<Tuple<AbstractPropertyAccessor, object>> GetPropertyAccessorsWithValues(BusinessContext businessContext, PropertyAccessorCache propertyAccessorCache, DynamicDictionary form)
		{
			// NOTE Here we need to process the form to transform the arrays which are in separated
			// fields to put them back in the same field.

			var processedForm = FormCollectionEmbedder.DecodeFormWithCollections (form);

			foreach (var propertyAccessorId in processedForm.GetDynamicMemberNames ())
			{
				var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId);
				DynamicDictionaryValue value = processedForm[propertyAccessorId];

				var convertedValue = EntityModule.ConvertValue (businessContext, propertyAccessor, value);

				yield return Tuple.Create (propertyAccessor, convertedValue);
			}
		}


		private static object ConvertValue(BusinessContext businessContext, AbstractPropertyAccessor propertyAccessor, DynamicDictionaryValue value)
		{
			switch (propertyAccessor.FieldType)
			{
				case FieldType.CheckBox:
				case FieldType.Date:
				case FieldType.Text:
					return EntityModule.ConvertForBooleanDateAndText (value);

				case FieldType.EntityCollection:
					return EntityModule.ConvertForEntityCollection (businessContext, value);

				case FieldType.EntityReference:
					return EntityModule.ConvertForEntityReference (businessContext, value);

				case FieldType.Enumeration:
					return EntityModule.ConvertForEnumeration (value);

				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertForBooleanDateAndText(DynamicDictionaryValue value)
		{
			var castedValue = (string) value;

			if (string.IsNullOrEmpty (castedValue))
			{
				castedValue = null;
			}

			return castedValue;
		}


		private static object ConvertForEntityCollection(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var sequence = (IEnumerable<string>) value.Value;

			return sequence
				.Where (id => !string.IsNullOrEmpty (id))
				.Select (id => Tools.ResolveEntity (businessContext, id))
				.ToList ();
		}

		private static object ConvertForEntityReference(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			AbstractEntity entity = null;

			if (!EntityModule.IsStringNullOrEmpty (entityId))
			{
				entity = Tools.ResolveEntity (businessContext, entityId);
			}

			return entity;
		}


		private static object ConvertForEnumeration(DynamicDictionaryValue value)
		{
			var enumerationValue = (string) value;

			if (EntityModule.IsStringNullOrEmpty (enumerationValue))
			{
				enumerationValue = null;
			}

			return enumerationValue;
		}


		private static bool IsStringNullOrEmpty(string text)
		{
			return string.IsNullOrEmpty (text)
				|| text == Constants.KeyForNullValue;
		}


	}


}
