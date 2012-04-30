//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListCollection<TData, TState> : ItemListCollection
			where TState : ItemState, new ()
	{
		public ItemListCollection(ItemCache<TData, TState> cache, IList<ItemListMark> marks)
			: base (cache, marks, new ItemListSelection (cache))
		{
		}

		public override IItemList Create()
		{
			var cache = this.Cache as ItemCache<TData, TState>;
			var itemList = new ItemList<TData, TState> (cache, this.Marks);

			itemList.ActiveIndex = this.ActiveIndex;
			itemList.FocusedIndex = this.FocusedIndex;

			this.Add (itemList);

			return itemList;
		}
	}
}
