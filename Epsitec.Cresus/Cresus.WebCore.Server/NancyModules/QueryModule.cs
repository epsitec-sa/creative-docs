//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Common.Support.Extensions;
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
	using Epsitec.Cresus.Core.Entities;
	using Nancy.Json;
	using Nancy.Responses;
	using Epsitec.Cresus.WebCore.Server.Results;

	/// <summary>
	/// This module is used to create query filters for databases
	/// </summary>
	public class QueryModule : AbstractAuthenticatedModule
	{
		public QueryModule(CoreServer coreServer)
			: base (coreServer, "/query")
		{
			
			// Provide user Queries
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return, in the format used
			//            by the DataIO class.
			// GET arguments:
			//
			Get["/{name}/load"] = p =>
				this.Execute (context => this.LoadQueries (context, p));

			// Create and persist a query filter for further use for current user
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return, in the format used
			//            by the DataIO class.
			// 
			// Post arguments:
			// - columns: The id of the columns used in query
			// - query:   The query, in the format used by FilterIO class.
			// - name:    The query name to persist
			Post["/{name}/save"] = p =>
				this.Execute (context => this.SaveQuery (context, p));

			// Create and persist a query filter for further use for current user
			// URL arguments:
			// - name:    The DRUID of the dataset whose definition to return, in the format used
			//            by the DataIO class.
			// 
			// Post arguments:
			// - name:    The query name to delete
			Post["/{name}/delete"] = p =>
				this.Execute (context => this.DeleteQuery (context, p));
		}

		private Response SaveQuery(BusinessContext businessContext, dynamic parameters)
		{	
			var workerApp	    = WorkerApp.Current;
			var caches			= this.CoreServer.Caches;		
			var databaseManager = this.CoreServer.DatabaseManager;
			var userManager		= workerApp.UserManager;
			
			string rawDatabaseId = parameters.name;
			string queryName    = Request.Query.name;

			if(queryName == null)
			{
				return new Response ()
				{
					StatusCode = HttpStatusCode.BadRequest
				};
			}

			var databaseId		= DataIO.ParseDruid (rawDatabaseId);
			var database		= databaseManager.GetDatabase (databaseId);
			var dataset         = database.DataSetMetadata;
			var settings        = userManager.ActiveSession.GetDataSetSettings (dataset);
			var queries         = settings.AvailableQueries;

			string rawQuery   = Tools.GetOptionalParameter (Request.Query.query);
			string rawColumns = Request.Query.columns;
			
			var query = FilterIO.ParseQuery (businessContext, caches, database, rawQuery);
			query.Name = queryName;
			query.Description = rawQuery;
			queries.RemoveAll (q => q.Name.ToSimpleText () == queryName);
			queries.Add (query);
			userManager.ActiveSession.SetDataSetSettings (dataset, settings);

			return new Response ()
			{
				StatusCode = HttpStatusCode.OK
			};
		}

		private Response DeleteQuery(BusinessContext businessContext, dynamic parameters)
		{
			var workerApp	    = WorkerApp.Current;
			var caches			= this.CoreServer.Caches;
			var databaseManager = this.CoreServer.DatabaseManager;
			var userManager		= workerApp.UserManager;

			string rawDatabaseId = parameters.name;
			string queryName    = Request.Query.name;

			if (queryName == null)
			{
				return new Response ()
				{
					StatusCode = HttpStatusCode.BadRequest
				};
			}

			var databaseId		= DataIO.ParseDruid (rawDatabaseId);
			var database		= databaseManager.GetDatabase (databaseId);
			var dataset         = database.DataSetMetadata;
			var settings        = userManager.ActiveSession.GetDataSetSettings (dataset);
			var queries         = settings.AvailableQueries;

			queries.RemoveAll (q => q.Name.ToSimpleText () == queryName);
			userManager.ActiveSession.SetDataSetSettings (dataset, settings);

			return new Response ()
			{
				StatusCode = HttpStatusCode.OK
			};
		}

		private Response LoadQueries(BusinessContext businessContext, dynamic parameters)
		{
			var workerApp	    = WorkerApp.Current;
			var caches			= this.CoreServer.Caches;
			var databaseManager = this.CoreServer.DatabaseManager;
			var userManager		= workerApp.UserManager;

			string rawDatabaseId = parameters.name;
			var databaseId		= DataIO.ParseDruid (rawDatabaseId);
			var database		= databaseManager.GetDatabase (databaseId);
			var dataset         = database.DataSetMetadata;
			var settings        = userManager.ActiveSession.GetDataSetSettings (dataset);
			var seq				= Enumerable.Range (1, settings.AvailableQueries.Count).Select(x => x);
			var queries         = settings.AvailableQueries.Select(q => q).ToArray();
			var content         = new Dictionary<string, object> ();
			
			QueryResult [] result = new QueryResult [settings.AvailableQueries.Count+1];
			
			//insert default
			result[0] = new QueryResult ()
			{
				Id = 0,
				Name = "Aucun",
				ReadOnly = true
			};

			foreach(var num in seq)
			{
				result[num] = new QueryResult () {
					Id = num,
					Name = queries[num-1].Name.ToSimpleText (),
					RawQuery = queries[num-1].Description.ToSimpleText (),
					ReadOnly = false
				};
			}

			return new JsonResponse (result, new DefaultJsonSerializer ())
			{
				StatusCode = HttpStatusCode.OK,
			};
		}
	}
}
