//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;

using System.Collections.Generic;

[assembly: ItemViewFactory (typeof (StructuredItemViewFactory), ItemType=typeof (StructuredData))]

namespace Epsitec.Common.UI.ItemViewFactories
{
	/// <summary>
	/// The <c>StructuredItemViewFactory</c> class is used to generate widgets
	/// for item views which represent structured data.
	/// </summary>
	internal sealed class StructuredItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		/// <summary>
		/// Creates the user interface for the specified item view.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>
		/// The widget which represents the data stored in the item view.
		/// </returns>
		public ItemViewWidget CreateUserInterface(ItemView itemView)
		{
			ItemPanel panel = itemView.Owner;
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);
			ItemTable             table  = ItemTable.GetItemTable (rootPanel);

			ItemViewWidget container = new ItemViewWidget (itemView);
			
			container.ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow;

			if (header == null)
			{
				//	There is no header, just fall back to the simplest form of
				//	data representation :
				
				Widgets.StaticText text = new Widgets.StaticText (container);
				text.Text = itemView.Item.ToString ();
				text.Dock = Widgets.DockStyle.Fill;
			}
			else
			{
				int count = header.ColumnCount;

				for (int i = 0; i < count; i++)
				{
					int columnId = header.GetColumnId (i);
					double width = header.GetColumnWidth (i);

					if ((columnId >= 0) &&
						(table != null) &&
						(columnId < table.Columns.Count))
					{
						//	The table has a column definition for the specified
						//	column :
						
						Support.Druid templateId = table.Columns[columnId].TemplateId;

						if (templateId.IsEmpty)
						{
							//	The column has no template ID associated; simply use
							//	a text representation for the item view data.

							this.CreateTextColumn (container, itemView, header, i, width);
						}
						else
						{
							//	The column has a template ID associated with it; create
							//	the panel defined by the template ID and bind it to the
							//	item view data.
							
							this.CreateTemplateBasedColumn (container, itemView, header, i, templateId, width);
						}
					}
					else
					{
						throw new System.InvalidOperationException ("No column definition found");
					}
				}
			}

			return container;
		}

		/// <summary>
		/// Disposes the user interface created by <c>CreateUserInterface</c>.
		/// </summary>
		/// <param name="widget">The widget to dispose.</param>
		public void DisposeUserInterface(ItemViewWidget widget)
		{
			if (widget.Children.Count == 1)
			{
				//	If the user interface is based on a template (panel created
				//	based on the column's template ID), then we will recycle the
				//	panel rather than simply discard it.
				
				Panel panel = widget.Children[0] as Panel;
				
				if (panel != null)
				{
					Support.Druid panelId = Panel.GetBundleId (panel);

					System.Diagnostics.Debug.Assert (panelId.IsValid);
					
					lock (this.exclusion)
					{
						this.cache[panelId].RecyclePanel (panel);
					}
				}
			}

			widget.Dispose ();
		}

		/// <summary>
		/// Gets the preferred size of the user interface associated with the
		/// specified item view.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>The preferred size.</returns>
		public Drawing.Size GetPreferredSize(ItemView itemView)
		{
			ItemPanel panel = itemView.Owner;
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
				//	TODO: better handling here... this only works for vertical layout!

				double dx = header.GetTotalWidth ();
				double dy = table.GetDefaultItemSize (itemView).Height;
				
				return new Drawing.Size (dx, dy);
			}
		}

		#endregion
		
		private void CreateTemplateBasedColumn(Widgets.Widget container, ItemView itemView, ItemPanelColumnHeader header, int index, Support.Druid templateId, double width)
		{
			Support.ResourceManager manager = Widgets.Helpers.VisualTree.FindResourceManager (container);
			Panel panel;

			//	Gets a recycled panel from the cache (or create a new panel if
			//	none can be found).
			
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

			panel.Dock = Widgets.DockStyle.Stacked;
			panel.PreferredWidth = width - panel.Margins.Width;

			//	Bind the panel contents with the item view data.
			
			object source = itemView.Item;
			string path   = header.GetColumnPropertyName (index);

			DataObject.SetDataContext (panel, new Binding (source, path));

			//	Finally, insert the panel into the container. We may not do this
			//	before, since the panel might inherit from the container's own
			//	data context, which might be incompatible with what the panel
			//	expects.
			
			panel.SetEmbedder (container);
		}

		private void CreateTextColumn(Widgets.Widget container, ItemView itemView, ItemPanelColumnHeader header, int index, double width)
		{
			Widgets.StaticText text = new Widgets.StaticText (container);

			text.Text           = header.GetColumnText (index, itemView.Item);
			text.Dock           = Widgets.DockStyle.Stacked;
			text.Margins        = new Drawing.Margins (3, 3, 0, 0);
			text.PreferredWidth = width - text.Margins.Width;
		}

		#region Cache Structure

		private struct Cache
		{
			public Cache(Support.Druid panelId)
			{
				this.panelId = panelId;
				this.panels = new List<Panel> ();
			}

			public void RecyclePanel(Panel panel)
			{
				//	Remember the panel for some future use. We detach it from 
				//	its parent and from its data context in order to avoid that
				//	it refreshes itself while sitting unused in the cache.

				panel.SetParent (null);
				DataObject.ClearDataContext (panel);
				
				this.panels.Add (panel);
			}

			public Panel GetPanel(Support.ResourceManager manager)
			{
				//	Find the panel with our panel ID for the specified resource
				//	manager.
				
				foreach (Panel panel in this.panels)
				{
					if (panel.ResourceManager == manager)
					{
						this.panels.Remove (panel);
						return panel;
					}
				}

				//	We could not find the panel in the cache. Create it based
				//	on the template ID.
				
				Panel newPanel = Panel.CreatePanel (this.panelId, manager);

				System.Diagnostics.Debug.Assert (Panel.GetBundleId (newPanel) == this.panelId);
				
				//	Force the resource manager association; we will use it when
				//	the panel gets put back into the cache (recycled).
				
				newPanel.ResourceManager = manager;
				
				return newPanel;
			}

			private Support.Druid panelId;
			private List<Panel> panels;
		}

		#endregion

		private readonly object exclusion = new object ();
		private Dictionary<Support.Druid, Cache> cache = new Dictionary<Support.Druid, Cache> ();
	}
}
