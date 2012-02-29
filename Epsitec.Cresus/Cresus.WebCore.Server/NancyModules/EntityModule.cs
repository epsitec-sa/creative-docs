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

			var errors = new Dictionary<string, object> ();

			AbstractEntity entity = EntityModule.ResolveEntity (businessContext, parameters.id);

			// NOTE Here we need to process the form to transform the arrays which are in separated
			// fields to put them back in the same field.

			DynamicDictionary formData = FormCollectionEmbedder.DecodeFormWithCollections (Request.Form);

			using (businessContext.Bind (entity))
			{
				foreach (var propertyAccessorId in formData.GetDynamicMemberNames ())
				{
					try
					{
						var value = formData[propertyAccessorId];
						var propertyAccessor = coreSession.PropertyAccessorCache.Get (propertyAccessorId);

						EntityModule.SetValue (businessContext, propertyAccessor, entity, value);
					}
					catch (Exception e)
					{
						errors.Add (propertyAccessorId, e.ToString ());
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


		private static void SetValue(BusinessContext businessContext, AbstractPropertyAccessor propertyAccessor, AbstractEntity entity, DynamicDictionaryValue value)
		{
			switch (propertyAccessor.FieldType)
			{
				case FieldType.EntityCollection:
					{
						var castedValue = (IEnumerable<string>) value.Value;
						var castedPropertyAccessor = (EntityCollectionPropertyAccessor) propertyAccessor;
			
						EntityModule.SetValueForEntityCollectionField (businessContext, castedPropertyAccessor, entity, castedValue);
						
						break;
					}
				case FieldType.EntityReference:
					{
						var castedValue = (string) value;
						var castedPropertyAccessor = (EntityReferencePropertyAccessor) propertyAccessor;
			
						EntityModule.SetValueForEntityReferenceField (businessContext, castedPropertyAccessor, entity, castedValue);
						
						break;
					}
				case FieldType.Enumeration:
					{
						var castedValue = (string) value;
						var castedPropertyAccessor = (TextPropertyAccessor) propertyAccessor;

						EntityModule.SetValueForEnumerationField (castedPropertyAccessor, entity, castedValue);

						break;
					}
				case FieldType.Date:
					{
						var castedValue = (string) value;
						var castedPropertyAccessor = (TextPropertyAccessor) propertyAccessor;

						EntityModule.SetValueForDateField (castedPropertyAccessor, entity, castedValue);

						break;
					}
				case FieldType.Text:
					{
						var castedValue = (string) value;
						var castedPropertyAccessor = (TextPropertyAccessor) propertyAccessor;

						EntityModule.SetValueForTextField (castedPropertyAccessor, entity, castedValue);

						break;
					}
				default:
					throw new NotImplementedException ();
			}
		}


		private static void SetValueForEntityCollectionField(BusinessContext businessContext, EntityCollectionPropertyAccessor propertyAccessor, AbstractEntity entity, IEnumerable<string> targetEntityIds)
		{
			var targetEntities = targetEntityIds
				.Where (id => !string.IsNullOrEmpty (id))
				.Select (id => EntityModule.ResolveEntity (businessContext, id))
				.ToList ();

			propertyAccessor.SetCollection (entity, targetEntities);
		}


		private static void SetValueForEntityReferenceField(BusinessContext businessContext, EntityReferencePropertyAccessor propertyAccessor, AbstractEntity entity, string targetEntityId)
		{
			var targetEntity = EntityModule.IsStringNullOrEmpty (targetEntityId)
				? null
				: EntityModule.ResolveEntity (businessContext, targetEntityId);

			propertyAccessor.SetEntityValue (entity, targetEntity);
		}


		private static void SetValueForEnumerationField(TextPropertyAccessor propertyAccessor, AbstractEntity entity, string fieldValue)
		{
			var value = EntityModule.IsStringNullOrEmpty (fieldValue)
				? null
				: fieldValue;

			propertyAccessor.SetString (entity, value);
		}


		private static void SetValueForDateField(TextPropertyAccessor propertyAccessor, AbstractEntity entity, string fieldValue)
		{
			propertyAccessor.SetString (entity, fieldValue);
		}


		private static void SetValueForTextField(TextPropertyAccessor propertyAccessor, AbstractEntity entity, string fieldValue)
		{
			// NOTE Here we convert empty strings to null strings if the property allows for null
			// values and we convert null strings to empty strings if the property doesn't allow for
			// null values.

			// TODO Here there is a bug. If the field is not a nullable type (that is, a type
			// derived from System.Nullable, the assignation to null won't work. That's because how
			// stuff are done in the marshaler.

			var value = string.IsNullOrEmpty (fieldValue) && propertyAccessor.Property.IsNullable
			    ? null
			    : fieldValue ?? "";

			propertyAccessor.SetString (entity, value);
		}


		private static bool IsStringNullOrEmpty(string text)
		{
			return string.IsNullOrEmpty (text)
				|| text == EntityModule.StringForNullValue;
		}


		public static readonly string StringForNullValue = "null";


	}


}
