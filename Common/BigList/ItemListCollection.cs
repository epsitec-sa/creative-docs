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
    public abstract class ItemListCollection : AbstractItemList, IEnumerable<IItemList>
    {
        public ItemListCollection(
            ItemCache cache,
            IList<ItemListMark> marks,
            ItemListSelection selection
        )
            : base(cache, marks, selection)
        {
            this.list = new List<IItemList>();
        }

        #region IEnumerable<IItemList> Members

        public IEnumerator<IItemList> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        public IItemList this[int index]
        {
            get { return this.list[index]; }
        }

        public int Count
        {
            get { return this.list.Count; }
        }

        public abstract ItemList Create();

        protected override void ResetList()
        {
            this.list.ForEach(x => x.ResetList());
        }

        protected void Add(IItemList itemList)
        {
            this.list.Add(itemList);
        }

        protected void ForEach(System.Action<IItemList> action)
        {
            this.list.ForEach(action);
        }

        protected override void ClearActiveIndex()
        {
            base.ClearActiveIndex();
            this.ForEach(x => x.ActiveIndex = -1);
        }

        protected override void SetActiveIndex(int index)
        {
            base.SetActiveIndex(index);
            this.ForEach(x => x.ActiveIndex = index);
        }

        protected override void ClearFocusedIndex()
        {
            base.ClearFocusedIndex();
            this.ForEach(x => x.FocusedIndex = -1);
        }

        protected override void SetFocusedIndex(int index)
        {
            base.SetFocusedIndex(index);
            this.ForEach(x => x.FocusedIndex = index);
        }

        private readonly List<IItemList> list;
    }
}
