using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class ReferenceSynchronizationJob : AbstractUpdateSynchronizationJob
	{


		public ReferenceSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, EntityKey? oldValue, EntityKey? newValue)
			: base (dataContextId, entityKey, fieldId)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}


		public EntityKey? OldValue
		{
			get;
			private set;
		}


		public EntityKey? NewValue
		{
			get;
			private set;
		}


	}


}
