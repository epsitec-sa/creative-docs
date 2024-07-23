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
    /// The <c>IRenderer</c> interface is common to all renderers.
    /// </summary>
    public interface IRenderer
    {
        ///// <summary>
        ///// Attaches the pixmap to the renderer. All the rendering will go to
        ///// the specified pixmap. Setting it to <c>null</c> detaches the pixmap.
        ///// </summary>
        ///// <value>The pixmap.</value>
        //DrawingBitmap DrawingBitmap { set; }

        /// <summary>
        /// Sets the alpha mask using the specified 8-bit component of the
        /// mask pixmap. The mask can be reset by specifying a <c>null</c>
        /// pixmap.
        /// </summary>
        /// <param name="maskPixmap">The mask pixmap.</param>
        /// <param name="component">The component.</param>
        void SetAlphaMask(DrawingBitmap maskPixmap, MaskComponent component);
    }
}
