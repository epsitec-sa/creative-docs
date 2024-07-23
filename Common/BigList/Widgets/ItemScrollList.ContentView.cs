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


using Epsitec.Common.Support;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.BigList.Widgets
{
    public partial class ItemScrollList
    {
        /// <summary>
        /// The <c>ContentView</c> class wraps an item list's content view and its associated scroller
        /// into a coherent ensemble.
        /// </summary>
        class ContentView : System.IDisposable
        {
            public ContentView(
                ItemScrollList host,
                IItemDataRenderer itemRenderer,
                IItemMarkRenderer markRenderer,
                Widget frame,
                AbstractScroller scroller
            )
            {
                this.host = host;
                this.scroller = scroller;

                this.view = new ItemListVerticalContentView()
                {
                    Parent = frame,
                    Dock = DockStyle.Fill,
                    ItemList = this.host.itemLists.Create(),
                    ItemRenderer = itemRenderer,
                    MarkRenderer = markRenderer,
                };

                this.counter = new SafeCounter();
                this.AttachEventHandlers();
                this.UpdateScroller();
            }

            public void UpdateScroller()
            {
                decimal index = this.view.ItemList.VisibleFrame.GetFirstFullyVisibleIndex();
                decimal visible = this.view.ItemList.VisibleFrame.FullyVisibleCount;
                decimal total = this.host.itemCache.ItemCount;

                if (total <= 0)
                {
                    index = 0;
                    visible = 0;
                }

                using (this.counter.Enter())
                {
                    this.scroller.SmallChange = 1;
                    this.scroller.LargeChange = visible < 1 ? 0 : visible - 1;
                    this.scroller.MinValue = 0;
                    this.scroller.MaxValue = total < visible ? 0 : total - visible;
                    this.scroller.Resolution = 1;
                    this.scroller.VisibleRangeRatio = total == 0 ? 1.0M : visible / total;
                    this.scroller.Value = index < 0 ? 0 : index;
                }
            }

            #region IDisposable Members

            public void Dispose()
            {
                this.DetachEventHandlers();

                this.view.Dispose();
            }

            #endregion


            private void AttachEventHandlers()
            {
                this.host.itemCache.ResetFired += this.HandleItemCacheResetFired;
                this.view.ItemList.VisibleFrame.VisibleContentChanged +=
                    this.HandleVisibleContentChanged;
                this.scroller.ValueChanged += this.HandleScrollerValueChanged;
            }

            private void DetachEventHandlers()
            {
                this.host.itemCache.ResetFired -= this.HandleItemCacheResetFired;
                this.view.ItemList.VisibleFrame.VisibleContentChanged -=
                    this.HandleVisibleContentChanged;
                this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
            }

            private void HandleItemCacheResetFired(object sender)
            {
                this.UpdateScroller();
            }

            private void HandleScrollerValueChanged(object sender)
            {
                if (this.counter.IsZero)
                {
                    this.view.ItemList.VisibleFrame.SyncVisibleIndex((int)this.scroller.Value);
                    this.view.Invalidate();
                }
            }

            private void HandleVisibleContentChanged(object sender)
            {
                this.UpdateScroller();
            }

            private readonly ItemScrollList host;
            private readonly AbstractScroller scroller;
            private readonly ItemListVerticalContentView view;
            private readonly SafeCounter counter;
        }
    }
}
