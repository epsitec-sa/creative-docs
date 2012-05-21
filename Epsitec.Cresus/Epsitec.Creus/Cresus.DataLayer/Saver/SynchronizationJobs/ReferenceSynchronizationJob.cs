using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>ReferenceSynchronizationJob</c> class describes the modifications that have been made
	/// to a reference field of an <see cref="AbstractEntity"/>.
	/// </summary>
	internal class ReferenceSynchronizationJob : AbstractFieldSynchronizationJob
	{


		/// <summary>
		/// Creates a new <c>ReferenceSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="sourceKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted by the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field targeted by the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="newTargetKey">The <see cref="EntityKey"/> of the new target of the field targeted by the <c>ValueSynchronizationJob</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="sourceKey"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="newTargetKey"/> is empty.</exception>
		public ReferenceSynchronizationJob(long dataContextId, EntityKey sourceKey, Druid fieldId, EntityKey? newTargetKey)
			: base (dataContextId, sourceKey, fieldId)
		{
			newTargetKey.ThrowIf (t => t.HasValue && t.Value == EntityKey.Empty, "newTargetKey cannot be empty.");

			this.NewTargetKey = newTargetKey;
		}


		/// <summary>
		/// The <see cref="EntityKey"/> that identifies the new target of the field of the
		/// <see cref="AbstractEntity"/> targeted by this instance.
		/// </summary>
		public EntityKey? NewTargetKey
		{
			get;
			private set;
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
