//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Cresus.WebCore.Server.Core.Databases;
using Epsitec.Cresus.WebCore.Server.Core.IO;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Extraction
{
	using Database = Core.Databases.Database;


	/// <summary>
	/// This class provides a view over a database, possible filtered and sorted. It can be used to
	/// access ranges of entities within this view.
	/// </summary>
	internal sealed class EntityExtractor : System.IDisposable
	{
		private EntityExtractor(Database database, DataSetMetadata metadata, DataSetAccessor accessor)
		{
			this.database = database;
			this.metadata = metadata;
			this.accessor = accessor;
		}


		public Database							Database
		{
			get
			{
				return this.database;
			}
		}

		public DataSetMetadata					Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public DataSetAccessor					Accessor
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


		public static EntityExtractor Create(BusinessContext businessContext, Caches caches, UserManager userManager, DatabaseManager databaseManager, System.Func<Database, DataSetAccessor> dataSetAccessorGetter, Druid databaseId, string rawSorters, string rawFilters,string rawQuery, System.Action<DataContext, Request, AbstractEntity> customizer = null)
		{
			var database = databaseManager.GetDatabase (databaseId);

			var dataSetMetadata = database.DataSetMetadata;
			var sorters = SorterIO.ParseSorters (caches, database, rawSorters);
			var filter = FilterIO.ParseFilter (businessContext, caches, database, rawFilters);

			var accessor = EntityExtractor.CreateAccessor (userManager, dataSetAccessorGetter, database, sorters, filter, rawQuery, customizer);

			return new EntityExtractor (database, dataSetMetadata, accessor);
		}

		private static DataSetAccessor CreateAccessor(UserManager userManager, System.Func<Database, DataSetAccessor> dataSetAccessorGetter, Database database, IEnumerable<ColumnRef<EntityColumnSort>> sorters, EntityFilter filter, string queryName, System.Action<DataContext, Request, AbstractEntity> customizer)
		{
			var dataSetMetadata = database.DataSetMetadata;

			var session = userManager.ActiveSession;
			var settings = session.GetDataSetSettings (dataSetMetadata);

			settings.Sort.Clear ();
			settings.Sort.AddRange (sorters);
			settings.Filter = filter;

			if(queryName != null)
			{
				var query = settings.AvailableQueries.Where (q => q.Name == queryName).FirstOrDefault ();
				if(query!= null)
				{
					settings.ActiveQuery = query;
				}
			}
			
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


		private readonly Database				database;
		private readonly DataSetMetadata		metadata;
		private readonly DataSetAccessor		accessor;
	}
}
