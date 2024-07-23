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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.OtherType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>OtherType</c> class describes a generic system type which does
    /// not fit into any of the predefined type categories.
    /// </summary>
    public sealed class OtherType : AbstractType
    {
        public OtherType()
            : base("Other") { }

        public OtherType(Caption caption)
            : base(caption) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Other; }
        }

        #region ISystemType Members

        public override System.Type SystemType
        {
            get
            {
                string systemTypeName = AbstractType.GetSystemType(this.Caption);

                if (string.IsNullOrEmpty(systemTypeName))
                {
                    return null;
                }

                System.Type systemType = AbstractType.GetSystemTypeFromSystemTypeName(
                    systemTypeName
                );

                return systemType;
            }
        }

        #endregion

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            System.Type expectedType = this.SystemType;

            if (expectedType == null)
            {
                return false;
            }
            else
            {
                return expectedType.IsAssignableFrom(value.GetType());
            }
        }

        public void DefineSystemType(System.Type type)
        {
            AbstractType.SetSystemType(this.Caption, type);
        }
    }
}
