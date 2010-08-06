using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	internal abstract class AbstractSynchronisationJob
	{


		protected AbstractSynchronisationJob(int dataContextId, EntityKey entityKey)
		{
			this.DataContextId = dataContextId;
			this.EntityKey = entityKey;
		}
		

		public int DataContextId
		{
			get;
			set;
		}


		public EntityKey EntityKey
		{
			get;
			private set;
		}


	}


}
