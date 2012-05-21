using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;


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
