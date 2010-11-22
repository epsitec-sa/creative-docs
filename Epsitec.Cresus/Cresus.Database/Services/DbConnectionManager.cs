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
		/// <returns>The data of the new <see cref="DbConnection"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionIdentity"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public DbConnection OpenConnection(string connectionIdentity)
		{
			this.CheckIsAttached ();

			connectionIdentity.ThrowIfNullOrEmpty ("connectionIdentity");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionIdentity, connectionIdentity },
				{ Tags.ColumnConnectionStatus, (int) DbConnectionStatus.Open },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return this.CreateDbConnection (data);
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
			this.CheckIsAttached ();

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
			this.CheckIsAttached ();

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
			this.CheckIsAttached ();

			SqlFunction condition = this.CreateConditionForConnectionId (id);

			var data = this.GetRowValues (condition);

			return data.Any () ? this.CreateDbConnection (data[0]) : null;
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
			this.CheckIsAttached ();

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
			this.CheckIsAttached ();

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
			this.CheckIsAttached ();

			SqlFunction condition = this.CreateConditionForConnectionStatus (DbConnectionStatus.Open);

			var data = this.GetRowValues (condition);

			return data.Select (d => this.CreateDbConnection (d));
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


		/// <summary>
		/// Builds a new instance of <see cref="DbConnection"/> given the data of a connection.
		/// </summary>
		/// <param name="data">The data of the connection.</param>
		/// <returns>The new instance of <see cref="DbConnection"/>.</returns>
		private DbConnection CreateDbConnection(IList<object> data)
		{
			DbId id = new DbId ((long) data[0]);
			string identity = (string) data[1];
			System.DateTime establishementTime = (System.DateTime) data[2];
			System.DateTime refreshTime = (System.DateTime) data[3];
			DbConnectionStatus status = (DbConnectionStatus) data[4];

			return new DbConnection (id, identity, status, establishementTime, refreshTime);
		}
		

	}


}
