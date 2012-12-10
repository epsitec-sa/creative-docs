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
				.Select (d => new DatabaseMenuItem (d));
		}


		private IEnumerable<AbstractMenuItem > GetSecondaryDatabases(IEnumerable<DataSetMetadata> dataSets)
		{
			return dataSets
				.Where (d => !d.DisplayGroupId.IsEmpty)
				.GroupBy (d => d.DisplayGroupId)
				.Select (g => new SubMenuItem (
					SafeResourceResolver.Instance.GetCaption (g.Key),
					g.Select (d => new DatabaseMenuItem (d))
				));
		}


		private Database GetDatabase(DataSetMetadata dataSet)
		{
			var entityTable = dataSet.EntityTableMetadata;

			IEnumerable<Column> columns;
			IEnumerable<Sorter> sorters;

			if (entityTable == null)
			{
				columns = Enumerable.Empty<Column> ();
				sorters = Enumerable.Empty<Sorter> ();
			}
			else
			{
				columns = this.GetColumns (entityTable);
				sorters = this.GetSorters (entityTable, columns);
			}

			return new Database(dataSet, columns, sorters);
		}


		private IEnumerable<Column> GetColumns(EntityTableMetadata entityTable)
		{
			return entityTable.Columns
				.Select (c => new Column (c))
				.ToList ();
		}


		private IEnumerable<Sorter> GetSorters(EntityTableMetadata entityTable, IEnumerable<Column> columns)
		{
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


		public Database GetDatabase(string name)
		{
			var entityType = Tools.ParseType (name);

			var dataSets = this.dataStore.DataSets;
			var dataSet = dataSets.FirstOrDefault (d => d.DataSetEntityType == entityType && d.IsDefault);

			if (dataSet == null)
			{
				throw new Exception ("The database '" + name + "' does not exist.");
			}
			
			return this.GetDatabase (dataSet);
		}


		private readonly DataStoreMetadata dataStore;


	}


}
