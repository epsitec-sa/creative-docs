//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;
using Epsitec.Cresus.Database.Collections;

using System.Linq;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRichCommand</c> class manages one or several commands which can
	/// be executed with <c>ISqlEngine</c>. It handles the select, update, insert
	/// and delete of information in data tables.
	/// </summary>
	public sealed class DbRichCommand : System.IDisposable, IReadOnly
	{


		public DbRichCommand(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			
			this.dataSet  = new System.Data.DataSet ();
			this.commands = new Collections.DbCommandList ();
			this.tables   = new Collections.DbTableList ();
			this.adapters = new List<System.Data.IDataAdapter> ();
			this.builders = new List<System.Data.Common.DbCommandBuilder> ();
			this.dataMappings = new Dictionary<string, DbDataTableMapping> ();
			this.relationSourceMappings = new Dictionary<string, DbDataTableMapping> ();
			this.relationTargetMappings = new Dictionary<string, DbDataTableMapping> ();
			this.fillDataSet = true;
		}

		/// <summary>
		/// Gets the individual commands associated with this rich command.
		/// </summary>
		/// <value>The commands.</value>
		public Collections.DbCommandList		Commands
		{
			get
			{
				return this.commands;
			}
		}

		/// <summary>
		/// Gets the individual table definitions associated with this rich command.
		/// </summary>
		/// <value>The table definitions.</value>
		public Collections.DbTableList			Tables
		{
			get
			{
				return this.tables;
			}
		}

		/// <summary>
		/// Gets the data set associated with this rich command.
		/// </summary>
		/// <value>The data set or <c>null</c> if there is no data set.</value>
		public System.Data.DataSet				DataSet
		{
			get
			{
				return this.dataSet;
			}
		}

		/// <summary>
		/// Gets the data table. This works only if there is exactly one data table.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Throws an invalid exception if there is more than one data table.</exception>
		/// <value>The data table or <c>null</c>.</value>
		public System.Data.DataTable			DataTable
		{
			get
			{
				System.Data.DataSet set = this.DataSet;
				
				if (set == null)
				{
					return null;
				}

				int count = set.Tables.Count;

				switch (count)
				{
					case 0:
						return null;

					case 1:
						return set.Tables[0];

					default:
						throw new System.InvalidOperationException ("Cannot return table - ambiguous data set");
				}
			}
		}

		/// <summary>
		/// Gets the infrastructure associated with this rich command.
		/// </summary>
		/// <value>The infrastructure.</value>
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		public bool								IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is read/write.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is read/write; otherwise, <c>false</c>.
		/// </value>
		public bool								IsReadWrite
		{
			get
			{
				return !this.isReadOnly;
			}
		}

		/// <summary>
		/// Gets the insert count for the replace statistics.
		/// </summary>
		/// <value>The insert count for the replace statistics.</value>
		public int								ReplaceStatisticsInsertCount
		{
			get
			{
				return this.statReplaceInsertCount;
			}
		}

		/// <summary>
		/// Gets the update count for the replace statistics.
		/// </summary>
		/// <value>The update count for the replace statistics.</value>
		public int								ReplaceStatisticsUpdateCount
		{
			get
			{
				return this.statReplaceUpdateCount;
			}
		}

		/// <summary>
		/// Gets the delete count for the replace statistics.
		/// </summary>
		/// <value>The delete count for the replace statistics.</value>
		public int								ReplaceStatisticsDeleteCount
		{
			get
			{
				return this.statReplaceDeleteCount;
			}
		}


		/// <summary>
		/// Creates a rich command from a table definition which can then be
		/// used to load the table into the data set.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, new DbTable[] { table }, new DbSelectCondition[] { null });
		}

		/// <summary>
		/// Creates a rich command from a table definition which can then be
		/// used to load a specific revision (all, live, copied, archive) of
		/// the data into the data set.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="selectRevision">The revision to select.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table, DbSelectRevision selectRevision)
		{
			return DbRichCommand.CreateFromTable (infrastructure, transaction, table, new DbSelectCondition (selectRevision));
		}

		/// <summary>
		/// Creates a rich command from a table definition which can then be
		/// used to load the table into the data set, using a specific selection.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="condition">The select condition or <c>null</c> to select nothing.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table, DbSelectCondition condition)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, new DbTable[] { table }, new DbSelectCondition[] { condition });
		}

		/// <summary>
		/// Creates a rich command from the definition of a collection of tables
		/// which can then be used to load the table into the data set.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tables">The table definitions.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, Collections.DbTableList tables)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, tables.ToArray ());
		}

		/// <summary>
		/// Creates a rich command from the definition of a collection of tables
		/// which can then be used to load the table into the data set.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tables">The table definitions.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, params DbTable[] tables)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, tables, new DbSelectCondition[tables.Length]);
		}

		/// <summary>
		/// Creates a rich command from the definition of a collection of tables
		/// which can then be used to load the table into the data set, using
		/// specific selections.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tables">The table definitions.</param>
		/// <param name="conditions">The select conditions (one for every table definition); use a
		/// <c>null</c> condition to select nothing for a given table.</param>
		/// <returns>The <c>DbRichCommand</c> instance.</returns>
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, DbTable[] tables, DbSelectCondition[] conditions)
		{
			if (tables.Length != conditions.Length)
			{
				throw new System.ArgumentException ("Mismatch of tables and conditions");
			}

			if (transaction == null)
			{
				try
				{
					transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
					return DbRichCommand.CreateFromTables (infrastructure, transaction, tables, conditions);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}

			DbRichCommand command = new DbRichCommand (infrastructure);

			int n = tables.Length;

			for (int i = 0; i < n; i++)
			{
				DbTable table = tables[i];
				DbSelectCondition condition = conditions[i];

				SqlSelect select = new SqlSelect ();
				ISqlBuilder builder = transaction.SqlBuilder;

				select.Fields.Add (SqlField.CreateAll ());
				select.Tables.Add (table.Name, SqlField.CreateName (table.GetSqlName ()));

				//	If there is no condition, this means we don't want to get any data
				//	for the specific table; just fetch the empty table by using an always
				//	false WHERE clause.

				if (condition == null)
				{
					select.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareFalse));
				}
				else
				{
					SqlField conditionSqlField = condition.CreateConditions (table, table.Name);

					if (conditionSqlField != null)
					{
						select.Conditions.Add (conditionSqlField);
					}
				}

				builder.SelectData (select);

				command.Commands.Add (builder.Command);
				command.Tables.Add (table);
			}

			//	Fetch the table contents into a data set :

			infrastructure.Execute (transaction, command);

			//	Relax the constraints imposed by ADO.NET based on the table schemas, so
			//	that we can fill the rows with partial data without getting exceptions :

			foreach (System.Data.DataTable table in command.DataSet.Tables)
			{
				DbRichCommand.RelaxConstraints (table);
			}

			return command;
		}

		/// <summary>
		/// Imports data into the specified table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table definition.</param>
		/// <param name="condition">The condition.</param>
		public void ImportTable(DbTransaction transaction, DbTable table, DbSelectCondition condition)
		{
			if (this.tables.Contains (table.Name))
			{
				DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table, condition);

				foreach (System.Data.DataTable data in command.DataSet.Tables)
				{
					this.ImportTable (table, data);
				}
			}
			else
			{
				SqlSelect select  = new SqlSelect ();
				ISqlBuilder builder = transaction.SqlBuilder;

				select.Fields.Add (SqlField.CreateAll ());
				select.Tables.Add (table.Name, SqlField.CreateName (table.GetSqlName ()));

				//	If there is no condition, this means we don't want to get any data
				//	for the specific table; just fetch the empty table by using an always
				//	false WHERE clause.

				bool oldFillDataSet = this.fillDataSet;

				if (condition == null)
				{
					this.fillDataSet = false;
					select.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareFalse));
				}
				else
				{
					SqlField conditionSqlField = condition.CreateConditions (table, table.Name);
					
					if (conditionSqlField != null)
					{
						select.Conditions.Add (conditionSqlField);
					}
				}

				this.infrastructure.DefaultSqlBuilder.SelectData (select);

				Collections.DbCommandList oldCommands = this.commands;
				Collections.DbCommandList newCommands = new Collections.DbCommandList ();

				Collections.DbTableList oldTables = this.tables;
				Collections.DbTableList newTables = new Collections.DbTableList ();

				var oldAdapters = this.adapters;
				var oldBuilders = this.builders;
				var newAdapters = new List<System.Data.IDataAdapter> ();
				var newBuilders = new List<System.Data.Common.DbCommandBuilder> ();

				this.commands = newCommands;
				this.tables   = newTables;
				this.adapters = newAdapters;
				this.builders = newBuilders;

				this.commands.Add (builder.Command);
				this.tables.Add (table);

				this.infrastructure.Execute (transaction, this);

				oldCommands.AddRange (newCommands);
				oldTables.AddRange (newTables);
				oldAdapters.AddRange (newAdapters);
				oldBuilders.AddRange (newBuilders);

				this.commands = oldCommands;
				this.tables   = oldTables;
				this.adapters = oldAdapters;
				this.builders = oldBuilders;
				this.fillDataSet = oldFillDataSet;

				DbRichCommand.RelaxConstraints (this.dataSet.Tables[this.dataSet.Tables.Count-1]);
			}
		}

		/// <summary>
		/// Imports data into the specified table, using a <see cref="System.Data.DataTable"/>
		/// as the source.
		/// </summary>
		/// <param name="tableDef">The table definition.</param>
		/// <param name="sourceTable">The table used as the data source.</param>
		private void ImportTable(DbTable tableDef, System.Data.DataTable sourceTable)
		{
			System.Data.DataTable table = this.DataSet.Tables[tableDef.Name];

			foreach (System.Data.DataRow row in sourceTable.Rows)
			{
				//	TODO: handle duplicate rows; what should be done (replace/update original/throw/...)

				table.ImportRow (row);
			}
		}

		/// <summary>
		/// Relaxes the data table constraints set up by ADO.NET so that we can
		/// fill the rows with partial data.
		/// </summary>
		/// <param name="table">The table.</param>
		internal static void RelaxConstraints(System.Data.DataTable table)
		{
			if (table.Columns.Contains (Tags.ColumnName))
			{
				if (table.Columns[Tags.ColumnId].Unique == false)
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Warning: Table {0} ID not unique, fixing.", table.TableName));
					table.Columns[Tags.ColumnId].Unique = true;
				}
			}
			
			//	If some columns are declared as non-nullable in the database, we have to
			//	remove set the AllowDBNull so that we can fill the table with partial
			//	rows without having ADO.NET complain about the broken constraint.
			
			foreach (System.Data.DataColumn column in table.Columns)
			{
				if (column.AllowDBNull == false)
				{
					column.AllowDBNull = true;
				}
			}
		}

		/// <summary>
		/// Locks the tables and make them read only. This prevents accidental calls
		/// to <c>UpdateTables</c> and <c>UpdateRealIds</c>.
		/// </summary>
		public void LockReadOnly()
		{
			this.isReadOnly = true;
		}
		
		/// <summary>
		/// Saves the tables by assigning real row ids to the rows, defining
		/// their log id and finally calling <c>UpdateTables</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void SaveTables(DbTransaction transaction)
		{
			this.SaveTables (transaction, null, null);
		}

		/// <summary>
		/// Saves the tables by assigning real row ids to the rows, defining
		/// their log id and finally calling <c>UpdateTables</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="callback">The callback.</param>
		public void SaveTables(DbTransaction transaction, System.Predicate<System.Data.DataTable> filter, RowIdAssignmentCallback callback)
		{
			this.UpdateLogIds ();
			this.AssignRealRowIds (transaction, filter, callback);
			this.UpdateTables (transaction);
		}

		/// <summary>
		/// Updates the tables by writing their contents to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void UpdateTables(DbTransaction transaction)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			//	Saves the data from the data set to the database, using either UPADTE or
			//	INSERT commands for each row. The DbRichCommand must have been filled with
			//	a call to Execute for this to work.
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.access);
			}
			
			this.CheckValidState ();
			this.AssertValidRowIds ();
			
			this.SetCommandTransaction (transaction);
			
			try
			{
				foreach (System.Data.IDataAdapter adapter in this.adapters)
				{
					adapter.Update (this.dataSet);
				}
			}
			finally
			{
				this.PopCommandTransaction ();
			}
		}
		
		/// <summary>
		/// Assign real row ids to the new data table rows.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void AssignRealRowIds(DbTransaction transaction)
		{
			this.AssignRealRowIds (transaction, null, null);
		}

		/// <summary>
		/// Assign real row ids to the new data table rows.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="callback">The callback.</param>
		public void AssignRealRowIds(DbTransaction transaction, System.Predicate<System.Data.DataTable> filter, RowIdAssignmentCallback callback)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.access);
			}
			
			this.CheckValidState ();
			this.SetCommandTransaction (transaction);
			
			try
			{
				foreach (System.Data.DataTable table in this.dataSet.Tables)
				{
					//	If there are temporary rows in the table, assign them real row ids, as
					//	we may not persist tables with temporary row ids :

					if ((filter == null) ||
						(filter (table)))
					{
						this.AssignTableRealRowIds (transaction, table, callback);
					}
				}
			}
			finally
			{
				this.PopCommandTransaction ();
			}
		}

		/// <summary>
		/// Updates the log ids associated with the data set, using the most
		/// current log id.
		/// </summary>
		public void UpdateLogIds()
		{
			this.UpdateLogIds (this.infrastructure.Logger.CurrentId);
		}

		/// <summary>
		/// Updates the log ids associated with the data set, using the specified
		/// log id.
		/// </summary>
		/// <param name="logId">The log id.</param>
		public void UpdateLogIds(DbId logId)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			foreach (System.Data.DataTable table in this.dataSet.Tables)
			{
				if (table.Columns.Contains (Tags.ColumnRefLog))
				{
					DbRichCommand.UpdateLogIds (table, logId);
				}
			}
		}

		/// <summary>
		/// Replaces the contents of the tables in the database by the contents
		/// of the data set. This is similar to <c>UpdateTables</c>, but overwriting
		/// the database contents even if the data changed since the call to
		/// <c>Execute</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void ReplaceTables(DbTransaction transaction)
		{
			this.ReplaceTables (transaction, null);
		}

		/// <summary>
		/// Replaces the contents of the tables in the database by the contents
		/// of the data set. This is similar to <c>UpdateTables</c>, but overwriting
		/// the database contents even if the data changed since the call to
		/// <c>Execute</c>.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="options">The replace options.</param>
		public void ReplaceTables(DbTransaction transaction, IReplaceOptions options)
		{
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.access);
			}
			
			this.CheckValidState ();
			this.AssertValidRowIds ();
			
			this.ReplaceTablesWithoutValidityChecking(transaction, options);
		}

		/// <summary>
		/// Replaces the table contents in the database without validity checking.
		/// This will effectively overwrite the data, even if it was changed by
		/// someone else in the meantime.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="options">The replace options or <c>null</c>.</param>
		public void ReplaceTablesWithoutValidityChecking(DbTransaction transaction, IReplaceOptions options)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			//	Don't touch the contents of the tables; writes them as is to the
			//	database, by using either UPDATE or INSERT.
			
			for (int i = 0; i < this.adapters.Count; i++)
			{
				System.Data.DataTable table = this.dataSet.Tables[i];
				
				int changeCount = 0;
				int deleteCount = 0;
				
				for (int r = 0; r < table.Rows.Count; r++)
				{
					System.Data.DataRow row = table.Rows[r];
					
					switch (row.RowState)
					{
						case System.Data.DataRowState.Added:
						case System.Data.DataRowState.Modified:
							changeCount++;
							break;
						
						case System.Data.DataRowState.Deleted:
							deleteCount++;
							break;
					}
				}
				
				if ((changeCount > 0) ||
					(deleteCount > 0))
				{
					this.ReplaceTablesWithoutValidityChecking (transaction, table, this.Tables[i], options);
				}

				this.AcceptChanges (transaction, table.TableName);
			}
		}

		/// <summary>
		/// Creates a new row in the specified table. The row id will be set to a new
		/// temporary row id.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The new row.</returns>
		public System.Data.DataRow CreateRow(string tableName)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			this.CheckValidState ();
			
			System.Data.DataTable table = this.dataSet.Tables[tableName];
			
			if (table == null)
			{
				throw new System.ArgumentException (string.Format ("Table {0} not found.", tableName), "tableName");
			}

			System.Data.DataRow row = DbRichCommand.CreateRow (table);
			
			table.Rows.Add (row);

			return row;
		}

		/// <summary>
		/// Finds the row in the specified table.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="rowId">The row id.</param>
		/// <returns>
		/// The row or <c>null</c> if it cannot be found in the table.
		/// </returns>
		public System.Data.DataRow FindRow(string tableName, DbId rowId)
		{
			System.Data.DataTable table = this.DataSet.Tables[tableName];
			System.Data.DataRow row = null;

			if (table != null && table.Rows.Count > 0)
			{
				if (!this.dataMappings.ContainsKey (tableName))
				{
					this.dataMappings[tableName] = new DbDataTableMapping (table, Tags.ColumnId);
				}

				DbDataTableMapping tableCache = this.dataMappings[tableName];

				if (tableCache.Contains (rowId.Value))
				{
					row = tableCache.GetRow (rowId.Value);
				}
			}

			return row;
		}

		public IEnumerable<System.Data.DataRow> FindRelationRows(string tableName, DbId sourceRowId)
		{
			List<System.Data.DataRow> rows = new List<System.Data.DataRow> ();
			System.Data.DataTable table = this.DataSet.Tables[tableName];

			if (table != null && table.Rows.Count > 0)
			{
				if (!this.relationSourceMappings.ContainsKey (tableName))
				{
					this.relationSourceMappings[tableName] = new DbDataTableMapping (table, Tags.ColumnRefSourceId);
				}

				DbDataTableMapping tableCache = this.relationSourceMappings[tableName];

				if (tableCache.Contains (sourceRowId.Value))
				{
					rows = tableCache.GetRows (sourceRowId.Value);
				}
			}

			return rows;
		}

		public static IEnumerable<System.Data.DataRow> FilterExistingRows(IEnumerable<System.Data.DataRow> collection)
		{
			foreach (System.Data.DataRow row in collection)
			{
				switch (row.RowState)
				{
					case System.Data.DataRowState.Deleted:
					case System.Data.DataRowState.Detached:
						break;
					
					case System.Data.DataRowState.Added:
					case System.Data.DataRowState.Modified:
					case System.Data.DataRowState.Unchanged:
						yield return row;
						break;

					default:
						throw new System.NotImplementedException ();
				}
			}
		}

		/// <summary>
		/// Deletes an existing row.
		/// </summary>
		/// <param name="row">The row to delete.</param>
		public void DeleteExistingRow(System.Data.DataRow row)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}
			
			DbRichCommand.DeleteRow (row);
		}

		/// <summary>
		/// Makes the specified row immutable. This creates a new row, if needed,
		/// which can then be further modified. This method has no effect if the
		/// table does not use <c>DbRevisionMode.Enabled</c>.
		/// </summary>
		/// <param name="row">The original row.</param>
		/// <returns>The row which replaces the original row.</returns>
		public System.Data.DataRow MakeRowImmutable(System.Data.DataRow row)
		{
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}

			DbTable table = this.tables[row.Table.TableName];

			if (table.RevisionMode == DbRevisionMode.TrackChanges)
			{
				DbRowStatus status = DbKey.GetRowStatus (row);

				switch (status)
				{
					case DbRowStatus.ArchiveCopy:
					case DbRowStatus.Copied:
						break;
					
					case DbRowStatus.Deleted:
						System.Diagnostics.Debug.WriteLine ("Deleted row is considered as an immutable row");
						break;
					
					case DbRowStatus.Live:
						DbRichCommand.ArchiveRowAndCreateNewCopy (ref row);
						break;
					
					default:
						throw new System.NotSupportedException (string.Format ("Status {0} not supported", status));
				}
			}

			return row;
		}

		public void DefineRowKey(System.Data.DataRow row, DbKey newKey)
		{
			DbKey oldKey = new DbKey (row);

			row.BeginEdit ();
			newKey.SetRowKey (row);
			row.EndEdit ();

			if ((oldKey.IsTemporary) &&
				(oldKey.Id != newKey.Id))
			{
				//	Search the relation tables in order to update them, if they
				//	happen to refer to this row.

				string rowTableName = row.Table.TableName;

				foreach (System.Data.DataTable relationTable in this.EnumerateRelationTables ())
				{
					DbTable relationTableDef = this.tables[relationTable.TableName];

					string sourceTableName = relationTableDef.RelationSourceTableName;
					string targetTableName = relationTableDef.RelationTargetTableName;

					System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (sourceTableName));
					System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (targetTableName));

					if (sourceTableName == rowTableName)
					{
						this.UpdateRelation (relationTable, Tags.ColumnRefSourceId, oldKey.Id, newKey.Id);
					}
					if (targetTableName == rowTableName)
					{
						this.UpdateRelation (relationTable, Tags.ColumnRefTargetId, oldKey.Id, newKey.Id);
					}
				}
			}
		}

		private void UpdateRelation(System.Data.DataTable relationTable, string columnName, DbId oldId, DbId newId)
		{
			long id = oldId.Value;

			Dictionary<string, DbDataTableMapping> mappings;

			if (columnName == Tags.ColumnRefSourceId)
			{
				mappings = this.relationSourceMappings;
			}
			else if (columnName == Tags.ColumnRefTargetId)
			{
				mappings = this.relationTargetMappings;
			}
			else
			{
				throw new System.ArgumentException ("Invalid columnName: " + columnName);
			}

			if (!mappings.ContainsKey (relationTable.TableName))
			{
				mappings[relationTable.TableName] = new DbDataTableMapping (relationTable, columnName);
			}

			DbDataTableMapping mapping = mappings[relationTable.TableName];

			List<System.Data.DataRow> dataRows = new List<System.Data.DataRow> ();

			if (mapping.Contains (id))
			{
				dataRows.AddRange (mapping.GetRows (id));
			}

			foreach (System.Data.DataRow dataRow in dataRows)
			{
				dataRow.BeginEdit ();
				dataRow[columnName] = newId.Value;
				dataRow.EndEdit ();
			}
		}

		private IEnumerable<System.Data.DataTable> EnumerateRelationTables()
		{
			foreach (System.Data.DataTable table in this.dataSet.Tables)
			{
				if ((table.Columns.Contains (Tags.ColumnRefSourceId)) &&
					(table.Columns.Contains (Tags.ColumnRefTargetId)))
				{
					yield return table;
				}
			}
		}


		/// <summary>
		/// Accepts the changes for all the tables attached to this instance.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void AcceptChanges(DbTransaction transaction)
		{
			if (transaction == null)
			{
				throw new System.ArgumentNullException ("No transaction specified");
			}
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}

			//	TODO: AcceptChanges should only be called when the transaction is committed successfully.

			this.dataSet.AcceptChanges ();
		}

		/// <summary>
		/// Accepts the changes for the specified table attached to this instance.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		public void AcceptChanges(DbTransaction transaction, string tableName)
		{
			if (transaction == null)
			{
				throw new System.ArgumentNullException ("No transaction specified");
			}
			if (this.isReadOnly)
			{
				throw new Exceptions.ReadOnlyException (this.access);
			}

			//	TODO: AcceptChanges should only be called when the transaction is committed successfully.

			this.dataSet.Tables[tableName].AcceptChanges ();
		}

		/// <summary>
		/// Checks all the row ids to make sure that none contains temporary row ids.
		/// </summary>
		public void AssertValidRowIds()
		{
			foreach (System.Data.DataTable table in this.dataSet.Tables)
			{
				DbRichCommand.AssertValidRowIds (table);
			}
		}

		/// <summary>
		/// Gets the active transaction.
		/// </summary>
		/// <returns>The active transaction or <c>null</c> if no transaction is currently active.</returns>
		public DbTransaction GetActiveTransaction()
		{
			if (this.activeTransactions.Count == 0)
			{
				return null;
			}
			else
			{
				return this.activeTransactions.Peek ();
			}
		}

		/// <summary>
		/// Fills the data set. This method may only be called by the <c>DbInfrastructure</c>
		/// class Use the <c>DbInfrastructure.Execute</c> method instead.
		/// </summary>
		/// <param name="access">The access.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="adapters">The adapters.</param>
		/// <param name="builders">The builders.</param>
		public void InternalFillDataSet(DbAccess access, DbTransaction transaction, System.Data.IDbDataAdapter[] adapters, System.Data.Common.DbCommandBuilder[] builders)
		{
			System.Diagnostics.Debug.Assert (access.IsValid);
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (access);
			}

			if (this.access.IsEmpty)
			{
				this.access = access;
			}
			
			//	Fill the data set based on the adapter objects provided by
			//	the caller :

			this.adapters.AddRange (adapters);
			this.builders.AddRange (builders);
			
			this.SetCommandTransaction (transaction);
			
			try
			{
				for (int i = 0; i < this.tables.Count; i++)
				{
					DbTable tableDef = this.tables[i];
					
					string  adoNameTable = "Table";
					string  dbNameTable  = tableDef.Name;
					
					//	Ensure that the column and table names match what we expect,
					//	based on the DbColumn and DbTable names.
					
					System.Data.ITableMapping mapping = this.adapters[i].TableMappings.Add (adoNameTable, dbNameTable);
					
					foreach (DbColumn columnDef in tableDef.Columns)
					{
						if (columnDef.Cardinality == DbCardinality.None)
						{
							if (columnDef.Localization == DbColumnLocalization.Localized)
							{
								foreach (string localizationSuffix in tableDef.Localizations)
								{
									string dbNameColumn  = columnDef.MakeLocalizedName (localizationSuffix);
									string adoNameColumn = columnDef.MakeLocalizedSqlName (localizationSuffix);

									mapping.ColumnMappings.Add (adoNameColumn, dbNameColumn);
								}
							}
							else
							{
								string dbNameColumn  = columnDef.Name;
								string adoNameColumn = columnDef.GetSqlName ();

								mapping.ColumnMappings.Add (adoNameColumn, dbNameColumn);
							}
						}
					}

					if (this.infrastructure.SchemasCache.ContainsKey (tableDef))
					{
						this.dataSet.Tables.Add (this.infrastructure.SchemasCache[tableDef].Clone ());
					}
					else
					{
						this.adapters[i].FillSchema (this.dataSet, System.Data.SchemaType.Mapped);
						this.infrastructure.SchemasCache[tableDef] = this.dataSet.Tables[tableDef.Name].Clone ();
					}
					
					this.adapters[i].MissingSchemaAction = System.Data.MissingSchemaAction.Error;
					
					if (this.fillDataSet)
					{
						this.adapters[i].Fill (this.dataSet);
					}
				}
			}
			finally
			{
				this.PopCommandTransaction ();
			}
			
			this.CreateDataRelations ();
		}

		#region IDisposable Members

		/// <summary>
		/// Releases the data set and the associated commands.
		/// </summary>
		public void Dispose()
		{
			System.Diagnostics.Debug.Assert (this.activeTransactions.Count == 0);
			
			if (this.dataSet != null)
			{
				this.dataSet.Dispose ();
				this.dataSet = null;
			}

			foreach (System.Data.IDbCommand command in this.commands.ToArray ())
			{
				command.Dispose ();
			}

			this.commands.Clear ();
			this.tables.Clear ();

			this.infrastructure = null;
			this.adapters = null;
			this.builders = null;
		}
		
		#endregion

		/// <summary>
		/// Extracts the live rows from a given collection.
		/// </summary>
		/// <param name="rows">A collection of rows.</param>
		/// <returns>The live rows.</returns>
		public static IEnumerable<System.Data.DataRow> GetLiveRows(System.Collections.IEnumerable rows)
		{
			foreach (System.Data.DataRow row in rows)
			{
				if (DbRichCommand.IsRowLive (row))
				{
					yield return row;
				}
			}
		}

		/// <summary>
		/// Determines whether the specified row is live.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified row is live; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRowLive(System.Data.DataRow row)
		{
			return !DbRichCommand.IsRowDeleted (row);
		}

		/// <summary>
		/// Determines whether the specified row is deleted.
		/// </summary>
		/// <param name="row">The row to check.</param>
		/// <returns>
		/// 	<c>true</c> if the specified row is deleted; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRowDeleted(System.Data.DataRow row)
		{
			if (row.RowState == System.Data.DataRowState.Deleted)
			{
				return true;
			}
			
			DbKey key = new DbKey (row);

			return (key.Status == DbRowStatus.Deleted);
		}

		/// <summary>
		/// Checks all the row ids for the given table to make sure that none contains
		/// temporary row ids.
		/// </summary>
		/// <param name="table">The table.</param>
		public static void AssertValidRowIds(System.Data.DataTable table)
		{
			if (table.Columns.Contains (Tags.ColumnId))
			{
				foreach (System.Data.DataRow row in table.Rows)
				{
					if (row.RowState != System.Data.DataRowState.Deleted)
					{
						DbKey key = new DbKey (row);

						System.Diagnostics.Debug.Assert (key.IsTemporary == false);
						System.Diagnostics.Debug.Assert (key.Id.ClientId != 0);
					}
				}
			}
			if ((table.Columns.Contains (Tags.ColumnRefSourceId)) &&
				(table.Columns.Contains (Tags.ColumnRefTargetId)))
			{
				foreach (System.Data.DataRow row in table.Rows)
				{
					if (row.RowState != System.Data.DataRowState.Deleted)
					{
						DbKey sourceKey = new DbKey (new DbId ((long) row[Tags.ColumnRefSourceId]));
						DbKey targetKey = new DbKey (new DbId ((long) row[Tags.ColumnRefTargetId]));

						System.Diagnostics.Debug.Assert (sourceKey.IsTemporary == false);
						System.Diagnostics.Debug.Assert (sourceKey.Id.ClientId != 0);
						System.Diagnostics.Debug.Assert (targetKey.IsTemporary == false);
						System.Diagnostics.Debug.Assert (targetKey.Id.ClientId != 0);
					}
				}
			}
		}

		/// <summary>
		/// Assign real row ids to the new data table rows for a given table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		/// <param name="callback">The callback.</param>
		private void AssignTableRealRowIds(DbTransaction transaction, System.Data.DataTable table, RowIdAssignmentCallback callback)
		{
			List<System.Data.DataRow> list = Collection.ToList<System.Data.DataRow> (DbRichCommand.FindRowsUsingTemporaryIds (table.Rows));
			
			if (list.Count == 0)
			{
				return;
			}
			
			DbTable tableDef = this.infrastructure.ResolveDbTable (transaction, table.TableName);
			
			//	Allocate real row ids for the temporary rows; thanks to the relations
			//	defined at the data set level, the foreign keys will be updated too.
			
			long id = this.infrastructure.NewRowIdInTable (transaction, tableDef, list.Count);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Allocating {0} new IDs for table {1} starting at {2}.", list.Count, table.TableName, id));
			
			foreach (System.Data.DataRow row in list)
			{
				DbKey oldKey = new DbKey (row);
				DbKey newKey = new DbKey (id++, oldKey.Status);

				if (callback != null)
				{
					newKey = callback (tableDef, table, oldKey, newKey);

					if (newKey.IsEmpty)
					{
						continue;
					}
				}

				System.Diagnostics.Debug.Assert (oldKey.IsTemporary == true);
				System.Diagnostics.Debug.Assert (newKey.IsTemporary == false);
				
				row.BeginEdit ();
				newKey.SetRowKey (row);
				row.EndEdit ();
			}
		}

		public delegate DbKey RowIdAssignmentCallback(DbTable tableDefinition, System.Data.DataTable table, DbKey oldKey, DbKey newKey);

		/// <summary>
		/// Updates the log ids associated with the table, using the specified log id.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="logId">The log id.</param>
		public static void UpdateLogIds(System.Data.DataTable table, DbId logId)
		{
			foreach (System.Data.DataRow row in table.Rows)
			{
				switch (row.RowState)
				{
					case System.Data.DataRowState.Added:
						row.BeginEdit ();
						DbRichCommand.DefineLogId (row, logId);
						row.EndEdit ();
						break;
					
					case System.Data.DataRowState.Modified:
						row.BeginEdit ();
						DbRichCommand.DefineLogId (row, logId);
						DbRichCommand.UpdateRowStatusAfterModification (row);
						row.EndEdit ();
						break;
				}
			}
		}


		public void Update(DbTransaction transaction, DbTable table, SqlFieldList fields, SqlFieldList conditions)
		{
			// TODO Use the data sets so that we don't mess up the synchronization process.
			
			transaction.SqlBuilder.Clear ();

			transaction.SqlBuilder.UpdateData (table.GetSqlName (), fields, conditions);

			System.Data.IDbCommand command = transaction.SqlBuilder.Command;
			command.Transaction = transaction.Transaction;
			command.ExecuteNonQuery ();

			if (table.Columns.Contains (Tags.ColumnRefLog))
			{
				SqlField logField = fields.SingleOrDefault (f => f.AsName == Tags.ColumnRefLog);
				long logValue = this.infrastructure.Logger.CurrentId.Value;

				if (logField != null)
				{
					logField.Overwrite (SqlField.CreateConstant (logValue, DbKey.RawTypeForId));
					logField.Alias = table.Columns[Tags.ColumnRefLog].GetSqlName ();
				}
				else
				{
					logField = SqlField.CreateConstant (logValue, DbKey.RawTypeForId);
					string alias = table.Columns[Tags.ColumnRefLog].GetSqlName ();

					fields.Add (alias, logField);
				}
			}

			transaction.SqlBuilder.Clear ();
		}


		/// <summary>
		/// Defines the log id for the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="logId">The log id.</param>
		private static void DefineLogId(System.Data.DataRow row, DbId logId)
		{
			System.Diagnostics.Debug.Assert (row.RowState != System.Data.DataRowState.Deleted);
			
			row[Tags.ColumnRefLog] = logId.Value;
		}

		/// <summary>
		/// Updates the row status after a modification. This will change the status
		/// from <c>DbRowStatus.Copied</c> to <c>DbRowStatus.Live</c>.
		/// </summary>
		/// <param name="row">The row.</param>
		private static void UpdateRowStatusAfterModification(System.Data.DataRow row)
		{
			DbRowStatus status = DbKey.GetRowStatus (row);

			switch (status)
			{
				case DbRowStatus.Copied:
					DbKey.SetRowStatus (row, DbRowStatus.Live);
					break;

				case DbRowStatus.Live:
					break;

				case DbRowStatus.Deleted:
					System.Diagnostics.Debug.WriteLine ("WARNING: modifying deleted row");
					break;

				case DbRowStatus.ArchiveCopy:
					System.Diagnostics.Debug.WriteLine ("WARNING: modifying archived row");
					break;

				default:
					throw new System.NotSupportedException (string.Format ("Status {0} not supported", status));
			}
		}


		/// <summary>
		/// Creates a new row in the specified table. The row id will be set to a new
		/// temporary row id.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <returns>The row.</returns>
		public static System.Data.DataRow CreateRow(System.Data.DataTable table)
		{
			//	Crée une ligne, mais ne l'ajoute pas à la table. L'ID affecté à la
			//	ligne est temporaire (mais unique); cf. DbKey.CheckTemporaryId.

			System.Data.DataRow row = table.NewRow ();

			if (table.Columns.Contains (Tags.ColumnId))
			{
				DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);

				row.BeginEdit ();
				key.SetRowKey (row);
				row.EndEdit ();
			}

			return row;
		}

		/// <summary>
		/// Creates a new row in the specified table. The row id will be set to a new
		/// temporary row id and it will be associated with the specified log id.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="logId">The log id.</param>
		/// <returns>The row.</returns>
		public static System.Data.DataRow CreateRow(System.Data.DataTable table, DbId logId)
		{
			System.Data.DataRow row = DbRichCommand.CreateRow (table);
			DbRichCommand.DefineLogId (row, logId);
			return row;
		}

		/// <summary>
		/// Deletes the specified row. Rows which already exist in the database are simply
		/// marked as deleted, but never really removed from their data table.
		/// </summary>
		/// <param name="row">The row.</param>
		public static void DeleteRow(System.Data.DataRow row)
		{
			DbKey rowKey = new DbKey (row);
			
			//	If the row still has a temporary key associated with it, then this
			//	means we can safely discard it as it has not yet been persisted to
			//	the database.
			
			if (rowKey.IsTemporary)
			{
				System.Data.DataTable table = row.Table;
				table.Rows.Remove (row);
			}
			else
			{
				row.BeginEdit ();
				row[Tags.ColumnStatus] = DbKey.ConvertToIntStatus (DbRowStatus.Deleted);
				row.EndEdit ();
			}
		}

		/// <summary>
		/// Kills the specified row. This really removes the row from its data
		/// table. Prefer <c>DeleteRow</c> which is non-destructive.
		/// </summary>
		/// <param name="row">The row.</param>
		public static void KillRow(System.Data.DataRow row)
		{
			//	This really deletes the row from its table; use this only where
			//	needed, as in the request queue, for instance.
			
			if (row.RowState != System.Data.DataRowState.Deleted)
			{
				row.Delete ();
			}
		}

		/// <summary>
		/// Creates a copy of the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The copy of the row or <c>null</c> if the specified row is not valid.</returns>
		public static System.Data.DataRow CopyRowIfValid(System.Data.DataRow row)
		{
			if ((row == null) ||
				(row.Table == null))
			{
				return null;
			}
			
			switch (row.RowState)
			{
				case System.Data.DataRowState.Deleted:
				case System.Data.DataRowState.Detached:
					return null;
			}
			
			System.Data.DataRow copy = row.Table.NewRow ();
			
			copy.ItemArray = row.ItemArray;
			
			return copy;
		}

		/// <summary>
		/// Archives the specified live row and creates new working copy.
		/// </summary>
		/// <param name="live">The live row.</param>
		private static void ArchiveRowAndCreateNewCopy(ref System.Data.DataRow live)
		{
			System.Diagnostics.Debug.Assert (DbKey.GetRowStatus (live) == DbRowStatus.Live);

			System.Data.DataTable table   = live.Table;
			System.Data.DataRow   archive = live;
			System.Data.DataRow   copy    = table.NewRow ();

			live = null;
			copy.ItemArray = archive.ItemArray;
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Copied);

			key.SetRowKey (copy);
			
			DbKey.SetRowStatus (archive, DbRowStatus.ArchiveCopy);
			
			table.Rows.Add (copy);

			live = copy;
		}



		/// <summary>
		/// Finds a row in a data table based on a row id.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="rowId">The row id.</param>
		/// <returns>
		/// The row or <c>null</c> if it cannot be found in the table.
		/// </returns>
		public static System.Data.DataRow FindRow(System.Data.DataTable table, DbId rowId)
		{
			return DbRichCommand.FindRow (table, table.Rows, rowId);
		}

		/// <summary>
		/// Finds the row in a collection of rows based on a row id.
		/// </summary>
		/// <param name="table">The table which defines the schema for the rows.</param>
		/// <param name="rows">The rows.</param>
		/// <param name="rowId">The row id.</param>
		/// <returns>
		/// The row or <c>null</c> if it cannot be found in the collection.
		/// </returns>
		public static System.Data.DataRow FindRow(System.Data.DataTable table, System.Collections.IEnumerable rows, DbId rowId)
		{
			long rowIdValue = rowId.Value;
			return DbRichCommand.IterateRows (table, rows, (row, id) => rowIdValue == id);
		}


		private static System.Data.DataRow IterateRows(System.Data.DataTable table, System.Collections.IEnumerable rows, System.Func<System.Data.DataRow, long, bool> action)
		{
			int columnIdIndex = table.Columns.IndexOf (Tags.ColumnId);

			foreach (System.Data.DataRow row in rows)
			{
				long rowIdValue;

				switch (row.RowState)
				{
					case System.Data.DataRowState.Deleted:
						rowIdValue = (long) row[columnIdIndex, System.Data.DataRowVersion.Original];
						break;

					case System.Data.DataRowState.Detached:
						continue;

					default:
						rowIdValue = (long) row[columnIdIndex];
						break;
				}

				if (action (row, rowIdValue))
				{
					return row;
				}
			}
			
			return null;
		}


		/// <summary>
		/// Finds the temporary rows in the collection.
		/// </summary>
		/// <param name="rows">The rows.</param>
		/// <returns>A collection of temporary rows.</returns>
		public static IEnumerable<System.Data.DataRow> FindRowsUsingTemporaryIds(System.Collections.IEnumerable rows)
		{
			foreach (System.Data.DataRow row in rows)
			{
				if (row.RowState != System.Data.DataRowState.Deleted)
				{
					DbKey key = new DbKey (row);
					
					if (key.IsTemporary)
					{
						yield return row;
					}
				}
			}
		}


		/// <summary>
		/// For debugging: dumps the specified command to the debug trace,
		/// including its parameter names and values.
		/// </summary>
		/// <param name="command">The command.</param>
		[System.Diagnostics.Conditional ("DEBUG")]
		public static void DebugDumpCommand(System.Data.IDbCommand command)
		{
			System.Diagnostics.Debug.WriteLine (command.CommandText);
			
			foreach (System.Data.IDataParameter commandParameter in command.Parameters)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0} = {1}, type {2}", commandParameter.ParameterName, commandParameter.Value, commandParameter.Value.GetType ().FullName));
			}
		}

		/// <summary>
		/// For debugging: dumps the specified row to the debug trace.
		/// </summary>
		/// <param name="row">The row.</param>
		[System.Diagnostics.Conditional ("DEBUG")]
		public static void DebugDumpRow(System.Data.DataRow row)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (object o in row.ItemArray)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (", ");
				}
				
				buffer.Append ("'");
				buffer.Append (o.ToString ());
				buffer.Append ("'");
			}
			
			System.Diagnostics.Debug.WriteLine (buffer.ToString ());
		}


		/// <summary>
		/// Creates the data relations between the columns of the data set
		/// by exploiting the properties of the <c>DbTable</c> and <c>DbColumn</c>
		/// definitions.
		/// </summary>
		private void CreateDataRelations()
		{
			foreach (DbTable dbChildTable in this.tables)
			{
				System.Data.DataTable adoChildTable = this.dataSet.Tables[dbChildTable.Name];
				
				foreach (DbForeignKey fk in dbChildTable.ForeignKeys)
				{
					System.Data.DataTable adoTargetTable = this.dataSet.Tables[fk.TargetTableName];

					if (adoTargetTable == null)
					{
						//	If the target table is not available in the data set, simply
						//	ignore the relation.
						
						continue;
					}

					int n = fk.Columns.Length;
					
					System.Data.DataColumn[] adoTargetCols = new System.Data.DataColumn[n];
					System.Data.DataColumn[] adoChildCols  = new System.Data.DataColumn[n];
					
					for (int i = 0; i < n; i++)
					{
						adoChildCols[i]  = adoChildTable.Columns[fk.Columns[i].Name];
						adoTargetCols[i] = adoTargetTable.Columns[fk.Columns[i].TargetColumnName];
					}

					System.Data.DataRelation relation = new System.Data.DataRelation (null, adoTargetCols, adoChildCols);
					this.dataSet.Relations.Add (relation);
					
					System.Data.ForeignKeyConstraint constraint = relation.ChildKeyConstraint;
					
					System.Diagnostics.Debug.Assert (constraint != null);
					System.Diagnostics.Debug.Assert (constraint.UpdateRule == System.Data.Rule.Cascade);
				}
			}
		}

		/// <summary>
		/// Sets the active command transaction and pushes it on top of the
		/// active transaction stack.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		private void SetCommandTransaction(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (transaction != null);
			System.Diagnostics.Debug.Assert (transaction.Transaction != null);
			
			System.Data.IDbTransaction dataTransaction = transaction.Transaction;

			this.activeTransactions.Push (transaction);

			try
			{
				foreach (var command in this.commands)
				{
					command.Transaction = dataTransaction;
				}
				foreach (var command in this.GetCommandBuilderCommands ())
				{
					command.Transaction = dataTransaction;
				}
			}
			catch
			{
				this.activeTransactions.Pop ();	//	TODO: check that this is OK if the exception occurred while setting up the this.commands transactions (since we will try to create commands in a builder where the original commands are not tied to active transactions
				throw;
			}
		}

		/// <summary>
		/// Pops the active command transaction and resets all command transactions
		/// to <c>null</c>.
		/// </summary>
		private void PopCommandTransaction()
		{
			this.activeTransactions.Pop ();

			foreach (var command in this.commands)
			{
				command.Transaction = null;
			}
			foreach (var command in this.GetCommandBuilderCommands ())
			{
				command.Transaction = null;
			}
		}

		/// <summary>
		/// Gets the command builder commands. This may only be called once the original
		/// select commands have live transactions associated to them.
		/// </summary>
		/// <returns>The update, insert and delete commands produced by the command builders.</returns>
		private IEnumerable<System.Data.IDbCommand> GetCommandBuilderCommands()
		{
			foreach (var builder in this.builders)
			{
				yield return builder.GetUpdateCommand ();
				yield return builder.GetInsertCommand ();
				yield return builder.GetDeleteCommand ();
			}
		}

		/// <summary>
		/// Checks the validity of the <c>DbRichCommand</c> state. If some
		/// internal state is not valid, throws an exception.
		/// </summary>
		private void CheckValidState()
		{
			if (this.access.IsValid == false)
			{
				throw new System.InvalidOperationException ("No database access defined.");
			}
			
			if (this.dataSet == null)
			{
				throw new Exceptions.GenericException (this.access, "No data set defined.");
			}
			
			if ((this.adapters == null) ||
				(this.adapters.Count == 0))
			{
				throw new Exceptions.GenericException (this.access, "No adapters defined.");
			}

			if ((this.builders == null) ||
				(this.builders.Count == 0))
			{
				throw new Exceptions.GenericException (this.access, "No builders defined.");
			}

			if (this.commands.Count == 0)
			{
				throw new Exceptions.GenericException (this.access, "No commands defined.");
			}
			
			if (this.tables.Count == 0)
			{
				throw new Exceptions.GenericException (this.access, "No tables defined.");
			}
			
			if (this.infrastructure == null)
			{
				throw new Exceptions.GenericException (this.access, "No infrastructure defined.");
			}
		}

		/// <summary>
		/// Replaces the table contents in the database without validity checking.
		/// This will effectively overwrite the data, even if it was changed by
		/// someone else in the meantime.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="dataTable">The data table.</param>
		/// <param name="tableDef">The table definition.</param>
		/// <param name="options">The replace options.</param>
		private void ReplaceTablesWithoutValidityChecking(DbTransaction transaction, System.Data.DataTable dataTable, DbTable tableDef, IReplaceOptions options)
		{
			string sqlTableName = tableDef.GetSqlName ();
			
			IDbAbstraction database  = transaction.Database;
			ISqlBuilder    builder   = database.SqlBuilder;
			ITypeConverter converter = this.infrastructure.Converter;
			
			var sqlUpdate = new Collections.SqlFieldList ();
			var sqlInsert = new Collections.SqlFieldList ();
			var sqlConds  = new Collections.SqlFieldList ();
			
			int colCount = tableDef.GetSqlColumnCount ();
			
			//	For every row in the table, create an SQL representation of it, including the
			//	fields to store the data.
			
			SqlColumn[] sqlColumns = new SqlColumn[colCount];
			
			int[]    updateParamIndex = new int[colCount];
			int[]    insertParamIndex = new int[colCount];
			object[] insertDefault    = new object[colCount];
			
			int index = 0;

			//	Create the SQL columns and decide how to map the row items to
			//	the SQL columns. This must take into account the fact that the
			//	DbTable may produce several low level SQL columns for every
			//	single high level DbColumn...
			
			foreach (DbColumn column in tableDef.Columns)
			{
				if (column.Cardinality != DbCardinality.None)
				{
					//	Skip columns which do not exist in the database; this is
					//	the case for relation columns, which are in fact implemented
					//	using an extra relation table.

					continue;
				}

				bool ignoreColumn = (options != null) && options.ShouldIgnoreColumn (column);
				
				foreach (SqlColumn sqlColumn in tableDef.CreateSqlColumns (converter, column))
				{
					sqlColumns[index] = sqlColumn;

					if (ignoreColumn)
					{
						//	Ignore this column when using the UPDATE command, but still
						//	provide a default value for the INSERT command.
						
						updateParamIndex[index] = -1;
						insertParamIndex[index] = sqlInsert.Count;
						insertDefault[index]    = options.GetDefaultValue (column);

						sqlInsert.Add (this.infrastructure.CreateEmptySqlField (column));
					}
					else
					{
						//	The column is to be included by the UPDATE command. Remember
						//	the parameter index for the specified column and don't provide
						//	a default value for the INSERT command.

						updateParamIndex[index] = sqlUpdate.Count;
						insertParamIndex[index] = sqlInsert.Count;
						insertDefault[index]    = null;

						sqlUpdate.Add (this.infrastructure.CreateEmptySqlField (column));
						sqlInsert.Add (this.infrastructure.CreateEmptySqlField (column));
					}

					index++;
				}
			}
			
			//	Create the condition for the UPDATE .. WHERE or DELETE .. WHERE clause :
			
			SqlField fieldIdName  = SqlField.CreateName (sqlTableName, sqlColumns[0].Name);
			SqlField fieldIdValue = sqlUpdate[0];
			
			sqlConds.Add (new SqlFunction (SqlFunctionCode.CompareEqual, fieldIdName, fieldIdValue));
			
			
			//	Create the UPDATE, INSERT and DELETE commands :
			
			System.Data.IDbCommand updateCommand;
			System.Data.IDbCommand insertCommand;
			System.Data.IDbCommand deleteCommand;
			
			builder.Clear ();
			builder.UpdateData (sqlTableName, sqlUpdate, sqlConds);
			
			updateCommand = builder.Command;
			
			builder.Clear ();
			builder.InsertData (sqlTableName, sqlInsert);
			
			insertCommand = builder.Command;
			
			builder.Clear ();
			builder.RemoveData (sqlTableName, sqlConds);
			
			deleteCommand = builder.Command;
			
			updateCommand.Transaction = transaction.Transaction;
			insertCommand.Transaction = transaction.Transaction;
			deleteCommand.Transaction = transaction.Transaction;
			
			int whereParamIndex = updateCommand.Parameters.Count - 1;
			
			try
			{
				//	For every row in the table, decide what to do : either update,
				//	insert or remove it, depending on its row state.
				
				foreach (System.Data.DataRow row in dataTable.Rows)
				{
					switch (row.RowState)
					{
						case System.Data.DataRowState.Added:
						case System.Data.DataRowState.Modified:
							this.UpdateOrInsertRowIntoTable (builder, row, sqlTableName, sqlColumns, insertDefault, index, updateParamIndex, insertParamIndex, whereParamIndex, updateCommand, insertCommand);
							break;
						
						case System.Data.DataRowState.Deleted:
							this.DeleteRowFromTable (builder, row, sqlTableName, sqlColumns, deleteCommand);
							break;

						default:
							continue;
					}
				}
			}
			finally
			{
				deleteCommand.Dispose ();
				updateCommand.Dispose ();
				insertCommand.Dispose ();
			}
		}

		private void DeleteRowFromTable(ISqlBuilder builder, System.Data.DataRow row, string sqlTableName, SqlColumn[] sqlColumns, System.Data.IDbCommand deleteCommand)
		{
			//	Delete the row from the database. This will use a DELETE
			//	command with the specified row id as the WHERE condition :

			object valueId = TypeConverter.ConvertToInternal (this.infrastructure.Converter, row[0, System.Data.DataRowVersion.Original], sqlColumns[0].Type);

			builder.SetCommandParameterValue (deleteCommand, 0, valueId);

			this.statReplaceDeleteCount += deleteCommand.ExecuteNonQuery ();
		}

		private void UpdateOrInsertRowIntoTable(ISqlBuilder builder, System.Data.DataRow row, string sqlTableName, SqlColumn[] sqlColumns, object[] insertDefault, int index, int[] updateParamIndex, int[] insertParamIndex, int whereParamIndex, System.Data.IDbCommand updateCommand, System.Data.IDbCommand insertCommand)
		{
			//	Update the row in the database. Try an UPDATE command and
			//	if it does not modify the table, use INSERT instead.

			int colCount = sqlColumns.Length;

			object rowId   = row[0, System.Data.DataRowVersion.Current];
			object valueId = TypeConverter.ConvertToInternal (this.infrastructure.Converter, rowId, sqlColumns[0].Type);

			builder.SetCommandParameterValue (updateCommand, whereParamIndex, valueId);

			for (int i = 0; i < colCount; i++)
			{
				if (updateParamIndex[i] < 0)
				{
					//	The column should be ignored when updating and used
					//	only when inserting a new row, and then, we must use
					//	a specific default value :

					builder.SetCommandParameterValue (insertCommand, i, insertDefault[index]);
				}
				else
				{
					//	The column has a value and will be updated or inserted
					//	normally :

					object value = TypeConverter.ConvertToInternal (this.infrastructure.Converter, row[i], sqlColumns[i].Type);

					builder.SetCommandParameterValue (updateCommand, updateParamIndex[i], value);
					builder.SetCommandParameterValue (insertCommand, insertParamIndex[i], value);
				}
			}

			//	Execute the UPDATE command. If no row was modified, then we
			//	must execute the INSERT command to make sure our data gets
			//	persisted in the database :

			int count = updateCommand.ExecuteNonQuery ();

			if (count == 0)
			{
				count = insertCommand.ExecuteNonQuery ();

				if (count != 1)
				{
					throw new Exceptions.FormatException (string.Format ("Insert into table {0} produced {1} changes (ID = {2}); 1 was expected", sqlTableName, count, insertCommand.Parameters[0]));
				}

				this.statReplaceInsertCount++;
			}
			else if (count == 1)
			{
				this.statReplaceUpdateCount++;
			}
			else
			{
				throw new Exceptions.FormatException (string.Format ("Update of table {0} produced {1} changes (ID = {2}); 0 or 1 expected", sqlTableName, count, updateCommand.Parameters[0]));
			}
		}

		struct TableRowId : System.IEquatable<TableRowId>
		{
			public TableRowId(string tableName, long rowId)
			{
				this.tableName = tableName;
				this.rowId     = rowId;
			}

			public string TableName
			{
				get
				{
					return this.tableName;
				}
			}

			public long RowId
			{
				get
				{
					return this.rowId;
				}
			}

			#region IEquatable<TableRowId> Members

			public bool Equals(TableRowId other)
			{
				return this.tableName == other.tableName
					&& this.rowId == other.rowId;
			}

			#endregion

			public override bool Equals(object obj)
			{
				if (obj is TableRowId)
				{
					return this.Equals ((TableRowId) obj);
				}
				else
				{
					return false;
				}
			}

			public override int GetHashCode()
			{
				return this.tableName.GetHashCode ()
					 ^ this.rowId.GetHashCode ();
			}

			private readonly string tableName;
			private readonly long rowId;
		}


		private DbInfrastructure				infrastructure;
		private Collections.DbCommandList		commands;
		private Collections.DbTableList			tables;
		private System.Data.DataSet				dataSet;
		private DbAccess						access;
		private List<System.Data.IDataAdapter>	adapters;
		private List<System.Data.Common.DbCommandBuilder> builders;

		private readonly Dictionary<string, DbDataTableMapping> dataMappings;
		private readonly Dictionary<string, DbDataTableMapping> relationSourceMappings;
		private readonly Dictionary<string, DbDataTableMapping> relationTargetMappings;

		private Stack<DbTransaction>			activeTransactions = new Stack<DbTransaction> ();
		
		private bool							isReadOnly;
		private int								statReplaceUpdateCount;
		private int								statReplaceInsertCount;
		private int								statReplaceDeleteCount;

		// This field is a fugly hack to tell the DbRichCommand itself that it does not need to fill
		// the DataSet in InternalFillDataSet in some cases.
		private bool fillDataSet;
	}
}
