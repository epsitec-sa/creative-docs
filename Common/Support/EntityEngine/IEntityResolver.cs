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

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>IEntityResolver</c> interface is used to resolve entities based
    /// on partial information.
    /// </summary>
    public interface IEntityResolver
    {
        /// <summary>
        /// Gets a collection of entities matching the specified template.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>The collection of entities.</returns>
        IEnumerable<AbstractEntity> Resolve(AbstractEntity template);

        /// <summary>
        /// Gets a collection of entities matching the specified criteria.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The collection of entities.</returns>
        IEnumerable<AbstractEntity> Resolve(Druid entityId, string criteria);
    }
}
