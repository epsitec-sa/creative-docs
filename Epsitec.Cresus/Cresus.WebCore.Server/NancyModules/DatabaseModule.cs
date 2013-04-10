using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{


	using Database = Core.Databases.Database;


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
			Get["/definition/{name}"] = p => this.Execute (wa => this.GetDatabase (wa, p));
			Get["/get/{name}"] = p => this.Execute ((wa, b) => this.GetEntities (wa, b, p));
			Get["/getindex/{name}/{id}"] = p => this.Execute ((wa, b) => this.GetEntityIndex (wa, b, p));
			Get["/export/{name}"] = p => this.Execute ((wa, b) => this.Export (wa, b, p));
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


		private Response GetDatabase(WorkerApp workerApp, dynamic parameters)
		{
			var userManager = workerApp.UserManager;
			var commandId = DataIO.ParseDruid ((string) parameters.name);

			var database = this.CoreServer.DatabaseManager.GetDatabase (userManager, commandId);		
			var content = database.GetDataDictionary (this.CoreServer.Caches);

			return CoreResponse.Success (content);
		}


		private Response GetEntities(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			int start = Request.Query.start;
			int limit = Request.Query.limit;

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				return DatabaseModule.GetEntities (caches, extractor, start, limit);
			}
		}


		internal static Response GetEntities(Caches caches, EntityExtractor extractor, int start, int limit)
		{
			var total = extractor.Accessor.GetItemCount ();
			var entities = extractor.Accessor.GetItems (start, limit);

			var dataContext = extractor.Accessor.IsolatedDataContext;
			var database = extractor.Database;

			database.LoadRelatedData (dataContext, entities);

			var data = entities
				.Select (e => database.GetEntityData (dataContext, caches, e))
				.ToList ();

			var content = new Dictionary<string, object> ()
			{
				{ "total", total },
				{ "entities", data },
			};

			return CoreResponse.Success (content);
		}


		private Response GetEntityIndex(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			string rawEntityKey = parameters.id;
			var entityKey = EntityIO.ParseEntityId (rawEntityKey);

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				int? index = extractor.Accessor.IndexOf (entityKey);

				if (index == -1)
				{
					index = null;
				}

				var content = new Dictionary<string, object> ()
				{
					{ "index", index },
				};

				return CoreResponse.Success (content);
			}
		}


		private Response Export(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			string rawColumns = Tools.GetOptionalParameter (this.Request.Query.columns);

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				return DatabaseModule.Export (caches, extractor, rawColumns);
			}
		}


		internal static Response Export(Caches caches, EntityExtractor extractor, string rawColumns)
		{
			var properties = caches.PropertyAccessorCache;
			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;
			var format = new CsvArrayFormat ();

			var columns = ColumnIO.ParseColumns (caches, extractor.Database, rawColumns);

			EntityWriter writer = new ArrayWriter (properties, metaData, columns, accessor, format);

			var filename = writer.Filename;
			var stream = writer.GetStream ();

			return CoreResponse.CreateStreamResponse (stream, filename);
		}


		private EntityExtractor GetEntityExtractor(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			Func<Database, DataSetAccessor> dataSetAccessorGetter = db =>
			{
				return db.GetDataSetAccessor (workerApp.DataSetGetter);
			};

			string rawDatabaseId = parameters.name;
			var databaseId = DataIO.ParseDruid (rawDatabaseId);

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId, rawSorters, rawFilters
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
