namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbLock</c> class is an immutable object that represents a lock in the CR_LOCK
	/// table in the database.
	/// </summary>
	public sealed class DbLock
	{


		/// <summary>
		/// Builds a new instance of <c>DbLock</c>.
		/// </summary>
		/// <param name="id">The <see cref="DbId"/> of the lock.</param>
		/// <param name="connectionId">The <see cref="DbId"/> of the connection of the lock.</param>
		/// <param name="name">The name of the lock.</param>
		/// <param name="counter">The counter that counts the number of times the lock has been acquired.</param>
		/// <param name="creationTime">The instant at which the lock has been acquired for the first time.</param>
		internal DbLock(DbId id, DbId connectionId, string name, int counter, System.DateTime creationTime)
		{
			this.Id = id;
			this.ConnectionId = connectionId;
			this.Name = name;
			this.Counter = counter;
			this.CreationTime = creationTime;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the lock represented by this instance.
		/// </summary>
		public DbId Id
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the connection of the lock represented by this instance.
		/// </summary>
		public DbId ConnectionId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the name of the lock represented by this instance.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the counter that gives the number of times that the lock represented by this instance
		/// has been acquired.
		/// </summary>
		internal int Counter
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the instant at which the lock represented by this instance has been acquired for
		/// the first time.
		/// </summary>
		public System.DateTime CreationTime
		{
			get;
			private set;
		}


	}


}
