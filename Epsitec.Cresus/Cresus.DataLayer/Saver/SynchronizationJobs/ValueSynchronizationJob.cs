using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	internal class ValueSynchronizationJob : AbstractUpdateSynchronizationJob
	{


		public ValueSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, object newValue)
			: base (dataContextId, entityKey, fieldId)
		{
			this.NewValue = newValue;
		}


		public object NewValue
		{
			get;
			private set;
		}


	}


}
