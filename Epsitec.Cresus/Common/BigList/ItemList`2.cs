//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemList<TData, TState> : ItemList
		where TState : ItemState, new ()
	{
		public ItemList(ItemCache<TData, TState> cache, List<ItemListMark> marks, ItemListFeatures features)
			: base (marks, features)
		{
			this.cache = cache;
			this.cache.Reset ();
		}

		public ItemList(IItemDataProvider<TData> provider, IItemDataMapper<TData> mapper)
		{
			this.cache = new ItemCache<TData, TState> (provider == null ? 100 : provider.Count, this.features)
			{
				DataProvider = provider,
				DataMapper   = mapper,
			};

			this.cache.Reset ();
		}


		public override ItemCache				Cache
		{
			get
			{
				return this.cache;
			}
		}

		public override int						Count
		{
			get
			{
				var provider = this.cache.DataProvider;

				if (provider == null)
				{
					return 0;
				}

				return provider.Count;
			}
		}


		private readonly ItemCache<TData, TState> cache;
	}
}
