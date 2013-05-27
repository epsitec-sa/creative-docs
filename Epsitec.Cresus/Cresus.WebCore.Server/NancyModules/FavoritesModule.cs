//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Favorites;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;

using Nancy;

using System;
using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	using Database = Core.Databases.Database;

	/// <summary>
	/// Used to retrieve data from the favorites cache.
	/// </summary>
	public class FavoritesModule : AbstractAuthenticatedModule
	{
		public FavoritesModule(CoreServer coreServer)
			: base (coreServer, "/favorites")
		{
			Get["/get/{name}"] = p => this.Execute ((wa, b) => this.GetEntities (wa, b, p));
			Get["/export/{name}"] = p => this.Execute ((wa, b) => this.Export (wa, b, p));
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


		private Response Export(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			using (EntityExtractor extractor = this.GetEntityExtractor (workerApp, businessContext, parameters))
			{
				return DatabaseModule.Export (caches, extractor, this.Request.Query);
			}
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

			string rawFavoritesId = parameters.name;
			var favorites = FavoritesCache.Current.Find (rawFavoritesId);

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);
			
			var databaseId = favorites.DatabaseId;

			Action<DataContext, DataLayer.Loader.Request, AbstractEntity> customizer = (d, r, e) =>
			{
				r.Conditions.Add (favorites.CreateCondition (e));
			};

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId, rawSorters, rawFilters, customizer
			);
		}
	}
}
