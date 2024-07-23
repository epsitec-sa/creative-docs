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


using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
    /// <summary>
    /// The <c>Bag</c> class is just a dictionary which does not throw exceptions
    /// when it is queried with unknown keys.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public class Bag<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public Bag()
        {
            this.dict = new Dictionary<TKey, TValue>();
        }

        public Bag(IDictionary<TKey, TValue> values)
            : this()
        {
            foreach (var item in values)
            {
                this.Add(item);
            }
        }

        #region IDictionary<TKey,TValue> Members

        public ICollection<TKey> Keys
        {
            get { return this.dict.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return this.dict.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue value;
                if (this.dict.TryGetValue(key, out value))
                {
                    return value;
                }
                else
                {
                    return default(TValue);
                }
            }
            set { this.dict[key] = value; }
        }

        public void Add(TKey key, TValue value)
        {
            this.dict.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return this.dict.Remove(key);
        }

        public bool ContainsKey(TKey key)
        {
            return this.dict.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.dict.TryGetValue(key, out value);
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public int Count
        {
            get { return this.dict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).Add(item);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).Remove(item);
        }

        public void Clear()
        {
            this.dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)this.dict).CopyTo(array, arrayIndex);
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.dict.GetEnumerator();
        }

        #endregion


        private readonly Dictionary<TKey, TValue> dict;
    }
}
