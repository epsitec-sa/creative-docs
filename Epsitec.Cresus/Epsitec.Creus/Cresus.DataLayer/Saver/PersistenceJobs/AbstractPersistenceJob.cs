using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;


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


		/// <summary>
		/// Calls the appropriate method that will convert this instance into an equivalent sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="converter">The <see cref="PersistenceJobConverter"/> to use for the conversion.</param>
		/// <returns>The sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
		public abstract IEnumerable<AbstractSynchronizationJob> Convert(PersistenceJobConverter converter);


		/// <summary>
		/// Gets the <see cref="DbTable"/> that will be affected in the database when this instance
		/// will be processed.
		/// </summary>
		/// <param name="tableComputer">The <see cref="PersistenceJobTableComputer"/> that will be used for the computation.</param>
		/// <returns>The sequence of <see cref="DbTable"/> affected by this instance.</returns>
		public abstract IEnumerable<DbTable> GetAffectedTables(PersistenceJobTableComputer tableComputer);

		
	}


}
