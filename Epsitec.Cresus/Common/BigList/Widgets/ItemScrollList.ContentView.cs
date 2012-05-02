//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

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
			public ContentView(ItemScrollList host, IItemDataRenderer itemRenderer, IItemMarkRenderer markRenderer, Widget frame, AbstractScroller scroller)
			{
				this.host = host;
				this.scroller = scroller;
				this.view = new ItemListVerticalContentView ()
				{
					Parent       = frame,
					Dock         = DockStyle.Fill,
					ItemList     = this.host.itemLists.Create (),
					ItemRenderer = itemRenderer,
					MarkRenderer = markRenderer,
				};
				
				this.counter = new SafeCounter ();
				this.AttachEventHandlers ();
				this.UpdateScroller ();
			}

			
			public void UpdateScroller()
			{
				decimal index   = this.view.ItemList.VisibleFrame.GetFirstFullyVisibleIndex ();
				decimal visible = this.view.ItemList.VisibleFrame.VisibleCount;
				decimal total   = this.host.itemCache.ItemCount;

				using (this.counter.Enter ())
				{
					this.scroller.SmallChange       = 4;
					this.scroller.LargeChange	    = visible < 1 ? 0 : visible - 1;
					this.scroller.MinValue          = 0;
					this.scroller.MaxValue          = total < 1 ? 0 : total - 1;
					this.scroller.Resolution        = 1;
					this.scroller.VisibleRangeRatio = total == 0 ? 0 : visible / total;
					this.scroller.Value				= index < 0 ? 0 : index;
				}
			}

			#region IDisposable Members

			public void Dispose()
			{
				this.DetachEventHandlers ();

				this.view.Dispose ();
			}

			#endregion


			private void AttachEventHandlers()
			{
				this.host.itemCache.ResetFired += this.HandleItemCacheResetFired;
				this.view.ItemList.VisibleFrame.VisibleContentChanged += this.HandleVisibleContentChanged;
				this.scroller.ValueChanged += this.HandleScrollerValueChanged;
			}
			
			private void DetachEventHandlers()
			{
				this.host.itemCache.ResetFired -= this.HandleItemCacheResetFired;
				this.view.ItemList.VisibleFrame.VisibleContentChanged -= this.HandleVisibleContentChanged;
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
			}
			
			
			private void HandleItemCacheResetFired(object sender)
			{
				this.UpdateScroller ();
			}

			private void HandleScrollerValueChanged(object sender)
			{
				if (this.counter.IsZero)
				{
					this.view.ItemList.VisibleFrame.SyncVisibleIndex ((int) this.scroller.Value);
					this.view.Invalidate ();
				}
			}

			private void HandleVisibleContentChanged(object sender)
			{
				this.UpdateScroller ();
			}

			
			private readonly ItemScrollList		host;
			private readonly AbstractScroller	scroller;
			private readonly ItemListVerticalContentView view;
			private readonly SafeCounter		counter;
		}
	}
}
