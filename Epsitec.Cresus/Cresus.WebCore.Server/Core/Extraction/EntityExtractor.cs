using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Favorites;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{


	using Database = Core.Databases.Database;


	internal sealed class EntityExtractor : IDisposable
	{


		public EntityExtractor(Database database, DataSetAccessor accessor)
		{
			this.database = database;
			this.accessor = accessor;
		}


		public Database Database
		{
			get
			{
				return this.database;
			}
		}


		public DataSetAccessor Accessor
		{
			get
			{
				return this.accessor;
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.accessor.Dispose ();
		}


		#endregion


		public static EntityExtractor Create(BusinessContext businessContext, Caches caches, UserManager userManager, DatabaseManager databaseManager, Func<Database, DataSetAccessor> dataSetAccessorGetter, FavoritesCollection favorites, string rawSorters, string rawFilters)
		{
			var databaseId = favorites.DatabaseId;
			
			Action<DataContext, Request, AbstractEntity> customizer = (d, r, e) =>
			{
				r.Conditions.Add (favorites.CreateCondition (e));
			};

			return EntityExtractor.Create
			(
				businessContext, caches, userManager, databaseManager, dataSetAccessorGetter,
				databaseId, rawSorters, rawFilters, customizer
			);
		}


		public static EntityExtractor Create(BusinessContext businessContext, Caches caches, UserManager userManager, DatabaseManager databaseManager, Func<Database, DataSetAccessor> dataSetAccessorGetter, Druid databaseId, string rawSorters, string rawFilters, Action<DataContext, Request, AbstractEntity> customizer = null)
		{
			var database = databaseManager.GetDatabase (databaseId);
			
			var sorters = SorterIO.ParseSorters (caches, database, rawSorters);
			var filter = FilterIO.ParseFilter (businessContext, caches, database, rawFilters);

			var accessor = EntityExtractor.CreateAccessor
			(
				userManager, dataSetAccessorGetter, database, sorters, filter, customizer
			);

			return new EntityExtractor (database, accessor);
		}


		private static DataSetAccessor CreateAccessor(UserManager userManager, Func<Database, DataSetAccessor> dataSetAccessorGetter, Database database, IEnumerable<ColumnRef<EntityColumnSort>> sorters, EntityFilter filter, Action<DataContext, Request, AbstractEntity> customizer)
		{
			var dataSetMetadata = database.DataSetMetadata;
			
			var session = userManager.ActiveSession;
			var settings = session.GetDataSetSettings (dataSetMetadata);

			settings.Sort.Clear ();
			settings.Sort.AddRange (sorters);
			settings.Filter = filter;

			session.SetDataSetSettings (dataSetMetadata, settings);

			DataSetAccessor accessor = null;

			try
			{
				accessor = dataSetAccessorGetter (database);
				accessor.MakeDependent ();

				if (customizer != null)
				{
					accessor.Customizers.Add (customizer);
				}

				return accessor;
			}
			catch
			{
				if (accessor != null)
				{
					accessor.Dispose ();
				}

				throw;
			}
		}


		private readonly Database database;


		private readonly DataSetAccessor accessor;


	}


}
