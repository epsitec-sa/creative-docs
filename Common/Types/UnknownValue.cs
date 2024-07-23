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
    /// The <c>UnknownValue</c> represents the item for a property which does
    /// not exist. Compare with <see cref="UndefinedValue"/>.
    /// </summary>
    public sealed class UnknownValue
    {
        private UnknownValue() { }

        /// <summary>
        /// Determines whether the object maps to an unknown item.
        /// </summary>
        /// <param name="value">The object which should be tested.</param>
        /// <returns><c>true</c> if the object is an unknown item; otherwise,
        /// <c>false</c>.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsUnknownValue(object value)
        {
            return (value == UnknownValue.Value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the unknown item.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the unknown item.</returns>
        public override string ToString()
        {
            return "<UnknownValue>";
        }

        public static readonly UnknownValue Value = new UnknownValue();
    }
}
