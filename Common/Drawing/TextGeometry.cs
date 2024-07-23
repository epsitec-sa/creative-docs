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

namespace Epsitec.Common.Drawing
{
    public struct TextGeometry
    {
        public TextGeometry(Point origin, double ascender, double descender, double textWidth)
        {
            this.origin = origin;
            this.ascender = ascender;
            this.descender = descender;
            this.textWidth = textWidth;
        }

        public TextGeometry(
            Rectangle bounds,
            string text,
            Font font,
            double size,
            ContentAlignment textAlignment
        )
            : this(bounds.X, bounds.Y, bounds.Width, bounds.Height, text, font, size, textAlignment)
        { }

        public TextGeometry(
            double x,
            double y,
            double width,
            double height,
            string text,
            Font font,
            double size,
            ContentAlignment textAlignment,
            Graphics graphics = null,
            GridSnapping gridSnapping = GridSnapping.None
        )
        {
            double textWidth = font.GetTextAdvance(text) * size;
            double textHeight = (font.Ascender - font.Descender) * size;

            switch (textAlignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.BottomRight:
                    y = y - font.Descender * size;
                    break;

                case ContentAlignment.MiddleLeft:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.MiddleRight:
                    y = y + (height - textHeight) / 2 - font.Descender * size;
                    break;

                case ContentAlignment.TopLeft:
                case ContentAlignment.TopCenter:
                case ContentAlignment.TopRight:
                    y = y + height - textHeight - font.Descender * size;
                    break;

                case ContentAlignment.BaselineLeft:
                case ContentAlignment.BaselineCenter:
                case ContentAlignment.BaselineRight:
                    break;

                case ContentAlignment.None:
                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", textAlignment.GetQualifiedName())
                    );
            }

            switch (textAlignment)
            {
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                case ContentAlignment.TopLeft:
                case ContentAlignment.BaselineLeft:
                    break;

                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.TopCenter:
                case ContentAlignment.BaselineCenter:
                    x = x + (width - textWidth) / 2;
                    break;

                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                case ContentAlignment.TopRight:
                case ContentAlignment.BaselineRight:
                    x = x + width - textWidth;
                    break;

                case ContentAlignment.None:
                default:
                    throw new System.NotSupportedException(
                        string.Format("{0} not supported", textAlignment.GetQualifiedName())
                    );
            }

            if (graphics == null)
            {
                this.origin = new Point(x, y);
            }
            else
            {
                this.origin = graphics.Align(new Point(x, y), gridSnapping);
            }

            this.ascender = font.Ascender * size;
            this.descender = font.Descender * size;
            this.textWidth = textWidth;
        }

        public Point Origin
        {
            get { return this.origin; }
        }

        public double Ascender
        {
            get { return this.ascender; }
        }

        public double Descender
        {
            get { return this.descender; }
        }

        public double Width
        {
            get { return this.textWidth; }
        }

        private readonly Point origin;
        private readonly double ascender;
        private readonly double descender;
        private readonly double textWidth;
    }
}
