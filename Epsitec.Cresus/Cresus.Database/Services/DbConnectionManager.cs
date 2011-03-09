using Epsitec.Common.Types.Collections;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;
using System.Data;
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
    public sealed class DbConnectionManager : DbAbstractTableService
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Creates a new <c>DbConnectionManager</c>.
		/// </summary>
		internal DbConnectionManager(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableConnection;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableConnection);

			DbColumn columnId = new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};

			DbColumn columnConnectionIdentity = new DbColumn (Tags.ColumnConnectionIdentity, types.DefaultString, DbColumnClass.Data, DbElementCat.Internal);

			DbColumn columnEstablishmentTime = new DbColumn (Tags.ColumnEstablishmentTime, types.DateTime, DbColumnClass.Data, DbElementCat.Internal)
			{
				IsAutoTimeStampOnInsert = true
			};

			DbColumn columnRefreshTime = new DbColumn (Tags.ColumnRefreshTime, types.DateTime, DbColumnClass.Data, DbElementCat.Internal)
			{
				IsAutoTimeStampOnInsert = true,
				IsAutoTimeStampOnUpdate = true
			};

			DbColumn columnConnectionStatus = new DbColumn (Tags.ColumnConnectionStatus, types.DefaultInteger, DbColumnClass.Data, DbElementCat.Internal);
			table.Columns.Add (columnId);
			table.Columns.Add (columnConnectionIdentity);
			table.Columns.Add (columnEstablishmentTime);
			table.Columns.Add (columnRefreshTime);
			table.Columns.Add (columnConnectionStatus);

			table.DefineCategory (DbElementCat.Internal);
			
			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			return table;
		}


		/// <summary>
		/// Opens a new connection with the given identity.
		/// </summary>
		/// <param name="connectionIdentity">The identity that describes the connection.</param>
		/// <returns>The data of the new <see cref="DbConnection"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionIdentity"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public DbConnection OpenConnection(string connectionIdentity)
		{
			this.CheckIsTurnedOn ();

			connectionIdentity.ThrowIfNullOrEmpty ("connectionIdentity");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionIdentity, connectionIdentity },
				{ Tags.ColumnConnectionStatus, (int) DbConnectionStatus.Open },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return DbConnection.CreateDbConnection (data);
		}


		/// <summary>
		/// Closes a given connection.
		/// </summary>
		/// <param name="id">The id of the connection that must be closed.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="id"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void CloseConnection(DbId id)
		{
			this.CheckIsTurnedOn ();

			id.ThrowIf (cId => cId.Value < 0, "connectionId cannot be lower than zero.");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionStatus, (int) DbConnectionStatus.Closed },
			};
			
			
			SqlFunction[] conditions = new SqlFunction[]
			{
				this.CreateConditionForConnectionId (id),
				this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
			};

			int nbRowsAffected = this.SetRowValues (columnNamesToValues, conditions);

			if (nbRowsAffected == 0)
			{
				throw new System.InvalidOperationException ("Could not close connection because it not open or it does not exist.");
			}
		}


		/// <summary>
		/// Checks if the given connection exists.
		/// </summary>
		/// <param name="id">The id of the connection.</param>
		/// <returns><c>true</c> if the connection exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="id"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public bool ConnectionExists(DbId id)
		{
			this.CheckIsTurnedOn ();

			id.ThrowIf (cId => cId.Value < 0, "connectionId cannot be lower than zero.");

			SqlFunction condition = this.CreateConditionForConnectionId (id);

			return this.RowExists (condition);
		}


		/// <summary>
		/// Gets the data of the connection that have the given <see cref="DbId"/>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the connection to get.</param>
		/// <returns>The data of the connection.</returns>
		public DbConnection GetConnection(DbId id)
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForConnectionId (id);

			var data = this.GetRowValues (condition);

			return data.Any () ? DbConnection.CreateDbConnection (data[0]) : null;
		}


		/// <summary>
		/// Ensures that the given connection stays alive.
		/// </summary>
		/// <param name="id">The id of the connection.</param>
		/// <returns><c>true</c> if the connection exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="id"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void KeepConnectionAlive(DbId id)
		{
			this.CheckIsTurnedOn ();

			id.ThrowIf (cId => cId.Value < 0, "connectionId cannot be lower than zero.");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{Tags.ColumnConnectionStatus, (int) DbConnectionStatus.Open}
			};
			
			SqlFunction[] conditions = new SqlFunction []
			{
				this.CreateConditionForConnectionId (id),
				this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
			};

			int nbRowsAffected = this.SetRowValues (columnNamesToValues, conditions);

			if (nbRowsAffected == 0)
			{
				throw new System.InvalidOperationException ("Could not keep connection alive because it is not open or it does not exist.");
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
			this.CheckIsTurnedOn ();

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{Tags.ColumnConnectionStatus, (int) DbConnectionStatus.Interrupted}
			};
			
			SqlFunction[] conditions = new SqlFunction []
			{
				this.CreateConditionForConnectionStatus (DbConnectionStatus.Open),
				this.CreateConditionForTimeOut (timeOutValue),
			};

			int nbRowsAffected = this.SetRowValues (columnNamesToValues, conditions);

			return nbRowsAffected > 0;
		}


		/// <summary>
		/// Gets the sequence of the ids of the connection that are open.
		/// </summary>
		/// <returns>The sequence of id of the open connections.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public IEnumerable<DbConnection> GetOpenConnections()
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForConnectionStatus (DbConnectionStatus.Open);

			var data = this.GetRowValues (condition);

			return data.Select (d => DbConnection.CreateDbConnection (d));
		}


		/// <summary>
		/// Finds all the locks given by <paramref name="lockNames"/> and returns them as pairs of
		/// connection and lock.
		/// the connections that owns them.
		/// </summary>
		/// <param name="lockNames">The sequence of lock names.</param>
		/// <returns>The mapping between the connection and the lock they own.</returns>
		public IEnumerable<System.Tuple<DbConnection, DbLock>> GetLockOwners(IEnumerable<string> lockNames)
		{
			lockNames.ThrowIfNull ("lockNames");

			string[] arrayOfLockNames = lockNames.ToArray ();

			if (arrayOfLockNames.Length > 0)
			{
				SqlSelect query = this.CreateQueryForLockOwners (arrayOfLockNames);

				return this.ExecuteQueryForLockOwners (query);
			}
			else
			{
				return EmptyEnumerable<System.Tuple<DbConnection, DbLock>>.Instance;
			}
		}


		/// <summary>
		/// Builds the SQL query used to retrieve some locks with their connections.
		/// </summary>
		/// <param name="lockNames">The sequence of lock names.</param>
		/// <returns>The SQL query.</returns>
		private SqlSelect CreateQueryForLockOwners(string[] lockNames)
		{
			DbTable connectionTable = this.DbTable;
			DbColumn cIdColumn = connectionTable.Columns[Tags.ColumnId];
			DbColumn cIdentityColumn = connectionTable.Columns[Tags.ColumnConnectionIdentity];
			DbColumn cEstablishementTimeColumn = connectionTable.Columns[Tags.ColumnEstablishmentTime];
			DbColumn cRefreshTimeColumn = connectionTable.Columns[Tags.ColumnRefreshTime];
			DbColumn cStatusColumn = connectionTable.Columns[Tags.ColumnConnectionStatus];

			DbTable lockTable = this.DbInfrastructure.ResolveDbTable (Tags.TableLock);
			DbColumn lIdColumn = lockTable.Columns[Tags.ColumnId];
			DbColumn lNameColumn = lockTable.Columns[Tags.ColumnName];
			DbColumn lConnectionIdColumn = lockTable.Columns[Tags.ColumnConnectionId];
			DbColumn lCounterColumn = lockTable.Columns[Tags.ColumnCounter];
			DbColumn lCreationTimeColumn = lockTable.Columns[Tags.ColumnDateTime];

			SqlSelect query = new SqlSelect ();

			string cTableName = connectionTable.GetSqlName ();
			string cIdColumnName = cIdColumn.GetSqlName ();
			string cIdColumnAlias = "c" + cIdColumnName;
			string cIdentityColumnName = cIdentityColumn.GetSqlName ();
			string cEstablishementTimeColumnName = cEstablishementTimeColumn.GetSqlName ();
			string cRefreshTimeColumnName = cRefreshTimeColumn.GetSqlName ();
			string cStatusColumnName = cStatusColumn.GetSqlName ();

			string lTableName = lockTable.GetSqlName ();
			string lIdColumnName = lIdColumn.GetSqlName ();
			string lIdColumnAlias = "l" + lIdColumnName;
			string lConnectionIdColumnName = lConnectionIdColumn.GetSqlName ();
			string lNameColumnName = lNameColumn.GetSqlName ();
			string lCounterColumnName = lCounterColumn.GetSqlName ();
			string lCreationTimeColumnName = lCreationTimeColumn.GetSqlName ();

			query.Tables.Add (SqlField.CreateAliasedName (cTableName, "c"));
			query.Tables.Add (SqlField.CreateAliasedName (lTableName, "l"));

			query.Joins.Add
			(
				SqlField.CreateJoin
				(
					new SqlJoin
					(
						SqlField.CreateAliasedName ("c", cIdColumnName, cIdColumnAlias),
						SqlField.CreateAliasedName ("l", lConnectionIdColumnName, lConnectionIdColumnName),
						SqlJoinCode.Inner,
						new SqlFieldList ()
						{
                			SqlField.CreateFunction
							(
								new SqlFunction
								(
                					SqlFunctionCode.SetIn,
                					SqlField.CreateAliasedName("l", lNameColumnName, lNameColumnName),
                					SqlField.CreateSet(new SqlSet(DbRawType.String, lockNames))
                				)
							)
						}
					)
				)
			);

			query.Fields.Add (SqlField.CreateAliasedName ("c", cIdColumnName, cIdColumnAlias));
			query.Fields.Add (SqlField.CreateAliasedName ("c", cIdentityColumnName, cIdentityColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("c", cEstablishementTimeColumnName, cEstablishementTimeColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("c", cRefreshTimeColumnName, cRefreshTimeColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("c", cStatusColumnName, cStatusColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("l", lIdColumnName, lIdColumnAlias));
			query.Fields.Add (SqlField.CreateAliasedName ("l", lNameColumnName, lNameColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("l", lConnectionIdColumnName, lConnectionIdColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("l", lCounterColumnName, lCounterColumnName));
			query.Fields.Add (SqlField.CreateAliasedName ("l", lCreationTimeColumnName, lCreationTimeColumnName));
			return query;
		}


		/// <summary>
		/// Executes the request that fetches some locks with their connection.
		/// </summary>
		/// <param name="query">The query to execute.</param>
		/// <returns>The mapping from the connection to the locks they own.</returns>
		private IEnumerable<System.Tuple<DbConnection, DbLock>> ExecuteQueryForLockOwners(SqlSelect query)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				transaction.SqlBuilder.SelectData (query);

				DataSet data = this.DbInfrastructure.ExecuteRetData (transaction);

				transaction.Commit ();

				var connections = new Dictionary<DbId, DbConnection> ();

				if (data.Tables.Count > 0)
				{
					foreach (DataRow row in data.Tables[0].Rows)
					{
						DbConnection dbConnection;

						DbLock dbLock         = DbLock.CreateLock (row.ItemArray.Skip (5).ToList ());
						DbId   dbConnectionId = dbLock.ConnectionId;

						if (connections.TryGetValue (dbConnectionId, out dbConnection))
                        {
							//	Reuse same connection object.
                        }
						else
						{
							dbConnection = DbConnection.CreateDbConnection (row.ItemArray.Take (5).ToList ());
							connections[dbConnectionId] = dbConnection;
						}

						yield return new System.Tuple<DbConnection, DbLock> (dbConnection, dbLock);
					}
				}
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
			this.CheckIsTurnedOn ();

			return this.CreateConditionForConnectionStatus (DbConnectionStatus.Open);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection id.
		/// </summary>
		/// <param name="id">The id of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForConnectionId(DbId id)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateConstant (id.Value, DbRawType.Int64)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection status.
		/// </summary>
		/// <param name="status">The status of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForConnectionStatus(DbConnectionStatus status)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnectionStatus].GetSqlName ()),
				SqlField.CreateConstant ((int) status, DbRawType.Int32)
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
			return new SqlFunction
			(
				SqlFunctionCode.CompareGreaterThan,
				SqlField.CreateFunction
				(
					new SqlFunction
					(
						SqlFunctionCode.MathSubstract,
						this.DbInfrastructure.DefaultSqlBuilder.GetSqlFieldForCurrentTimeStamp (),
						SqlField.CreateName (this.DbTable.Columns[Tags.ColumnRefreshTime].GetSqlName ())		
					)
				),
				SqlField.CreateConstant (timeOutValue.TotalDays, DbRawType.SmallDecimal)
			);
		}


	}


}
