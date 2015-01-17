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
	public class SingleGroupsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public SingleGroupsTreeTableFiller(DataAccessor accessor, INodeGetter<TreeNode> nodeGetter, int width)
			: base (accessor, nodeGetter)
		{
			this.width = width;
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

				int w0 = this.width*3/4;
				int w1 = this.width*1/2;

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,   TreeTableColumnType.Tree,   w0, Res.Strings.SingleGroupsTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number, TreeTableColumnType.String, w1, Res.Strings.SingleGroupsTreeTableFiller.Number.ToString ()));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			for (int i=0; i<2; i++)
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

				var name   = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var number = GroupsLogic.GetFullNumber (this.accessor, node.Guid);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;
				var cell0 = new TreeTableCellTree (level, type, name, cellState);
				var cell1 = new TreeTableCellString (number, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell0);
				content.Columns[columnRank++].AddRow (cell1);
			}

			return content;
		}


		private readonly int width;
	}
}
