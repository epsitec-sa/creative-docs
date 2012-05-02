//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItemProvider : IItemDataProvider<BrowserListItem>
	{
		public BrowserListItemProvider(BrowserList list)
		{
			this.list = list;
		}

		#region IItemDataProvider<BrowserListItem> Members

		public bool Resolve(int index, out BrowserListItem value)
		{
			if ((index < 0) ||
				(index >= this.list.Count))
			{
				value = null;
				return false;
			}

			value = this.list[index];

			return true;
		}

		#endregion

		#region IItemDataProvider Members

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		#endregion

		private readonly BrowserList			list;
	}
}
