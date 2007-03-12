//	Copyright © 2006-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly:ItemViewFactory (typeof (GroupItemViewFactory), ItemType=typeof (CollectionViewGroup))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class GroupItemViewFactory : AbstractItemViewFactory
	{
		#region IItemViewFactory Members

		public override Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			ItemPanelGroup group = new ItemPanelGroup ();

			group.ParentPanel = panel;
			group.ParentView  = itemView;

			group.PreferredWidth = itemView.Size.Width;
			
			return group;
		}

		public override void DisposeUserInterface(ItemView itemView, Widgets.Widget widget)
		{
			widget.Dispose ();
		}

		#endregion
		
		protected override Widgets.Widget CreateElement(string name, ItemPanel panel, ItemView view, ItemViewShape shape)
		{
			return null;
		}

		public override Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			Drawing.Size size = base.GetPreferredSize (panel, itemView);

			size.Height = itemView.Size.Height;

			return size;
		}
	}
}
