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
using System.Linq;

namespace Epsitec.Common.Types.Collections
{
    /// <summary>
    /// The <c>EmptyList&lt;T&gt;</c> class implements the <c>IList&lt;T&gt;</c> and
    /// <c>IList</c> interfaces for an empty read-only list.
    /// </summary>
    /// <typeparam name="T">Item type</typeparam>
    public sealed class EmptyList<T> : IList<T>, System.Collections.IList
    {
        private EmptyList() { }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return -1;
        }

        public void Insert(int index, T item)
        {
            throw new System.InvalidOperationException();
        }

        public void RemoveAt(int index)
        {
            throw new System.InvalidOperationException();
        }

        public T this[int index]
        {
            get { throw new System.ArgumentOutOfRangeException("index"); }
            set { throw new System.InvalidOperationException(); }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            throw new System.InvalidOperationException();
        }

        public void Clear()
        {
            throw new System.InvalidOperationException();
        }

        public bool Contains(T item)
        {
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex) { }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(T item)
        {
            throw new System.InvalidOperationException();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Enumerable.Empty<T>().GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new System.InvalidOperationException();
        }

        public bool Contains(object value)
        {
            return false;
        }

        public int IndexOf(object value)
        {
            return -1;
        }

        public void Insert(int index, object value)
        {
            throw new System.InvalidOperationException();
        }

        public bool IsFixedSize
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new System.InvalidOperationException();
        }

        object System.Collections.IList.this[int index]
        {
            get { throw new System.ArgumentOutOfRangeException("index"); }
            set { throw new System.InvalidOperationException(); }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(System.Array array, int index) { }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        #endregion

        public static readonly EmptyList<T> Instance = new EmptyList<T>();
    }
}
