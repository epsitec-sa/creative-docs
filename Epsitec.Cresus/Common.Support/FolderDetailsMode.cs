using System;
using System.Collections.Generic;
using System.Text;

namespace Epsitec.Common.Support
{
	public class FolderDetailsMode
	{
		public FolderDetailsMode()
		{
		}

		public FileInfoSelection IconSelection
		{
			get
			{
				return this.iconSelection;
			}
			set
			{
				this.iconSelection = value;
			}
		}

		public FileInfoIconSize IconSize
		{
			get
			{
				return this.iconSize;
			}
			set
			{
				this.iconSize = value;
			}
		}

		public static FolderDetailsMode LargeIcons
		{
			get
			{
				FolderDetailsMode mode = new FolderDetailsMode ();
				
				mode.IconSelection = FileInfoSelection.Normal;
				mode.IconSize = FileInfoIconSize.Large;
				
				return mode;
			}
		}

		public static FolderDetailsMode SmallIcons
		{
			get
			{
				FolderDetailsMode mode = new FolderDetailsMode ();

				mode.IconSelection = FileInfoSelection.Normal;
				mode.IconSize = FileInfoIconSize.Small;

				return mode;
			}
		}

		public static FolderDetailsMode NoIcons
		{
			get
			{
				FolderDetailsMode mode = new FolderDetailsMode ();

				mode.IconSelection = FileInfoSelection.Normal;
				mode.IconSize = FileInfoIconSize.None;

				return mode;
			}
		}
		
		private FileInfoIconSize iconSize;
		private FileInfoSelection iconSelection;
	}
}
