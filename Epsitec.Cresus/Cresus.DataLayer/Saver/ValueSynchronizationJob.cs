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


		protected object OldValue
		{
			get;
			private set;
		}


		protected object NewValue
		{
			get;
			private set;
		}


		protected override void Synchronize(DataContext dataContext)
		{
			throw new System.NotImplementedException ();
		}


	}


}
