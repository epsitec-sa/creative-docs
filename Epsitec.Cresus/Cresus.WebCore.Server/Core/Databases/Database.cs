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


		public Database
		(
			DataSetMetadata dataSetMetadata,
			IEnumerable<Column> columns,
			IEnumerable<Sorter> sorters,
			IEnumerable<AbstractContextualMenuItem> menuItems,
			IEnumerable<LabelExportItem> labelExportItems,
			bool enableCreate,
			bool enableDelete,
			int? creationViewId,
			int? deletionViewId
		)
		{
			this.dataSetMetadata = dataSetMetadata;
			this.columns = columns.ToList ();
			this.sorters = sorters.ToList ();
			this.menuItems = menuItems.ToList ();
			this.labelExportItems = labelExportItems.ToList ();
			this.enableCreate = enableCreate;
			this.enableDelete = enableDelete;
			this.creationViewId = creationViewId;
			this.deletionViewId = deletionViewId;
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


		public IEnumerable<AbstractContextualMenuItem> MenuItems
		{
			get
			{
				return this.menuItems;
			}
		}


		public IEnumerable<LabelExportItem> LabelExportItems
		{
			get
			{
				return this.labelExportItems;
			}
		}


		public bool EnableCreate
		{
			get
			{
				return this.enableCreate;
			}
		}


		public bool EnableDelete
		{
			get
			{
				return this.enableDelete;
			}
		}


		public int? CreationViewId
		{
			get
			{
				return this.creationViewId;
			}
		}

		public int? DeletionViewId
		{
			get
			{
				return this.deletionViewId;
			}
		}

		public Type EntityType
		{
			get
			{
				return this.DataSetMetadata.EntityTableMetadata.EntityType;
			}
		}


		public DataSetAccessor GetDataSetAccessor(DataSetGetter dataSetGetter)
		{
			return dataSetGetter.ResolveAccessor (this.DataSetMetadata);
		}


		public AbstractEntity CreateEntity(BusinessContext businessContext)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Static;
			var method = typeof (Database).GetMethod ("CreateEntityImplementation", flags);
			var genericMethod = method.MakeGenericMethod (this.EntityType);
			var arguments = new object[] { businessContext };

			return (AbstractEntity) genericMethod.Invoke (null, arguments);
		}


		private static T CreateEntityImplementation<T>(BusinessContext businessContext)
			where T : AbstractEntity, new ()
		{
			var entity = businessContext.CreateAndRegisterEntity<T> ();

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IncludeEmpty);

			return entity;
		}


		public Dictionary<string, object> GetEntityData
		(
			IEnumerable<Column> columns,
			DataContext dataContext,
			Caches caches,
			AbstractEntity entity
		)
		{
			var menuItems = Enumerable.Empty<SummaryNavigationContextualMenuItem> ();

			return this.GetEntityData (columns, menuItems, dataContext, caches, entity);
		}


		public Dictionary<string, object> GetEntityData
		(
			IEnumerable<Column> columns,
			IEnumerable<SummaryNavigationContextualMenuItem> menuItems,
			DataContext dataContext,
			Caches caches,
			AbstractEntity entity
		)
		{
			var id = EntityIO.GetEntityId (dataContext, entity);
			var summary = entity.GetCompactSummary ().ToSimpleText ();

			var data = new Dictionary<string, object> ()
			{
				{ "id", id },
				{ "summary", summary },
			};

			foreach (var column in columns)
			{
				var columnId = column.GetId (caches);

				data[columnId] = column.GetColumnData (dataContext, caches, entity);
			}

			// For now, the columns cannot have a value of type entity, and the menu items must all
			// have a value of type entity. Therefore we know that they are different and that we
			// don't compute the same value twice. If these one of these assumption where to change,
			// we might want to check for duplicates between the columns and the menu items.

			foreach (var menuItem in menuItems)
			{
				var columnId = menuItem.GetId (caches);

				data[columnId] = menuItem.GetEntityId (dataContext, caches, entity);
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

			var menuItems = this.MenuItems
				.Select (m => m.GetDataDictionary (caches))
				.ToList ();

			var labelItems = this.LabelExportItems
				.Select (s => s.GetDataDictionary ())
				.ToList ();

			return new Dictionary<string, object> ()
			{
				{ "enableCreate", this.EnableCreate },
				{ "enableDelete", this.EnableDelete },
				{ "entityTypeId", caches.TypeCache.GetId (this.EntityType) },
				{ "creationViewId", this.CreationViewId },
				{ "deletionViewId", this.DeletionViewId },
				{ "columns", columns },
				{ "sorters", sorters },
				{ "menuItems", menuItems },
				{ "labelItems", labelItems },
			};
		}


		private readonly DataSetMetadata dataSetMetadata;


		private readonly List<Column> columns;


		private readonly List<Sorter> sorters;


		private readonly List<AbstractContextualMenuItem> menuItems;


		private readonly List<LabelExportItem> labelExportItems;


		private readonly bool enableCreate;


		private readonly bool enableDelete;


		private readonly int? creationViewId;


		private readonly int? deletionViewId;


	}


}
