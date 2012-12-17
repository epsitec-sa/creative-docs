using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	/// <summary>
	/// Used to retrieve data about the databases, such as the list of defined databases, the number
	/// of entities within a database, a subset of a database or to create or delete an entity
	/// within a database.
	/// </summary>
	public class DatabaseModule : AbstractAuthenticatedModule
	{


		public DatabaseModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			Get["/list"] = p => this.Execute (wa => this.GetDatabaseList (wa));
			Get["/definition/{name}"] = p => this.GetDatabase (p);
			Get["/get/{name}"] = p => this.Execute (wa => this.GetEntities (wa, p));
			Get["/getindex/{name}/{id}"] = p => this.Execute (wa => this.GetEntityIndex (wa, p));
			Post["/delete"] = p => this.Execute (b => this.DeleteEntities (b));
			Post["/create/"] = p => this.Execute (b => this.CreateEntity (b));
		}


		private Response GetDatabaseList(WorkerApp workerApp)
		{
			var databases = this.CoreServer.DatabaseManager
				.GetDatabases (workerApp.UserManager)
				.Select (d => d.GetDataDictionary ())
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "menu", databases },
			};

			return CoreResponse.Success (content);
		}


		private Response GetDatabase(dynamic parameters)
		{
			var databaseCommandId = DataIO.ParseDruid ((string) parameters.name);
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseCommandId);

			var content = database.GetDataDictionary (this.CoreServer.Caches);

			return CoreResponse.Success (content);
		}


		private Response GetEntities(WorkerApp workerApp, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter = db =>
			{
				return db.GetDataSetAccessor (workerApp.DataSetGetter);
			};

			string rawDatabaseId = parameters.name;
			var databaseId = DataIO.ParseDruid (rawDatabaseId);

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);
			
			int start = Request.Query.start;
			int limit = Request.Query.limit;
			
			return Tools.GetEntities
			(
				caches, userManager, databaseManager, dataSetAccessorGetter, databaseId, rawSorters,
				rawFilters, start, limit
			);
		}


		private Response GetEntityIndex(WorkerApp workerApp, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter = db =>
			{
				return db.GetDataSetAccessor (workerApp.DataSetGetter);
			};

			string rawDatabaseId = parameters.name;
			var databaseId = DataIO.ParseDruid (rawDatabaseId);

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);
			string rawEntityKey = parameters.id;

			return Tools.GetEntityIndex
			(
				caches, userManager, databaseManager, dataSetAccessorGetter, databaseId, rawSorters,
				rawFilters, rawEntityKey
			);
		}


		private Response DeleteEntities(BusinessContext businessContext)
		{
			var entityIds = (string) Request.Form.entityIds;
			var entities = EntityIO.ResolveEntities (businessContext, entityIds).ToList ();

			var success = true;

			using (businessContext.Bind (entities))
			{
				foreach (var entity in entities)
				{
					success = businessContext.DeleteEntity (entity);

					if (!success)
					{
						break;
					}
				}
			}

			if (success)
			{
				businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);
			}

			return success
				? CoreResponse.Success ()
				: CoreResponse.Failure ();
		}


		private Response CreateEntity(BusinessContext businessContext)
		{
			var databaseCommandId = DataIO.ParseDruid ((string) Request.Form.databaseName);
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseCommandId);

			var dataContext = businessContext.DataContext;

			var entity = database.CreateEntity (businessContext);
			var entityData = database.GetEntityData (dataContext, this.CoreServer.Caches, entity);

			return CoreResponse.Success (entityData);
		}


	}


}
