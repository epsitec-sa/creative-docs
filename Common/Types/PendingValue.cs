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
    /// The <c>PendingValue</c> represents the value for an asynchronous binding
    /// which has not yet returned a result.
    /// </summary>
    public sealed class PendingValue
    {
        private PendingValue() { }

        /// <summary>
        /// Determines whether the object maps to a pending value.
        /// </summary>
        /// <param name="value">The object which should be tested.</param>
        /// <returns><c>true</c> if the object is a pending value; otherwise,
        /// <c>false</c>.</returns>
        public static bool IsPendingValue(object value)
        {
            return (value == PendingValue.Value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the pending value.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the pending value.</returns>
        public override string ToString()
        {
            return "<PendingValue>";
        }

        public static readonly PendingValue Value = new PendingValue();
    }
}
