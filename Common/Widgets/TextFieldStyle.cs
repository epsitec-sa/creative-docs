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
    /// The <c>TextFieldStyle</c> enumeration defines how a text field looks like
    /// and how it behaves.
    /// </summary>
    public enum TextFieldStyle
    {
        /// <summary>
        /// Normal text field. This is the standard, default, text line.
        /// </summary>
        Normal,

        /// <summary>
        /// Flat text field, without any frame nor margins.
        /// </summary>
        Flat,

        /// <summary>
        /// Multiline text field.
        /// </summary>
        Multiline,

        /// <summary>
        /// Combo box with a drop-down list.
        /// </summary>
        Combo,

        /// <summary>
        /// Text field with up/down buttons to increment/decrement a value.
        /// </summary>
        UpDown,

        /// <summary>
        /// Simple text field with a thin frame, just like a menu item. This
        /// is mainly used for text painted by <see cref="ScrollList"/>.
        /// </summary>
        Simple,
    }
}
