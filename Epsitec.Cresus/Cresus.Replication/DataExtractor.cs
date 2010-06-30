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
	public sealed class DataExtractor
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
			
			DbAbstractCondition part1 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbSimpleConditionOperator.GreaterThanOrEqual, syncIdMin);
			DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnRefLog]), DbSimpleConditionOperator.LessThanOrEqual, syncIdMax);

			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2),
			};
			
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

			DbAbstractCondition part1 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbSimpleConditionOperator.GreaterThanOrEqual, syncIdMin);
			DbAbstractCondition part2 = DbSimpleCondition.CreateCondition (new DbTableColumn (table.Columns[Tags.ColumnId]), DbSimpleConditionOperator.LessThanOrEqual, syncIdMax);

			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = new DbConditionCombiner (DbConditionCombinerOperator.And, part1, part2)
			};

			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}
		
		public System.Data.DataTable ExtractDataUsingIds(DbTable table, IEnumerable<long> ids)
		{
			DbColumn          idColumn  = table.Columns[Tags.ColumnId];
			DbTableColumn     tableCol  = new DbTableColumn (idColumn);

			DbConditionCombiner conditionCombiner = new DbConditionCombiner (DbConditionCombinerOperator.Or);
	
			foreach (long id in ids)
			{
				conditionCombiner.AddCondition (DbSimpleCondition.CreateCondition (tableCol, DbSimpleConditionOperator.Equal, id));
			}

			DbSelectCondition condition = new DbSelectCondition ()
			{
				Condition = conditionCombiner
			};
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, this.transaction, table, condition))
			{
				return command.DataTable;
			}
		}		
		
		private readonly DbInfrastructure				infrastructure;
		private readonly DbTransaction					transaction;
	}
}
