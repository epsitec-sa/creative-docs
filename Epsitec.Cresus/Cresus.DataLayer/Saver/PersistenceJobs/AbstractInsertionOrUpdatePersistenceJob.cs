using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{
	
	internal abstract class AbstractInsertionOrUpdatePersistenceJob : AbstractPersistenceJob
	{


		protected AbstractInsertionOrUpdatePersistenceJob(AbstractEntity entity, Druid localEntityId, PersistenceJobType jobType) : base (entity)
		{
			this.LocalEntityId = localEntityId;
			this.JobType = jobType;
		}


		public Druid LocalEntityId
		{
			get;
			private set;
		}


		public PersistenceJobType JobType
		{
			get;
			private set;
		}


	}


}
