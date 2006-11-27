//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

[assembly: ItemViewFactory (typeof (StructuredItemViewFactory), ItemType=typeof (StructuredData))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class StructuredItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (panel);

			if (header == null)
			{
				Widgets.StaticText text = new Widgets.StaticText ();
				text.Text = itemView.Item.ToString ();
				return text;
			}
			else
			{
				Widgets.Widget container = new Widgets.Widget ();
				int count = header.ColumnCount;

				container.ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow;

				for (int i = 0; i < count; i++)
				{
					Widgets.StaticText text = new Widgets.StaticText (container);

					text.Text = header.GetColumnText (i, itemView.Item);
					text.Dock = Widgets.DockStyle.Stacked;
					text.PreferredWidth = header.GetColumnWidth (i) - 6;
					text.Margins = new Drawing.Margins (3, 3, 0, 0);
				}

				return container;
			}
		}

		public Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (panel);

			if (header == null)
			{
				return itemView.Size;
			}
			else
			{
				return new Drawing.Size (header.GetTotalWidth (), itemView.Size.Height);
			}
		}

		#endregion
	}
}
