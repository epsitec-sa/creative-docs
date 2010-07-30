using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
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
