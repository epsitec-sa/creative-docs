using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

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


		public IEnumerable<AbstractMenuItem> GetDatabases(UserManager userManager)
		{
			var dataSets = this.GetDataSets (userManager).ToList ();

			var mainDatabases = this.GetMainDatabases (dataSets);
			var secondaryDatabases = this.GetSecondaryDatabases (dataSets);

			return mainDatabases.Concat (secondaryDatabases);
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


		private IEnumerable<AbstractMenuItem> GetMainDatabases(IEnumerable<DataSetMetadata> dataSets)
		{
			return dataSets
				.Where (d => d.DisplayGroupId.IsEmpty)
				.Where (d => d.IsDisplayed)
				.Select (d => new DatabaseMenuItem (d));
		}


		private IEnumerable<AbstractMenuItem > GetSecondaryDatabases(IEnumerable<DataSetMetadata> dataSets)
		{
			return dataSets
				.Where (d => !d.DisplayGroupId.IsEmpty)
				.Where (d => d.IsDisplayed)
				.GroupBy (d => d.DisplayGroupId)
				.Select (g => new SubMenuItem (
					SafeResourceResolver.Instance.GetCaption (g.Key),
					g.Select (d => new DatabaseMenuItem (d))
				));
		}


		private Database GetDatabase(DataSetMetadata dataSet, IEnumerable<ColumnRef<EntityColumnSort>> customSorters)
		{
			var entityTable = dataSet.EntityTableMetadata;

			var columns = this.GetColumns (entityTable);

			var sorters = customSorters == null
				? this.GetSorters (entityTable, columns)
				: this.GetSorters (customSorters, columns);

			var enableCreate = dataSet.EnableCreate;
			var enableDelete = dataSet.EnableDelete;
			var creationViewId = dataSet.CreationViewId;
			var deletionViewId = dataSet.DeletionViewId;
			
			return new Database (dataSet, columns, sorters, enableCreate, enableDelete, creationViewId, deletionViewId);
		}


		private IEnumerable<Column> GetColumns(EntityTableMetadata entityTable)
		{
			if (entityTable == null)
			{
				return Enumerable.Empty<Column> ();
			}

			return entityTable.Columns
				.Select (c => new Column (c))
				.ToList ();
		}


		private IEnumerable<Sorter> GetSorters(IEnumerable<ColumnRef<EntityColumnSort>> customSorters, IEnumerable<Column> columns)
		{
			return from customSorter in customSorters
				   let column = columns.FirstOrDefault (c => c.Name == customSorter.Id)
				   where column != null
				   let sortOrder = this.GetSortOrder (customSorter.Value.SortOrder)
				   select new Sorter (column, sortOrder);
		}


		private IEnumerable<Sorter> GetSorters(EntityTableMetadata entityTable, IEnumerable<Column> columns)
		{
			if (entityTable == null)
			{
				return Enumerable.Empty<Sorter> ();
			}

			return from entityColumn in entityTable.GetSortColumns ()
				   let column = columns.First (c => c.MetaData == entityColumn)
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


		public Database GetDatabase(UserManager userManager, Druid commandId)
		{
			var dataSet = this.GetDataSet (commandId);

			var customSettings = userManager.ActiveSession.GetDataSetSettings (dataSet);

			var customSorters = customSettings.Sort.Count > 0
				? customSettings.Sort
				: null;

			return this.GetDatabase (dataSet, customSorters);
		}


		public Database GetDatabase(Druid commandId)
		{
			var dataSet = this.GetDataSet (commandId);

			return this.GetDatabase (dataSet, null);
		}


		private DataSetMetadata GetDataSet(Druid commandId)
		{
			var dataSet = this.dataStore.FindDataSet (commandId);

			if (dataSet == null)
			{
				var id = commandId.ToCompactString ();

				throw new Exception ("The database '" + id + "' does not exist.");
			}

			return dataSet;
		}


		public Druid GetDatabaseCommandId(Type entityType)
		{
			var dataSet = this.dataStore.FindDefaultDataSet (entityType);

			if (dataSet == null)
			{
				throw new Exception ("The database '" + entityType.FullName + "' does not exist.");
			}

			return dataSet.Command.Caption.Id;
		}


		private readonly DataStoreMetadata dataStore;


	}


}
