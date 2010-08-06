using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	internal class CollectionSynchronizationJob : AbstractFieldSynchronizationJob
	{


		public CollectionSynchronizationJob(int dataContextId, EntityKey sourceKey, Druid fieldId, IEnumerable<EntityKey> newTargetKeys)
			: base (dataContextId, sourceKey, fieldId)
		{
			this.NewTargetKeys = newTargetKeys.ToList ();
		}


		public IEnumerable<EntityKey> NewTargetKeys
		{
			get;
			private set;
		}


	}


}
