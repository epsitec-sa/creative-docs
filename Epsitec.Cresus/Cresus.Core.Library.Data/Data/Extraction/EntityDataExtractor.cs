//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Data.Extraction
{
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


		public void Fill(IEnumerable<AbstractEntity> entities)
		{
			this.Fill (entities.Select (x => new EntityDataRow (this.metadata, x)));
		}

		public void Fill(IEnumerable<EntityDataRow> rows)
		{
			this.rows.Clear ();
			this.rows.AddRange (rows);

			this.collections.ForEach (x => x.Fill (this.rows));
		}

		
		public void Insert(EntityDataRow row)
		{
			this.collections.ForEach (x => x.Insert (row));
		}

		public void Remove(EntityDataRow row)
		{
			this.collections.ForEach (x => x.Remove (row));
		}

		private EntityDataMetadata					metadata;
		private readonly List<EntityDataRow>		rows;
		private readonly List<EntityDataCollection>	collections;
	}
}