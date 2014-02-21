//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class UserFieldsTreeTableFiller : AbstractTreeTableFiller<GuidNode>
	{
		public UserFieldsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<GuidNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.UserFieldType;
				yield return ObjectField.UserFieldColumnWidth;
				yield return ObjectField.UserFieldLineWidth;
				yield return ObjectField.UserFieldLineCount;
				yield return ObjectField.UserFieldTopMargin;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Nom"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Type"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Lg colonne"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Lg ligne"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Nb lignes"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Marge sup."));

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
				var userField = this.accessor.Settings.GetUserField (node.Guid);

				var text0  = userField.Name;
				var text1  = EnumDictionaries.GetFieldTypeName (userField.Type);
				var text2  = userField.ColumnWidth;
				var text3  = userField.LineWidth;
				var text4  = userField.LineCount;
				var text5  = userField.TopMargin;

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var s0  = new TreeTableCellString (text0, cellState);
				var s1  = new TreeTableCellString (text1, cellState);
				var s2  = new TreeTableCellInt    (text2, cellState);
				var s3  = new TreeTableCellInt    (text3, cellState);
				var s4  = new TreeTableCellInt    (text4, cellState);
				var s5  = new TreeTableCellInt    (text5, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (s0);
				content.Columns[columnRank++].AddRow (s1);
				content.Columns[columnRank++].AddRow (s2);
				content.Columns[columnRank++].AddRow (s3);
				content.Columns[columnRank++].AddRow (s4);
				content.Columns[columnRank++].AddRow (s5);
			}

			return content;
		}
	}
}
