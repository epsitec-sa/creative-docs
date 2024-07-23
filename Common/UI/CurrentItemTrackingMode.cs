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
    /// The <c>CurrentItemTrackingMode</c> enumeration defines how an
    /// <see cref="ItemPanel"/> behaves when the current item of the
    /// <see cref="Epsitec.Common.Types.ICollectionView"/> changes.
    /// </summary>
    public enum CurrentItemTrackingMode
    {
        /// <summary>
        /// Don't track the current item.
        /// </summary>
        None,

        /// <summary>
        /// Automatically synchronize the focus with the current item.
        /// </summary>
        AutoFocus,

        /// <summary>
        /// Automatically select the current item.
        /// </summary>
        AutoSelect,

        /// <summary>
        /// Automatically select the current item; when no current item is defined,
        /// automatically deselect all items.
        /// </summary>
        AutoSelectAndDeselect,
    }
}
