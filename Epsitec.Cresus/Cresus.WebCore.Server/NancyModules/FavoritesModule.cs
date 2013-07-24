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


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{
	using Database = Core.Databases.Database;

	/// <summary>
	/// This module is used to retrieve data from the favorites cache, in a similar way as it is
	/// done in the DatabaseModule.
	/// </summary>
	public class FavoritesModule : AbstractAuthenticatedModule
	{
		public FavoritesModule(CoreServer coreServer)
			: base (coreServer, "/favorites")
		{
			// Gets the data of the entities in a favorite cache entry.
			// URL arguments:
			// - name:    The id of the favorite entry, as used by the FavoritesCache class.
			// GET arguments:
			// - start:   The index of the first entity to return, as an integer.
			// - limit:   The maximum number of entities to return, as an integer.
			// - columns: The id of the columns whose data to return, in the format used by the
			//            ColumnIO class.
			// - sort:    The sort clauses, in the format used by SorterIO class.
			// - filter:  The filters, in the format used by FilterIO class.
			Get["/get/{name}"] = p =>
				this.Execute ((wa, b) => this.GetEntities (wa, b, p));

			// Exports the entities of a favorite cache entry to a file.
			// URL arguments:
			// - name:    The id of the favorite entry, as used by the FavoritesCache class.
			// GET arguments:
			// - sort:    The sort clauses, in the format used by SorterIO class.
			// - filter:  The filters, in the format used by FilterIO class.
			// - type:    The type of export to do.
			//            - array for entity daty as a csv file.
			//            - label for labels as a pdf file.
			// If the type is array, then:
			// - columns: The id of the columns whose data to return, in the format used by the
			//            ColumnIO class.
			// If the type is label, then:
			// - layout:  The kind of layout desired. This is the value of the LabelLayout type, as
			//            used by the Enum.Parse(...) method.
			// - text:    The id of the LabelTextFactory used to generate the label text, as an
			//            integer value.
			Get["/export/{name}"] = p =>
				this.Execute ((wa, b) => this.Export (wa, b, p));
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
