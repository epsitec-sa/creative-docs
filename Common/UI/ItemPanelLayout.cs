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


namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>ItemPanelLayout</c> enumeration lists all supported layout modes
    /// for the items represented by an <see cref="ItemPanel"/>.
    /// </summary>
    public enum ItemPanelLayout : byte
    {
        None,

        /// <summary>
        /// The <c>VerticalList</c> represents the items in a similar way to the
        /// detailed view of Microsoft Windows Explorer.
        /// </summary>
        VerticalList,

        /// <summary>
        /// The <c>RowOfTiles</c> represents each item with a rectangular tile;
        /// the tiles are arranged in rows.
        /// </summary>
        RowsOfTiles,

        /// <summary>
        /// The <c>ColumnOfTiles</c> represents each item with a rectangluar tile;
        /// the tiles are arranges in columns.
        /// </summary>
        ColumnsOfTiles,
    }
}
