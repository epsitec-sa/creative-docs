//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public static class TreeTableFiller2<T>
		where T : struct
	{
		public static void FillColumns(AbstractTreeTableFiller<T> filler, NavigationTreeTableController controller)
		{
			controller.SetColumns (filler.Columns, 1);
		}

		public static void FillContent(AbstractTreeTableFiller<T> filler, NavigationTreeTableController controller, int firstRow, int count, int selection)
		{
			var content = filler.GetContent (firstRow, count, selection);

			int i = 0;
			foreach (var column in content.Columns)
			{
				controller.SetColumnCells (i++, column);
			}
		}
	}
}
