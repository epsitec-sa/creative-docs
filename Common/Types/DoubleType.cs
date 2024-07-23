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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.DoubleType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe DoubleType décrit des valeurs de type System.Double.
    /// </summary>
    public class DoubleType : AbstractNumericType
    {
        public DoubleType()
            : this(DecimalRange.Empty) { }

        public DoubleType(DecimalRange range)
            : base("Double", range) { }

        public DoubleType(Caption caption)
            : base(caption) { }

        public DoubleType(decimal min, decimal max, decimal resolution)
            : this(new DecimalRange(min, max, resolution)) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Double; }
        }

        public override System.Type SystemType
        {
            get { return typeof(double); }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is double)
            {
                if (this.Range.IsEmpty)
                {
                    return true;
                }
                else
                {
                    decimal num = (decimal)(double)value;
                    return this.Range.Constrain(num) == num;
                }
            }

            return false;
        }

        public const decimal TenTo24 = 1000000M * 1000000M * 1000000M * 1000000M;
        public const decimal TenTo28 = 1000000M * 1000000M * 1000000M * 1000000M * 10000M;

        public static DoubleType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (DoubleType.defaultValue == null)
                {
                    DoubleType.defaultValue = (DoubleType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[1005]"));
                }

                return DoubleType.defaultValue;
            }
        }

        private static DoubleType defaultValue;
    }
}
