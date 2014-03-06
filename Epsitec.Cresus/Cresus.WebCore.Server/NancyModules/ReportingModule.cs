using System;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Extraction;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.NancyHosting;
using Nancy;


namespace Epsitec.Cresus.WebCore.Server.NancyModules
{

	using System.Collections.Generic;
	using Epsitec.Common.Support.EntityEngine;
	using Epsitec.Common.Types;
	using Epsitec.Cresus.Core.Metadata;
	using Epsitec.Cresus.DataLayer.Context;
	using Epsitec.Cresus.DataLayer.Expressions;
	using Epsitec.Cresus.WebCore.Server.Core.Databases;
	using Database = Core.Databases.Database;
	/// <summary>
	/// This module is used to retrieve some configuration data for the production of pdf files.
	/// </summary>
	public class ReportingModule : AbstractAuthenticatedModule
	{
		public ReportingModule(CoreServer coreServer)
			: base (coreServer, "/reporting")
		{
			///TEST REPORT
			//https://localhost/proxy/reporting/[LVOR13]/[LVOA13]-1000000001/test
			Get["/{datasetId}/{entityId}/test"] = p =>
				this.Execute ((wa, b) => this.ProduceTestReport (wa, b, p));
		}

		private Response ProduceTestReport(WorkerApp workerApp, BusinessContext businessContext, dynamic parameters)
		{
			using (EntityExtractor reportExtractor = this.GetReportExtractor(workerApp,businessContext,parameters.datasetId,parameters.entityId))
			{
				return ReportingModule.CreateReport (reportExtractor);
			}
		}

		internal static Response CreateReport(EntityExtractor extractor)
		{
			
			var itemCount = extractor.Accessor.GetItemCount ();

			if (itemCount > 10000)
			{
				throw new System.InvalidOperationException ("Too many items in extractor: " + itemCount.ToString ());
			}

			EntityWriter writer = ReportingModule.GetEntityWriter (extractor);

			var filename = writer.GetFilename ();
			var stream   = writer.GetStream ();

			return CoreResponse.CreateStreamResponse (stream, filename);
		}

		private EntityExtractor GetReportExtractor(WorkerApp workerApp, BusinessContext businessContext, string datasetId,string entityId)
		{
			var caches = this.CoreServer.Caches;

			var userManager = workerApp.UserManager;
			var databaseManager = this.CoreServer.DatabaseManager;
			

			Func<Database, DataSetAccessor> dataSetAccessorGetter = db =>
			{
				return db.GetDataSetAccessor (workerApp.DataSetGetter);
			};

			string rawDatabaseId = datasetId;
			var databaseId = DataIO.ParseDruid (rawDatabaseId);
			var database = databaseManager.GetDatabase (databaseId);
			
			var entity = EntityIO.ResolveEntity (businessContext, (string) entityId);
			var context = DataContextPool.GetDataContext (entity);
			var id = context.GetNormalizedEntityKey (entity).Value.RowKey.Id.Value;

			Action<DataContext, DataLayer.Loader.Request, AbstractEntity> customizer = (d, r, e) =>
			{
				r.Conditions.Add (ReportingModule.CreateCondition (e, id));
			};

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId,"","",customizer
			);
		}

		private static EntityWriter GetEntityWriter(EntityExtractor extractor)
		{
			var metaData = extractor.Metadata;
			var accessor = extractor.Accessor;

			var layout      = LabelLayout.Sheet_A4_Simple;
			var entitytype  = metaData.EntityTableMetadata.EntityType;

			return new LetterDocumentWriter (metaData, accessor, layout);
		}

		private static  DataExpression CreateCondition(AbstractEntity example,long id)
		{
			return new ValueSetComparison (InternalField.CreateId (example), SetComparator.In, new Constant[] {new Constant (id)});
		}


	}


}
