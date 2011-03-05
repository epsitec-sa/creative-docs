using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>DeleteSynchronizationJob</c> class is used to indicate that an <see cref="AbstractEntity"/>
	/// has been deleted in a <see cref="DataContext"/>.
	/// </summary>
	internal class DeleteSynchronizationJob : AbstractSynchronizationJob
	{
		

		/// <summary>
		/// Creates a new <c>DeleteSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>AbstractSynchronizationJob</c>.</param>
		/// <param name="entityKey">The <see cref="EntityKey"/> that identifies the deleted <see cref="AbstractEntity"/> targeted by the <c>AbstractSynchronizationJob.</c></param>
		public DeleteSynchronizationJob(long dataContextId, EntityKey entityKey)
			: base (dataContextId, entityKey)
		{
		}


		/// <summary>
		/// Calls the appropriate method in order to apply the modifications of this instance to the
		/// given <see cref="DataContext"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> to which apply the modifications of this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public override void Synchronize(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			dataContext.Synchronize (this);
		}


	}


}
