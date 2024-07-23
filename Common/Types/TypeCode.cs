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


namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>TypeCode</c> enumeration lists all well known types, for which
    /// there is an <see cref="AbstractType"/> derived class.
    /// </summary>
    [DesignerVisible]
    public enum TypeCode
    {
        /// <summary>
        /// Invalid or unsupported type.
        /// </summary>
        Invalid = 0,

        /// <summary>
        /// Boolean type, defined by <see cref="BooleanType"/>.
        /// </summary>
        Boolean = 10,

        /// <summary>
        /// Integer type, defined by <see cref="IntegerType"/>.
        /// </summary>
        Integer,

        /// <summary>
        /// Long integer type, defined by <see cref="LongIntegerType"/>.
        /// </summary>
        LongInteger,

        /// <summary>
        /// Decimal type, defined by <see cref="DecimalType"/>.
        /// </summary>
        Decimal,

        /// <summary>
        /// Double type, defined by <see cref="DoubleType"/>.
        /// </summary>
        Double,

        /// <summary>
        /// Date and time type, defined by <see cref="DateTimeType"/>.
        /// </summary>
        DateTime = 20,

        /// <summary>
        /// Date type, defined by <see cref="DateType"/>.
        /// </summary>
        Date,

        /// <summary>
        /// Time type, defined by <see cref="TimeType"/>.
        /// </summary>
        Time,

        /// <summary>
        /// Binary type, defined by <see cref="BinaryType"/>.
        /// </summary>
        Binary = 30,

        /// <summary>
        /// String type, defined by <see cref="StringType"/>.
        /// </summary>
        String,

        /// <summary>
        /// Enumeration type, defined by <see cref="EnumType"/>.
        /// </summary>
        Enum,

        /// <summary>
        /// Structured type, defined by <see cref="StructuredType"/>.
        /// </summary>
        Structured,

        /// <summary>
        /// Dynamic structured type, defined by <see cref="DynamicStructuredType"/>.
        /// </summary>
        Dynamic,

        /// <summary>
        /// Collection type, defined by <see cref="CollectionType"/>.
        /// </summary>
        Collection = 40,

        /// <summary>
        /// Other type, not directly supported by a specific type class, defined
        /// by <see cref="OtherType"/>.
        /// </summary>
        Other = 100,
    }
}
