using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	
	
	/// <summary>
	/// The <c>EntityModificationEntry</c> class is an immutable object that represents an entry in
	/// the entity modification log.
	/// </summary>
	public sealed class EntityModificationEntry
	{
		
		
		/// <summary>
		/// Builds a new <c>EntityModificationEntry</c>.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry.</param>
		/// <param name="connectionId">The connection <see cref="DbId"/> of the entry.</param>
		/// <param name="time">The time at which the entry was created.</param>
		internal EntityModificationEntry(DbId entryId, DbId connectionId, System.DateTime time)
		{
			entryId.ThrowIf (id => id.IsEmpty, "entryId cannot be empty");
			connectionId.ThrowIf (id => id.IsEmpty, "connectionId cannot be empty");

			this.entryId = entryId;
			this.connectionId = connectionId;
			this.time = time;
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the entry represented by this instance.
		/// </summary>
		public DbId EntryId
		{
			get
			{
				return this.entryId;
			}
		}


		/// <summary>
		/// Gets the <see cref="DbId"/> of the connection referenced by the entry represented by this
		/// instance.
		/// </summary>
		public DbId ConnectionId
		{
			get
			{
				return this.connectionId;
			}
		}


		/// <summary>
		/// Gets the time at which the entry represented by this instance has been created.
		/// </summary>
		public System.DateTime Time
		{
			get
			{
				return this.time;
			}
		}


		private readonly DbId entryId;


		private readonly DbId connectionId;


		private readonly System.DateTime time;

	}


}