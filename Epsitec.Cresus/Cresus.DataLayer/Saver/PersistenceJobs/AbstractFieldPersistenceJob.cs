using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{
	
	/// <summary>
	/// The <c>AbstractFieldPersistenceJob</c> class is the base class for the description of all
	/// the modification that have been made to the fields of an <see cref="AbstractEntity"/> and
	/// that are to be persisted in the database.
	/// </summary>
	internal abstract class AbstractFieldPersistenceJob : AbstractPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>AbstractFieldPersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>AbstractFieldPersistenceJob</c>.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type that holds the fields concerned by the <c>AbstractFieldPersistenceJob</c>.</param>
		/// <param name="jobType">The job type of the <c>AbstractFieldPersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="localEntityId"/> is empty.</exception>
		protected AbstractFieldPersistenceJob(AbstractEntity entity, Druid localEntityId, PersistenceJobType jobType) : base (entity)
		{
			localEntityId.ThrowIf (l => l.IsEmpty, "localEntityId cannot be null.");
			
			this.LocalEntityId = localEntityId;
			this.JobType = jobType;
		}


		/// <summary>
		/// The <see cref="Druid"/> of the <see cref="AbstractEntity"/> type that holds the fields
		/// that are targeted by this instance.
		/// </summary>
		public Druid LocalEntityId
		{
			get;
			private set;
		}


		/// <summary>
		/// The type of the job, that is, if it is an insertion or an update.
		/// </summary>
		public PersistenceJobType JobType
		{
			get;
			private set;
		}


	}


}
