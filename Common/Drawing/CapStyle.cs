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
    /// The <c>CapStyle</c> enumeration defines the style used to paint the
    /// line caps (i.e. the extremities of an open path).
    /// </summary>
    public enum CapStyle
    {
        /// <summary>
        /// Butt cap, the line starts and stops at the exact coordinates.
        /// </summary>
        Butt = 0,

        /// <summary>
        /// Square cap, the line extends by half its width the start and stop
        /// coordinates.
        /// </summary>
        Square = 1,

        /// <summary>
        /// Round cap, the line starts and stops with a rounded extremity, with
        /// a radius equal to the half of its width.
        /// </summary>
        Round = 2
    }
}
