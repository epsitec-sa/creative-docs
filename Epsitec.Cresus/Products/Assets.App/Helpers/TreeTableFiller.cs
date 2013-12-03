//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class TreeTableFiller<T>
		where T : struct
	{
		public static void FillColumns(AbstractTreeTableFiller<T> filler,
			NavigationTreeTableController controller, int dockToLeftCount = 1)
		{
			//	Met à jour les colonnes du contrôleur.
			controller.SetColumns (filler.Columns, dockToLeftCount);
		}

		public static void FillContent(AbstractTreeTableFiller<T> filler,
			NavigationTreeTableController controller, int selection, bool crop)
		{
			//	Met à jour le contenu du contrôleur. Si crop = true, on s'arrange
			//	pour rendre visible la sélection.
			controller.RowsCount = filler.Count;

			int visibleCount = controller.VisibleRowsCount;
			int rowsCount    = controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = controller.TopVisibleRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = controller.GetTopVisibleRow (selection);
				}

				if (controller.TopVisibleRow != firstRow)
				{
					controller.TopVisibleRow = firstRow;
				}

				selection -= controller.TopVisibleRow;
			}

			TreeTableFiller<T>.FillContent (filler, controller, firstRow, count, selection);
		}

		private static void FillContent(AbstractTreeTableFiller<T> filler,
			NavigationTreeTableController controller, int firstRow, int count, int selection)
		{
			var contentItem = filler.GetContent (firstRow, count, selection);

			int i = 0;
			foreach (var columnItem in contentItem.Columns)
			{
				controller.SetColumnCells (i++, columnItem);
			}
		}
	}
}
