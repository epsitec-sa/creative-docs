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
			this.itemCache.Reset ();
			
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
			
			var list1 = this.itemLists.Create ();
			var list2 = this.itemLists.Create ();

			var view1 = new ItemListVerticalContentView ()
			{
				Parent   = this.splitView.Frame1,
				Dock = DockStyle.Fill,
				ItemList = list1,
				ItemRenderer = itemRenderer,
				MarkRenderer = markRenderer,
			};

			var view2 = new ItemListVerticalContentView ()
			{
				Parent   = this.splitView.Frame2,
				Dock = DockStyle.Fill,
				ItemList = list2,
				ItemRenderer = itemRenderer,
				MarkRenderer = markRenderer,
			};
		}

		private void AttachItemListEventHandlers()
		{
			this.itemLists.ActiveIndexChanged += this.HandleItemListsActiveIndexChanged;
			this.itemLists.FocusedIndexChanged += this.HandleItemListsFocusedIndexChanged;
			this.itemLists.Selection.SelectionChanged += this.HandleItemListsSelectionChanged;
		}
		
		private void DetachItemListEventHandlers()
		{
			this.itemLists.ActiveIndexChanged -= this.HandleItemListsActiveIndexChanged;
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
		
		private ItemListCollection				itemLists;
		private ItemCache						itemCache;
	}
}
