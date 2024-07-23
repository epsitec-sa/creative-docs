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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.DecimalType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe DecimalType décrit des valeurs de type System.Decimal.
    /// </summary>
    public class DecimalType : AbstractNumericType
    {
        public DecimalType()
            : this(int.MinValue, int.MaxValue, 1) { }

        public DecimalType(DecimalRange range)
            : base("Decimal", range) { }

        public DecimalType(decimal min, decimal max, decimal resolution)
            : this(new DecimalRange(min, max, resolution)) { }

        public DecimalType(Caption caption)
            : base(caption) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Decimal; }
        }

        public override System.Type SystemType
        {
            get { return typeof(decimal); }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is decimal)
            {
                if (this.Range.IsEmpty)
                {
                    return true;
                }
                else
                {
                    decimal num = (decimal)value;
                    return this.Range.Constrain(num) == num;
                }
            }

            return false;
        }

        public static DecimalType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (DecimalType.defaultValue == null)
                {
                    DecimalType.defaultValue = (DecimalType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[1004]"));
                }

                return DecimalType.defaultValue;
            }
        }

        private static DecimalType defaultValue;
    }
}
