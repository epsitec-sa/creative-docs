//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	/// <summary>
	/// Glu entre NavigationTreeTableController et AbstractTreeTableFiller.
	/// </summary>
	public static class TreeTableFiller<T>
		where T : struct
	{
		public static SortingInstructions GetSortingInstructions(NavigationTreeTableController controller)
		{
			//	Retourne les instructions de tri choisies dans le contrôleur TreeTable.
			var primaryField   = ObjectField.Unknown;
			var primaryType    = SortedType.None;
			var secondaryField = ObjectField.Unknown;
			var secondaryType  = SortedType.None;

			var sortedColumns = controller.SortedColumns.ToArray ();

			if (sortedColumns.Length >= 1)
			{
				var sortedColumn = sortedColumns[0];

				primaryField = sortedColumn.Field;
				primaryType  = sortedColumn.Type;
			}

			if (sortedColumns.Length >= 2)
			{
				var sortedColumn = sortedColumns[1];

				secondaryField = sortedColumn.Field;
				secondaryType  = sortedColumn.Type;
			}

			return new SortingInstructions (primaryField, primaryType, secondaryField, secondaryType);
		}


		public static void FillColumns(NavigationTreeTableController controller, AbstractTreeTableFiller<T> filler, string treeTableName)
		{
			//	Met à jour les colonnes du contrôleur TreeTable.
			controller.SetColumns (filler.Columns, filler.DefaultSorting, filler.DefaultDockToLeftCount, treeTableName);
		}


		public static void FillContent(NavigationTreeTableController controller,
			AbstractTreeTableFiller<T> filler, int selection, bool crop)
		{
			//	Met à jour le contenu du contrôleur TreeTable. Si crop = true, on s'arrange
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

			TreeTableFiller<T>.FillContent (controller, filler, firstRow, count, selection);
		}

		private static void FillContent(NavigationTreeTableController controller,
			AbstractTreeTableFiller<T> filler, int firstRow, int count, int selection)
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
