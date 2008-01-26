//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CacheManager</c> class provides a central point to manage the
	/// various caches which are active at any point in time.
	/// </summary>
	public static class CacheManager
	{
		static CacheManager()
		{
		}

		public static void TrimCaches()
		{
			CacheManager.ForEach (rec => rec.TrimCacheAction ());
		}

		public static void RegisterTrimCache(object cache, System.Action trimCacheAction)
		{
			CacheRecord record = CacheManager.GetCacheRecord (cache);

			record.TrimCacheAction = trimCacheAction;
		}

		public static void RemoveCache(object cache)
		{
			lock (CacheManager.exclusion)
			{
				CacheManager.records.Remove (cache);
			}
		}


		private static void ForEach(System.Action<CacheRecord> action)
		{
			List<CacheRecord> records = new List<CacheRecord> ();

			lock (CacheManager.exclusion)
			{
				records.AddRange (CacheManager.records.Values);
			}

			foreach (CacheRecord record in records)
			{
				action (record);
			}
		}

		
		private static CacheRecord GetCacheRecord(object cache)
		{
			CacheRecord record;

			lock (CacheManager.exclusion)
			{
				CacheManager.records.TryGetValue (cache, out record);

				if (record == null)
				{
					record = new CacheRecord ();
					CacheManager.records[cache] = record;
				}
			}

			return record;
		}

		#region CacheRecord Class

		/// <summary>
		/// The <c>CacheRecord</c> class stores the registered actions for a
		/// given cache object.
		/// </summary>
		private class CacheRecord
		{
			public CacheRecord()
			{
			}

			public System.Action TrimCacheAction
			{
				get;
				set;
			}
		}

		#endregion


		private static readonly object exclusion = new object ();
		private static readonly Dictionary<object, CacheRecord> records = new Dictionary<object, CacheRecord> ();
	}
}
