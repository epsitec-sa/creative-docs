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


using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly: ItemViewFactory(typeof(GroupItemViewFactory), ItemType = typeof(CollectionViewGroup))]

namespace Epsitec.Common.UI.ItemViewFactories
{
    internal sealed class GroupItemViewFactory : AbstractItemViewFactory
    {
        #region IItemViewFactory Members

        public override ItemViewWidget CreateUserInterface(ItemView itemView)
        {
            ItemPanelGroup group = itemView.Group;

            if (group == null)
            {
                group = new ItemPanelGroup(itemView);
            }

            group.PreferredWidth = itemView.Size.Width;

            return group;
        }

        public override void DisposeUserInterface(ItemViewWidget widget)
        {
            ItemPanelGroup group = widget as ItemPanelGroup;

            group.Dispose();
        }

        #endregion

        protected override Widgets.Widget CreateElement(
            string name,
            ItemPanel panel,
            ItemView view,
            ItemViewShape shape
        )
        {
            return null;
        }

        public override Drawing.Size GetPreferredSize(ItemView itemView)
        {
            ItemPanelGroup group = itemView.Group;

            if (group == null)
            {
                group = new ItemPanelGroup(itemView);
            }

            group.ChildPanel.PreferredLayoutWidth = itemView.Owner.PreferredLayoutWidth;

            return group.GetBestFitSize();
        }
    }
}
