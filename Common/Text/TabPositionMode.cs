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
    /// L'énumération TabPositionMode définit comment la position d'un tabulateur
    /// est définie.
    /// </summary>
    public enum TabPositionMode
    {
        Absolute = 0, //	position absolue
        AbsoluteIndent = 1, //	position absolue, indente toutes les lignes

        LeftRelative = 2, //	position relative à la marge de gauche
        LeftRelativeIndent = 3, //	position relative, indente toutes les lignes

        Force = 4, //	position absolue (sans changement de ligne)
        ForceIndent = 5, //	position absolue, indente toutes les lignes
    }
}
