//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class EntriesTreeTableFiller : AbstractTreeTableFiller<EntryNode>
	{
		public EntriesTreeTableFiller(DataAccessor accessor, INodeGetter<EntryNode> nodeGetter)
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
				yield return ObjectField.EventType;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   100, "Date"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  60, "Débit"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  60, "Crédit"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String,  70, "Pièce"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   300, "Libellé"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Amount, 100, "Montant"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph ,  30, ""));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<7; i++)
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
				var type  = node.NodeType;

				var date = TypeConverters.DateToString (node.Date);
				var glyph = TimelineData.TypeToGlyph (node.EventType);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				TreeTableCellTree cell1, cell5;

				if (string.IsNullOrEmpty (date))
				{
					//	Si la colonne est vide, on évite de dessiner le petit bouton triangulaire.
					cell1 = new TreeTableCellTree (0, NodeType.Final, null, cellState);
				}
				else
				{
					cell1 = new TreeTableCellTree (level, type, date, cellState);
				}

				var cell2 = new TreeTableCellString  (node.Debit,  cellState);
				var cell3 = new TreeTableCellString  (node.Credit, cellState);
				var cell4 = new TreeTableCellString  (node.Stamp,  cellState);

				if (string.IsNullOrEmpty (node.Title))
				{
					//	Si la colonne est vide, on évite de dessiner le petit bouton triangulaire.
					cell5 = new TreeTableCellTree (0, NodeType.Final, null, cellState);
				}
				else
				{
					cell5 = new TreeTableCellTree (level, type, node.Title, cellState);
				}

				var cell6 = new TreeTableCellDecimal (node.Value, cellState);
				var cell7 = new TreeTableCellGlyph (glyph, cellState);

				int columnRank = 0;
				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
				content.Columns[columnRank++].AddRow (cell6);
				content.Columns[columnRank++].AddRow (cell7);
			}

			return content;
		}
	}
}
