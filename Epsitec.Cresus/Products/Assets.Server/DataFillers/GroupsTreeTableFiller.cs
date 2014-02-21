//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class GroupsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public GroupsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				yield return ObjectField.Name;
				yield return ObjectField.Description;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Tree,   250, "Groupe"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, 400, "Description"));

				return list.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var cf = new TreeTableColumnItem ();
			var c1 = new TreeTableColumnItem ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node  = this.nodeGetter[firstRow+i];
				var level = node.Level;
				var type  = node.Type;
				var obj   = this.accessor.GetObject (BaseType.Groups, node.Guid);

				var nom         = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var description = ObjectCalculator.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Description);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var sf = new TreeTableCellTree   (level, type, nom, cellState);
				var s1 = new TreeTableCellString (description,      cellState);

				cf.AddRow (sf);
				c1.AddRow (s1);
			}

			var content = new TreeTableContentItem ();

			content.Columns.Add (cf);
			content.Columns.Add (c1);

			return content;
		}
	}
}
