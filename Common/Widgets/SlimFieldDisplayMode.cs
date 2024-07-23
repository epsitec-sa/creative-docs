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
    /// The <c>SlimFieldDisplayMode</c> enumeration defines how a <see cref="SlimField"/>
    /// displays its contents.
    /// </summary>
    public enum SlimFieldDisplayMode
    {
        /// <summary>
        /// Display the label. This is used when the field is empty and its meaning cannot
        /// be inferred by the context.
        /// </summary>
        Label,

        /// <summary>
        /// Display the label when in edition mode (similar to <see cref="Label"/>).
        /// </summary>
        LabelEdition,

        /// <summary>
        /// Display the text value. A prefix and a suffix might be included with the text value
        /// to make the data more meaningful.
        /// </summary>
        Text,

        /// <summary>
        /// Display the text value when in edition mode (similar to <see cref="Text"/>).
        /// </summary>
        TextEdition,

        /// <summary>
        /// Display the menu. This is used to let the user select one of the values.
        /// </summary>
        Menu,

        /// <summary>
        /// Not a display mode: measure the width of the text value.
        /// </summary>
        MeasureTextOnly,

        /// <summary>
        /// Not a display mode: measure the width of the text prefix.
        /// </summary>
        MeasureTextPrefix,

        /// <summary>
        /// Not a display mode: measure the width of the text suffix.
        /// </summary>
        MeasureTextSuffix,
    }
}
