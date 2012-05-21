//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Common.BigList
{
	public abstract class ItemList : AbstractItemList
	{
		protected ItemList(ItemCache cache, IList<ItemListMark> marks, ItemListSelection selection)
			: base (cache, marks ?? new List<ItemListMark> (), selection ?? new ItemListSelection (cache))
		{
			this.visibleFrame = new ItemListVisibleFrame (this);
		}


		public ItemListVisibleFrame				VisibleFrame
		{
			get
			{
				return this.visibleFrame;
			}
		}


		protected override void ResetList()
		{
			this.ClearActiveIndex ();
			this.ClearFocusedIndex ();
			this.ClearVisibleFrame ();
		}

		public ItemState GetItemState(int index)
		{
			return this.Cache.GetItemState (index, ItemStateDetails.Full);
		}

		public ItemHeight GetItemHeight(int index)
		{
			return this.Cache.GetItemHeight (index);
		}

		public void SetItemState(int index, ItemState state)
		{
			this.Cache.SetItemState (index, state, ItemStateDetails.Full);
		}

		public void SetItemHeight(int index, int height)
		{
			var state = this.GetItemState (index);
			state.Height = height;
			this.SetItemState (index, state);
		}

		public ItemListMarkOffset GetOffset(ItemListMark mark)
		{
			return this.visibleFrame.GetOffset (mark);
		}




		public static ItemCache<TData, TState> CreateCache<TData, TState>(IItemDataProvider<TData> provider,
			/**/														  IItemDataMapper<TData> mapper,
			/**/														  ItemListFeatures features)
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

			var cache = new ItemCache<TData, TState> (capacity, features)
			{
				DataProvider = provider,
				DataMapper   = mapper,
			};

			cache.Reset ();

			return cache;
		}

		
		protected void ResetCache()
		{
			this.Cache.Reset ();
		}

		private void ClearVisibleFrame()
		{
			this.visibleFrame.Reset ();
		}
		
		private readonly ItemListVisibleFrame	visibleFrame;
	}
}