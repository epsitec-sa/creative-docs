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
    /// The <c>UndefinedValue</c> represents the value for a property which has
    /// not been set. Compare with <see cref="InvalidValue"/>.
    /// </summary>
    public sealed class UndefinedValue
    {
        private UndefinedValue() { }

        /// <summary>
        /// Determines whether the object maps to an undefined value.
        /// </summary>
        /// <param name="value">The object which should be tested.</param>
        /// <returns><c>true</c> if the object is an undefined value; otherwise,
        /// <c>false</c>.</returns>
        [System.Diagnostics.DebuggerStepThrough]
        public static bool IsUndefinedValue(object value)
        {
            return (value == UndefinedValue.Value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the undefined value.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the undefined value.</returns>
        public override string ToString()
        {
            return "<UndefinedValue>";
        }

        /// <summary>
        /// Casts the value to the specified type. If the value is undefined,
        /// returns the default value instead.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>Either the casted value or the default value.</returns>
        public static T GetValue<T>(object value, T defaultValue)
        {
            if (value == UndefinedValue.Value)
            {
                return defaultValue;
            }
            else
            {
                return (T)value;
            }
        }

        /// <summary>
        /// Casts the value to the specified type. If the value is undefined,
        /// returns the default value instead.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="isNullEqualToUndefined">if set to <c>true</c> treat <c>null</c> as an undefined value too.</param>
        /// <returns>
        /// Either the casted value or the default value.
        /// </returns>
        public static T GetValue<T>(object value, T defaultValue, bool isNullEqualToUndefined)
        {
            if ((value == UndefinedValue.Value) || (isNullEqualToUndefined && (value == null)))
            {
                return defaultValue;
            }
            else
            {
                return (T)value;
            }
        }

        public static readonly UndefinedValue Value = new UndefinedValue();
    }
}
