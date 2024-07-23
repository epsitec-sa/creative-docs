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
    /// The <c>FontClassInfo</c> structure stores width and stretchability
    /// information about a range of glyphs.
    /// </summary>
    public struct FontClassInfo
    {
        public FontClassInfo(GlyphClass id, int count, double width, double elasticity)
        {
            this.classId = id;
            this.count = count;
            this.width = width;
            this.elasticity = elasticity;
            this.scale = 1.0;
        }

        public GlyphClass GlyphClass
        {
            get { return this.classId; }
        }

        public int Count
        {
            get { return this.count; }
        }

        public double Width
        {
            get { return this.width; }
        }

        public double Elasticity
        {
            get { return this.elasticity; }
        }

        public double Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        private GlyphClass classId;
        private int count; //	number of glyphs belonging to this class
        private double width; //	accumulated glyph width
        private double elasticity; //	glyph elasticity
        private double scale; //	horizontal glyph scale
    }
}
