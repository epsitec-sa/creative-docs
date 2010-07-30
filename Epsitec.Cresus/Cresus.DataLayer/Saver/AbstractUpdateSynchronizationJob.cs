using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;


namespace Epsitec.Cresus.DataLayer.Saver
{


	internal abstract class AbstractUpdateSynchronizationJob : AbstractSynchronisationJob
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
