//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class EntriesTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public EntriesTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.EntryDate;
				yield return ObjectField.EntryDebitAccount;
				yield return ObjectField.EntryCreditAccount;
				yield return ObjectField.EntryStamp;
				yield return ObjectField.EntryTitle;
				yield return ObjectField.EntryAmount;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Date,     80, "Date"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   60, "Débit"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   60, "Crédit"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,   70, "Pièce"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,    300, "Libellé"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, 100, "Montant"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<6; i++)
			{
				content.Columns.Add (new TreeTableColumnItem ());
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (BaseType.Entries, node.Guid);

				var date   = ObjectProperties.GetObjectPropertyDate    (obj, this.Timestamp, ObjectField.EntryDate);
				var debit  = ObjectProperties.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.EntryDebitAccount);
				var credit = ObjectProperties.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.EntryCreditAccount);
				var stamp  = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.EntryStamp);
				var title  = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.EntryTitle);
				var amount = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.EntryAmount);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellDate    (date,                     cellState);
				var cell2 = new TreeTableCellString  (this.GetAccount (debit),  cellState);
				var cell3 = new TreeTableCellString  (this.GetAccount (credit), cellState);
				var cell4 = new TreeTableCellString  (stamp,                    cellState);
				var cell5 = new TreeTableCellTree    (level, type, title,       cellState);
				var cell6 = new TreeTableCellDecimal (amount,                   cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
				content.Columns[columnRank++].AddRow (cell6);
			}

			return content;
		}

		private static string Leveled(int level, string text)
		{
			if (level == 0)
			{
				return text;
			}
			else
			{
				var indent = new string (' ', level*2);
				return indent + text;
			}
		}

		private string GetAccount(Guid accountGuid)
		{
			return AccountsLogic.GetNumber (this.accessor, accountGuid);
		}
	}
}
