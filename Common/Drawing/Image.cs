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


namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// La classe Image permet de représenter une image générique.
    /// </summary>
    public abstract class Image : System.IDisposable
    {
        public abstract Size Size { get; }

        public string DebugTag
        {
            get => this.debugTag;
            set
            {
                if (this.debugTag == null)
                {
                    this.debugTag = value;
                }
                else
                {
                    throw new System.InvalidOperationException("DebugTag already set");
                }
            }
        }

        public double Width
        {
            get { return this.Size.Width; }
        }

        public double Height
        {
            get { return this.Size.Height; }
        }
        public bool IsEmpty
        {
            get { return this.Size.IsEmpty; }
        }

        public abstract Bitmap BitmapImage { get; }

        public abstract void DefineZoom(double zoom);

        public abstract void DefineColor(Drawing.Color color);

        public abstract void DefineAdorner(object adorner);

        public virtual bool IsPaintStyleDefined(GlyphPaintStyle style)
        {
            return this.GetImageForPaintStyle(style) != null;
        }

        public virtual Image GetImageForPaintStyle(GlyphPaintStyle style)
        {
            if (style == GlyphPaintStyle.Normal)
            {
                return this;
            }

            return null;
        }

        #region IDisposable Members
        public abstract void Dispose();
        #endregion

        private string debugTag;
    }
}
