//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbReader</c> class implements a factory which returns instances
	/// of <see cref="System.Data.IDataReader"/> based on a query definition.
	/// </summary>
	public class DbReader
	{
		public DbReader(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.longToShortTableAliasMap = new Dictionary<string, string> ();
			this.shortAliasToTableMap = new Dictionary<string, DbTable> ();
			this.shortAliasToColumnMap = new Dictionary<string, DbTableColumn> ();
			this.renamedTableColumns = new List<DbTableColumn>();
			this.conditions = new List<DbSelectCondition> ();
			this.orderByTableColumns = new Dictionary<string, SqlSortOrder> ();
		}


		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		public void AddQueryFields(IEnumerable<DbTableColumn> queryFields)
		{
			foreach (DbTableColumn queryField in queryFields)
			{
				this.RegisterTableColumn (queryField);
			}
		}

		public void AddSortOrder(DbTableColumn tableColumn, SqlSortOrder order)
		{
			string shortColumnAlias = this.FindShortColumnAlias (tableColumn);

			if (shortColumnAlias == null)
			{
				throw new System.ArgumentException (string.Format ("Specified sort column '{0}' does not belong to the query fields", tableColumn.ToString ()));
			}

			this.orderByTableColumns[shortColumnAlias] = order;
		}

		private string FindShortColumnAlias(DbTableColumn tableColumn)
		{
			foreach (KeyValuePair<string, DbTableColumn> pair in this.shortAliasToColumnMap)
			{
				if (pair.Value == tableColumn)
				{
					return pair.Key;
				}
			}

			return null;
		}

		private string GetShortColumnAlias(DbTableColumn tableColumn)
		{
			string shortColumnAlias = this.FindShortColumnAlias (tableColumn);

			if (shortColumnAlias == null)
			{
				shortColumnAlias = this.RegisterTableColumn (tableColumn);
			}

			return shortColumnAlias;
		}

		public void AddCondition(DbSelectCondition condition)
		{
			foreach (DbTableColumn column in condition.Columns)
			{
				this.RegisterTable (column);
			}

			this.conditions.Add (condition);
		}


		public System.Data.IDataReader CreateReader(DbTransaction transaction)
		{
			ISqlBuilder builder = transaction.SqlBuilder;
			SqlSelect select = this.CreateSelect ();

			builder.SelectData (select);

			System.Data.IDbCommand command = builder.Command;
			command.Transaction = transaction.Transaction;
			System.Data.IDataReader reader = command.ExecuteReader ();

			return reader;
		}



		internal SqlSelect CreateSelect()
		{
			SqlSelect select = new SqlSelect ();

			this.CreateSelectColumns (select);
			this.CreateSelectTables (select);
			this.CreateSelectConditions (select);
			
			return select;
		}

		private string RegisterTableColumn(DbTableColumn originalTableColumn)
		{
			int columnIndex = this.renamedTableColumns.Count;
			
			string shortColumnAlias = string.Format (System.Globalization.CultureInfo.InvariantCulture, "C{0}", columnIndex);
			string shortTableAlias  = this.RegisterTable (originalTableColumn);
			
			DbTableColumn renamedTableColumn = new DbTableColumn (originalTableColumn.Column);

			renamedTableColumn.TableAlias  = shortTableAlias;
			renamedTableColumn.ColumnAlias = shortColumnAlias;

			this.renamedTableColumns.Add (renamedTableColumn);

			this.shortAliasToColumnMap[shortColumnAlias] = originalTableColumn;

			return shortColumnAlias;
		}

		private string RegisterTable(DbTableColumn originalTableColumn)
		{
			string longTableAlias = originalTableColumn.TableAlias;
			string shortTableAlias;

			if (this.longToShortTableAliasMap.TryGetValue (longTableAlias, out shortTableAlias))
			{
				//	The table is already known and has a short name associated to it.
			}
			else
			{
				//	The table is not yet known; allocate a short name ("T0", "T1", ...)
				//	which can then be used as an alias for the table.

				int tableIndex = this.longToShortTableAliasMap.Count;
				
				shortTableAlias = string.Format (System.Globalization.CultureInfo.InvariantCulture, "T{0}", tableIndex);

				this.longToShortTableAliasMap[longTableAlias] = shortTableAlias;
				this.shortAliasToTableMap[shortTableAlias] = originalTableColumn.Table;
			}

			return shortTableAlias;
		}

		private void CreateSelectColumns(SqlSelect select)
		{
			foreach (DbTableColumn tableColumn in this.renamedTableColumns)
			{
				string   alias     = tableColumn.ColumnAlias;
				DbColumn column    = tableColumn.Column;
				string   tableName = tableColumn.TableAlias;

				SqlField fieldDefinition = SqlField.CreateAliasedName (tableName, column.GetSqlName (), alias);
				SqlSortOrder order;

				if (this.orderByTableColumns.TryGetValue (tableColumn.ColumnAlias, out order))
				{
					fieldDefinition.SortOrder = order;
				}

				select.Fields.Add (fieldDefinition);
			}
		}

		private void CreateSelectTables(SqlSelect select)
		{
			foreach (KeyValuePair<string, DbTable> item in this.GetAliasTables ())
			{
				string  alias = item.Key;
				DbTable table = item.Value;

				SqlField tableDefinition = SqlField.CreateAliasedName (table.GetSqlName (), alias);

				select.Tables.Add (tableDefinition);
			}
		}

		private void CreateSelectConditions(SqlSelect select)
		{
			foreach (DbSelectCondition condition in this.conditions)
			{
				condition.ReplaceTableColumns (this.TranslateToShortTableAlias);
				condition.CreateConditions (select.Conditions);
			}
		}

		private DbTableColumn TranslateToShortTableAlias(DbTableColumn tableColumn)
		{
			string tableAlias = this.longToShortTableAliasMap[tableColumn.TableAlias];
			
			return new DbTableColumn (tableAlias, tableColumn.Column);
		}

		private IEnumerable<KeyValuePair<string, DbTable>> GetAliasTables()
		{
			List<string> keys = new List<string> (this.shortAliasToTableMap.Keys);
			
			keys.Sort ();
			
			foreach (string key in keys)
			{
				DbTable value = this.shortAliasToTableMap[key];

				yield return new KeyValuePair<string, DbTable> (key, value);
			}
		}


		private readonly DbInfrastructure infrastructure;
		private readonly Dictionary<string, string> longToShortTableAliasMap;
		private readonly Dictionary<string, DbTable> shortAliasToTableMap;
		private readonly Dictionary<string, DbTableColumn> shortAliasToColumnMap;
		private readonly List<DbTableColumn> renamedTableColumns;
		private readonly List<DbSelectCondition> conditions;
		private readonly Dictionary<string, SqlSortOrder> orderByTableColumns;
	}
}
