//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using System.Collections.Generic;

namespace Epsitec.Cresus.Replication
{
	/// <summary>
	/// The <c>DataExtractor</c> class provides support to extract data sets from
	/// a table, matching a single log id, a range of log ids, or specific row ids.
	/// </summary>
	internal sealed class DataExtractor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataExtractor"/> class.
		/// </summary>
		/// <param name="infrastructure">The database infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		public DataExtractor(DbInfrastructure infrastructure, DbTransaction transaction)
		{
			this.infrastructure = infrastructure;
			this.transaction    = transaction;
		}
		
		
		public System.Data.DataTable ExtractDataUsingLogId(DbTable table, DbId logId)
		{
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter);
			
			condition.AddCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbCompare.Equal, logId);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingLogIds(DbTable table, DbId syncStartId, DbId syncEndId)
		{
			long syncIdMin = syncStartId.Value;
			long syncIdMax = syncEndId.Value;
			
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter);
			
			condition.AddCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbCompare.GreaterThanOrEqual, syncIdMin);
			condition.AddCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbCompare.LessThanOrEqual, syncIdMax);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingIds(DbTable table, DbId syncStartId, DbId syncEndId)
		{
			long sync_id_min = syncStartId.Value;
			long sync_id_max = syncEndId.Value;
			
			System.Diagnostics.Debug.Assert (DbId.GetClass (sync_id_min) == DbIdClass.Standard);
			System.Diagnostics.Debug.Assert (DbId.GetClass (sync_id_max) == DbIdClass.Standard);
			
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter);
			
			condition.AddCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbCompare.GreaterThanOrEqual, sync_id_min);
			condition.AddCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbCompare.LessThanOrEqual, sync_id_max);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingIds(DbTable table, IEnumerable<long> ids)
		{
			DbSelectCondition condition = new DbSelectCondition (this.infrastructure.Converter);
			DbColumn          idColumn  = table.Columns[Tags.ColumnId];
			DbTableColumn     tableCol  = new DbTableColumn (idColumn);
			
			condition.Combiner = DbCompareCombiner.Or;
			
			foreach (long id in ids)
			{
				condition.AddCondition (tableCol, DbCompare.Equal, id);
			}
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
#if false
		public TableRowSet[] ExtractRowSetsUsingLogId(DbId logId)
		{
			return this.ExtractRowSetsUsingLogId (logId, DbElementCat.Any);
		}
		
		public TableRowSet[] ExtractRowSetsUsingLogId(DbId logId, DbElementCat category)
		{
			DbTable[] tables = this.infrastructure.FindDbTables (this.transaction, category, DbRowSearchMode.All);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i].Name == Tags.TableLog)
				{
					continue;
				}
				
				System.Data.DataTable data = this.ExtractDataUsingLogId (tables[i], logId);
				
				if (data.Rows.Count > 0)
				{
					list.Add (new TableRowSet (tables[i], data));
				}
			}
			
			TableRowSet[] sets = new TableRowSet[list.Count];
			list.CopyTo (sets);
			return sets;
		}
		
		public TableRowSet[] ExtractRowSetsUsingLogIds(DbId sync_start_id, DbId sync_end_id)
		{
			return this.ExtractRowSetsUsingLogIds (sync_start_id, sync_end_id, DbElementCat.Any);
		}
		
		public TableRowSet[] ExtractRowSetsUsingLogIds(DbId sync_start_id, DbId sync_end_id, DbElementCat category)
		{
			DbTable[] tables = this.infrastructure.FindDbTables (this.transaction, category, DbRowSearchMode.All);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i].Name == Tags.TableLog)
				{
					continue;
				}
				
				System.Data.DataTable data = this.ExtractDataUsingLogIds (tables[i], sync_start_id, sync_end_id);
				
				if (data.Rows.Count > 0)
				{
					list.Add (new TableRowSet (tables[i], data));
				}
			}
			
			TableRowSet[] sets = new TableRowSet[list.Count];
			list.CopyTo (sets);
			return sets;
		}
		
		
		public sealed class TableRowSet
		{
			public TableRowSet(DbTable table, System.Data.DataTable data)
			{
				this.table = table;
				this.ids   = new long[data.Rows.Count];
				
				for (int i = 0; i < data.Rows.Count; i++)
				{
					this.ids[i] = (long) data.Rows[i][0];
				}
			}
			
			
			public DbTable						Table
			{
				get
				{
					return this.table;
				}
			}
			
			public long[]						RowIds
			{
				get
				{
					return this.ids;
				}
			}
			
			
			readonly DbTable					table;
			readonly long[]						ids;
		}
#endif		
		
		readonly DbInfrastructure				infrastructure;
		readonly DbTransaction					transaction;
	}
}
