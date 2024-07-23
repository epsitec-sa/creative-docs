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
    /// The <c>MaskComponent</c> enumeration defines which component to
    /// use in a mask pixmap for masking rendering operations.
    /// </summary>
    public enum MaskComponent
    {
        /// <summary>
        /// Don't use any component.
        /// </summary>
        None = -1,

        /// <summary>
        /// Use the alpha channel for masking.
        /// </summary>
        A = 0,

        /// <summary>
        /// Use the alpha channel for masking.
        /// </summary>
        Alpha = 0,

        /// <summary>
        /// Use the red channel for masking.
        /// </summary>
        R = 1,

        /// <summary>
        /// Use the red channel for masking.
        /// </summary>
        Red = 1,

        /// <summary>
        /// Use the green channel for masking.
        /// </summary>
        G = 2,

        /// <summary>
        /// Use the green channel for masking.
        /// </summary>
        Green = 2,

        /// <summary>
        /// Use the blue channel for masking.
        /// </summary>
        B = 3,

        /// <summary>
        /// Use the blue channel for masking.
        /// </summary>
        Blue = 3
    }
}
