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
    [System.Flags]
    public enum TextBreakMode : ushort
    {
        None = 0x0000,
        Undefined = 0x8000,

        Hyphenate = 0x0001, //	césure des mots, si possible
        Ellipsis = 0x0002, //	ajoute une ellipse (...) si le dernier mot est tronqué
        Overhang = 0x0004, //	permet de dépasser la largeur si on ne peut pas faire autrement
        Split = 0x0008, //	coupe brutalement si on ne peut pas faire autrement

        SingleLine = 0x0100, //	force tout sur une ligne (utile avec Ellipsis, Overhang et Split)
    }
}
