using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.Database
{
	
	
	public sealed class DbUidManager : IAttachable
	{


		public DbUidManager()
		{
			this.isAttached = false;
		}


		#region IAttachable Members

		
		public void Attach(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{			
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			dbTable.ThrowIfNull ("dbTable");

			this.dbInfrastructure = dbInfrastructure;
			this.table = dbTable;

			this.isAttached = true;
		}

		
		public void Detach()
		{
			this.isAttached = false;

			this.dbInfrastructure = null;
			this.table = null;
		}


		#endregion


		public void CreateUidCounter(string name, long min, long max)
		{
			this.CheckIsAttached ();
			
			name.ThrowIfNullOrEmpty ("name");
			min.ThrowIf (m => m < 0, "min cannot be lower than zero");
			max.ThrowIf (m => m < 0, "max cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fields = new SqlFieldList ();

				DbColumn columnName = this.table.Columns[Tags.ColumnName];
				DbColumn columnMin = this.table.Columns[Tags.ColumnUidMin];
				DbColumn columnMax = this.table.Columns[Tags.ColumnUidMax];
				DbColumn columnCurrent = this.table.Columns[Tags.ColumnUidCurrent];

				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnName, name));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnMin, min));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnMax, max));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnCurrent, 0));

				transaction.SqlBuilder.InsertData (this.table.GetSqlName (), fields);

				this.dbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		public void DeleteUidCounter(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForName(name)
				};

				transaction.SqlBuilder.RemoveData (this.table.GetSqlName (), conditions);

				this.dbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		public bool ExistsUidCounter(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.table.GetSqlName ()));
				query.Fields.Add
				(
					SqlField.CreateAggregate
					(
						SqlAggregateFunction.Count,
						SqlField.CreateName (Tags.TableUid, Tags.ColumnId)
					)
				);
				query.Conditions.Add (this.CreateConditionForName (name));

				transaction.SqlBuilder.SelectData (query);

				object value = this.dbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (value != null) && (((int) value) > 0);
			}
		}


		public IEnumerable<string> GetUidCounterNames()
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.table.GetSqlName ()));
				query.Fields.Add (Tags.ColumnName, SqlField.CreateName (Tags.TableUid, Tags.ColumnName));

				transaction.SqlBuilder.SelectData (query);

				DataTable table = this.dbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

				List<string> names = new List<string> ();
				
				foreach (DataRow row in table.Rows)
				{
					string name = (string) row[Tags.ColumnName];

					names.Add (name);
				}

				transaction.Commit ();

				return names;
			}
		}


		public long GetUidCounterMin(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long minValue = this.GetValue (transaction, name, Tags.ColumnUidMin);

				transaction.Commit ();

				return minValue;
			}
		}


		public long GetUidCounterMax(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long maxValue = this.GetValue (transaction, name, Tags.ColumnUidMax);

				transaction.Commit ();

				return maxValue;
			}
		}


		public long GetUidCounterCurrent(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long currentValue = this.GetValue (transaction, name, Tags.ColumnUidCurrent);

				transaction.Commit ();

				return currentValue;
			}
		}


		public void SetUidCounterMin(string name, long min)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			min.ThrowIf (m => m < 0, "min cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, Tags.ColumnUidMin, min);

				transaction.Commit ();
			}
		}


		public void SetUidCounterMax(string name, long max)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			max.ThrowIf (m => m < 0, "max cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, Tags.ColumnUidMax, max);

				transaction.Commit ();
			}
		}


		public void SetUidCounterCurrent(string name, long current)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			current.ThrowIf (m => m < 0, "current cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, Tags.ColumnUidCurrent, current);

				transaction.Commit ();
			}
		}
		

		private long GetValue(DbTransaction transaction, string counterName, string valueName)
		{
			SqlSelect query = new SqlSelect ();

			query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.table.GetSqlName ()));
			query.Fields.Add (valueName, SqlField.CreateName (Tags.TableUid, valueName));
			query.Conditions.Add (this.CreateConditionForName (counterName));

			transaction.SqlBuilder.SelectData (query);

			DataTable table = this.dbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

			if (table.Rows.Count == 0)
			{
				throw new System.Exception ("Not enough rows for counter");
			}

			if (table.Rows.Count > 1)
			{
				throw new System.Exception ("Too much rows for counter");
			}

			return (long) table.Rows[0][valueName];
		}


		private void SetValue(DbTransaction transaction, string counterName, string valueName, long value)
		{
			DbColumn column = this.table.Columns[valueName];

			SqlFieldList fields = new SqlFieldList ()
			{
				this.dbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForName (counterName),
			};

			transaction.SqlBuilder.UpdateData (this.table.GetSqlName (), fields, conditions);
			this.dbInfrastructure.ExecuteNonQuery (transaction);
		}


		private SqlFunction CreateConditionForName(string name)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[Tags.ColumnName].GetSqlName ()),
				SqlField.CreateConstant (name, DbRawType.String)
			);
		}


		private void CheckIsAttached()
		{
			if (!this.isAttached)
			{
				throw new System.InvalidOperationException ("Cannot use this instance because it is detached.");
			}
		}


		private bool isAttached;


		private DbInfrastructure dbInfrastructure;


		private DbTable table;


	}


}
