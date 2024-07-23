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


namespace Epsitec.Common.Drawing.Platform
{
    internal struct Color32
    {
        public Color32(byte[] array, int offset)
        {
            this.Blue = array[offset + 0];
            this.Green = array[offset + 1];
            this.Red = array[offset + 2];
            this.Alpha = array[offset + 3];
        }

        public byte Blue;
        public byte Green;
        public byte Red;
        public byte Alpha;

        public int ARGB
        {
            get
            {
                return ((((((this.Alpha << 8) | this.Red) << 8) | this.Green) << 8) | this.Blue);
            }
        }
    }
}
