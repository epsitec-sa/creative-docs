using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class CollectionSynchronizationJob : AbstractUpdateSynchronizationJob
	{


		public CollectionSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, IEnumerable<EntityKey> newValues)
			: base (dataContextId, entityKey, fieldId)
		{
			this.NewValues = new ReadOnlyCollection<EntityKey> (newValues.ToList ());
		}


		public ReadOnlyCollection<EntityKey> NewValues
		{
			get;
			private set;
		}


	}


}
