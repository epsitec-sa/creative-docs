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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.LongIntegerType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe LongIntegerType décrit une valeur de type System.Int64.
    /// </summary>
    public class LongIntegerType : AbstractNumericType
    {
        public LongIntegerType()
            : this(long.MinValue, long.MaxValue) { }

        public LongIntegerType(long min, long max)
            : base("LongInteger", new DecimalRange(min, max)) { }

        public LongIntegerType(Caption caption)
            : base(caption) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.LongInteger; }
        }

        public override System.Type SystemType
        {
            get { return typeof(long); }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is long)
            {
                if (this.Range.IsEmpty)
                {
                    return true;
                }
                else
                {
                    long num = (long)value;
                    return this.Range.Constrain(num) == num;
                }
            }

            return false;
        }

        public static LongIntegerType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (LongIntegerType.defaultValue == null)
                {
                    LongIntegerType.defaultValue = (LongIntegerType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[1007]"));
                }

                return LongIntegerType.defaultValue;
            }
        }

        private static LongIntegerType defaultValue;
    }
}
