//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public class PersonsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public PersonsTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public override IEnumerable<ObjectField> Fields
		{
			get
			{
				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons))
				{
					yield return userField.Field;
				}
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				AbstractTreeTableCell.AddColumnDescription (columns, accessor.Settings.GetUserFields (BaseType.Persons));

				return columns.ToArray ();
			}
		}

		public override TreeTableContentItem GetContent(int firstRow, int count, int selection)
		{
			var content = new TreeTableContentItem ();

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons))
			{
				var column  = new TreeTableColumnItem ();
				content.Columns.Add (column);
			}

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.nodeGetter.Count)
				{
					break;
				}

				var node = this.nodeGetter[firstRow+i];
				var guid = node.Guid;
				var obj  = this.accessor.GetObject (BaseType.Persons, guid);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				int columnRank = 0;
				foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons))
				{
					bool inputValue = (columnRank == 0);
					var cell = AbstractTreeTableCell.CreateTreeTableCell (obj, this.Timestamp, userField, inputValue, cellState);

					content.Columns[columnRank++].AddRow (cell);
				}
			}

			return content;
		}
	}
}
