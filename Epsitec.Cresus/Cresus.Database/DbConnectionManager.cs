using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{


    public sealed class DbConnectionManager : DbAbstractAttachable
	{


		// TODO Comment this class.
		// Marc

		
		public DbConnectionManager(System.TimeSpan timeOutValue) : base ()
		{
			this.TimeOutValue = timeOutValue;
		}


		public System.TimeSpan TimeOutValue
		{
			get;
			private set;
		}


		public long OpenConnection(string connectionIdentity)
		{
			this.CheckIsAttached ();

			connectionIdentity.ThrowIfNullOrEmpty ("connectionIdentity");

			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			int status = (int) DbConnectionStatus.Opened;

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
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Opened),
				};

				int nbRowsAffected = this.SetRowValue (fields, conditions);

				transaction.Commit ();

				if (nbRowsAffected == 0)
				{
					throw new System.InvalidOperationException ("Could not close connection because it not open.");
				}
			}
		}


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
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Opened),
				};

				int nbRowsAffected = this.SetRowValue (fields, conditions);

				transaction.Commit ();

				if (nbRowsAffected == 0)
				{
					throw new System.InvalidOperationException ("Could not keep connection alive because it is not open anymore.");
				}
			}
		}


		public bool InterruptDeadConnections()
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
				this.CreateConditionForConnectionStatus (DbConnectionStatus.Opened),
				this.CreateConditionForTimeOut (),
			};

			int nbRowsAffected = this.SetRowValue (fields, conditions);

			return nbRowsAffected > 0;
		}


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


		public IEnumerable<long> GetOpenedConnectionIds()
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbColumn column = this.DbTable.Columns[Tags.ColumnId];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnectionStatus (DbConnectionStatus.Opened)
				};

				List<long> ids = this.GetRowsValue (column, conditions).Cast<long> ().ToList ();

				transaction.Commit ();

				return ids;
			}
		}


		internal SqlFunction CreateConditionForOpenedConnections()
		{
			this.CheckIsAttached ();

			return this.CreateConditionForConnectionStatus (DbConnectionStatus.Opened);
		}


		private SqlFunction CreateConditionForConnectionId(long connectionId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateConstant (connectionId, DbRawType.Int64)
			);
		}


		private SqlFunction CreateConditionForConnectionStatus(DbConnectionStatus connectionStatus)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnectionStatus].GetSqlName ()),
				SqlField.CreateConstant ((int) connectionStatus, DbRawType.Int32)
			);
		}


		private SqlFunction CreateConditionForTimeOut()
		{
			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			System.DateTime timeOutTime = databaseTime - this.TimeOutValue;
			
			return new SqlFunction
			(
				SqlFunctionCode.CompareLessThan,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnectionLastSeen].GetSqlName ()),
				SqlField.CreateConstant (timeOutTime, DbRawType.DateTime)
			);
		}
		

	}


}
