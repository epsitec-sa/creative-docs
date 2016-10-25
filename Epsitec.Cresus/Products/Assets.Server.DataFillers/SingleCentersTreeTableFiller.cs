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
	public class SingleCentersTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public SingleCentersTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
		}


		public BaseType							BaseType;

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
				return 0;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,   TreeTableColumnType.String, SingleCentersTreeTableFiller.nameWidth));
				columns.Add (new TreeTableColumnDescription (ObjectField.Number, TreeTableColumnType.String, SingleCentersTreeTableFiller.numberWidth));

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

				var node   = this.nodeGetter[firstRow+i];
				var center = this.accessor.GetObject (this.BaseType, node.Guid);

				var name = ObjectProperties.GetObjectPropertyString  (center, this.Timestamp, ObjectField.Name, inputValue: true);
				var desc = ObjectProperties.GetObjectPropertyString  (center, this.Timestamp, ObjectField.Number);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellString (name, cellState);
				var cell2 = new TreeTableCellString (desc, cellState);

				content.Columns[0].AddRow (cell1);
				content.Columns[1].AddRow (cell2);
			}

			return content;
		}


		public const int TotalWidth =
			SingleCentersTreeTableFiller.nameWidth +
			SingleCentersTreeTableFiller.numberWidth;

		private const int nameWidth   = 200;
		private const int numberWidth =  70;
	}
}
