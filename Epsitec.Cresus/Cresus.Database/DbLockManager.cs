using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Data;


namespace Epsitec.Cresus.Database
{


	public sealed class DbLockManager : DbAbstractAttachable
	{


		public DbLockManager() : base ()
		{
		}


		public void InsertLock(string lockName, string userName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");
			userName.ThrowIfNullOrEmpty ("userName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				System.DateTime dateTime = this.GetCurrentDateTime ();
				
				SqlFieldList fields = new SqlFieldList ();

				DbColumn columnLockName = this.DbTable.Columns[Tags.ColumnName];
				DbColumn columnUserName = this.DbTable.Columns[Tags.ColumnUser];
				DbColumn columnDateTime = this.DbTable.Columns[Tags.ColumnDateTime];

				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnLockName, lockName));
				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnUserName, userName));
				fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnDateTime, dateTime));

				transaction.SqlBuilder.InsertData (this.DbTable.GetSqlName (), fields);

				this.DbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		public void RemoveLock(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

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


		public bool ExistsLock(string lockName)
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


		public void UpdateLockDateTime(string userName)
		{
			this.CheckIsAttached ();

			userName.ThrowIfNullOrEmpty ("userName");
			
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				System.DateTime dateTime = this.GetCurrentDateTime ();

				this.SetValue (transaction, userName, Tags.ColumnDateTime, dateTime);

				transaction.Commit ();
			}
		}


		public System.TimeSpan GetLockTimeSpan(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				System.DateTime dateTime = this.GetCurrentDateTime ();

				System.DateTime lockDateTime = (System.DateTime) this.GetValue (transaction, lockName, Tags.ColumnDateTime);
				System.DateTime currentDateTime = this.GetCurrentDateTime ();

				transaction.Commit ();

				return currentDateTime - lockDateTime;
			}
		}


		public string GetLockUserName(string lockName)
		{
			this.CheckIsAttached ();

			lockName.ThrowIfNullOrEmpty ("lockName");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				System.DateTime dateTime = this.GetCurrentDateTime ();

				object userName = this.GetValue (transaction, lockName, Tags.ColumnUser);

				transaction.Commit ();

				return (string) userName;
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

			if (table.Rows.Count == 0)
			{
				throw new System.Exception ("Not enough rows for counter");
			}

			if (table.Rows.Count > 1)
			{
				throw new System.Exception ("Too much rows for counter");
			}

			return table.Rows[0][valueName];
		}


		private void SetValue(DbTransaction transaction, string userName, string valueName, object value)
		{
			DbColumn column = this.DbTable.Columns[valueName];

			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForUserName (userName),
			};

			transaction.SqlBuilder.UpdateData (this.DbTable.GetSqlName (), fields, conditions);
			this.DbInfrastructure.ExecuteNonQuery (transaction);
		}


		private System.DateTime GetCurrentDateTime()
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				transaction.SqlBuilder.GetCurrentTimeStamp ();

				object value = this.DbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (System.DateTime) value;
			}
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


		private SqlFunction CreateConditionForUserName(string userName)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnUser].GetSqlName ()),
				SqlField.CreateConstant (userName, DbRawType.String)
			);
		}
		

	}


}
