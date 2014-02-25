//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class AccountsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public AccountsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Number;
				yield return ObjectField.Name;
				yield return ObjectField.AccountCategory;
				yield return ObjectField.AccountType;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   100, "Numéro"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 200, "Compte"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Catégorie"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 100, "Type"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<4; i++)
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
				var obj   = this.accessor.GetObject (BaseType.Accounts, node.Guid);

				var number   = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Number, inputValue: true);
				var name     = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name);
				var category = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.AccountCategory);
				var accType  = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.AccountType);

				var c = EnumDictionaries.GetAccountCategoryName ((AccountCategory) category);
				var t = EnumDictionaries.GetAccountTypeName     ((AccountType)     accType);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell0 = new TreeTableCellTree   (level, type, number, cellState);
				var cell1 = new TreeTableCellString (name,                cellState);
				var cell2 = new TreeTableCellString (c,                   cellState);
				var cell3 = new TreeTableCellString (t,                   cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell0);
				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
			}

			return content;
		}
	}
}
