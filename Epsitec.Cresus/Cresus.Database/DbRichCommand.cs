//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbRichCommand</c> class manages one or several commands which can
	/// be executed with <c>ISqlEngine</c>. It handles the select, update, insert
	/// and delete of information in data tables.
	/// </summary>
	public sealed class DbRichCommand : System.IDisposable
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
			return DbRichCommand.CreateFromTable (infrastructure, transaction, table, new DbSelectCondition (infrastructure.TypeConverter, selectRevision));
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
					select.Conditions.Add (new SqlFunction (SqlFunctionType.CompareFalse));
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
					this.ReplaceTable (transaction, table, this.Tables[i], options);
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
		public System.Data.DataRow CreateNewRow(string tableName)
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
			ICollection<System.Data.DataRow> list = DbRichCommand.FindRowsUsingTemporaryIds (table);
			
			if (list.Count == 0)
			{
				return;
			}
			
			DbKey tableKey = infrastructure.FindDbTableKey (transaction, table.TableName);
			
			//	Allocate real row ids for the temporary rows; thanks to the relations
			//	defined at the data set level, the foreign keys will be updated too.
			
			long id = infrastructure.NewRowIdInTable (transaction, tableKey, list.Count);
			
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
						row.BeginEdit ();
						row[Tags.ColumnRefLog] = logId.Value;
						row.EndEdit ();
						break;
				}
			}
		}
		
		public static void DefineLogId(System.Data.DataRow row, DbId logId)
		{
			if (row.RowState != System.Data.DataRowState.Deleted)
			{
				row.BeginEdit ();
				row[Tags.ColumnRefLog] = logId.Value;
				row.EndEdit ();
			}
		}
		
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
		public static void CreateRow(System.Data.DataTable table, DbId logId, out System.Data.DataRow row)
		{
			row = DbRichCommand.CreateRow (table);
			DbRichCommand.DefineLogId (row, logId);
		}
		
		public static void DeleteRow(System.Data.DataRow dataRow)
		{
			DbKey rowKey = new DbKey (dataRow);
			
			//	Si la ligne a encore une clef temporaire, cela signifie qu'elle n'a pas encore
			//	été écrite dans la base; on peut donc simplement supprimer la ligne de la table.
			//	Dans le cas contraire, on ne supprime jamais réellement les lignes effacées et
			//	on change simplement le statut de la ligne à "deleted".
			
			if (rowKey.IsTemporary)
			{
				System.Data.DataTable table = dataRow.Table;
				table.Rows.Remove (dataRow);
			}
			else
			{
				dataRow[Tags.ColumnStatus] = DbKey.ConvertToIntStatus (DbRowStatus.Deleted);
			}
		}
		
		public static void KillRow(System.Data.DataRow row)
		{
			//	Supprime réellement la ligne de la table. Cette méthode est réservée à un
			//	usage très limité; en principe, on utilisera DeleteRow, sauf pour la queue
			//	des requêtes, par exemple.
			
			if (row.RowState != System.Data.DataRowState.Deleted)
			{
				row.Delete ();
			}
		}
		
		
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
		
		public static System.Data.DataRow FindRow(System.Data.DataTable table, DbId id)
		{
			int n = table.Rows.Count;
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = table.Rows[i];
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
		
		public static System.Data.DataRow FindRow(System.Data.DataRow[] rows, DbId id)
		{
			int n = rows.Length;
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = rows[i];
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
		
		public static ICollection<System.Data.DataRow> FindRowsUsingTemporaryIds(System.Data.DataTable table)
		{
			//	Passe en revue toutes les lignes de la table pour déterminer s'il y a des
			//	clefs temporaires en utilisation et retourne la liste des lignes concernées.

			List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
			
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (row.RowState != System.Data.DataRowState.Deleted)
				{
					DbKey key = new DbKey (row);
					
					if (key.IsTemporary)
					{
						list.Add (row);
					}
				}
			}
			
			return list;
		}
		
		
		public static void DebugDumpCommand(System.Data.IDbCommand command)
		{
			System.Diagnostics.Debug.WriteLine (command.CommandText);
			
			foreach (System.Data.IDataParameter commandParameter in command.Parameters)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0} = {1}, type {2}", commandParameter.ParameterName, commandParameter.Value, commandParameter.Value.GetType ().FullName));
			}
		}
		
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


		private void CreateDataRelations()
		{
			//	Crée pour le DataSet actuel les relations entre les diverses colonnes,
			//	en s'appuyant sur les propriétés de DbTable/DbColumn.
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable               dbChildTable  = this.tables[i];
				System.Data.DataTable adoChildTable = this.dataSet.Tables[dbChildTable.Name];
				DbForeignKey[]        dbForeignKeys = dbChildTable.ForeignKeys;
				
				foreach (DbForeignKey fk in dbForeignKeys)
				{
					int n = fk.Columns.Length;

					System.Data.DataTable adoTargetTable = this.dataSet.Tables[fk.TargetTableName];

					if (adoTargetTable == null)
					{
						//	La table cible n'est pas chargée dans le DataSet, ce qui veut dire
						//	que l'on doit ignorer la relation.
						
						continue;
					}
					
					System.Data.DataColumn[] adoTargetCols = new System.Data.DataColumn[n];
					System.Data.DataColumn[] adoChildCols  = new System.Data.DataColumn[n];
					
					for (int j = 0; j < n; j++)
					{
						adoChildCols[j]  = adoChildTable.Columns[fk.Columns[j].CreateDisplayName ()];
						adoTargetCols[j] = adoTargetTable.Columns[fk.Columns[j].TargetColumnName];
					}

					System.Data.DataRelation relation = new System.Data.DataRelation (null, adoTargetCols, adoChildCols);
					this.dataSet.Relations.Add (relation);
					
					System.Data.ForeignKeyConstraint constraint = relation.ChildKeyConstraint;
					
					System.Diagnostics.Debug.Assert (constraint != null);
					System.Diagnostics.Debug.Assert (constraint.UpdateRule == System.Data.Rule.Cascade);
				}
			}
		}

		private void SetCommandTransaction(DbTransaction transaction)
		{
			System.Data.IDbTransaction dataTransaction = transaction.Transaction;
			
			System.Diagnostics.Debug.Assert (transaction != null);

			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = dataTransaction;
			}

			this.activeTransactions.Push (transaction);
		}

		private void PopCommandTransaction()
		{
			this.activeTransactions.Pop ();
			
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = null;
			}
		}

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

		private void ReplaceTable(DbTransaction transaction, System.Data.DataTable dataTable, DbTable dbTable, IReplaceOptions options)
		{
			string tableName = dbTable.CreateSqlName ();
			
			IDbAbstraction database  = transaction.Database;
			ISqlBuilder    builder   = database.SqlBuilder;
			ITypeConverter converter = this.infrastructure.TypeConverter;
			
			Collections.SqlFields sqlUpdate = new Collections.SqlFields ();
			Collections.SqlFields sqlInsert = new Collections.SqlFields ();
			Collections.SqlFields sqlConds  = new Collections.SqlFields ();
			
			int colCount = dbTable.Columns.Count;
			int rowCount = dataTable.Rows.Count;
			
			//	Crée pour chaque colonne de la table une représentation SQL et un
			//	champ qui permettra de stocker la valeur :
			
			SqlColumn[] sqlColumns = new SqlColumn[colCount];
			
			int[]    updateMap     = new int[colCount];
			object[] insertDefault = new object[colCount];
			
			for (int c = 0; c < colCount; c++)
			{
				//	TODO: handle multiple cultures...
				DbColumn column = dbTable.Columns[c];
				sqlColumns[c] = column.CreateSqlColumn (converter, null);
				
				if ((options == null) ||
					(options.IgnoreColumn (c, column) == false))
				{
					//	Aucune option particulière pour cette colonne. Ajoute simplement
					//	la colonne à la fois pour le UPDATE et pour le INSERT :
					
					sqlUpdate.Add (this.infrastructure.CreateEmptySqlField (column));
					sqlInsert.Add (this.infrastructure.CreateEmptySqlField (column));
					
					updateMap[c]     = c;
					insertDefault[c] = null;
				}
				else
				{
					//	Les options indiquent que l'on doit ignorer cette colonne lors du
					//	UPDATE; on va aussi fournir une valeur par défaut pour le INSERT :
					
					sqlInsert.Add (this.infrastructure.CreateEmptySqlField (column));
					
					updateMap[c]     = -1;
					insertDefault[c] = options.GetDefaultValue (c, column);
				}
			}
			
			//	Crée la condition pour le UPDATE ... WHERE CR_ID = n
			
			SqlField fieldIdName  = SqlField.CreateName (tableName, sqlColumns[0].Name);
			SqlField fieldIdValue = sqlUpdate[0];
			
			sqlConds.Add (new SqlFunction (SqlFunctionType.CompareEqual, fieldIdName, fieldIdValue));
			
			
			//	Crée les commandes pour le UPDATE et pour le INSERT :
			
			System.Data.IDbCommand updateCommand;
			System.Data.IDbCommand insertCommand;
			System.Data.IDbCommand deleteCommand;
			
			builder.Clear ();
			builder.UpdateData (tableName, sqlUpdate, sqlConds);
			
			updateCommand = builder.Command;
			
			builder.Clear ();
			builder.InsertData (tableName, sqlInsert);
			
			insertCommand = builder.Command;
			
			builder.Clear ();
			builder.RemoveData (tableName, sqlConds);
			
			deleteCommand = builder.Command;
			
			updateCommand.Transaction = transaction.Transaction;
			insertCommand.Transaction = transaction.Transaction;
			deleteCommand.Transaction = transaction.Transaction;
			
			int paramIdIndex = updateCommand.Parameters.Count - 1;
			
			try
			{
				//	Passe en revue toutes les lignes de la table :
				
				for (int r = 0; r < rowCount; r++)
				{
					System.Data.DataRow row = dataTable.Rows[r];
					
					if ((row.RowState != System.Data.DataRowState.Added) &&
						(row.RowState != System.Data.DataRowState.Modified) &&
						(row.RowState != System.Data.DataRowState.Deleted))
					{
						continue;
					}
					
					if (row.RowState == System.Data.DataRowState.Deleted)
					{
						//	Supprime la ligne en question de la table; met juste à jour l'ID de
						//	la ligne dans la commande avant d'exécuter celle-ci :
						
						int    count;
						object valueId = sqlColumns[0].ConvertToInternalType (row[0, System.Data.DataRowVersion.Original]);
						
						builder.SetCommandParameterValue (deleteCommand, 0, valueId);
						count = deleteCommand.ExecuteNonQuery ();
						
						this.statReplaceDeleteCount += count;
					}
					else
					{
						//	Met à jour la ligne en question dans la table. Tente d'abord un UPDATE
						//	et en cas d'échec, recourt à INSERT.
						//	Commence par mettre à jour tous les paramètres des deux commandes :
						
						int    count;
						object valueId = sqlColumns[0].ConvertToInternalType (row[0, System.Data.DataRowVersion.Current]);
						
						builder.SetCommandParameterValue (updateCommand, paramIdIndex, valueId);
						
						for (int c = 0; c < colCount; c++)
						{
							int mapC = updateMap[c];
							
							//	La colonne peut-elle être utilisée telle quelle dans un UPDATE ?
							
							if (mapC < 0)
							{
								//	La colonne ne sera utilisée que pour le INSERT; dans ce cas
								//	il faudra utiliser une valeur par défaut en lieu et place de
								//	la valeur proposée dans la source :
								
								builder.SetCommandParameterValue (insertCommand, c, insertDefault[c]);
							}
							else
							{
								object value = sqlColumns[c].ConvertToInternalType (row[c]);
								
								builder.SetCommandParameterValue (updateCommand, mapC, value);
								builder.SetCommandParameterValue (insertCommand, c, value);
							}
						}
						
						count = updateCommand.ExecuteNonQuery ();
						
						if (count == 0)
						{
							//	Le UPDATE n'a modifié aucune ligne dans la base de données; cela signifie que la
							//	ligne n'était pas connue. On va donc procéder à son insertion :
							
							count = insertCommand.ExecuteNonQuery ();
							
							if (count != 1)
							{
								throw new Exceptions.FormatException (string.Format ("Insert into table {0} produced {1} changes (ID = {2}). Expected exactly 1.", tableName, count, insertCommand.Parameters[0]));
							}
							
							this.statReplaceInsertCount++;
						}
						else if (count == 1)
						{
							this.statReplaceUpdateCount++;
						}
						else
						{
							throw new Exceptions.FormatException (string.Format ("Update of table {0} produced {1} changes (ID = {2}). Expected 0 or 1.", tableName, count, updateCommand.Parameters[0]));
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
		
		
		public void InternalFillDataSet(DbAccess access, DbTransaction transaction, System.Data.IDbDataAdapter[] adapters)
		{
			//	Utiliser DbInfrastructure.Execute en lieu et place de cette méthode !
			
			//	Cette méthode ne devrait jamais être appelée par un utilisateur : elle est réservée
			//	aux classes implémentant ISqlEngine. Pour s'assurer que personne ne se trompe, on
			//	vérifie l'identité de l'appelant :
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
			
			//	Définit et remplit le DataSet en se basant sur les données fournies
			//	par l'objet 'adapter' (ADO.NET).

			this.access    = access;
			this.dataSet  = new System.Data.DataSet ();
			this.adapters  = adapters;
			
			this.SetCommandTransaction (transaction);
			
			try
			{
				for (int i = 0; i < this.tables.Count; i++)
				{
					DbTable dbTable = this.tables[i];
					
					string  adoNameTable = "Table";
					string  dbNameTable  = dbTable.Name;
					
					//	Il faut (re)nommer les tables afin d'avoir les noms qui correspondent
					//	à ce que définit DbTable, et faire pareil pour les colonnes.
					
					System.Data.ITableMapping mapping = this.adapters[i].TableMappings.Add (adoNameTable, dbNameTable);
					
					for (int c = 0; c < dbTable.Columns.Count; c++)
					{
						DbColumn dbColumn = dbTable.Columns[c];
						
						string dbNameColumn  = dbColumn.CreateDisplayName ();
						string adoNameColumn = dbColumn.CreateSqlName ();
						
						mapping.ColumnMappings.Add (adoNameColumn, dbNameColumn);
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
		
		public void Dispose()
		{
			if (this.dataSet != null)
			{
				this.dataSet.Dispose ();
				this.dataSet = null;
			}

			System.Data.IDbCommand[] commands = this.commands.ToArray ();

			for (int i = 0; i < commands.Length; i++)
			{
				commands[i].Dispose ();
			}
		}
		
		#endregion
		


		private DbInfrastructure infrastructure;
		private Collections.DbCommands commands;
		private Collections.DbTables tables;
		private System.Data.DataSet dataSet;
		private DbAccess access;
		private System.Data.IDataAdapter[] adapters;

		Stack<DbTransaction>					activeTransactions = new Stack<DbTransaction> ();
		
		private bool							isReadOnly;
		private int								statReplaceUpdateCount;
		private int								statReplaceInsertCount;
		private int								statReplaceDeleteCount;
	}
}
