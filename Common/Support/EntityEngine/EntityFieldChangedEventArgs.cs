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
    /// The <c>EntityChangedEventArgs</c> class is used to report changes related to
    /// an entity contents.
    /// </summary>
    public class EntityFieldChangedEventArgs : EntityEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFieldChangedEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public EntityFieldChangedEventArgs(
            AbstractEntity entity,
            string id,
            object oldValue,
            object newValue
        )
            : base(entity)
        {
            this.id = id;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        /// <summary>
        /// Gets the id of the field which changed.
        /// </summary>
        /// <value>The id of the field.</value>
        public string Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets the old value, if any.
        /// </summary>
        /// <value>The old value.</value>
        public object OldValue
        {
            get { return this.oldValue; }
        }

        /// <summary>
        /// Gets the new value, if any.
        /// </summary>
        /// <value>The new value.</value>
        public object NewValue
        {
            get { return this.newValue; }
        }

        private readonly string id;
        private readonly object oldValue;
        private readonly object newValue;
    }
}
