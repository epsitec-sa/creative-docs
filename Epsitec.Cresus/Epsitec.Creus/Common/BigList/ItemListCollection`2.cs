//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemListCollection<TData, TState> : ItemListCollection
			where TState : ItemState, new ()
	{
		public ItemListCollection(ItemCache<TData, TState> cache, IList<ItemListMark> marks, ItemListSelection selection)
			: base (cache, marks, selection)
		{
		}

		public override ItemList Create()
		{
			var cache = this.Cache as ItemCache<TData, TState>;
			var itemList = new ItemList<TData, TState> (cache, this.Marks, this.Selection);

			itemList.ActiveIndex = this.ActiveIndex;
			itemList.FocusedIndex = this.FocusedIndex;

			itemList.ActiveIndexChanged  += this.HandleItemListActiveIndexChanged;
			itemList.FocusedIndexChanged += this.HandleItemListFocusedIndexChanged;

			this.Add (itemList);

			return itemList;
		}


		private void HandleItemListActiveIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			this.ActiveIndex = e.NewIndex;
		}

		private void HandleItemListFocusedIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			this.FocusedIndex = e.NewIndex;
		}
	}
}
