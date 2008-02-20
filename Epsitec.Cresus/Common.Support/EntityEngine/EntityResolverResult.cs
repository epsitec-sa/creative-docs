//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	/// <summary>
	/// The <c>EntityResolverResult</c> class represents the result of a call
	/// to the <see cref="M:EntityResolver.Resolve"/> method.
	/// </summary>
	public sealed class EntityResolverResult
	{
		public EntityResolverResult(IEnumerable<AbstractEntity> entities)
		{
			this.entityEnumerator = entities == null ? null : entities.GetEnumerator ();
			this.cache = new List<AbstractEntity> ();
			this.view = new CollectionView (this.cache);
		}


		public AbstractEntity FirstResult
		{
			get
			{
				return this.GetResult (0);
			}
		}

		public IList<AbstractEntity> AllResults
		{
			get
			{
				if (this.entityEnumerator != null)
				{
					while (this.entityEnumerator.MoveNext ())
					{
						this.cache.Add (this.entityEnumerator.Current);
					}

					this.entityEnumerator.Dispose ();
					this.entityEnumerator = null;
				}

				return this.cache;
			}
		}

		public ICollectionView CollectionView
		{
			get
			{
				return this.view;
			}
		}

		public static EntityResolverResult Empty
		{
			get
			{
				return new EntityResolverResult (null);
			}
		}


		private AbstractEntity GetResult(int index)
		{
			while (index >= this.cache.Count)
			{
				if (this.entityEnumerator == null)
				{
					return null;
				}

				if (this.entityEnumerator.MoveNext ())
				{
					this.cache.Add (this.entityEnumerator.Current);
				}
				else
				{
					this.entityEnumerator.Dispose ();
					this.entityEnumerator = null;
					
					return null;
				}
			}

			return this.cache[index];
		}

		


		private IEnumerator<AbstractEntity> entityEnumerator;
		private readonly List<AbstractEntity> cache;
		private readonly CollectionView view;
	}
}
