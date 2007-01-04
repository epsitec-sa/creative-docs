//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly:ItemViewFactory (typeof (GroupItemViewFactory), ItemType=typeof (CollectionViewGroup))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class GroupItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			ItemPanelGroup group = new ItemPanelGroup ();

			group.ParentPanel = panel;
			group.ParentView  = itemView;
			
			return group;
		}

		public void DisposeUserInterface(ItemView itemView, Widgets.Widget widget)
		{
			widget.Dispose ();
		}

		public Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			return itemView.Size;
		}

		#endregion
	}
}
