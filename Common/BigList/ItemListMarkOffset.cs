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
    /// The <c>ItemListMarkOffset</c> structure describes the offset of an <see cref="ItemListMark"/>
    /// relative to the <see cref="ItemList"/>.
    /// </summary>
    public struct ItemListMarkOffset
    {
        public ItemListMarkOffset(int offset)
        {
            this.offset = offset ^ ItemListMarkOffset.Valid;
        }

        public bool IsVisible
        {
            get
            {
                if (this.offset == 0)
                {
                    return false;
                }

                return (this.Offset > System.Int32.MinValue)
                    && (this.Offset < System.Int32.MaxValue);
            }
        }

        public bool IsBefore
        {
            get { return this.Offset == System.Int32.MinValue; }
        }

        public bool IsAfter
        {
            get { return this.Offset == System.Int32.MaxValue; }
        }

        public bool IsEmpty
        {
            get { return this.offset == 0; }
        }

        public int Offset
        {
            get
            {
                if (this.IsEmpty)
                {
                    return 0;
                }

                return this.offset ^ ItemListMarkOffset.Valid;
            }
        }

        public static readonly ItemListMarkOffset Empty = new ItemListMarkOffset();
        public static readonly ItemListMarkOffset Before = new ItemListMarkOffset(
            System.Int32.MinValue
        );
        public static readonly ItemListMarkOffset After = new ItemListMarkOffset(
            System.Int32.MaxValue
        );

        public override string ToString()
        {
            if (this.IsBefore)
            {
                return "<";
            }
            if (this.IsAfter)
            {
                return ">";
            }
            if (this.IsEmpty)
            {
                return "*";
            }

            return string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}",
                this.offset
            );
        }

        private const int Valid = 0x10000000;

        private readonly int offset;
    }
}
