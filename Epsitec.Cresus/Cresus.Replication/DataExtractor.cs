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
			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbSimpleConditionOperator.Equal, logId),
			};
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingLogIds(DbTable table, DbId syncStartId, DbId syncEndId)
		{
			long syncIdMin = syncStartId.Value;
			long syncIdMax = syncEndId.Value;
			
			DbSelectCondition condition = new DbSelectCondition ();

			DbAbstractCondition part1 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbSimpleConditionOperator.GreaterThanOrEqual, syncIdMin);
			DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbSimpleConditionOperator.LessThanOrEqual, syncIdMax);

			condition.Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2);
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingIds(DbTable table, DbId syncStartId, DbId syncEndId)
		{
			long syncIdMin = syncStartId.Value;
			long syncIdMax = syncEndId.Value;
			
			System.Diagnostics.Debug.Assert (DbId.GetClass (syncIdMin) == DbIdClass.Standard);
			System.Diagnostics.Debug.Assert (DbId.GetClass (syncIdMax) == DbIdClass.Standard);
			
			DbSelectCondition condition = new DbSelectCondition ();

			DbAbstractCondition part1 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbSimpleConditionOperator.GreaterThanOrEqual, syncIdMin);
			DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbSimpleConditionOperator.LessThanOrEqual, syncIdMax);

			condition.Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2);

			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingIds(DbTable table, IEnumerable<long> ids)
		{
			DbSelectCondition condition = new DbSelectCondition ();
			DbColumn          idColumn  = table.Columns[Tags.ColumnId];
			DbTableColumn     tableCol  = new DbTableColumn (idColumn);

			DbConditionCombiner conditionCombiner = new DbConditionCombiner ()
			{
				Combiner = DbConditionCombinerOperator.Or,
			};
						
			foreach (long id in ids)
			{
				conditionCombiner.AddCondition (DbSimpleCondition.CreateCondition (tableCol, DbSimpleConditionOperator.Equal, id));
			}

			condition.Condition = conditionCombiner;
			
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
		
		public TableRowSet[] ExtractRowSetsUsingLogIds(DbId syncStartId, DbId syncEndId)
		{
			return this.ExtractRowSetsUsingLogIds (syncStartId, syncEndId, DbElementCat.Any);
		}
		
		public TableRowSet[] ExtractRowSetsUsingLogIds(DbId syncStartId, DbId syncEndId, DbElementCat category)
		{
			DbTable[] tables = this.infrastructure.FindDbTables (this.transaction, category, DbRowSearchMode.All);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < tables.Length; i++)
			{
				if (tables[i].Name == Tags.TableLog)
				{
					continue;
				}
				
				System.Data.DataTable data = this.ExtractDataUsingLogIds (tables[i], syncStartId, syncEndId);
				
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
