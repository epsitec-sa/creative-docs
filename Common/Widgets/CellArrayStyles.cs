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


namespace Epsitec.Common.Widgets
{
    [System.Flags]
    public enum CellArrayStyles
    {
        None = 0x00000000, // neutre
        ScrollNorm = 0x00000001, // défilement avec un ascenseur
        ScrollMagic = 0x00000002, // défilement aux extrémités
        Stretch = 0x00000004, // occupe toute la place
        Header = 0x00000010, // en-tête
        Mobile = 0x00000020, // dimensions mobiles
        Separator = 0x00000040, // lignes de séparation
        Sort = 0x00000080, // choix pour tri possible
        SelectCell = 0x00000100, // sélectionne une cellule individuelle
        SelectLine = 0x00000200, // sélectionne toute la ligne
        SelectMulti = 0x00000400, // sélections multiples possibles avec Ctrl et Shift
    }
}
