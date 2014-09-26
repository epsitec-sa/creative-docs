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


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.EntryTitle, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 0;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.EntryDate,          TreeTableColumnType.Tree,   100, Res.Strings.EntriesTreeTableFiller.Date.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EntryDebitAccount,  TreeTableColumnType.String,  90, Res.Strings.EntriesTreeTableFiller.Debit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EntryCreditAccount, TreeTableColumnType.String,  90, Res.Strings.EntriesTreeTableFiller.Credit.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EntryStamp,         TreeTableColumnType.String,  70, Res.Strings.EntriesTreeTableFiller.Stamp.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EntryTitle,         TreeTableColumnType.Tree,   350, Res.Strings.EntriesTreeTableFiller.Title.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EntryAmount,        TreeTableColumnType.Amount, 100, Res.Strings.EntriesTreeTableFiller.Amount.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.EventType,          TreeTableColumnType.Glyph ,  30, ""));

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
