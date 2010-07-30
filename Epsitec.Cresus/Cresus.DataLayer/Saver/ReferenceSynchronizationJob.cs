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


		protected EntityKey? OldValue
		{
			get;
			private set;
		}


		protected EntityKey? NewValue
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
