using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbConnectionManager</c> class provides the low level tools required to manage an high
	/// level connection to the database.
	/// Basically, a connection contains an id which defines it, an identity which contains some
	/// information about it, a status (open, closed, interrupted). In addition, it contains two
	/// times, the times at which it was open, and the last time at which it has given some sign of
	/// life.
	/// A connection might be automatically closed by calls to the method InterruptDeadConnections if
	/// it has not given any sign of life with the KeepConnectionAlive method recently.
	/// </summary>
    public sealed class DbConnectionManager : DbAbstractAttachable
	{


		/// <summary>
		/// Creates a new <c>DbConnectionManager</c>.
		/// </summary>
		internal DbConnectionManager() : base ()
		{
		}


		/// <summary>
		/// Opens a new connection with the given identity.
		/// </summary>
		/// <param name="connectionIdentity">The identity that describes the connection.</param>
		/// <returns>The id allocated to the new connection.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionIdentity"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long OpenConnection(string connectionIdentity)
		{
			this.CheckIsAttached ();

			connectionIdentity.ThrowIfNullOrEmpty ("connectionIdentity");

			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			int status = (int) DbConnectionStatus.Open;

			SqlFieldList fieldsToInsert = new SqlFieldList ();

			DbColumn columnConnectionIdentity = this.DbTable.Columns[Tags.ColumnConnectionIdentity];
			DbColumn columnConnectionSince = this.DbTable.Columns[Tags.ColumnConnectionSince];
			DbColumn columnCounexionLastSeen = this.DbTable.Columns[Tags.ColumnConnectionLastSeen];
			DbColumn columnCounexionStatus = this.DbTable.Columns[Tags.ColumnConnectionStatus];

			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnectionIdentity, connectionIdentity));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnectionSince, databaseTime));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounexionLastSeen, databaseTime));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounexionStatus, status));

			return this.AddRow (fieldsToInsert);
		}


		/// <summary>
		/// Closes a given connection.
		/// </summary>
		/// <param name="connectionId">The id of the connection that must be closed.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void CloseConnection(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionStatus];
				int status = (int) DbConnectionStatus.Closed;

				SqlFieldList fields = new SqlFieldList ()
				{
					this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, status)
				};

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
				};

				int nbRowsAffected = this.SetRowValue (fields, conditions);

				transaction.Commit ();

				if (nbRowsAffected == 0)
				{
					throw new System.InvalidOperationException ("Could not close connection because it not open.");
				}
			}
		}


		/// <summary>
		/// Checks if the given connection exists.
		/// </summary>
		/// <param name="connectionId">The id of the connection.</param>
		/// <returns><c>true</c> if the connection exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public bool ConnectionExists(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnectionId (connectionId),
			};

			return this.RowExists (conditions);
		}


		/// <summary>
		/// Ensures that the given connection stays alive.
		/// </summary>
		/// <param name="connectionId">The id of the connection.</param>
		/// <returns><c>true</c> if the connection exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void KeepConnectionAlive(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionLastSeen];
				System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();

				SqlFieldList fields = new SqlFieldList ()
				{
					this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, databaseTime)
				};

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
				};

				int nbRowsAffected = this.SetRowValue (fields, conditions);

				transaction.Commit ();

				if (nbRowsAffected == 0)
				{
					throw new System.InvalidOperationException ("Could not keep connection alive because it is not open anymore.");
				}
			}
		}


		/// <summary>
		/// Sets the state of all open connections inactive for more than a given timeout value to
		/// interrupted.
		/// </summary>
		/// <param name="timeOutValue">The value after which an inactive open connection is considered as dead.</param>
		/// <returns><c>true</c> if at least one connection has been interrupted, <c>false</c> if none where interrupted.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public bool InterruptDeadConnections(System.TimeSpan timeOutValue)
		{
			this.CheckIsAttached ();

			DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionStatus];
			int status = (int) DbConnectionStatus.Interrupted;
			
			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, status)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
				this.CreateConditionForTimeOut (timeOutValue),
			};

			int nbRowsAffected = this.SetRowValue (fields, conditions);

			return nbRowsAffected > 0;
		}


		/// <summary>
		/// Gets the identity of a given connection.
		/// </summary>
		/// <param name="connectionId">The id of the connection</param>
		/// <returns>The identity of the connection.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public string GetConnectionIdentity(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionIdentity];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
				};

				object connectionIdentity = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (string) connectionIdentity;
			}
		}


		/// <summary>
		/// Gets the status of a given connection.
		/// </summary>
		/// <param name="connectionId">The id of the connection</param>
		/// <returns>The status of the connection.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public DbConnectionStatus GetConnectionStatus(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionStatus];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
				};

				object connectionStatus = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (DbConnectionStatus) connectionStatus;
			}
		}


		/// <summary>
		/// Gets the time at which a given connection was opened.
		/// </summary>
		/// <param name="connectionId">The id of the connection</param>
		/// <returns>The time at which the connection has been opened.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public System.DateTime GetConnectionSince(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionSince];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
				};

				object connectionSince = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (System.DateTime) connectionSince;
			}
		}


		/// <summary>
		/// Gets the last time at which a given connection has given some sign of life.
		/// </summary>
		/// <param name="connectionId">The id of the connection</param>
		/// <returns>The last time at which the connection has given sign of life.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public System.DateTime GetConnectionLastSeen(long connectionId)
		{
			this.CheckIsAttached ();

			connectionId.ThrowIf (cId => cId < 0, "connectionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnectionExists (connectionId))
				{
					throw new System.InvalidOperationException ("The connection does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnectionLastSeen];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionId (connectionId),
				};

				object connectionLastSeen = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (System.DateTime) connectionLastSeen;
			}
		}


		/// <summary>
		/// Gets the sequence of the ids of the connection that are open.
		/// </summary>
		/// <returns>The sequence of id of the open connections.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public IEnumerable<long> GetOpenConnectionIds()
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbColumn column = this.DbTable.Columns[Tags.ColumnId];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Open)
				};

				List<long> ids = this.GetRowsValue (column, conditions).Cast<long> ().ToList ();

				transaction.Commit ();

				return ids;
			}
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for the connections that are open.
		/// </summary>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		internal SqlFunction CreateConditionForOpenConnections()
		{
			this.CheckIsAttached ();

			return this.CreateConditionForConnectionStatus (DbConnectionStatus.Open);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection id.
		/// </summary>
		/// <param name="connectionId">The id of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForConnectionId(long connectionId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateConstant (connectionId, DbRawType.Int64)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection status.
		/// </summary>
		/// <param name="connectionStatus">The status of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForConnectionStatus(DbConnectionStatus connectionStatus)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnectionStatus].GetSqlName ()),
				SqlField.CreateConstant ((int) connectionStatus, DbRawType.Int32)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given timeout.
		/// </summary>
		/// <param name="timeOutValue">The time out value for the connections.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForTimeOut(System.TimeSpan timeOutValue)
		{
			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			System.DateTime timeOutTime = databaseTime - timeOutValue;
			
			return new SqlFunction
			(
				SqlFunctionCode.CompareLessThan,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnectionLastSeen].GetSqlName ()),
				SqlField.CreateConstant (timeOutTime, DbRawType.DateTime)
			);
		}
		

	}


}
