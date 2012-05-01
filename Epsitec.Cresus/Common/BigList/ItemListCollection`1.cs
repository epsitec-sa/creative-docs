//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListCollection<TData> : ItemListCollection<TData, ItemState>
	{
		public ItemListCollection(ItemCache<TData, ItemState> cache, IList<ItemListMark> marks, ItemListSelection selection)
			: base (cache, marks, selection)
		{
		}
	}
}
