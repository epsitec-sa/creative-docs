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
			ItemTable             table  = ItemTable.GetItemTable (panel);

			if (header == null)
			{
				//	There is no header, just fall back to the simplest form of
				//	data representation :
				
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
					int columnId = header.GetColumnId (i);
					double width = header.GetColumnWidth (i);

					if ((columnId >= 0) &&
						(table != null) &&
						(columnId < table.Columns.Count))
					{
						Support.Druid templateId = table.Columns[columnId].TemplateId;

						if (templateId.IsEmpty)
						{
							StructuredItemViewFactory.CreateTextColumn (container, itemView, header, i, width);
						}
						else
						{
							StructuredItemViewFactory.CreateTemplateBasedColumn (container, itemView, templateId, header, i, width);
						}
					}
					else
					{
						StructuredItemViewFactory.CreateTextColumn (container, itemView, header, i, width);
					}
				}

				return container;
			}
		}

		private static void CreateTemplateBasedColumn(Widgets.Widget container, ItemView itemView, Support.Druid templateId, ItemPanelColumnHeader header, int index, double width)
		{
			Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (container);
			Panel panel = Panel.CreatePanel (templateId, manager);
			
			panel.SetEmbedder (container);
			
			panel.Dock           = Widgets.DockStyle.Stacked;
			panel.PreferredWidth = width - panel.Margins.Width;

			object source = itemView.Item;
			string path   = header.GetColumnPropertyName (index);
			
			DataObject.SetDataContext (panel, new Binding (source, path));
		}

		private static void CreateTextColumn(Widgets.Widget container, ItemView itemView, ItemPanelColumnHeader header, int index, double width)
		{
			Widgets.StaticText text = new Widgets.StaticText (container);

			text.Text           = header.GetColumnText (index, itemView.Item);
			text.Dock           = Widgets.DockStyle.Stacked;
			text.Margins        = new Drawing.Margins (3, 3, 0, 0);
			text.PreferredWidth = width - text.Margins.Width;
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
