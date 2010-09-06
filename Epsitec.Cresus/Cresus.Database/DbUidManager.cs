using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbUidManager</c> class provides the low levels tools used to generate unique ids in
	/// the database. These counters are addressed by a name and a slot number. In addition, each
	/// contains a minimum value, a maximum value and a next value.
	/// </summary>
	public sealed class DbUidManager : IAttachable
	{


		/// <summary>
		/// Builds a new instance of <see cref="DbUidManager"/>.
		/// </summary>
		public DbUidManager()
		{
			// TODO Make this internal? Yes but then I must find a way to make it visible to the
			// unit test project with the InternalsVisibleTo attributes.
			// Marc

			this.isAttached = false;
		}


		#region IAttachable Members

		
		/// <summary>
		/// Attaches this instance to the specified database table.
		/// </summary>
		/// <param name="dbInfrastructure">The infrastructure.</param>
		/// <param name="dbTable">The database table.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure" /> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbTable" /> is <c>null</c>.</exception>
		public void Attach(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{			
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			dbTable.ThrowIfNull ("dbTable");

			this.dbInfrastructure = dbInfrastructure;
			this.table = dbTable;

			this.isAttached = true;
		}


		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
		public void Detach()
		{
			this.isAttached = false;

			this.dbInfrastructure = null;
			this.table = null;
		}


		#endregion


		/// <summary>
		/// Creates a new counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="min">The minimum value of the counter.</param>
		/// <param name="max">The maximum value of the counter.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="min"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="max"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void CreateUidCounter(string name, int slot, long min, long max)
		{
			this.CheckIsAttached ();
			
			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");
			min.ThrowIf (m => m < 0, "min cannot be lower than zero");
			max.ThrowIf (m => m < 0, "max cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fields = new SqlFieldList ();

				DbColumn columnName = this.table.Columns[Tags.ColumnName];
				DbColumn columnSlot = this.table.Columns[Tags.ColumnUidSlot];
				DbColumn columnMin = this.table.Columns[Tags.ColumnUidMin];
				DbColumn columnMax = this.table.Columns[Tags.ColumnUidMax];
				DbColumn columnNext = this.table.Columns[Tags.ColumnUidNext];

				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnName, name));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnSlot, slot));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnMin, min));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnMax, max));
				fields.Add (this.dbInfrastructure.CreateSqlFieldFromAdoValue (columnNext, min));

				transaction.SqlBuilder.InsertData (this.table.GetSqlName (), fields);

				this.dbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Deletes a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void DeleteUidCounter(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForName (name),
					this.CreateConditionForSlot (slot),
				};

				transaction.SqlBuilder.RemoveData (this.table.GetSqlName (), conditions);

				this.dbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Tells whether a counter for uids exists in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns><c>true</c> if the counter exists, <c>false</c> if it doesn't.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public bool ExistsUidCounter(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

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
				query.Conditions.Add (this.CreateConditionForSlot (slot));

				transaction.SqlBuilder.SelectData (query);

				object value = this.dbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (value != null) && (((int) value) > 0);
			}
		}

		
		/// <summary>
		/// Gets the list of all slots for a given name.
		/// </summary>
		/// <param name="name">The name of the slots to get.</param>
		/// <returns>The list of slots.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public IEnumerable<int> GetUidCounterSlots(string name)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.table.GetSqlName ()));
				query.Fields.Add (Tags.ColumnName, SqlField.CreateName (Tags.TableUid, Tags.ColumnName));
				query.Fields.Add (Tags.ColumnUidSlot, SqlField.CreateName (Tags.TableUid, Tags.ColumnUidSlot));
				query.Conditions.Add (this.CreateConditionForName (name));

				transaction.SqlBuilder.SelectData (query);

				DataTable table = this.dbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

				List<int> names = new List<int> ();

				foreach (DataRow row in table.Rows)
				{
					int slot = (int) row[Tags.ColumnUidSlot];

					names.Add (slot);
				}

				transaction.Commit ();

				return names;
			}
		}


		/// <summary>
		/// Gets the minimal value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns>The minimum value of the counter.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long GetUidCounterMin(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long minValue = this.GetValue (transaction, name, slot, Tags.ColumnUidMin);

				transaction.Commit ();

				return minValue;
			}
		}


		/// <summary>
		/// Gets the maximum value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns>The maximum value of the counter.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long GetUidCounterMax(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long maxValue = this.GetValue (transaction, name, slot, Tags.ColumnUidMax);

				transaction.Commit ();

				return maxValue;
			}
		}


		/// <summary>
		/// Gets the next value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns>The next value of the counter.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long GetUidCounterNext(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				long nextValue = this.GetValue (transaction, name, slot, Tags.ColumnUidNext);

				transaction.Commit ();

				return nextValue;
			}
		}


		/// <summary>
		/// Sets the minimal value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="min">The minimum value to set.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="min"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void SetUidCounterMin(string name, int slot, long min)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");
			min.ThrowIf (m => m < 0, "min cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, slot, Tags.ColumnUidMin, min);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Sets the maximal value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="max">The maximal value to set.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="max"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void SetUidCounterMax(string name, int slot, long max)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");
			max.ThrowIf (m => m < 0, "max cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, slot, Tags.ColumnUidMax, max);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Sets the next value of a counter for uids in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="next">The next value to set.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="next"/> is lower tha zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public void SetUidCounterNext(string name, int slot, long next)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");
			next.ThrowIf (m => m < 0, "next cannot be lower than zero");

			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				this.SetValue (transaction, name, slot, Tags.ColumnUidNext, next);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Gets a given value of a counter for uids in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use for the request.</param>
		/// <param name="counterName">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="valueName">The name of the value to get.</param>
		/// <returns>The requested value.</returns>
		/// <exception cref="System.Exception">If the counter is invalid.</exception>
		private long GetValue(DbTransaction transaction, string counterName, int slot, string valueName)
		{
			SqlSelect query = new SqlSelect ();

			query.Tables.Add (Tags.TableUid, SqlField.CreateName (this.table.GetSqlName ()));
			query.Fields.Add (valueName, SqlField.CreateName (Tags.TableUid, valueName));
			query.Conditions.Add (this.CreateConditionForName (counterName));
			query.Conditions.Add (this.CreateConditionForSlot (slot));

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


		/// <summary>
		/// Sets a given value of a counter for uids in the database.
		/// </summary>
		/// <param name="transaction">The <see cref="DbTransaction"/> to use for the request.</param>
		/// <param name="counterName">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="valueName">The name of the value to get.</param>
		/// <param name="value">The value to use.</param>
		private void SetValue(DbTransaction transaction, string counterName, int slot, string valueName, long value)
		{
			DbColumn column = this.table.Columns[valueName];

			SqlFieldList fields = new SqlFieldList ()
			{
				this.dbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForName (counterName),
				this.CreateConditionForSlot (slot),
			};

			transaction.SqlBuilder.UpdateData (this.table.GetSqlName (), fields, conditions);
			this.dbInfrastructure.ExecuteNonQuery (transaction);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> to use as a condition for the name of a counter for
		/// uids.
		/// </summary>
		/// <param name="name">The name of the counter to match.</param>
		/// <returns>The <see cref="SqlFunction"/> to use as a condition.</returns>
		private SqlFunction CreateConditionForName(string name)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[Tags.ColumnName].GetSqlName ()),
				SqlField.CreateConstant (name, DbRawType.String)
			);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> to use as a condition for the slot of a counter for
		/// uids.
		/// </summary>
		/// <param name="slot">The slot of the counter to match.</param>
		/// <returns>The <see cref="SqlFunction"/> to use as a condition.</returns>
		private SqlFunction CreateConditionForSlot(int slot)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.table.Columns[Tags.ColumnUidSlot].GetSqlName ()),
				SqlField.CreateConstant (slot, DbRawType.Int32)
			);
		}


		/// <summary>
		/// Checks that this instance is attached to a <see cref="DbInfrastructure"/>.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		private void CheckIsAttached()
		{
			if (!this.isAttached)
			{
				throw new System.InvalidOperationException ("Cannot use this instance because it is detached.");
			}
		}


		/// <summary>
		/// The state of this instance.
		/// </summary>
		private bool isAttached;


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object to use to communicate with the database.
		/// </summary>
		private DbInfrastructure dbInfrastructure;


		/// <summary>
		/// The <see cref="DbTable"/> used to store the counters data.
		/// </summary>
		private DbTable table;


	}


}
