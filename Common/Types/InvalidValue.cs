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
    /// The <c>InvalidValue</c> class represents an invalid value. Do not confuse
    /// this with an undefined value which is represented by an instance of
    /// class <see cref="UndefinedValue"/>.
    /// </summary>
    public sealed class InvalidValue
    {
        private InvalidValue() { }

        /// <summary>
        /// Determines whether the object is the an invalid value.
        /// </summary>
        /// <param name="value">The object which should be tested.</param>
        /// <returns><c>true</c> if the object is an invalid value; otherwise,
        /// <c>false</c>.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsInvalidValue(object value)
        {
            return (value == InvalidValue.Value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the invalid value.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the invalid value.</returns>
        public override string ToString()
        {
            return "<InvalidValue>";
        }

        public static readonly InvalidValue Value = new InvalidValue();
    }
}
