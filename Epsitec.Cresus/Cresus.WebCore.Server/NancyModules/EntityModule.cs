using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Controllers;

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
	/// This module is used to update the data of existing entities.
	/// </summary>
	public class EntityModule : AbstractAuthenticatedModule
	{


		public EntityModule(CoreServer coreServer)
			: base (coreServer, "/entity")
		{
			// Edits the values of an entity.
			// URL arguments:
			// - id:    The entity key of the entity to edit, in the format used by the EntityIO
			//          class.
			// POST arguments:
			// The post arguments are dynamic. They are the fields of the entity that must be
			// edited. The name of the argument is the id of the field to edit, as used by the
			// PropertyAccessorCache, and the value is the value of the field, in the format used
			// by the FieldIO class.
			Post["/edit/{id}"] = p =>
				this.Execute (b => this.Edit (b, p));

			// Invokes an autoCreator on an entity.
			// Post arguments:
			// - entityId:        The entity key of the entity on which the autoCreator will be
			//                    invoked, in the format used by the EntityIO class.
			// - autoCreatorId:   The id of the autoCreator to invoke, as used by the
			//                    AutoCreatorCache class.
			Post["/autoCreate"] = _ =>
				this.Execute (b => this.AutoCreateNullEntity (b));

			// Executes an action on an entity.
			// URL arguments:
			// - viewMode:   The view mode of the EntityViewController to use, as used by the
			//               DataIO class.
			// - viewId:     The view id of the EntityViewController to use, as used by the DataIO
			//               class.
			// - entityId:   The entity key of the entity on which the EntityViewController will be
			//               used, in the format used by the EntityIO class.
			// POST arguments:
			// The post arguments are dynamic. The are the ids of the edition values of the form
			// that will be passed to the callback that executes the action. Their name is the id
			// of the edition fields and their values are the value of these edition fields.
			Post["/action/entity/{viewMode}/{viewId}/{entityId}"] = p =>
				this.Execute (b => this.ExecuteEntityAction (b, p));

			// Executes an action in job queue on an entity.
			// URL arguments:
			// - viewMode:   The view mode of the EntityViewController to use, as used by the
			//               DataIO class.
			// - viewId:     The view id of the EntityViewController to use, as used by the DataIO
			//               class.
			// - entityId:   The entity key of the entity on which the EntityViewController will be
			//               used, in the format used by the EntityIO class.
			// POST arguments:
			// The post arguments are dynamic. The are the ids of the edition values of the form
			// that will be passed to the callback that executes the action. Their name is the id
			// of the edition fields and their values are the value of these edition fields.
			Post["/actionqueue/entity/{viewMode}/{viewId}/{entityId}"] = (p =>
			{
				CoreJob job	 = null;
				this.Execute (b => this.CreateJob (b, "action", false, out job));
				this.Enqueue (job, b => this.ExecuteEntityActionInQueue (b,job, p));
				return CoreResponse.Success ();
			});

			// Executes an action on an entity, with an additional entity. This is used for
			// instance in actions on an entity list.
			// URL arguments:
			// - viewMode:             The view mode of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - viewId:               The view id of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - entityId:             The entity key of the entity on which the EntityViewController
			//                         will be used, in the format used by the EntityIO class.
			// - additionalEntityId:   The entity key of the additional entity on which the
			//                         EntityViewController will be used, in the format used by the
			//                         EntityIO class.
			// POST arguments:
			// The post arguments are dynamic. The are the ids of the edition values of the form
			// that will be passed to the callback that executes the action. Their name is the id
			// of the edition fields and their values are the value of these edition fields.
			Post["/action/entity/{viewMode}/{viewId}/{entityId}/{additionalEntityId}"] = p =>
				this.Execute (b => this.ExecuteEntityAction (b, p));

			// Executes an action on an entity, with an additional entity. This is used for
			// instance in actions on an entity list.
			// URL arguments:
			// - viewMode:             The view mode of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - viewId:               The view id of the EntityViewController to use, as used by
			//                         the DataIO class.
			// - entityId:             The entity key of the entity on which the EntityViewController
			//                         will be used, in the format used by the EntityIO class.
			// POST arguments:
			// - entityIds: The entity keys of the additional entities to batches, in the
			//              format used by the EntityIO class.
			Post["/action/entity/{viewMode}/{viewId}/{entityId}/list"] = p =>
				this.Execute (b => this.ExecuteActionForEntityList (b, p));

			// Executes an action on an entity type. This is used for the creation controllers for
			// instance.
			// URL arguments:
			// - viewMode:   The view mode of the EntityViewController to use, as used by the
			//               DataIO class.
			// - viewId:     The view id of the EntityViewController to use, as used by the DataIO
			//               class.
			// - typeId:     The id of the entity type with which to used the EntityViewController,
			//               in the format used by the TypeCache class.
			// POST arguments:
			// The post arguments are dynamic. The are the ids of the edition values of the form
			// that will be passed to the callback that executes the action. Their name is the id
			// of the edition fields and their values are the value of these edition fields.
			Post["/action/type/{viewMode}/{viewId}/{typeId}"] = p =>
				this.Execute (b => this.ExecuteTypeAction (b, p));
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

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
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

			using (Tools.Bind (businessContext, entity))
			{
				var child = autoCreator.Execute (businessContext, entity);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

				return this.CreateEntityIdResponse (businessContext, child);
			}
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

		private void ExecuteEntityActionInQueue(BusinessContext businessContext,CoreJob job, dynamic parameters)
		{
			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.entityId);
			var additionalEntity = EntityIO.ResolveEntity (businessContext, (string) parameters.additionalEntityId);

			this.ExecuteActionInQueue (businessContext,job, entity, additionalEntity, parameters);
		}

		private Response ExecuteActionForEntityList(BusinessContext businessContext, dynamic parameters)
		{
			var entity = EntityIO.ResolveEntity (businessContext, (string) parameters.entityId);

			return this.ExecuteActionForEntityList (businessContext, entity, parameters);
		}


		private Response ExecuteAction(BusinessContext businessContext, AbstractEntity entity, AbstractEntity additionalEntity, dynamic parameters)
		{
			var viewMode = DataIO.ParseViewMode ((string) parameters.viewMode);
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);

			using (var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId))
			{
				var actionProvider = controller as IActionExecutorProvider;
				var functionProvider = controller as IFunctionExecutorProvider;

				if (actionProvider != null)
				{
					return this.ExecuteAction (businessContext, actionProvider, entity, additionalEntity);
				}
				else if (functionProvider != null)
				{
					return this.ExecuteAction (businessContext, functionProvider, entity, additionalEntity);
				}
				else
				{
					return CoreResponse.Failure ();
				}
			}
		}

		private void ExecuteActionInQueue(BusinessContext businessContext,CoreJob job, AbstractEntity entity, AbstractEntity additionalEntity, dynamic parameters)
		{
			var viewMode = DataIO.ParseViewMode ((string) parameters.viewMode);
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);

			using (var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId))
			{
				var actionProvider = controller as IActionExecutorProvider;
				var functionProvider = controller as IFunctionExecutorProvider;

				if (actionProvider != null)
				{
					this.ExecuteActionInQueue (businessContext,job, actionProvider, entity, additionalEntity);
				}
				else
				{
					job.Finish ("erreur");
				}
			}
		}


		private Response ExecuteAction(BusinessContext businessContext, IActionExecutorProvider actionProvider, AbstractEntity entity, AbstractEntity additionalEntity)
		{
			var executor = actionProvider.GetExecutor ();

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

		private void ExecuteActionInQueue(BusinessContext businessContext,CoreJob job, IActionExecutorProvider actionProvider, AbstractEntity entity, AbstractEntity additionalEntity)
		{
			var executor = actionProvider.GetExecutor ();

			DynamicDictionary form = Request.Form;
			var arguments = this.GetArguments (executor, form, businessContext);

			using (businessContext.Bind (entity))
			using (businessContext.Bind (additionalEntity))
			{
				job.Start ("veuillez patienter...");
				executor.Call (arguments);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
				job.Finish ("");
			}
		}


		private Response ExecuteAction(BusinessContext businessContext, IFunctionExecutorProvider functionProvider, AbstractEntity entity, AbstractEntity additionalEntity)
		{
			var executor = functionProvider.GetExecutor ();

			DynamicDictionary form = Request.Form;
			var arguments = this.GetArguments (executor, form, businessContext);

			AbstractEntity newEntity;

			using (businessContext.Bind (entity))
			using (businessContext.Bind (additionalEntity))
			{
				newEntity = executor.Call (arguments);

				using (businessContext.Bind (newEntity))
				{
					businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
				}
			}

			return this.CreateEntityIdResponse (businessContext, newEntity);
		}

		private Response ExecuteActionForEntityList(BusinessContext businessContext, AbstractEntity entity, dynamic parameters)
		{
			var viewMode = DataIO.ParseViewMode ((string) parameters.viewMode);
			var viewId = DataIO.ParseViewId ((string) parameters.viewId);

			string rawEntityIds = this.Request.Form.entityIds;
			var entities = EntityIO.ResolveEntities (businessContext, rawEntityIds).ToList ();
			foreach (var additionalEntity in entities)
			{
				using (var controller = Mason.BuildController (businessContext, entity, additionalEntity, viewMode, viewId))
				{
					var actionProvider = controller as IActionExecutorProvider;
					var executor = actionProvider.GetExecutor ();
					DynamicDictionary form = new DynamicDictionary ();
					var arguments = this.GetArguments (executor, form, businessContext);

					using (businessContext.Bind (entity))
					using (businessContext.Bind (additionalEntity))
					{
						executor.Call (arguments);

						businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
					}
				}
			}
			return CoreResponse.Success ();
			
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
