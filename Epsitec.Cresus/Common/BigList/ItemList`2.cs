//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemList<TData, TState> : ItemList
		where TState : ItemState, new ()
	{
		public ItemList(ItemCache<TData, TState> cache, IList<ItemListMark> marks, ItemListSelection selection)
			: base (cache, marks, selection)
		{
		}

		public ItemList(IItemDataProvider<TData> provider, IItemDataMapper<TData> mapper, ItemListSelection selection, ItemListFeatures features = null)
			: this (ItemList.CreateCache<TData, TState> (provider, mapper, features), null, selection)
		{
		}
	}
}
