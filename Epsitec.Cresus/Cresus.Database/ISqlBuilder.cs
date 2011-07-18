//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using System.Collections.Generic;


namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>ISqlBuilder</c> interface is used to build SQL commands based on an
	/// abstract tree like representation.
	/// </summary>
	public interface ISqlBuilder : ISqlValidator, System.IDisposable
	{
		/// <summary>
		/// Creates a new SQL builder.
		/// </summary>
		/// <returns>An SQL builder.</returns>
		ISqlBuilder NewSqlBuilder();

		/// <summary>
		/// Gets or sets a value indicating whether the builder should be
		/// cleared automatically when a new command definition starts.
		/// </summary>
		/// <value><c>true</c> for automatic call to <c>Clear</c>; otherwise, <c>false</c>.</value>
		bool AutoClear
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the type of the command.
		/// </summary>
		/// <value>The type of the command.</value>
		DbCommandType CommandType
		{
			get;
		}

		/// <summary>
		/// Gets the database command object.
		/// </summary>
		/// <value>The command.</value>
		System.Data.IDbCommand Command
		{
			get;
		}

		/// <summary>
		/// Gets the number of SQL commands encoded by the database command.
		/// </summary>
		/// <value>The command count.</value>
		int CommandCount
		{
			get;
		}

		/// <summary>
		/// Creates the command within the specified transaction, based on
		/// the builder definition.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The command.</returns>
		System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction);

		/// <summary>
		/// Creates the command within the specified transaction, based on
		/// an SQL command.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="sqlCommand">The SQL command.</param>
		/// <returns>The command.</returns>
		System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction, string sqlCommand);

		/// <summary>
		/// Clears this SQL builder. You must clear the builder before starting
		/// any command definition, unless the builder is configured with <c>AutoClear</c>
		/// set to <c>true</c>.
		/// </summary>
		void Clear();

		/// <summary>
		/// Specifies that an additional command will be added to the currently
		/// defined command.
		/// </summary>
		void AppendMore();

		/// <summary>
		/// Inserts the specified table definition into the database.
		/// </summary>
		/// <param name="table">The table.</param>
		void InsertTable(SqlTable table);

		/// <summary>
		/// Removes the specified table from the database.
		/// </summary>
		/// <param name="table">The table.</param>
		void RemoveTable(SqlTable table);
		
		/// <summary>
		/// Inserts columns into the table definition.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The columns.</param>
		void InsertTableColumns(string tableName, SqlColumn[] columns);

		void RenameTableColumn(string tableName, string oldColumnName, string newColumnName);
		
		/// <summary>
		/// Removes the columns from a table definition.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The columns.</param>
		void RemoveTableColumns(string tableName, SqlColumn[] columns);

		/// <summary>
		/// Adds an auto increment clause on a column.
		/// </summary>
		/// <param name="tableName">The name of the table containing the column.</param>
		/// <param name="columnName">The name of the column</param>
		/// <param name="initialValue">The initial value of the auto increment.</param>
		void SetAutoIncrementOnTableColumn(string tableName, string columnName, long initialValue);

		/// <summary>
		/// Drops an auto increment clause on a column.
		/// </summary>
		/// <param name="tableName">The name of the table containing the column.</param>
		/// <param name="columnName">The name of the column</param>
		void DropAutoIncrementOnTableColumn(string tableName, string columnName);

		/// <summary>
		/// Adds an auto timestamp clause on a column.
		/// </summary>
		/// <param name="tableName">The name of the table containing the column.</param>
		/// <param name="columnName">The name of the column</param>
		/// <param name="onInsert">Tells whether the value of the column must be updated on INSERT queries.</param>
		/// <param name="onUpdate">Tells whether the value of the column must be updated on UPDATE queries.</param>
		void SetAutoTimeStampOnTableColumn(string tableName, string columnName, bool onInsert, bool onUpdate);

		/// <summary>
		/// Drops an auto timestamp clause on a column.
		/// </summary>
		/// <param name="tableName">The name of the table containing the column.</param>
		/// <param name="columnName">The name of the column</param>
		void DropAutoTimeStampOnTableColumn(string tableName, string columnName);

		/// <summary>
		/// Create a new index.
		/// </summary>
		/// <param name="tableName">The name of the table on which to create the index.</param>
		/// <param name="index">The index to create.</param>
		void CreateIndex(string tableName, SqlIndex index);

		void SetTableComment(string tableName, string comment);

		/// <summary>
		/// Resets an index, that is, does whatever it takes to clean it in order to make it more
		/// efficient.
		/// </summary>
		/// <param name="index">The index to reset.</param>
		void ResetIndex(SqlIndex index);

		/// <summary>
		/// Drops an index.
		/// </summary>
		/// <param name="index">The index to drop.</param>
		void DropIndex(SqlIndex index);
		
		/// <summary>
		/// Selects data based on a query.
		/// </summary>
		/// <param name="query">The query.</param>
		void SelectData(SqlSelect query);

		/// <summary>
		/// Inserts data into a table, based on a collection of fields.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="fields">The fields.</param>
		void InsertData(string tableName, Collections.SqlFieldList fields);
		
		/// <summary>
		/// Inserts data into a table, based on a collection of fields and returns the values of
		/// another collection of fields.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="fieldsToInsert">The fields to insert.</param>
		/// <param name="fieldsToReturn">The fields whose value to return.</param>
		void InsertData(string tableName, Collections.SqlFieldList fieldsToInsert, Collections.SqlFieldList fieldsToReturn);
				
		/// <summary>
		/// Updates data from a table, based on a collection of fields and a
		/// set of conditions.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="fields">The fields.</param>
		/// <param name="conditions">The conditions.</param>
		void UpdateData(string tableName, Collections.SqlFieldList fields, Collections.SqlFieldList conditions);

		/// <summary>
		/// Removes the data from a table, based on a set of conditions.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="conditions">The conditions.</param>
		void RemoveData(string tableName, Collections.SqlFieldList conditions);

		/// <summary>
		/// Executes a stored procedure.
		/// </summary>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <param name="fields">The fields.</param>
		void ExecuteProcedure(string procedureName, Collections.SqlFieldList fields);

		/// <summary>
		/// Gets the value of the current time in the database.
		/// </summary>
		void GetCurrentTimeStamp();

		/// <summary>
		/// Gets the <see cref="SqlField"/> for the SQL expression that represents the current time
		/// in the database.
		/// </summary>
		/// <returns></returns>
		SqlField GetSqlFieldForCurrentTimeStamp();
		
		/// <summary>
		/// Sets the SQL parameters for a stored procedure.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="fields">The fields.</param>
		void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFieldList fields);
		
		/// <summary>
		/// Gets the SQL parameters for a stored procedure.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="fields">The fields.</param>
		void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFieldList fields);

		/// <summary>
		/// Sets a value for a parameterized command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="index">The parameter index.</param>
		/// <param name="value">The value.</param>
		void SetCommandParameterValue(System.Data.IDbCommand command, int index, object value);

		/// <summary>
		/// Gets a value for a parameterized command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="index">The parameter index.</param>
		/// <returns>The value.</returns>
		object GetCommandParameterValue(System.Data.IDbCommand command, int index);

		/// <summary>
		/// Gets the wildcards supported by the <c>LIKE</c> comparison.
		/// </summary>
		/// <returns>An array of characters which are recognized by the <c>LIKE</c>
		/// predicator as wildcard specifiers.</returns>
		char[] GetSupportedCompareLikeWildcards();

	}
}
