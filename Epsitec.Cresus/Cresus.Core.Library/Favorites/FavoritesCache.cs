//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Favorites
{
	public sealed class FavoritesCache
	{
		private FavoritesCache()
		{
			this.cache = new Dictionary<System.Guid, FavoritesCollection> ();
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

			lock (this.cache)
			{
				this.cache[guid] = collection;
			}

			return guid.ToString ();
		}

		public string Push(System.Collections.IEnumerable collection, System.Type type, Druid databaseId)
		{
			var args = new object[] { collection, databaseId };
			return this.Push (System.Activator.CreateInstance (typeof (FavoritesCollection<>).MakeGenericType (type), args) as FavoritesCollection);
		}

		public FavoritesCollection<T> Find<T>(string key)
			where T : AbstractEntity, new ()
		{
			return this.Find (key) as FavoritesCollection<T>;
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


		private static readonly FavoritesCache	current = new FavoritesCache ();

		private readonly Dictionary<System.Guid, FavoritesCollection> cache;
	}
}