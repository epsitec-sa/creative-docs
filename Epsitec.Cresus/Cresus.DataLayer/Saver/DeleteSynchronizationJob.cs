using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class DeleteSynchronizationJob : AbstractSynchronisationJob
	{


		public DeleteSynchronizationJob(int dataContextId, EntityKey entityKey)
			: base (dataContextId, entityKey)
		{
		}


		protected override void Synchronize(DataContext dataContext)
		{
			throw new System.NotImplementedException ();
		}


	}


}
