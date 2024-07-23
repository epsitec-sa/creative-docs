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
    /// The <c>EntityEventArgs</c> class is used to report changes related to
    /// an entity.
    /// </summary>
    public class EntityEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityEventArgs(AbstractEntity entity)
        {
            this.entity = entity;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        /// <value>The entity.</value>
        public AbstractEntity Entity
        {
            get { return this.entity; }
        }

        private AbstractEntity entity;
    }
}
