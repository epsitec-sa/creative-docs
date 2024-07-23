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
    /// The <c>TextBreakRun</c> structure defines a text run (a series of
    /// characters which share the same font and scale).
    /// </summary>
    public struct TextBreakRun
    {
        public TextBreakRun(Font font, int length, double scale, string locale)
        {
            this.font = font;
            this.length = length;
            this.scale = scale;
            this.locale = locale;
        }

        public Font Font
        {
            get { return this.font; }
        }

        public int Length
        {
            get { return this.length; }
        }

        public double Scale
        {
            get { return this.scale; }
        }

        public string Locale
        {
            get { return this.locale; }
        }

        private Font font;
        private int length;
        private double scale;
        private string locale;
    }
}
