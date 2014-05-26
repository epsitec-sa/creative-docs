//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class UserFieldsTreeTableFiller : AbstractTreeTableFiller<GuidNode>
	{
		public UserFieldsTreeTableFiller(DataAccessor accessor, INodeGetter<GuidNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.Name, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}

		public override int						DefaultDockToLeftCount
		{
			get
			{
				return 1;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,                  TreeTableColumnType.String, 150, "Nom"));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldType,         TreeTableColumnType.String, 100, "Type"));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldColumnWidth,  TreeTableColumnType.Int,     70, "Lg colonne"));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldLineWidth,    TreeTableColumnType.Int,     70, "Lg ligne"));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldLineCount,    TreeTableColumnType.Int,     70, "Nb lignes"));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldTopMargin,    TreeTableColumnType.Int,     70, "Marge sup."));
				columns.Add (new TreeTableColumnDescription (ObjectField.UserFieldSummaryOrder, TreeTableColumnType.Int,     70, "Ds résumé"));

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
				var userField = this.accessor.GlobalSettings.GetUserField (node.Guid);

				var text0  = userField.Name;
				var text1  = EnumDictionaries.GetFieldTypeName (userField.Type);
				var text2  = userField.ColumnWidth;
				var text3  = userField.LineWidth;
				var text4  = userField.LineCount;
				var text5  = userField.TopMargin;
				var text6  = userField.SummaryOrder;

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell0 = new TreeTableCellString (text0, cellState);
				var cell1 = new TreeTableCellString (text1, cellState);
				var cell2 = new TreeTableCellInt    (text2, cellState);
				var cell3 = new TreeTableCellInt    (text3, cellState);
				var cell4 = new TreeTableCellInt    (text4, cellState);
				var cell5 = new TreeTableCellInt    (text5, cellState);
				var cell6 = new TreeTableCellInt    (text6, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell0);
				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
				content.Columns[columnRank++].AddRow (cell5);
				content.Columns[columnRank++].AddRow (cell6);
			}

			return content;
		}
	}
}
