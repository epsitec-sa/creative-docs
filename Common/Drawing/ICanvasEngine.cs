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
    /// L'interface ICanvasEngine définit un moteur capable de peindre dans
    /// une surface de type Bitmap pour remplir un Canvas à une taille donnée.
    /// </summary>
    public interface ICanvasEngine
    {
        Canvas.IconKey[] IconKeys { get; }

        Size Size { get; }

        Point Origin { get; }

        void Paint(
            Graphics graphics,
            Size size,
            GlyphPaintStyle style,
            Color color,
            int page,
            object adorner
        );
    }
}
