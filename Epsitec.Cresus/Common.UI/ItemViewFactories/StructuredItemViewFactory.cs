//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

using System.Collections.Generic;

[assembly: ItemViewFactory (typeof (StructuredItemViewFactory), ItemType=typeof (StructuredData))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	internal sealed class StructuredItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);
			ItemTable             table  = ItemTable.GetItemTable (rootPanel);

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
							this.CreateTemplateBasedColumn (container, itemView, templateId, header, i, width);
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

		public void DisposeUserInterface(ItemView itemView, Widgets.Widget widget)
		{
			if (widget.Children.Count == 1)
			{
				Panel panel = widget.Children[0] as Panel;
				
				if (panel != null)
				{
					Support.Druid panelId = Panel.GetBundleId (panel);

					System.Diagnostics.Debug.Assert (panelId.IsValid);
					
					lock (this.exclusion)
					{
						this.cache[panelId].Release (panel);
					}
				}
			}

			widget.Dispose ();
		}

		public Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);
			ItemTable table = ItemTable.GetItemTable (rootPanel);

			if (header == null)
			{
				return itemView.Size;
			}
			else if (table == null)
			{
				return new Drawing.Size (header.GetTotalWidth (), itemView.Size.Height);
			}
			else
			{
				return new Drawing.Size (header.GetTotalWidth (), table.GetDefaultItemSize (itemView).Height);
			}
		}

		#endregion
		
		private void CreateTemplateBasedColumn(Widgets.Widget container, ItemView itemView, Support.Druid templateId, ItemPanelColumnHeader header, int index, double width)
		{
			Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (container);
			Panel panel;

			lock (this.exclusion)
			{
				Cache templateCache;

				if (this.cache.TryGetValue (templateId, out templateCache) == false)
				{
					templateCache = new Cache (templateId);
					this.cache[templateId] = templateCache;
				}
				
				panel = templateCache.GetPanel (manager);
			}

			panel.Dock           = Widgets.DockStyle.Stacked;
			panel.PreferredWidth = width - panel.Margins.Width;

			object source = itemView.Item;
			string path   = header.GetColumnPropertyName (index);

			DataObject.SetDataContext (panel, new Binding (source, path));

			panel.SetEmbedder (container);
		}

		private static void CreateTextColumn(Widgets.Widget container, ItemView itemView, ItemPanelColumnHeader header, int index, double width)
		{
			Widgets.StaticText text = new Widgets.StaticText (container);

			text.Text           = header.GetColumnText (index, itemView.Item);
			text.Dock           = Widgets.DockStyle.Stacked;
			text.Margins        = new Drawing.Margins (3, 3, 0, 0);
			text.PreferredWidth = width - text.Margins.Width;
		}

		private struct Cache
		{
			public Cache(Support.Druid panelId)
			{
				this.panelId = panelId;
				this.panels = new List<Panel> ();
			}

			public void Release(Panel panel)
			{
				panel.SetParent (null);
				DataObject.ClearDataContext (panel);
				this.panels.Add (panel);
			}

			public Panel GetPanel(Support.ResourceManager manager)
			{
				foreach (Panel panel in this.panels)
				{
					if (panel.ResourceManager == manager)
					{
						this.panels.Remove (panel);
						return panel;
					}
				}

				Panel newPanel = Panel.CreatePanel (this.panelId, manager);
				newPanel.ResourceManager = manager;

				System.Diagnostics.Debug.Assert (Panel.GetBundleId (newPanel) == this.panelId);
				
				return newPanel;
			}

			private Support.Druid panelId;
			private List<Panel> panels;
		}

		private object exclusion = new object ();
		private Dictionary<Support.Druid, Cache> cache = new Dictionary<Support.Druid, Cache> ();
	}
}
