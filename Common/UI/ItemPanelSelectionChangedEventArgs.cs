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


using Epsitec.Common.Types.Collections;
using System.Collections.Generic;

namespace Epsitec.Common.UI
{
    /// <summary>
    /// The <c>ItemPanelSelectionChanged</c> class contains the list of selected
    /// and deselected items; it also provides a <c>Cancel</c> property which can
    /// be used to cancel the operation.
    /// </summary>
    public class ItemPanelSelectionChangedEventArgs : Support.CancelEventArgs
    {
        public ItemPanelSelectionChangedEventArgs(
            IList<ItemView> selected,
            IList<ItemView> deselected
        )
        {
            this.selected = new ReadOnlyList<ItemView>(new List<ItemView>(selected));
            this.deselected = new ReadOnlyList<ItemView>(new List<ItemView>(deselected));
        }

        public ReadOnlyList<ItemView> SelectedItems
        {
            get { return this.selected; }
        }

        public ReadOnlyList<ItemView> DeselectedItems
        {
            get { return this.deselected; }
        }

        private ReadOnlyList<ItemView> selected;
        private ReadOnlyList<ItemView> deselected;
    }
}
