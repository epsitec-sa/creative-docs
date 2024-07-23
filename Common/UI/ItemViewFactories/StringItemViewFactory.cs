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


using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly: ItemViewFactory(typeof(StringItemViewFactory), ItemType = typeof(string))]
[assembly: ItemViewFactory(typeof(StringItemViewFactory), ItemType = typeof(object))]

namespace Epsitec.Common.UI.ItemViewFactories
{
    internal sealed class StringItemViewFactory : IItemViewFactory
    {
        #region IItemViewFactory Members

        public ItemViewWidget CreateUserInterface(ItemView itemView)
        {
            ItemViewWidget container = new ItemViewWidget(itemView);
            Widgets.StaticText text = new Widgets.StaticText(container);

            text.Text = itemView.Item.ToString();
            text.Dock = Widgets.DockStyle.Fill;

            return container;
        }

        public void DisposeUserInterface(ItemViewWidget widget)
        {
            widget.Dispose();
        }

        public Drawing.Size GetPreferredSize(ItemView itemView)
        {
            return itemView.Size;
        }

        #endregion
    }
}
