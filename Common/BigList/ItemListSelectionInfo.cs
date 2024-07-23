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


namespace Epsitec.Common.BigList
{
    /// <summary>
    /// The <c>ItemListSelectionInfo</c> structure stores a pair of index and selection state
    /// for items where the selection changed. It records usually the current state.
    /// </summary>
    public struct ItemListSelectionInfo : System.IEquatable<ItemListSelectionInfo>
    {
        public ItemListSelectionInfo(int index, bool isSelected)
        {
            this.index = index;
            this.isSelected = isSelected;
        }

        public int Index
        {
            get { return this.index; }
        }

        public bool IsSelected
        {
            get { return this.isSelected; }
        }

        private readonly int index;
        private readonly bool isSelected;

        #region IEquatable<ItemListSelectionInfo> Members

        public bool Equals(ItemListSelectionInfo other)
        {
            return this.index == other.index && this.isSelected == other.isSelected;
        }

        #endregion


        public static bool operator ==(ItemListSelectionInfo a, ItemListSelectionInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ItemListSelectionInfo a, ItemListSelectionInfo b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            if (obj is ItemListSelectionInfo)
            {
                return this.Equals((ItemListSelectionInfo)obj);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.index ^ (this.isSelected ? 0x10000000 : 0x00000000);
        }
    }
}
