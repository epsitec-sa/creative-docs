﻿//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
	public sealed class DbReader : System.IDisposable
	{
		public DbReader(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			this.longToShortTableAliasMap = new Dictionary<string, string> ();
			this.shortAliasToTableMap = new Dictionary<string, DbTable> ();
			this.shortAliasToColumnMap = new Dictionary<string, DbTableColumn> ();
			this.renamedTables = new List<DbTable> ();
			this.renamedTableColumns = new List<DbTableColumn>();
			this.conditions = new List<DbSelectCondition> ();
			this.orderByTableColumns = new Dictionary<string, SqlSortOrder> ();
		}


		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		public SqlSelectPredicate				SelectPredicate
		{
			get;
			set;
		}

		public bool								IncludeRowKeys
		{
			get;
			set;
		}

		
		public void AddQueryField(DbTableColumn queryField)
		{
			this.RegisterTableColumn (queryField);
		}

		public void AddQueryFields(IEnumerable<DbTableColumn> queryFields)
		{
			foreach (DbTableColumn queryField in queryFields)
			{
				this.AddQueryField (queryField);
			}
		}

		public void AddSortOrder(DbTableColumn tableColumn, SqlSortOrder order)
		{
			string shortColumnAlias = this.FindShortColumnAlias (tableColumn);

			if (shortColumnAlias == null)
			{
				throw new System.ArgumentException (string.Format ("Specified sort column '{0}' does not belong to the query fields", tableColumn.ToString ()));
			}

			DbTableColumn renamedColumn = this.FindRenamedTableColumn (shortColumnAlias);

			System.Diagnostics.Debug.Assert (renamedColumn != null);
			System.Diagnostics.Debug.Assert (this.orderByTableColumns.ContainsKey (shortColumnAlias) == false);

			//	Make sure the order of the columns is such that the sort order
			//	will indeed match what the caller expects :
			
			this.renamedTableColumns.Remove (renamedColumn);
			this.renamedTableColumns.Add (renamedColumn);

			this.orderByTableColumns[shortColumnAlias] = order;
		}

		public void AddCondition(DbSelectCondition condition)
		{
			foreach (DbTableColumn column in condition.Columns)
			{
				this.RegisterTable (column);
			}

			this.conditions.Add (condition);
		}


		public void CreateDataReader(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.dataReader == null);

			ISqlBuilder builder = transaction.SqlBuilder;
			SqlSelect select = this.CreateSelect ();

			select.Predicate = this.SelectPredicate;
			builder.SelectData (select);

			System.Data.IDbCommand command = builder.Command;
			command.Transaction = transaction.Transaction;
			System.Data.IDataReader reader = command.ExecuteReader ();

			this.dataReader = reader;
			this.dataReaderClosed = false;
		}

		public IEnumerable<RowData> Rows
		{
			get
			{
				if (this.dataReaderClosed)
				{
					throw new System.InvalidOperationException ("DataReader has been closed");
				}
				if (this.dataReader == null)
				{
					throw new System.InvalidOperationException ("Call to CreateDataReader is missing");
				}

				int n = this.renamedTableColumns.Count;
				int m = this.renamedTables.Count;

				int[] reordering = new int[n];
				object[] buffer  = new object[n+m];

				for (int i = 0; i < n; i++)
				{
					string columnNumber = this.renamedTableColumns[i].ColumnAlias.Substring (1);
					reordering[i] = int.Parse (columnNumber, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
				}

				while (this.dataReader.Read ())
				{
					object[] values = new object[n];
					DbKey[]  keys;

					this.dataReader.GetValues (buffer);

					for (int i = 0; i < n; i++)
					{
						values[reordering[i]] = buffer[i];
					}

					if (this.IncludeRowKeys)
					{
						keys = new DbKey[m];

						for (int i = 0; i < m; i++)
						{
							keys[i] = new DbKey (new DbId ((long) buffer[n+i]));
						}
					}
					else
					{
						keys = null;
					}

					yield return new RowData (values, keys);
				}

				this.CloseDataReader ();
			}
		}

		public IEnumerable<DbTable> Tables
		{
			get
			{
				return this.renamedTables;
			}
		}

		#region RowData Structure

		/// <summary>
		/// The <c>RowData</c> structure stores both the values and the keys
		/// for a given row.
		/// </summary>
		public struct RowData
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="RowData"/> structure.
			/// </summary>
			/// <param name="values">The values.</param>
			/// <param name="keys">The keys.</param>
			public RowData(object[] values, DbKey[] keys)
			{
				this.values = values;
				this.keys = keys;
			}

			/// <summary>
			/// Gets the values.
			/// </summary>
			/// <value>The values.</value>
			public object[] Values
			{
				get
				{
					return this.values;
				}
			}

			/// <summary>
			/// Gets the keys.
			/// </summary>
			/// <value>The keys.</value>
			public DbKey[] Keys
			{
				get
				{
					return this.keys;
				}
			}

			private readonly object[] values;
			private readonly DbKey[] keys;
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			this.CloseDataReader ();
		}

		#endregion

		private void CloseDataReader()
		{
			if ((this.dataReader != null) &&
				(this.dataReaderClosed == false))
			{
				this.dataReaderClosed = true;
				this.dataReader.Close ();
				this.dataReader.Dispose ();
				this.dataReader = null;
			}
		}

		private DbTableColumn FindRenamedTableColumn(string shortColumnAlias)
		{
			foreach (DbTableColumn tableColumn in this.renamedTableColumns)
			{
				if (tableColumn.ColumnAlias == shortColumnAlias)
				{
					return tableColumn;
				}
			}

			return null;
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
				this.renamedTables.Add (originalTableColumn.Table);
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

			if (this.IncludeRowKeys)
			{
				//	The caller expects us to generate behind his back the queries to
				//	also retrieve the ids of the table rows :

				int index = 0;

				foreach (DbTable table in this.renamedTables)
				{
					string alias     = string.Format (System.Globalization.CultureInfo.InvariantCulture, "I{0}", index);
					string tableName = string.Format (System.Globalization.CultureInfo.InvariantCulture, "T{0}", index);

					SqlField fieldDefinition = SqlField.CreateAliasedName (tableName, Tags.ColumnId, alias);

					select.Fields.Add (fieldDefinition);

					index++;
				}
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
			if (this.conditions.Count > 0)
			{
				foreach (DbSelectCondition condition in this.conditions)
				{
					condition.ReplaceTableColumns (this.TranslateToShortTableAlias);
					condition.CreateConditions (select.Conditions);
				}
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
		private readonly List<DbTable> renamedTables;
		private readonly List<DbTableColumn> renamedTableColumns;
		private readonly List<DbSelectCondition> conditions;
		private readonly Dictionary<string, SqlSortOrder> orderByTableColumns;
		private System.Data.IDataReader dataReader;
		private bool dataReaderClosed;
	}
}
