using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	/// <summary>
	/// The <c>CollectionSynchronizationJob</c> class describes the modifications that have been made
	/// to a collection field of an <see cref="AbstractEntity"/>.
	/// </summary>
	internal class CollectionSynchronizationJob : AbstractFieldSynchronizationJob
	{


		/// <summary>
		/// Creates a new <c>ReferenceSynchronizationJob</c>.
		/// </summary>
		/// <param name="dataContextId">The unique id of the <see cref="DataContext"/> that is creating the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="sourceKey">The <see cref="EntityKey"/> that identifies the <see cref="AbstractEntity"/> targeted by the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="fieldId">The <see cref="Druid"/> of the field targeted by the <c>ReferenceSynchronizationJob</c>.</param>
		/// <param name="newTargetKeys">The <see cref="EntityKey"/> of the new targets of the field targeted by the <c>ValueSynchronizationJob</c>.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="sourceKey"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="fieldId"/> is empty.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="newTargetKeys"/> is <c>null</c>.</exception>
		public CollectionSynchronizationJob(int dataContextId, EntityKey sourceKey, Druid fieldId, IEnumerable<EntityKey> newTargetKeys)
			: base (dataContextId, sourceKey, fieldId)
		{
			newTargetKeys.ThrowIfNull ("newTargetKeys");

			this.NewTargetKeys = newTargetKeys.ToList ();
		}


		/// <summary>
		/// The <see cref="EntityKey"/> of the <see cref="AbstractEntity"/> targeted by the field of
		/// the <see cref="AbstractEntity"/> targeted by this instance.
		/// </summary>
		public IEnumerable<EntityKey> NewTargetKeys
		{
			get;
			private set;
		}


	}


}
