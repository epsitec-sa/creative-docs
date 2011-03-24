using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Saver.PersistenceJobs;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Saver
{


	/// <summary>
	/// The <c>PersistenceJobTableComputer</c> class is used to assess which <see cref="DbTable"/>
	/// will be affected by <see cref="AbstractPersistenceJob"/> when they are executed.
	/// </summary>
	internal sealed class PersistenceJobTableComputer
	{


		/// <summary>
		/// Builds a new instance of <c>PersistenceJobTableComputer</c>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> used by this instance.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dataContext"/> is <c>null</c>.</exception>
		public PersistenceJobTableComputer(DataContext dataContext)
		{
			dataContext.ThrowIfNull ("dataContext");

			this.DataContext = dataContext;
		}


		/// <summary>
		/// The <see cref="DataContext"/> used by this instance.
		/// </summary>
		private DataContext DataContext
		{
			get;
			set;
		}


		/// <summary>
		/// The <see cref="SchemaEngine"/> used by this instance.
		/// </summary>
		private EntitySchemaEngine SchemaEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.SchemaEngine;
			}
		}


		private EntityTypeEngine EntityTypeEngine
		{
			get
			{
				return this.DataContext.DataInfrastructure.EntityTypeEngine;
			}
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="DeletePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="DeletePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="DeletePersistenceJob"/>.</returns>
		public IEnumerable<DbTable> GetAffectedTables(DeletePersistenceJob job)
		{
			job.ThrowIfNull ("job");

			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			return this.GetAffectedTablesHelper (job);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="DeletePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="DeletePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="DeletePersistenceJob"/>.</returns>
		private IEnumerable<DbTable> GetAffectedTablesHelper(DeletePersistenceJob job)
		{
			Druid leafEntityId = job.Entity.GetEntityStructuredTypeId ();

			IEnumerable<DbTable> localEntityTables =
				from localEntityType in this.EntityTypeEngine.GetBaseTypes (leafEntityId)
				select this.SchemaEngine.GetEntityTable (localEntityType.CaptionId);

			IEnumerable<DbTable> collectionOutTables = 
				from localEntityType in this.EntityTypeEngine.GetBaseTypes (leafEntityId)
				let localEntityId = localEntityType.CaptionId
				from field in this.EntityTypeEngine.GetLocalCollectionFields (localEntityId)
				let fieldId = field.CaptionId
				select this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);

			List<DbTable> referenceInTables = new List<DbTable> ();
			List<DbTable> collectionInTables = new List<DbTable> ();

			foreach (var item in this.EntityTypeEngine.GetReferencingFields (leafEntityId))
			{
				Druid localSourceEntityId = item.Key.CaptionId;

				foreach (var field in item.Value)
				{
					Druid fieldId = field.CaptionId;
					
					switch (field.Relation)
					{
						case FieldRelation.Reference:
							referenceInTables.Add (this.SchemaEngine.GetEntityTable (localSourceEntityId));
							break;

						case FieldRelation.Collection:
							collectionInTables.Add (this.SchemaEngine.GetEntityFieldTable (localSourceEntityId, fieldId));
							break;

						default:
							throw new System.NotImplementedException ();
					}
				}
			}

			return localEntityTables
				.Concat (collectionOutTables)
				.Concat (collectionInTables)
				.Concat (referenceInTables);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="ValuePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="ValuePersistenceJob"/>.</returns>
		public IEnumerable<DbTable> GetAffectedTables(ValuePersistenceJob job)
		{
			job.ThrowIfNull ("job");

			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			return this.GetAffectedTablesHelper (job);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="ValuePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ValuePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="ValuePersistenceJob"/>.</returns>
		private IEnumerable<DbTable> GetAffectedTablesHelper(ValuePersistenceJob job)
		{
			Druid localEntityId = job.LocalEntityId;

			yield return this.SchemaEngine.GetEntityTable (localEntityId);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="ReferencePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="ReferencePersistenceJob"/>.</returns>
		public IEnumerable<DbTable> GetAffectedTables(ReferencePersistenceJob job)
		{
			job.ThrowIfNull ("job");

			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			return this.GetAffectedTablesHelper (job);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="ReferencePersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="ReferencePersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="ReferencePersistenceJob"/>.</returns>
		private IEnumerable<DbTable> GetAffectedTablesHelper(ReferencePersistenceJob job)
		{
			Druid localEntityId = job.LocalEntityId;

			yield return this.SchemaEngine.GetEntityTable (localEntityId);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="CollectionPersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="CollectionPersistenceJob"/>.</returns>
		public IEnumerable<DbTable> GetAffectedTables(CollectionPersistenceJob job)
		{
			job.ThrowIfNull ("job");

			// This method does not do the job itself but call an helper method so the arguments are
			// checked immediately and the execution of the helper is deferred.
			// Marc.

			return this.GetAffectedTablesHelper (job);
		}


		/// <summary>
		/// Gets all the <see cref="DbTable"/> that will be affected by the execution of a given
		/// <see cref="CollectionPersistenceJob"/>.
		/// </summary>
		/// <param name="job">The <see cref="CollectionPersistenceJob"/> whose affected <see cref="DbTable"/> to compute.</param>
		/// <returns>The <see cref="DbTable"/> that will be affected by the <see cref="CollectionPersistenceJob"/>.</returns>
		private IEnumerable<DbTable> GetAffectedTablesHelper(CollectionPersistenceJob job)
		{
			Druid localEntityId = job.LocalEntityId;
			Druid fieldId = job.FieldId;

			yield return this.SchemaEngine.GetEntityFieldTable (localEntityId, fieldId);
		}


	}


}
