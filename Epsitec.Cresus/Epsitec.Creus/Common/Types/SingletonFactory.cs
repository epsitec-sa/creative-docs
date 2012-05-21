//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>SingletonFactory</c> class is used to retrieve singleton instances of some runtime
	/// type. An internal cache is used to avoid allocating singletons more than once for a given
	/// type.
	/// </summary>
	public static class SingletonFactory
	{
		public static T GetSingleton<T>(System.Type type)
			where T : class
		{
			SingletonFactory.cacheExclusion.Wait ();

			try
			{
				object value;

				if (SingletonFactory.cache.TryGetValue (type, out value) == false)
				{
					value = System.Activator.CreateInstance (type);
					SingletonFactory.cache[type] = value;
				}

				return value as T;
			}
			finally
			{
				SingletonFactory.cacheExclusion.Release ();
			}
		}

		public static T GetSingleton<T>(System.Type genericType, params System.Type[] typeArguments)
			where T : class
		{
			return SingletonFactory.GetSingleton<T> (genericType.MakeGenericType (typeArguments));
		}


		private static System.Threading.SemaphoreSlim	cacheExclusion	= new System.Threading.SemaphoreSlim (1);
		private static Dictionary<System.Type, object>	cache			= new Dictionary<System.Type, object> ();
	}
}
