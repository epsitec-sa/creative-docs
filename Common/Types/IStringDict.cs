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
    /// The <c>IStringDict</c> interface gives access to a string based dictionary.
    /// </summary>
    public interface IStringDict
    {
        /// <summary>
        /// Gets the keys of the known values.
        /// </summary>
        /// <value>The keys.</value>
        string[] Keys { get; }

        /// <summary>
        /// Gets or sets the value with the specified key.
        /// </summary>
        /// <value>The <c>string</c> value.</value>
        string this[string key] { get; set; }

        /// <summary>
        /// Gets the number of known values.
        /// </summary>
        /// <value>The number of known values.</value>
        int Count { get; }

        /// <summary>
        /// Adds the specified key/value pair to the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add(string key, string value);

        /// <summary>
        /// Removes the specified key/value pair from the dictionary.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the key was successfully removed; otherwise, <c>false</c>.
        /// </returns>
        bool Remove(string key);

        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether the dictionary contains the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the dictionary contains the specified key; otherwise, <c>false</c>.
        /// </returns>
        bool ContainsKey(string key);
    }
}
