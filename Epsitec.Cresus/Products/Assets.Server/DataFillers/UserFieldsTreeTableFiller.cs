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
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 150, "Nom"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Lg colonne"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Lg ligne"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Nb lignes"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Int,     70, "Marge sup."));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c0 = new TreeTableColumnItem ();
			var c1 = new TreeTableColumnItem ();
			var c2 = new TreeTableColumnItem ();
			var c3 = new TreeTableColumnItem ();
			var c4 = new TreeTableColumnItem ();
			var c5 = new TreeTableColumnItem ();

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

				c0.AddRow (s0);
				c1.AddRow (s1);
				c2.AddRow (s2);
				c3.AddRow (s3);
				c4.AddRow (s4);
				c5.AddRow (s5);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c0);
			content.Columns.Add (c1);
			content.Columns.Add (c2);
			content.Columns.Add (c3);
			content.Columns.Add (c4);
			content.Columns.Add (c5);

			return content;
		}
	}
}
