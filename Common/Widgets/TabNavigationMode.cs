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
    /// The <c>TabNavigationMode</c> enumeration defines the navigation modes
    /// supported by a widget. This defines how the widget handles the Tab and
    /// cursor keys.
    /// </summary>
    [System.Flags]
    public enum TabNavigationMode
    {
        /// <summary>
        /// Ignore Tab navigation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Accept Tab navigation focus for this widget when Tab is pressed.
        /// </summary>
        ActivateOnTab = 0x00000001,

        /// <summary>
        /// Accept Tab navigation focus for this widget when an horizontal cursor
        /// is pressed.
        /// </summary>
        ActivateOnCursorX = 0x00000002,

        /// <summary>
        /// Accept Tab navigation focus for this widget when a vertical cursor is
        /// pressed.
        /// </summary>
        ActivateOnCursorY = 0x00000004,

        /// <summary>
        /// Accept Tab navigation focus for this widget when any cursor is pressed.
        /// </summary>
        ActivateOnCursor = ActivateOnCursorX + ActivateOnCursorY,

        /// <summary>
        /// Forward Tab navigation focus to the children of this widget.
        /// </summary>
        ForwardToChildren = 0x00010000,

        /// <summary>
        /// Only forward Tab navigation focus to the children of this widget,
        /// but do not accept the focus itself. Should always be combined with
        /// <c>ForwardToChildren</c>.
        /// </summary>
        ForwardOnly = 0x00020000,

        /// <summary>
        /// Skip this widget if it is read only.
        /// </summary>
        SkipIfReadOnly = 0x00040000,

        /// <summary>
        /// Accept Tab navigation focus for this widget and for its children.
        /// </summary>
        ForwardTabActive = ActivateOnTab | ForwardToChildren,

        /// <summary>
        /// Accept Tab navigation focus only for the children of this widget.
        /// </summary>
        ForwardTabPassive = ActivateOnTab | ForwardToChildren | ForwardOnly,
    }
}
