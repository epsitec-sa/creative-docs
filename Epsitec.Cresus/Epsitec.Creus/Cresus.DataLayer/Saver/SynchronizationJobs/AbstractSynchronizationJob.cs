using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>AbstractSynchronizationJob</c> is the base class for all the descriptions of the
	/// modifications that have been made in a <see cref="DataContext"/> and that are used to update
	/// other <see cref="DataContext"/>.
	/// </summary>
	internal abstract class AbstractSynchronizationJob
	{


		/// <summary>
		/// Creates a new <c>AbstractSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>AbstractSynchronizationJob</c>.</param>
		/// <param name="entityKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted by the <c>AbstractSynchronizationJob</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="entityKey"/> is empty.</exception>
		protected AbstractSynchronizationJob(long dataContextId, EntityKey entityKey)
		{
			entityKey.ThrowIf (k => k.IsEmpty, "entityKey cannot be empty");
			
			this.DataContextId = dataContextId;
			this.EntityKey = entityKey;
		}
		

		/// <summary>
		/// Gets the unique id of the <see cref="DataContext"/> that created this instance.
		/// </summary>
		public long DataContextId
		{
			get;
			private set;
		}


		/// <summary>
		/// Gets the <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted
		/// by this instance.
		/// </summary>
		public EntityKey EntityKey
		{
			get;
			private set;
		}


		/// <summary>
		/// Calls the appropriate method in order to apply the modifications of this instance to the
		/// given <see cref="DataContext"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> to which apply the modifications of this instance.</param>
		public abstract void Synchronize(DataContext dataContext);


	}


}
