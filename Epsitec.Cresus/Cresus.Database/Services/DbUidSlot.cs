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


	}


}
