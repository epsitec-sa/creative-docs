//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets.Layouts;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public sealed class ItemListColumnCollection : ObservableList<ItemListColumn>
	{
		public ItemListColumnCollection()
		{
		}


		public ItemListColumn GetColumn(int index)
		{
			return this.FirstOrDefault (x => x.Index == index);
		}

		public ItemListColumn DetectColumn(Point pos)
		{
			return this.FirstOrDefault (x => x.Contains (pos));
		}

		public double Layout(double availableSpace)
		{
			return ColumnLayoutList.Fit (this.OrderBy (x => x.Index).Select (x => x.Layout), availableSpace);
		}
	}
}
