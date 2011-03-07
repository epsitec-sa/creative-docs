using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>ValuePersistenceJob</c> class describes the modifications that have been made to the
	/// value fields of an <see cref="AbstractEntity"/>. It contains all the modifications of the
	/// value fields of a given subtype of the <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ValuePersistenceJob : AbstractFieldPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>ValuePersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>ValuePersistenceJob</c>.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type that holds the fields concerned by the <c>ValuePersistenceJob</c>.</param>
		/// <param name="fieldIdsWithValues">The mapping between the modified fields and their values.</param>
		/// <param name="IsRootTypeJob">Indicates whether <paramref name="localEntityId"/> is the root type of the <see cref="AbstractEntity"/> or not.</param>
		/// <param name="jobType">The job type of the <c>ValuePersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="localEntityId"/> is empty.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="fieldIdsWithValues"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldIdsWithValues"/> contains an empty <see cref="Druid"/>.</exception>
		public ValuePersistenceJob(AbstractEntity entity, Druid localEntityId, Dictionary<Druid, object> fieldIdsWithValues, bool IsRootTypeJob, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			fieldIdsWithValues.ThrowIfNull ("fieldIdsWithValues");
			fieldIdsWithValues.ThrowIf (fv => fv.ContainsKey (Druid.Empty), "fieldIdsWithValues cannot contain empty druids");

			this.fieldIdsWithValues = new Dictionary<Druid, object> (fieldIdsWithValues);
			this.IsRootTypeJob = IsRootTypeJob;
		}


		/// <summary>
		/// The mapping between the field ids and their values.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<KeyValuePair<Druid, object>> GetFieldIdsWithValues()
		{
			return this.fieldIdsWithValues;
		}


		/// <summary>
		/// Indicates whether this job concerns the root type of the <see cref="AbstractEntity"/>.
		/// </summary>
		public bool IsRootTypeJob
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


		/// <summary>
		/// Holds the mapping between the field ids and their values.
		/// </summary>
		private Dictionary<Druid, object> fieldIdsWithValues;


	}


}
