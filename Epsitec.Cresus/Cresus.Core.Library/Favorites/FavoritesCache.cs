//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Favorites
{
	/// <summary>
	/// The <c>FavoritesCache</c> class manages persistent collection of items, which are
	/// long-lived and identified by a unique ID.
	/// </summary>
	public sealed class FavoritesCache
	{
		private FavoritesCache()
		{
			this.cache = new Dictionary<System.Guid, FavoritesCollection> ();
			this.hashMap = new Dictionary<string, Record> ();
		}


		public static FavoritesCache			Current
		{
			get
			{
				return FavoritesCache.current;
			}
		}

		
		public string Push(FavoritesCollection collection)
		{
			var guid = System.Guid.NewGuid ();
			var hash = collection.StrongHash;

			lock (this.hashMap)
			{
				Record record;

				//	If a collection with the same strong hash could be found in the cache, don't
				//	store the new collection, but rather share the already existing one.
				
				if (this.hashMap.TryGetValue (hash, out record))
				{
					return record.Guid.ToString ();
				}

				record = new Record ()
				{
					Collection = collection,
					Guid = guid,
				};

				this.hashMap[hash] = record;
			}

			lock (this.cache)
			{
				this.cache[guid] = collection;
			}

			return guid.ToString ();
		}

		public string Push(IEnumerable<AbstractEntity> collection, System.Type type, Druid databaseId)
		{
			var favorites = new FavoritesCollection (collection, type, databaseId);

			return this.Push (favorites);
		}

		public FavoritesCollection Find(string key)
		{
			var guid = System.Guid.Parse (key);
			FavoritesCollection collection = null;

			lock (this.cache)
			{
				this.cache.TryGetValue (guid, out collection);
			}
			
			return collection;
		}


		#region Record structure

		private struct Record
		{
			public FavoritesCollection			Collection;
			public System.Guid					Guid;
		}

		#endregion

		private static readonly FavoritesCache	current = new FavoritesCache ();

		private readonly Dictionary<System.Guid, FavoritesCollection>	cache;
		private readonly Dictionary<string, Record>	hashMap;
	}
}
