using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Metadata;

using System;
using System.Collections.Generic;

using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SetControllers
{
	/// <remarks>
	/// For all the comments here, assume that we are in the following case when I describe things
	/// for an example:
	/// - There are three entities, persons, groups, membership. Membership is a join entity that
	///   binds a person and a group together.
	/// - We want to express the set of persons that are within a group. So TEntity = group,
	///   TDisplay = membership and TPick = person.
	/// - If you like drawings, we have: person &lt;- membership -&gt; group.
	/// </remarks>
	public abstract class SetViewController<TEntity, TDisplay, TPick> : EntityViewController<TEntity>, ISetViewController
		where TEntity : AbstractEntity, new ()
		where TDisplay : AbstractEntity, new ()
		where TPick : AbstractEntity, new ()
	{
		protected sealed override void CreateBricks(BrickWall<TEntity> wall)
		{
			throw new NotSupportedException ();
		}

		/// <summary>
		/// Sets up the given dataSetAccessor so that is return only meaningfull data. In our
		/// example, we would add a filter clause that will filter out any membership entity that
		/// is not related to the current group.
		/// </summary>
		/// <param name="entity">
		/// The entity equivalent to this.Entity but in the DataContext of the DataSetAccessor.
		/// </param>
		/// <param name="dataSetAccessor">The DataSetAccessor to customize.</param>
		protected abstract void SetupDisplayDataSetAccessor(TEntity entity, DataSetAccessor dataSetAccessor);

		/// <summary>
		/// Sets up the given dataSetAccessor so that it returns only meaningfull data. In our
		/// example, we might filter out any person that is already a member of the current group.
		/// </summary>
		/// <param name="entity">
		/// The entity equivalent to this.Entity but in the DataContext of the DataSetAccessor.
		/// </param>
		/// <param name="dataSetAccessor">The DataSetAccessor to customize.</param>
		protected abstract void SetupPickDataSetAccessor(TEntity entity, DataSetAccessor dataSetAccessor);

		protected abstract void AddItems(IEnumerable<TPick> entitiesToAdd);

		protected abstract void RemoveItems(IEnumerable<TDisplay> entitiesToRemove);

		private TEntity GetEntityInDataSetDataContext(DataSetAccessor dataSetAccessor)
		{
			var sender = this.BusinessContext.DataContext;
			var receiver = dataSetAccessor.IsolatedDataContext;

			return DataLayer.Context.DataContext.CopyEntity (sender, this.Entity, receiver);
		}

		#region ISetViewController Members

		public abstract FormattedText GetTitle();

		public abstract string GetIcon();

		/// <summary>
		/// Gets the id of the command that identifies the data set that must be displayed to the
		/// user. Typicaly this will be a data set for the entity that makes the join between the
		/// two ends of the relation.
		/// In our example the data set would be a data set of membership entities.
		/// </summary>
		public abstract Druid GetDisplayDataSetId();

		/// <summary>
		/// Gets the id of the command that identified the data set from which the user will pick
		/// new items to add to the set. Typicall this will be a data set for the entity that is
		/// at the other end of the relation.
		/// In our example the data set would be a data set of person entities.
		/// </summary>
		public abstract Druid GetPickDataSetId();

		public DataSetAccessor GetDisplayDataSetAccessor(DataSetGetter dataSetGetter, DataStoreMetadata dataStoreMetadata)
		{
			var id = this.GetDisplayDataSetId ();
			var dataSet = dataStoreMetadata.FindDataSet (id);
			var accessor = dataSetGetter.ResolveAccessor (dataSet);

			accessor.DisableScopeFilter = true;

			var entity = this.GetEntityInDataSetDataContext (accessor);

			this.SetupDisplayDataSetAccessor (entity, accessor);

			return accessor;
		}

		public DataSetAccessor GetPickDataSetAccessor(DataSetGetter dataSetGetter, DataStoreMetadata dataStoreMetadata)
		{
			var id = this.GetPickDataSetId ();
			var dataSet = dataStoreMetadata.FindDataSet (id);
			var accessor = dataSetGetter.ResolveAccessor (dataSet);

			var entity = this.GetEntityInDataSetDataContext (accessor);

			this.SetupPickDataSetAccessor (entity, accessor);

			return accessor;
		}

		public void AddItems(IEnumerable<AbstractEntity> entitiesToAdd)
		{
			this.AddItems(entitiesToAdd.Cast<TPick>());
		}

		public void RemoveItems(IEnumerable<AbstractEntity> entitiesToRemove)
		{
			this.RemoveItems(entitiesToRemove.Cast<TDisplay>());
		}

		#endregion
	}
}