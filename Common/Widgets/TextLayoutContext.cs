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


namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// The <c>TextLayoutContext</c> class stores cursor position and selection
    /// information for a given <see cref="TextLayout"/> object.
    /// </summary>
    public class TextLayoutContext
    {
        public TextLayoutContext(TextLayout textLayout)
        {
            this.textLayout = textLayout;
            this.PrepareOffset = -1;
            this.MaxLength = 1000;
        }

        public TextLayoutContext(TextLayoutContext context)
        {
            this.textLayout = context.textLayout;
            context.CopyTo(this);
        }

        public void CopyTo(TextLayoutContext context)
        {
            context.cursorFrom = this.cursorFrom;
            context.cursorTo = this.cursorTo;
            context.CursorAfter = this.CursorAfter;
            context.CursorLine = this.CursorLine;
            context.CursorPosX = this.CursorPosX;
            context.PrepareOffset = this.PrepareOffset;
            context.PrepareLength1 = this.PrepareLength1;
            context.PrepareLength2 = this.PrepareLength2;
            context.MaxLength = this.MaxLength;
            context.UndoSeparator = this.UndoSeparator;
        }

        public int CursorFrom
        {
            get { return this.cursorFrom; }
            set
            {
                this.cursorFrom = value;
                this.UndoSeparator = true;

                if (this.PrepareOffset != -1)
                {
                    this.textLayout.Simplify(this);
                }
            }
        }

        public int CursorTo
        {
            get { return this.cursorTo; }
            set
            {
                this.cursorTo = value;
                this.UndoSeparator = true;

                if (this.PrepareOffset != -1)
                {
                    textLayout.Simplify(this);
                }
            }
        }

        public bool HasSelection
        {
            get { return this.cursorFrom != this.cursorTo; }
        }

        public bool CursorAfter { get; set; }

        public int CursorLine { get; set; }

        public double CursorPosX { get; set; }

        public int PrepareOffset { get; set; }

        public int PrepareLength1 { get; set; }

        public int PrepareLength2 { get; set; }

        public int MaxLength { get; set; }

        public bool UndoSeparator { get; set; }

        public void OffsetCursor(int offset)
        {
            this.cursorFrom += offset;
            this.cursorTo += offset;
        }

        private readonly TextLayout textLayout;
        private int cursorFrom;
        private int cursorTo;
    }
}
