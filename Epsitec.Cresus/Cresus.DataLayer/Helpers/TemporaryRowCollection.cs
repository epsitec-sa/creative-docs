//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	public class TemporaryRowCollection
	{
		public TemporaryRowCollection()
		{
			this.records = new List<Record> ();
		}

		public void Add(EntityDataMapping mapping, System.Data.DataRow row)
		{
			System.Diagnostics.Debug.Assert (mapping.RowKey.IsTemporary);

			foreach (Record record in this.records)
			{
				if (record.Mapping == mapping)
				{
					record.Rows.Add (row);
					return;
				}
			}

			this.records.Add (new Record (mapping, row));
		}

		public void UpdateRowKey(DbKey oldKey, DbKey newKey)
		{
			System.Diagnostics.Debug.Assert (oldKey.IsTemporary == true);
			System.Diagnostics.Debug.Assert (newKey.IsTemporary == false);

			DbId id = oldKey.Id;

			for (int i = 0; i < this.records.Count; i++)
			{
				Record record = this.records[i];

				if (record.Id == id)
				{
					record.Mapping.RowKey = newKey;

					foreach (System.Data.DataRow row in record.Rows)
					{
						row.BeginEdit ();
						DbKey.SetRowId (row, newKey.Id);
						DbKey.SetRowStatus (row, newKey.Status);
						row.EndEdit ();
					}

					this.records.RemoveAt (i);
					break;
				}
			}
		}

		struct Record
		{
			public Record(EntityDataMapping mapping, System.Data.DataRow row)
			{
				this.mapping = mapping;
				this.rows = new	List<System.Data.DataRow> ();
				this.rows.Add (row);
			}

			public DbId Id
			{
				get
				{
					return this.mapping.RowKey.Id;
				}
			}

			public EntityDataMapping Mapping
			{
				get
				{
					return this.mapping;
				}
			}

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

		readonly List<Record> records;
	}
}
