//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Services
{
	/// <summary>
	/// The <c>UriCache&lt;T&gt;</c> class provides a thread specific cache,
	/// which maintains a mapping between an URI and its WCF proxy instance.
	/// </summary>
	/// <typeparam name="T">Any interface type.</typeparam>
	static class UriCache<T> where T : class
	{
		/// <summary>
		/// Adds the specified URI to instance mapping.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="instance">The instance.</param>
		public static void Add(System.Uri uri, T instance)
		{
			if (UriCache<T>.cache == null)
			{
				UriCache<T>.cache = new Dictionary<string, T> ();
			}

			UriCache<T>.cache[uri.OriginalString] = instance;
		}

		/// <summary>
		/// Resolves the specified instance to its associated URI.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>The associated URI, if any.</returns>
		public static System.Uri Resolve(T instance)
		{
			if (UriCache<T>.cache == null)
			{
				return null;
			}

			var result = from item in UriCache<T>.cache
						 where item.Value == instance
						 select new System.Uri (item.Key);

			return result.FirstOrDefault ();
		}

		/// <summary>
		/// Resolves the specified URI to its associated WCF proxy instance.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns>The associated WCF proxy instance, if any.</returns>
		public static T Resolve(System.Uri uri)
		{
			if (UriCache<T>.cache == null)
			{
				return null;
			}

			T instance;

			UriCache<T>.cache.TryGetValue (uri.OriginalString, out instance);

			return instance;
		}

		[System.ThreadStatic]
		static Dictionary<string, T> cache;
	}
}
