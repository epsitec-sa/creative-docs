﻿using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database
{


	// TODO Comment this class.
	// Marc


	public sealed class DbLockManager : DbAbstractAttachable
	{


		public DbLockManager() : base ()
		{
		}


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


		private void RemoveLock(string lockName)
		{
			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			this.RemoveRow (conditions);
		}


		private void IncrementLockCounter(string lockName)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				int counterValue = this.GetLockCounterValue (lockName);

				this.SetLockCounterValue (lockName, counterValue + 1);
				
				transaction.Commit ();
			}
		}


		private void DecrementLockCounter(string lockName)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				int counterValue = this.GetLockCounterValue (lockName);

				this.SetLockCounterValue (lockName, counterValue - 1);

				transaction.Commit ();
			}
		}

		private int GetLockCounterValue(string lockName)
		{
			return (int) this.GetValue (lockName, Tags.ColumnCounter);
		}


		private void SetLockCounterValue(string lockName, int counterValue)
		{
			this.SetValue (lockName, Tags.ColumnCounter, counterValue);
		}

		
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


		public void RemoveInactiveLocks()
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForInactiveLocks (),
			};

			this.RemoveRow (conditions);
		}


		private object GetValue(string lockName, string valueName)
		{
			DbColumn column = this.DbTable.Columns[valueName];

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForLockName (lockName),
			};

			return this.GetRowValue (column, conditions);
		}


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


		private SqlFunction CreateConditionForLockName(string lockName)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnName].GetSqlName ()),
				SqlField.CreateConstant (lockName, DbRawType.String)
			);
		}


		private SqlFunction CreateConditionForInactiveLocks()
		{
			SqlSelect queryForOpenedConnectionIds = new SqlSelect ();

			DbTable connectionTable = this.DbInfrastructure.ResolveDbTable (Tags.TableConnection);
			DbColumn connectionIdColumn = connectionTable.Columns[Tags.ColumnId];

			queryForOpenedConnectionIds.Tables.Add ("t", SqlField.CreateName (connectionTable.GetSqlName ()));
			queryForOpenedConnectionIds.Fields.Add ("c", SqlField.CreateName ("t", connectionIdColumn.GetSqlName ()));
			queryForOpenedConnectionIds.Conditions.Add (this.DbInfrastructure.ConnectionManager.CreateConditionForOpenedConnections ());

			return new SqlFunction
			(
				SqlFunctionCode.SetNotIn,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateSubQuery (queryForOpenedConnectionIds)
			);
		}
		

	}


}
