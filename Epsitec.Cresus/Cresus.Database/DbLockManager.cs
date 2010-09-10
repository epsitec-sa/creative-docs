using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Data;


namespace Epsitec.Cresus.Database
{


	// TODO Comment this class.
	// Marc


	public sealed class DbLockManager : DbAbstractAttachable
	{


		public DbLockManager() : base ()
		{
		}


		public void RequestLock(string lockName, long connexionId)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connexionId.ThrowIf (c => c < 0, "connexionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (this.IsLockOwned (lockName))
				{
					if (this.GetLockConnexionId (lockName) != connexionId)
					{
						throw new System.InvalidOperationException ("Cannot obtain lock because it is owned by another user.");
					}

					this.IncrementLockCounter (lockName);
				}
				else
				{
					this.InsertLock (lockName, connexionId);
				}

				transaction.Commit ();
			}
		}


		public void ReleaseLock(string lockName, long connexionId)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			connexionId.ThrowIf (c => c < 0, "connexionId cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (this.IsLockOwned (lockName))
				{
					if (this.GetLockConnexionId (lockName) != connexionId)
					{
						throw new System.InvalidOperationException ("Cannot release lock because it is owned by another user.");
					}

					if (this.GetLockCounterValue (lockName) == 0)
					{
						this.RemoveLock (lockName);
					}
					else
					{
						this.DecrementLockCounter (lockName);
					}
				}

				transaction.Commit ();
			}
		}


		private void InsertLock(string lockName, long connexionId)
		{
			SqlFieldList fields = new SqlFieldList ();

			DbColumn columnLockName = this.DbTable.Columns[Tags.ColumnName];
			DbColumn columnConnexionId = this.DbTable.Columns[Tags.ColumnConnexionId];
			DbColumn columnCounter = this.DbTable.Columns[Tags.ColumnCounter];

			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnLockName, lockName));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnexionId, connexionId));
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


		public long? GetLockConnexionId(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long? connexionId = null;
								
				if (this.IsLockOwned (lockName))
				{
					connexionId = (long) this.GetValue (lockName, Tags.ColumnConnexionId);
				}

				transaction.Commit ();

				return connexionId;
			}
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
		

	}


}
