//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

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
	using Epsitec.Cresus.Core.Library;

	/// <summary>
	/// This module is used to retrieve data about the databases, such as the list of defined
	/// databases, the number of entities within a database, a subset of a database, to create or
	/// delete entities within a database, etc.
	/// </summary>
	public class DatabaseModule : AbstractAuthenticatedModule
	{
		public DatabaseModule(CoreServer coreServer)
			: base (coreServer, "/database")
		{
			// Gets the databases that should be displayed in the top menu of the application. They
			// are returned as a tree that represents the structure of this menu. The data returned
			// for each database is limited to its id, title and icon.
			Get["/list"] = p =>
				this.Execute (wa => this.GetDatabaseList (wa));

			// Gets the complete definition of a database, including all columns, sorters and other
			// parameters used to configure it.
			// URL arguments:
			// - name:    the DRUID of the dataset whose definition to return.
			Get["/definition/{name}"] = p =>
				this.Execute (wa => this.GetDatabase (wa, p));

			// Gets the data of the entities in a database.
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return, in the format used
			//            by the DataIO class.
			// GET arguments:
			// - start:   The index of the first entity to return, as an integer.
			// - limit:   The maximum number of entities to return, as an integer.
			// - columns: The id of the columns whose data to return, in the format used by the
			//            ColumnIO class.
			// - sort:    The sort clauses, in the format used by SorterIO class.
			// - filter:  The filters, in the format used by FilterIO class.
			Get["/get/{name}"] = p =>
				this.Execute (context => this.GetEntities (context, p));

			// Gets the index of an entity within a database.
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return, in the format used
			//            by the DataIO class..
			// - id:      The entity key of the entity whose index to return, in the format used by
			//            the EntityIO class.
			// GET arguments:
			// - sort:    The sort clauses, in the format used by SorterIO class.
			// - filter:  The filters, in the format used by FilterIO class.
			Get["/getindex/{name}/{id}"] = p =>
				this.Execute (context => this.GetEntityIndex (context, p));

			// Exports the entities of a database to a file.
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return.
			// GET arguments:
			// - sort:    The sort clauses, in the format used by SorterIO class.
			// - filter:  The filters, in the format used by FilterIO class.
			// - type:    The type of export to do.
			//            - array for entity daty as a csv file.
			//            - label for labels as a pdf file.
			//			  - bag for entity to user bah
			// If the type is array, then:
			// - columns: The id of the columns whose data to return, in the format used by the
			//            ColumnIO class.
			// If the type is label, then:
			// - layout:  The kind of layout desired. This is the value of the LabelLayout type, as
			//            used by the Enum.Parse(...) method.
			// - text:    The id of the LabelTextFactory used to generate the label text, as an
			//            integer value.
			Get["/export/{name}"] = (p =>
			{
				var type = "inconnu";
				CoreJob job	 = null;

				if(this.Request.Query.type == "label")
				{
					type = "Export fichier PDF";
				}
				if(this.Request.Query.type == "array")
				{
					type = "Export fichier CSV";
				}
				if (this.Request.Query.type == "bag")
				{
					type = "Export dans le panier";
					
				}

				this.Execute (b => this.CreateJob (b, type, true, out job));
				this.Enqueue (job, context => this.LongRunningExport (context, job, p));

				return new Response ()
				{
					StatusCode = HttpStatusCode.Accepted
				};
			});

			// Deletes some enties.
			// POST arguments:
			// - entityIds:  The id of the entities to delete, as used by the EntityIO class.
			Post["/delete"] = p =>
				this.Execute (b => this.DeleteEntities (b));

			// Ceates a new entity.
			// POST arguments:
			// - databaseName:    The DRUID of the dataset in which to create a new entity, in the
			//                    format used by the DataIO class.
			Post["/create/"] = p =>
				this.Execute (b => this.CreateEntity (b));
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

		internal static Response Export(Caches caches, EntityExtractor extractor, dynamic query)
		{
			var itemCount = extractor.Accessor.GetItemCount ();

			if (itemCount > 10000)
			{
				throw new System.InvalidOperationException ("Too many items in extractor: " + itemCount.ToString ());
			}

			EntityWriter writer = DatabaseModule.GetEntityWriter (caches, extractor, query);

			var filename = writer.GetFilename ();
			var stream   = writer.GetStream ();

			return CoreResponse.CreateStreamResponse (stream, filename);
		}

		internal static void ExportToDisk(string filename, Caches caches, EntityExtractor extractor, dynamic query)
		{
			var itemCount = extractor.Accessor.GetItemCount ();

			EntityWriter writer = DatabaseModule.GetEntityWriter (caches, extractor, query);

			var stream   = writer.GetStream ();
			var depotPath = CoreContext.GetFileDepotPath ("downloads");
			using (var fileStream = System.IO.File.Create (depotPath + "\\" + filename))
			{
				stream.CopyTo (fileStream);
			}
		}

		internal static void ExportToEntityUserBag(string user, Caches caches, EntityExtractor extractor, dynamic query)
		{
			foreach (var entity in extractor.Accessor.GetAllItems ())
			{
				EntityBag.Add (entity,"test");
			}
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

		private Response GetEntities(BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			string rawColumns = Request.Query.columns;
			int start = Request.Query.start;
			int limit = Request.Query.limit;

			using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
			{
				return DatabaseModule.GetEntities (caches, extractor, rawColumns, start, limit);
			}
		}

		private Response GetEntityIndex(BusinessContext businessContext, dynamic parameters)
		{
			string rawEntityKey = parameters.id;
			var entityKey = EntityIO.ParseEntityId (rawEntityKey);

			using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
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

		private Response Export(BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
			{
				return DatabaseModule.Export (caches, extractor, this.Request.Query);
			}
		}

		private Response NotifyUIForExportWaiting(WorkerApp workerApp, CoreJob task)
		{
			var entityBag = EntityBagManager.GetCurrentEntityBagManager ();
			entityBag.AddToBag (task.Username, task.Title, task.SummaryView, task.Id, When.Now);

			var statusBar = StatusBarManager.GetCurrentStatusBarManager ();
			statusBar.AddToBar ("text", task.StatusView, "", task.Id, When.Now);

			return new Response ()
			{
				StatusCode = HttpStatusCode.Accepted
			};
		}

		private void LongRunningExport(BusinessContext businessContext, CoreJob job, dynamic parameters)
		{
			var user		= LoginModule.GetUserName (this);

			if(this.Request.Query.type != "bag")
			{
				var fileExt		= this.Request.Query.type == "label" ? ".pdf" : ".csv";
				var filename    = DownloadsModule.GenerateFileNameForUser (user, fileExt);
				var finishMetaData = "<br><input type='button' onclick='Epsitec.Cresus.Core.app.downloadFile(\"" + filename + "\");' value='Télécharger' />";

				try
				{
					job.Start ();
					var caches		= this.CoreServer.Caches;

					using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
					{
						DatabaseModule.ExportToDisk (filename, caches, extractor, this.Request.Query);
					}
				}
				catch
				{
					finishMetaData = "Une erreur est survenue. tâche annulée";
				}
				finally
				{
					job.Finish (finishMetaData);
				}
			}
			else
			{
				var finishMetaData = "";
				try
				{
					job.Start ();
					var caches		= this.CoreServer.Caches;
					
					using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
					{
						var entityCount    = extractor.Accessor.GetItemCount ();
						if (entityCount < 1000)
						{
							finishMetaData = entityCount + "éléments ajouté au panier";
							DatabaseModule.ExportToEntityUserBag (user, caches, extractor, this.Request.Query);
						}
						else
						{
							finishMetaData = "Exportation impossible, plus de 1000 éléments";
						}
					}
				}
				catch
				{
					finishMetaData = "Une erreur est survenue. tâche annulée";
				}
				finally
				{
					job.Finish (finishMetaData);
				}	
			}

			
			
		}

		private void UpdateTaskStatusInBag(CoreJob task)
		{
			var user = LoginModule.GetUserName (this);
			var entityBag = EntityBagManager.GetCurrentEntityBagManager ();

			entityBag.RemoveFromBag (user, task.Id, When.Now);
			entityBag.AddToBag (user, task.Title, task.SummaryView, task.Id, When.Now);
		}

		private void UpdateTaskStatus(CoreJob task)
		{
			var user = LoginModule.GetUserName (this);
			var statusBar = StatusBarManager.GetCurrentStatusBarManager ();

			statusBar.RemoveFromBar (task.Id, When.Now);
			statusBar.AddToBar ("text", task.StatusView, "", task.Id, When.Now);
		}

		private void RemoveTaskStatus(CoreJob task)
		{
			var user = LoginModule.GetUserName (this);
			var statusBar = StatusBarManager.GetCurrentStatusBarManager ();
			statusBar.RemoveFromBar (task.Id, When.Now);
		}


		private EntityExtractor GetEntityExtractor(BusinessContext businessContext, dynamic parameters)
		{
			var workerApp = WorkerApp.Current;
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

		
		private static EntityWriter GetEntityWriter(Caches caches, EntityExtractor extractor, dynamic query)
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

		private static EntityWriter GetArrayWriter(Caches caches, EntityExtractor extractor, dynamic query)
		{
			string rawColumns = query.columns;

			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var properties = caches.PropertyAccessorCache;
			var format     = new CsvArrayFormat ();
			var columns    = ColumnIO.ParseColumns (caches, extractor.Database, rawColumns);

			return new ArrayWriter (properties, metaData, columns, accessor, format)
			{
				RemoveDuplicates = true
			};
		}

		private static EntityWriter GetLabelWriter(EntityExtractor extractor, dynamic query)
		{
			string rawLayout        = query.layout;
			int    rawTextFactoryId = query.text;

			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var layout      = (LabelLayout) Enum.Parse (typeof (LabelLayout), rawLayout);
			var entitytype  = metaData.EntityTableMetadata.EntityType;
			var textFactory = LabelTextFactoryResolver.Resolve (entitytype, rawTextFactoryId);

			return new LabelWriter (metaData, accessor, textFactory, layout)
			{
				RemoveDuplicates = true
			};
		}

		private static EntityWriter GetReportWriter(EntityExtractor extractor, dynamic query)
		{
			string rawLayout        = query.layout;
			int    rawTextFactoryId = query.text;

			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var layout      = (LabelLayout) Enum.Parse (typeof (LabelLayout), rawLayout);
			var entitytype  = metaData.EntityTableMetadata.EntityType;
			var textFactory = LabelTextFactoryResolver.Resolve (entitytype, rawTextFactoryId);

			return new LabelWriter (metaData, accessor, textFactory, layout)
			{
				RemoveDuplicates = true
			};
		}
	}
}
