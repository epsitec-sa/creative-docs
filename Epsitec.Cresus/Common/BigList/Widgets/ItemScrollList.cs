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
	/// <summary>
	/// The <c>ItemScrollList</c> displays items based on an item list content view. The
	/// scroll list can be split in two, thanks to a <see cref="VSplitView"/>. If manages
	/// one or two vertical scrollers.
	/// </summary>
	public partial class ItemScrollList : Widget
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

		
		public int								ActiveIndex
		{
			get
			{
				if (this.itemLists == null)
				{
					return -1;
				}
				else
				{
					return this.itemLists.ActiveIndex;
				}
			}
			set
			{
				if (this.itemLists == null)
				{
					if (value == -1)
					{
						return;
					}

					throw new System.IndexOutOfRangeException ();
				}

				this.itemLists.ActiveIndex = value;
			}
		}
		
		public ItemListFeatures					Features
		{
			get
			{
				if (this.itemLists == null)
				{
					return null;
				}
				else
				{
					return this.itemLists.Features;
				}
			}
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

		public ItemCache						ItemCache
		{
			get
			{
				return this.itemCache;
			}
		}


		/// <summary>
		/// Gets the item data for the specified index. If the index is not valid or if the
		/// data cannot be retrieved from the <see cref="IItemDataProvider"/>, then this
		/// method returns simply <c>null</c>.
		/// </summary>
		/// <exception cref="System.ArgumentException">When the <typeparamref name="TData"/> does
		/// not match the real data type.</exception>
		/// <typeparam name="TData">The expected data type.</typeparam>
		/// <param name="index">The index.</param>
		/// <returns>The item data or <c>null</c>.</returns>
		public TData GetItemData<TData>(int index)
		{
			return this.itemCache.GetItemData (index).GetData<TData> ();
		}


		public void SetUpItemList<TData>(IItemDataProvider<TData> provider,
			/**/						 IItemDataMapper<TData> mapper,
			/**/						 IItemDataRenderer itemRenderer,
			/**/						 IItemMarkRenderer markRenderer = null,
			/**/						 ItemListFeatures features = null,
			/**/						 ItemListSelection selection = null,
			/**/						 IList<ItemListMark> marks = null)
		{
			this.SetUpItemList<TData, ItemState> (provider, mapper, itemRenderer, markRenderer, features, selection, marks);
		}

		public void SetUpItemList<TData, TState>(IItemDataProvider<TData> provider,
			/**/								 IItemDataMapper<TData> mapper,
			/**/								 IItemDataRenderer itemRenderer,
			/**/								 IItemMarkRenderer markRenderer = null,
			/**/								 ItemListFeatures features = null,
			/**/								 ItemListSelection selection = null,
			/**/								 IList<ItemListMark> marks = null)
			where TState : ItemState, new ()
		{
			this.ClearItemList ();

			if (features == null)
			{
				features = new ItemListFeatures ()
				{
					SelectionMode = ItemSelectionMode.ExactlyOne,
				};
			}

			int capacity = provider == null ? 100 : provider.Count;
			
			var cache = new ItemCache<TData, TState> (capacity, features)
			{
				DataProvider = provider,
				DataMapper   = mapper,
			};

			this.itemCache = cache;
			
			if (selection == null)
			{
				selection = new ItemListSelection (this.itemCache);
			}

			this.itemLists = new ItemListCollection<TData, TState> (cache, marks, selection);

			this.AttachItemListEventHandlers ();

			this.contentViews.Add (new ContentView (this, itemRenderer, markRenderer, this.splitView.Frame1, this.splitView.Scroller1));
			this.contentViews.Add (new ContentView (this, itemRenderer, markRenderer, this.splitView.Frame2, this.splitView.Scroller2));
			
			this.itemCache.Reset ();
		}


		public void RefreshContents()
		{
			this.itemLists.Reset ();
			this.Invalidate ();
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.ClearItemList ();
			}

			base.Dispose (disposing);
		}
		
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();

			this.contentViews.ForEach (x => x.UpdateScroller ());
		}

		private void ClearItemList()
		{
			if (this.itemLists != null)
			{
				this.DetachItemListEventHandlers ();
				this.itemLists = null;
			}

			if (this.contentViews.Count > 0)
			{
				this.contentViews.ForEach (x => x.Dispose ());
				this.contentViews.Clear ();
			}
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
