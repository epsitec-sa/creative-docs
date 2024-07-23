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


using System.Linq;

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The <c>EnumKeyValue{T}</c> class is used to store an <c>enum</c> key
    /// and associated texts which represent its value.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    public sealed class EnumKeyValues<T> : EnumKeyValues
    {
        public EnumKeyValues(T key, EnumValue item, params string[] values)
        {
            this.key = key;
            this.enumValue = item;
            this.values = values
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new FormattedText(x))
                .ToArray();
        }

        public EnumKeyValues(T key, EnumValue item, params FormattedText[] values)
        {
            this.key = key;
            this.enumValue = item;
            this.values = values.Where(x => !x.IsNullOrEmpty()).ToArray();
        }

        /// <summary>
        /// Gets the key which is an <c>enum</c> value.
        /// </summary>
        /// <value>The key.</value>
        public T Key
        {
            get { return this.key; }
        }

        /// <summary>
        /// Gets the values for the key.
        /// </summary>
        /// <value>The values.</value>
        public override FormattedText[] Values
        {
            get { return this.values; }
        }

        /// <summary>
        /// Gets the enum value description (if any) for this value.
        /// </summary>
        public override EnumValue EnumValue
        {
            get { return this.enumValue; }
        }

        private readonly T key;
        private readonly FormattedText[] values;
        private readonly EnumValue enumValue;
    }
}
