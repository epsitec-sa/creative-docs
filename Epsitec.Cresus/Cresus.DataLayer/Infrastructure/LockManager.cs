//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Data;

using System.Linq;
using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>LockManager</c> class provides tools required to interact with the locks in the
	/// database.
	/// A lock is associated to a connection as defined by <see cref="ConnectionManager"/>
	/// and is defined by a name. In addition, a lock contains a value which tells how many time it
	/// has been acquired by the user, which makes it reentrant.
	/// </summary>
	internal sealed class LockManager
	{
		
		
		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Creates a new <c>LockManager</c>.
		/// </summary>
		public LockManager(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var tableLock = schemaEngine.GetServiceTable (LockManager.TableFactory.TableName);
			var tableConnection = schemaEngine.GetServiceTable (ConnectionManager.TableFactory.TableName);
			var tableQueryHelper = new TableQueryHelper (dbInfrastructure, tableLock);

			this.tableLock = tableLock;
			this.tableConnection = tableConnection;
			this.dbInfrastructure = dbInfrastructure;
			this.tableLockQueryHelper = tableQueryHelper;
		}


		public System.Tuple<bool, IEnumerable<Lock>> RequestLocks(DbId connectionId, IList<string> lockNames)
		{
			connectionId.ThrowIf (cId => cId.IsEmpty, "connectionId cannot be empty");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Count == 0, "lockNames is empty.");
			lockNames.ThrowIf (names => names.Any (string.IsNullOrEmpty), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count != names.Distinct ().Count (), "lockNames cannot contain duplicates.");

			using (DbTransaction transaction = this.tableLockQueryHelper.CreateLockTransaction ())
			{
				var locks = this.GetLocks (lockNames);

				var unavailableLocks = locks.Where (l => l.Owner.Id != connectionId).ToList ();

				bool canLock = unavailableLocks.IsEmpty ();

				if (canLock)
				{
					foreach (string lockName in lockNames.Where (n => locks.All (l => l.Name != n)))
					{
						this.CreateLock (connectionId, lockName);
					}

					this.IncrementLockCounter (locks);
				}

				transaction.Commit ();

				return System.Tuple.Create (canLock, (IEnumerable<Lock>) unavailableLocks);
			}
		}


		public void ReleaseLocks(DbId connectionId, IList<string> lockNames)
		{
			connectionId.ThrowIf (cId => cId.IsEmpty, "connectionId cannot be empty");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Count == 0, "lockNames is empty.");
			lockNames.ThrowIf (names => names.Any (string.IsNullOrEmpty), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count != names.Distinct ().Count (), "lockNames cannot contain duplicates.");

			using (DbTransaction transaction = this.tableLockQueryHelper.CreateLockTransaction ())
			{
				var locks = this.GetLocks (lockNames).ToList ();

				var ownedLocks = locks.Where (l => l.Owner.Id == connectionId);

				if (lockNames.Count != ownedLocks.Count ())
				{
					throw new System.InvalidOperationException ("One or more locks is not owned by the given connection");
				}

				this.DeleteLockWithCounterAtZero (locks);
				this.DecrementLockCounter (locks);

				transaction.Commit ();
			}
		}


		public void ReleaseOwnedLocks(DbId connectionId, IList<string> lockNames)
		{
			connectionId.ThrowIf (cId => cId.IsEmpty, "connectionId cannot be empty");
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Count == 0, "lockNames is empty.");
			lockNames.ThrowIf (names => names.Any (string.IsNullOrEmpty), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count != names.Distinct ().Count (), "lockNames cannot contain duplicates.");

			using (DbTransaction transaction = this.tableLockQueryHelper.CreateLockTransaction ())
			{
				var locks = this.GetLocks (lockNames).Where (l => l.Owner.Id == connectionId).ToList ();

				this.DeleteLockWithCounterAtZero (locks);
				this.DecrementLockCounter (locks);

				transaction.Commit ();
			}
		}


		public IList<Lock> GetLocks(IList<string> lockNames)
		{
			lockNames.ThrowIfNull ("lockNames");
			lockNames.ThrowIf (names => names.Count == 0, "lockNames is empty.");
			lockNames.ThrowIf (names => names.Any (string.IsNullOrEmpty), "lock names cannot be null or empty.");
			lockNames.ThrowIf (names => names.Count != names.Distinct ().Count (), "lockNames cannot contain duplicates.");

			List<Lock> locks = new List<Lock> ();

			if (lockNames.Count > 0)
			{
				DbColumn cIdColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnIdName];
				DbColumn cIdentityColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnIdentityName];
				DbColumn cEstablishementTimeColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnEstablishmentTimeName];
				DbColumn cRefreshTimeColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnRefreshTimeName];
				DbColumn cStatusColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnStatusName];

				DbColumn lNameColumn = this.tableLock.Columns[LockManager.TableFactory.ColumnNameName];
				DbColumn lConnectionIdColumn = this.tableLock.Columns[LockManager.TableFactory.ColumnConnectionIdName];
				DbColumn lCounterColumn = this.tableLock.Columns[LockManager.TableFactory.ColumnCounterName];
				DbColumn lCreationTimeColumn = this.tableLock.Columns[LockManager.TableFactory.ColumnTimeName];

				SqlSelect query = new SqlSelect ();

				string cTableName = this.tableConnection.GetSqlName ();
				string cIdColumnName = cIdColumn.GetSqlName ();
				string cIdentityColumnName = cIdentityColumn.GetSqlName ();
				string cEstablishementTimeColumnName = cEstablishementTimeColumn.GetSqlName ();
				string cRefreshTimeColumnName = cRefreshTimeColumn.GetSqlName ();
				string cStatusColumnName = cStatusColumn.GetSqlName ();

				string lTableName = this.tableLock.GetSqlName ();
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
							SqlField.CreateAliasedName ("c", cIdColumnName, cIdColumnName),
							SqlField.CreateAliasedName ("l", lConnectionIdColumnName, lConnectionIdColumnName),
							SqlJoinCode.Inner
						)
					)
				);

				query.Conditions.Add
				(
					SqlField.CreateFunction
					(
						new SqlFunction
						(
							SqlFunctionCode.SetIn,
							SqlField.CreateAliasedName ("l", lNameColumnName, lNameColumnName),
							SqlField.CreateSet (new SqlSet (DbRawType.String, lockNames))
						)
					)
				);

				query.Fields.Add (SqlField.CreateAliasedName ("c", cIdColumnName, cIdColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("c", cIdentityColumnName, cIdentityColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("c", cEstablishementTimeColumnName, cEstablishementTimeColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("c", cRefreshTimeColumnName, cRefreshTimeColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("c", cStatusColumnName, cStatusColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("l", lNameColumnName, lNameColumnName));
				query.Fields.Add (SqlField.CreateAliasedName ("l", lCreationTimeColumnName, lCreationTimeColumnName));

				using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
				{
					transaction.SqlBuilder.SelectData (query);

					DataSet data = this.dbInfrastructure.ExecuteRetData (transaction);

					transaction.Commit ();

					var connections = new Dictionary<DbId, Connection> ();

					foreach (DataRow row in data.Tables[0].Rows)
					{
						DbId connectionId = new DbId ((long) row.ItemArray[0]);

						if (!connections.ContainsKey (connectionId))
						{
							string connectionIdentity = (string) row.ItemArray[1];
							System.DateTime connectionEstablishementTime = (System.DateTime) row.ItemArray[2];
							System.DateTime connectionRefreshTime = (System.DateTime) row.ItemArray[3];
							ConnectionStatus connectionStatus = (ConnectionStatus) row.ItemArray[4];

							Connection connection = new Connection (connectionId, connectionIdentity, connectionStatus, connectionEstablishementTime, connectionRefreshTime);
							connections[connectionId] = connection;
						}
					}

					foreach (DataRow row in data.Tables[0].Rows)
					{
						DbId connectionId = new DbId ((long) row.ItemArray[0]);
						Connection lockOwner = connections[connectionId];

						string lockName = (string) row.ItemArray[5];
						System.DateTime lockCreationTime = (System.DateTime) row.ItemArray[6];

						Lock l = new Lock (lockOwner, lockName, lockCreationTime);

						locks.Add (l);
					}
				}

			}

			return locks;
		}


		public void KillDeadLocks()
		{
			SqlFunction condition = this.CreateConditionForInactiveLocks ();

			this.tableLockQueryHelper.RemoveRows (condition);
		}


		private void CreateLock(long connectionId, string lockName)
		{
			IDictionary<string, object> columnNameToValues = new Dictionary<string, object> ()
			{
			    {LockManager.TableFactory.ColumnNameName, lockName},
			    {LockManager.TableFactory.ColumnConnectionIdName, connectionId},
			    {LockManager.TableFactory.ColumnCounterName, 0},
			};

			this.tableLockQueryHelper.AddRow (columnNameToValues);
		}


		private void DeleteLockWithCounterAtZero(IList<Lock> locks)
		{
			if (locks.Any ())
			{
				SqlFunction[] conditions = new SqlFunction[]
				{
					this.CreateConditionForLockNames (locks),
					this.CreateConditionForLockCounterAtZero (),
				};

				this.tableLockQueryHelper.RemoveRows (conditions);
			}
		}


		private void IncrementLockCounter(IList<Lock> locks)
		{
			this.UpdateLockCounter (locks, SqlFunctionCode.MathAdd);
		}


		private void DecrementLockCounter(IList<Lock> locks)
		{
			this.UpdateLockCounter (locks, SqlFunctionCode.MathSubstract);
		}


		private void UpdateLockCounter(IList<Lock> locks, SqlFunctionCode functionCode)
		{
			if (locks.Any ())
			{
				using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
				{
					SqlFieldList fields = new SqlFieldList ();

					SqlField sqlField = SqlField.CreateFunction
					(
						new SqlFunction
						(
							functionCode,
							SqlField.CreateName (this.tableLock.Columns[LockManager.TableFactory.ColumnCounterName]),
							SqlField.CreateConstant (1, DbRawType.Int32)
						)
					);

					sqlField.Alias = this.tableLock.Columns[LockManager.TableFactory.ColumnCounterName].GetSqlName ();

					fields.Add (sqlField);

					SqlFieldList c = new SqlFieldList ();

					c.Add (SqlField.CreateFunction (this.CreateConditionForLockNames (locks)));

					transaction.SqlBuilder.UpdateData (this.tableLock.GetSqlName (), fields, c);

					object nbRowsAffected = this.dbInfrastructure.ExecuteNonQuery (transaction);

					transaction.Commit ();
				}
			}
		}


		private SqlFunction CreateConditionForLockCounterAtZero()
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.tableLock.Columns[LockManager.TableFactory.ColumnCounterName].GetSqlName ()),
				SqlField.CreateConstant (0, DbRawType.Int32)
			);
		}


		private SqlFunction CreateConditionForLockNames(IEnumerable<Lock> locks)
		{
			return new SqlFunction
			(
				SqlFunctionCode.SetIn,
				SqlField.CreateName (this.tableLock.Columns[LockManager.TableFactory.ColumnNameName].GetSqlName ()),
				SqlField.CreateSet (new SqlSet (DbRawType.String, locks.Select (l => l.Name)))
			);
		}


		private SqlFunction CreateConditionForInactiveLocks()
		{
			SqlSelect queryForOpenConnectionIds = new SqlSelect ();

			DbColumn connectionIdColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnIdName];
			DbColumn connectionStatusColumn = this.tableConnection.Columns[ConnectionManager.TableFactory.ColumnStatusName];

			queryForOpenConnectionIds.Tables.Add ("t", SqlField.CreateName (this.tableConnection.GetSqlName ()));
			queryForOpenConnectionIds.Fields.Add ("c", SqlField.CreateName ("t", connectionIdColumn.GetSqlName ()));
			queryForOpenConnectionIds.Conditions.Add
			(
				new SqlFunction
				(
					SqlFunctionCode.CompareEqual,
					SqlField.CreateName (connectionStatusColumn.GetSqlName ()),
					SqlField.CreateConstant ((int) ConnectionStatus.Open, DbRawType.Int32)
				)
			);

			return new SqlFunction
			(
				SqlFunctionCode.SetNotIn,
				SqlField.CreateName (this.tableLock.Columns[LockManager.TableFactory.ColumnIdName].GetSqlName ()),
				SqlField.CreateSubQuery (queryForOpenConnectionIds)
			);
		}


		private readonly DbTable tableLock;


		private readonly DbTable tableConnection ;


		private readonly DbInfrastructure dbInfrastructure;


		private readonly TableQueryHelper tableLockQueryHelper;



		public static TableBuilder TableFactory
		{
			get
			{
				return LockManager.tableFactory;
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
					return "CR_LOCK";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);
				DbTypeDef typeDateTime = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Other.DateTime);
				DbTypeDef typeInteger = new DbTypeDef (IntegerType.Default);
				DbTypeDef typeName = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Str.Name);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId = new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnName = new DbColumn (this.ColumnNameName, typeName, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnConnectionId = new DbColumn (this.ColumnConnectionIdName, typeKeyId, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnCounter = new DbColumn (this.ColumnCounterName, typeInteger, DbColumnClass.Data, DbElementCat.Internal);

				DbColumn columnTime = new DbColumn (this.ColumnTimeName, typeDateTime, DbColumnClass.Data, DbElementCat.Internal)
				{
					IsAutoTimeStampOnInsert = true
				};

				table.Columns.Add (columnId);
				table.Columns.Add (columnName);
				table.Columns.Add (columnConnectionId);
				table.Columns.Add (columnCounter);
				table.Columns.Add (columnTime);

				table.DefineCategory (DbElementCat.Internal);

				table.DefinePrimaryKey (columnId);
				table.UpdatePrimaryKeyInfo ();

				table.AddIndex ("IDX_LOCK_NAME", SqlSortOrder.Ascending, columnName);
				table.AddIndex ("IDX_LOCK_CONNECTION", SqlSortOrder.Ascending, columnConnectionId);

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


			public string ColumnNameName
			{
				get
				{
					return "CR_NAME";
				}
			}


			public string ColumnConnectionIdName
			{
				get
				{
					return "CR_CONNECTION_ID";
				}
			}


			public string ColumnCounterName
			{
				get
				{
					return "CR_COUNTER";
				}
			}


			public string ColumnTimeName
			{
				get
				{
					return "CR_TIME";
				}
			}


		}


	}


}
