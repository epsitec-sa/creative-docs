using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbEntityDeletionLogEntry</c> is an immutable class that describes an entry in the
	/// entity deletion log table.
	/// </summary>
	public sealed class DbEntityDeletionLogEntry
	{
		

		/// <summary>
		/// Builds a new <c>DbDeletionLogEntry.</c>
		/// </summary>
		/// <param name="entryId">The id of the deletion log entry.</param>
		/// <param name="logEntryId">The id of the log entry related to it.</param>
		/// <param name="instanceType">The long that defines the Druid of the instance type of the entity that has been deleted.</param>
		/// <param name="entityId">The id of the entity that has been deleted.</param>
		internal DbEntityDeletionLogEntry(DbId entryId, DbId logEntryId, long instanceType, DbId entityId)
		{
			this.EntryId = entryId;
			this.LogEntryId = logEntryId;
			this.InstanceType = instanceType;
			this.EntityId = entityId;
		}


		/// <summary>
		/// Gets the id of the deletion log entry.
		/// </summary>
		public DbId EntryId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the id of the log entry related to this instance.
		/// </summary>
		public DbId LogEntryId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the long that defines the Druid that defines the concrete type of the entity that
		/// has been deleted.
		/// </summary>
		public long InstanceType
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the id of the entity that has been deleted.
		/// </summary>
		public DbId EntityId
		{
			get;
			private set;
		}


		/// <summary>
		/// Creates a new instance of <see cref="DbEntityDeletionLogEntry"/> based on the given data.
		/// </summary>
		/// <param name="data">The data of the deletion log entry.</param>
		/// <returns>The <see cref="DbEntityDeletionLogEntry"/>.</returns>
		internal static DbEntityDeletionLogEntry CreateDbEntityDeletionEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId logEntryId = new DbId ((long) data[1]);
			long instanceType = (long) data[2];
			DbId entityId = new DbId ((long) data[3]);

			return new DbEntityDeletionLogEntry (entryId, logEntryId, instanceType, entityId);
		}


	}


}
