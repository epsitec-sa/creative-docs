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
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fields = new SqlFieldList ();

				DbColumn columnLockName = this.DbTable.Columns[Tags.ColumnName];
				DbColumn columnConnexionId = this.DbTable.Columns[Tags.ColumnConnexionId];
				DbColumn columnCounter = this.DbTable.Columns[Tags.ColumnCounter];

				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnLockName, lockName));
				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnexionId, connexionId));
				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounter, 0));

				transaction.SqlBuilder.InsertData (this.DbTable.GetSqlName (), fields);

				this.DbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		private void RemoveLock(string lockName)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForLockName (lockName),
				};

				transaction.SqlBuilder.RemoveData (this.DbTable.GetSqlName (), conditions);

				this.DbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
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
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				object counterValue = this.GetValue (transaction, lockName, Tags.ColumnCounter);

				transaction.Commit ();

				return (int) counterValue;
			}
		}


		private void SetLockCounterValue(string lockName, int counterValue)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, lockName, Tags.ColumnCounter, counterValue);

				transaction.Commit ();
			}
		}

		
		public bool IsLockOwned(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.DbTable.GetSqlName ()));
				query.Fields.Add
				(
					SqlField.CreateAggregate
					(
						SqlAggregateFunction.Count,
						SqlField.CreateName (Tags.TableUid, Tags.ColumnId)
					)
				);
				query.Conditions.Add (this.CreateConditionForLockName (lockName));

				transaction.SqlBuilder.SelectData (query);

				object value = this.DbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (value != null) && (((int) value) > 0);
			}
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
					connexionId = (long) this.GetValue (transaction, lockName, Tags.ColumnConnexionId);
				}

				transaction.Commit ();

				return connexionId;
			}
		}


		private object GetValue(DbTransaction transaction, string lockName, string valueName)
		{
			SqlSelect query = new SqlSelect ();

			query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.DbTable.GetSqlName ()));
			query.Fields.Add (valueName, SqlField.CreateName (Tags.TableUid, valueName));
			query.Conditions.Add (this.CreateConditionForLockName (lockName));

			transaction.SqlBuilder.SelectData (query);

			DataTable table = this.DbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

			return table.Rows[0][valueName];
		}


		private void SetValue(DbTransaction transaction, string lockName, string valueName, object value)
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

			transaction.SqlBuilder.UpdateData (this.DbTable.GetSqlName (), fields, conditions);
			this.DbInfrastructure.ExecuteNonQuery (transaction);
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
