using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

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
			Post["/{id}"] = p => this.ExecuteWithCoreSession (cs => this.Update (cs, p));
		}


		private Response Update(CoreSession coreSession, dynamic parameters)
		{
			var businessContext = coreSession.GetBusinessContext ();
			var propertyAccessorCache = coreSession.PropertyAccessorCache;

			DynamicDictionary form = Request.Form;
			var propertyAccessorsWithValues = EntityModule.GetPropertyAccessorsWithValues (businessContext, propertyAccessorCache, form)
				.ToList ();

			var entity = EntityModule.ResolveEntity (businessContext, (string) parameters.id);

			var invalidItems = propertyAccessorsWithValues
				.Where (i => !i.Item1.CheckValue (entity, i.Item2))
				.ToList ();

			if (invalidItems.Any ())
			{
				var errors = invalidItems.ToDictionary (i => i.Item1.Id, i => "Invalid value");

				return Response.AsCoreError (errors);
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
					return Response.AsCoreSuccess ();
				}
			}
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
					return EntityModule.ConvertForBooleanAndDate (value);

				case FieldType.EntityCollection:
					return EntityModule.ConvertForEntityCollection (businessContext, value);

				case FieldType.EntityReference:
					return EntityModule.ConvertForEntityReference (businessContext, value);

				case FieldType.Enumeration:
					return EntityModule.ConvertForEnumeration (value);

				case FieldType.Text:
					return EntityModule.ConvertForText (propertyAccessor, value);

				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertForBooleanAndDate(DynamicDictionaryValue value)
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
				.Select (id => EntityModule.ResolveEntity (businessContext, id))
				.ToList ();
		}

		private static object ConvertForEntityReference(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			AbstractEntity entity = null;

			if (!EntityModule.IsStringNullOrEmpty (entityId))
			{
				entity = EntityModule.ResolveEntity (businessContext, entityId);
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


		private static object ConvertForText(AbstractPropertyAccessor propertyAccessor, DynamicDictionaryValue value)
		{
			// NOTE Here we convert empty strings to null strings if the property allows for null
			// values and we convert null strings to empty strings if the property doesn't allow for
			// null values.

			var textValue = (string) value;

			if (string.IsNullOrEmpty (textValue))
			{
				if (propertyAccessor.Property.IsNullable)
				{
					textValue = null;
				}
				else
				{
					textValue = "";
				}
			}

			return textValue;
		}


		private static AbstractEntity ResolveEntity(BusinessContext businessContext, string entityId)
		{
			var entityKey = EntityKey.Parse (entityId);

			return businessContext.DataContext.ResolveEntity (entityKey);
		}


		private static bool IsStringNullOrEmpty(string text)
		{
			return string.IsNullOrEmpty (text)
				|| text == EntityModule.StringForNullValue;
		}


		public static readonly string StringForNullValue = "null";


	}


}
