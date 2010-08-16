using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>CollectionPersistenceJob</c> class describes the modifications that have been made to
	/// a single collection field of an <see cref="AbstractEntity"/> and that are to be persisted in
	/// the database.
	/// </summary>
	internal class CollectionPersistenceJob : AbstractFieldPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>CollectionPersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>CollectionPersistenceJob</c>.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type that holds the fields concerned by the <c>CollectionPersistenceJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field that has been modified.</param>
		/// <param name="targets">The new targets of the field that has been modified.</param>
		/// <param name="jobType">The job type of the <c>CollectionPersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="localEntityId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="targets"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="targets"/> contains <c>null</c>.</exception>
		public CollectionPersistenceJob(AbstractEntity entity, Druid localEntityId, Druid fieldId, IEnumerable<AbstractEntity> targets, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			fieldId.ThrowIf (f => f.IsEmpty, "fieldId cannot be empty");
			targets.ThrowIfNull ("targets");

			this.FieldId = fieldId;
			this.Targets = targets.ToList ();

			this.Targets.ThrowIf (t => t.Contains (null), "targets cannot contain null.");
		}


		/// <summary>
		/// The <see cref="Druid"/> of the field that has been modified.
		/// </summary>
		public Druid FieldId
		{
			get;
			private set;
		}

		
		/// <summary>
		/// The new targets of the field that has been modified.
		/// </summary>
		public IEnumerable<AbstractEntity> Targets
		{
			get;
			private set;
		}


		/// <summary>
		/// Calls the appropriate method that will convert this instance into an equivalent sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="converter">The <see cref="PersistenceJobConverter"/> to use for the conversion.</param>
		/// <returns> The sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="converter"/> is <c>null</c>.</exception>
		public override IEnumerable<AbstractSynchronizationJob> Convert(PersistenceJobConverter converter)
		{
			converter.ThrowIfNull ("converter");

			return converter.Convert (this);
		}


		/// <summary>
		/// Gets the <see cref="DbTable"/> that will be affected in the database when this instance
		/// will be processed.
		/// </summary>
		/// <param name="tableComputer">The <see cref="PersistenceJobTableComputer"/> that will be used for the computation.</param>
		/// <returns>The sequence of <see cref="DbTable"/> affected by this instance.</returns>
		public override IEnumerable<DbTable> GetAffectedTables(PersistenceJobTableComputer tableComputer)
		{
			tableComputer.ThrowIfNull ("tableComputer");

			return tableComputer.GetAffectedTables (this);
		}


	}


}
