using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	internal class ReferencePersistenceJob : AbstractFieldPersistenceJob
	{


		public ReferencePersistenceJob(AbstractEntity entity, Druid localEntityId, Druid fieldId, AbstractEntity target, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			this.FieldId = fieldId;
			this.Target = target;
		}


		public Druid FieldId
		{
			get;
			private set;
		}


		public AbstractEntity Target
		{
			get;
			private set;
		}


	}


}
