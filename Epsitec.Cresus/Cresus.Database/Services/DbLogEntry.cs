using System.Collections.Generic;


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
		internal DbLogEntry(DbId entryId, DbId connectionId, System.DateTime dateTime)
		{
			this.EntryId = entryId;
			this.ConnectionId = connectionId;
			this.DateTime = dateTime;
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
		/// Creates a new instance of <see cref="DbLogEntry"/> based on the given data.
		/// </summary>
		/// <param name="data">The data of the log entry.</param>
		/// <returns>The <see cref="DbLogEntry"/>.</returns>
		internal static DbLogEntry CreateDbLogEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId connectionId = new DbId ((long) data[1]);
			System.DateTime dateTime = (System.DateTime) data[2];

			return new DbLogEntry (entryId, connectionId, dateTime);
		}


	}


}