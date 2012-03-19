//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemList<T> : ItemList
	{
		public ItemList()
		{
			this.cache = new ItemCache<T> (1000);
		}

		protected override int GetItemHeight(int index)
		{
			return this.cache.GetItemHeight (index);
		}


		private readonly ItemCache<T>	cache;
	}
}
