//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Helpers;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer.Helpers
{
	/// <summary>
	/// The <c>TemporaryRowCollection</c> class stores one entry for each newly
	/// created entity.
	/// </summary>
	internal class TemporaryRowCollection
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TemporaryRowCollection"/> class.
		/// </summary>
		public TemporaryRowCollection()
		{
			this.entries = new List<Entry> ();
		}

		/// <summary>
		/// Associates an entity mapping with a new row. All rows will share the
		/// same <see cref="DbKey"/>.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="mapping">The entity mapping.</param>
		/// <param name="row">The data row.</param>
		public void AssociateRow(DbRichCommand command, EntityDataMapping mapping, System.Data.DataRow row)
		{
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsTemporary);

			DbKey rowKey = mapping.RowKey;

			command.DefineRowKey (row, rowKey);
			
			foreach (Entry entry in this.entries)
			{
				if (entry.Mapping == mapping)
				{
					entry.Rows.Add (row);
					return;
				}
			}

			this.entries.Add (new Entry (mapping, row));
		}

		/// <summary>
		/// Updates the associated row keys. This will update the entity mapping
		/// for this entry and all row IDs.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="oldKey">The old key.</param>
		/// <param name="newKey">The new key.</param>
		public EntityDataMapping UpdateAssociatedRowKeys(DbRichCommand command, DbKey oldKey, DbKey newKey)
		{
			System.Diagnostics.Debug.Assert (oldKey.IsTemporary == true);
			System.Diagnostics.Debug.Assert (newKey.IsTemporary == false);

			DbId id = oldKey.Id;

			for (int i = 0; i < this.entries.Count; i++)
			{
				Entry entry = this.entries[i];

				if (entry.Id == id)
				{
					//	Update the row key for the entity mapping described by
					//	this entry and then update every row too :
					
					entry.Mapping.RowKey = newKey;

					foreach (System.Data.DataRow row in entry.Rows)
					{
						command.DefineRowKey (row, newKey);
					}

					//	Remove the entry. It does no longer contain temporary
					//	key ids and can therefore be discarded.
					
					this.entries.RemoveAt (i);

					return entry.Mapping;
				}
			}

			return null;
		}

		#region Entry Structure

		struct Entry
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Entry"/> structure.
			/// </summary>
			/// <param name="mapping">The entity mapping.</param>
			/// <param name="row">The associated row.</param>
			public Entry(EntityDataMapping mapping, System.Data.DataRow row)
			{
				this.mapping = mapping;
				this.rows = new	List<System.Data.DataRow> ();
				this.rows.Add (row);
			}

			/// <summary>
			/// Gets the id shared among all rows.
			/// </summary>
			/// <value>The id.</value>
			public DbId Id
			{
				get
				{
					return this.mapping.RowKey.Id;
				}
			}

			/// <summary>
			/// Gets the entity mapping.
			/// </summary>
			/// <value>The entity mapping.</value>
			public EntityDataMapping Mapping
			{
				get
				{
					return this.mapping;
				}
			}

			/// <summary>
			/// Gets the collection of rows associated with this entry.
			/// </summary>
			/// <value>The collection of rows.</value>
			public IList<System.Data.DataRow> Rows
			{
				get
				{
					return this.rows;
				}
			}

			readonly EntityDataMapping mapping;
			readonly List<System.Data.DataRow> rows;
		}

		#endregion

		readonly List<Entry> entries;
	}
}
