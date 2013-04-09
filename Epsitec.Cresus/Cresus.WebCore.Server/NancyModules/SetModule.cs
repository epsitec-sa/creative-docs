using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.SetControllers;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.Layout;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	using Database = Core.Databases.Database;


	public class SetModule : AbstractAuthenticatedModule
	{


		public SetModule(CoreServer coreServer)
			: base (coreServer, "/set")
		{
			Get["/{viewId}/{entityId}/get/{dataset}"] = p => this.Execute ((wa, b) => this.GetEntities (wa, b, p));
			Post["/{viewId}/{entityId}/add"] = p => this.Execute (b => this.Add (b, p));
			Post["/{viewId}/{entityId}/remove"] = p => this.Execute (b => this.Remove (b, p));
		}


		private Response GetEntities(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			using (ISetViewController controller = this.GetController (businessContext, parameters))
			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, controller, parameters))
			{
				return DatabaseModule.GetEntities (caches, extractor, start, limit);
			}
		}


		private EntityExtractor GetEntityExtractor(WorkerApp workerApp, BusinessContext businessContext, ISetViewController controller, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);

			string dataSetName = parameters.dataset;

			var dataSetGetter = workerApp.DataSetGetter;
			var dataStore = workerApp.DataStoreMetaData;

			Druid databaseId;
			Func<Database, DataSetAccessor> dataSetAccessorGetter;

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

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId, rawSorters, rawFilters
			);
		}


		private ISetViewController GetController(BusinessContext businessContext, dynamic parameters)
		{
			string rawEntityId = parameters.entityId;
			string rawViewId = parameters.viewId;
			
			var entity = EntityIO.ResolveEntity (businessContext, rawEntityId);
			var viewId = DataIO.ParseViewId (rawViewId);
			var viewMode = ViewControllerMode.Set;

			return Mason.BuildController<ISetViewController>
			(
				businessContext, entity, null, viewMode, viewId
			);
		}


		private Response Add(BusinessContext businessContext, dynamic parameters)
		{
			Action<ISetViewController, IEnumerable<AbstractEntity>> action = (c, e) => c.AddItems (e);

			return this.ExecuteAction (businessContext, action, parameters);
		}


		private Response Remove(BusinessContext businessContext, dynamic parameters)
		{
			Action<ISetViewController, IEnumerable<AbstractEntity>> action = (c, e) => c.RemoveItems (e);

			return this.ExecuteAction (businessContext, action, parameters);
		}


		private Response ExecuteAction(BusinessContext businessContext, Action<ISetViewController, IEnumerable<AbstractEntity>> action, dynamic parameters)
		{
			string rawEntityIds = this.Request.Form.entityIds;
			var entities = EntityIO.ResolveEntities (businessContext, rawEntityIds).ToList ();

			using (ISetViewController controller = this.GetController (businessContext, parameters))
			{
				using (businessContext.Bind (controller.GetEntity ()))
				using (businessContext.Bind (entities))
				{
					action (controller, entities);

					businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

					return CoreResponse.Success ();
				}
			}
		}


	}


}
