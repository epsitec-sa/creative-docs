namespace Epsitec.Cresus.Database.Services
{
	
	
	/// <summary>
	/// The <c>DbLogEntry</c> class is an immutable object that represents an entry in the log
	/// table in the database.
	/// </summary>
	public sealed class DbLogEntry
	{
		
		
		/// <summary>
		/// Builds a new <c>DbLogEntry</c>.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry.</param>
		/// <param name="connectionId">The connection <see cref="DbId"/> of the entry.</param>
		/// <param name="dateTime">The time at which the entry was created.</param>
		/// <param name="sequenceNumber">The ordered sequence number of the entry.</param>
		internal DbLogEntry(DbId entryId, DbId connectionId, System.DateTime dateTime, long sequenceNumber)
		{
			this.EntryId = entryId;
			this.ConnectionId = connectionId;
			this.DateTime = dateTime;
			this.SequenceNumber = sequenceNumber;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the entry represented by this instance.
		/// </summary>
		public DbId EntryId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the connection referenced by the entry represented by this
		/// instance.
		/// </summary>
		public DbId ConnectionId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the time at which the entry represented by this instance has been created.
		/// </summary>
		public System.DateTime DateTime
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the unique ordered sequence number of the entry represented by this instance.
		/// </summary>
		public long SequenceNumber
		{
			get;
			private set;
		}


	}


}