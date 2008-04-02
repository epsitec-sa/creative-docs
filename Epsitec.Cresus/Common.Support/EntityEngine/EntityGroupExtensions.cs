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
			GroupBucket<T> bucket = new GroupBucket<T> ();

			foreach (T item in collection)
			{
				object key = keySelector (item);
				EntityGroup<T> group = bucket[key];
				group.Add (item);
			}

			return bucket.Groups;
		}

		
		class GroupBucket<T> : Dictionary<object, EntityGroup<T>> where T : AbstractEntity, new ()
		{
			public new EntityGroup<T> this[object key]
			{
				get
				{
					EntityGroup<T> group;
					
					if (this.TryGetValue (key, out group))
					{
						//	OK, nothing more to do here
					}
					else
					{
						group = new EntityGroup<T> ()
						{
							Key = key
						};
						
						base[key] = group;
						this.keys.Add (key);
					}
					
					return group;
				}
			}

			public IEnumerable<IGrouping<object, T>> Groups
			{
				get
				{
					foreach (object key in this.keys)
					{
						yield return base[key];
					}
				}
			}

			private readonly List<object> keys = new List<object> ();
		}
	}
}
