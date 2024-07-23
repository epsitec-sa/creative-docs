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
    /// The <c>EntityResolver</c> class provides support for common resolution
    /// tasks related to the <see cref="IEntityResolver"/> interface.
    /// </summary>
    public static class EntityResolver
    {
        /// <summary>
        /// Finds the most appropriate entity based on the specified resolver
        /// and search template.
        /// </summary>
        /// <param name="resolver">The entity resolver.</param>
        /// <param name="template">The search template.</param>
        /// <returns>The most appropriate entity or <c>null</c>.</returns>
        public static EntityResolverResult Resolve(
            IEntityResolver resolver,
            AbstractEntity template
        )
        {
            if (resolver != null)
            {
                return new EntityResolverResult(resolver.Resolve(template));
            }

            return null;
        }

        /// <summary>
        /// Finds the most appropriate entity based on the specified resolver
        /// and search criteria.
        /// </summary>
        /// <param name="resolver">The entity resolver.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The most appropriate entity or <c>null</c>.</returns>
        public static EntityResolverResult Resolve(
            IEntityResolver resolver,
            Druid entityId,
            string criteria
        )
        {
            if (resolver != null)
            {
                return new EntityResolverResult(resolver.Resolve(entityId, criteria));
            }

            return null;
        }
    }
}
