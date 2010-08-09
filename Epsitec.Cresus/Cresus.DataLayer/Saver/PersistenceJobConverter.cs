using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Saver.SynchronizationJobs;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	/// <summary>
	/// The<c>PersistenceJobConverter</c> class is used to transform <see cref="AbstractPersistenceJob"/>
	/// into the corresponding <see cref="AbstractSynchronizationJob"/>.
	/// </summary>
	internal sealed class PersistenceJobConverter
	{
		

		/// <summary>
		/// Creates a new <c>PersistenceJobConverter</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> that will be used to create the <see cref="AbstractSynchronizationJob"/>.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public PersistenceJobConverter(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> that is used to create the <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		/// <summary>
		/// Converts the given <see cref="AbstractPersistenceJob"/> into the corresponding sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="AbstractPersistenceJob"/> to convert.</param>
		/// <returns>The converted sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
		/// <exception cref="System.ArgumentNullException">If <paramref name="job"/> is <c>null</c>.</exception>
		public IEnumerable<AbstractSynchronizationJob> Convert(AbstractPersistenceJob job)
		{
			job.ThrowIfNull ("job");

			return this.Convert ((dynamic) job);
		}


		/// <summary>
		/// Converts the given <see cref="DeletePersistenceJob"/> into the corresponding sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="DeletePersistenceJob"/> to convert.</param>
		/// <returns>The converted sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
		private IEnumerable<DeleteSynchronizationJob> Convert(DeletePersistenceJob job)
		{
			int dataContextId = this.DataContext.UniqueId;
			EntityKey entityKey = this.DataContext.GetEntityKey (job.Entity).Value;

			yield return new DeleteSynchronizationJob (dataContextId, entityKey);
		}


		/// <summary>
		/// Converts the given <see cref="ValuePersistenceJob"/> into the corresponding sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> to convert.</param>
		/// <returns>The converted sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
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


		/// <summary>
		/// Converts the given <see cref="ReferencePersistenceJob"/> into the corresponding sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> to convert.</param>
		/// <returns>The converted sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
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


		/// <summary>
		/// Converts the given <see cref="CollectionPersistenceJob"/> into the corresponding sequence
		/// of <see cref="AbstractSynchronizationJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> to convert.</param>
		/// <returns>The converted sequence of <see cref="AbstractSynchronizationJob"/>.</returns>
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
