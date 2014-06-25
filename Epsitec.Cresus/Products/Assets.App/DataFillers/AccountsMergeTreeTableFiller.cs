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
	public class AccountsMergeTreeTableFiller : AbstractTreeTableFiller<AccountMergeTodo>
	{
		public AccountsMergeTreeTableFiller(DataAccessor accessor, INodeGetter<AccountMergeTodo> nodeGetter)
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
			var importColumn = new TreeTableColumnItem ();
			var mergeColumn  = new TreeTableColumnItem ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];

				var importText = AccountsMergeTreeTableFiller.GetImportDescription  (node);
				var mergeText  = AccountsMergeTreeTableFiller.GetCurrentDescription (node);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var importCell = new TreeTableCellString (importText, cellState);
				var mergeCell  = new TreeTableCellString (mergeText,  cellState);

				importColumn.AddRow (importCell);
				mergeColumn .AddRow (mergeCell);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (importColumn);
			content.Columns.Add (mergeColumn);

			return content;
		}

		private static string GetImportDescription(AccountMergeTodo todo)
		{
			return AccountsLogic.GetSummary (todo.ImportedAccount);
		}

		private static string GetCurrentDescription(AccountMergeTodo todo)
		{
			if (todo.IsAdd)
			{
				return "Ajouter";
			}
			else
			{
				return "Fusionner avec " + AccountsLogic.GetSummary (todo.MergeWithAccount);
			}
		}


		private const int ImportColumnWidth  = 200;
		private const int CurrentColumnWidth = 300;

		public const int TotalWidth = AccountsMergeTreeTableFiller.ImportColumnWidth
									+ AccountsMergeTreeTableFiller.CurrentColumnWidth;
	}
}
