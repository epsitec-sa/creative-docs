//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support.Platform
{
	public static class FileInfo
	{
		public static FolderItem CreateFolderItem(FolderId file, FolderQueryMode mode)
		{
			return Win32.FileInfo.CreateFolderItem (file, mode);
		}

		public static FolderItem CreateFolderItem(string path, FolderQueryMode mode)
		{
			return Win32.FileInfo.CreateFolderItem (path, mode);
		}

		internal static FolderItem CreateFolderItem(FolderItemHandle handle, FolderQueryMode mode)
		{
			return Win32.FileInfo.CreateFolderItem (handle, mode);
		}

		public static IEnumerable<FolderItem> GetFolderItems(FolderItem path, FolderQueryMode mode)
		{
			return Win32.FileInfo.GetFolderItems (path, mode);
		}

		public static FolderItem GetParentFolderItem(FolderItem path, FolderQueryMode mode)
		{
			return Win32.FileInfo.GetParentFolderItem (path, mode);
		}
	}
}
