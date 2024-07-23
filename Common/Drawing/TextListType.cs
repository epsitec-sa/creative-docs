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


namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// The <c>TextListType</c> enumeration defines the type of a text list,
    /// either a glyph based list or a numbered list.
    /// </summary>
    public enum TextListType
    {
        /// <summary>
        /// The text does not belong to a text list.
        /// </summary>
        None,

        /// <summary>
        /// The text belongs to a fixed bullet list, defined using
        /// <see cref="TextListGlyph"/>.
        /// </summary>
        Fixed,

        /// <summary>
        /// The text belongs to a numbered list.
        /// </summary>
        Numbered,
    }
}
