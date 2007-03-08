//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.UI.ItemViewFactories;
using Epsitec.Common.Widgets;

[assembly: ItemViewFactory (typeof (FileListItemViewFactory), ItemType=typeof (FileListItem))]

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>FileItemViewFactory</c> class populates the ItemView with the
	/// visual representation of a <c>FileItem</c> instance.
	/// </summary>
	class FileListItemViewFactory : AbstractItemViewFactory
	{
		public static Widget FindFileNameWidget(ItemView itemView)
		{
			Widget itemLine = itemView == null ? null : itemView.Widget;
			Widget itemName = itemLine == null ? null : itemLine.FindChild ("name");

			return itemName;
		}
		
		protected override Widget CreateElement(string name, ItemPanel panel, ItemView view, ItemViewShape shape)
		{
			FileListItem item = view.Item as FileListItem;
			double iconSize;

			switch (shape)
			{
				case ItemViewShape.Tile:
				case ItemViewShape.ToolTip:
					iconSize = view.Size.Height-18-2-2;  // TODO: hauteur d'une ligne, faire mieux
					break;
				
				case ItemViewShape.Row:
				default:
					iconSize = view.Size.Height;
					break;
			}

			switch (name)
			{
				case "icon": return FileListItemViewFactory.CreateFileIcon (item, shape, iconSize);
				case "name": return FileListItemViewFactory.CreateFileName (item, shape);
				case "info": return FileListItemViewFactory.CreateFileInfo (item, shape);
				case "date": return FileListItemViewFactory.CreateFileDate (item, shape);
				case "size": return FileListItemViewFactory.CreateFileSize (item, shape);
			}

			return null;
		}

		private static Widget CreateFileIcon(FileListItem item, ItemViewShape shape, double size)
		{
			if (shape == ItemViewShape.ToolTip)
			{
				return null;
			}

			ImagePlaceholder fileIcon;

			fileIcon = new ImagePlaceholder ();
			fileIcon.Margins = new Margins (1, 1, 1, 1);

			string iconName = item.GetIconName (size < 32 ? FileInfoIconSize.Small : FileInfoIconSize.Large);

			if (string.IsNullOrEmpty (iconName))
			{
				Image bitmap;
				bool icon;
				item.AttachedImagePlaceholder = fileIcon;
				item.GetImage (out bitmap, out icon);
				fileIcon.Image = bitmap;
				fileIcon.PaintFrame = icon ? false : true;
				fileIcon.DisplayMode = icon ? ImageDisplayMode.Center : ImageDisplayMode.Stretch;
				fileIcon.IconName = null;
			}
			else
			{
				fileIcon.IconName = iconName;
				fileIcon.Image = null;
				fileIcon.PaintFrame = false;
			}

			fileIcon.PreferredHeight = size;

			return fileIcon;
		}

		private static Widget CreateFileName(FileListItem item, ItemViewShape shape)
		{
			StaticText fileName = new StaticText ();

			string text = item.ShortFileName;
			if (shape == ItemViewShape.ToolTip)
			{
				text = string.Concat(Common.Dialogs.Res.Strings.Dialog.File.Header.FileName, ": ", text);
			}
			else if (shape == ItemViewShape.Tile)
			{
				fileName.ContentAlignment = ContentAlignment.MiddleCenter;
			}
			else
			{
				fileName.Margins = new Margins (6, 0, 0, 0);
				fileName.ContentAlignment = ContentAlignment.MiddleLeft;
			}
			fileName.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileName.Text = text;
			fileName.PreferredSize = fileName.GetBestFitSize();
			
			return fileName;
		}

		private static Widget CreateFileInfo(FileListItem item, ItemViewShape shape)
		{
			switch (shape)
			{
				case ItemViewShape.Tile:
					return null;
				
				case ItemViewShape.ToolTip:
					if (string.IsNullOrEmpty (item.Description))
					{
						return null;
					}
					break;
			}

			StaticText fileInfo = new StaticText ();

			string text = item.Description;
			if (string.IsNullOrEmpty(text))
			{
				text = "";
			}
			if (shape == ItemViewShape.ToolTip)
			{
				//?text = text.Replace("<br/>", ", ");
			}
			else
			{
				fileInfo.Margins = new Margins (6, 6, 0, 0);
			}

			fileInfo.ContentAlignment = ContentAlignment.MiddleLeft;
			fileInfo.TextBreakMode = TextBreakMode.Hyphenate;
			fileInfo.Text = text;
			fileInfo.PreferredSize = fileInfo.GetBestFitSize();

			return fileInfo;
		}

		private static Widget CreateFileDate(FileListItem item, ItemViewShape shape)
		{
			switch (shape)
			{
				case ItemViewShape.Tile:
					return null;
				
				case ItemViewShape.ToolTip:
					if (string.IsNullOrEmpty (item.FileDate))
					{
						return null;
					}
					break;
			}
			
			StaticText fileDate = new StaticText ();

			string text = item.FileDate;
			if (shape == ItemViewShape.ToolTip)
			{
				text = string.Concat(Common.Dialogs.Res.Strings.Dialog.File.Header.Date, ": ", text);
			}
			else
			{
				fileDate.Margins = new Margins (6, 0, 0, 0);
			}

			fileDate.ContentAlignment = ContentAlignment.MiddleLeft;
			fileDate.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileDate.Text = text;
			fileDate.PreferredSize = fileDate.GetBestFitSize();

			return fileDate;
		}

		private static Widget CreateFileSize(FileListItem item, ItemViewShape shape)
		{
			switch (shape)
			{
				case ItemViewShape.Tile:
					return null;
				
				case ItemViewShape.ToolTip:
					if (string.IsNullOrEmpty (item.FileSize))
					{
						return null;
					}
					break;
			}

			StaticText fileSize = new StaticText ();

			string text = item.FileSize;
			if (shape == ItemViewShape.ToolTip)
			{
				text = string.Concat(Common.Dialogs.Res.Strings.Dialog.File.Header.Size, ": ", text);
			}
			else
			{
				fileSize.Margins = new Margins(0, 6, 0, 0);
			}

			fileSize.ContentAlignment = ContentAlignment.MiddleLeft;
			fileSize.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileSize.Text = text;
			fileSize.PreferredSize = fileSize.GetBestFitSize();

			return fileSize;
		}
	}
}
