/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>CacheManager</c> class provides a central point to manage the
    /// various caches which are active at any point in time.
    /// </summary>
    public static class CacheManager
    {
        /// <summary>
        /// Initializes the <see cref="CacheManager"/> class.
        /// </summary>
        static CacheManager() { }

        /// <summary>
        /// Trims all the caches, removing dead entries.
        /// </summary>
        public static void TrimCaches()
        {
            CacheManager.ForEach(rec => rec.TrimCacheAction());
        }

        /// <summary>
        /// Registers a cache trimming function for the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="trimCacheAction">The cache trimming function.</param>
        public static void RegisterTrimCache(object cache, System.Action trimCacheAction)
        {
            CacheRecord record = CacheManager.GetCacheRecord(cache);

            record.TrimCacheAction = trimCacheAction;
        }

        /// <summary>
        /// Removes the cache from the cache manager.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public static void RemoveCache(object cache)
        {
            lock (CacheManager.exclusion)
            {
                CacheManager.records.Remove(cache);
            }
        }

        /// <summary>
        /// Loops through every cache record and applies the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void ForEach(System.Action<CacheRecord> action)
        {
            List<CacheRecord> records = new List<CacheRecord>();

            lock (CacheManager.exclusion)
            {
                records.AddRange(CacheManager.records.Values);
            }

            foreach (CacheRecord record in records)
            {
                action(record);
            }
        }

        /// <summary>
        /// Gets the cache record for the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <returns>The <c>CacheRecord</c>.</returns>
        private static CacheRecord GetCacheRecord(object cache)
        {
            CacheRecord record;

            lock (CacheManager.exclusion)
            {
                CacheManager.records.TryGetValue(cache, out record);

                if (record == null)
                {
                    record = new CacheRecord();
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
            public CacheRecord() { }

            public System.Action TrimCacheAction { get; set; }
        }

        #endregion


        private static readonly object exclusion = new object();
        private static readonly Dictionary<object, CacheRecord> records =
            new Dictionary<object, CacheRecord>();
    }
}
