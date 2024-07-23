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

namespace Epsitec.Common.Types
{
    /// <summary>
    /// The generic DependencyObjectList class implements a generic List which
    /// provides also an ICollation on DependencyObject.
    /// </summary>
    public class DependencyObjectList<T> : List<T>, ICollection<DependencyObject>
        where T : DependencyObject
    {
        #region ICollection<DependencyObject> Members

        void ICollection<DependencyObject>.Add(DependencyObject item)
        {
            this.Add(item as T);
        }

        void ICollection<DependencyObject>.Clear()
        {
            this.Clear();
        }

        bool ICollection<DependencyObject>.Contains(DependencyObject item)
        {
            return this.Contains(item as T);
        }

        void ICollection<DependencyObject>.CopyTo(DependencyObject[] array, int arrayIndex)
        {
            this.ToArray().CopyTo(array, arrayIndex);
        }

        int ICollection<DependencyObject>.Count
        {
            get { return this.Count; }
        }

        bool ICollection<DependencyObject>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<DependencyObject>.Remove(DependencyObject item)
        {
            return this.Remove(item as T);
        }

        #endregion

        #region IEnumerable<DependencyObject> Members

        IEnumerator<DependencyObject> IEnumerable<DependencyObject>.GetEnumerator()
        {
            foreach (T item in this)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (T item in this)
            {
                yield return item;
            }
        }

        #endregion
    }
}
