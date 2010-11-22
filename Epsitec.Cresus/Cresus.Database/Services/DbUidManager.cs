using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbUidManager</c> class provides the low levels tools used to generate unique ids in
	/// the database. These counters are addressed by a name and a slot number. In addition, each
	/// contains a minimum value, a maximum value and a next value.
	/// </summary>
	public sealed class DbUidManager : DbAbstractAttachable
	{


		/// <summary>
		/// Builds a new instance of <see cref="DbUidManager"/>.
		/// </summary>
		internal DbUidManager() : base()
		{
		}


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

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (this.ExistsUidCounter (name, slot))
				{
					throw new System.InvalidOperationException ("The counter already exists.");
				}

				this.InsertUidCounter (name, slot, min, max);

				transaction.Commit ();
			}
		}
		

		/// <summary>
		/// Builds and executes the request used to insert an uid counter.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="min">The minimum value of the counter.</param>
		/// <param name="max">The maximum value of the counter.</param>
		private void InsertUidCounter(string name, int slot, long min, long max)
		{
			SqlFieldList fields = new SqlFieldList ();

			DbColumn columnName = this.DbTable.Columns[Tags.ColumnName];
			DbColumn columnSlot = this.DbTable.Columns[Tags.ColumnUidSlot];
			DbColumn columnMin = this.DbTable.Columns[Tags.ColumnUidMin];
			DbColumn columnMax = this.DbTable.Columns[Tags.ColumnUidMax];
			DbColumn columnNext = this.DbTable.Columns[Tags.ColumnUidNext];

			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnName, name));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnSlot, slot));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnMin, min));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnMax, max));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnNext, min));

			this.AddRow (fields);
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

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				if (!this.ExistsUidCounter (name, slot))
				{
					throw new System.InvalidOperationException ("The counter does not exists.");
				}

				this.RemoveUidCounter (name, slot);

				transaction.Commit ();
			}
		}
		

		/// <summary>
		/// Builds and executes the request used to remove an uid counter.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		private void RemoveUidCounter(string name, int slot)
		{
			SqlFunction[] conditions = new SqlFunction[]
            {
                this.CreateConditionForName (name),
                this.CreateConditionForSlot (slot),
            };

			this.RemoveRows (conditions);
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

			SqlFunction[] conditions = new SqlFunction[]
			{
				this.CreateConditionForName (name),
				this.CreateConditionForSlot (slot),
			};

			return this.RowExists (conditions);
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

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				DbColumn column = this.DbTable.Columns[Tags.ColumnUidSlot];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForName (name)
				};

				List<int> slots = this.GetRowsValue (column, conditions).Cast<int> ().ToList ();

				transaction.Commit ();

				return slots;
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

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ExistsUidCounter (name, slot))
				{
					throw new System.InvalidOperationException ("The counter does not exists.");
				}

				long min = this.GetValue (name, slot, Tags.ColumnUidMin);

				transaction.Commit ();

				return min;
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

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ExistsUidCounter (name, slot))
				{
					throw new System.InvalidOperationException ("The counter does not exists.");
				}

				long max = this.GetValue (name, slot, Tags.ColumnUidMax);

				transaction.Commit ();

				return max;
			}
		}


		/// <summary>
		/// Gets the next value of a counter for uids in the database and increments it automatically,
		/// so that two consecutive calls won't return the same value.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long? GetUidCounterNext(string name, int slot)
		{
			this.CheckIsAttached ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadWrite))
            {
				if (!this.ExistsUidCounter (name, slot))
				{
					throw new System.InvalidOperationException ("The counter does not exists.");
				}

				long max = this.GetValue (name, slot, Tags.ColumnUidMax);
				long next = this.GetValue (name, slot, Tags.ColumnUidNext);

				bool isNextValidUid = (next <= max);

				if (isNextValidUid)
				{
					this.SetValue (name, slot, Tags.ColumnUidNext, next + 1);
				}
				
				transaction.Commit();

				return isNextValidUid ? (long?) next : null;
			}
		}


		/// <summary>
		/// Gets a given value of a counter for uids in the database.
		/// </summary>
		/// <param name="counterName">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="valueName">The name of the value to get.</param>
		/// <returns>The requested value.</returns>
		/// <exception cref="System.Exception">If the counter is invalid.</exception>
		private long GetValue(string counterName, int slot, string valueName)
		{
			DbColumn column = this.DbTable.Columns[valueName];
			
			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForName (counterName),
				this.CreateConditionForSlot (slot),
			};

			return (long) this.GetRowValue (column, conditions);
		}


		/// <summary>
		/// Sets a given value of a counter for uids in the database.
		/// </summary>
		/// <param name="counterName">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="valueName">The name of the value to get.</param>
		/// <param name="value">The value to use.</param>
		private void SetValue(string counterName, int slot, string valueName, long value)
		{
			DbColumn column = this.DbTable.Columns[valueName];

			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForName (counterName),
				this.CreateConditionForSlot (slot),
			};

			this.SetRowValue (fields, conditions);
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
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnName].GetSqlName ()),
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
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnUidSlot].GetSqlName ()),
				SqlField.CreateConstant (slot, DbRawType.Int32)
			);
		}


	}


}
