//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using System.Collections.Generic;

namespace Epsitec.Common.BigList
{
    /// <summary>
    /// The <c>AbstractItemList</c> class is the lowest level abstract class for the item list
    /// classes, which implement <see cref="IItemList"/>.
    /// </summary>
    public abstract class AbstractItemList : IItemList
    {
        protected AbstractItemList(
            ItemCache cache,
            IList<ItemListMark> marks,
            ItemListSelection selection
        )
        {
            this.cache = cache;
            this.marks = marks;
            this.selection = selection;

            this.activeIndex = -1;
            this.focusedIndex = -1;
        }

        #region IItemList Members

        public ItemListFeatures Features
        {
            get { return this.cache.Features; }
        }

        public IList<ItemListMark> Marks
        {
            get { return this.marks; }
        }

        public int ItemCount
        {
            get { return this.cache.ItemCount; }
        }

        public int ActiveIndex
        {
            get { return this.activeIndex; }
            set
            {
                if (value == -1)
                {
                    this.ClearActiveIndex();
                }
                else
                {
                    this.SetActiveIndex(value);
                }
            }
        }

        public int FocusedIndex
        {
            get { return this.focusedIndex; }
            set
            {
                if (value == -1)
                {
                    this.ClearFocusedIndex();
                }
                else
                {
                    this.SetFocusedIndex(value);
                }
            }
        }

        public ItemListSelection Selection
        {
            get { return this.selection; }
        }

        public ItemCache Cache
        {
            get { return this.cache; }
        }

        public void Reset()
        {
            //	By first resetting the list, then resetting the cache, we make sure that in the
            //	case of an ItemListCollection, the cache will be reset only once (looping will
            //	be done only in the call to ResetList).

            this.ResetList();
            this.Cache.Reset();
        }

        /// <summary>
        /// Resets the list active index, focused index and visible frame, if any. This does
        /// not affect the item cache.
        /// </summary>
        void IItemList.ResetList()
        {
            this.ResetList();
        }

        #endregion

        protected abstract void ResetList();

        protected virtual void SetActiveIndex(int index)
        {
            if ((index < 0) || (index >= this.ItemCount))
            {
                throw new System.ArgumentOutOfRangeException("index", "Index out of bounds");
            }

            int oldIndex = this.activeIndex;
            int newIndex = index;

            if (this.activeIndex != index)
            {
                this.activeIndex = index;
                this.OnActiveIndexChanged(new ItemListIndexEventArgs(oldIndex, newIndex));
            }
        }

        protected virtual void ClearActiveIndex()
        {
            int oldIndex = this.activeIndex;
            int newIndex = -1;

            if (this.activeIndex != -1)
            {
                this.activeIndex = -1;
                this.OnActiveIndexChanged(new ItemListIndexEventArgs(oldIndex, newIndex));
            }
        }

        protected virtual void SetFocusedIndex(int index)
        {
            if ((index < 0) || (index >= this.ItemCount))
            {
                throw new System.ArgumentOutOfRangeException("index", "Index out of bounds");
            }

            int oldIndex = this.focusedIndex;
            int newIndex = index;

            if (this.focusedIndex != index)
            {
                this.focusedIndex = index;
                this.OnFocusedIndexChanged(new ItemListIndexEventArgs(oldIndex, newIndex));
            }
        }

        protected virtual void ClearFocusedIndex()
        {
            int oldIndex = this.focusedIndex;
            int newIndex = -1;

            if (this.focusedIndex != -1)
            {
                this.focusedIndex = -1;
                this.OnFocusedIndexChanged(new ItemListIndexEventArgs(oldIndex, newIndex));
            }
        }

        private void OnActiveIndexChanged(ItemListIndexEventArgs e)
        {
            var handler = this.ActiveIndexChanged;
            handler.Raise(this, e);
        }

        private void OnFocusedIndexChanged(ItemListIndexEventArgs e)
        {
            var handler = this.FocusedIndexChanged;
            handler.Raise(this, e);
        }

        public event EventHandler<ItemListIndexEventArgs> ActiveIndexChanged;
        public event EventHandler<ItemListIndexEventArgs> FocusedIndexChanged;

        private readonly ItemCache cache;
        private readonly IList<ItemListMark> marks;
        private readonly ItemListSelection selection;
        private int activeIndex;
        private int focusedIndex;
    }
}
