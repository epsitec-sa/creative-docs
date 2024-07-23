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
    /// The <c>IValueStore</c> interface provides the basic getter and setter
    /// used to access values in a generic data store.
    /// </summary>
    public interface IValueStore
    {
        /// <summary>
        /// Gets the value for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the value.</param>
        /// <returns>The value, or either <see cref="UndefinedValue.Value"/> if the
        /// value is currently undefined or <see cref="UnknownValue.Value"/> if the
        /// identifier does not map to a known value.</returns>
        object GetValue(string id);

        /// <summary>
        /// Sets the value for the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the value.</param>
        /// <param name="value">The value to store into the structure record;
        /// specifying <see cref="UndefinedValue.Value"/> clears the value.
        /// <see cref="UnknownValue.Value"/> may not be specified as a value.</param>
        /// <param name="mode">The set mode.</param>
        void SetValue(string id, object value, ValueStoreSetMode mode);
    }
}
