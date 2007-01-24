//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.App.DocumentEditor.Dialogs;
using Epsitec.Common.Document;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;
using Epsitec.Common.Widgets;

[assembly: ItemViewFactory (typeof (FileItemViewFactory), ItemType=typeof (FileItem))]

namespace Epsitec.App.DocumentEditor.Dialogs
{
	/// <summary>
	/// The <c>FileItemViewFactory</c> class populates the ItemView with the
	/// visual representation of a <c>FileItem</c> instance.
	/// </summary>
	class FileItemViewFactory : IItemViewFactory
	{
		#region IItemViewFactory Members

		public Epsitec.Common.Widgets.Widget CreateUserInterface(ItemPanel panel, ItemView itemView)
		{
			FileItem item = itemView.Item as FileItem;
			ItemPanel rootPanel = panel.RootPanel;
			ItemPanelColumnHeader header = ItemPanelColumnHeader.GetColumnHeader (rootPanel);

			System.Diagnostics.Debug.Assert (item != null);
			System.Diagnostics.Debug.Assert (header != null);
			System.Diagnostics.Debug.Assert (header.ColumnCount == 5);
			
			Widget container = new Widget ();

			container.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			ImagePlaceholder fileIcon = new ImagePlaceholder (container);
			StaticText fileName = new StaticText (container);
			StaticText fileInfo = new StaticText (container);
			StaticText fileDate = new StaticText (container);
			StaticText fileSize = new StaticText (container);

			fileIcon.Dock = DockStyle.Stacked;
			fileIcon.Margins = new Margins (1, 1, 1, 1);

			if (string.IsNullOrEmpty (item.FixIcon))
			{
				Image bitmap;
				bool icon;
				item.GetImage (out bitmap, out icon);
				fileIcon.DrawingImage = bitmap;
				fileIcon.PaintFrame = !icon;
				fileIcon.StretchImage = !icon;
				fileIcon.FixIcon = null;
			}
			else
			{
				fileIcon.FixIcon = Misc.Icon (item.FixIcon);
				fileIcon.DrawingImage = null;
				fileIcon.PaintFrame = false;
			}
			
			fileIcon.PreferredWidth = header.GetColumnWidth (0) - fileIcon.Margins.Width;

			fileName.Dock = DockStyle.Stacked;
			fileName.Margins = new Margins (6, 0, 0, 0);
			fileName.ContentAlignment = ContentAlignment.MiddleLeft;
			fileName.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileName.Text = item.ShortFileName;
			fileName.PreferredWidth = header.GetColumnWidth (1) - fileName.Margins.Width;

			fileInfo.Dock = DockStyle.Stacked;
			fileInfo.Margins = new Margins (6, 6, 0, 0);
			fileInfo.ContentAlignment = ContentAlignment.MiddleLeft;
			fileInfo.TextBreakMode = TextBreakMode.Hyphenate;
			fileInfo.Text = item.Description;
			fileInfo.PreferredWidth = header.GetColumnWidth (2) - fileInfo.Margins.Width;

			fileDate.Dock = DockStyle.Stacked;
			fileDate.Margins = new Margins (6, 0, 0, 0);
			fileDate.ContentAlignment = ContentAlignment.MiddleLeft;
			fileDate.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileDate.Text = item.FileDate;
			fileDate.PreferredWidth = header.GetColumnWidth (3) - fileDate.Margins.Width;

			fileSize.Dock = DockStyle.Stacked;
			fileSize.Margins = new Margins (0, 6, 0, 0);
			fileSize.ContentAlignment = ContentAlignment.MiddleRight;
			fileSize.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileSize.Text = item.FileSize;
			fileSize.PreferredWidth = header.GetColumnWidth (4) - fileSize.Margins.Width;
			
			return container;
		}

		public void DisposeUserInterface(ItemView itemView, Epsitec.Common.Widgets.Widget widget)
		{
			widget.Dispose ();
		}

		public Epsitec.Common.Drawing.Size GetPreferredSize(ItemPanel panel, ItemView itemView)
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
				return new Size (header.GetTotalWidth (), itemView.Size.Height);
			}
			else
			{
				//	TODO: better handling here... this only works for vertical layout!

				double dx = header.GetTotalWidth ();
				double dy = rootPanel.ItemViewDefaultSize.Height;

				return new Size (dx, dy);
			}
		}

		#endregion
	}
}
