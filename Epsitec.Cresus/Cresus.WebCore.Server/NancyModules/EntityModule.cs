using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Layout;

using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections;
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
			Post["/executeAction/{viewId}/{entityId}"] = p => this.Execute (b => this.ExecuteAction (b, p));
		}


		private Response Edit(BusinessContext businessContext, dynamic parameters)
		{
			DynamicDictionary form = Request.Form;
			var propertyAccessorsWithValues = EntityModule.GetPropertyAccessorsWithValues (businessContext, this.CoreServer.Caches, form)
				.ToList ();

			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.id);

			var invalidItems = from item in propertyAccessorsWithValues
							   let accessor = item.Item1
							   let value = item.Item2
							   let result = accessor.CheckValue (value)
							   where !result.IsValid
							   let id = accessor.Id
							   let message = result.ErrorMessage.IsNullOrEmpty ()
									? Res.Strings.IncorrectValue
									: result.ErrorMessage
							   select Tuple.Create (id, message);

			var errorItems = invalidItems.ToDictionary
			(
				i => i.Item1,
				i => (object) i.Item2.ToString ()
			);

			if (errorItems.Any ())
			{
				return CoreResponse.FormFailure (errorItems);
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
					businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
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


		private static IEnumerable<Tuple<AbstractPropertyAccessor, object>> GetPropertyAccessorsWithValues(BusinessContext businessContext, Caches caches, DynamicDictionary form)
		{
			foreach (var propertyAccessorId in form.GetDynamicMemberNames ())
			{
				var propertyAccessorCache = caches.PropertyAccessorCache;
				var propertyAccessor = propertyAccessorCache.Get (propertyAccessorId);

				var fieldType = propertyAccessor.FieldType;
				var valueType = propertyAccessor.Type;
				var value = (DynamicDictionaryValue) form[propertyAccessorId];

				var convertedValue = EntityModule.ConvertValue (businessContext, value, fieldType, valueType);

				yield return Tuple.Create (propertyAccessor, convertedValue);
			}
		}


		private Response AutoCreateNullEntity(BusinessContext businessContext)
		{
			// NOTE Should we add some locking here in order to ensure that we don't have two
			// clients that auto create an entity at the same time ?

			var autoCreatorCache = this.CoreServer.Caches.AutoCreatorCache;

			var entity = EntityIO.ResolveEntity (businessContext, (string) Request.Form.entityId);
			string autoCreatorId = Request.Form.autoCreatorId;
			var autoCreator = autoCreatorCache.Get (autoCreatorId);

			var child = autoCreator.Execute (businessContext, entity);

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

			var entityId = EntityIO.GetEntityId (businessContext, child);

			var content = new Dictionary<string, object> ()
			{
				{ "entityId", entityId },
			};

			return CoreResponse.Success (content);
		}

		private Response ExecuteAction(BusinessContext businessContext, dynamic parameters)
		{
			var viewMode = ViewControllerMode.Action;
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);
			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.entityId);

			using (var viewController = Mason.BuildController (businessContext, entity, viewMode, viewId))
			{
				var actionViewController = (IActionViewController) viewController;
				var actionExecutor = actionViewController.GetExecutor ();

				try
				{
					DynamicDictionary form = Request.Form;
					var arguments = this.GetArguments (actionExecutor, form, businessContext);

					using (businessContext.Bind(entity))
					{
						actionExecutor.Call (entity, arguments);

						businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
					}			
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

			return CoreResponse.Success ();
		}


		private IList<object> GetArguments(ActionExecutor actionExecutor, DynamicDictionary form, BusinessContext businessContext)
		{
			var argumentTypes = actionExecutor.GetArgumentTypes ().ToList ();
			var arguments = new List<object> ();
			var argumentsCheck = new List<bool> ();

			for (int i = 0; i < argumentTypes.Count; i++)
			{
				arguments.Add (null);
				argumentsCheck.Add (false);
			}

			foreach (var name in form.GetDynamicMemberNames ())
			{
				var value = (DynamicDictionaryValue) form[name];

				var index = int.Parse (name.Substring (2));
				var argumentType = argumentTypes[index];
				var fieldType = FieldTypeSelector.GetFieldType (argumentType);

				var convertedValue = EntityModule.ConvertValue (businessContext, value, fieldType, argumentType);

				arguments[index] = convertedValue;
				argumentsCheck[index] = true;
			}

			if (argumentsCheck.Any (b => b == false))
			{
				throw new ArgumentException ("Missing arguments");
			}

			return arguments;
		}


		private static object ConvertValue(BusinessContext businessContext, DynamicDictionaryValue value, FieldType fieldType, Type valueType)
		{
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
					return EntityModule.ConvertForEntityCollection (businessContext, value, valueType);

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
					clientValue = EntityModule.ConvertNancyValue (value, v => (string) v);
					break;

				case FieldType.Text:
					clientValue = EntityModule.ConvertNancyValue (value, v => (string) v) ?? "";
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


		private static object ConvertForEntityCollection(BusinessContext businessContext, DynamicDictionaryValue value, Type valueType)
		{
			var rawValue = (string) value.Value;
			var values = rawValue.Split (";")
				.Where (id => !string.IsNullOrEmpty (id))
				.Select (id => EntityIO.ResolveEntity (businessContext, id));

			var listType = typeof (List<>);
			var genericListType = listType.MakeGenericType (valueType.GetGenericArguments ()[0]);

			var list = (IList) Activator.CreateInstance (genericListType);
			list.AddRange (values);

			return list;
		}


		private static object ConvertForEntityReference(BusinessContext businessContext, DynamicDictionaryValue value)
		{
			var entityId = (string) value;

			AbstractEntity entity = null;

			if (!string.IsNullOrEmpty (entityId) && !Constants.KeyForNullValue.Equals (entityId))
			{
				entity = EntityIO.ResolveEntity (businessContext, entityId);
			}

			return entity;
		}


	}


}
