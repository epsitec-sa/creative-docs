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
		public static void FillColumns(AbstractTreeTableFiller<T> filler, NavigationTreeTableController controller)
		{
			controller.SetColumns (filler.Columns, 1);
		}

		public static void FillContent(AbstractTreeTableFiller<T> filler, NavigationTreeTableController controller, int firstRow, int count, int selection)
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
