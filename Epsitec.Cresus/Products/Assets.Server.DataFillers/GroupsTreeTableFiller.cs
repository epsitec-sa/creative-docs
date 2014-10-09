﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class GroupsTreeTableFiller : AbstractTreeTableFiller<TreeNode>
	{
		public GroupsTreeTableFiller(DataAccessor accessor, INodeGetter<TreeNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override SortingInstructions		DefaultSorting
		{
			get
			{
				return new SortingInstructions (ObjectField.Number, SortedType.Ascending, ObjectField.Name, SortedType.Ascending);
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,                         TreeTableColumnType.Tree,   250, Res.Strings.GroupsTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number,                       TreeTableColumnType.String, 100, Res.Strings.GroupsTreeTableFiller.Number.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Description,                  TreeTableColumnType.String, 400, Res.Strings.GroupsTreeTableFiller.Description.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.GroupSuggestedDuringCreation, TreeTableColumnType.String,  70, Res.Strings.GroupsTreeTableFiller.GroupSuggestedDuringCreation.ToString ()));

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
				var obj   = this.accessor.GetObject (BaseType.Groups, node.Guid);

				var name        = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var number      = GroupsLogic.GetFullNumber (this.accessor, node.Guid);
				var description = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Description);
				var creation    = ObjectProperties.GetObjectPropertyInt    (obj, this.Timestamp, ObjectField.GroupSuggestedDuringCreation);

				var textCreation = (creation == 1)
					? Res.Strings.GroupsTreeTableFiller.GroupUsedDuringCreationYes.ToString ()
					: Res.Strings.GroupsTreeTableFiller.GroupUsedDuringCreationNo.ToString ();

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell0 = new TreeTableCellTree   (level, type, name, cellState);
				var cell1 = new TreeTableCellString (number,            cellState);
				var cell2 = new TreeTableCellString (description,       cellState);
				var cell3 = new TreeTableCellString (textCreation,      cellState);

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
