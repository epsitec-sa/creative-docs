//	Copyright � 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.BigList;
using Epsitec.Common.Debug;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Data;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Epsitec.Cresus.Core.Controllers.BrowserControllers
{
	public class BrowserListItemProvider : IItemDataProvider<BrowserListItem>
	{
		public BrowserListItemProvider(BrowserListContext context)
		{
			this.context  = context;
			
			this.dataCache  = new Dictionary<int, BrowserListItem> ();
			this.reverseMap = new Dictionary<DbKey, int> ();

			this.DefaultPrefetchCount = 50;
		}

		public int								DefaultPrefetchCount
		{
			get;
			set;
		}

		public DataSetAccessor					Accessor
		{
			get
			{
				var accessor = this.context.Accessor;

				accessor.ThrowIfNull ("accessor");

				return accessor;
			}
		}

		#region IItemDataProvider<BrowserListItem> Members

		public bool Resolve(int index, out BrowserListItem value)
		{
			if ((index < 0) ||
				(index >= this.Count))
			{
				value = null;
				return false;
			}

			long 탎 = Profiler.ElapsedMicroseconds (() => this.FillDataCache (index));

			System.Diagnostics.Debug.WriteLine ("Resolved index {0} in {1} 탎", index, 탎);

			return this.dataCache.TryGetValue (index, out value);
		}

		#endregion

		#region IItemDataProvider Members

		public int Count
		{
			get
			{
				if (this.count == null)
				{
					long 탎;

					this.count = Profiler.ElapsedMicroseconds (() => this.Accessor.GetItemCount (), out 탎);

					System.Diagnostics.Debug.WriteLine ("Retrieved item count in {0} 탎", 탎);
				}

				return this.count.Value;
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

				return this.Accessor.IndexOf (entityKey);
			}
			else
			{
				return -1;
			}
		}

		public void Reset()
		{
			this.reverseMap.Clear ();
			this.dataCache.Clear ();
			
			this.count = null;
		}


		private void FillDataCache(int start, int prefetchCount = 0)
		{
			if (prefetchCount == 0)
			{
				prefetchCount = this.DefaultPrefetchCount;
			}

			int end = this.GetNextKnownIndex (start, prefetchCount);

			if (end == start)
			{
				return;
			}

			this.LoadDataCache (start, end);
		}

		private void LoadDataCache(int first, int last)
		{
			var keys   = this.Accessor.GetItemKeys (first, last - first + 1);
			int offset = 0;

			for (int index = first; index <= last && offset < keys.Length; index++)
			{
				var key = keys[offset++];

				this.dataCache[index] = this.CreateBrowserListItem (key);
				this.reverseMap[key.RowKey] = index;
			}
		}

		private BrowserListItem CreateBrowserListItem(EntityKey entityKey)
		{
			var entity = this.context.ResolveEntity (entityKey);
			
			return new BrowserListItem (entity, entityKey);
		}


		private int GetNextKnownIndex(int start, int take)
		{
			int end = start + take;
			int max = this.Count;

			while (take-- > 0)
			{
				if ((this.dataCache.ContainsKey (start)) ||
					(start >= max))
				{
					return start;
				}

				start++;
			}
			
			return end;
		}


		private readonly BrowserListContext		context;
		private readonly Dictionary<int, BrowserListItem> dataCache;
		private readonly Dictionary<DbKey, int>	reverseMap;
		private int? count;
	}
}
