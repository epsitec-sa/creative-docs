/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;

namespace Epsitec.Common.BigList
{
    public class ItemListCollection<TData, TState> : ItemListCollection
        where TState : ItemState, new()
    {
        public ItemListCollection(
            ItemCache<TData, TState> cache,
            IList<ItemListMark> marks,
            ItemListSelection selection
        )
            : base(cache, marks, selection) { }

        public override ItemList Create()
        {
            var cache = this.Cache as ItemCache<TData, TState>;
            var itemList = new ItemList<TData, TState>(cache, this.Marks, this.Selection);

            itemList.ActiveIndex = this.ActiveIndex;
            itemList.FocusedIndex = this.FocusedIndex;

            itemList.ActiveIndexChanged += this.HandleItemListActiveIndexChanged;
            itemList.FocusedIndexChanged += this.HandleItemListFocusedIndexChanged;

            this.Add(itemList);

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
