using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>ReferencePersistenceJob</c> class describes the modifications that have been made to
	/// the reference fields of an <see cref="AbstractEntity"/> and that are to be persisted in
	/// the database. It contains all the modifications of the reference fields of a given subtype
	/// of the <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ReferencePersistenceJob : AbstractFieldPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>ReferencePersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type that holds the fields concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="fieldIdsWithTargets">The mapping between the modified fields and their values.</param>
		/// <param name="jobType">The job type of the <c>ReferencePersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="localEntityId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldIdsWithTargets"/> is empty.</exception>
		public ReferencePersistenceJob(AbstractEntity entity, Druid localEntityId, Dictionary<Druid, AbstractEntity> fieldIdsWithTargets, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			fieldIdsWithTargets.ThrowIfNull ("fieldIdsWithTargets");

			this.fieldIdsWithTargets = new Dictionary<Druid, AbstractEntity> (fieldIdsWithTargets);
		}


		/// <summary>
		/// The mapping between the field ids and their values.
		/// </summary>
		public IEnumerable<KeyValuePair<Druid, AbstractEntity>> GetFieldIdsWithTargets()
		{
			return this.fieldIdsWithTargets;
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
		/// <returns> The sequence of <see cref="DbTable"/> affected by this instance.</returns>
		public override IEnumerable<DbTable> GetAffectedTables(PersistenceJobTableComputer tableComputer)
		{
			tableComputer.ThrowIfNull ("tableComputer");

			return tableComputer.GetAffectedTables (this);
		}


		/// <summary>
		/// Holds the mapping between the field ids and their values.
		/// </summary>
		private readonly Dictionary<Druid, AbstractEntity> fieldIdsWithTargets;


	}


}
