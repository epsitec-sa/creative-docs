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
    using Epsitec.Cresus.WebCore.Server.Processors;
    using System.Collections.Concurrent;
    using Cresus.Core.Resolvers;
    using Core.IO;
    using Common.Support;
    using System.Linq;
    using System.Collections.Generic;/// <summary>
                                     /// This module is used to retrieve data from the favorites cache, in a similar way as it is
                                     /// done in the DatabaseModule.
                                     /// </summary>
    public class FavoritesModule : AbstractAuthenticatedModule
	{
		public FavoritesModule(CoreServer coreServer)
			: base (coreServer, "/favorites")
		{

            var instances = InterfaceImplementationResolver<IReportingProcessor>.CreateInstances(coreServer);
            var processors = instances.Select(x => new KeyValuePair<string, IReportingProcessor>(x.Name, x));
            this.processors = new System.Collections.Concurrent.ConcurrentDictionary<string, IReportingProcessor>(processors);

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
				this.Execute (context => this.GetEntities (context, p));

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
				this.Execute (context => this.Export (context, p));
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


		private Response Export(BusinessContext businessContext, dynamic parameters)
		{
			var caches = this.CoreServer.Caches;

			using (EntityExtractor extractor = this.GetEntityExtractor (businessContext, parameters))
			{
                var writer = this.GetEntityWriter (businessContext, caches, extractor, this.Request.Query);
                return DatabaseModule.Export (businessContext, caches, extractor, writer, this.Request.Query);
			}
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

			string rawFavoritesId = parameters.name;
			var favorites = FavoritesCache.Current.Find (rawFavoritesId);

			string rawSorters = Tools.GetOptionalParameter (Request.Query.sort);
			string rawFilters = Tools.GetOptionalParameter (Request.Query.filter);
			string rawQuery = Tools.GetOptionalParameter (Request.Query.query);

			var databaseId = favorites.DatabaseId;
			
			Action<DataContext, DataLayer.Loader.Request, AbstractEntity> customizer = (d, r, e) =>
			{
				r.Conditions.Add (favorites.CreateCondition (e));
			};

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId, rawSorters, rawFilters, rawQuery, customizer
			);
		}

        private EntityWriter GetEntityWriter(BusinessContext context, Caches caches, EntityExtractor extractor, dynamic query)
        {
            string type = query.type;

            switch (type)
            {
                case "array":
                    return this.GetArrayWriter(caches, extractor, query);

                case "label":
                    return this.GetLabelWriter(extractor, query);

                case "report":
                    return this.GetReportWriter(context, extractor, query);

                default:
                    throw new NotImplementedException();
            }
        }

        private EntityWriter GetArrayWriter(Caches caches, EntityExtractor extractor, dynamic query)
        {
            string rawColumns = query.columns;

            var metaData = extractor.Metadata;
            var accessor = extractor.Accessor;

            var properties = caches.PropertyAccessorCache;
            var format = new CsvArrayFormat();
            var columns = ColumnIO.ParseColumns(caches, extractor.Database, rawColumns);

            return new ArrayWriter(properties, metaData, columns, accessor, format)
            {
                RemoveDuplicates = true
            };
        }

        private EntityWriter GetLabelWriter(EntityExtractor extractor, dynamic query)
        {
            string rawLayout = query.layout;
            int rawTextFactoryId = query.text;

            var metaData = extractor.Metadata;
            var accessor = extractor.Accessor;

            var layout = (LabelLayout)Enum.Parse(typeof(LabelLayout), rawLayout);
            var entitytype = metaData.EntityTableMetadata.EntityType;
            var textFactory = LabelTextFactoryResolver.Resolve(entitytype, rawTextFactoryId);

            return new LabelWriter(metaData, accessor, textFactory, layout)
            {
                RemoveDuplicates = true
            };
        }

        private EntityWriter GetReportWriter(BusinessContext context, EntityExtractor extractor, dynamic query)
        {
            var metaData = extractor.Metadata;
            var accessor = extractor.Accessor;
            var processorName = (string)query.text;

            IReportingProcessor processor;
            if (this.processors.TryGetValue(processorName, out processor))
            {
                return new ReportWriter(metaData, accessor, context, query, processor);
            }

            return null;
        }

        private readonly ConcurrentDictionary<string, IReportingProcessor> processors;
    }
}
