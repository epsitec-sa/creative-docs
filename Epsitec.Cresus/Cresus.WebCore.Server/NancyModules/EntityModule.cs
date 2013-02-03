using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.ActionControllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;
using Epsitec.Cresus.WebCore.Server.Layout;

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
			Post["/action/entity/{viewMode}/{viewId}/{entityId}"] = p => this.Execute (b => this.ExecuteEntityAction (b, p));
			Post["/action/entity/{viewMode}/{viewId}/{entityId}/{additionalEntityId}"] = p => this.Execute (b => this.ExecuteEntityAction (b, p));
			Post["/action/type/{viewMode}/{viewId}/{typeId}"] = p => this.Execute (b => this.ExecuteTypeAction (b, p));
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

				var valueType = propertyAccessor.Type;
				var value = (DynamicDictionaryValue) form[propertyAccessorId];
				var fieldType = propertyAccessor.FieldType;
				
				var convertedValue = FieldIO.ConvertFromClient (businessContext, value, valueType, fieldType);

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

			return this.CreateEntityIdResponse (businessContext, child);
		}


		private Response CreateEntityIdResponse(BusinessContext businessContext, AbstractEntity entity)
		{
			var entityId = EntityIO.GetEntityId (businessContext, entity);

			var content = new Dictionary<string, object> ()
			{
				{ "entityId", entityId },
			};

			return CoreResponse.Success (content);
		}


		private Response ExecuteTypeAction(BusinessContext businessContext, dynamic parameters)
		{
			var type = this.CoreServer.Caches.TypeCache.GetItem ((string) parameters.typeId);
			var dummyEntity = (AbstractEntity) Activator.CreateInstance (type);

			return this.ExecuteAction (businessContext, dummyEntity, null, parameters);
		}


		private Response ExecuteEntityAction(BusinessContext businessContext, dynamic parameters)
		{
			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.entityId);
			var additionalEntity = EntityIO.ResolveEntity (businessContext, (string) parameters.additionalEntityId);

			return this.ExecuteAction (businessContext, entity, additionalEntity, parameters);
		}


		private Response ExecuteAction(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, dynamic parameters)
		{
			var viewMode = DataIO.ParseViewMode ((string) parameters.viewMode);
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);
			
			using (var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId))
			{
				try
				{
					var actionController = controller as IActionViewController;
					var creationController = controller as IBrickCreationViewController;

					if (actionController != null)
					{
						return this.ExecuteAction (businessContext, actionController, entity, additionalEntity);
					}
					else if (creationController != null)
					{
						return this.ExecuteAction (businessContext, creationController, entity, additionalEntity);
					}
					else
					{
						return CoreResponse.Failure ();
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
		}


		private Response ExecuteAction(BusinessContext businessContext, IActionViewController controller, AbstractEntity entity, AbstractEntity additionalEntity)
		{
			var executor = controller.GetExecutor ();

			DynamicDictionary form = Request.Form;
			var arguments = this.GetArguments (executor, form, businessContext);

			using (businessContext.Bind (entity))
			using (businessContext.Bind (additionalEntity))
			{
				executor.Call (arguments);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			return CoreResponse.Success ();
		}


		private Response ExecuteAction(BusinessContext businessContext, IBrickCreationViewController controller, AbstractEntity entity, AbstractEntity additionalEntity)
		{
			var executor = controller.GetExecutor ();

			DynamicDictionary form = Request.Form;
			var arguments = this.GetArguments (executor, form, businessContext);

			AbstractEntity newEntity;

			using (businessContext.Bind (entity))
			using (businessContext.Bind (additionalEntity))
			{
				newEntity = executor.Call (arguments);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			return this.CreateEntityIdResponse (businessContext, newEntity);
		}


		private IList<object> GetArguments(AbstractExecutor actionExecutor, DynamicDictionary form, BusinessContext businessContext)
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
		
				var convertedValue = FieldIO.ConvertFromClient (businessContext, value, argumentType);

				arguments[index] = convertedValue;
				argumentsCheck[index] = true;
			}

			if (argumentsCheck.Any (b => b == false))
			{
				throw new ArgumentException ("Missing arguments");
			}

			return arguments;
		}


	}


}
