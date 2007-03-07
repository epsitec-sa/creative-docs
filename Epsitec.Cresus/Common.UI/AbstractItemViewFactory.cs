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

		public abstract Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView);
		public abstract void DisposeUserInterface(ItemView itemView, Widgets.Widget widget);

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

			double dx = itemView.Size.Width;
			double dy = itemView.Size.Height;
			
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
	}
}
