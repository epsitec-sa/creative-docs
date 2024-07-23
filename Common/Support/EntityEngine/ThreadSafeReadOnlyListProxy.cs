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
using Epsitec.Common.Types.Exceptions;

namespace Epsitec.Common.Support.EntityEngine
{
    /// <summary>
    /// The <see cref="ThreadSafeReadOnlyListProxy"/> is an implementation of <see cref="IList"/>
    /// associated to an entity which will wrap calls to an underlying implementation of list. It
    /// will throw an exception when an attempt to modify the collection is done and it will acquire
    /// the global lock associated with the entity for any read operation on the list.
    /// </summary>
    internal sealed class ThreadSafeReadOnlyListProxy : IList
    {
        public ThreadSafeReadOnlyListProxy(AbstractEntity entity, IList list)
        {
            this.entity = entity;
            this.list = list;
        }

        #region IList Members


        public int Add(object value)
        {
            throw new ReadOnlyException();
        }

        public void Clear()
        {
            throw new ReadOnlyException();
        }

        public bool Contains(object value)
        {
            using (this.entity.LockWrite())
            {
                return this.list.Contains(value);
            }
        }

        public int IndexOf(object value)
        {
            using (this.entity.LockWrite())
            {
                return this.list.IndexOf(value);
            }
        }

        public void Insert(int index, object value)
        {
            throw new ReadOnlyException();
        }

        public bool IsFixedSize
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list.IsFixedSize;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list.IsReadOnly;
                }
            }
        }

        public void Remove(object value)
        {
            throw new ReadOnlyException();
        }

        public void RemoveAt(int index)
        {
            throw new ReadOnlyException();
        }

        public object this[int index]
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list[index];
                }
            }
            set { throw new ReadOnlyException(); }
        }

        #endregion


        #region ICollection Members

        public void CopyTo(System.Array array, int index)
        {
            using (this.entity.LockWrite())
            {
                this.list.CopyTo(array, index);
            }
        }

        public int Count
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list.Count;
                }
            }
        }

        public bool IsSynchronized
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list.IsSynchronized;
                }
            }
        }

        public object SyncRoot
        {
            get
            {
                using (this.entity.LockWrite())
                {
                    return this.list.SyncRoot;
                }
            }
        }

        #endregion


        #region IEnumerable Members


        public IEnumerator GetEnumerator()
        {
            using (this.entity.LockWrite())
            {
                return new ArrayList(this.list).GetEnumerator();
            }
        }

        #endregion


        private readonly AbstractEntity entity;

        private readonly IList list;
    }
}
