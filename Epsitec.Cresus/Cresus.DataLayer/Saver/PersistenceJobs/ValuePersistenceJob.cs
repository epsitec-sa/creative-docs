using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Saver.PersistenceJobs
{


	internal class ValuePersistenceJob : AbstractFieldPersistenceJob
	{


		public ValuePersistenceJob(AbstractEntity entity, Druid localEntityId, Dictionary<Druid, object> fieldIdsWithValues, bool IsRootTypeJob, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			this.fieldIdsWithValues = new Dictionary<Druid, object> (fieldIdsWithValues);
			this.IsRootTypeJob = IsRootTypeJob;
		}


		public IEnumerable<KeyValuePair<Druid, object>> GetFieldIdsWithValues()
		{
			return this.fieldIdsWithValues;
		}


		public bool IsRootTypeJob
		{
			get;
			private set;
		}


		private Dictionary<Druid, object> fieldIdsWithValues;


	}


}
