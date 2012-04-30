//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.BigList
{
	public class ItemList<TData> : ItemList<TData, ItemState>
	{
		public ItemList(IItemDataProvider<TData> provider, IItemDataMapper<TData> mapper, ItemListSelection selection)
			: base (provider, mapper, selection)
		{
		}
	}
}
