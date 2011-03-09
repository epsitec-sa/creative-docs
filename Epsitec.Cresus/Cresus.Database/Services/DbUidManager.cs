using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbUidManager</c> class provides the low levels tools used to generate unique ids in
	/// the database. These counters are addressed by a name and a slot number. In addition, each
	/// contains a minimum value, a maximum value and a next value.
	/// </summary>
	public sealed class DbUidManager : DbAbstractTableService
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Builds a new instance of <see cref="DbUidManager"/>.
		/// </summary>
		internal DbUidManager(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableUid;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableUid);

			DbColumn columnId =    new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};
			
			DbColumn columnName = new DbColumn (Tags.ColumnName, types.Name, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnUidSlot = new DbColumn (Tags.ColumnUidSlot, types.DefaultInteger, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnUidMin = new DbColumn (Tags.ColumnUidMin, types.DefaultLongInteger, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnUidMax = new DbColumn (Tags.ColumnUidMax, types.DefaultLongInteger, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnUidNext = new DbColumn (Tags.ColumnUidNext, types.DefaultLongInteger, DbColumnClass.Data, DbElementCat.Internal);

			table.Columns.Add (columnId);
			table.Columns.Add (columnName);
			table.Columns.Add (columnUidSlot);
			table.Columns.Add (columnUidMin);
			table.Columns.Add (columnUidMax);
			table.Columns.Add (columnUidNext);

			table.DefineCategory (DbElementCat.Internal);
			
			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			table.AddIndex ("IDX_UID_NAME", SqlSortOrder.Ascending, columnName);

			return table;
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
		/// <exception cref="System.InvalidOperationException">If the counter already exists.</exception>
		public void CreateUidCounter(string name, int slot, long min, long max)
		{
			this.CheckIsTurnedOn ();
			
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
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{Tags.ColumnName, name},
				{Tags.ColumnUidSlot, slot},
				{Tags.ColumnUidMin, min},
				{Tags.ColumnUidMax, max},
				{Tags.ColumnUidNext, min},
			};

			this.AddRow (columnNamesToValues);
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
			this.CheckIsTurnedOn ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

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
			this.CheckIsTurnedOn ();

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
		/// Gets the list of all slots for a given uid name.
		/// </summary>
		/// <param name="name">The name of the slots to get.</param>
		/// <returns>The list with the data of the slots.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public IEnumerable<DbUidSlot> GetUidCounterSlots(string name)
		{
			this.CheckIsTurnedOn ();

			name.ThrowIfNullOrEmpty ("name");

			SqlFunction condition = this.CreateConditionForName (name);

			var data = this.GetRowValues (condition);

			return data.Select (d => DbUidSlot.CreateDbUidSlot (d));
		}


		/// <summary>
		/// Gets the data of a uid counter in the database.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns>The data of the uid counter.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public DbUidSlot GetUidCounter(string name, int slot)
		{
			this.CheckIsTurnedOn ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			SqlFunction[] conditions = new SqlFunction[]
			{
				this.CreateConditionForName (name),
				this.CreateConditionForSlot (slot),
			};

			var data = this.GetRowValues (conditions);

			return data.Any () ? DbUidSlot.CreateDbUidSlot (data[0]) : null;
		}


		/// <summary>
		/// Gets the next value of an uid counter in the database and increments it automatically,
		/// so that two consecutive calls won't return the same value.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <returns>The next value for the uid counter, or <c>null</c> if there is no valid next value.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="slot"/> is lower than zero.</exception>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		public long? GetUidCounterNextValue(string name, int slot)
		{
			this.CheckIsTurnedOn ();

			name.ThrowIfNullOrEmpty ("name");
			slot.ThrowIf (s => s < 0, "slot cannot be lower than zero");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadWrite))
            {
				DbUidSlot dbUid = this.GetUidCounter (name, slot);
				
				if (dbUid == null)
				{
					throw new System.InvalidOperationException ("The counter does not exists.");
				}

				bool isNextValidUid = (dbUid.NextValue <= dbUid.MaxValue);

				if (isNextValidUid)
				{
					this.SetUidCounterNextValue (name, slot, dbUid.NextValue + 1);
				}
				
				transaction.Commit();

				return isNextValidUid ? (long?) dbUid.NextValue : null;
			}
		}
		

		/// <summary>
		/// Executes the request that sets the next value of an uid counter to a given value.
		/// </summary>
		/// <param name="name">The name of the counter.</param>
		/// <param name="slot">The slot number of the counter.</param>
		/// <param name="nextValue">The next next value of the uid counter.</param>
		private void SetUidCounterNextValue(string name, int slot, long nextValue)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
            {
                {Tags.ColumnUidNext, nextValue},
            };

			SqlFunction[] conditions = new SqlFunction[]
            {
                this.CreateConditionForName (name),
                this.CreateConditionForSlot (slot),
            };

			this.SetRowValues (columnNamesToValues, conditions);
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
