﻿//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
				this.RenameTableColumn (queryField);
			}
		}

		public void AddCondition()
		{
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

		private void RenameTableColumn(DbTableColumn originalTableColumn)
		{
			int columnIndex = this.renamedTableColumns.Count;
			int tableIndex = this.longToShortTableAliasMap.Count;

			string longTableAlias = originalTableColumn.TableAlias;
			
			string shortColumnAlias = string.Format (System.Globalization.CultureInfo.InvariantCulture, "C{0}", columnIndex);
			string shortTableAlias;

			if (this.longToShortTableAliasMap.TryGetValue (longTableAlias, out shortTableAlias) == false)
			{
				shortTableAlias = string.Format (System.Globalization.CultureInfo.InvariantCulture, "T{0}", tableIndex);

				this.longToShortTableAliasMap[longTableAlias] = shortTableAlias;
				this.shortAliasToTableMap[shortTableAlias] = originalTableColumn.Table;
			}

			DbTableColumn renamedTableColumn = new DbTableColumn (originalTableColumn.Table, originalTableColumn.Column);

			renamedTableColumn.TableAlias  = shortTableAlias;
			renamedTableColumn.ColumnAlias = shortColumnAlias;
			
			this.renamedTableColumns.Add (renamedTableColumn);

			this.shortAliasToColumnMap[shortColumnAlias] = originalTableColumn;
		}

		private void CreateSelectColumns(SqlSelect select)
		{
			foreach (DbTableColumn tableColumn in this.renamedTableColumns)
			{
				string   alias     = tableColumn.ColumnAlias;
				DbColumn column    = tableColumn.Column;
				string   tableName = tableColumn.TableAlias;

				SqlField fieldDefinition = SqlField.CreateAliasedName (tableName, column.GetSqlName (), alias);
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
	}
}
