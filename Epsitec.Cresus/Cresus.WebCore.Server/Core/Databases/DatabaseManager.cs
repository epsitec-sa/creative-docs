using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.Core.Business.UserManagement;

using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class DatabaseManager
	{


		public DatabaseManager()
		{
			this.dataStore = DataStoreMetadata.Current;
		}


		public IEnumerable<Database> GetDatabases(UserManager userManager)
		{
			return this.GetDataSets (userManager).Select (d => this.GetDatabase (d));
		}


		private IEnumerable<DataSetMetadata> GetDataSets(UserManager userManager)
		{
			var user = userManager.AuthenticatedUser;
			var roles = userManager.GetUserRoles (user).ToList ();

			var dataSets = this.dataStore.DataSets;

			var globalDataSets = dataSets.Where (x => this.IsGlobalDataSet(x));
			var userDataSets = dataSets.Where (x => this.IsUserDataSet (x, roles));

			return globalDataSets.Concat (userDataSets);
		}


		private bool IsGlobalDataSet(DataSetMetadata dataSet)
		{
			return dataSet.DisplayGroupId.IsEmpty;
		}


		private bool IsUserDataSet(DataSetMetadata dataSet, IEnumerable<string> roles)
		{
			return dataSet.DisplayGroupId.IsValid && dataSet.MatchesAnyUserRole (roles);
		}


		private Database GetDatabase(DataSetMetadata dataSet)
		{
			var entityType = dataSet.DataSetEntityType;
			
			var entityTypeId = EntityInfo.GetTypeId (entityType);
			var commandCaption = dataSet.BaseShowCommand.Caption;
			var title = commandCaption.DefaultLabel;
			var iconUri = commandCaption.Icon;

			IEnumerable<Column> columns;
			IEnumerable<Sorter> sorters;
						
			var entityTable = this.dataStore.FindTable (entityTypeId);

			if (entityTable == null)
			{
				columns = Enumerable.Empty<Column> ();
				sorters = Enumerable.Empty<Sorter> ();
			}
			else
			{
				var mapping = this.GetColumnMapping (entityTable);
				columns = this.GetColumns (entityTable, mapping);
				sorters = this.GetSorters (entityTable, mapping);
			}

			return Database.Create (entityType, title, iconUri, columns, sorters);
		}


		private Dictionary<EntityColumnMetadata, Column> GetColumnMapping(EntityTableMetadata entityTable)
		{
			var mapping = from entityColumn in entityTable.Columns
						  let title = entityColumn.GetColumnTitle ().ToString ()
						  let name = InvariantConverter.ToString (entityColumn.CaptionId.ToLong ())
						  let hidden = entityColumn.DefaultDisplay.Mode != ColumnDisplayMode.Visible
						  let sortable = entityColumn.CanSort
						  let filterable = entityColumn.CanFilter
						  let lambda = entityColumn.Expression
						  let column = new Column (title, name, hidden, sortable, filterable, lambda)
						  select Tuple.Create (entityColumn, column);

			return mapping.ToDictionary (t => t.Item1, t => t.Item2);
		}


		private IEnumerable<Column> GetColumns(EntityTableMetadata entityTable, Dictionary<EntityColumnMetadata, Column> mapping)
		{
			// Here we can't simply enumerate the values in the mapping, since we want to keep the
			// columns in the order they are in the entityTable, and the dictionary doesn't provide
			// any guarantee on the order of its elements when they are enumerated.

			return entityTable.Columns.Select (c => mapping[c]);
		}


		private IEnumerable<Sorter> GetSorters(EntityTableMetadata entityTable, Dictionary<EntityColumnMetadata, Column> mapping)
		{
			return from entityColumn in entityTable.GetSortColumns ()
				   let column = mapping[entityColumn]
				   let sortOrder = this.GetSortOrder (entityColumn.DefaultSort.SortOrder)
				   select new Sorter (column, sortOrder);
		}


		private SortOrder GetSortOrder(ColumnSortOrder sortOrder)
		{
			switch (sortOrder)
			{
				case ColumnSortOrder.Ascending:
					return SortOrder.Ascending;

				case ColumnSortOrder.Descending:
					return SortOrder.Descending;

				default:
					throw new NotImplementedException ();
			}
		}


		public Database GetDatabase(string name)
		{
			var entityType = Tools.ParseType (name);

			// We check if we have a definition for the database and if we don't, we create a
			// a default database for it.

			var dataSets = this.dataStore.DataSets;
			var dataSet = dataSets.FirstOrDefault (d => d.DataSetEntityType == entityType);

			if (dataSet == null)
			{
				return Database.Create (entityType);
			}
			else
			{
				return this.GetDatabase (dataSet);
			}
		}


		private readonly DataStoreMetadata dataStore;


	}


}
