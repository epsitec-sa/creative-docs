using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	internal class ReferenceSynchronizationJob : AbstractFieldSynchronizationJob
	{


		public ReferenceSynchronizationJob(int dataContextId, EntityKey sourceKey, Druid fieldId, EntityKey? newTargetKey)
			: base (dataContextId, sourceKey, fieldId)
		{
			this.NewTargetKey = newTargetKey;
		}


		public EntityKey? NewTargetKey
		{
			get;
			private set;
		}


	}


}
