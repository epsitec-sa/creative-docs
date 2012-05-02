//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItemMapper : IItemDataMapper<BrowserListItem>
	{
		#region IItemDataMapper<BrowserListItem> Members

		public ItemData<BrowserListItem> Map(BrowserListItem value)
		{
			ItemState state = new ItemState ()
			{
				Height = 18,
			};

			return new ItemData<BrowserListItem> (value, state);
		}

		#endregion
	}
}
