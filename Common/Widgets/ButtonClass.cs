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
    /// The <c>ButtonClass</c> enumeration specifies how a command button
    /// should be rendered, based on its class.
    /// </summary>
    [Types.DesignerVisible]
    public enum ButtonClass
    {
        /// <summary>
        /// No associated button class.
        /// </summary>
        [Types.Hidden]
        None,

        /// <summary>
        /// Plain text button for a dialog, used to implement "OK" and
        /// "Cancel" buttons, for instance.
        /// </summary>
        DialogButton,

        /// <summary>
        /// Rich text button for a dialog, used to implement standard
        /// buttons, including an optional icon.
        /// </summary>
        RichDialogButton,

        /// <summary>
        /// Flat button for use in the ribbon, in tool palettes, etc.
        /// The content (icon and/or text) is managed by the button itself.
        /// </summary>
        FlatButton,
    }
}
