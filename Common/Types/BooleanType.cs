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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.BooleanType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe BooleanType décrit des valeurs de type System.Boolean.
    /// </summary>
    public class BooleanType : AbstractNumericType
    {
        public BooleanType()
            : base("Boolean", new DecimalRange(0, 1, 1)) { }

        public BooleanType(Caption caption)
            : base(caption) { }

        public override System.Type SystemType
        {
            get { return typeof(bool); }
        }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Boolean; }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is bool)
            {
                return true;
            }

            return false;
        }

        static BooleanType()
        {
            DependencyPropertyMetadata metadata = new DependencyPropertyMetadata("Boolean");

            BooleanType.DefaultControllerProperty.OverrideMetadata(typeof(BooleanType), metadata);
        }

        public static BooleanType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (BooleanType.defaultValue == null)
                {
                    BooleanType.defaultValue = (BooleanType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[1003]"));
                }

                return BooleanType.defaultValue;
            }
        }

        private static BooleanType defaultValue;
    }
}
