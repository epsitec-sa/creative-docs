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
    public enum WidgetInternalState : uint
    {
        None = 0,

        Disposing = 0x00000001,
        Disposed = 0x00000002,

        WasValid = 0x00000004,

        Embedded = 0x00000008, //	=> widget appartient au parent (widgets composés)

        Focusable = 0x00000010,
        Selectable = 0x00000020,
        Engageable = 0x00000040, //	=> peut être enfoncé par une pression
        Frozen = 0x00000080, //	=> n'accepte aucun événement

        ExecCmdOnPressed = 0x00001000, //	=> exécute la commande quand on presse le widget

        AutoMnemonic = 0x00100000,
        AutoFitWidth = 0x00200000,

        PossibleContainer = 0x01000000, //	widget peut être la cible d'un drag & drop en mode édition
        EditionEnabled = 0x02000000, //	widget peut être édité
        Fence = 0x04000000, //	widget marqué comme frontière (usages multiples)

        DebugActive = 0x80000000 //	widget marqué pour le debug
    }
}
