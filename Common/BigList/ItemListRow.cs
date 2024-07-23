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
    public sealed class ItemListRow
    {
        public ItemListRow(int index, int offset, ItemHeight height, bool isLast)
        {
            this.index = index;
            this.offset = offset;
            this.height = height;
            this.isLast = isLast;
        }

        public int Index
        {
            get { return this.index; }
        }

        public int Offset
        {
            get { return this.offset; }
        }

        public ItemHeight Height
        {
            get { return this.height; }
        }

        public bool IsLast
        {
            get { return this.isLast; }
        }

        public bool IsFirst
        {
            get { return this.index == 0; }
        }

        public bool IsEven
        {
            get { return (this.index & 0x01) == 0x00; }
        }

        public bool IsOdd
        {
            get { return (this.index & 0x01) == 0x01; }
        }

        private readonly int index;
        private readonly int offset;
        private readonly ItemHeight height;
        private readonly bool isLast;
    }
}
