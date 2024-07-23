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
    /// The <c>INullableType</c> interface can be used to check whether an object
    /// of a given type represents the <c>null</c> value.
    /// </summary>
    public interface INullableType
    {
        /// <summary>
        /// Gets a value indicating whether the value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value represents the <c>null</c> value; otherwise, <c>false</c>.
        /// </returns>
        bool IsNullValue(object value);

        /// <summary>
        /// Gets a value indicating whether this type may represent <c>null</c> values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this type may represent <c>null</c> values; otherwise, <c>false</c>.
        /// </value>
        bool IsNullable { get; }
    }
}
