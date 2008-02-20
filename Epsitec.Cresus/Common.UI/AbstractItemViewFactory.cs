//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		/// <param name="itemView">The item view.</param>
		/// <returns>
		/// The widget which represents the data stored in the item view.
		/// </returns>
		public virtual ItemViewWidget CreateUserInterface(ItemView itemView)
		{
			return this.CreateElements (itemView);
		}

		/// <summary>
		/// Disposes the user interface created by <c>CreateUserInterface</c>.
		/// </summary>
		/// <param name="widget">The widget to dispose.</param>
		public virtual void DisposeUserInterface(ItemViewWidget widget)
		{
			widget.Dispose ();
		}

		/// <summary>
		/// Gets the preferred size of the user interface associated with the
		/// specified item view.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>The preferred size.</returns>
		public virtual Drawing.Size GetPreferredSize(ItemView itemView)
		{
			ItemPanel panel = itemView.Owner;
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
				switch (panel.GetPanelLayout ())
				{
					case ItemPanelLayout.VerticalList:
						dx = header.GetTotalWidth ();
						break;

					case ItemPanelLayout.RowsOfTiles:
					case ItemPanelLayout.ColumnsOfTiles:
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Layout {0} not supported", panel.GetPanelLayout ()));
				}

			}
			
			return new Drawing.Size (dx, dy);
		}

		/// <summary>
		/// Creates the elements used to represent the item.
		/// </summary>
		/// <param name="itemView">The item view.</param>
		/// <returns>The container which hosts all the elements.</returns>
		protected ItemViewWidget CreateElements(ItemView itemView)
		{
			ItemPanel panel = itemView.Owner;
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);

			System.Diagnostics.Debug.Assert (header != null);
			System.Diagnostics.Debug.Assert (header.ColumnCount > 0);

			ItemViewWidget viewContainer = new ItemViewWidget (itemView);
			viewContainer.PreferredSize = itemView.Size;

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

			if (shape == ItemViewShape.Tile)
			{
				viewContainer.Padding = new Drawing.Margins(2, 2, AbstractItemViewFactory.TileMargin, AbstractItemViewFactory.TileMargin);
			}

			for (int i = 0; i < header.ColumnCount; i++)
			{
				string name  = header.GetColumnPropertyName (i);
				double width = header.GetColumnWidth (i);

				Widgets.Widget element = this.CreateElement (name, panel, itemView, shape);

				if (element != null)
				{
					element.Name = name;
					element.Dock = Widgets.DockStyle.Stacked;
					element.PreferredWidth = width - element.Margins.Width;
					element.SetFrozen(true);
					viewContainer.Children.Add (element);
				}
			}

			Widgets.ToolTip.Contents tooltipContainer = new Widgets.ToolTip.Contents ();
			tooltipContainer.ContainerLayoutMode = Widgets.ContainerLayoutMode.VerticalFlow;
			tooltipContainer.Padding = new Epsitec.Common.Drawing.Margins(5, 5, 5, 5);

			for (int i = 0; i < header.ColumnCount; i++)
			{
				string name  = header.GetColumnPropertyName (i);
				double width = header.GetColumnWidth (i);

				Widgets.Widget element = this.CreateElement (name, panel, itemView, ItemViewShape.ToolTip);

				if (element != null)
				{
					element.Name = name;
					element.Dock = Widgets.DockStyle.Stacked;
					element.SetFrozen(true);
					tooltipContainer.Children.Add (element);
				}
			}

			Widgets.Layouts.LayoutMeasure widthMeasure;
			Widgets.Layouts.LayoutMeasure heightMeasure;
			tooltipContainer.GetMeasures (out widthMeasure, out heightMeasure);
			tooltipContainer.PreferredWidth  = widthMeasure.Desired;
			tooltipContainer.PreferredHeight = heightMeasure.Desired;

			if (tooltipContainer.HasChildren)
			{
				Widgets.ToolTip.Default.SetToolTip (viewContainer, tooltipContainer);
			}

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


		public static readonly double TileMargin = 6;
	}
}
