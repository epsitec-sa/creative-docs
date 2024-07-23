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
using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityResolverResult</c> class represents the result of a call
    /// to the <see cref="M:EntityResolver.Resolve"/> method. It caches the
    /// data provided by the source enumerable and provides a collection view
    /// for it.
    /// </summary>
    public sealed class EntityResolverResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityResolverResult"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public EntityResolverResult(IEnumerable<AbstractEntity> entities)
        {
            this.entityEnumerator = entities == null ? null : entities.GetEnumerator();
            this.cache = new List<AbstractEntity>();
            this.view = new CollectionView(this.cache);
        }

        /// <summary>
        /// Gets the first result.
        /// </summary>
        /// <value>The first result.</value>
        public AbstractEntity FirstResult
        {
            get { return this.GetResult(0); }
        }

        /// <summary>
        /// Gets a list of all results.
        /// </summary>
        /// <value>All results.</value>
        public IList<AbstractEntity> AllResults
        {
            get
            {
                if (this.entityEnumerator != null)
                {
                    while (this.entityEnumerator.MoveNext())
                    {
                        this.cache.Add(this.entityEnumerator.Current);
                    }

                    this.entityEnumerator.Dispose();
                    this.entityEnumerator = null;
                }

                return this.cache;
            }
        }

        /// <summary>
        /// Gets the collection view representing the list of results.
        /// </summary>
        /// <value>The collection view.</value>
        public ICollectionView CollectionView
        {
            get { return this.view; }
        }

        /// <summary>
        /// Gets the empty result set.
        /// </summary>
        /// <value>The empty result set.</value>
        public static EntityResolverResult Empty
        {
            get { return new EntityResolverResult(null); }
        }

        /// <summary>
        /// Gets the result at the specified index. This uses a cache which is
        /// only filled on a demand base.
        /// </summary>
        /// <param name="index">The index of the result.</param>
        /// <returns>The result or <c>null</c>.</returns>
        private AbstractEntity GetResult(int index)
        {
            while (index >= this.cache.Count)
            {
                if (this.entityEnumerator == null)
                {
                    return null;
                }

                if (this.entityEnumerator.MoveNext())
                {
                    this.cache.Add(this.entityEnumerator.Current);
                }
                else
                {
                    this.entityEnumerator.Dispose();
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
