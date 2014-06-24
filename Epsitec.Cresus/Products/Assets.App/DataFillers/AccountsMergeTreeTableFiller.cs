//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.NodeGetters;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public class AccountsMergeTreeTableFiller : AbstractTreeTableFiller<AccountsMergeNode>
	{
		public AccountsMergeTreeTableFiller(DataAccessor accessor, INodeGetter<AccountsMergeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.MergeImport, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
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
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (ObjectField.MergeImport,  TreeTableColumnType.String, AccountsMergeTreeTableFiller.ImportColumnWidth,  "Import"));
				list.Add (new TreeTableColumnDescription (ObjectField.MergeCurrent, TreeTableColumnType.String, AccountsMergeTreeTableFiller.CurrentColumnWidth, "Actuel"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var c1 = new TreeTableColumnItem ();
			var c2 = new TreeTableColumnItem ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];

				var d1 = AccountsMergeTreeTableFiller.GetImportDescription  (node.ImportedAccount);
				var d2 = AccountsMergeTreeTableFiller.GetCurrentDescription (node.CurrentAccount);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var s1 = new TreeTableCellString (d1, cellState);
				var s2 = new TreeTableCellString (d2, cellState);

				c1.AddRow (s1);
				c2.AddRow (s2);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (c1);
			content.Columns.Add (c2);

			return content;
		}

		private static string GetImportDescription(DataObject account)
		{
			return AccountsLogic.GetSummary (account);
		}

		private static string GetCurrentDescription(DataObject account)
		{
			if (account == null)
			{
				return "Ajouter";
			}
			else
			{
				return "Fusionner avec " + AccountsLogic.GetSummary (account);
			}
		}


		private const int ImportColumnWidth  = 200;
		private const int CurrentColumnWidth = 300;

		public const int TotalWidth = AccountsMergeTreeTableFiller.ImportColumnWidth
									+ AccountsMergeTreeTableFiller.CurrentColumnWidth;
	}
}
