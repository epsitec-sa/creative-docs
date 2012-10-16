using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

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


		public string Title
		{
			get
			{
				return this.DataSetMetadata.BaseShowCommand.Caption.DefaultLabel;
			}
		}


		public string Name
		{
			get
			{
				return Tools.TypeToString (this.DataSetMetadata.DataSetEntityType);
			}
		}


		public string IconClass
		{
			get
			{
				var iconUri = this.DataSetMetadata.BaseShowCommand.Caption.Icon;
				var type = this.DataSetMetadata.DataSetEntityType;
				
				return IconManager.GetCssClassName (type, iconUri, IconSize.ThirtyTwo);
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
			var genericMethod = method.MakeGenericMethod (this.DataSetMetadata.DataSetEntityType);
			var arguments = new object[] { businessContext };

			return (AbstractEntity) genericMethod.Invoke (null, arguments);
		}


		private static T CreateEntityImplementation<T>(BusinessContext businessContext)
			where T : AbstractEntity, new ()
		{
			var entity = businessContext.CreateEntity<T> ();

			// NOTE Here we need to include the empty entities, otherwise we might be in the case
			// where the entity that we just have created will be empty and thus not saved and this
			// will lead the user to click like a maniac on the "create" button without noticeable
			// result other than him becoming mad :-P

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

			return entity;
		}


		public Dictionary<string, object> GetEntityData(DataContext dataContext, PropertyAccessorCache propertyAccessorCache, AbstractEntity entity)
		{
			var id = Tools.GetEntityId (dataContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();

			var data = new Dictionary<string, object> ()
			{
			    { "id", id },
			    { "summary", summary },
			};

			foreach (var column in this.Columns.Where (c => !c.Hidden))
			{
				data[column.Name] = column.GetColumnData (propertyAccessorCache, entity);
			}

			return data;
		}


		public Dictionary<string, object> GetSummaryDataDictionary()
		{
			return new Dictionary<string, object> ()
			{
			    { "title", this.Title },
			    { "name", this.Name },
			    { "cssClass", this.IconClass },
			};
		}


		public Dictionary<string, object> GetDataDictionary(PropertyAccessorCache propertyAccessorCache)
		{
			var columns = this.columns
				.Select (c => c.GetDataDictionary (propertyAccessorCache))
				.ToList ();

			var sorters = this.Sorters
				.Select (s => s.GetDataDictionary ())
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
