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
	public class MethodsTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public MethodsTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
			: base (accessor, nodeGetter)
		{
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

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,       TreeTableColumnType.String, MethodsTreeTableFiller.nameWidth, Res.Strings.ExpressionsTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.Expression, TreeTableColumnType.String, MethodsTreeTableFiller.expWidth,  Res.Strings.ExpressionsTreeTableFiller.Expression.ToString ()));

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
				var guid  = node.Guid;
				var obj   = this.accessor.GetObject (BaseType.Methods, guid);

				var name = ObjectProperties.GetObjectPropertyString (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var exp  = MethodsLogic.GetExpressionSummary (this.accessor, guid);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellString (name, cellState);
				var cell2 = new TreeTableCellString (exp,  cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
			}

			return content;
		}


		private const int nameWidth = 250;
		private const int expWidth  = 120;

		public const int totalWidth  =
			MethodsTreeTableFiller.nameWidth +
			MethodsTreeTableFiller.expWidth;
	}
}
