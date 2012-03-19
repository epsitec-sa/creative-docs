//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemList<T> : ItemList
	{
		public ItemList(IItemDataProvider<T> provider, IItemDataMapper<T> mapper)
		{
			this.cache = new ItemCache<T> (provider == null ? 100 : provider.Count)
			{
				DataProvider = provider,
				DataMapper = mapper,
			};
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


		private readonly ItemCache<T>			cache;
	}
}
