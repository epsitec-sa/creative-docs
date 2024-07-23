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
    /// L'interface IFeel donne accès aux fonctions propres au "feel" de l'interface
    /// graphique (IAdorner gère le "look" du "look & feel", IFeel the "feel").
    /// </summary>
    public interface IFeel
    {
        bool TestAcceptKey(Message message); //	touche RETURN
        bool TestSelectItemKey(Message message); //	touche SPACE ou RETURN
        bool TestPressButtonKey(Message message); //	touche SPACE
        bool TestCancelKey(Message message); //	touche ESCAPE
        bool TestNavigationKey(Message message); //	touche de navigation dans l'interface
        bool TestComboOpenKey(Message message); //	touche Arrow Up ou touche Arrow Down

        Shortcut AcceptShortcut { get; }
        Shortcut CancelShortcut { get; }
    }
}
