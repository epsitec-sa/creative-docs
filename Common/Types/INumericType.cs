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
    /// The <c>INumericType</c> interface describes a numeric type which is
    /// constrained by a <see cref="T:DecimalRange"/>.
    /// </summary>
    public interface INumericType : INamedType
    {
        /// <summary>
        /// Gets the range of values accepted by this numeric type.
        /// </summary>
        /// <value>The range of values.</value>
        DecimalRange Range { get; }

        /// <summary>
        /// Gets the preferred range of values. This is not used as a constraint;
        /// it is just a hint for the user interface controls.
        /// </summary>
        /// <value>The preferred range of values.</value>
        DecimalRange PreferredRange { get; }

        /// <summary>
        /// Gets the value which should be used by the user interface to increment
        /// or decrement a number by a small amount.
        /// </summary>
        /// <value>The small step value.</value>
        decimal SmallStep { get; }

        /// <summary>
        /// Gets the value which should be used by the user interface to increment
        /// or decrement a number by a large amount.
        /// </summary>
        /// <value>The large step value.</value>
        decimal LargeStep { get; }

        /// <summary>
        /// Gets a value indicating whether this numeric type should use compact storage.
        /// </summary>
        /// <value><c>true</c> if this numeric type should use compact storage; otherwise, <c>false</c>.</value>
        bool UseCompactStorage { get; }
    }
}
