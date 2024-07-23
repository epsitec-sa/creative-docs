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


using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityFieldConverter</c> class is used to efficiently map <see cref="EntityField"/>
    /// and <see cref="PropertyInfo"/> instances, by using internal caches.
    /// </summary>
    internal static class EntityFieldConverter
    {
        /// <summary>
        /// Converts the field to the corresponding property info.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The property info.</returns>
        public static PropertyInfo ConvertToProperty(EntityField field)
        {
            PropertyInfo match;

            if (EntityFieldConverter.propertyInfoCache.TryGetValue(field, out match))
            {
                return match;
            }

            var entityType = EntityInfo.GetType(field.EntityId);
            var fieldId = field.FieldId;

            var properties =
                from property in entityType.GetProperties()
                where
                    property
                        .GetCustomAttributes<EntityFieldAttribute>(true)
                        .Any(a => a.FieldId == fieldId)
                select property;

            match = properties.FirstOrDefault();

            EntityFieldConverter.propertyInfoCache[field] = match;

            return match;
        }

        /// <summary>
        /// Converts the property info to the corresponding field.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns>The field.</returns>
        public static EntityField ConvertToEntityField(PropertyInfo propertyInfo)
        {
            EntityField match;

            if (EntityFieldConverter.entityFieldCache.TryGetValue(propertyInfo, out match))
            {
                return match;
            }

            match = new EntityField(propertyInfo);

            EntityFieldConverter.entityFieldCache[propertyInfo] = match;

            return match;
        }

        private static readonly ConcurrentDictionary<EntityField, PropertyInfo> propertyInfoCache =
            new ConcurrentDictionary<EntityField, PropertyInfo>();
        private static readonly ConcurrentDictionary<PropertyInfo, EntityField> entityFieldCache =
            new ConcurrentDictionary<PropertyInfo, EntityField>();
    }
}
