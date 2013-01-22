//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>ConnectionManager</c> class provides the low level tools required to manage an high
	/// level connection to the database.
	/// Basically, a connection contains an id which defines it, an identity which contains some
	/// information about it, a status (open, closed, interrupted). In addition, it contains two
	/// times, the times at which it was open, and the last time at which it has given some sign of
	/// life.
	/// A connection might be automatically closed by calls to the methodKillDeadConnections if
	/// it has not given any sign of life with the KeepConnectionAlive method recently.
	/// </summary>
	internal sealed class ConnectionManager
	{


		// TODO Comment this class
		// Marc


		/// <summary>
		/// Creates a new <c>ConnectionManager</c>.
		/// </summary>
		public ConnectionManager(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var table = schemaEngine.GetServiceTable (ConnectionManager.TableFactory.TableName);
			var tableQueryHelper = new TableQueryHelper (dbInfrastructure, table);

			this.table = table;
			this.dbInfrastructure = dbInfrastructure;
			this.tableQueryHelper = tableQueryHelper;
		}


		/// <summary>
		/// Opens a new connection with the given identity.
		/// </summary>
		/// <param name="connectionIdentity">The identity that describes the connection.</param>
		/// <returns>The data of the new <see cref="Connection"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionIdentity"/> is <c>null</c> or empty.</exception>
		public Connection OpenConnection(string connectionIdentity)
		{
			connectionIdentity.ThrowIfNullOrEmpty ("connectionIdentity");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ ConnectionManager.TableFactory.ColumnIdentityName, connectionIdentity },
				{ ConnectionManager.TableFactory.ColumnStatusName, (int) ConnectionStatus.Open },
			};

			IList<object> data = this.tableQueryHelper.AddRow (columnNamesToValues);

			return this.CreateConnection (data);
		}


		/// <summary>
		/// Closes a given connection.
		/// </summary>
		/// <param name="id">The id of the connection that must be closed.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="id"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void CloseConnection(DbId id)
		{
			id.ThrowIf (cId => cId.IsEmpty, "id cannot be empty.");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ ConnectionManager.TableFactory.ColumnStatusName, (int) ConnectionStatus.Closed },
			};			
			
			SqlFunction[] conditions = new SqlFunction[]
			{
				this.CreateConditionForId (id),
				this.CreateConditionForStatus (ConnectionStatus.Open),
			};

			int nbRowsAffected;

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				nbRowsAffected = this.tableQueryHelper.SetRow (columnNamesToValues, conditions);

				transaction.Commit ();
			}
	
			if (nbRowsAffected == 0)
			{
				throw new System.InvalidOperationException ("Could not close connection because it not open or it does not exist.");
			}
		}


		/// <summary>
		/// Gets the data of the connection that have the given <see cref="DbId"/>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the connection to get.</param>
		/// <returns>The data of the connection.</returns>
		public Connection GetConnection(DbId id)
		{
			id.ThrowIf (cId => cId.IsEmpty, "id cannot be empty.");

			SqlFunction condition = this.CreateConditionForId (id);

			var data = this.tableQueryHelper.GetRows (condition);

			return data.Any () ? this.CreateConnection (data[0]) : null;
		}


		/// <summary>
		/// Ensures that the given connection stays alive.
		/// </summary>
		/// <param name="id">The id of the connection.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="id"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection does not exists.</exception>
		/// <exception cref="System.InvalidOperationException">If the connection is not open.</exception>
		public void KeepConnectionAlive(DbId id)
		{
			id.ThrowIf (cId => cId.IsEmpty, "id cannot be empty.");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ConnectionManager.TableFactory.ColumnStatusName, (int) ConnectionStatus.Open}
			};
			
			SqlFunction[] conditions = new SqlFunction []
			{
				this.CreateConditionForId (id),
				this.CreateConditionForStatus (ConnectionStatus.Open),
			};

			int nbRowsAffected;

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				nbRowsAffected = this.tableQueryHelper.SetRow (columnNamesToValues, conditions);

				transaction.Commit ();
			}

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
		public bool KillDeadConnections(System.TimeSpan timeOutValue)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ConnectionManager.TableFactory.ColumnStatusName, (int) ConnectionStatus.Interrupted}
			};
			
			SqlFunction[] conditions = new SqlFunction []
			{
				this.CreateConditionForStatus (ConnectionStatus.Open),
				this.CreateConditionForTimeOut (timeOutValue),
			};

			int nbRowsAffected;

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				nbRowsAffected = this.tableQueryHelper.SetRow (columnNamesToValues, conditions);

				transaction.Commit ();
			}

			return nbRowsAffected > 0;
		}


		/// <summary>
		/// Gets the data of the connection that are open.
		/// </summary>
		/// <returns>The data of the open connections.</returns>
		public IEnumerable<Connection> GetOpenConnections()
		{
			SqlFunction condition = this.CreateConditionForStatus (ConnectionStatus.Open);

			var data = this.tableQueryHelper.GetRows (condition);

			return data.Select (d => this.CreateConnection (d));
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection id.
		/// </summary>
		/// <param name="id">The id of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForId(DbId id)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[ConnectionManager.TableFactory.ColumnIdName].GetSqlName ()),
				SqlField.CreateConstant (id.Value, DbRawType.Int64)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given connection status.
		/// </summary>
		/// <param name="status">The status of the connection.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForStatus(ConnectionStatus status)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[ConnectionManager.TableFactory.ColumnStatusName].GetSqlName ()),
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
						this.dbInfrastructure.DefaultSqlBuilder.GetSqlFieldForCurrentTimeStamp (),
						SqlField.CreateName (this.table.Columns[ConnectionManager.TableFactory.ColumnRefreshTimeName].GetSqlName ())		
					)
				),
				SqlField.CreateConstant (timeOutValue.TotalDays, DbRawType.SmallDecimal)
			);
		}


		/// <summary>
		/// Builds a new instance of <see cref="Connection"/> given the data of a connection.
		/// </summary>
		/// <param name="data">The data of the connection.</param>
		/// <returns>The new instance of <see cref="Connection"/>.</returns>
		private Connection CreateConnection(IList<object> data)
		{
			DbId id = new DbId ((long) data[0]);
			string identity = (string) data[1];
			System.DateTime establishementTime = (System.DateTime) data[2];
			System.DateTime refreshTime = (System.DateTime) data[3];
			ConnectionStatus status = (ConnectionStatus) data[4];

			return new Connection (id, identity, status, establishementTime, refreshTime);
		}


		private readonly DbTable table;


		private readonly DbInfrastructure dbInfrastructure;


		private readonly TableQueryHelper tableQueryHelper;


		public static TableBuilder TableFactory
		{
			get
			{
				return ConnectionManager.tableFactory;
			}
		}


		private static readonly TableBuilder tableFactory = new TableBuilder ();


		public class TableBuilder : ITableFactory
		{


			#region ITableHelper Members


			public string TableName
			{
				get
				{
					return "CR_CONNECTION";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);
				DbTypeDef typeString = new DbTypeDef (StringType.Default);
				DbTypeDef typeDateTime = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Other.DateTime);
				DbTypeDef typeInteger = new DbTypeDef (IntegerType.Default);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId = new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnConnectionIdentity = new DbColumn (this.ColumnIdentityName, typeString, DbColumnClass.Data, DbElementCat.Internal);

				DbColumn columnEstablishmentTime = new DbColumn (this.ColumnEstablishmentTimeName, typeDateTime, DbColumnClass.Data, DbElementCat.Internal)
				{
					IsAutoTimeStampOnInsert = true
				};

				DbColumn columnRefreshTime = new DbColumn (this.ColumnRefreshTimeName, typeDateTime, DbColumnClass.Data, DbElementCat.Internal)
				{
					IsAutoTimeStampOnInsert = true,
					IsAutoTimeStampOnUpdate = true
				};

				DbColumn columnConnectionStatus = new DbColumn (this.ColumnStatusName, typeInteger, DbColumnClass.Data, DbElementCat.Internal);
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


			#endregion


			public string ColumnIdName
			{
				get
				{
					return "CR_ID";
				}
			}


			public string ColumnIdentityName
			{
				get
				{
					return "CR_IDENTITY";
				}
			}


			public string ColumnEstablishmentTimeName
			{
				get
				{
					return "CR_ESTB_TIME";
				}
			}


			public string ColumnRefreshTimeName
			{
				get
				{
					return "CR_RFR_TIME";
				}
			}


			public string ColumnStatusName
			{
				get
				{
					return "CR_STATUS";
				}
			}


		}


	}


}
