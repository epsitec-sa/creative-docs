//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class AccountsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public AccountsTreeTableFiller(DataAccessor accessor, INodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.Number, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Number,          TreeTableColumnType.Tree,   160, Res.Strings.AccountsTreeTableFiller.Number.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Name,            TreeTableColumnType.String, 300, Res.Strings.AccountsTreeTableFiller.Title.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountCategory, TreeTableColumnType.String, 100, Res.Strings.AccountsTreeTableFiller.Category.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AccountType,     TreeTableColumnType.String, 100, Res.Strings.AccountsTreeTableFiller.Type.ToString ()));

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
				var obj   = this.accessor.GetObject (node.BaseType, node.Guid);

				var number   = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Number, inputValue: true);
				var name     = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name);
				var category = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.AccountCategory);
				var accType  = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.AccountType);

				string c = category.HasValue ? EnumDictionaries.GetAccountCategoryName ((AccountCategory) category) : null;
				string t = accType .HasValue ? EnumDictionaries.GetAccountTypeName     ((AccountType)     accType ) : null;

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
