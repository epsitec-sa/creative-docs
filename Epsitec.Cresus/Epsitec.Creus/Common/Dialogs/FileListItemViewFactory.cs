//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
					iconSize = view.Size.Height;
					iconSize -= StaticText.DefaultFontHeight*2;
					iconSize -= AbstractItemViewFactory.TileMargin*2;
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

			string iconUri = item.GetIconUri (size < 32 ? FileInfoIconSize.Small : FileInfoIconSize.Large);

			if (string.IsNullOrEmpty (iconUri))
			{
				Image bitmap;
				bool icon;
				item.AttachedImagePlaceholder = fileIcon;
				item.GetImage (out bitmap, out icon);
				fileIcon.Image = bitmap;
				fileIcon.PaintFrame = icon ? false : true;
				fileIcon.DisplayMode = icon ? ImageDisplayMode.Center : ImageDisplayMode.Stretch;
				fileIcon.IconUri = null;
			}
			else
			{
				fileIcon.IconUri = iconUri;
				fileIcon.Image = null;
				fileIcon.PaintFrame = false;
			}

			fileIcon.PreferredHeight = size;

			if (size > 64 && shape == ItemViewShape.Tile)  // très grande icône ?
			{
				Widgets.FrameBox frame = new FrameBox();  // dessine un cadre autour
				frame.DrawFullFrame = true;
				frame.PreferredHeight = size;
				frame.Margins = new Margins (1, 1, 1, 1);

				fileIcon.Margins = new Margins (0, 0, 0, 0);
				fileIcon.Dock = DockStyle.Fill;
				fileIcon.SetParent(frame);
				return frame;
			}
			else
			{
				return fileIcon;
			}
		}

		private static Widget CreateFileName(FileListItem item, ItemViewShape shape)
		{
			StaticText fileName = new StaticText ();

			string text = Widgets.TextLayout.ConvertToTaggedText(item.ShortFileName);
			if (shape == ItemViewShape.ToolTip)
			{
				text = string.Concat(Common.Dialogs.Res.Strings.Dialog.File.Header.FileName, ": <b>", text, "</b>");
				fileName.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				fileName.Text = text;
				fileName.PreferredSize = fileName.GetBestFitSize();
			}
			else if (shape == ItemViewShape.Tile)
			{
				fileName.ContentAlignment = ContentAlignment.TopCenter;
				fileName.TextBreakMode = TextBreakMode.Split;
				fileName.Text = text;
				fileName.PreferredHeight = StaticText.DefaultFontHeight*2+2;  // place pour deux lignes
			}
			else
			{
				fileName.Margins = new Margins (6, 0, 0, 0);
				fileName.ContentAlignment = ContentAlignment.MiddleLeft;
				fileName.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				fileName.Text = text;
				fileName.PreferredSize = fileName.GetBestFitSize();
			}
			
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
			if (text == null)
			{
				text = "";
			}

			if (shape != ItemViewShape.ToolTip)
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

			string text = Widgets.TextLayout.ConvertToTaggedText(item.FileDate);
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

			string text = Widgets.TextLayout.ConvertToTaggedText(item.FileSize);
			if (shape == ItemViewShape.ToolTip)
			{
				text = string.Concat(Common.Dialogs.Res.Strings.Dialog.File.Header.Size, ": ", text);
				fileSize.ContentAlignment = ContentAlignment.MiddleLeft;
			}
			else
			{
				fileSize.Margins = new Margins(0, 6, 0, 0);
				fileSize.ContentAlignment = ContentAlignment.MiddleRight;
			}

			fileSize.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			fileSize.Text = text;
			fileSize.PreferredSize = fileSize.GetBestFitSize();

			return fileSize;
		}
	}
}
