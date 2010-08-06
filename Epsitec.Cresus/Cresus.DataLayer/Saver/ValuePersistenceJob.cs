using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class ValuePersistenceJob : AbstractInsertionOrUpdatePersistenceJob
	{


		public ValuePersistenceJob(AbstractEntity entity, Druid localEntityId, Dictionary<Druid, object> fieldIdsWithValues, PersistenceJobType jobType)
			: base (entity, localEntityId, jobType)
		{
			this.fieldIdsWithValues = new Dictionary<Druid, object> (fieldIdsWithValues);
		}


		public IEnumerable<KeyValuePair<Druid, object>> GetFieldIdsWithValues()
		{
			return this.fieldIdsWithValues;
		}


		private Dictionary<Druid, object> fieldIdsWithValues;


	}


}
