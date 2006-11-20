//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
		/// <param name="tableName">Name of the table.</param>
		void RemoveTable(string tableName);
		
		/// <summary>
		/// Inserts columns into the table definition.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The columns.</param>
		void InsertTableColumns(string tableName, SqlColumn[] columns);

		/// <summary>
		/// Updates the columns in a table definition.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The columns.</param>
		void UpdateTableColumns(string tableName, SqlColumn[] columns);
		
		/// <summary>
		/// Removes the columns from a table definition.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="columns">The columns.</param>
		void RemoveTableColumns(string tableName, SqlColumn[] columns);

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
		void InsertData(string tableName, Collections.SqlFields fields);
		
		/// <summary>
		/// Updates data from a table, based on a collection of fields and a
		/// set of conditions.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="fields">The fields.</param>
		/// <param name="conditions">The conditions.</param>
		void UpdateData(string tableName, Collections.SqlFields fields, Collections.SqlFields conditions);

		/// <summary>
		/// Removes the data from a table, based on a set of conditions.
		/// </summary>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="conditions">The conditions.</param>
		void RemoveData(string tableName, Collections.SqlFields conditions);

		/// <summary>
		/// Executes a stored procedure.
		/// </summary>
		/// <param name="procedureName">Name of the procedure.</param>
		/// <param name="fields">The fields.</param>
		void ExecuteProcedure(string procedureName, Collections.SqlFields fields);

		/// <summary>
		/// Sets the SQL parameters for a stored procedure.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="fields">The fields.</param>
		void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);
		
		/// <summary>
		/// Gets the SQL parameters for a stored procedure.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="fields">The fields.</param>
		void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields);

		/// <summary>
		/// Sets a value for a parameterized command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="index">The parameter index.</param>
		/// <param name="value">The value.</param>
		void SetCommandParameterValue(System.Data.IDbCommand command, int index, object value);

		/// <summary>
		/// Gets a value for a parametrized command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="index">The parameter index.</param>
		/// <returns>The value.</returns>
		object GetCommandParameterValue(System.Data.IDbCommand command, int index);
	}
}
