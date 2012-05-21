using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>EntityDeletionEntry</c> is an immutable class that describes an entry in the
	/// entity deletion log table.
	/// </summary>
	internal sealed class EntityDeletionEntry
	{
		

		/// <summary>
		/// Builds a new <c>DbDeletionLogEntry.</c>
		/// </summary>
		/// <param name="entryId">The id of the deletion log entry.</param>
		/// <param name="entityModificationEntryId">The id of the modification log entry related to it.</param>
		/// <param name="entityTypeId">The <see cref="Druid"/> that defined the type of the entity that has been deleted.</param>
		/// <param name="entityId">The id of the entity that has been deleted.</param>
		public EntityDeletionEntry(DbId entryId, DbId entityModificationEntryId, Druid entityTypeId, DbId entityId)
		{
			entryId.ThrowIf (id => id.IsEmpty, "entryId cannot be empty.");
			entityModificationEntryId.ThrowIf (id => id.IsEmpty, "entityModificationEntryId cannot be empty.");
			entityTypeId.ThrowIf (id => id.IsEmpty, "entityTypeId cannot be empty.");
			entityId.ThrowIf (id => id.IsEmpty, "entityId cannot be empty.");

			this.entryId = entryId;
			this.entityModificationEntryId = entityModificationEntryId;
			this.entityTypeId = entityTypeId;
			this.entityId = entityId;
		}


		/// <summary>
		/// Gets the id of the deletion log entry.
		/// </summary>
		public DbId EntryId
		{
			get
			{
				return this.entryId;
			}
		}


		/// <summary>
		/// Gets the id of the modification log entry related to this instance.
		/// </summary>
		public DbId EntityModificationEntryId
		{
			get
			{
				return this.entityModificationEntryId;
			}
		}


		/// <summary>
		/// Gets the long that defines the Druid that defines the concrete type of the entity that
		/// has been deleted.
		/// </summary>
		public Druid EntityTypeId
		{
			get
			{
				return this.entityTypeId;
			}
		}


		/// <summary>
		/// Gets the id of the entity that has been deleted.
		/// </summary>
		public DbId EntityId
		{
			get
			{
				return this.entityId;
			}
		}


		private readonly DbId entryId;


		private readonly DbId entityModificationEntryId;


		private readonly Druid entityTypeId;


		private readonly DbId entityId;

	}


}
