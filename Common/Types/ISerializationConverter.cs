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
    /// The <c>ISerializationConverter</c> is used by the serialization engine
    /// to convert an object to a string (when serializing) and vice versa (when
    /// deserializing).
    /// </summary>
    public interface ISerializationConverter
    {
        /// <summary>
        /// Converts the value to a serializable string.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="context">The serialization context.</param>
        /// <returns>The value represented as a string.</returns>
        string ConvertToString(object value, IContextResolver context);

        /// <summary>
        /// Converts the serializable string to its live value.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="context">The deserialization context.</param>
        /// <returns>The value represented by the string.</returns>
        object ConvertFromString(string value, IContextResolver context);
    }
}
