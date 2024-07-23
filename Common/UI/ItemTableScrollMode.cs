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
    /// The <c>ItemTableScrollMode</c> enumeration defines how an <see
    /// cref="ItemTable"/> scrolls its contents.
    /// </summary>
    public enum ItemTableScrollMode
    {
        /// <summary>
        /// Don't display a scroller and don't scroll.
        /// </summary>
        None,

        /// <summary>
        /// Scroll in a linear way; the scroller defines the position in the
        /// surface displayed by the <see cref="ItemTable"/>.
        /// </summary>
        Linear,

        /// <summary>
        /// Scroll item by item; the scroller defines the first visible item
        /// in the <see cref="ItemTable"/>.
        /// </summary>
        ItemBased
    }
}
