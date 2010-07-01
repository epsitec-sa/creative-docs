//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.Requests
{
	
	
	/// <summary>
	/// The <c>ExecutionEngine</c> class executes requests which modify the
	/// database.
	/// </summary>
	public sealed class ExecutionEngine : System.IDisposable
	{


		public ExecutionEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
		}
		
		
		public DbInfrastructure Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		

		public DbId CurrentLogId
		{
			get
			{
				return this.currentLogId;
			}
		}
		

		public DbTransaction CurrentTransaction
		{
			get
			{
				return this.currentTransaction;
			}
		}
		

		public ISqlBuilder CurrentSqlBuilder
		{
			get
			{
				return this.currentSqlBuilder;
			}
		}


		/// <summary>
		/// Executes the specified request within the active transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="request">The request.</param>
		public void Execute(DbTransaction transaction, AbstractRequest request)
		{
			try
			{
				this.DefineCurrentLogId ();
				this.DefineCurrentTransaction (transaction);
				
				this.expectedRowsChanged = 0;
				
				request.Execute (this);
				
				int rowsChanged = this.infrastructure.ExecuteSilent (transaction, this.currentSqlBuilder);
				
				if (rowsChanged != this.expectedRowsChanged)
				{
					throw new Database.Exceptions.ConflictingException (string.Format ("Request execution expected to update {0} rows; updated {1} instead.", this.expectedRowsChanged, rowsChanged));
				}
			}
			finally
			{
				this.CleanUp ();
			}
		}


		/// <summary>
		/// Generates an insert data command. The data is converted to a format compatible
		/// with the underlying database. The command inserts a single row.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The data columns.</param>
		/// <param name="values">The data values.</param>
		public void GenerateInsertDataCommand(string tableName, string[] columns, object[] values)
		{
			if (columns.Length != values.Length)
			{
				throw new System.ArgumentException ("Columns/values mismatch.");
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("Inserting row in table {0}; columns: {1}", tableName, string.Join (", ", columns)));
			
			DbTable      table      = this.FindTable (tableName);
			SqlColumn[]  sqlColumns = ExecutionEngine.CreateSqlColumns (this.infrastructure, table, columns);
			SqlFieldList sqlFields  = ExecutionEngine.CreateSqlValues (this.infrastructure, sqlColumns, values);
			
			if (ExecutionEngine.IsLogIdNeededForTable (table))
			{
				ExecutionEngine.FixLogId (table, columns, sqlFields, this.currentLogId);
			}
			
			this.PrepareCommand ();
			this.currentSqlBuilder.InsertData (table.GetSqlName (), sqlFields);
			
			this.expectedRowsChanged++;
		}


		/// <summary>
		/// Generates an update data command. The data is converted to a format compatible
		/// with the underlying database. The command updates a single row.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="condColumns">The condition columns.</param>
		/// <param name="condValues">The condition values.</param>
		/// <param name="dataColumns">The data columns.</param>
		/// <param name="dataValues">The data values.</param>
		public void GenerateUpdateDataCommand(string tableName, string[] condColumns, object[] condValues, string[] dataColumns, object[] dataValues)
		{
			if (condColumns.Length != condValues.Length)
			{
				throw new System.ArgumentException ("Condition columns/values mismatch.");
			}
			if (dataColumns.Length != dataValues.Length)
			{
				throw new System.ArgumentException ("Data columns/values mismatch.");
			}

			System.Diagnostics.Debug.WriteLine (string.Format ("Updating row in table {0}; columns: {1}; conditions: {2}", tableName, string.Join (", ", dataColumns), string.Join (", ", condColumns)));
			
			DbTable table = this.FindTable (tableName);

			SqlColumn[]  sqlCondColumns = ExecutionEngine.CreateSqlColumns (this.infrastructure, table, condColumns);
			SqlColumn[]  sqlDataColumns = ExecutionEngine.CreateSqlColumns (this.infrastructure, table, dataColumns);
			SqlFieldList sqlCondValues  = ExecutionEngine.CreateSqlValues (this.infrastructure, sqlCondColumns, condValues);
			SqlFieldList sqlDataFields  = ExecutionEngine.CreateSqlValues (this.infrastructure, sqlDataColumns, dataValues);

			if (ExecutionEngine.IsLogIdNeededForTable (table))
			{
				ExecutionEngine.FixLogId (table, dataColumns, sqlDataFields, this.currentLogId);
			}

			SqlFieldList sqlCondFields = new SqlFieldList ();
			
			for (int i = 0; i < sqlCondValues.Count; i++)
			{
				SqlField name  = SqlField.CreateName (sqlCondColumns[i].Name);
				SqlField value = sqlCondValues[i];
				
				sqlCondFields.Add (new SqlFunction (SqlFunctionCode.CompareEqual, name, value));
			}
			
			this.PrepareCommand ();
			this.currentSqlBuilder.UpdateData (table.GetSqlName (), sqlDataFields, sqlCondFields);
			
			this.expectedRowsChanged++;
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}


		private void Dispose(bool disposing)
		{
		}


		#endregion


		/// <summary>
		/// Defines the current log id by using the active one, provided by the logger.
		/// </summary>
		private void DefineCurrentLogId()
		{
			System.Diagnostics.Debug.Assert (this.currentLogId.Value == 0);

			this.currentLogId = this.infrastructure.Logger.CurrentId;
		}


		/// <summary>
		/// Defines the current transaction.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		private void DefineCurrentTransaction(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.currentTransaction == null);

			this.currentTransaction = transaction;
			this.currentSqlBuilder  = transaction.SqlBuilder.NewSqlBuilder ();
		}


		/// <summary>
		/// Finds the specified table definition.
		/// </summary>
		/// <exception cref="System.ArgumentException">Thrown if the table cannot be found.</exception>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>The table definition.</returns>
		private DbTable FindTable(string tableName)
		{
			DbTable table = this.infrastructure.ResolveDbTable (this.currentTransaction, tableName);

			if (table == null)
			{
				throw new System.ArgumentException (string.Concat ("Cannot find table ", tableName, "."));
			}

			return table;
		}

		
		/// <summary>
		/// Prepares the context for a new command.
		/// </summary>
		private void PrepareCommand()
		{
			System.Diagnostics.Debug.Assert (this.currentTransaction != null);
			System.Diagnostics.Debug.Assert (this.currentSqlBuilder != null);

			if (this.pendingCommands > 0)
			{
				this.currentSqlBuilder.AppendMore ();
			}
			else
			{
				this.currentSqlBuilder.Clear ();
			}

			this.pendingCommands++;
		}


		/// <summary>
		/// Cleans up after execution of a request.
		/// </summary>
		private void CleanUp()
		{
			System.Diagnostics.Debug.Assert (this.currentTransaction != null);

			if (this.pendingCommands > 0)
			{
				this.currentSqlBuilder.Clear ();
				this.pendingCommands = 0;
			}

			if (this.currentSqlBuilder != null)
			{
				this.currentSqlBuilder.Dispose ();
			}

			this.currentLogId       = DbId.Zero;
			this.currentTransaction = null;
			this.currentSqlBuilder  = null;
		}
		
		
		/// <summary>
		/// Determines whether a log id is needed for the specified table.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <returns>
		/// 	<c>true</c> if a log id is needed for the specified table; otherwise, <c>false</c>.
		/// </returns>
		private static bool IsLogIdNeededForTable(DbTable table)
		{
			return table.Columns.Contains (Tags.ColumnRefLog);
		}


		/// <summary>
		/// Fixes the log id in the incoming fields by using the current log id instead
		/// of the one provided by the caller.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="columnNames">The column names.</param>
		/// <param name="fields">The fields.</param>
		/// <param name="currentLogId">The current log id.</param>
		private static void FixLogId(DbTable table, string[] columnNames, SqlFieldList fields, DbId currentLogId)
		{
			for (int i = 0; i < columnNames.Length; i++)
			{
				if (columnNames[i] == Tags.ColumnRefLog)
				{
					//	Found the CREF_LOG column; replace the value with the current log id.
					
					System.Diagnostics.Debug.Assert (fields[i].RawType == DbKey.RawTypeForId);
					System.Diagnostics.Debug.Assert (fields[i].Alias == table.Columns[Tags.ColumnRefLog].GetSqlName ());
					
					fields[i].Overwrite (SqlField.CreateConstant (currentLogId.Value, DbKey.RawTypeForId));
					fields[i].Alias = table.Columns[Tags.ColumnRefLog].GetSqlName ();
					
					return;
				}
			}
			
			//	We did not find any field defining the log id; add one more field to store
			//	the specified information :
			
			SqlField field = SqlField.CreateConstant (currentLogId.Value, DbKey.RawTypeForId);
			string   alias = table.Columns[Tags.ColumnRefLog].GetSqlName ();
			
			fields.Add (alias, field);
		}


		/// <summary>
		/// Creates the SQL column definitions.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="table">The table.</param>
		/// <param name="columnNames">The column names.</param>
		/// <returns>An array of SQL column definitions.</returns>
		private static SqlColumn[] CreateSqlColumns(DbInfrastructure infrastructure, DbTable table, string[] columnNames)
		{
			SqlColumn[]    columns   = new SqlColumn[columnNames.Length];
			ITypeConverter converter = infrastructure.Converter;
			
			for (int i = 0; i < columns.Length; i++)
			{
				//	TODO: handle multiple cultures...
				
				columns[i] = table.Columns[columnNames[i]].CreateSqlColumn (converter, null);
			}
			
			return columns;
		}


		/// <summary>
		/// Creates the SQL values and returns them as a field list.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="columns">The columns.</param>
		/// <param name="values">The raw values.</param>
		/// <returns>
		/// The field list of the converted SQL values.
		/// </returns>
		private static SqlFieldList CreateSqlValues(DbInfrastructure infrastructure, SqlColumn[] columns, object[] values)
		{
			SqlFieldList fields = new SqlFieldList ();
			
			for (int i = 0; i < columns.Length; i++)
			{
				SqlField field = infrastructure.CreateSqlFieldFromAdoValue (columns[i], values[i]);
				string   alias = columns[i].Name;
				
				fields.Add (alias, field);
			}
			
			System.Diagnostics.Debug.Assert (fields.Count == columns.Length);
			System.Diagnostics.Debug.Assert (fields.Count == values.Length);
			
			return fields;
		}


		private readonly DbInfrastructure				infrastructure;

		private DbId									currentLogId;
		private DbTransaction							currentTransaction;
		private ISqlBuilder								currentSqlBuilder;
		private int										pendingCommands;
		private int										expectedRowsChanged;
	
	
	}


}
