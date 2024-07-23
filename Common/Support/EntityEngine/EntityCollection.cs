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
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityCollection</c> class is used as a common base class for all
    /// its generic versions.
    /// </summary>
    public abstract class EntityCollection : ObservableList<object>
    {
        public bool IsVirtualizerEnabled
        {
            get { return this.enableVirtualizer; }
        }

        /// <summary>
        /// Enables the virtualization of null entities. See <see cref="EntityNullReferenceVirtualizer"/>.
        /// </summary>
        internal void EnableEntityNullReferenceVirtualizer()
        {
            this.enableVirtualizer = true;
        }

        /// <summary>
        /// Virtualizes the null references of the specified entity, if this has
        /// been enabled for this collection.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="entity">The entity.</param>
        protected void Virtualize<T>(T entity)
            where T : AbstractEntity
        {
            if (this.enableVirtualizer)
            {
                EntityNullReferenceVirtualizer.PatchNullReferences(entity);
            }
        }

        private bool enableVirtualizer;
    }
}
