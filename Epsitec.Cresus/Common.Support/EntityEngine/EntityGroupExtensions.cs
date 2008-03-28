//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	public static class EntityGroupExtension
	{
		public static IEnumerable<IGrouping<object, T>> GroupBy<T>(this IEnumerable<T> collection, System.Func<T, object> keySelector) where T : AbstractEntity, new ()
		{
			Dictionary<object, EntityGroup<T>> buckets = new Dictionary<object, EntityGroup<T>> ();

			foreach (T item in collection)
			{
				object key = keySelector (item);
				EntityGroup<T> group;
				if (buckets.TryGetValue (key, out group))
				{
					group.Add (item);
				}
				else
				{
					group = new EntityGroup<T> ()
					{
						Key = key
					};
					group.Add (item);
					buckets[key] = group;
				}
			}

			foreach (var item in buckets.Values)
			{
				yield return item;
			}
		}
	}
}
