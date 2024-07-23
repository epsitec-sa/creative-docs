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


using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Epsitec.Common.Support.Extensions
{
    public static class ConcurrentDictionaryExtensions
    {
        /// <summary>
        /// Removes the specified key from the concurrent dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The concurrent dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the key could be removed; otherwise, <c>false</c>.</returns>
        public static bool Remove<TKey, TValue>(
            this ConcurrentDictionary<TKey, TValue> dict,
            TKey key
        )
        {
            TValue value;

            return dict.TryRemove(key, out value);
        }
    }
}
