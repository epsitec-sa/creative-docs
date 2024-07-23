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
using System.Reflection;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <c>EntityField</c> class defines an entity field based on the entity's DRUID
    /// and the field's ID. <c>EntityField</c> can be mapped to a <see cref="PropertyInfo"/>.
    /// See also <see cref="EntityFieldPath"/>.
    /// </summary>
    public sealed class EntityField : System.IEquatable<EntityField>
    {
        public EntityField()
            : this(Druid.Empty, null) { }

        public EntityField(Druid entityId, string fieldId)
        {
            this.entityId = entityId;
            this.fieldId = fieldId ?? "";
        }

        public EntityField(PropertyInfo propertyInfo)
            : this(Druid.Empty, null)
        {
            if (propertyInfo == null)
            {
                return;
            }

            var structuredType = EntityInfo.GetStructuredType(propertyInfo.DeclaringType);
            var fieldAttribute = propertyInfo
                .GetCustomAttributes<EntityFieldAttribute>(true)
                .FirstOrDefault();

            if (
                (structuredType == null)
                || (fieldAttribute == null)
                || (fieldAttribute.FieldId == null)
            )
            {
                return;
            }

            this.entityId = structuredType.CaptionId;
            this.fieldId = fieldAttribute.FieldId;
        }

        public Druid EntityId
        {
            get { return entityId; }
        }

        public string FieldId
        {
            get { return fieldId; }
        }

        public static explicit operator PropertyInfo(EntityField field)
        {
            return EntityFieldConverter.ConvertToProperty(field);
        }

        public static explicit operator EntityField(PropertyInfo propertyInfo)
        {
            return EntityFieldConverter.ConvertToEntityField(propertyInfo);
        }

        public static EntityField Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new System.ArgumentNullException("vaue");
            }

            int pos = value.IndexOf(':');

            if (pos < 1)
            {
                throw new System.FormatException("EntityField is not properly formatted");
            }

            var druid = value.Substring(0, pos);
            var field = value.Substring(pos + 1);

            return new EntityField(Druid.Parse(druid), field);
        }

        public override string ToString()
        {
            return string.Concat(this.entityId.ToString(), ":", this.fieldId);
        }

        public override int GetHashCode()
        {
            return this.entityId.GetHashCode() ^ this.fieldId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EntityField);
        }

        #region IEquatable<EntityField> Members

        public bool Equals(EntityField other)
        {
            if (other == null)
            {
                return false;
            }

            return this.entityId == other.entityId && this.fieldId == other.fieldId;
        }

        #endregion


        private readonly Druid entityId;
        private readonly string fieldId;
    }
}
