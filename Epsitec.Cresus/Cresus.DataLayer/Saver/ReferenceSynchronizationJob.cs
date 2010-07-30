using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class ReferenceSynchronizationJob : AbstractUpdateSynchronizationJob
	{


		public ReferenceSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, EntityKey? newValue)
			: base (dataContextId, entityKey, fieldId)
		{
			this.NewValue = newValue;
		}


		public EntityKey? NewValue
		{
			get;
			private set;
		}


	}


}
