//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Favorites;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	/// <summary>
	/// Used to retrieve data from the favorites cache.
	/// </summary>
	public class FavoritesModule : AbstractAuthenticatedModule
	{
		public FavoritesModule(CoreServer coreServer)
			: base (coreServer, "/favorites")
		{
			Get["/get/{name}"] = p => this.Execute (wa => wa.Execute (b => this.GetEntities (wa, b, p)));
		}


		private Response GetEntities(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;
			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;

			System.Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter = db => db.GetDataSetAccessor (workerApp.DataSetGetter);

			string rawFavoritesId = parameters.name;

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);
			
			int start = Request.Query.start;
			int limit = Request.Query.limit;

			var favorites = FavoritesCache.Current.Find (rawFavoritesId);
			
			return Tools.GetEntities
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				favorites, rawSorters, rawFilters, start, limit
			);
		}
	}
}
