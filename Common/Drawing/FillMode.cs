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
    /// The <c>FillMode</c> enumeration specifies how intersecting surfaces
    /// are filled.
    /// </summary>
    public enum FillMode
    {
        /// <summary>
        /// Use the even/odd fill rule (for a given point, if moving in a straight
        /// line to infinity, we cross the path an odd number of times, then the
        /// point will be considered as inside the surface).
        /// </summary>
        EvenOdd = 1,

        /// <summary>
        /// Use the non zero winding rule (a hole must be drawn in the opposite
        /// order as the surrounding path).
        /// </summary>
        NonZero = 2
    }
}
