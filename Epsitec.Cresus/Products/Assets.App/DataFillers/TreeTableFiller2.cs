//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public static class TreeTableFiller2
	{
		public static void FillColumns(AbstractTreeTableFiller2 filler, NavigationTreeTableController controller)
		{
			controller.SetColumns (filler.Columns, 1);
		}

		public static void FillContent(AbstractTreeTableFiller2 filler, NavigationTreeTableController controller, int firstRow, int count, int selection)
		{
			var content = filler.GetContent (firstRow, count, selection);

			int i = 0;
			foreach (var column in content.Columns)
			{
				var type = column.Type;

				if (type == typeof (TreeTableCellComputedAmount))
				{
					controller.SetColumnCells (i, column.ComputedAmountRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellDate))
				{
					controller.SetColumnCells (i, column.DateRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellDecimal))
				{
					controller.SetColumnCells (i, column.DecimalRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellGlyph))
				{
					controller.SetColumnCells (i, column.GlyphRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellGuid))
				{
					controller.SetColumnCells (i, column.GuidRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellInt))
				{
					controller.SetColumnCells (i, column.IntRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellString))
				{
					controller.SetColumnCells (i, column.StringRows.ToArray ());
				}
				else if (type == typeof (TreeTableCellTree))
				{
					controller.SetColumnCells (i, column.TreeRows.ToArray ());
				}
				else
				{
					//?System.Diagnostics.Debug.Fail ("Not implemented");
				}

				i++;
			}
		}
	}
}
