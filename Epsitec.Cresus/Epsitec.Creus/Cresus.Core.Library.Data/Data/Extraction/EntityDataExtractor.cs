//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
	/// <summary>
	/// The <c>EntityDataExtractor</c> class is used to extract data from entities in
	/// order to populate collections of sorted <see cref="EntityDataRow"/> in
	/// <see cref="EntityDataCollection"/> instances.
	/// </summary>
	public sealed class EntityDataExtractor
	{
		public EntityDataExtractor(EntityDataMetadata metadata)
		{
			this.rows = new List<EntityDataRow> ();
			this.collections = new List<EntityDataCollection> ();

			this.metadata = metadata;
		}

		
		public EntityDataMetadata				Metadata
		{
			get
			{
				return this.metadata;
			}
		}

		public IList<EntityDataCollection>		Collections
		{
			get
			{
				return this.collections.AsReadOnly ();
			}
		}


		/// <summary>
		/// Creates a sorted collection based on a given comparer. The collection
		/// must be created before any data is added into the extractor.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <returns>The sorted collection.</returns>
		public EntityDataCollection CreateCollection(IComparer<EntityDataRow> comparer)
		{
			var collection = new EntityDataCollection (comparer, this.rows);
			
			this.collections.Add (collection);

			return collection;
		}

		public void DeleteCollection(EntityDataCollection collection)
		{
			if (this.collections.Remove (collection) == false)
			{
				throw new System.ArgumentException ("Cannot delete the collection: it doesn't belong to this extractor");
			}
		}

		/// <summary>
		/// Inserts the specified entity into the sorted collections.
		/// </summary>
		/// <param name="entity">The entity.</param>
		public void Insert(AbstractEntity entity)
		{
			this.Insert (new EntityDataRow (this.metadata, entity));
		}

		/// <summary>
		/// Fills the sorted collections with the specified entities. The previous
		/// content will be cleared first.
		/// </summary>
		/// <param name="entities">The entities.</param>
		public void Fill(IEnumerable<AbstractEntity> entities)
		{
			this.Fill (entities.Select (x => new EntityDataRow (this.metadata, x)));
		}

		
		private void Insert(EntityDataRow row)
		{
			this.collections.ForEach (x => x.Insert (row));
		}

		private void Remove(EntityDataRow row)
		{
			this.collections.ForEach (x => x.Remove (row));
		}

		private void Fill(IEnumerable<EntityDataRow> rows)
		{
			this.rows.Clear ();
			this.rows.AddRange (rows);

			this.collections.ForEach (x => x.Fill (this.rows));
		}


		private readonly EntityDataMetadata			metadata;
		private readonly List<EntityDataRow>		rows;
		private readonly List<EntityDataCollection>	collections;
	}
}