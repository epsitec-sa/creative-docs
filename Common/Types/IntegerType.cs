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


[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Types.IntegerType))]

namespace Epsitec.Common.Types
{
    /// <summary>
    /// La classe IntegerType décrit une valeur de type System.Int32.
    /// </summary>
    public class IntegerType : AbstractNumericType
    {
        public IntegerType()
            : this(int.MinValue, int.MaxValue) { }

        public IntegerType(int min, int max)
            : base("Integer", new DecimalRange(min, max)) { }

        public IntegerType(Caption caption)
            : base(caption) { }

        /// <summary>
        /// Gets the type code for the type.
        /// </summary>
        /// <value>The type code.</value>
        public override TypeCode TypeCode
        {
            get { return TypeCode.Integer; }
        }

        public override System.Type SystemType
        {
            get { return typeof(int); }
        }

        public override bool IsValidValue(object value)
        {
            if (this.IsNullValue(value))
            {
                return this.IsNullable;
            }

            if (value is int)
            {
                if (this.Range.IsEmpty)
                {
                    return true;
                }
                else
                {
                    int num = (int)value;

                    return this.Range.Constrain(num) == num;
                }
            }

            return false;
        }

        public static IntegerType Default
        {
            get
            {
                TypeRosetta.InitializeKnownTypes();

                if (IntegerType.defaultValue == null)
                {
                    IntegerType.defaultValue = (IntegerType)
                        TypeRosetta.CreateTypeObject(Support.Druid.Parse("[1006]"));
                }

                return IntegerType.defaultValue;
            }
        }

        private static IntegerType defaultValue;
    }
}
