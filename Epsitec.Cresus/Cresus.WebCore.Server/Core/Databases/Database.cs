using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core.IO;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Reflection;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Database
	{


		public Database(DataSetMetadata dataSetMetadata, IEnumerable<Column> columns, IEnumerable<Sorter> sorters)
		{
			this.dataSetMetadata = dataSetMetadata;
			this.columns = columns.ToList ();
			this.sorters = sorters.ToList ();
		}


		public DataSetMetadata DataSetMetadata
		{
			get
			{
				return this.dataSetMetadata;
			}
		}


		public IEnumerable<Column> Columns
		{
			get
			{
				return this.columns;
			}
		}


		public IEnumerable<Sorter> Sorters
		{
			get
			{
				return this.sorters;
			}
		}


		public DataSetAccessor GetDataSetAccessor(DataSetGetter dataSetGetter)
		{
			var dataSet = this.DataSetMetadata;

			var dataSetAccessor = dataSetGetter.ResolveAccessor (dataSet);
			dataSetAccessor.MakeDependent ();

			return dataSetAccessor;
		}


		public AbstractEntity CreateEntity(BusinessContext businessContext)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Static;
			var method = typeof (Database).GetMethod ("CreateEntityImplementation", flags);
			var genericMethod = method.MakeGenericMethod (this.DataSetMetadata.EntityTableMetadata.EntityType);
			var arguments = new object[] { businessContext };

			return (AbstractEntity) genericMethod.Invoke (null, arguments);
		}


		private static T CreateEntityImplementation<T>(BusinessContext businessContext)
			where T : AbstractEntity, new ()
		{
			var entity = businessContext.CreateEntity<T> ();

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

			return entity;
		}


		public Dictionary<string, object> GetEntityData(DataContext dataContext, Caches caches, AbstractEntity entity)
		{
			var id = EntityIO.GetEntityId (dataContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();

			var data = new Dictionary<string, object> ()
			{
			    { "id", id },
			    { "summary", summary },
			};

			foreach (var column in this.Columns.Where (c => !c.Hidden))
			{
				var columnId = column.GetId (caches);

				data[columnId] = column.GetColumnData (caches, entity);
			}

			return data;
		}


		public Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			var columns = this.columns
				.Select (c => c.GetDataDictionary (caches))
				.ToList ();

			var sorters = this.Sorters
				.Select (s => s.GetDataDictionary (caches))
				.ToList ();

			return new Dictionary<string, object> ()
			{
				{ "columns", columns },
				{ "sorters", sorters },
			};
		}


		private readonly DataSetMetadata dataSetMetadata;
		
		
		private readonly List<Column> columns;


		private readonly List<Sorter> sorters;


	}


}
