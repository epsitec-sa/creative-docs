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
    /// The <c>ContentAlignment</c> enumeration defines how content should be
    /// aligned in a 2 dimension space.
    /// </summary>
    public enum ContentAlignment : byte
    {
        /// <summary>
        /// No alignment.
        /// </summary>
        None,

        /// <summary>
        /// Bottom left alignment.
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Bottom center alignment.
        /// </summary>
        BottomCenter,

        /// <summary>
        /// Bottom right alignment.
        /// </summary>
        BottomRight,

        /// <summary>
        /// Middle left alignment.
        /// </summary>
        MiddleLeft,

        /// <summary>
        /// Middle center alignment.
        /// </summary>
        MiddleCenter,

        /// <summary>
        /// Middle right alignment.
        /// </summary>
        MiddleRight,

        /// <summary>
        /// Top left alignment.
        /// </summary>
        TopLeft,

        /// <summary>
        /// Top center alignment.
        /// </summary>
        TopCenter,

        /// <summary>
        /// Top right alignment.
        /// </summary>
        TopRight,

        /// <summary>
        /// Baseline left alignment.
        /// </summary>
        BaselineLeft,

        /// <summary>
        /// Baseline center aliment.
        /// </summary>
        BaselineCenter,

        /// <summary>
        /// Baseline right alignment,
        /// </summary>
        BaselineRight,

        /// <summary>
        /// Undefined alignment.
        /// </summary>
        Undefined = 0xff,
    }
}
