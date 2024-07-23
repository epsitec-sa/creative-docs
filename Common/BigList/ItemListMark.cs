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


using Epsitec.Common.Support.Extensions;

namespace Epsitec.Common.BigList
{
    /// <summary>
    /// The <c>ItemListMark</c> class represents a mark in the <see cref="ItemList"/>. The
    /// mark is either attached before or after an item. Its offset is computed by the
    /// <see cref="ItemList.GetOffset"/> method.
    /// </summary>
    public class ItemListMark
    {
        public ItemListMark() { }

        public ItemListMarkAttachment Attachment { get; set; }

        public int Index { get; set; }

        public int Breadth { get; set; }

        public int GetInsertionIndex()
        {
            switch (this.Attachment)
            {
                case ItemListMarkAttachment.Before:
                    return this.Index;
                case ItemListMarkAttachment.After:
                    return this.Index + 1;
            }

            throw this.Attachment.NotSupportedException();
        }

        public void SetInsertionIndex(int index)
        {
            switch (this.Attachment)
            {
                case ItemListMarkAttachment.Before:
                    this.Index = index;
                    return;

                case ItemListMarkAttachment.After:
                    this.Index = index - 1;
                    return;
            }

            throw this.Attachment.NotSupportedException();
        }
    }
}
