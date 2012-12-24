using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Layout;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	public class SetModule : AbstractAuthenticatedModule
	{


		public SetModule(CoreServer coreServer)
			: base (coreServer, "/set")
		{
			Get["/{viewId}/{entityId}/get/{dataset}"] = p => this.Execute (wa => wa.Execute (b => this.GetEntities (wa, b, p)));
			Post["/{viewId}/{entityId}/add"] = p => this.Execute (b => this.Add (b, p));
			Post["/{viewId}/{entityId}/remove"] = p => this.Execute (b => this.Remove (b, p));
		}


		private Response GetEntities(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			string rawEntityId = parameters.entityId;
			string rawViewId = parameters.viewId;
			string dataSetName = parameters.dataset;

			var entity = EntityIO.ResolveEntity (businessContext, rawEntityId);
			var viewId = DataIO.ParseViewId (rawViewId);
			var viewMode = ViewControllerMode.Set;

			using (var controller = Mason.BuildController<ISetViewController> (businessContext, entity, viewMode, viewId))
			{
				var dataSetGetter = workerApp.DataSetGetter;
				var dataStore = workerApp.DataStoreMetaData;

				Druid databaseId;
				Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter;

				if (dataSetName == "display")
				{
					databaseId = controller.GetDisplayDataSetId ();
					dataSetAccessorGetter = _ => controller.GetDisplayDataSetAccessor (dataSetGetter, dataStore);
				}
				else if (dataSetName == "pick")
				{
					databaseId = controller.GetPickDataSetId ();
					dataSetAccessorGetter = _ => controller.GetPickDataSetAccessor (dataSetGetter, dataStore);
				}
				else
				{
					throw new ArgumentException ("Invalid data set name.");
				}

				return Tools.GetEntities
				(
					businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
					databaseId, rawSorters, rawFilters, start, limit
				);
			}
		}


		private Response Add(BusinessContext businessContext, dynamic parameters)
		{
			string rawEntityId = parameters.entityId;
			string rawViewId = parameters.viewId;
			string rawEntityIds = this.Request.Form.entityIds;
			
			return this.ExecuteAction (businessContext, rawEntityId, rawViewId, rawEntityIds, (c, e) => c.AddItems (e));
		}


		private Response Remove(BusinessContext businessContext, dynamic parameters)
		{
			string rawEntityId = parameters.entityId;
			string rawViewId = parameters.viewId;
			string rawEntityIds = this.Request.Form.entityIds;

			return this.ExecuteAction (businessContext, rawEntityId, rawViewId, rawEntityIds, (c, e) => c.RemoveItems (e));
		}


		private Response ExecuteAction(BusinessContext businessContext, string rawEntityId, string rawViewId, string rawEntityIds, Action<ISetViewController, IEnumerable<AbstractEntity>> action)
		{
			var entity = EntityIO.ResolveEntity (businessContext, rawEntityId);
			var viewId = DataIO.ParseViewId (rawViewId);
			var viewMode = ViewControllerMode.Set;

			var entities = EntityIO.ResolveEntities (businessContext, rawEntityIds).ToList ();

			using (businessContext.Bind (entity))
			using (businessContext.Bind (entities))
			using (var controller = Mason.BuildController<ISetViewController> (businessContext, entity, viewMode, viewId))
			{
				action (controller, entities);

				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

				return CoreResponse.Success ();
			}
		}


	}


}
