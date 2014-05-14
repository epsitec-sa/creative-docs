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
	public class SingleAccountsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public SingleAccountsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   100, "Numéro"));
				columns.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 300, "Compte"));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			content.Columns.Add (new TreeTableColumnItem ());
			content.Columns.Add (new TreeTableColumnItem ());

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

				var number = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Number, inputValue: true);
				var name   = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellTree (level, type, number, cellState);
				var cell2 = new TreeTableCellString (name, cellState);

				content.Columns[0].AddRow (cell1);
				content.Columns[1].AddRow (cell2);
			}

			return content;
		}
	}
}
