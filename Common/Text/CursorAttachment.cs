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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// L'énumération CursorAttachment définit comment un curseur est attaché
    /// au texte sous-jacent.
    /// </summary>
    public enum CursorAttachment : byte
    {
        Floating = 0, //	flottant (en cas de destruction du texte, ajuste

        //	..simplement la position du curseur à l'extrémité
        //	..de la zone détruite)

        ToNext = 1, //	attaché au caractère suivant
        ToPrevious = 2, //	attaché au caractère précédent

        Temporary = 10, //	comme Floating, mais à ignorer par les undo/redo
    }
}
