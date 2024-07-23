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
    /// The <c>ButtonShowCondition</c> enumeration defines when buttons will
    /// be shown in a <see cref="TextFieldEx"/> widget.
    /// </summary>
    public enum ButtonShowCondition
    {
        /// <summary>
        /// Always show the accept/reject buttons.
        /// </summary>
        Always,

        /// <summary>
        /// Show the accept/reject buttons when the widget has the focus.
        /// </summary>
        WhenFocused,

        /// <summary>
        /// Show the accept/reject buttons when the widget has the keyboard
        /// focus.
        /// </summary>
        WhenKeyboardFocused,

        /// <summary>
        /// Show the accept/reject buttons when the widget contains modified
        /// data.
        /// </summary>
        WhenModified,

        /// <summary>
        /// Never show the accept/reject buttons.
        /// </summary>
        Never
    }
}
