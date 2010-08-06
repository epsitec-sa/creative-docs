using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	internal class CollectionPersistenceJob : AbstractInsertionOrUpdatePersistenceJob
	{


		public CollectionPersistenceJob(AbstractEntity entity, Druid localEntityId, Druid fieldId, IEnumerable<AbstractEntity> targets, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			this.FieldId = fieldId;
			this.Targets = targets.ToList ();
		}


		public Druid FieldId
		{
			get;
			private set;
		}

		
		public IEnumerable<AbstractEntity> Targets
		{
			get;
			private set;
		}


	}


}
