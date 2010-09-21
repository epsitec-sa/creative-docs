using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbLockManager</c> class provides the low level tools required to interact with the
	/// high level locks.
	/// An high level lock is associated to a connection as defined by <see cref="DbConnectionManager"/>
	/// and is defined by a name. In addition, a lock contains a value which tells how many time it
	/// has been acquired by the user, which makes it reentrant.
	/// </summary>
	public sealed class DbLockManager : DbAbstractAttachable
	{


		/// <summary>
		/// Creates a new <c>DbLockManager</c>.
		/// </summary>
		public DbLockManager() : base ()
		{
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
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (this.IsLockOwned (lockName))
				{
					if (this.GetLockConnectionId (lockName) != connectionId)
					{
						throw new System.InvalidOperationException ("Cannot obtain lock because it is owned by another user.");
					}

					this.IncrementLockCounter (lockName);
				}
				else
				{
					this.InsertLock (lockName, connectionId);
				}

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Releases a given locl for a given connectionId. If the lock cannot be release because it
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
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connectionId.ThrowIf (c => c < 0, "connectionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (!this.IsLockOwned (lockName))
				{
					throw new System.InvalidOperationException ("The lock does not exists.");
				}

				if (this.GetLockConnectionId (lockName) != connectionId)
				{
					throw new System.InvalidOperationException ("The lock is owned by another user.");
				}

				if (this.GetLockCounterValue (lockName) == 0)
				{
					this.RemoveLock (lockName);
				}
				else
				{
					this.DecrementLockCounter (lockName);
				}
				
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Inserts a brand new lock in the database.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		/// <param name="connectionId">The connection id of the lock.</param>
		private void InsertLock(string lockName, long connectionId)
		{
			SqlFieldList fields = new SqlFieldList ();

			DbColumn columnLockName = this.DbTable.Columns[Tags.ColumnName];
			DbColumn columnConnectionId = this.DbTable.Columns[Tags.ColumnConnectionId];
			DbColumn columnCounter = this.DbTable.Columns[Tags.ColumnCounter];

			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnLockName, lockName));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnectionId, connectionId));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounter, 0));

			this.AddRow (fields);
		}


		/// <summary>
		/// Removes a lock in the database.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		private void RemoveLock(string lockName)
		{
			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			this.RemoveRows (conditions);
		}


		/// <summary>
		/// Increments the counter of a given lock by 1.
		/// </summary>
		/// <param name="lockName">The name of the lock whose counter to increment.</param>
		private void IncrementLockCounter(string lockName)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				int counterValue = this.GetLockCounterValue (lockName);

				this.SetLockCounterValue (lockName, counterValue + 1);
				
				transaction.Commit ();
			}
		}


		/// <summary>
		/// Decrements the counter of a given lock by 1.
		/// </summary>
		/// <param name="lockName">The name of the lock whose counter to decrement.</param>
		private void DecrementLockCounter(string lockName)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				int counterValue = this.GetLockCounterValue (lockName);

				this.SetLockCounterValue (lockName, counterValue - 1);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Gets the current counter value of a given lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose counter value to get.</param>
		/// <returns>The current counter value of the lock.</returns>
		private int GetLockCounterValue(string lockName)
		{
			return (int) this.GetValue (lockName, Tags.ColumnCounter);
		}


		/// <summary>
		/// Sets the current counter value of a given lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose counter value to set.</param>
		/// <param name="counterValue">The current counter value of the lock.</param>
		private void SetLockCounterValue(string lockName, int counterValue)
		{
			this.SetValue (lockName, Tags.ColumnCounter, counterValue);
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
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			return this.RowExists (conditions);
		}



		/// <summary>
		/// Gets the connection id of the owner of the given lock.
		/// </summary>
		/// <param name="lockName">The name of the lock.</param>
		/// <returns>The connection id of the lock.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="lockName"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If the lock does not exist.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long GetLockConnectionId(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.IsLockOwned (lockName))
				{
					throw new System.InvalidOperationException ("The lock does not exists.");
				}

				long connectionId = (long) this.GetValue (lockName, Tags.ColumnConnectionId);

				transaction.Commit ();

				return connectionId;
			}
		}


		/// <summary>
		/// Deletes all locks that are associated with an inactive connection (i.e. a connection that
		/// is not open).
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		/// </summary>
		public void RemoveInactiveLocks()
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForInactiveLocks (),
			};

			this.RemoveRows (conditions);
		}


		/// <summary>
		/// Gets a single value of a single lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose value to get.</param>
		/// <param name="valueName">The name of the value to get.</param>
		/// <returns>The value.</returns>
		private object GetValue(string lockName, string valueName)
		{
			DbColumn column = this.DbTable.Columns[valueName];

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			return this.GetRowValue (column, conditions);
		}


		/// <summary>
		/// Sets a single value of a single lock.
		/// </summary>
		/// <param name="lockName">The name of the lock whose value to set.</param>
		/// <param name="valueName">The name of the value to set.</param>
		/// <param name="value">The new value for the value.</param>
		private void SetValue(string lockName, string valueName, object value)
		{
			DbColumn column = this.DbTable.Columns[valueName];

			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			this.SetRowValue (fields, conditions);
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
			queryForOpenConnectionIds.Conditions.Add (this.DbInfrastructure.ConnectionManager.CreateConditionForOpenConnections ());

			return new SqlFunction
			(
				SqlFunctionCode.SetNotIn,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateSubQuery (queryForOpenConnectionIds)
			);
		}
		

	}


}
