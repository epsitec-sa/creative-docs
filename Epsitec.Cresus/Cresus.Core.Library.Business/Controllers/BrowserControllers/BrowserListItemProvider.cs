//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItemProvider : IItemDataProvider<BrowserListItem>
	{
		public BrowserListItemProvider(BrowserList list)
		{
			this.list = list;
			this.reverseMap = new Dictionary<DbKey, int> ();
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
			
			this.reverseMap[value.RowKey] = index;

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


		public int IndexOf(EntityKey? entityKey)
		{
			if (entityKey.HasValue)
			{
				var rowKey = entityKey.Value.RowKey;
				int index;

				if (this.reverseMap.TryGetValue (rowKey, out index))
				{
					return index;
				}

				//	TODO: ask the database about this row

				return this.list.IndexOf (entityKey);
			}
			else
			{
				return -1;
			}
		}

		public void Reset()
		{
			this.reverseMap.Clear ();
		}

		private readonly BrowserList			list;
		private readonly Dictionary<DbKey, int>	reverseMap;
	}
}
