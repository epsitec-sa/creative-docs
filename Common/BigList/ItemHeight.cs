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
    /// The <c>ItemHeight</c> structure stores the height of an item, including the
    /// external margins (if any), as a compact, 32-bit value.
    /// </summary>
    public struct ItemHeight : System.IEquatable<ItemHeight>
    {
        public ItemHeight(int height, int marginBefore, int marginAfter)
        {
            this.height = (ushort)height;
            this.marginBefore = (byte)marginBefore;
            this.marginAfter = (byte)marginAfter;
        }

        public ItemHeight(int height)
        {
            this.height = (ushort)height;
            this.marginBefore = 0;
            this.marginAfter = 0;
        }

        public int Height
        {
            get { return this.height; }
        }

        public int TotalHeight
        {
            get { return this.Height + this.MarginBefore + this.MarginAfter; }
        }

        public int MarginBefore
        {
            get { return this.marginBefore; }
        }

        public int MarginAfter
        {
            get { return this.marginAfter; }
        }

        #region IEquatable<ItemHeight> Members

        public bool Equals(ItemHeight other)
        {
            return this.height == other.height
                && this.marginBefore == other.marginBefore
                && this.marginAfter == other.marginAfter;
        }

        #endregion

        private readonly ushort height;
        private readonly byte marginBefore;
        private readonly byte marginAfter;
    }
}
