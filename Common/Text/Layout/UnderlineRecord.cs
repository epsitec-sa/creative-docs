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


namespace Epsitec.Common.Text.Layout
{
    /// <summary>
    /// La classe XlineRecord représente un changement de soulignement, biffé,
    /// etc. au sein d'un paragraphe.
    /// </summary>
    public class XlineRecord
    {
        public XlineRecord(
            RecordType recordType,
            int textOffset,
            System.Collections.ICollection xlines,
            Properties.FontColorProperty textColor,
            /**/double x,
            double y,
            double ascender,
            double descender,
            int frameIndex,
            bool isVisible
        )
        {
            int count = xlines == null ? 0 : xlines.Count;

            this.recordType = recordType;
            this.textOffset = textOffset;
            this.xlines = new Properties.AbstractXlineProperty[count];

            if (xlines != null)
            {
                xlines.CopyTo(this.xlines, 0);
            }

            this.textColor = textColor;

            this.x = x;
            this.y = y;

            this.ascender = ascender;
            this.descender = descender;

            this.frameIndex = frameIndex;
            this.isVisible = isVisible;
        }

        public RecordType Type
        {
            get { return this.recordType; }
        }

        public int TextOffset
        {
            get { return this.textOffset; }
        }

        public Properties.AbstractXlineProperty[] Xlines
        {
            get { return this.xlines; }
        }

        public int FrameIndex
        {
            get { return this.frameIndex; }
        }

        public bool IsVisible
        {
            get { return this.isVisible; }
        }

        public double X
        {
            get { return this.x; }
        }

        public double Y
        {
            get { return this.y; }
        }

        public double Ascender
        {
            get { return this.ascender; }
        }

        public double Descender
        {
            get { return this.descender; }
        }

        public Properties.FontColorProperty TextColor
        {
            get { return this.textColor; }
        }

        #region RecordType Enumeration
        public enum RecordType
        {
            Change,

            LineStart,
            LineEnd
        }
        #endregion

        private RecordType recordType;
        private int textOffset;
        private Properties.AbstractXlineProperty[] xlines;
        private int frameIndex;
        private bool isVisible;
        private double x,
            y;
        private double ascender,
            descender;
        private Properties.FontColorProperty textColor;
    }
}
