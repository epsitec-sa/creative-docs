//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>IDbAbstraction</c> interface gives access to ADO.NET through
	/// a provider specific abstraction layer.
	/// </summary>
	public interface IDbAbstraction : System.IDisposable
	{
		/// <summary>
		/// Gets the database abstraction factory.
		/// </summary>
		/// <value>The database abstraction factory.</value>
		IDbAbstractionFactory Factory
		{
			get;
		}

		/// <summary>
		/// Gets the database connection.
		/// </summary>
		/// <value>The database connection.</value>
		System.Data.IDbConnection Connection
		{
			get;
		}

		/// <summary>
		/// Gets the SQL builder.
		/// </summary>
		/// <value>The SQL builder.</value>
		ISqlBuilder SqlBuilder
		{
			get;
		}

		/// <summary>
		/// Gets the SQL engine.
		/// </summary>
		/// <value>The SQL engine.</value>
		ISqlEngine SqlEngine
		{
			get;
		}

		/// <summary>
		/// Gets the database service tools.
		/// </summary>
		/// <value>The database service tools.</value>
		IDbServiceTools ServiceTools
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the connection was properly initialized.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the connection was properly initialized; otherwise, <c>false</c>.
		/// </value>
		bool IsConnectionInitialized
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the connection is open.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the connection is open; otherwise, <c>false</c>.
		/// </value>
		bool IsConnectionOpen
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the connection is alive. This is the
		/// case if the connection is open and not in the broken state.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this connection is alive; otherwise, <c>false</c>.
		/// </value>
		bool IsConnectionAlive
		{
			get;
		}

		/// <summary>
		/// Queries the names for the user tables in the database.
		/// </summary>
		/// <returns>An array of names.</returns>
		string[] QueryUserTableNames();

		/// <summary>
		/// Queries the path of the database folder.
		/// </summary>
		/// <returns>The path of the database folder.</returns>
		string QueryDatabaseFolderPath();

		/// <summary>
		/// Creates a new database command object.
		/// </summary>
		/// <returns>A database command object.</returns>
		System.Data.IDbCommand NewDbCommand();

		/// <summary>
		/// Creates a new database adapter object for a given command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns>A database adapter object.</returns>
		System.Data.IDataAdapter NewDataAdapter(System.Data.IDbCommand command);

		/// <summary>
		/// Begins a read only transaction.
		/// </summary>
		/// <returns>The database transaction object.</returns>
		System.Data.IDbTransaction BeginReadOnlyTransaction();

		/// <summary>
		/// Begins a read only transaction that locks the given <see cref="DbTable"/> for shared
		/// read access.
		/// </summary>
		/// <param name="tablesToLock">The <see cref="DbTable"/> to lock.</param>
		/// <returns>The database transaction object.</returns>
		System.Data.IDbTransaction BeginReadOnlyTransaction(IEnumerable<DbTable> tablesToLock);
		
		/// <summary>
		/// Begins a read-write transaction.
		/// </summary>
		/// <returns>The database transaction object.</returns>
		System.Data.IDbTransaction BeginReadWriteTransaction();

		/// <summary>
		/// Begins a read-write transaction that locks the given <see cref="DbTable"/> for exclusive
		/// write access.
		/// </summary>
		/// <param name="tablesToLock">The <see cref="DbTable"/> to lock.</param>
		/// <returns>The database transaction object.</returns>
		System.Data.IDbTransaction BeginReadWriteTransaction(IEnumerable<DbTable> tablesToLock);

		/// <summary>
		/// Releases the connection.
		/// </summary>
		void ReleaseConnection();

		void DropDatabase();

	}
}
