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


namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityContextEventArgs</c> class is used to report changes related to
    /// an entity with respect to an <see cref="EntityContext"/>.
    /// </summary>
    public class EntityContextEventArgs : EntityEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityContextEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="oldContext">The old context.</param>
        /// <param name="newContext">The new context.</param>
        public EntityContextEventArgs(
            AbstractEntity entity,
            EntityContext oldContext,
            EntityContext newContext
        )
            : base(entity)
        {
            this.oldContext = oldContext;
            this.newContext = newContext;
        }

        /// <summary>
        /// Gets the old context.
        /// </summary>
        /// <value>The context.</value>
        public EntityContext OldContext
        {
            get { return this.oldContext; }
        }

        /// <summary>
        /// Gets the new context.
        /// </summary>
        /// <value>The context.</value>
        public EntityContext NewContext
        {
            get { return this.newContext; }
        }

        private EntityContext oldContext;
        private EntityContext newContext;
    }
}
