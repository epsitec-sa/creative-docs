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
	public class SingleCategoriesTreeTableFiller : AbstractTreeTableFiller<SortableNode>
	{
		public SingleCategoriesTreeTableFiller(DataAccessor accessor, INodeGetter<SortableNode> nodeGetter)
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
				return 0;
			}
		}

		public override TreeTableColumnDescription[] Columns
		{
			get
			{
				var columns = new List<TreeTableColumnDescription> ();

				columns.Add (new TreeTableColumnDescription (ObjectField.Name,                  TreeTableColumnType.String,  220, Res.Strings.SingleCategoriesTreeTableFiller.Name.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.MethodGuid,            TreeTableColumnType.String,  180, Res.Strings.SingleCategoriesTreeTableFiller.Method.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationRate,      TreeTableColumnType.Rate,     50, Res.Strings.SingleCategoriesTreeTableFiller.AmortizationRate.ToString ()));
				columns.Add (new TreeTableColumnDescription (ObjectField.AmortizationYearCount, TreeTableColumnType.Decimal,  50, Res.Strings.SingleCategoriesTreeTableFiller.AmortizationYearCount.ToString ()));

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

				var node = this.nodeGetter[firstRow+i];
				var obj  = this.accessor.GetObject (BaseType.Categories, node.Guid);

				var name   = ObjectProperties.GetObjectPropertyString  (obj, this.Timestamp, ObjectField.Name, inputValue: true);
				var mg     = ObjectProperties.GetObjectPropertyGuid    (obj, this.Timestamp, ObjectField.MethodGuid);
				var rate   = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationRate);
				var years  = ObjectProperties.GetObjectPropertyDecimal (obj, this.Timestamp, ObjectField.AmortizationYearCount);

				var method = MethodsLogic.GetMethod (this.accessor, mg);

				SingleCategoriesTreeTableFiller.Hide (method, ObjectField.AmortizationRate,      ref rate);
				SingleCategoriesTreeTableFiller.Hide (method, ObjectField.AmortizationYearCount, ref years);

				var m = MethodsLogic.GetSummary (this.accessor, mg);

				var cellState = (i == selection) ? CellState.Selected : CellState.None;

				var cell1 = new TreeTableCellString  (name,  cellState);
				var cell2 = new TreeTableCellString  (m,     cellState);
				var cell3 = new TreeTableCellDecimal (rate,  cellState);
				var cell4 = new TreeTableCellDecimal (years, cellState);

				int columnRank = 0;

				content.Columns[columnRank++].AddRow (cell1);
				content.Columns[columnRank++].AddRow (cell2);
				content.Columns[columnRank++].AddRow (cell3);
				content.Columns[columnRank++].AddRow (cell4);
			}

			return content;
		}

		private static void Hide(AmortizationMethod method, ObjectField field, ref decimal? value)
		{
			if (Amortizations.IsHidden (method, field))
			{
				value = null;
			}
		}
	}
}
