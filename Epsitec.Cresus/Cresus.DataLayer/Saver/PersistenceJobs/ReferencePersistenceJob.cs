using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>ReferencePersistenceJob</c> class describes the modifications that have been made to
	/// a single reference field of an <see cref="AbstractEntity"/> and that are to be persisted in
	/// the database.
	/// </summary>
	internal class ReferencePersistenceJob : AbstractFieldPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>ReferencePersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="localEntityId">The <see cref="Druid"/> of the type that holds the fields concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="target">The new target of the field concerned by the <c>ReferencePersistenceJob</c>.</param>
		/// <param name="jobType">The job type of the <c>ReferencePersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="localEntityId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		public ReferencePersistenceJob(AbstractEntity entity, Druid localEntityId, Druid fieldId, AbstractEntity target, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			fieldId.ThrowIf (f => f.IsEmpty, "fieldId cannot be empty");
			
			this.FieldId = fieldId;
			this.Target = target;
		}


		/// <summary>
		/// The <see cref="Druid"/> of the modified field.
		/// </summary>
		public Druid FieldId
		{
			get;
			private set;
		}


		/// <summary>
		/// The new target of the modified field.
		/// </summary>
		public AbstractEntity Target
		{
			get;
			private set;
		}


	}


}
