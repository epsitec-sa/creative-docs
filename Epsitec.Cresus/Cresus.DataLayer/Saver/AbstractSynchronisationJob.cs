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
		

		private int DataContextId
		{
			get;
			set;
		}


		protected EntityKey EntityKey
		{
			get;
			private set;
		}


		public void Synchronize(IEnumerable<DataContext> dataContexts)
		{
			var dataContextsToSynchronize = dataContexts.Where (d => d.UniqueId != this.DataContextId);
			
			foreach (DataContext dataContext in dataContextsToSynchronize)
			{
				this.Synchronize (dataContext);
			}
		}


		protected abstract void Synchronize(DataContext dataContext);


	}


}
