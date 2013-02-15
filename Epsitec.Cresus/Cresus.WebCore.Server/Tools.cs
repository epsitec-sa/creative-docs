using Epsitec.Common.Debug;

using Epsitec.Common.IO;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.WebCore.Server.Core;
using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;
using Epsitec.Cresus.WebCore.Server.NancyHosting;

using Nancy;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server
{


	internal static class Tools
	{


		public static IDisposable Bind(this BusinessContext businessContext, params AbstractEntity[] entities)
		{
			return businessContext.Bind ((IEnumerable<AbstractEntity>) entities);
		}


		public static IDisposable Bind(this BusinessContext businessContext, IEnumerable<AbstractEntity> entities)
		{
			var entitiesToDispose = new List<AbstractEntity> ();

			try
			{
				foreach (var entity in entities)
				{
					businessContext.Register (entity);

					entitiesToDispose.Add (entity);
				}
			}
			catch (Exception)
			{
				foreach (var entity in entitiesToDispose)
				{
					businessContext.Unregister (entity);
				}

				throw;
			}

			Action action = () =>
			{
				foreach (var entity in entities)
				{
					businessContext.Unregister (entity);
				}
			};

			return DisposableWrapper.CreateDisposable (action);
		}


		public static Response GetEntities(BusinessContext businessContext, Caches caches, UserManager userManager, DatabaseManager databaseManager, Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter, Druid databaseId, string rawSorters, string rawFilters, int start, int limit)
		{
			var database = databaseManager.GetDatabase (databaseId);

			Tools.SetupSortersAndFilters (businessContext, caches, userManager, database, rawSorters, rawFilters);

			using (var dataSetAccessor = dataSetAccessorGetter (database))
			{
				dataSetAccessor.MakeDependent ();

				var dataContext = dataSetAccessor.IsolatedDataContext;

				var total = dataSetAccessor.GetItemCount ();
				var entities = dataSetAccessor.GetItems (start, limit);
				
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
		}


		public static Response GetEntityIndex(BusinessContext businessContext, Caches caches, UserManager userManager, DatabaseManager databaseManager, Func<Core.Databases.Database, DataSetAccessor> dataSetAccessorGetter, Druid databaseId, string rawSorters, string rawFilters, string rawEntityKey)
		{
			var database = databaseManager.GetDatabase (databaseId);

			Tools.SetupSortersAndFilters (businessContext, caches, userManager, database, rawSorters, rawFilters);

			var entityKey = EntityIO.ParseEntityId (rawEntityKey);

			using (var dataSetAccessor = dataSetAccessorGetter (database))
			{
				dataSetAccessor.MakeDependent ();

				int? index = dataSetAccessor.IndexOf (entityKey);

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


		public static void SetupSortersAndFilters(BusinessContext businessContext, Caches caches, UserManager userManager, Core.Databases.Database database, string rawSorters, string rawFilters)
		{
			var dataSetMetadata = database.DataSetMetadata;
			
			var session = userManager.ActiveSession;
			var settings = session.GetDataSetSettings (dataSetMetadata);

			var sorters = SorterIO.ParseSorters (caches, database, rawSorters);
			var filter = FilterIO.ParseFilter (businessContext, caches, database, rawFilters);

			settings.Sort.Clear ();
			settings.Sort.AddRange (sorters);
			settings.Filter = filter;

			session.SetDataSetSettings (dataSetMetadata, settings);
		}


		public static string GetOptionalParameter(dynamic parameter)
		{
			return parameter.HasValue
				? parameter.Value
				: null;
		}


		[Conditional ("DEBUG")]
		public static void LogMessage(string message)
		{
			Logger.LogToConsole (message);
		}


		public static void LogError(string message)
		{
			Tools.LogMessage (message);

			var path = Tools.GetErrorFilePath ();
			ErrorLogger.LogErrorMessage (message, path);
		}


		private static string GetErrorFilePath()
		{
			var d = DateTime.Now;
			var template = "crash {0} {1:0000}-{2:00}-{3:00} {4:00}-{5:00}-{6:00}.log";
			var name = string.Format (template, Guid.NewGuid(), d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second);
			
			return Path.Combine
			(
				Environment.GetFolderPath (Environment.SpecialFolder.CommonApplicationData),
				"Epsitec",
				"WebCore",
				"Logs",
				name
			);
		}


	}


}
