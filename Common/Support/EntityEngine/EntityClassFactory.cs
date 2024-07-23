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
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    using Assembly = System.Reflection.Assembly;

    /// <summary>
    /// The <c>EntityClassFactory</c> class is used to allocate entity instances
    /// based on entity ids. The mapping between entity id and entity class
    /// must be marked with the <see cref="EntityAttribute"/> attribute, at
    /// the <c>assembly</c> level.
    /// </summary>
    public class EntityClassFactory
        : PlugIns.PlugInFactory<AbstractEntity, EntityClassAttribute, Druid>
    {
        /// <summary>
        /// Creates an empty entity instance.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The new entity instance or <c>null</c> if the id
        /// cannot be resolved.</returns>
        public static AbstractEntity CreateEmptyEntity(Druid id)
        {
            return EntityClassFactory.CreateInstance(id);
        }

        /// <summary>
        /// Finds the system type of an entity based on its entity id.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The entity type if it is known; otherwise, <c>null</c>.</returns>
        internal static System.Type FindEntityType(Druid id)
        {
            if (id.IsEmpty)
            {
                return null;
            }
            else
            {
                return EntityClassFactory.FindType(id);
            }
        }

        /// <summary>
        /// Gets the entity id for the specified entity type.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <returns>The <see cref="Druid"/> of the specified entity or <c>Druid.Empty</c>.</returns>
        internal static Druid GetEntityId(System.Type type)
        {
            return EntityClassFactory.FindId(type);
        }

        /// <summary>
        /// Gets all the entity ids for all known entities in the current application domain.
        /// </summary>
        /// <returns>The collection of <see cref="Druid"/> of the known entities.</returns>
        internal static IEnumerable<Druid> GetAllEntityIds()
        {
            return EntityClassFactory.FindAll().Select(x => x.Item1);
        }
    }
}
