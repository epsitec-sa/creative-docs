using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>DeletePersistenceJob</c> class indicates that an <see cref="AbstractEntity"/> has been
	/// deleted and that it must be removed with all its references from the database.
	/// </summary>
	internal class DeletePersistenceJob : AbstractPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>DeletePersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>DeletePersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		public DeletePersistenceJob(AbstractEntity entity) : base (entity)
		{
		}


	}


}
