//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly:ItemViewFactory (typeof (StringItemViewFactory), ItemType=typeof (string))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class StringItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			Widgets.StaticText text = new Widgets.StaticText ();

			text.Text = itemView.Item.ToString ();
			
			return text;
		}

		public Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			return itemView.Size;
		}

		#endregion
	}
}
