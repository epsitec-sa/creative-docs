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
    /// The <c>StructuredTypeFieldCollection</c> is used only when serializing the
    /// <c>StructuredType</c> contents.
    /// </summary>
    internal class StructuredTypeFieldCollection : ICollection<StructuredTypeField>
    {
        public StructuredTypeFieldCollection(StructuredType owner)
        {
            this.owner = owner;
        }

        #region ICollection<StructuredTypeField> Members

        void ICollection<StructuredTypeField>.Add(StructuredTypeField field)
        {
            this.owner.Fields.Add(field);
        }

        void ICollection<StructuredTypeField>.Clear()
        {
            this.owner.Fields.Clear();
        }

        bool ICollection<StructuredTypeField>.Contains(StructuredTypeField item)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        void ICollection<StructuredTypeField>.CopyTo(StructuredTypeField[] array, int arrayIndex)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        int ICollection<StructuredTypeField>.Count
        {
            get { return this.owner.Fields.Count; }
        }

        bool ICollection<StructuredTypeField>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<StructuredTypeField>.Remove(StructuredTypeField item)
        {
            throw new System.Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<StructuredTypeField> Members

        IEnumerator<StructuredTypeField> IEnumerable<StructuredTypeField>.GetEnumerator()
        {
            return this.owner.Fields.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.owner.Fields.Values.GetEnumerator();
        }

        #endregion

        private StructuredType owner;
    }
}
