using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbLockManager</c> class provides the low level tools required to interact with the
	/// high level locks.
	/// An high level lock is associated to a connection as defined by <see cref="DbConnectionManager"/>
	/// and is defined by a name. In addition, a lock contains a value which tells how many time it
	/// has been acquired by the user, which makes it reentrant.
	/// </summary>
	public sealed class DbLockManager : DbAbstractTableService
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Creates a new <c>DbLockManager</c>.
		/// </summary>
		internal DbLockManager(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableLock;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableLock);

			DbColumn columnId = new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};
			
			DbColumn columnName = new DbColumn (Tags.ColumnName, types.Name, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnConnectionId = new DbColumn (Tags.ColumnConnectionId, types.KeyId, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnCounter = new DbColumn (Tags.ColumnCounter, types.DefaultInteger, DbColumnClass.Data, DbElementCat.Internal);
			
			DbColumn columnDateTime = new DbColumn (Tags.ColumnDateTime, types.DateTime, DbColumnClass.Data, DbElementCat.Internal)
			{
				IsAutoTimeStampOnInsert = true
			};

			table.Columns.Add (columnId);
			table.Columns.Add (columnName);
			table.Columns.Add (columnConnectionId);
			table.Columns.Add (columnCounter);
			table.Columns.Add (columnDateTime);

			table.DefineCategory (DbElementCat.Internal);
			
			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			table.AddIndex ("IDX_LOCK_NAME", SqlSortOrder.Ascending, columnName);
			table.AddIndex ("IDX_LOCK_CONNECTION", SqlSortOrder.Ascending, columnConnectionId);

			return table;
		}


		/// <summary>
		/// Request a given lock for a given connectionId. If the lock cannot be acquired, an
		/// <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		/// <param name="lockName">The name of the lock to acquire.</param>
		/// <param name="connectionId">The id of the connection.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="lockName"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the lock cannot be acquired.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void RequestLock(string lockName, long connectionId)
		{
			this.CheckIsTurnedOn ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbLock dbLock = this.GetLock (lockName);

				if (dbLock != null)
				{
					if (dbLock.ConnectionId != connectionId)
					{
						throw new System.InvalidOperationException ("Cannot obtain lock because it is owned by another user.");
					}

					this.SetLockCounterValue (lockName, dbLock.Counter + 1);
				}
				else
				{
					this.InsertLock (lockName, connectionId);
				}
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Releases a given lock for a given connectionId. If the lock cannot be release because it
		/// does not exists or because it is owned by another connection, an
		/// <see cref="System.InvalidOperationException"/> is thrown.
		/// </summary>
		/// <param name="lockName">The name of the lock to acquire.</param>
		/// <param name="connectionId">The id of the connection.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="lockName"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="connectionId"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If the lock cannot be released.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void ReleaseLock(string lockName, long connectionId)
		{
			this.CheckIsTurnedOn ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbLock dbLock = this.GetLock (lockName);
				
				if (dbLock == null)
				{
					throw new System.InvalidOperationException ("The lock does not exists.");
				}

				if (dbLock.ConnectionId != connectionId)
				{
					throw new System.InvalidOperationException ("The lock is owned by another user.");
				}

				if (dbLock.Counter == 0)
				{
					this.RemoveLock (lockName);
				}
				else
				{
					this.SetLockCounterValue (lockName, dbLock.Counter - 1);
				}
				
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Gets the data of a single lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose value to get.</param>
		/// <returns>The data of the <see cref="DbLock"/>.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="lockName"/> is <c>null</c> or empty.</exception>
		public DbLock GetLock(string lockName)
		{
			this.CheckIsTurnedOn ();
			
			lockName.ThrowIfNullOrEmpty ("lockName");

			SqlFunction condition = this.CreateConditionForLockName (lockName);

			var data = this.GetRowValues (condition);

			return data.Any () ? DbLock.CreateLock (data[0]) : null;
		}


		/// <summary>
		/// Tells whether a given lock is owned.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		/// <returns><c>true</c> if the lock is owned, <c>false</c> if it is not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="lockName"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public bool IsLockOwned(string lockName)
		{
			this.CheckIsTurnedOn ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			SqlFunction condition = this.CreateConditionForLockName (lockName);

			return this.RowExists (condition);
		}


		/// <summary>
		/// Inserts a brand new lock in the database.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		/// <param name="connectionId">The connection id of the lock.</param>
		private void InsertLock(string lockName, DbId connectionId)
		{
			IDictionary<string, object> columnNameToValues = new Dictionary<string, object> ()
			{
				{Tags.ColumnName, lockName},
				{Tags.ColumnConnectionId, connectionId.Value},
				{Tags.ColumnCounter, 0},
			};

			this.AddRow (columnNameToValues);
		}


		/// <summary>
		/// Removes a lock in the database.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		private void RemoveLock(string lockName)
		{
			SqlFunction condition = this.CreateConditionForLockName (lockName);

			this.RemoveRows (condition);
		}


		/// <summary>
		/// Sets the value of the counter of a single lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose counter value to set.</param>
		/// <param name="counterValue">The new value for the counter value.</param>
		private void SetLockCounterValue(string lockName, int counterValue)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{Tags.ColumnCounter, counterValue},
			};

			SqlFunction condition = this.CreateConditionForLockName (lockName);

			this.SetRowValues (columnNamesToValues, condition);
		}


		/// <summary>
		/// Deletes all locks that are associated with an inactive connection (i.e. a connection that
		/// is not open).
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		/// </summary>
		public void RemoveInactiveLocks()
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForInactiveLocks ();

			this.RemoveRows (condition);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for a given lock name.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForLockName(string lockName)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnName].GetSqlName ()),
				SqlField.CreateConstant (lockName, DbRawType.String)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true only for the locks whose connection is active.
		/// </summary>
		/// <returns>The <see cref="SqlFunction"/> object that defines the condition.</returns>
		private SqlFunction CreateConditionForInactiveLocks()
		{
			SqlSelect queryForOpenConnectionIds = new SqlSelect ();

			DbTable connectionTable = this.DbInfrastructure.ResolveDbTable (Tags.TableConnection);
			DbColumn connectionIdColumn = connectionTable.Columns[Tags.ColumnId];

			queryForOpenConnectionIds.Tables.Add ("t", SqlField.CreateName (connectionTable.GetSqlName ()));
			queryForOpenConnectionIds.Fields.Add ("c", SqlField.CreateName ("t", connectionIdColumn.GetSqlName ()));
			queryForOpenConnectionIds.Conditions.Add (this.DbInfrastructure.ServiceManager.ConnectionManager.CreateConditionForOpenConnections ());

			return new SqlFunction
			(
				SqlFunctionCode.SetNotIn,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateSubQuery (queryForOpenConnectionIds)
			);
		}


	}


}
