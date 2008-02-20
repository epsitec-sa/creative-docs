//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	public struct FolderQueryMode
	{
		public FileInfoIconSelection IconSelection
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

		public bool AsOpenFolder
		{
			get
			{
				return this.asOpenFolder;
			}
			set
			{
				this.asOpenFolder = value;
			}
		}

		public static FolderQueryMode LargeIcons
		{
			get
			{
				FolderQueryMode mode = new FolderQueryMode ();
				
				mode.IconSelection = FileInfoIconSelection.Normal;
				mode.IconSize = FileInfoIconSize.Large;
				
				return mode;
			}
		}

		public static FolderQueryMode SmallIcons
		{
			get
			{
				FolderQueryMode mode = new FolderQueryMode ();

				mode.IconSelection = FileInfoIconSelection.Normal;
				mode.IconSize = FileInfoIconSize.Small;

				return mode;
			}
		}

		public static FolderQueryMode NoIcons
		{
			get
			{
				FolderQueryMode mode = new FolderQueryMode ();

				mode.IconSelection = FileInfoIconSelection.Normal;
				mode.IconSize = FileInfoIconSize.None;

				return mode;
			}
		}

		public FolderQueryMode Open()
		{
			this.asOpenFolder = true;
			return this;
		}
		
		private FileInfoIconSize iconSize;
		private bool asOpenFolder;
		private FileInfoIconSelection iconSelection;
	}
}
