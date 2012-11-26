using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to update value of an existing entity
	/// </summary>
	public class EntityModule : AbstractAuthenticatedModule
	{


		public EntityModule(CoreServer coreServer)
			: base (coreServer, "/entity")
		{
			Post["/edit/{id}"] = p => this.Execute (b => this.Edit (b, p));
			Post["/autoCreate"] = _ => this.Execute (b => this.AutoCreateNullEntity (b));
		}


		private Response Edit(BusinessContext businessContext, dynamic parameters)
		{
			DynamicDictionary form = Request.Form;
			var propertyAccessorsWithValues = EntityModule.GetPropertyAccessorsWithValues (businessContext, this.CoreServer.Caches, form)
				.ToList ();

			var entity = Tools.ResolveEntity (businessContext, (string) parameters.id);

			var invalidItems = propertyAccessorsWithValues
				.Where (i => !i.Item1.CheckValue (i.Item2))
				.ToList ();

			if (invalidItems.Any ())
			{
				var errors = invalidItems.ToDictionary (
					i => i.Item1.Id,
					i => (object) Res.Strings.IncorrectValue.ToSimpleText ()
				);

				return CoreResponse.FormFailure (errors);
			}

			using (businessContext.Bind (entity))
			{
				foreach (var propertyAccessorWithValue in propertyAccessorsWithValues)
				{
					var propertyAccessor = propertyAccessorWithValue.Item1;
					var value = propertyAccessorWithValue.Item2;

					propertyAccessor.SetValue (entity, value);
				}

				try
				{
					businessContext.SaveChanges (LockingPolicy.KeepLock);
				}
				catch (BusinessRuleException e)
				{
					var errors = new Dictionary<string, object> ()
					{
						{ "business", e.Message } 
					};

					return CoreResponse.FormFailure (errors);
				}
			}

			return CoreResponse.FormSuccess ();
		}


		private Response AutoCreateNullEntity(BusinessContext businessContext)
		{
			// NOTE Should we add some locking here in order to ensure that we don't have two
			// clients that auto create an entity at the same time ?

			var autoCreatorCache = this.CoreServer.Caches.AutoCreatorCache;

			var entity = Tools.ResolveEntity (businessContext, (string) Request.Form.entityId);
			string autoCreatorId = Request.Form.autoCreatorId;
			var autoCreator = autoCreatorCache.Get (autoCreatorId);

			var child = autoCreator.Execute (businessContext, entity);

			businessContext.SaveChanges (LockingPolicy.KeepLock);

			var entityId = Tools.GetEntityId (businessContext, child);

			var content = new Dictionary<string, object> ()
			{
				{ "entityId", entityId },
			};

			return CoreResponse.Success (content);
		}


		private static IEnumerable<Tuple<AbstractPropertyAccessor, object>> GetPropertyAccessorsWithValues(BusinessContext businessContext, Caches caches, DynamicDictionary form)
		{
			foreach (var propertyAccessorId in form.GetDynamicMemberNames ())
			{
				var propertyAccessorCache = caches.PropertyAccessorCache;
				var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId);
				var value = (DynamicDictionaryValue) form[propertyAccessorId];

				var convertedValue = EntityModule.ConvertValue (businessContext, propertyAccessor, value);

				yield return Tuple.Create (propertyAccessor, convertedValue);
			}
		}


		private static object ConvertValue(BusinessContext businessContext, AbstractPropertyAccessor propertyAccessor, DynamicDictionaryValue value)
		{
			var fieldType = propertyAccessor.FieldType;
			var valueType = propertyAccessor.Type;
			
			switch (fieldType)
			{
				case FieldType.Boolean:
				case FieldType.Date:
				case FieldType.Decimal:
				case FieldType.Enumeration:
				case FieldType.Integer:
				case FieldType.Text:
					return EntityModule.ConvertForValue (value, fieldType, valueType);

				case FieldType.EntityCollection:
					return EntityModule.ConvertForEntityCollection (businessContext, value);

				case FieldType.EntityReference:
					return EntityModule.ConvertForEntityReference (businessContext, value);


				default:
					throw new NotImplementedException ();
			}
		}


		private static object ConvertForValue(DynamicDictionaryValue value, FieldType fieldType, Type valueType)
		{
			object clientValue;

			switch (fieldType)
			{
				case FieldType.Boolean:
					clientValue = EntityModule.ConvertNancyValue (value, v => (bool) v);
					break;

				case FieldType.Date:
				case FieldType.Enumeration:
				case FieldType.Text:
					clientValue = EntityModule.ConvertNancyValue (value, v => (string) v);
					break;

				case FieldType.Decimal:
					clientValue = EntityModule.ConvertNancyValue (value, v => (decimal) v);
					break;

				case FieldType.Integer:
					clientValue = EntityModule.ConvertNancyValue (value, v => (long) v);
					break;

				case FieldType.EntityCollection:
				case FieldType.EntityReference:
				default:
					throw new NotImplementedException ();
			}

			var fieldValue = ValueConverter.ConvertClientToField (clientValue, fieldType, valueType);

			return ValueConverter.ConvertFieldToEntity (fieldValue, fieldType, valueType);
		}


		private static object ConvertNancyValue(DynamicDictionaryValue value, Func<DynamicDictionaryValue, object> converter)
		{
			if (!value.HasValue || string.IsNullOrEmpty ((string) value))
			{
				return null;
			}

			return converter (value);
		}


		private static object ConvertForEntityCollection(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var rawValue = (string) value.Value;
			var sequence = rawValue.Split (";");

			return sequence
				.Where (id => !string.IsNullOrEmpty (id))
				.Select (id => Tools.ResolveEntity (businessContext, id))
				.ToList ();
		}


		private static object ConvertForEntityReference(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			AbstractEntity entity = null;

			if (!string.IsNullOrEmpty (entityId) && !Constants.KeyForNullValue.Equals (entityId))
			{
				entity = Tools.ResolveEntity (businessContext, entityId);
			}

			return entity;
		}


	}


}
