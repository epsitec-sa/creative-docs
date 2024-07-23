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
    /// <summary>
    /// L'énumération MouseButtons définit les boutons de la souris connus.
    /// Plusieurs boutons peuvent être combinés.
    /// </summary>
    [System.Flags]
    public enum MouseButtons
    {
        None = 0,

        Left = 0x00100000,
        Right = 0x00200000,
        Middle = 0x00400000,
        XButton1 = 0x00800000,
        XButton2 = 0x01000000
    }
}
