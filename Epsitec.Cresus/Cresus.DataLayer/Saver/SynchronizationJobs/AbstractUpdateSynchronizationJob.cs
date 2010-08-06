using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs
{


	internal abstract class AbstractUpdateSynchronizationJob : AbstractSynchronizationJob
	{


		protected AbstractUpdateSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId)
			: base (dataContextId, entityKey)
		{
			this.FieldId = fieldId;
		}


		public Druid FieldId
		{
			get;
			private set;
		}


	}


}
