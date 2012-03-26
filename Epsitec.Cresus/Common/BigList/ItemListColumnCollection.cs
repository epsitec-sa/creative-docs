//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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

		public void Reset()
		{
			this.Sort ((x, y) => y.Index - x.Index);
		}

		public double Layout(double availableSpace)
		{
			return ColumnLayoutList.Fit (this.Select (x => x.Layout), availableSpace);
		}
	}
}
