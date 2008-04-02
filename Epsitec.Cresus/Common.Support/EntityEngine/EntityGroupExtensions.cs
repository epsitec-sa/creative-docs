//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityGroupExtension</c> class implements extension methods used
	/// by the LINQ engine when manipulating <see cref="AbstractEntity"/>
	/// derived objects.
	/// </summary>
	public static class EntityGroupExtension
	{
		/// <summary>
		/// Implements the <c>GroupBy</c> operation used by LINQ when processing
		/// objects derived from <see cref="AbstractEntity"/>.
		/// </summary>
		/// <typeparam name="T">The object type, derived from <see cref="AbstractEntity"/>.</typeparam>
		/// <param name="collection">The collection.</param>
		/// <param name="keySelector">The key selector used for grouping.</param>
		/// <returns>The collection of groups, provided in the same order as the
		/// original collection.</returns>
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

		#region GroupBucket Class

		/// <summary>
		/// The <c>GroupBucket&lt;T&gt;</c> class implements a specialized
		/// dictionary.
		/// </summary>
		/// <typeparam name="T">The object type, derived from <see cref="AbstractEntity"/>.</typeparam>
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

			/// <summary>
			/// Gets the groups, in the same order as they were first
			/// encountered.
			/// </summary>
			/// <value>The groups.</value>
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

		#endregion
	}
}
