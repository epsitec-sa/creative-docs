using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	/// <summary>
	/// The <c>AbstractPersistenceJob</c> class is the base class for all the descriptions of all
	/// the modifications that have been made to an <see cref="AbstractEntity"/> and that is used to
	/// persist these modifications to the database.
	/// </summary>
	internal abstract class AbstractPersistenceJob
	{


		/// <summary>
		/// Creates a new <c>AbstractPersistenceJob</c>.
		/// </summary>
		/// <param name="entity">The <see cref="AbstractEntity"/> concerned by the <c>AbstractPersistenceJob</c>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="entity"/> is <c>null</c>.</exception>
		protected AbstractPersistenceJob(AbstractEntity entity)
		{
			entity.ThrowIfNull ("entity");

			this.Entity = entity;
		}
		
		
		/// <summary>
		/// The <see cref="AbstractEntity"/> concerned by this instance.
		/// </summary>
		public AbstractEntity Entity
		{
			get;
			private set;
		}

		
	}


}
