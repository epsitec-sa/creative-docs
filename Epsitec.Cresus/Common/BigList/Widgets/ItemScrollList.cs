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
	public class ItemScrollList : Widget
	{
		public ItemScrollList()
		{
			this.headerView = new ItemListColumnHeaderView ()
			{
				Parent = this,
				Dock = DockStyle.Top,
				PreferredHeight = 24,
			};

			this.splitView = new VSplitView ()
			{
				Parent = this,
				Dock = DockStyle.Fill,
			};

			this.contentViews = new List<ContentView> ();
		}


		public ItemListColumnHeaderView			Header
		{
			get
			{
				return this.headerView;
			}
		}

		public ItemListSelection				Selection
		{
			get
			{
				if (this.itemLists == null)
				{
					return null;
				}
				else
				{
					return this.itemLists.Selection;
				}
			}
		}

		

		public ItemData<T> GetItemData<T>(int index)
		{
			var data = this.itemCache.GetItemData (index) as ItemData<T>;

			return data;
		}


		public void SetupItemList<TData, TState>(IItemDataProvider<TData> provider,
			/**/								 IItemDataMapper<TData> mapper,
			/**/								 IItemDataRenderer itemRenderer,
			/**/								 IItemMarkRenderer markRenderer = null,
			/**/								 ItemListFeatures features = null,
			/**/								 ItemListSelection selection = null,
			/**/								 IList<ItemListMark> marks = null)
			where TState : ItemState, new ()
		{
			if (features == null)
			{
				features = new ItemListFeatures ()
				{
					SelectionMode = ItemSelectionMode.ExactlyOne,
				};
			}

			int capacity = provider == null ? 100 : provider.Count;
			var cache   = new ItemCache<TData, TState> (capacity, features)
			{
				DataProvider = provider,
				DataMapper   = mapper,
			};

			this.itemCache = cache;
			
			if (selection == null)
			{
				selection = new ItemListSelection (this.itemCache);
			}

			if (this.itemLists != null)
			{
				this.DetachItemListEventHandlers ();
				this.itemLists = null;
			}

			this.itemLists = new ItemListCollection<TData, TState> (cache, marks, selection);

			this.AttachItemListEventHandlers ();

			this.contentViews.Add (new ContentView (this, itemRenderer, markRenderer, this.splitView.Frame1, this.splitView.Scroller1));
			this.contentViews.Add (new ContentView (this, itemRenderer, markRenderer, this.splitView.Frame2, this.splitView.Scroller2));
			
			this.itemCache.Reset ();
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			this.contentViews.ForEach (x => x.UpdateScroller ());
		}

		class ContentView
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

				this.host.itemCache.ResetFired += this.HandleItemCacheResetFired;
				this.view.ItemList.VisibleContentChanged += this.HandleVisibleContentChanged;
				this.scroller.ValueChanged += this.HandleScrollerValueChanged;

				this.UpdateScroller ();
			}

			private void HandleItemCacheResetFired(object sender)
			{
				this.UpdateScroller ();
			}

			private void HandleScrollerValueChanged(object sender)
			{
				this.view.ItemList.VisibleIndex = (int) this.scroller.Value;
				this.view.Invalidate ();
			}

			private void HandleVisibleContentChanged(object sender)
			{
				this.UpdateScroller ();
			}

			
			public void UpdateScroller()
			{
				decimal index   = this.view.ItemList.VisibleIndex;
				decimal visible = this.view.ItemList.VisibleCount;
				decimal total   = this.host.itemCache.ItemCount;

				this.scroller.MinValue          = 0;
				this.scroller.MaxValue          = total - 1;
				this.scroller.Resolution        = 1;
				this.scroller.VisibleRangeRatio = total == 0 ? 1 : visible / total;
				this.scroller.Value				= index;
			}

			private readonly ItemScrollList host;
			private readonly AbstractScroller	scroller;
			private readonly ItemListVerticalContentView view;

		}
		
		private void AttachItemListEventHandlers()
		{
			this.itemLists.ActiveIndexChanged  += this.HandleItemListsActiveIndexChanged;
			this.itemLists.FocusedIndexChanged += this.HandleItemListsFocusedIndexChanged;
			
			this.itemLists.Selection.SelectionChanged += this.HandleItemListsSelectionChanged;
		}
		
		private void DetachItemListEventHandlers()
		{
			this.itemLists.ActiveIndexChanged  -= this.HandleItemListsActiveIndexChanged;
			this.itemLists.FocusedIndexChanged -= this.HandleItemListsFocusedIndexChanged;
			
			this.itemLists.Selection.SelectionChanged -= this.HandleItemListsSelectionChanged;
		}

		
		private void HandleItemListsActiveIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			this.OnActiveIndexChanged (e);
			this.Invalidate ();
		}

		private void HandleItemListsFocusedIndexChanged(object sender, ItemListIndexEventArgs e)
		{
			this.Invalidate ();
		}

		private void HandleItemListsSelectionChanged(object sender, ItemListSelectionEventArgs e)
		{
			this.OnSelectionChanged (e);
			this.Invalidate ();
		}


		private void OnActiveIndexChanged(ItemListIndexEventArgs e)
		{
			var handler = this.ActiveIndexChanged;
			handler.Raise (this, e);
		}

		private void OnSelectionChanged(ItemListSelectionEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine ("Selection: {0}", e);

			var handler = this.SelectionChanged;
			handler.Raise (this, e);
		}
		
		public EventHandler<ItemListIndexEventArgs> ActiveIndexChanged;

		public EventHandler<ItemListSelectionEventArgs> SelectionChanged;


		private readonly VSplitView				splitView;
		private readonly ItemListColumnHeaderView headerView;
		private readonly List<ContentView>		contentViews;
		
		private ItemListCollection				itemLists;
		private ItemCache						itemCache;
	}
}
