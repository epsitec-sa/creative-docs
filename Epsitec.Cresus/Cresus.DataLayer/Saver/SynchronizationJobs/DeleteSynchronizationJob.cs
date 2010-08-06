using Epsitec.Common.Support.EntityEngine;

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
		public DeleteSynchronizationJob(int dataContextId, EntityKey entityKey)
			: base (dataContextId, entityKey)
		{
		}


	}


}
