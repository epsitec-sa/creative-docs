using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.DataLayer.Saver
{


	internal class ValueSynchronizationJob : AbstractUpdateSynchronizationJob
	{


		public ValueSynchronizationJob(int dataContextId, EntityKey entityKey, Druid fieldId, object oldValue, object newValue)
			: base (dataContextId, entityKey, fieldId)
		{
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}


		public object OldValue
		{
			get;
			private set;
		}


		public object NewValue
		{
			get;
			private set;
		}


	}


}
