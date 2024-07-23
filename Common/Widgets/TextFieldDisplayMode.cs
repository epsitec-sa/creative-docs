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
    /// The <c>TextFieldDisplayMode</c> enumeration defines how text from
    /// text fields should be displayed.
    /// </summary>
    public enum TextFieldDisplayMode
    {
        /// <summary>
        /// Displays the text in its default representation.
        /// </summary>
        Default,

        /// <summary>
        /// Displays the text with a blue background to show that it was typed
        /// in by the user and that the value overrides a default value.
        /// </summary>
        OverriddenValue,

        /// <summary>
        /// Displays an italic text with an orange background to show that this
        /// is an inherited value (e.g. defined in a style).
        /// </summary>
        InheritedValue,

        /// <summary>
        /// Displays a hint instead of the text typed in by the user. The real
        /// text must be part of the hint and will be made somehow more visible.
        /// </summary>
        ActiveHint,

        /// <summary>
        /// Displays a hint instead of the text typed in by the user. Visually,
        /// there will be no difference between the standard text and the hint.
        /// </summary>
        PassiveHint,

        /// <summary>
        /// Displays the text with a background defined by the property BackColor.
        /// </summary>
        UseBackColor,

        /// <summary>
        /// ActiveHint and UseBackColor.
        /// </summary>
        ActiveHintAndUseBackColor,

        Transparent,
    }
}
