using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbUidSlot</c> class is an immutable object that represent an uid slot in the database,
	/// i.e a row in the CR_UID table.
	/// </summary>
	public sealed class DbUidSlot
	{


		/// <summary>
		/// Buids a new instance of <c>DbUidSlot</c>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the uid slot.</param>
		/// <param name="name">The name of the uid slot.</param>
		/// <param name="slotNumber">The slot number of the uid slot.</param>
		/// <param name="minValue">The minimum value of the uid slot.</param>
		/// <param name="maxValue">The maximum value of the uid slot.</param>
		/// <param name="nextValue">The next value of the uid slot.</param>
		public DbUidSlot(DbId id, string name, int slotNumber, long minValue, long maxValue, long nextValue)
		{
			this.Id = id;
			this.Name = name;
			this.SlotNumber = slotNumber;
			this.MinValue = minValue;
			this.MaxValue = maxValue;
			this.NextValue = nextValue;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the uid slot represented by this instance.
		/// </summary>
		public DbId Id
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the name of the uid slot represented by this instance.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the slot number of the uid slot represented by this instance.
		/// </summary>
		public int SlotNumber
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the minimum value of the uid slot represented by this instance.
		/// </summary>
		public long MinValue
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the maximum value of the uid slot represented by this instance.
		/// </summary>
		public long MaxValue
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the next value of the uid slot represented by this intance.
		/// </summary>
		internal long NextValue
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates a new instance of <see cref="DbUidSlot"/> based on the raw data of an uid counter.
		/// </summary>
		/// <param name="data">The data of the uid counter.</param>
		/// <returns>The new instance of <see cref="DbUidSlot"/>.</returns>
		internal static DbUidSlot CreateDbUidSlot(IList<object> data)
		{
			DbId id = new DbId ((long) data[0]);
			string name = (string) data[1];
			int slotNumber = (int) data[2];
			long minValue = (long) data[3];
			long maxValue = (long) data[4];
			long nextValue = (long) data[5];

			return new DbUidSlot (id, name, slotNumber, minValue, maxValue, nextValue);
		}


	}


}
