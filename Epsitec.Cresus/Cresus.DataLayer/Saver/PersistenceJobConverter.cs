using Epsitec.Common.Support;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	internal sealed class PersistenceJobConverter
	{



		public PersistenceJobConverter(DataContext dataContext)
		{
			this.DataContext = dataContext;
		}


		private DataContext DataContext
		{
			get;
			set;
		}


		public IEnumerable<AbstractSynchronizationJob> Convert(AbstractPersistenceJob job)
		{
			return this.Convert ((dynamic) job);
		}


		private IEnumerable<DeleteSynchronizationJob> Convert(DeletePersistenceJob job)
		{
			int dataContextId = this.DataContext.UniqueId;
			EntityKey entityKey = this.DataContext.GetEntityKey (job.Entity).Value;

			yield return new DeleteSynchronizationJob (dataContextId, entityKey);
		}


		private IEnumerable<ValueSynchronizationJob> Convert(ValuePersistenceJob job)
		{
			if (job.JobType == PersistenceJobType.Update)
			{
				int dataContextId = this.DataContext.UniqueId;
				EntityKey entityKey = this.DataContext.GetEntityKey (job.Entity).Value;
				
				foreach (var update in job.GetFieldIdsWithValues ())
				{
					Druid fieldId = update.Key;
					object value = update.Value;

					yield return new ValueSynchronizationJob (dataContextId, entityKey, fieldId, value);
				}
			}
		}


		private IEnumerable<ReferenceSynchronizationJob> Convert(ReferencePersistenceJob job)
		{
			if (job.JobType == PersistenceJobType.Update)
			{
				int dataContextId = this.DataContext.UniqueId;
				EntityKey sourceKey = this.DataContext.GetEntityKey (job.Entity).Value;
				Druid fieldId = job.FieldId;
				
				EntityKey? targetKey;

				if (job.Target == null)
				{
					targetKey = null;
				}
				else
				{
					targetKey = this.DataContext.GetEntityKey (job.Target).Value;
				}

				yield return new ReferenceSynchronizationJob (dataContextId, sourceKey, fieldId, targetKey);
			}
		}


		private IEnumerable<CollectionSynchronizationJob> Convert(CollectionPersistenceJob job)
		{
			if (job.JobType == PersistenceJobType.Update)
			{
				int dataContextId = this.DataContext.UniqueId;
				EntityKey sourceKey = this.DataContext.GetEntityKey (job.Entity).Value;
				Druid fieldId = job.FieldId;

				IEnumerable<EntityKey> targetKeys = job.Targets.Select (e => this.DataContext.GetEntityKey (e).Value);

				yield return new CollectionSynchronizationJob (dataContextId, sourceKey, fieldId, targetKeys);
			}
		}


	}


}
