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

namespace Epsitec.Common.Support
{
    /// <summary>
    /// The <c>EntityFieldAttribute</c> class defines an <c>EntityField</c>
    /// attribute, which is used by <see cref="EntityContext"/> to map fields
    /// to .NET class properties.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EntityFieldAttribute : System.Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFieldAttribute"/> class.
        /// </summary>
        /// <param name="fieldId">The field id.</param>
        public EntityFieldAttribute(string fieldId)
        {
            this.fieldId = fieldId;
        }

        /// <summary>
        /// Gets the field id.
        /// </summary>
        /// <value>The field id.</value>
        public string FieldId
        {
            get { return this.fieldId; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is virtual.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this field is virtual; otherwise, <c>false</c>.
        /// </value>
        public bool IsVirtual
        {
            get { return this.isVirtual; }
            set { this.isVirtual = value; }
        }

        private string fieldId;
        private bool isVirtual;
    }
}
