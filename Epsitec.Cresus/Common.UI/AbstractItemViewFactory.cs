//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.UI
{
	/// <summary>
	/// The <c>AbstractItemViewFactory</c> class provides the basic plumbing
	/// required to implement the <see cref="IItemViewFactory"/> interface.
	/// </summary>
	public abstract class AbstractItemViewFactory : IItemViewFactory
	{
		protected AbstractItemViewFactory()
		{
		}

		/// <summary>
		/// Creates the user interface for the specified item view.
		/// </summary>
		/// <param name="panel">The panel.</param>
		/// <param name="itemView">The item view.</param>
		/// <returns>
		/// The widget which represents the data stored in the item view.
		/// </returns>
		public virtual Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			return this.CreateElements (panel, itemView);
		}

		/// <summary>
		/// Disposes the user interface created by <c>CreateUserInterface</c>.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <param name="widget">The widget to dispose.</param>
		public virtual void DisposeUserInterface(ItemView itemView, Widgets.Widget widget)
		{
			widget.Dispose ();
		}

		/// <summary>
		/// Gets the preferred size of the user interface associated with the
		/// specified item view.
		/// </summary>
		/// <param name="panel">The panel.</param>
		/// <param name="itemView">The item view.</param>
		/// <returns>The preferred size.</returns>
		public virtual Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
		{
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);
			ItemTable rootTable = ItemTable.GetItemTable (rootPanel);

			double dx;
			double dy;

#if false
			if (rootTable != null)
			{
				Drawing.Size size = rootTable.GetDefaultItemSize (itemView);
				
				dx = size.Width;
				dy = size.Height;
			}
			else
#endif
			{
				Drawing.Size size = rootPanel.ItemViewDefaultSize;
				
				dx = size.Width;
				dy = size.Height;
			}
			
			if (header != null)
			{
				switch (panel.Layout)
				{
					case ItemPanelLayout.VerticalList:
						dx = header.GetTotalWidth ();
						break;

					case ItemPanelLayout.RowsOfTiles:
					case ItemPanelLayout.ColumnsOfTiles:
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Layout {0} not supported", panel.Layout));
				}

			}
			
			return new Drawing.Size (dx, dy);
		}

		/// <summary>
		/// Creates the elements used to represent the item.
		/// </summary>
		/// <param name="panel">The panel.</param>
		/// <param name="view">The item view.</param>
		/// <returns>The container which hosts all the elements.</returns>
		protected Widgets.Widget CreateElements(ItemPanel panel, ItemView view)
		{
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);

			System.Diagnostics.Debug.Assert (header != null);
			System.Diagnostics.Debug.Assert (header.ColumnCount > 0);

			Widgets.Widget viewContainer = new Widgets.Widget ();
			viewContainer.PreferredSize = view.Size;
			ItemViewShape shape = panel.ItemViewShape;

			switch (shape)
			{
				case ItemViewShape.Row:
					viewContainer.ContainerLayoutMode = Widgets.ContainerLayoutMode.HorizontalFlow;
					break;

				case ItemViewShape.Tile:
					viewContainer.ContainerLayoutMode = Widgets.ContainerLayoutMode.VerticalFlow;
					break;

				default:
					throw new System.NotSupportedException (string.Format ("ItemViewShape {0} not supported", panel.ItemViewShape));
			}

			for (int i = 0; i < header.ColumnCount; i++)
			{
				string name  = header.GetColumnPropertyName (i);
				double width = header.GetColumnWidth (i);

				Widgets.Widget element = this.CreateElement (name, panel, view, shape);

				if (element != null)
				{
					element.Name = name;
					element.Dock = Widgets.DockStyle.Stacked;
					element.PreferredWidth = width - element.Margins.Width;
					element.SetFrozen(true);
					viewContainer.Children.Add (element);
				}
			}

			Widgets.Widget tooltipContainer = new Widgets.Widget ();
			tooltipContainer.ContainerLayoutMode = Widgets.ContainerLayoutMode.VerticalFlow;

			for (int i = 0; i < header.ColumnCount; i++)
			{
				string name  = header.GetColumnPropertyName (i);
				double width = header.GetColumnWidth (i);

				Widgets.Widget element = this.CreateElement (name, panel, view, ItemViewShape.ToolTip);

				if (element != null)
				{
					element.Name = name;
					element.Dock = Widgets.DockStyle.Stacked;
					element.PreferredWidth = width - element.Margins.Width;
					element.SetFrozen(true);
					tooltipContainer.Children.Add (element);
				}
			}

			tooltipContainer.PreferredSize = new Epsitec.Common.Drawing.Size(150, 100);
			Widgets.ToolTip.Default.SetToolTip(viewContainer, tooltipContainer);

			return viewContainer;
		}

		/// <summary>
		/// Creates a single element, based on a property name.
		/// </summary>
		/// <param name="name">The property name.</param>
		/// <param name="panel">The panel.</param>
		/// <param name="view">The item view.</param>
		/// <param name="shape">The shape of the container.</param>
		/// <returns>
		/// The widget which represents the named property.
		/// </returns>
		protected abstract Widgets.Widget CreateElement(string name, ItemPanel panel, ItemView view, ItemViewShape shape);
	}
}
