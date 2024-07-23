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
    /// The <c>IItemViewFactory</c> interface can be used to initialize <see cref="ItemView"/>
    /// instances based on the item which should be represented.
    /// </summary>
    public interface IItemViewFactory
    {
        /// <summary>
        /// Creates the user interface for the specified item view.
        /// </summary>
        /// <param name="itemView">The item view.</param>
        /// <returns>The widget which represents the data stored in the item view.</returns>
        ItemViewWidget CreateUserInterface(ItemView itemView);

        /// <summary>
        /// Disposes the user interface created by <c>CreateUserInterface</c>.
        /// </summary>
        /// <param name="widget">The widget to dispose.</param>
        void DisposeUserInterface(ItemViewWidget widget);

        /// <summary>
        /// Gets the preferred size of the user interface associated with the
        /// specified item view.
        /// </summary>
        /// <param name="itemView">The item view.</param>
        /// <returns>The preferred size.</returns>
        Drawing.Size GetPreferredSize(ItemView itemView);
    }
}
