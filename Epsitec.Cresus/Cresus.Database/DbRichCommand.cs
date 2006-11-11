//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

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
			
			this.commands = new Collections.DbCommands ();
			this.tables   = new Collections.DbTables ();
		}

		/// <summary>
		/// Gets the individual commands associated with this rich command.
		/// </summary>
		/// <value>The commands.</value>
		public Collections.DbCommands			Commands
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
		public Collections.DbTables				Tables
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
			return DbRichCommand.CreateFromTable (infrastructure, transaction, table, new DbSelectCondition (infrastructure.Converter, selectRevision));
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
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, Collections.DbTables tables)
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
				select.Tables.Add (table.Name, SqlField.CreateName (table.CreateSqlName ()));
				
				//	If there is no condition, this means we don't want to get any data
				//	for the specific table; just fetch the empty table by using an always
				//	false WHERE clause.
				
				if (condition == null)
				{
					select.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareFalse));
				}
				else
				{
					condition.CreateConditions (table, select.Conditions);
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
		/// Relaxes the data table constraints set up by ADO.NET so that we can
		/// fill the rows with partial data.
		/// </summary>
		/// <param name="table">The table.</param>
		internal static void RelaxConstraints(System.Data.DataTable table)
		{
			if (table.Columns[Tags.ColumnId].Unique == false)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Warning: Table {0} ID not unique, fixing.", table.TableName));
				table.Columns[Tags.ColumnId].Unique = true;
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
				for (int i = 0; i < this.adapters.Length; i++)
				{
					this.adapters[i].Update (this.dataSet);
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
					
					DbRichCommand.AssignRealRowIds (this.infrastructure, transaction, table);
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
				DbRichCommand.UpdateLogIds (table, logId);
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
			
			for (int i = 0; i < this.adapters.Length; i++)
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
		/// class and this is verified at execution time by doing a stack walk. Use the
		/// <c>DbInfrastructure.Execute</c> method instead.
		/// </summary>
		/// <param name="access">The access.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="adapters">The adapters.</param>
		public void InternalFillDataSet(DbAccess access, DbTransaction transaction, System.Data.IDbDataAdapter[] adapters)
		{
			//	Verify that we have been called by DbInfrastructure through the
			//	ISqlEngine layer :
			//
			//	xxx --> DbInfrastructure --> ISqlEngine --> DbRichCommand
			
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace (true);
			System.Diagnostics.StackFrame caller1 = trace.GetFrame (1);
			System.Diagnostics.StackFrame caller2 = trace.GetFrame (2);
			
			System.Type callerClassType = caller1.GetMethod ().DeclaringType;
			System.Type reqInterfType   = typeof (Epsitec.Cresus.Database.ISqlEngine);
			
			if ((callerClassType.GetInterface (reqInterfType.FullName) != reqInterfType) ||
				(caller2.GetMethod ().DeclaringType != typeof (DbInfrastructure)))
			{
				throw new System.InvalidOperationException (string.Format ("Method may not be called by {0}.{1}", callerClassType.FullName, caller1.GetMethod ().Name));
			}

			System.Diagnostics.Debug.Assert (access.IsValid);
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (access);
			}
			
			if (this.dataSet != null)
			{
				throw new Exceptions.GenericException (access, "DataSet already exists.");
			}
			
			//	Fill the data set based on the adapter objects provided by
			//	the caller :

			this.access   = access;
			this.dataSet  = new System.Data.DataSet ();
			this.adapters = adapters;
			
			this.SetCommandTransaction (transaction);
			
			try
			{
				for (int i = 0; i < this.tables.Count; i++)
				{
					DbTable dbTable = this.tables[i];
					
					string  adoNameTable = "Table";
					string  dbNameTable  = dbTable.Name;
					
					//	Ensure that the column and table names match what we expect,
					//	based on the DbColumn and DbTable names.
					
					System.Data.ITableMapping mapping = this.adapters[i].TableMappings.Add (adoNameTable, dbNameTable);
					
					foreach (DbColumn dbColumn in dbTable.Columns)
					{
						if (dbColumn.Localization == DbColumnLocalization.Localized)
						{
							foreach (string localizationSuffix in dbTable.Localizations)
							{
								string dbNameColumn  = dbColumn.MakeLocalizedName (localizationSuffix);
								string adoNameColumn = dbColumn.MakeLocalizedSqlName (localizationSuffix);

								mapping.ColumnMappings.Add (adoNameColumn, dbNameColumn);
							}
						}
						else
						{
							string dbNameColumn  = dbColumn.Name;
							string adoNameColumn = dbColumn.CreateSqlName ();

							mapping.ColumnMappings.Add (adoNameColumn, dbNameColumn);
						}
					}
					
					this.adapters[i].MissingSchemaAction = System.Data.MissingSchemaAction.AddWithKey;
					this.adapters[i].Fill (this.dataSet);
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
		}
		
		#endregion

		/// <summary>
		/// Extracts the live rows from a given collection.
		/// </summary>
		/// <param name="rows">A collection of rows.</param>
		/// <returns>The live rows.</returns>
		public static System.Data.DataRow[] GetLiveRows(System.Collections.IEnumerable rows)
		{
			List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
			
			foreach (System.Data.DataRow row in rows)
			{
				if (DbRichCommand.IsRowLive (row))
				{
					list.Add (row);
				}
			}
			
			return list.ToArray ();
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

		/// <summary>
		/// Assign real row ids to the new data table rows for a given table.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="table">The table.</param>
		public static void AssignRealRowIds(DbInfrastructure infrastructure, DbTransaction transaction, System.Data.DataTable table)
		{
			List<System.Data.DataRow> list = Collection.ToList<System.Data.DataRow> (DbRichCommand.FindRowsUsingTemporaryIds (table.Rows));
			
			if (list.Count == 0)
			{
				return;
			}
			
			DbTable dbTable = infrastructure.ResolveDbTable (transaction, table.TableName);
			
			//	Allocate real row ids for the temporary rows; thanks to the relations
			//	defined at the data set level, the foreign keys will be updated too.
			
			long id = infrastructure.NewRowIdInTable (transaction, dbTable, list.Count);
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Allocating {0} new IDs for table {1} starting at {2}.", list.Count, table.TableName, id));
			
			foreach (System.Data.DataRow row in list)
			{
				DbKey key = new DbKey (row);
				
				System.Diagnostics.Debug.Assert (key.IsTemporary);
				
				key = new DbKey (id++);
				
				row.BeginEdit ();
				row[Tags.ColumnId]     = key.Id.Value;
				row[Tags.ColumnStatus] = key.IntStatus;
				row.EndEdit ();
			}
		}

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
					case System.Data.DataRowState.Modified:
						DbRichCommand.DefineLogId (row, logId);
						break;
				}
			}
		}

		/// <summary>
		/// Defines the log id for the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <param name="logId">The log id.</param>
		private static void DefineLogId(System.Data.DataRow row, DbId logId)
		{
			System.Diagnostics.Debug.Assert (row.RowState != System.Data.DataRowState.Deleted);
			
			row.BeginEdit ();
			row[Tags.ColumnRefLog] = logId.Value;
			row.EndEdit ();
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
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
			
			row.BeginEdit ();
			row[Tags.ColumnId]     = key.Id.Value;
			row[Tags.ColumnStatus] = key.IntStatus;
			row.EndEdit ();

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
		/// Finds a row in a data table based on a row id.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="id">The id.</param>
		/// <returns>The row or <c>null</c> if it cannot be found in the table.</returns>
		public static System.Data.DataRow FindRow(System.Data.DataTable table, DbId id)
		{
			return DbRichCommand.FindRow (table.Rows, id);
		}

		/// <summary>
		/// Finds the row in a collection of rows based on a row id.
		/// </summary>
		/// <param name="rows">The rows.</param>
		/// <param name="id">The id.</param>
		/// <returns>The row or <c>null</c> if it cannot be found in the collection.</returns>
		public static System.Data.DataRow FindRow(System.Collections.IEnumerable rows, DbId id)
		{
			foreach (System.Data.DataRow row in rows)
			{
				long rowId;
				
				if (row.RowState == System.Data.DataRowState.Deleted)
				{
					rowId = (long) row[Tags.ColumnId, System.Data.DataRowVersion.Original];
				}
				else if (row.RowState == System.Data.DataRowState.Detached)
				{
					continue;
				}
				else
				{
					rowId = (long) row[Tags.ColumnId];
				}
				
				if (id.Value == rowId)
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
				foreach (System.Data.IDbCommand command in this.commands)
				{
					command.Transaction = dataTransaction;
				}
			}
			catch
			{
				this.activeTransactions.Pop ();
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

			foreach (System.Data.IDbCommand command in this.commands)
			{
				command.Transaction = null;
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
				(this.adapters.Length == 0))
			{
				throw new Exceptions.GenericException (this.access, "No adapters defined.");
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
		/// <param name="dbTable">The table definition.</param>
		/// <param name="options">The replace options.</param>
		private void ReplaceTablesWithoutValidityChecking(DbTransaction transaction, System.Data.DataTable dataTable, DbTable dbTable, IReplaceOptions options)
		{
			string sqlTableName = dbTable.CreateSqlName ();
			
			IDbAbstraction database  = transaction.Database;
			ISqlBuilder    builder   = database.SqlBuilder;
			ITypeConverter converter = this.infrastructure.Converter;
			
			Collections.SqlFields sqlUpdate = new Collections.SqlFields ();
			Collections.SqlFields sqlInsert = new Collections.SqlFields ();
			Collections.SqlFields sqlConds  = new Collections.SqlFields ();
			
			int colCount = dbTable.GetSqlColumnCount ();
			int rowCount = dataTable.Rows.Count;
			
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
			
			foreach (DbColumn column in dbTable.Columns)
			{
				bool ignoreColumn = (options != null) && options.ShouldIgnoreColumn (column);
				
				foreach (SqlColumn sqlColumn in dbTable.CreateSqlColumns (converter, column))
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
					if ((row.RowState != System.Data.DataRowState.Added) &&
						(row.RowState != System.Data.DataRowState.Modified) &&
						(row.RowState != System.Data.DataRowState.Deleted))
					{
						continue;
					}
					
					if (row.RowState == System.Data.DataRowState.Deleted)
					{
						//	Delete the row from the database. This will use a DELETE
						//	command with the specified row id as the WHERE condition :

						object valueId = TypeConverter.ConvertToInternal (this.infrastructure.Converter, row[0, System.Data.DataRowVersion.Original], sqlColumns[0].Type);
						
						builder.SetCommandParameterValue (deleteCommand, 0, valueId);
						
						this.statReplaceDeleteCount += deleteCommand.ExecuteNonQuery ();
					}
					else
					{
						//	Update the row in the database. Try an UPDATE command and
						//	if it does not modify the table, use INSERT instead.

						object valueId = TypeConverter.ConvertToInternal (this.infrastructure.Converter, row[0, System.Data.DataRowVersion.Current], sqlColumns[0].Type);

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
				}
			}
			finally
			{
				deleteCommand.Dispose ();
				updateCommand.Dispose ();
				insertCommand.Dispose ();
			}
		}


		private DbInfrastructure				infrastructure;
		private Collections.DbCommands			commands;
		private Collections.DbTables			tables;
		private System.Data.DataSet				dataSet;
		private DbAccess						access;
		private System.Data.IDataAdapter[]		adapters;

		private Stack<DbTransaction>			activeTransactions = new Stack<DbTransaction> ();
		
		private bool							isReadOnly;
		private int								statReplaceUpdateCount;
		private int								statReplaceInsertCount;
		private int								statReplaceDeleteCount;
	}
}
