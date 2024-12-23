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


using System.Collections;
using System.Collections.Generic;
using Epsitec.Common.Types.Collections;

namespace Epsitec.Common.Support.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(
            this Dictionary<TKey, TValue> set,
            IEnumerable<KeyValuePair<TKey, TValue>> collection
        )
        {
            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                set.Add(item.Key, item.Value);
            }
        }

        public static void AddRange<TKey, TValue>(
            this Dictionary<TKey, TValue> set,
            IEnumerable<System.Tuple<TKey, TValue>> collection
        )
        {
            foreach (System.Tuple<TKey, TValue> item in collection)
            {
                set.Add(item.Item1, item.Item2);
            }
        }

        public static void AddIfValueNotNull<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            TKey key,
            TValue value
        )
            where TValue : class
        {
            if (value != null)
            {
                dictionary.Add(key, value);
            }
        }

        /// <summary>
        /// Returns a read only dictionary that contains the elements of the given
        /// <see cref="IDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <remarks>
        /// This LINQ method will execute immediately and has a complexity of O(1).
        /// </remarks>
        /// <typeparam name="TKey">The type of the key elements.</typeparam>
        /// <typeparam name="TValue">The type of the value elements.</typeparam>
        /// <param name="sequence">The dictionary of which to obtain a read only instance.>/param>
        /// <returns>The new readonly dictionary.</returns>
        /// <exception cref="System.ArgumentNullException">If <paramref name="dictionary"/> is <c>null</c>.</exception>
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnlyDictionary<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary
        )
        {
            dictionary.ThrowIfNull("dictionary");

            return new ReadOnlyDictionary<TKey, TValue>(dictionary);
        }

        /// <summary>
        /// Converts a Dictionary to a sequence of DictionaryEntry.
        /// </summary>
        /// <remarks>
        /// This method is usefull only if you want to enumerate a generic Dictionary{TKey,TValue}
        /// into a non generic sequence of DictionaryEntry. If you call any Linq method or use a
        /// foreach on a generic Dictionary, you will enumerate KeyValuePair{TKey,TValue} and you
        /// can't use them if you don't know at compile time the values of TKey and TValue. This
        /// method lets you do this. Fore more information, you can have a look at the following:
        /// http://stackoverflow.com/questions/7683294
        /// http://stackoverflow.com/questions/9713311
        /// </remarks>
        /// <param name="dictionary">The dictionary that will be converted to a sequence of DictionaryEntry</param>
        /// <returns>The sequence of DictionaryEntry</returns>
        public static IEnumerable<DictionaryEntry> AsEntries(this IDictionary dictionary)
        {
            dictionary.ThrowIfNull("dictionary");

            // Here we use an helper method so that the argument check is immediate but the
            // execution of the body is deferred.
            // Marc

            return DictionaryExtensions.AsEntriesInternal(dictionary);
        }

        private static IEnumerable<DictionaryEntry> AsEntriesInternal(IDictionary dictionary)
        {
            foreach (DictionaryEntry entry in dictionary)
            {
                yield return entry;
            }
        }

        public static List<V> GetOrCreateList<K, V>(this Dictionary<K, List<V>> dic, K key)
        {
            List<V> list;
            if (!dic.TryGetValue(key, out list))
            {
                list = new List<V>();
                dic[key] = list;
            }
            return list;
        }

        public static V GetValueOrDefault<K, V>(
            this Dictionary<K, V> dictionary,
            K key,
            V defaultValue
        )
            where V : class
        {
            V value;
            if (dictionary.TryGetValue(key, out value))
            {
                return value ?? defaultValue;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
