using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Resolvers;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
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

			string rawColumns = Request.Query.columns;
			int start = Request.Query.start;
			int limit = Request.Query.limit;

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				return DatabaseModule.GetEntities (caches, extractor, rawColumns, start, limit);
			}
		}


		internal static Response GetEntities(Caches caches, EntityExtractor extractor, string rawColumns, int start, int limit)
		{
			var total = extractor.Accessor.GetItemCount ();
			var entities = extractor.Accessor.GetItems (start, limit);

			var dataContext = extractor.Accessor.IsolatedDataContext;
			var database = extractor.Database;

			var columns = ColumnIO.ParseColumns (caches, extractor.Database, rawColumns).ToList ();
			var menuItems = database.MenuItems
				.OfType<SummaryNavigationContextualMenuItem> ()
				.ToList ();

			var columnExpressions = columns.Select (c => c.LambdaExpression);
			var menuItemsExpressions = menuItems.Select (m => m.LambdaExpression);
			var expressions = columnExpressions.Concat (menuItemsExpressions);

			dataContext.LoadRelatedData (entities, expressions);

			var data = entities
				.Select (e => database.GetEntityData (columns, menuItems, dataContext, caches, e))
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

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				return DatabaseModule.Export (caches, extractor, this.Request.Query);
			}
		}


		internal static Response Export(Caches caches, EntityExtractor extractor, dynamic query)
		{
			if (extractor.Accessor.GetItemCount () > 10000)
			{
				throw new InvalidOperationException ("Too much data in extractor");
			}

			EntityWriter writer = DatabaseModule.GetEntityWriter (caches, extractor, query);

			var filename = writer.GetFilename ();
			var stream = writer.GetStream ();

			return CoreResponse.CreateStreamResponse (stream, filename);
		}


		private static EntityWriter GetEntityWriter
		(
			Caches caches,
			EntityExtractor extractor,
			dynamic query
		)
		{
			string type = query.type;

			switch (type)
			{
				case "array":
					return DatabaseModule.GetArrayWriter (caches, extractor, query);

				case "label":
					return DatabaseModule.GetLabelWriter (extractor, query);

				default:
					throw new NotImplementedException ();
			}
		}


		private static EntityWriter GetArrayWriter
		(
			Caches caches,
			EntityExtractor extractor,
			dynamic query
		)
		{
			string rawColumns = query.columns;

			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var properties = caches.PropertyAccessorCache;
			var format = new CsvArrayFormat ();
			var columns = ColumnIO.ParseColumns (caches, extractor.Database, rawColumns);

			return new ArrayWriter (properties, metaData, columns, accessor, format);
		}


		private static EntityWriter GetLabelWriter
		(
			EntityExtractor extractor,
			dynamic query
		)
		{
			string rawLayout = query.layout;
			int rawTextFactoryId = query.text;

			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var layout = (LabelLayout) Enum.Parse (typeof (LabelLayout), rawLayout);
			var entitytype = metaData.EntityTableMetadata.EntityType;
			var textFactory = LabelTextFactoryResolver.Resolve (entitytype, rawTextFactoryId);

			return new LabelWriter (metaData, accessor, textFactory, layout);
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
			var caches = this.CoreServer.Caches;

			var databaseCommandId = DataIO.ParseDruid ((string) Request.Form.databaseName);
			var database = this.CoreServer.DatabaseManager.GetDatabase (databaseCommandId);
			var columns = database.Columns;

			var dataContext = businessContext.DataContext;

			var entity = database.CreateEntity (businessContext);
			var entityData = database.GetEntityData (columns, dataContext, caches, entity);

			return CoreResponse.Success (entityData);
		}


	}


}
