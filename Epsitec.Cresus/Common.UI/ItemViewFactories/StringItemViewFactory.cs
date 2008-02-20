//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly: ItemViewFactory (typeof (StringItemViewFactory), ItemType=typeof (string))]
[assembly: ItemViewFactory (typeof (StringItemViewFactory), ItemType=typeof (object))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class StringItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public ItemViewWidget CreateUserInterface(ItemView itemView)
		{
			ItemViewWidget container = new ItemViewWidget (itemView);
			Widgets.StaticText text = new Widgets.StaticText (container);

			text.Text = itemView.Item.ToString ();
			text.Dock = Widgets.DockStyle.Fill;
			
			return container;
		}

		public void DisposeUserInterface(ItemViewWidget widget)
		{
			widget.Dispose ();
		}

		public Drawing.Size GetPreferredSize(ItemView itemView)
		{
			return itemView.Size;
		}

		#endregion
	}
}
